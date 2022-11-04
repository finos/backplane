/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/


using Finos.Fdc3.Backplane.Client.API;
using Finos.Fdc3.Backplane.Client.Broadcast.Channels;
using Finos.Fdc3.Backplane.Client.Concurrency;
using Finos.Fdc3.Backplane.Client.Transport;
using Finos.Fdc3.Backplane.DTO.Envelope;
using Finos.Fdc3.Backplane.DTO.Envelope.Receive;
using Finos.Fdc3.Backplane.DTO.Envelope.Send;
using Finos.Fdc3.Backplane.DTO.FDC3;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Finos.Fdc3.Backplane.Client.Test")]
namespace Finos.Fdc3.Backplane.Client.Middleware
{
    internal class ClientMiddleware : IClientMiddleware
    {
        private readonly IBackplaneTransport _desktopAgentTransport;
        private readonly ILogger<IClientMiddleware> _logger;
        private readonly ISchedulerProvider _schedulerProvider;
        private AppIdentifier _appMetadata;

        public IObservable<ConnectionState> ConnectionStateStream => _desktopAgentTransport.ConnectionStateStream;

        public ClientMiddleware(ILogger<IClientMiddleware> logger,
            IBackplaneTransport desktopAgentTransport,
            ISchedulerProvider schedulerProvider)
        {
            _schedulerProvider = schedulerProvider;
            _desktopAgentTransport = desktopAgentTransport;
            _logger = logger;
        }


        public async Task<IListener> AddContextListenerAsync(string contextType, Action<IContext> handler, string channelId, CancellationToken ct = default)
        {
            string subscribedContextType = contextType;
            Action<IContext> handlerLocal = handler;
            string channelIdSubscribedLocal = channelId;
            _logger.LogInformation($"Adding context listener for context type: {contextType}");
            IDisposable disposable = _desktopAgentTransport.ReceiveDataStream.SubscribeOn(_schedulerProvider.TaskPool).Subscribe(messageEnvelope =>
            {
                try
                {
                    _logger.LogDebug($"Receive Broadcast handler: Received message from backplane with message {JsonConvert.SerializeObject(messageEnvelope)}");
                    if (messageEnvelope.ActionType != Fdc3Action.Broadcast)
                    {
                        return;
                    }

                    _logger.LogInformation($"Receive Broadcast handler: Received message envelope from backplane with id from {messageEnvelope.Meta.Source.AppId} with messageId: {messageEnvelope.Meta.UniqueMessageId}");
                    string json = messageEnvelope.Payload.Context.ToString();
                    Context receivedContext = new Context(messageEnvelope.Payload.Context);

                    // null context type indicates listen all context type
                    if (subscribedContextType == null || subscribedContextType == receivedContext.Type)
                    {
                        if (channelIdSubscribedLocal == messageEnvelope.Payload.ChannelId)
                        {
                            handler(new Context(messageEnvelope.Payload.Context));
                            _logger.LogDebug($"Handler invoked for received context envelope: {JsonConvert.SerializeObject(messageEnvelope)}");
                        }
                        else
                        {
                            _logger.LogDebug($"Not handling contextType: {receivedContext.Type} as it has different channel associated than subscribed: {channelIdSubscribedLocal}");
                        }
                    }
                    else
                    {
                        _logger.LogDebug($"Not handling contextType: {subscribedContextType} as it did not match subscribed context type: {subscribedContextType}");
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Failed to handle broadcast context received from backplane: {ex}");
                }

            });
            return await Task.FromResult(new Listener(async (token) =>
            {
                disposable.Dispose();
                await Task.CompletedTask;
                _logger.LogInformation($"Unsubscribed from context type: {subscribedContextType}");
            }));
        }

        public async Task BroadcastAsync(IContext context, string channelId, CancellationToken ct = default)
        {
            BroadcastContextEnvelope messageEnvelope = new BroadcastContextEnvelope()
            {
                ChannelId = channelId,
                Context = (Context)context,
                Metadata = new EnvelopeMetadata() { Source = _appMetadata, UniqueMessageId = Guid.NewGuid().ToString() }
            };
            _logger.LogInformation($"Broadcasting context with messageId {messageEnvelope.Metadata.UniqueMessageId} on channel: {channelId} to backplane");
            await _desktopAgentTransport.BroadcastAsync(messageEnvelope, ct);

        }

        public void Dispose()
        {
            _logger.LogInformation("Disposing..");
            _desktopAgentTransport.Dispose();
        }

        public async Task<IContext> GetCurrentContextAsync(string channelId, CancellationToken ct = default)
        {
            return await _desktopAgentTransport.GetCurrentContextAsync(channelId, ct);
        }

        public async Task<IEnumerable<IChannel>> GetSystemChannelsAsync(CancellationToken ct = default)
        {
            IEnumerable<Channel> channelsDto = await _desktopAgentTransport.GetSystemChannelsAsync(ct);
            IEnumerable<ChannelClient> systemChannels = channelsDto?.Select(x => new ChannelClient(this, x));
            if (!systemChannels.Any())
            {
                throw new InvalidOperationException("No system channels populated!");
            }

            return systemChannels;
        }

        public async Task InitializeAsync(AppIdentifier appMetadata, int retryCount, Func<int, TimeSpan> retryIntervalProvider, CancellationToken ct = default)
        {
            _appMetadata = appMetadata;
            await _desktopAgentTransport.InitializeConnectionAsync(retryCount, retryIntervalProvider, ct);
        }
    }
}
