/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/


using Finos.Fdc3.Backplane.DTO.Envelope.Receive;
using Finos.Fdc3.Backplane.DTO.Envelope.Send;
using Finos.Fdc3.Backplane.DTO.FDC3;
using System;
using System.Collections.Generic;
using System.Threading;

using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.Transport
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBackplaneTransport : IDisposable
    {
        /// <summary>
        /// Initialize transport
        /// </summary>
        /// <param name="retryCount"></param>
        /// <param name="RetryIntervalProvider"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task InitializeConnectionAsync(int retryCount, Func<int, TimeSpan> RetryIntervalProvider, CancellationToken ct);

        /// <summary>
        /// Data Stream
        /// </summary>
        IObservable<MessageEnvelope> ReceiveDataStream { get; }

        /// <summary>
        /// Observable for connection status
        /// </summary>
        IObservable<ConnectionState> ConnectionStateStream { get; }

        /// <summary>
        /// Get system channels data from server.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IEnumerable<Channel>> GetSystemChannelsAsync(CancellationToken ct = default);

        /// <summary>
        /// Get last broadcasted context over the channel.
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IContext> GetCurrentContextAsync(string channelId, CancellationToken ct = default);

        /// <summary>
        /// Broadcast Context
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task BroadcastAsync(BroadcastContextEnvelope message, CancellationToken ct = default);

    }
}
