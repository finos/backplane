using Finos.Fdc3.Backplane.Config;
using Finos.Fdc3.Backplane.Models;
using Finos.Fdc3.Backplane.MultiHost;
using Finos.Fdc3.Backplane.Utils;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly IHttpClientFactory _httpClientFactory;
        private CancellationToken _cancellationToken;
        

        public MemberNodesHealthCheckService(IHostingUtils hostingUtils,
            IConfigRepository config, INodeRegistrationClient nodeRegistrationClient, ILogger<MemberNodesHealthCheckService> logger, IHttpClientFactory httpClientFactory, INodesRepository memberNodesRepository)
        {
            _config = config;
            _logger = logger;
            _hostingUtils = hostingUtils;
            _nodeRegistrationClient = nodeRegistrationClient;
            _httpClientFactory = httpClientFactory;
            _memberNodesRepository = memberNodesRepository;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _cancellationToken = ct;
            // hook on backplane start
            _hostingUtils.BackplaneStart += OnBackplaneRunning;
            await Task.CompletedTask;
        }

        private void OnBackplaneRunning()
        {
            _ = PerformHealthCheckInBackground();
        }

        private async Task PerformHealthCheckInBackground()
        {
            var healthCheckIntervalMs = _config.MemberNodesHealthCheckIntervalInMilliSeconds;
            var httpRequestTimeOutInMs = _config.HttpRequestTimeoutInMilliSeconds;
            _logger.LogInformation($"HealthCheck service started with health check interval of {healthCheckIntervalMs} ms.");
            List<Uri> onlineNodes = new List<Uri>();
            List<Uri> offlineNodes = new List<Uri>();

            while (!_cancellationToken.IsCancellationRequested)
            {
                try
                {
                    //check status of active nodes
                    if (_memberNodesRepository.MemberNodes.Any(u => u.IsActive))
                    {
                        onlineNodes.AddRange(_memberNodesRepository.MemberNodes.Where(u => u.IsActive).Select(x => x.Uri));
                        foreach (Uri uri in onlineNodes)
                        {
                            HttpResponseMessage response = await HttpUtils.PostAsync(_httpClientFactory, new Uri($"{uri}/backplane/api/v1.0/addmembernode"), new Node() { Uri = _nodeRegistrationClient.CurrentNodeUri },httpRequestTimeOutInMs);
                            if (!response.IsSuccessStatusCode)
                            {
                                // response code does not indicate success.Mark the node inactive
                                _memberNodesRepository.AddOrUpdateDeactiveNode(_memberNodesRepository.MemberNodes.First(u => u.Uri == uri).Uri);
                            }
                        }
                    }
                    //health check for off-line nodes
                    if (_memberNodesRepository.MemberNodes.Any(u => u.IsActive == false))
                    {
                        offlineNodes.AddRange(_memberNodesRepository.MemberNodes.Where(u => u.IsActive == false).Select(x => x.Uri));
                        foreach (Uri uri in offlineNodes)
                        {
                            HttpResponseMessage response = await HttpUtils.PostAsync(_httpClientFactory, new Uri($"{uri}/backplane/api/v1.0/addmembernode"), new Node() { Uri = _nodeRegistrationClient.CurrentNodeUri },httpRequestTimeOutInMs);
                            if (response.IsSuccessStatusCode)
                            {
                                // response code indicate success.Mark the node active
                                _memberNodesRepository.AddOrUpdateActiveNode(_memberNodesRepository.MemberNodes.First(u => u.Uri == uri).Uri);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Exception occurred in member backplane health check service. Message: {ex}");
                }
                //wait for health check interval.
                await Task.Delay(healthCheckIntervalMs, _cancellationToken);
                onlineNodes.Clear();
                offlineNodes.Clear();
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("member backplane health check service stopped.");
            _hostingUtils.BackplaneStart -= OnBackplaneRunning;
            await base.StopAsync(stoppingToken);
        }
    }
}
