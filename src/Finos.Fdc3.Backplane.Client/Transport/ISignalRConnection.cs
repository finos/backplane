/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/


using System;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.Transport
{
    /// <summary>
    /// SignalR connection
    /// </summary>
    public interface ISignalRConnection : IAsyncDisposable
    {
        /// <summary>
        /// Connection closed event
        /// </summary>
        event Func<Exception, Task> Closed;

        /// <summary>
        /// Reconnected event
        /// </summary>
        event Func<string, Task> Reconnected;

        /// <summary>
        /// 
        /// </summary>
        ConnectionState State { get; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task StartAsync(CancellationToken ct = default);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="arg"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task InvokeAsync(string methodName, object arg, CancellationToken cancellationToken = default);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="methodName"></param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task InvokeAsync(string methodName, object arg1, object arg2, CancellationToken cancellationToken = default);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TResult> InvokeAsync<TResult>(string methodName, CancellationToken cancellationToken = default);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TResult"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="arg"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        Task<TResult> InvokeAsync<TResult>(string methodName, object arg, CancellationToken cancellationToken = default);
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="methodName"></param>
        /// <param name="handler"></param>
        /// <returns></returns>
        IDisposable On<T>(string methodName, Action<T> handler);

    }
}