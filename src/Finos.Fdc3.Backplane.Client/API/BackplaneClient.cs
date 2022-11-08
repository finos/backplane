/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.Client.Transport;
using Finos.Fdc3.Backplane.DTO.Envelope;
using Finos.Fdc3.Backplane.DTO.FDC3;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.API
{
    /// <summary>
    /// Desktop Agent Implementation.
    /// </summary>
    internal class BackplaneClient : IBackplaneClient
    {
        private readonly ILogger<BackplaneClient> _logger;
        private readonly List<Channel> _systemChannels;
        private readonly IBackplaneTransport _backplaneTransport;
        private readonly SemaphoreSlim _threadSafeInitializationHandle = new SemaphoreSlim(1, 1);
        private bool _isInitialized = false;
        private AppIdentifier _appIdentifier;

        public BackplaneClient(ILogger<BackplaneClient> logger, IBackplaneTransport backplaneTransport)
        {
            _logger = logger;
            _systemChannels = new List<Channel>();
            _backplaneTransport = backplaneTransport;
        }

        public async Task<AppIdentifier> InitializeAsync(InitializeParams initializeParams, Action<MessageEnvelope> onMessage, Func<Exception, Task> onDisconnect, CancellationToken ct = default)
        {
            if (_isInitialized)
            {
                return _appIdentifier;
            }
            await _threadSafeInitializationHandle.WaitAsync(ct).ConfigureAwait(false);
            try
            {

                if (_isInitialized)
                {
                    return _appIdentifier;
                }

                _logger.LogInformation($"Initializing client...");
                await _backplaneTransport.ConnectAsync(initializeParams.Url, onMessage, onDisconnect, ct).ConfigureAwait(false);
                IEnumerable<Channel> systemChannels = await _backplaneTransport.GetSystemChannelsAsync(ct).ConfigureAwait(false);
                _systemChannels.AddRange(systemChannels);
                _logger.LogInformation($"Populated system channels:{string.Join(", ", _systemChannels.Select(x => x?.ToString()))}");
                _isInitialized = true;
                _logger.LogInformation($"Initialized the backplane client successfully!");
                _appIdentifier = initializeParams.AppIdentifier;
                return _appIdentifier;
            }
            finally
            {
                _threadSafeInitializationHandle.Release();
            }

        }

        public async Task BroadcastAsync(Context context, string channelId, CancellationToken ct = default)
        {
            await EnsureInitialized(ct).ConfigureAwait(false);
            MessageEnvelope messageEnvelope = new MessageEnvelope()
            {
                ActionType = Fdc3Action.Broadcast,
                Payload = new EnvelopeData() { Context = context, ChannelId = channelId },
                Meta = new EnvelopeMeta()
                {
                    Source = _appIdentifier,
                    UniqueMessageId = Guid.NewGuid().ToString(),
                }
            };
            await _backplaneTransport.BroadcastAsync(messageEnvelope, ct).ConfigureAwait(false);

        }

        public async Task<Context> GetCurrentContextAsync(string channelId, CancellationToken ct = default)
        {
            await EnsureInitialized(ct).ConfigureAwait(false);
            return await _backplaneTransport.GetCurrentContextAsync(channelId, ct).ConfigureAwait(false);
        }

        public async Task<IEnumerable<Channel>> GetSystemChannelsAsync(CancellationToken ct = default)
        {
            await EnsureInitialized(ct).ConfigureAwait(false);
            return _systemChannels;
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

        public async ValueTask DisposeAsync()
        {
            _threadSafeInitializationHandle?.Dispose();
            await _backplaneTransport.DisposeAsync();
            _isInitialized = false;
        }


    }
}
