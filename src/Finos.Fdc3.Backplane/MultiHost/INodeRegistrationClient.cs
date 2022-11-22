/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.MultiHost
{
    /// <summary>
    /// Register backplane for discovery.
    /// For example: In multihost scenario, node can be resgistered in service discovery for current user 
    /// and later used to form cluster on nodes running under same user
    /// </summary>
    public interface INodeRegistrationClient
    {
        /// <summary>
        /// Get backplane uri
        /// </summary>
        Uri CurrentNodeUri { get; }

        /// <summary>
        /// Register the uri of backplane
        /// </summary>
        /// <param name="uri">uri at which backplane is running, ex: http://hostname:port</param>
        /// <param name="ct">cancellation token</param>
        /// <returns></returns>
        Task RegisterAsync(Uri uri, CancellationToken ct = default);
    }
}
