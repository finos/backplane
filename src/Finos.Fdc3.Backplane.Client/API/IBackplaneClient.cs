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
        /// 
        /// </summary>
        /// <param name="onMessage"></param>
        /// <param name="onDisconnect"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<AppIdentifier> ConnectAsync(Action<MessageEnvelope> onMessage, Func<Exception, Task> onDisconnect, CancellationToken ct = default);

        /// <summary>
        /// Publishes context to other apps on the desktop. 
        /// Calling `broadcast` at the `DesktopAgent` scope will push the context to whatever `Channel` the app is joined to. 
        /// If the app is not currently joined to a channel, calling `desktoAgentObject.broadcast` will have no effect. 
        /// Apps can still directly broadcast and listen to context on any channel via the methods on the `Channel` class.
        /// </summary>
        /// <param name="context">Context Json</param>
        /// <param name="channelId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task BroadcastAsync(Context context, string channelId, CancellationToken ct = default);

        /// <summary>
        /// Get current context for channel
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Context> GetCurrentContextAsync(string channelId, CancellationToken ct = default);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IEnumerable<Channel>> GetSystemChannelsAsync(CancellationToken ct = default);

    }
}