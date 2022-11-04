/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.Models.Config;
using Finos.Fdc3.Backplane.Utils;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using NLog;
using NLog.Web;
using System;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane
{
    public class Program
    {
        private static SingleInstance _singleInstance;
        private static Logger _logger;
        public static async Task Main(string[] args)
        {
            //Init logger.
            InitLogger();
            // single instance check.
            _singleInstance = new SingleInstance();

            if (_singleInstance.IsAlreadyRunning)
            {
                _logger.Fatal($"Another instance of backplane service already running. Exiting the current instance..");
                _singleInstance.Dispose();
                LogManager.Shutdown();
                Environment.Exit(-10);
            }

            await Init(args);
        }
        private static void InitLogger()
        {
            _logger = LogManager.GetCurrentClassLogger();
        }

        public static async Task Init(string[] args)
        {
            try
            {
                //setup app settings.
                IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.json", optional: false).Build();
                //Read port from app.settings.
                PortsConfig portsList = new PortsConfig();
                config.GetSection($"PortsConfig").Bind(portsList);
                foreach (int port in portsList.Ports)
                {
                    try
                    {
                        using IWebHost host = CreateWebHostBuilder(args, port).Build();
                        IHostingUtils hostingUtil = (IHostingUtils)host.Services.GetService(typeof(IHostingUtils));
                        hostingUtil.RegisterBackplane();
                        _logger.Info("Backplane service started...");
                        await host.RunAsync();
                        _logger.Info("Backplane service stopped...");
                    }
                    catch (Exception exception)
                    {
                        _logger.Error(exception, $"An exception occurred while starting the backplane services on port: {port}");
                    }
                }
            }
            catch (Exception exception)
            {
                _logger.Error(exception, "An exception occurred while starting the services.");
                throw;
            }
            finally
            {
                //dispose below once if all of defined ports are tried or program exiting.
                _singleInstance.Dispose();
                LogManager.Shutdown();
            }
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, int port)
        {
            return WebHost.CreateDefaultBuilder(args)
                    .ConfigureAppConfiguration((hostingContext, config) =>
                    {
                        config.AddEnvironmentVariables();
                        config.AddCommandLine(args);
                    })
                    .UseStartup<Startup>()
                    .UseNLog()
                    .UseKestrel(options => options.ListenAnyIP(port));
        }
    }
}
