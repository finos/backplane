/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.Client.Transport;
using Finos.Fdc3.Backplane.Client.Utils;
using Finos.Fdc3.Backplane.DTO.Envelope;
using Finos.Fdc3.Backplane.DTO.FDC3;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.API
{
    /// <summary>
    /// Backplane client exposing API to communicate with backplane.
    /// </summary>
    internal class BackplaneClient : IBackplaneClient
    {
        private readonly List<Channel> _userChannels;
        private AppIdentifier _appIdentifier;
        private readonly Lazy<IBackplaneTransport> _backplaneTransport;


        public BackplaneClient(Lazy<IBackplaneTransport> backplaneTransport)
        {
            _userChannels = new List<Channel>();
            _backplaneTransport = backplaneTransport;
        }

        /// <summary>
        /// Connects with backplane.
        /// </summary>
        /// <param name="onMessage">Handler to be invoked when backplane send message to this client</param>
        /// <param name="onDisconnect">Handler to be invoked when websocket disconnection happens</param>
        /// <param name="ct">cancellation token</param>
        /// <returns></returns>
        public async Task<AppIdentifier> ConnectAsync(Action<MessageEnvelope> onMessage, Func<Exception, Task> onDisconnect,CancellationToken ct = default)
        {
            _appIdentifier = await _backplaneTransport.Value.ConnectAsync(onMessage,onDisconnect, ct).ConfigureAwait(false);
            IEnumerable<Channel> channels = await _backplaneTransport.Value.GetUserChannelsAsync().ConfigureAwait(false);
            _userChannels.AddRange(channels);
            return _appIdentifier;
        }

        /// <summary>
        /// Broadcast context  
        /// </summary>
        /// <param name="context">FDC3 Context</param>
        /// <param name="channelId">channelId to associate context with</param>
        /// <param name="ct">cancellation token</param>
        /// <returns></returns>
        public async Task BroadcastAsync(Context context, string channelId, CancellationToken ct = default)
        {
            MessageEnvelope messageEnvelope = MessageEnvelopGenerator.GetMessageEnvelope(context, channelId, _appIdentifier);
            await _backplaneTransport.Value.BroadcastAsync(messageEnvelope, ct).ConfigureAwait(false);
        }

        /// <summary>
        /// Get FDC3 recommended 8 user channels from backplane
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Channel>> GetUserChannelsAsync(CancellationToken ct = default)
        {
            return await Task.FromResult(_userChannels);
        }

        /// <summary>
        /// Dispose client.
        /// </summary>
        /// <returns></returns>
        public async ValueTask DisposeAsync()
        {
            await _backplaneTransport.Value.DisposeAsync();
        }

    }
}
