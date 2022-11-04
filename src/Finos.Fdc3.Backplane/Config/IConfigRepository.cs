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
        /// Channels list from config
        /// </summary>
        List<Channel> ChannelsList { get; }
        TimeSpan HttpRequestTimeoutInMilliSeconds { get; }
        TimeSpan BackplaneRegistrationRetryIntervalInSeconds { get; }
        TimeSpan MemberNodesHealthCheckIntervalInMilliSeconds { get; }
    }
}
