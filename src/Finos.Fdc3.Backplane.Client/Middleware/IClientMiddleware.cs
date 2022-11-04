/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/


using Finos.Fdc3.Backplane.Client.Transport;
using Finos.Fdc3.Backplane.DTO.FDC3;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.Middleware
{
    /// <summary>
    /// Service that communicates with transport layer.
    /// </summary>
    public interface IClientMiddleware : IDisposable
    {
        /// <summary>
        /// Connection State
        /// </summary>
        IObservable<ConnectionState> ConnectionStateStream { get; }
        /// <summary>
        /// Initialze connection
        /// </summary>
        /// <param name="appMetadata"></param>
        /// <param name="retryCount">No of times to retry connection creation</param>
        /// <param name="retryIntervalProvider">Retry interval</param>
        /// <param name="ct">Cancellationtoken</param>
        /// <returns></returns>
        Task InitializeAsync(AppIdentifier appMetadata, int retryCount, Func<int, TimeSpan> retryIntervalProvider, CancellationToken ct = default);

        /// <summary>
        /// Add context listener
        /// </summary>
        /// <param name="contextType"></param>
        /// <param name="handler"></param>
        /// <param name="channelId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IListener> AddContextListenerAsync(string contextType, Action<IContext> handler, string channelId, CancellationToken ct = default);

        /// <summary>
        /// Broadcast Context
        /// </summary>
        /// <param name="context"></param>
        /// <param name="channelId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task BroadcastAsync(IContext context, string channelId, CancellationToken ct = default);

        /// <summary>
        /// Get last broadcasted context over the channel.
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IContext> GetCurrentContextAsync(string channelId, CancellationToken ct = default);

        /// <summary>
        /// Get system channels
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<IChannel>> GetSystemChannelsAsync(CancellationToken ct = default);

    }
}
