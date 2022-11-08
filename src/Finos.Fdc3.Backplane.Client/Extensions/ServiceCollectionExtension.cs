/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.Client.API;
using Finos.Fdc3.Backplane.Client.Transport;
using Finos.Fdc3.Backplane.DTO.Envelope;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.Extensions
{
    /// <summary>
    /// Extension over IServiceCollection for DI 
    /// </summary>
    public static class ServiceCollectionExtension
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="serviceCollection"></param>
        /// <param name="initializeParams"></param>
        /// <param name="urlProvider"></param>
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
