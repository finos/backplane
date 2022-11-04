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

        public List<Channel> ChannelsList { get; private set; }
        public TimeSpan HttpRequestTimeoutInMilliSeconds { get; private set; }
        public TimeSpan BackplaneRegistrationRetryIntervalInSeconds { get; private set; }
        public TimeSpan MemberNodesHealthCheckIntervalInMilliSeconds { get; private set; }


        public ConfigRepository(ILogger<ConfigRepository> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
            ChannelsList = new List<Channel>();
            PopulatePropertiesFromConfig();
        }
        private void PopulatePropertiesFromConfig()
        {
            IEnumerable<ChannelConfig> systemChannels = _config.GetSection("ChannelsConfig:SystemChannels").Get<IEnumerable<ChannelConfig>>();
            IEnumerable<Channel> systemChannelsDto = systemChannels.Select(x => new Channel(x.Id, x.Type, new DisplayMetadata(x.Name, x.Color, x.Glyph)));
            ChannelsList.AddRange(systemChannelsDto);
            _logger.LogInformation($"Populated system channels from config: {string.Join(",", ChannelsList.Select(x => x.Id))}");
            HttpRequestTimeoutInMilliSeconds = TimeSpan.FromMilliseconds(int.Parse(_config["HttpRequestTimeoutInMilliSeconds"]));
            BackplaneRegistrationRetryIntervalInSeconds = TimeSpan.FromMinutes(int.Parse(_config["BackplaneRegistrationRetryIntervalInMinutes"]));
            MemberNodesHealthCheckIntervalInMilliSeconds = TimeSpan.FromMilliseconds(int.Parse(_config["MemberNodesHealthCheckIntervalInMilliSeconds"]));
        }


    }
}
