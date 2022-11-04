/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.DTO.FDC3
{
    /// <summary>
    /// Interface representing a context channel.
    /// </summary>
    public interface IChannel
    {
        /// <summary>
        /// Constant that uniquely identifies this channel.
        /// </summary>
        string Id { get; }

        /// <summary>
        ///  Uniquely defines each channel type.
        /// </summary>
        string Type { get; }

        /// <summary>
        /// Channels may be visualized and selectable by users. DisplayMetadata may be used to provide hints on how to see them.
        /// </summary>
        DisplayMetadata DisplayMetadata { get; }

        /// <summary>
        /// Broadcasts the given context on this channel. This is equivalent to joining the channel and then calling the
        /// top-level FDC3 `broadcast` function.
        /// Note that this function can be used without first joining the channel, allowing applications to broadcast on
        /// channels that they aren't a member of.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task BroadcastAsync(IContext context, CancellationToken ct = default);

        /// <summary>
        /// Returns the last context that was broadcast on this channel. All channels initially have no context, until a
        /// context is broadcast on the channel.If there is not yet any context on the channel, this method
        /// will return `null`.
        /// The context of a channel will be captured regardless of how the context is broadcasted on this channel - whether
        /// using the top-level FDC3 `broadcast` function, or using the channel-level {@link broadcast} function on this object
        /// Optionally a {@link contextType} can be provided, in which case the current context of the matching type will
        /// be returned(if any).
        /// </summary>
        /// <param name="contextype"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IContext> GetCurrentContextAsync(string contextype = null, CancellationToken ct = default);

        /// <summary>
        /// Adds a listener for incoming contexts of the specified context type whenever a broadcast happens on this channel.
        /// </summary>
        /// <param name="contextType"></param>
        /// <param name="handler"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IListener> AddContextListenerAsync(string contextType, Action<IContext> handler, CancellationToken ct = default);
    }
}
