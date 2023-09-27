/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/
using Finos.Fdc3.Backplane.DTO;
using Finos.Fdc3.Backplane.Models.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Finos.Fdc3.Backplane.Config
{
    /// <summary>
    /// Backplane configurations 
    /// </summary>
    public class ConfigRepository : IConfigRepository
    {
        private readonly ILogger<ConfigRepository> _logger;
        private readonly IConfiguration _config;
        private readonly List<Channel> _channels;
        private readonly List<Uri> _memberNodes;

        /// <summary>
        /// List of 8 recommended user channels populated from config.
        /// </summary>

        public IEnumerable<Channel> Channels => _channels;

        /// <summary>
        /// HTTP request timeout in ms
        /// </summary>
        public TimeSpan HttpRequestTimeoutInMilliseconds { get; private set; }

        /// <summary>
        /// In multihost scenario, member nodes health check interval in ms.
        /// </summary>
        public TimeSpan MemberNodesHealthCheckIntervalInMilliseconds { get; private set; }

        /// <summary>
        /// SignalR hub end point
        /// </summary>
        public string HubEndpoint { get; private set; }

        /// <summary>
        /// REST endpoint to register node in cluster in multihost scenario.
        /// </summary>
        public string AddNodeEndpoint { get; private set; }

        /// <summary>
        /// REST endpoint to propagate message to other member node in multihost scenario.
        /// </summary>
        public string BroadcastEndpoint { get; private set; }

        /// <summary>
        /// Multihost scenario: member nodes defined in config statically
        /// </summary>
        public IEnumerable<Uri> MemberNodes => _memberNodes;


        public ConfigRepository(ILogger<ConfigRepository> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            _channels = new List<Channel>();
            _memberNodes = new List<Uri>();
            PopulatePropertiesFromConfig();
        }
        private void PopulatePropertiesFromConfig()
        {
            IEnumerable<Uri> memberNodesFromConfig = _config.GetSection("MultiHostConfig:MemberNodes").Get<IEnumerable<Uri>>();
            _memberNodes.AddRange(memberNodesFromConfig);
            IEnumerable<ChannelConfig> userChannelsConfig = _config.GetSection("ChannelsConfig:UserChannels").Get<IEnumerable<ChannelConfig>>();
            IEnumerable<Channel> userChannels = userChannelsConfig.Select(x => new Channel() { Id = x.Id, Type = (TypeEnum)Enum.Parse(typeof(TypeEnum), x.Type), DisplayMetadata = new DisplayMetadata() { Name = x.Name, Color = x.Color, Glyph = x.Glyph } });
            _channels.AddRange(userChannels);
            _logger.LogInformation($"Populated user channels from config: {string.Join(",", Channels.Select(x => x.Id))}");
            HttpRequestTimeoutInMilliseconds = TimeSpan.FromMilliseconds(_config.GetValue<int>("HttpRequestTimeoutInMilliseconds", 5000));
            MemberNodesHealthCheckIntervalInMilliseconds = TimeSpan.FromMilliseconds(_config.GetValue<int>("MemberNodesHealthCheckIntervalInMilliseconds", 5000));
            HubEndpoint = _config.GetValue<string>("HubEndpoint");
            AddNodeEndpoint = _config.GetValue<string>("AddNodeEndpoint");
            BroadcastEndpoint = _config.GetValue<string>("BroadcastEndpoint");
        }

    }
}
