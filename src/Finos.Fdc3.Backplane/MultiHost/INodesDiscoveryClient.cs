/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.MultiHost
{
    /// <summary>
    /// Discovery of member backplanes. 
    /// </summary>
    public interface INodesDiscoveryClient
    {
        /// <summary>
        /// Discover member backplane.
        /// </summary>
        /// <param name="ct">cancellation token</param>
        /// <returns>Member nodes uri</returns>
        Task<IEnumerable<Uri>> DiscoverAsync(CancellationToken ct = default);
    }
}
