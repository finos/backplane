/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.Config;
using Finos.Fdc3.Backplane.MultiHost;
using Finos.Fdc3.Backplane.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.WorkerService
{
    /// <summary>
    /// Background service to keep check of active member nodes 
    /// and update member node repository 
    /// </summary>
    public class MemberNodesHealthCheckService : BackgroundService
    {
        private readonly IConfigRepository _config;
        private readonly ILogger<MemberNodesHealthCheckService> _logger;
        private readonly IHostingUtils _hostingUtils;
        private readonly INodeRegistrationClient _nodeRegistrationClient;
        private readonly INodesRepository _memberNodesRepository;
        private readonly INodesDiscoveryClient _nodesDiscoveryClient;
        private readonly IHttpClientFactory _httpClientFactory;
        private CancellationToken _cancellationToken;


        public MemberNodesHealthCheckService(IHostingUtils hostingUtils, INodesDiscoveryClient nodesDiscoveryClient,
            IConfigRepository config, INodeRegistrationClient nodeRegistrationClient,
            ILogger<MemberNodesHealthCheckService> logger,
            IHttpClientFactory httpClientFactory, INodesRepository memberNodesRepository)
        {
            _config = config;
            _logger = logger;
            _hostingUtils = hostingUtils;
            _nodeRegistrationClient = nodeRegistrationClient;
            _httpClientFactory = httpClientFactory;
            _memberNodesRepository = memberNodesRepository;
            _nodesDiscoveryClient = nodesDiscoveryClient;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _cancellationToken = ct;
            // hook on backplane start
            _hostingUtils.BackplaneStarted += OnBackplaneRunning;
            await Task.CompletedTask;
        }

        private void OnBackplaneRunning()
        {
            _ = PerformHealthCheckInBackground();
        }

        private async Task PerformHealthCheckInBackground()
        {
            TimeSpan healthCheckIntervalMs = _config.MemberNodesHealthCheckIntervalInMilliseconds;
            TimeSpan httpRequestTimeOutInMs = _config.HttpRequestTimeoutInMilliseconds;
            string addNodePath =  _config.AddNodeEndpoint;


            _logger.LogInformation($"HealthCheck service started with health check interval of {healthCheckIntervalMs} ms.");

            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    IEnumerable<Uri> discoveredNodes = await _nodesDiscoveryClient.DiscoverAsync(_cancellationToken);
                    foreach (Uri nodeUri in discoveredNodes)
                    {
                        try
                        {
                            Uri addMemberNodeUri = new Uri(nodeUri, addNodePath);
                            HttpResponseMessage response = await HttpUtils.PostAsync(_httpClientFactory, addMemberNodeUri, _nodeRegistrationClient.CurrentNodeUri, httpRequestTimeOutInMs);
                            if (response.IsSuccessStatusCode)
                            {
                                _memberNodesRepository.AddNode(nodeUri);
                                _logger.LogDebug($"Added/updated node: {nodeUri} as node is live.");
                            }
                            else
                            {
                                // response code does not indicate success, remove the node.
                                _memberNodesRepository.RemoveNode(nodeUri);
                                _logger.LogDebug($"Removed node: {nodeUri} from member nodes as node did not respond");
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError($"Failed to check heartbeat of node:{nodeUri}.{ex}");
                        }

                    }

                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception occurred in member backplane health check service. Message: {ex}");
                }
                //wait for health check interval.
                await Task.Delay(healthCheckIntervalMs, _cancellationToken);
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("member backplane health check service stopped.");
            _hostingUtils.BackplaneStarted -= OnBackplaneRunning;
            await base.StopAsync(stoppingToken);
        }
    }
}
