/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.DTO;
using Finos.Fdc3.Backplane.DTO.Envelope;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.API
{
    /// <summary>
    /// Backplane client exposing API to communicate with backplane.
    /// </summary>
    public interface IBackplaneClient : IAsyncDisposable
    {

        /// <summary>
        /// Connects with backplane.
        /// </summary>
        /// <param name="onMessage">Handler to be invoked when backplane send message to this client</param>
        /// <param name="onDisconnect">Handler to be invoked when websocket disconnection happens</param>
        /// <param name="ct">cancellation token</param>
        /// <returns></returns>
        Task<AppIdentifier> ConnectAsync(Action<MessageEnvelope> onMessage, Func<Exception, Task> onDisconnect, CancellationToken ct = default);

        /// <summary>
        /// Broadcast context  
        /// </summary>
        /// <param name="context">FDC3 Context</param>
        /// <param name="channelId">channelId to associate context with</param>
        /// <param name="ct">cancellation token</param>
        /// <returns></returns>
        Task BroadcastAsync(Context context, string channelId, CancellationToken ct = default);

        /// <summary>
        /// Get FDC3 recommended 8 user channels from backplane
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IEnumerable<Channel>> GetUserChannelsAsync(CancellationToken ct = default);

    }
}