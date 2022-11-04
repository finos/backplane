/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/


using Finos.Fdc3.Backplane.Client.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Finos.Fdc3.Backplane.Client.Test")]
namespace Finos.Fdc3.Backplane.Client.Transport
{
    internal class SignalRConnection : IConnection
    {
        private HubConnection _connection;
        private readonly ILogger<IConnection> _logger;

        public event Func<Exception, Task> Closed
        {
            add
            {
                _connection.Closed += value;
            }
            remove
            {
                _connection.Closed -= value;
            }
        }

        public event Func<string, Task> Reconnected
        {
            add
            {
                _connection.Reconnected += value;
            }
            remove
            {
                _connection.Reconnected -= value;
            }
        }

        public ConnectionState State => Transform(_connection.State);

        public SignalRConnection(ILogger<IConnection> logger)
        {
            _logger = logger;
        }

        public void Build(string url, CancellationToken ct)
        {
            _connection = new HubConnectionBuilder()
                  .WithUrl(url)
                  .AddNewtonsoftJsonProtocol()
                  .ConfigureLogging(logging =>
                  {
                      logging.AddProvider(_logger.AsLoggerProvider());
                  }).Build();
        }

        public async Task StartAsync(CancellationToken ct = default)
        {
            await _connection.StartAsync(ct);
        }

        public IDisposable On<T>(string methodName, Action<T> handler)
        {
            return _connection.On(methodName, handler);
        }

        public Task InvokeAsync(string methodName, object arg, CancellationToken cancellationToken = default)
        {
            return _connection.InvokeAsync(methodName, arg, cancellationToken);
        }

        public Task InvokeAsync(string methodName, object arg1, object arg2, CancellationToken cancellationToken = default)
        {
            return _connection.InvokeAsync(methodName, arg1, arg2, cancellationToken);
        }

        public Task<TResult> InvokeAsync<TResult>(string methodName, CancellationToken cancellationToken = default)
        {
            return _connection.InvokeAsync<TResult>(methodName, cancellationToken);
        }

        public Task<TResult> InvokeAsync<TResult>(string methodName, object arg, CancellationToken cancellationToken = default)
        {
            return _connection.InvokeAsync<TResult>(methodName, arg, cancellationToken);
        }

        public ValueTask DisposeAsync()
        {
            return _connection.DisposeAsync();
        }

        private ConnectionState Transform(HubConnectionState hubConnectionState)
        {
            switch (hubConnectionState)
            {
                case HubConnectionState.Connected:
                    return ConnectionState.Connected;
                case HubConnectionState.Connecting:
                    return ConnectionState.Connecting;
                case HubConnectionState.Disconnected:
                    return ConnectionState.Disconnected;
                case HubConnectionState.Reconnecting:
                    return ConnectionState.Reconnecting;
                default:
                    return ConnectionState.Disconnected;
            }
        }
    }
}

