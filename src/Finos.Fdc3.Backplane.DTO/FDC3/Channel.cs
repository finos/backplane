/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using Newtonsoft.Json;

namespace Finos.Fdc3.Backplane.DTO.FDC3
{
    /// <summary>
    /// Object representing a context channel.
    /// </summary>
    public class Channel
    {
        /// <summary>
        /// Constant that uniquely identifies this channel.
        /// </summary>
        [JsonProperty("id", Required = Required.Always)]
        public string Id { get; }

        /// <summary>
        ///  Uniquely defines each channel type.
        ///  Can be "user", "app" or "private".
        /// </summary>
        [JsonProperty("type", Required = Required.Always)]
        public string Type { get; }

        /// <summary>
        /// Channels may be visualized and selectable by users. DisplayMetadata may be used to provide hints on how to see them.
        /// </summary>
        [JsonProperty("displayMetadata")]
        public DisplayMetadata DisplayMetadata { get; }

        /// <summary>
        /// Creates channel object
        /// </summary>
        /// <param name="id">Constant that uniquely identifies this channel.</param>
        /// <param name="type">Uniquely defines each channel type.</param>
        /// <param name="displayMetadata">Channels may be visualized and selectable by users. DisplayMetadata may be used to provide hints on how to see them.</param>
        public Channel(string id, string type, DisplayMetadata displayMetadata = null)
        {
            Id = id;
            Type = type;
            DisplayMetadata = displayMetadata;
        }
    }
}
