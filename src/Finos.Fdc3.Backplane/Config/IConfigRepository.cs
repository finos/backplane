/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/
using Finos.Fdc3.Backplane.DTO.FDC3;
using System;
using System.Collections.Generic;

namespace Finos.Fdc3.Backplane.Config
{
    /// <summary>
    /// Configuration
    /// </summary>
    public interface IConfigRepository
    {
        /// <summary>
        /// Member nodes from config
        /// </summary>
        IEnumerable<Uri> MemberNodes { get; }

        /// <summary>
        /// Channels list from config
        /// </summary>
        IEnumerable<Channel> Channels { get; }
        /// <summary>
        /// Http Request timeout
        /// </summary>
        TimeSpan HttpRequestTimeoutInMilliseconds { get; }

        /// <summary>
        /// Health check interval for member nodes
        /// </summary>
        TimeSpan MemberNodesHealthCheckIntervalInMilliseconds { get; }

        /// <summary>
        /// Add node endpoint in mutihost scenario
        /// </summary>
        string AddNodeEndpoint { get; }

        /// <summary>
        /// Broadcast endpoint 
        /// </summary>
        string BroadcastEndpoint { get; }

        /// <summary>
        /// Backplane endpoint
        /// </summary>
        string HubEndpoint { get; }
    }
}
