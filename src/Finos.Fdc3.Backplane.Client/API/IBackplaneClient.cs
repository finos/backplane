/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.DTO.Envelope;
using Finos.Fdc3.Backplane.DTO.FDC3;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.API
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBackplaneClient : IAsyncDisposable
    {

        /// <summary>
        /// Start connection with backplane
        /// </summary>
        /// <param name="onMessage">Message handler</param>
        /// <param name="onDisconnect">On disconnection</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<AppIdentifier> ConnectAsync(Action<MessageEnvelope> onMessage, Func<Exception, Task> onDisconnect, CancellationToken ct = default);

        /// <summary>
        /// Broadcast context  
        /// </summary>
        /// <param name="context">Context Json</param>
        /// <param name="channelId">channelId</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task BroadcastAsync(Context context, string channelId, CancellationToken ct = default);

        /// <summary>
        /// Get system channels from backplane
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IEnumerable<Channel>> GetSystemChannelsAsync(CancellationToken ct = default);

    }
}