/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.Config;
using Finos.Fdc3.Backplane.Hubs;
using Finos.Fdc3.Backplane.MultiHost;
using Finos.Fdc3.Backplane.Utils;
using Finos.Fdc3.Backplane.WorkerService;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Net;

namespace Finos.Fdc3.Backplane
{
    public class Startup
    {
        private readonly ILogger<Startup> _logger;
        public Startup(IConfiguration configuration, ILogger<Startup> logger)
        {
            Configuration = configuration;
            _logger = logger;
        }

        private IConfiguration Configuration { get; }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddHttpClient("Backplane", (httpConfig) =>
             {
                 httpConfig.Timeout = TimeSpan.FromMilliseconds(Configuration.GetValue<int>("HttpRequestTimeoutInMilliseconds"));
             }
            );
            services.AddApiVersioning(x =>
            {
                // If the client hasn't specified the API version in the request, use the default API version number 
                x.AssumeDefaultVersionWhenUnspecified = true;
                x.ReportApiVersions = true;
            });
            services.AddLogging(configure => { configure.AddConsole(); configure.SetMinimumLevel(LogLevel.Information); });
            services.AddTransient<IDesktopAgentHub, DesktopAgentsHub>();
            services.AddControllers();
            services.AddSignalR(hubOptions =>
            {
                hubOptions.EnableDetailedErrors = true;
            }).AddNewtonsoftJsonProtocol((config) => config.PayloadSerializerSettings = new Newtonsoft.Json.JsonSerializerSettings() { TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Auto });
            services.AddControllers().AddNewtonsoftJson();
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", corsPolicyBuilder =>
                {
                    corsPolicyBuilder.SetIsOriginAllowed((host) => true)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials();
                });
            });
            // Register services here.
            services.AddSingleton<IHostingUtils, HostingUtils>();
            services.AddSingleton<IConfigRepository, ConfigRepository>();
            services.AddSingleton<INodesRepository, NodesRepository>();
            services.AddSingleton<INodeRegistrationClient, NodeRegistrationClient>();
            services.AddTransient<INodesDiscoveryClient, NodesDiscoveryClient>();
            services.AddHostedService<MemberNodesHealthCheckService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IServiceProvider serviceProvider, IApplicationBuilder app, IWebHostEnvironment env, IHostApplicationLifetime hostApplicationLifetime, INodeRegistrationClient registrationService)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseExceptionHandler(a => a.Run(async context =>
            {
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/json";
                IExceptionHandlerFeature contextFeature = context.Features.Get<IExceptionHandlerFeature>();
                if (contextFeature != null)
                {
                    _logger.LogError($"Unhandled fatal error {contextFeature.Error}");
                    await context.Response.WriteAsync(new
                    {
                        context.Response.StatusCode,
                        Message = "Internal Server Error."
                    }.ToString());
                }
            }));
            app.UseCors("CorsPolicy");
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<DesktopAgentsHub>(Configuration.GetValue<string>("HubEndPoint"));
            });
        }

    }
}
