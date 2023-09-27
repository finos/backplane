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

namespace Finos.Fdc3.Backplane.Client.Transport
{
    /// <summary>
    /// Backplane transport interface
    /// </summary>
    public interface IBackplaneTransport : IAsyncDisposable
    {
        /// <summary>
        /// Connect with backplane
        /// </summary>
        /// <param name="onMessage"></param>
        /// <param name="onDisconnect"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<AppIdentifier> ConnectAsync(Action<MessageEnvelope> onMessage, Func<Exception, Task> onDisconnect, CancellationToken ct = default);

        /// <summary>
        /// Get user channels from backplane
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IEnumerable<Channel>> GetUserChannelsAsync(CancellationToken ct = default);

        /// <summary>
        /// Broadcast Context
        /// </summary>
        /// <param name="message"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task BroadcastAsync(MessageEnvelope message, CancellationToken ct = default);

    }
}