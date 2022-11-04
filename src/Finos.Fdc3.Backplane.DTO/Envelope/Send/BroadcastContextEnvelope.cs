/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Finos.Fdc3.Backplane.DTO.Envelope.Send
{
    /// <summary>
    /// The message which is sent by client to backplane 
    /// as broadcast payload
    /// </summary>
    public class BroadcastContextEnvelope
    {
        /// <summary>
        /// Context channel id
        /// </summary>
        [JsonProperty("channelId", Required = Required.Always)]
        public string ChannelId { get; set; }

        /// <summary>
        /// Context json
        /// </summary>
        [JsonProperty("context", Required = Required.Always)]
        public JObject Context { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("meta", Required = Required.Always)]
        public EnvelopeMetadata Metadata { get; set; }

    }
}
