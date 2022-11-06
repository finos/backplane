/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/


using Finos.Fdc3.Backplane.Client.Extensions;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;

namespace Finos.Fdc3.Backplane.Client.Transport
{
    /// <summary>
    /// SignalR connection builder
    /// </summary>
    internal class SignalRConnectionBuilder : ISignalRConnectionBuilder
    {
        private readonly ILogger<ISignalRConnectionBuilder> _logger;

        public SignalRConnectionBuilder(ILogger<ISignalRConnectionBuilder> logger)
        {
            _logger = logger;
        }
        public ISignalRConnection Build(Uri uri)
        {
            HubConnection hubConnection = new HubConnectionBuilder()
                 .WithUrl(uri).AddNewtonsoftJsonProtocol()
                 .ConfigureLogging(logging =>
                 {
                     logging.AddProvider(_logger.AsLoggerProvider());
                 }).Build();
            return new SignalRConnection(hubConnection);
        }
    }
}
