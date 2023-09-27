/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/
using Finos.Fdc3.Backplane.DTO;
using System;
using System.Collections.Generic;

namespace Finos.Fdc3.Backplane.Config
{
    /// <summary>
    ///  Backplane configurations 
    /// </summary>
    public interface IConfigRepository
    {
        /// <summary>
        /// Multihost scenario: member nodes defined in config statically
        /// </summary>
        IEnumerable<Uri> MemberNodes { get; }

        /// <summary>
        /// List of 8 recommended user channels populated from config.
        /// </summary>
        IEnumerable<Channel> Channels { get; }

        /// <summary>
        /// HTTP request timeout in ms
        /// </summary>
        TimeSpan HttpRequestTimeoutInMilliseconds { get; }

        /// <summary>
        /// In multihost scenario, member nodes health check interval in ms.
        /// </summary>
        TimeSpan MemberNodesHealthCheckIntervalInMilliseconds { get; }

        /// <summary>
        /// REST endpoint to register node in cluster in multihost scenario.
        /// </summary>
        string AddNodeEndpoint { get; }

        /// <summary>
        /// REST endpoint to propagate message to other member node in multihost scenario.
        /// </summary>
        string BroadcastEndpoint { get; }

        /// <summary>
        /// SignalR hub end point
        /// </summary>
        string HubEndpoint { get; }
    }
}
