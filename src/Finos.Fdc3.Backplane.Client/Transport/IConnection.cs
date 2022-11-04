/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/


using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Finos.Fdc3.Backplane.Client.Test")]
namespace Finos.Fdc3.Backplane.Client.Transport
{
    internal interface IConnection : IAsyncDisposable
    {
        event Func<Exception, Task> Closed;
        event Func<string, Task> Reconnected;
        ConnectionState State { get; }
        Task StartAsync(CancellationToken ct = default);
        Task InvokeAsync(string methodName, object arg, CancellationToken cancellationToken = default);
        Task InvokeAsync(string methodName, object arg1, object arg2, CancellationToken cancellationToken = default);
        Task<TResult> InvokeAsync<TResult>(string methodName, CancellationToken cancellationToken = default);
        Task<TResult> InvokeAsync<TResult>(string methodName, object arg, CancellationToken cancellationToken = default);
        IDisposable On<T>(string methodName, Action<T> handler);
        void Build(string url, CancellationToken ct);
    }
}