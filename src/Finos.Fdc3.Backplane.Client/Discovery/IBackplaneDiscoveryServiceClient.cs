/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.Discovery
{
    /// <summary>
    /// Backplane discovery. 
    /// </summary>
    public interface IBackplaneDiscoveryServiceClient
    {
        /// <summary>
        /// Discover backplane service and return its address.
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task<Uri> DiscoverAsync(CancellationToken ct);
    }
}