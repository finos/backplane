/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.Client.Transport;
using Finos.Fdc3.Backplane.DTO.FDC3;
using Finos.Fdc3.Backplane.DTO.FDC3.Exceptions.Channel;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.API
{
    /// <summary>
    /// 
    /// </summary>
    public interface IBackplaneClient
    {
        /// <summary>
        ///  Adds a listener for incoming context broadcasts from the Desktop Agent.
        ///  If the consumer is only interested in a context of a particular type, they can they can specify that type. 
        ///  If the consumer is able to receive context of any type or will inspect types received, 
        ///  then they can pass `null` as the `contextType` parameter to receive all context types.
        ///  Context broadcasts are only received from apps that are joined to the same channel as the listening application,
        ///  hence, if the application is not currently joined to a channel no broadcasts will be received. 
        ///  If this function is called after the app has already joined a channel and the channel already contains context
        ///  that would be passed to the context listener, then it will be called immediately with that context.
        /// </summary>
        /// <param name="contextType">Context type to listen, for example fdc3.instrument</param>
        /// <param name="handler">Handler</param>
        /// <returns></returns>
        Task<IListener> AddContextListenerAsync(string contextType, Action<IContext> handler, CancellationToken ct = default);

        /// <summary>
        /// Publishes context to other apps on the desktop. 
        /// Calling `broadcast` at the `DesktopAgent` scope will push the context to whatever `Channel` the app is joined to. 
        /// If the app is not currently joined to a channel, calling `desktoAgentObject.broadcast` will have no effect. 
        /// Apps can still directly broadcast and listen to context on any channel via the methods on the `Channel` class.
        /// </summary>
        /// <param name="context">Context Json</param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task BroadcastAsync(IContext context, CancellationToken ct = default);

        /// <summary>
        /// Dispose
        /// </summary>
        void Dispose();

        /// <summary>
        /// Returns the `Channel` object for the current channel membership.
        /// Returns `null` if the app is not joined to a channel.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<IChannel> GetCurrentChannelAsync(CancellationToken ct = default);


        /// <summary>
        /// Retrieves a list of the System channels available for the app to join
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<IChannel>> GetSystemChannelsAsync(CancellationToken ct = default);

        /// <summary>
        /// Initialize desktop agent client by initiating connection with server with wait and retry for retryCount.
        /// After Successful initialization, client is ready to perform fdc3 operations.
        /// </summary>
        /// <param name="appMetadata"></param>
        /// <param name="retryCount">No of times to retry initialization in case of failure.
        /// Failure scenario could be like failure in establishing connection with server.
        /// Default value is 1
        /// </param>
        /// <param name="retryIntervalProvider">Wait interval provider between each retry. Default value is 1 sec</param>
        /// <param name="ct">cancellation token</param>
        /// <returns>void</returns>
        Task InitializeAsync(AppIdentifier appMetadata, int retryCount = 1, Func<int, TimeSpan> retryIntervalProvider = null, CancellationToken ct = default);

        /// <summary>
        /// Joins the app to the specified channel.
        /// If an app is joined to a channel, all `fdc3.broadcast` calls will go to the channel, and all listeners assigned via `fdc3.addContextListener` 
        /// will listen on the channel. If the channel already contains context that would be passed to context listeners assed via `fdc3.addContextListener` 
        /// then those listeners will be called immediately with that context.
        /// An app can only be joined to one channel at a time.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ct"></param>
        /// <exception cref="ChannelAccessDeniedException" ></exception>
        /// <exception cref="ChannelCreationFailedException"></exception>
        /// <exception cref="NoChannelFoundException"></exception>
        Task JoinChannelAsync(string id, CancellationToken ct = default);

        /// <summary>
        /// Removes the app from any channel membership.
        /// Context broadcast and listening through the top-level `desktopAgentObject.broadcast` and `desktopAgentObject.addContextListener` will be a no-op when the app is not on a channel.
        /// </summary>
        Task LeaveCurrentChannelAsync(CancellationToken ct = default);

        /// <summary>
        /// Connection status observable
        /// </summary>
        /// <returns></returns>
        IObservable<ConnectionState> ObserveClientConnectionState();

        /// <summary>
        /// Observe client notifications
        /// </summary>
        /// <returns></returns>
        IObservable<Notification> ObserveClientNotifications();
    }
}