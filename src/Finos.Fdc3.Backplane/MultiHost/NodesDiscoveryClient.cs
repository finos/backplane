/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.Config;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
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
        private readonly IConfigRepository _configRepository;
        private readonly IEnumerable<Uri> _memberNodes;

        public NodesDiscoveryClient(ILogger<NodesDiscoveryClient> logger, IConfigRepository configRepository)
        {
            _logger = logger;
            _configRepository = configRepository;
            _memberNodes = _configRepository.MemberNodes;
        }

        /// <summary>
        /// Discovers member nodes
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task<IEnumerable<Uri>> DiscoverAsync(CancellationToken ct = default)
        {
            //You can implement own discovery mechanism. Current implementation uses config based settings
            IEnumerable<Uri> nodes = _memberNodes;
            _logger.LogDebug($"Discovered nodes:{nodes}");
            return await Task.FromResult(nodes);
        }

    }
}
