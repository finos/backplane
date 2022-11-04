/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.Discovery
{
    internal class BackplaneDiscoveryServiceClient : IBackplaneDiscoveryServiceClient
    {
        private readonly ILogger<BackplaneDiscoveryServiceClient> _logger;

        public BackplaneDiscoveryServiceClient(ILogger<BackplaneDiscoveryServiceClient> logger)
        {
            _logger = logger;
        }


        public async Task<Uri> DiscoverAsync(CancellationToken ct)
        {
            _logger.LogInformation($"Discovering local backplane service: {"Finos.Fdc3.DesktopAgent.Backplane"}, machine name : {Environment.MachineName}");
            return await Task.FromResult(new Uri("http://localhost:49201"));
        }
    }
}
