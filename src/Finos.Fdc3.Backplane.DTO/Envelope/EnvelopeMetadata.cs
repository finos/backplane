/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.DTO.FDC3;
using Newtonsoft.Json;

namespace Finos.Fdc3.Backplane.DTO.Envelope
{
    /// <summary>
    /// Meta data 
    /// </summary>
    public class EnvelopeMetadata
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
}
