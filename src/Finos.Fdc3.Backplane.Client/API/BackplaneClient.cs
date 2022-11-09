/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
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
    /// Desktop Agent Implementation.
    /// </summary>
    internal class BackplaneClient : IBackplaneClient
    {
        private readonly List<Channel> _systemChannels;
        private AppIdentifier _appIdentifier;
        private readonly Lazy<IBackplaneTransport> _backplaneTransport;


        public BackplaneClient(Lazy<IBackplaneTransport> backplaneTransport)
        {
            _systemChannels = new List<Channel>();
            _backplaneTransport = backplaneTransport;
        }

        public async Task<AppIdentifier> ConnectAsync(Action<MessageEnvelope> onMessage, Func<Exception, Task> onDisconnect,CancellationToken ct = default)
        {
            _appIdentifier = await _backplaneTransport.Value.ConnectAsync(onMessage,onDisconnect, ct).ConfigureAwait(false);
            IEnumerable<Channel> channels = await _backplaneTransport.Value.GetSystemChannelsAsync().ConfigureAwait(false);
            _systemChannels.AddRange(channels);
            return _appIdentifier;
        }

        public async Task BroadcastAsync(Context context, string channelId, CancellationToken ct = default)
        {
            MessageEnvelope messageEnvelope = MessageEnvelopGenerator.GetMessageEnvelope(context, channelId, _appIdentifier);
            await _backplaneTransport.Value.BroadcastAsync(messageEnvelope, ct).ConfigureAwait(false);
        }


        public async Task<IEnumerable<Channel>> GetSystemChannelsAsync(CancellationToken ct = default)
        {
            return await Task.FromResult(_systemChannels);
        }

        public async ValueTask DisposeAsync()
        {
            await _backplaneTransport.Value.DisposeAsync();
        }

    }
}
