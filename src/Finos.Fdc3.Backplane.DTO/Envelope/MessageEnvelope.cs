/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
*/


using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace Finos.Fdc3.Backplane.DTO.Envelope
{
    /// <summary>
    /// The message object shared between backplane and clients.
    /// </summary>
    public class MessageEnvelope
    {
        /// <summary>
        /// Identifier used to declare what aspect of FDC3 that the message relates to.e.g. "broadcast"
        /// </summary>
        [JsonConverter(typeof(StringEnumConverter), typeof(CamelCaseNamingStrategy))]
        [JsonProperty("type", Required = Required.Always)]
        public Fdc3Action ActionType { get; set; }

        /// <summary>
        /// Request body, containing any the arguments to the FDC3 interactions.
        /// </summary>
        [JsonProperty("payload", Required = Required.Always)]
        public EnvelopeData Payload { get; set; }

        /// <summary>
        /// Metadata relating to the message, its sender and destination.
        /// </summary>
        [JsonProperty("meta", Required = Required.Always)]
        public EnvelopeMeta Meta { get; set; }

    }

    /// <summary>
    ///  Request body, containing any the arguments to the FDC3 interactions.
    /// </summary>
    public class EnvelopeData
    {
        /// <summary>
        ///Used to indicate which channel `broadcast` functions were called on.
        /// </summary>
        [JsonProperty("channelId", Required = Required.Always)]
        public string ChannelId { get; set; }

        /// <summary>
        /// Used as an argument to `broadcast`, `findIntent` and `raiseIntent` functions.
        /// </summary>
        [JsonProperty("context", Required = Required.Always)]
        public Context Context { get; set; }
    }

    /// <summary>
    /// Metadata relating to the message, its sender and destination.
    /// </summary>
    public class EnvelopeMeta
    {
        /// <summary>
        /// AppIdentifier for the source application that the request was received from.
        /// </summary>
        [JsonProperty("source", Required = Required.Always)]
        public AppIdentifier Source { get; set; }

        /// <summary>
        /// Unique GUID for this request.
        /// </summary>
        [JsonProperty("requestGuid", Required = Required.Always)]
        public string RequestGuid { get; set; }
    }

    /// <summary>
    /// Identifier used to declare what aspect of FDC3 that the message relates to.e.g. "broadcast"
    /// </summary>
    public enum Fdc3Action
    {
        /// <summary>
        /// Broadcast context
        /// </summary>
        Broadcast

    }

}
