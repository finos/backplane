/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.MultiHost
{
    /// <summary>
    /// Template implementation of member nodes discovery interface. 
    /// Please use your own discovery mechanism here.
    /// </summary>
    public class NodesDiscoveryClient : INodesDiscoveryClient
    {
        private readonly ILogger<NodesDiscoveryClient> _logger;

        public NodesDiscoveryClient(ILogger<NodesDiscoveryClient> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Discovers member nodes
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Uri>> DiscoverAsync(CancellationToken ct = default)
        {
            //Implement here discovery mechanism.
            IEnumerable<Uri> nodes = await Task.FromResult(Enumerable.Empty<Uri>());
            _logger.LogInformation($"Discovered nodes:{nodes}");
            return nodes;
        }

    }
}
