/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/


using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.MultiHost
{
    /// <summary>
    /// Template implementation of registration interface. 
    /// Please use your own registry mechanism here.
    /// </summary>
    public class NodeRegistrationClient : INodeRegistrationClient
    {
        private readonly ILogger<INodeRegistrationClient> _logger;


        public NodeRegistrationClient(ILogger<INodeRegistrationClient> logger)
        {
            _logger = logger;
        }

        /// <summary>
        /// Current node uri
        /// </summary>
        public Uri CurrentNodeUri { get; private set; }

        /// <summary>
        /// Register node for discovery. 
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public async Task RegisterAsync(Uri uri, CancellationToken ct = default)
        {
            CurrentNodeUri = uri;
            // write logic here to register this backplane to persistent storage and that can further be queried. Like service discovery etc. 
            // Since multi host interop is limited to DA running in context of same user, User name could be key in registration.
            _logger.LogInformation($"Service Registration Complete: Address:{Environment.MachineName} for user: {Environment.UserName}");
            await Task.CompletedTask;
        }
    }
}
