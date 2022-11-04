/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.Client.Discovery;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;

[assembly: InternalsVisibleTo("Finos.Fdc3.Backplane.Client.Test")]
namespace Finos.Fdc3.Backplane.Client.Transport
{
    internal class SignalRConnectionFactory : IConnectionFactory
    {
        private readonly IServiceProvider _provider;
        private readonly IBackplaneDiscoveryServiceClient _backplaneDiscoveryService;

        public SignalRConnectionFactory(IServiceProvider provider)
        {
            _provider = provider;
            _backplaneDiscoveryService = provider.GetService<IBackplaneDiscoveryServiceClient>();
        }

        public async Task<IConnection> Create(CancellationToken ct = default)
        {
            ILogger<IConnection> logger = _provider.GetService<ILogger<IConnection>>();
            IConnection connection = new SignalRConnection(logger);
            string hubUrl = await GetHubUrl(ct);
            connection.Build(hubUrl, ct);
            return connection;

        }

        private async Task<string> GetHubUrl(CancellationToken ct)
        {
            Uri backplaneAddress = await _backplaneDiscoveryService.DiscoverAsync(ct);
            return $"{backplaneAddress}backplane/v1.0";
        }
    }
}

