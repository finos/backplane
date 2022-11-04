/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.Client.Middleware;
using Finos.Fdc3.Backplane.Client.Transport;
using Finos.Fdc3.Backplane.DTO.FDC3;
using Finos.Fdc3.Backplane.DTO.FDC3.Exceptions.Channel;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Disposables;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Finos.Fdc3.Backplane.Client.Test")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
namespace Finos.Fdc3.Backplane.Client.API
{
    /// <summary>
    /// Desktop Agent Implementation.
    /// </summary>
    internal class BackplaneClient : IBackplaneClient
    {
        private readonly ILogger<BackplaneClient> _logger;
        private readonly List<IChannel> _systemChannels;
        private readonly CompositeDisposable _disposables;
        private readonly Subject<Notification> _notificationsStream;
        private readonly ConcurrentDictionary<string, ContextTypeAndHandler> _contextHandlers;
        private readonly SemaphoreSlim _threadSafeChannelAccessHandle = new SemaphoreSlim(1, 1);
        private readonly SemaphoreSlim _threadSafeInitializationHandle = new SemaphoreSlim(1, 1);
        private readonly IClientMiddleware _clientMiddleware;
        private bool _isChannelChanging = false;
        private bool _isInitialized = false;
        private IChannel _currentChannel;


        public BackplaneClient(ILogger<BackplaneClient> logger,
           IClientMiddleware clientMiddleware)
        {
            _logger = logger;
            _systemChannels = new List<IChannel>();
            _contextHandlers = new ConcurrentDictionary<string, ContextTypeAndHandler>();
            _notificationsStream = new Subject<Notification>();
            _clientMiddleware = clientMiddleware;
            _disposables = new CompositeDisposable
            {
                _clientMiddleware,
                _threadSafeChannelAccessHandle,
                _threadSafeInitializationHandle,
                _notificationsStream
            };

        }

        public async Task InitializeAsync(AppIdentifier appMetadata, int retryCount = 1,
                             Func<int, TimeSpan> retryIntervalProvider = null,
                             CancellationToken ct = default)
        {
            if (_isInitialized)
            {
                return;
            }
            await _threadSafeInitializationHandle.WaitAsync(ct).ConfigureAwait(false);
            try
            {

                if (_isInitialized)
                {
                    return;
                }
                _logger.LogInformation($"Initializing client...");
                if (retryIntervalProvider == null)
                {
                    retryIntervalProvider = (t) => TimeSpan.FromSeconds(1);
                    _logger.LogInformation($"Set retry interval to default value of 1 second");
                }
                await _clientMiddleware.InitializeAsync(appMetadata, retryCount, retryIntervalProvider, ct).ConfigureAwait(false);
                IEnumerable<IChannel> systemChannels = await _clientMiddleware.GetSystemChannelsAsync(ct).ConfigureAwait(false);
                _systemChannels.AddRange(systemChannels);
                _logger.LogInformation($"Populated system channels:{string.Join(", ", _systemChannels.Select(x => x?.ToString()))}");
                _isInitialized = true;
                _logger.LogInformation($"Initialized da client successfully!");
            }
            finally
            {
                _threadSafeInitializationHandle.Release();
            }
        }



        public async Task<IListener> AddContextListenerAsync(string contextType, Action<IContext> handler, CancellationToken ct = default)
        {
            await EnsureInitialized(CancellationToken.None);
            IListener listener = null;
            await _threadSafeChannelAccessHandle.WaitAsync().ConfigureAwait(false);
            try
            {

                if (_currentChannel != null)
                {
                    listener = await _currentChannel.AddContextListenerAsync(contextType, handler);
                }
            }
            finally
            {
                _threadSafeChannelAccessHandle.Release();
            }

            string contextHandlerId = Guid.NewGuid().ToString();
            _contextHandlers.TryAdd(contextHandlerId, new ContextTypeAndHandler() { ContextType = contextType, Handler = handler, Listener = listener });
            return new Listener(async (token) =>
            {
                if (_contextHandlers.TryRemove(contextHandlerId, out ContextTypeAndHandler contextListenerHandler))
                {
                    await contextListenerHandler.Listener.UnsubscribeAsync(token);
                }
            });
        }

        public async Task<IEnumerable<IChannel>> GetSystemChannelsAsync(CancellationToken ct = default)
        {
            await EnsureInitialized(ct).ConfigureAwait(false);
            return _systemChannels;
        }


