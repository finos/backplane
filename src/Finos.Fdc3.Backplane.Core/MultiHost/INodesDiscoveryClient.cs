/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Core.MultiHost
{
    /// <summary>
    /// Interface for discovering member backplanes. So that inter-desktop communication can be supported. 
    /// </summary>
    public interface INodesDiscoveryClient
    {
        Task<IEnumerable<Uri>> DiscoverAsync(CancellationToken ct = default);
    }
}
