/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
*/


using Finos.Fdc3.Backplane.DTO.FDC3;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;

namespace Finos.Fdc3.Backplane.DTO.Envelope
{
    /// <summary>
    /// The message which is sent by backplane to clients.
    /// Clients listen this message on transport
    /// It wraps FDC3 data like intent,context etc.
    /// </summary>
    public class MessageEnvelope
    {
        /// <summary>
        /// Fdc3 operation enum: RaiseIntent, Broadcast, Open
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter), typeof(CamelCaseNamingStrategy))]
        [JsonProperty("type", Required = Required.Always)]
        public Fdc3Action ActionType { get; set; }

        /// <summary>
        /// Wraps Fdc3 data.
        /// </summary>
        [JsonProperty("payload", Required = Required.Always)]
        public EnvelopeData Payload { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("meta", Required = Required.Always)]
        public EnvelopeMeta Meta { get; set; }

    }

    /// <summary>
    /// Fdc3 data envelope
    /// </summary>
    public class EnvelopeData
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
    }

    /// <summary>
    /// Meta data 
    /// </summary>
    public class EnvelopeMeta
    {
        /// <summary>
        /// Sender details
        /// </summary>
        [JsonProperty("source", Required = Required.Always)]
        public AppIdentifier Source { get; set; }

        /// <summary>
        /// This is unique id of message which would be set by sender and transfered as is to destination.
        /// Useful in tracing messages.
        /// </summary>
        /// 
        [JsonProperty("uniqueMessageId", Required = Required.Always)]
        public string UniqueMessageId { get; set; }
    }

    /// <summary>
    /// Enum denoting Fdc3 operation
    /// </summary>
    public enum Fdc3Action
    {
        /// <summary>
        /// Broadcast context
        /// </summary>
        Broadcast

    }

}
