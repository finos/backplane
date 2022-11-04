using Finos.Fdc3.Backplane.MultiHost;
using Microsoft.AspNetCore.Hosting.Server;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;

namespace Finos.Fdc3.Backplane.Utils
{
    /// <summary>
    /// Hosting utils
    /// </summary>
    public class HostingUtils : IHostingUtils
    {
        private readonly ILogger _logger;
        private readonly INodeRegistrationClient _nodeRegistrationClient;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostApplicationLifetime _lifetime;

        public HostingUtils(INodeRegistrationClient nodeRegistrationClient, ILogger<IHostingUtils> logger, IServiceProvider serviceProvider, IHostApplicationLifetime lifetime)
        {
            _nodeRegistrationClient = nodeRegistrationClient;
            _serviceProvider = serviceProvider;
            _lifetime = lifetime;
            _logger = logger;

        }

        /// <summary>
        /// Event to notify the backplane is up and running.
        /// </summary>
        public event Action BackplaneStart;

        /// <summary>
        /// Trigger registration of backplane when, its up and running.
        /// </summary>
        /// <returns></returns>
        public CancellationTokenRegistration RegisterBackplane()
        {
            return _lifetime.ApplicationStarted.Register(async () =>
             {
                 //Notify backplane is running..
               
                 try
                 {
                     IServer server = _serviceProvider.GetRequiredService<IServer>();
                     IServerAddressesFeature addressFeature = server.Features.Get<IServerAddressesFeature>();
                     foreach (string addresses in addressFeature.Addresses)
                     {
                         _logger.LogInformation("Listening on address: " + addresses);
                     }

                     if (addressFeature.Addresses.Count == 1)
                     {
                         Uri backplaneUri = new Uri(addressFeature.Addresses.First());
                         backplaneUri = ReplaceHost(backplaneUri.OriginalString, Environment.MachineName);
                         await _nodeRegistrationClient.RegisterAsync(backplaneUri);
                         BackplaneStart?.Invoke();
                         _logger.LogInformation("Listening on hosted address: " + backplaneUri);
                     }
                     else
                     {
                         _logger.LogError("Invalid State!.Multiple addresses exposed by server. Aborting service registration");
                     }

                 }
                 catch (Exception ex)
                 {
                     _logger.LogError(ex, $"Service registration failed.");
                 }
             });

        }

        public Uri ReplaceHost(string original, string newHostName)
        {
            UriBuilder builder = new UriBuilder(original)
            {
                Host = newHostName
            };
            return builder.Uri;
        }
    }
}