        public async Task JoinChannelAsync(string id, CancellationToken ct = default)
        {
            await EnsureInitialized(ct).ConfigureAwait(false);
            await _threadSafeChannelAccessHandle.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                // don't do anything if you are trying to join the same channel
                if (_currentChannel?.Id == id)
                {
                    return;
                }

                if (_isChannelChanging)
                {
                    throw new ChannelAccessDeniedException($"Currently in process of changing channels. Rejecting this request: ${id} ");
                }
                IChannel channel = _systemChannels.FirstOrDefault(x => x.Id == id);
                if (channel == null)
                {
                    throw new NoChannelFoundException($"Channel id:{id} not found");
                }
                if (_currentChannel != null)
                {
                    _isChannelChanging = true;
                    LeaveCurrentChannel();
                }
                _currentChannel = channel;
                //re-subscribe all registered handlers to newly joined channel.
                List<string> keys = _contextHandlers.Keys.ToList();
                keys.ForEach(async (x) =>
                {
                    if (_contextHandlers.TryGetValue(x, out ContextTypeAndHandler contextTypeListenerAndHandler))
                    {
                        IListener listener = await _currentChannel.AddContextListenerAsync(contextTypeListenerAndHandler.ContextType, contextTypeListenerAndHandler.Handler);
                        contextTypeListenerAndHandler.Listener = listener;
                    }
                });
                _logger.LogInformation($"Joined channel: {channel.Id}");
            }
            finally
            {
                _isChannelChanging = false;
                _threadSafeChannelAccessHandle.Release();
            }
        }


        public async Task LeaveCurrentChannelAsync(CancellationToken ct = default)
        {
            await EnsureInitialized(ct).ConfigureAwait(false);
            await _threadSafeChannelAccessHandle.WaitAsync(ct).ConfigureAwait(false);
            try
            {

                LeaveCurrentChannel();
            }
            finally
            {
                _threadSafeChannelAccessHandle.Release();
            }
        }

        private void LeaveCurrentChannel()
        {
            if (_currentChannel != null)
            {
                _currentChannel = null;
                // unsubscribe to everything that was already subscribed
                List<string> keys = _contextHandlers.Keys.ToList();
                keys.ForEach(x =>
                {
                    if (_contextHandlers.TryGetValue(x, out ContextTypeAndHandler contextListenerHandler))
                    {
                        contextListenerHandler.Listener.UnsubscribeAsync();
                    }
                });
            }
            _logger.LogInformation($"Leave channel called.No channel joined now.");
        }


        public async Task<IChannel> GetCurrentChannelAsync(CancellationToken ct = default)
        {
            await EnsureInitialized(ct).ConfigureAwait(false);
            await _threadSafeChannelAccessHandle.WaitAsync(ct).ConfigureAwait(false);
            try
            {

                return _currentChannel;
            }
            finally
            {
                _threadSafeChannelAccessHandle.Release();
            }

        }

        public async Task<IChannel> GetOrCreateChannelAsync(string channelId, CancellationToken ct = default)
        {
            await _threadSafeChannelAccessHandle.WaitAsync(ct).ConfigureAwait(false);
            try
            {

                IChannel existingChannel = _systemChannels.FirstOrDefault(x => x.Id == channelId);
                if (existingChannel != null)
                {
                    return existingChannel;
                }

                throw new ChannelCreationFailedException("Creation of new channels not supported!. Ensure to pass existing channel id");
            }
            finally
            {
                _threadSafeChannelAccessHandle.Release();
            }
        }

        public IObservable<ConnectionState> ObserveClientConnectionState()
        {
            return _clientMiddleware.ConnectionStateStream;
        }

        public IObservable<Notification> ObserveClientNotifications()
        {
            return _notificationsStream;
        }


        public void Dispose()
        {
            _disposables.Dispose();
            _contextHandlers.Clear();
            _systemChannels.Clear();
        }

        private async Task EnsureInitialized(CancellationToken ct)
        {
            await _threadSafeInitializationHandle.WaitAsync(ct).ConfigureAwait(false);
            try
            {
                if (_isInitialized == false)
                {
                    throw new InvalidOperationException("Desktop Agent Client not initialized!. Operations can be performed post successfull initialization only.");
                }
            }
            finally
            {
                _threadSafeInitializationHandle.Release();
            }

        }

        public async Task BroadcastAsync(IContext context, CancellationToken ct = default)
        {
            await EnsureInitialized(CancellationToken.None).ConfigureAwait(false);
            await _threadSafeChannelAccessHandle.WaitAsync(ct).ConfigureAwait(false);
            try
            {

                if (_currentChannel == null)
                {
                    string errorMessage = "No channel joined!.Broadcast has no effect";
                    _logger.LogError(errorMessage);
                    _notificationsStream.OnNext(new Notification(NotificationType.Error, errorMessage));
                    return;
                }
                await _currentChannel.BroadcastAsync(context, ct).ConfigureAwait(false);
                _notificationsStream.OnNext(new Notification(NotificationType.Info, $"Broadcast context successful."));
            }
            catch (Exception ex)
            {
                _logger.LogError($"An error occured in broadcast context: {JsonConvert.SerializeObject(context)}.{ex}");
                _notificationsStream.OnNext(new Notification(NotificationType.Error, $"Failed to broadcast context.Error message: {ex.Message}"));
            }
            finally
            {
                _threadSafeChannelAccessHandle.Release();
            }
        }

    }
    internal class ContextTypeAndHandler
    {
        public string ContextType { get; set; }
        public Action<IContext> Handler { get; set; }
        public IListener Listener { get; set; }
    }
}
