/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
*/


using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Finos.Fdc3.Backplane.DTO.Envelope.Receive
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
        [JsonProperty("type")]
        public Fdc3Action ActionType { get; set; }

        /// <summary>
        /// Wraps Fdc3 data.
        /// </summary>
        [JsonProperty("payload")]
        public EnvelopeData Payload { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [JsonProperty("meta", Required = Required.Always)]
        public EnvelopeMetadata Meta { get; set; }

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
