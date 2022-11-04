/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.Client.API;
using Finos.Fdc3.Backplane.Client.Concurrency;
using Finos.Fdc3.Backplane.Client.Discovery;
using Finos.Fdc3.Backplane.Client.Middleware;
using Finos.Fdc3.Backplane.Client.Resilliency;
using Finos.Fdc3.Backplane.Client.Transport;
using Microsoft.Extensions.DependencyInjection;

namespace Finos.Fdc3.Backplane.Client.Extensions
{
    /// <summary>
    /// Extension over IServiceCollection for DI 
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Registers required dependencies for desktop agent client
        /// </summary>
        /// <param name="serviceCollection"></param>
        public static void ConfigureBackplaneClient(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddSingleton<IRetryPolicyProvider, RetryPolicyProvider>();
            serviceCollection.AddTransient<IClientMiddleware, ClientMiddleware>();
            serviceCollection.AddTransient<IBackplaneTransport, BackplaneTransport>();
            serviceCollection.AddTransient<IConnection, SignalRConnection>();
            serviceCollection.AddSingleton<IConnectionFactory, SignalRConnectionFactory>(cd =>
            {
                ServiceProvider provider = serviceCollection.BuildServiceProvider();
                return new SignalRConnectionFactory(provider);
            });
            serviceCollection.AddSingleton<ISchedulerProvider, SchedulerProvider>();
            serviceCollection.AddSingleton<IBackplaneDiscoveryServiceClient, BackplaneDiscoveryServiceClient>();
            serviceCollection.AddTransient<IBackplaneClient, BackplaneClient>();

        }
    }
}
