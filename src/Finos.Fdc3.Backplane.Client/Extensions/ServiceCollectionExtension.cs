/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.Client.API;
using Finos.Fdc3.Backplane.Client.Transport;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Finos.Fdc3.Backplane.Client.Extensions
{
    /// <summary>
    /// Extension over IServiceCollection for configuring backplane client . 
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// Configure backplane client with required services and parameters
        /// </summary>
        /// <param name="serviceCollection">Service collections</param>
        /// <param name="initializeParams">parameters, ex: client identifier.</param>
        /// <param name="urlProvider">delegate providing url of backplane to connect</param>
        public static void ConfigureBackplaneClient(this IServiceCollection serviceCollection, InitializeParams initializeParams, Func<Uri> urlProvider)
        {
            serviceCollection.AddTransient<IBackplaneClient, BackplaneClient>();
            serviceCollection.AddTransient<IBackplaneTransport, SignalRBackplaneTransport>()
            .AddTransient(x => new Lazy<IBackplaneTransport>(
            () =>
            {
                ServiceProvider provider = serviceCollection.BuildServiceProvider();
                return new SignalRBackplaneTransport(provider, initializeParams, urlProvider);
            }));


        }
    }
}
