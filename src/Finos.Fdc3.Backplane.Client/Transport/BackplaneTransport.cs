/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/


using Finos.Fdc3.Backplane.Client.Resilliency;
using Finos.Fdc3.Backplane.DTO.Envelope.Receive;
using Finos.Fdc3.Backplane.DTO.Envelope.Send;
using Finos.Fdc3.Backplane.DTO.FDC3;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Polly;
using System;
using System.Collections.Generic;
using System.Reactive.Subjects;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Finos.Fdc3.Backplane.Client.Test")]
namespace Finos.Fdc3.Backplane.Client.Transport
{
    internal class BackplaneTransport : IBackplaneTransport
    {
        private readonly IConnectionFactory _connectionFactory;
        private readonly ILogger<IBackplaneTransport> _logger;
        private readonly IRetryPolicyProvider _retryPolicyProvider;
        private IConnection _connection;
        private IAsyncPolicy _retryPolicyForever;
        private readonly Subject<ConnectionState> _connectionStateStream;
        private readonly Subject<MessageEnvelope> _dataStream;
        private const string MSG_CONNECTION_CLOSED = "Underlying connection is closed!";

        public IObservable<ConnectionState> ConnectionStateStream => _connectionStateStream;
        public IObservable<MessageEnvelope> ReceiveDataStream => _dataStream;

        public BackplaneTransport(ILogger<IBackplaneTransport> logger,
            IConnectionFactory connectionFactory,
            IRetryPolicyProvider retryPolicyProvider)
        {
            _logger = logger;
            _retryPolicyProvider = retryPolicyProvider;
            _connectionFactory = connectionFactory;
            _connectionStateStream = new Subject<ConnectionState>();
            _dataStream = new Subject<MessageEnvelope>();
        }

        public async Task InitializeConnectionAsync(int retryCount, Func<int, TimeSpan> retryIntervalProvider, CancellationToken ct = default)
        {
            _retryPolicyForever = _retryPolicyProvider.GetAsyncRetryPolicy<Exception>(int.MaxValue, retryIntervalProvider);
            IAsyncPolicy retryPolicy = _retryPolicyProvider.GetAsyncRetryPolicy<Exception>(retryCount, retryIntervalProvider);
            await retryPolicy.ExecuteAsync(async () => await StartConnection(ct));
        }

        public async Task BroadcastAsync(BroadcastContextEnvelope message, CancellationToken ct = default)
        {
            if (_connection.State != ConnectionState.Connected)
            {
                throw new InvalidOperationException(MSG_CONNECTION_CLOSED);
            }
            await _connection.InvokeAsync("Broadcast", message, false, ct);
            _logger.LogInformation($"Broadcast successfull for message: {JsonConvert.SerializeObject(message)}");
        }

        public async Task<IContext> GetCurrentContextAsync(string channelId, CancellationToken ct = default)
        {
            if (_connection.State != ConnectionState.Connected)
            {
                throw new InvalidOperationException(MSG_CONNECTION_CLOSED);
            }
            JObject ctxJObject = await _connection.InvokeAsync<JObject>("GetCurrentContextForChannel", channelId, ct);
            _logger.LogInformation($"Received current context for channel: {channelId} from server");
            return ctxJObject == null ? null : new DTO.FDC3.Context(ctxJObject);
        }

        public async Task<IEnumerable<Channel>> GetSystemChannelsAsync(CancellationToken ct = default)
        {
            if (_connection.State != ConnectionState.Connected)
            {
                throw new InvalidOperationException(MSG_CONNECTION_CLOSED);
            }
            return await _connection.InvokeAsync<IEnumerable<Channel>>("GetSystemChannels", ct);
        }

        private async Task StartConnection(CancellationToken ct)
        {
            try
            {
                _connection = await _connectionFactory.Create(ct);
                _connection.Closed += HubConnectionClosed;
                AttachHandlers();
                await _connection.StartAsync();
                _connectionStateStream.OnNext(ConnectionState.Connected);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to start connection. {ex}");
                await DisposeConnectionObjects();
                throw;
            }
        }

        private async Task HubConnectionClosed(Exception arg)
        {
            _logger.LogError($"Connection lost with server..");
            _connectionStateStream.OnNext(ConnectionState.Disconnected);
            await DisposeConnectionObjects();
            await _retryPolicyForever.ExecuteAsync(async () => await StartConnection(CancellationToken.None));
        }

        private void AttachHandlers()
        {
            _connection.On<MessageEnvelope>("ReceiveBroadcastMessage", HandleReceiveContext);
            _connection.On<MessageEnvelope>("ReceiveRaiseIntentMessage", HandleReceiveContext);
        }

        private async Task DisposeConnectionObjects()
        {
            _connection.Closed -= HubConnectionClosed;
            await _connection.DisposeAsync();
        }

        private void HandleReceiveContext(MessageEnvelope obj)
        {
            _dataStream.OnNext(obj);
        }

        public void Dispose()
        {
            _dataStream.Dispose();
            _connectionStateStream.Dispose();
            _connection.Closed -= HubConnectionClosed;
            _connection.DisposeAsync();
        }
    }
}

