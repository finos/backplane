/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/


using Finos.Fdc3.Backplane.DTO.Envelope;
using Finos.Fdc3.Backplane.DTO.FDC3;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Finos.Fdc3.Backplane.Client.Test")]
namespace Finos.Fdc3.Backplane.Client.Transport
{
    internal class SignalRBackplaneTransport : IBackplaneTransport
    {
        private readonly ILogger<IBackplaneTransport> _logger;
        private readonly ISignalRConnectionBuilder _signalRConnectionBuilder;
        private const string MSG_CONNECTION_CLOSED = "Underlying connection is closed!";
        private ISignalRConnection _signalRConnection;

        public SignalRBackplaneTransport(ILogger<IBackplaneTransport> logger,
            ISignalRConnectionBuilder signalRConnectionBuilder)
        {
            _logger = logger;
            _signalRConnectionBuilder = signalRConnectionBuilder;
        }


        public async Task<Context> GetCurrentContextAsync(string channelId, CancellationToken ct = default)
        {
            if (_signalRConnection.State != ConnectionState.Connected)
            {
                throw new InvalidOperationException(MSG_CONNECTION_CLOSED);
            }
            JObject ctxJObject = await _signalRConnection.InvokeAsync<JObject>("GetCurrentContextForChannel", channelId, ct);
            _logger.LogInformation($"Received current context for channel: {channelId} from server");
            return ctxJObject == null ? null : new Context(ctxJObject);
        }

        public async Task<IEnumerable<Channel>> GetSystemChannelsAsync(CancellationToken ct = default)
        {
            if (_signalRConnection.State != ConnectionState.Connected)
            {
                throw new InvalidOperationException(MSG_CONNECTION_CLOSED);
            }
            return await _signalRConnection.InvokeAsync<IEnumerable<Channel>>("GetSystemChannels", ct);
        }

        public async Task BroadcastAsync(MessageEnvelope message, CancellationToken ct = default)
        {
            if (_signalRConnection.State != ConnectionState.Connected)
            {
                throw new InvalidOperationException(MSG_CONNECTION_CLOSED);
            }
            await _signalRConnection.InvokeAsync("Broadcast", message, ct);
            _logger.LogInformation($"Broadcast successfull for message: {JsonSerializer.Serialize(message)}");
        }


        public async Task ConnectAsync(Uri uri,
            Action<MessageEnvelope> onMessage, Func<Exception, Task> onDisconnect, CancellationToken ct = default)
        {
            _signalRConnection = _signalRConnectionBuilder.Build(uri);
            _signalRConnection.On("OnMessage", onMessage);
            _signalRConnection.Closed += onDisconnect;
            _logger.LogInformation($"Connecting with backplane...");
            await _signalRConnection.StartAsync();
        }

        public async ValueTask DisposeAsync()
        {
            await _signalRConnection.DisposeAsync();
        }

    }
}

