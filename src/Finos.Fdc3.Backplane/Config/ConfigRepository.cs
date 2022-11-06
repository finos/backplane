/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/
using Finos.Fdc3.Backplane.DTO.FDC3;
using Finos.Fdc3.Backplane.Models.Config;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Finos.Fdc3.Backplane.Config
{
    /// <summary>
    /// Configurations 
    /// </summary>
    public class ConfigRepository : IConfigRepository
    {
        private readonly ILogger<ConfigRepository> _logger;
        private readonly IConfiguration _config;

        /// <summary>
        /// List of system channels
        /// </summary>
        public List<Channel> ChannelsList { get; private set; }
        public TimeSpan HttpRequestTimeoutInMilliseconds { get; private set; }
        public TimeSpan MemberNodesHealthCheckIntervalInMilliseconds { get; private set; }
        public string HubEndpoint { get; private set; }
        public string AddNodeEndpoint { get; private set; }

        public ConfigRepository(ILogger<ConfigRepository> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            ChannelsList = new List<Channel>();
            PopulatePropertiesFromConfig();
        }
        private void PopulatePropertiesFromConfig()
        {
            IEnumerable<ChannelConfig> systemChannelsConfig = _config.GetSection("ChannelsConfig:SystemChannels").Get<IEnumerable<ChannelConfig>>();
            IEnumerable<Channel> systemChannels = systemChannelsConfig.Select(x => new Channel(x.Id, x.Type, new DisplayMetadata(x.Name, x.Color, x.Glyph)));
            ChannelsList.AddRange(systemChannels);
            _logger.LogInformation($"Populated system channels from config: {string.Join(",", ChannelsList.Select(x => x.Id))}");
            HttpRequestTimeoutInMilliseconds = TimeSpan.FromMilliseconds(_config.GetValue<int>("HttpRequestTimeoutInMilliseconds", 5000));
            MemberNodesHealthCheckIntervalInMilliseconds = TimeSpan.FromMilliseconds(_config.GetValue<int>("MemberNodesHealthCheckIntervalInMilliseconds", 5000));
            HubEndpoint = _config.GetValue<string>("HubEndpoint");
            AddNodeEndpoint = _config.GetValue<string>("AddNodeEndpoint");
        }

    }
}
