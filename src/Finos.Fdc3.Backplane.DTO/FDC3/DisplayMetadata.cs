/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using Newtonsoft.Json;

namespace Finos.Fdc3.Backplane.DTO.FDC3
{
    /// <summary>
    /// A user channel will be global enough to have a presence across many apps. This gives us some hints
    /// to render them in a standard way.It is assumed it may have other properties too, but if it has these,
    /// this is their meaning.
    /// </summary>
    public class DisplayMetadata
    {
        /// <summary>
        /// A user-readable name for this channel, e.g: `"Red"`
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; }

        /// <summary>
        /// The color that should be associated within this channel when displaying this channel in a UI, e.g: `0xFF0000`.
        /// </summary>
        [JsonProperty("color")]
        public string Color { get; }

        /// <summary>
        /// A URL of an image that can be used to display this channel
        /// </summary>
        [JsonProperty("glyph")]
        public string Glyph { get; }

        /// <summary>
        /// Creates DisplayMetadata object to associate with channel.
        /// </summary>
        /// <param name="name">A user-readable name for this channel, e.g: `"Red"`</param>
        /// <param name="color">The color that should be associated within this channel when displaying this channel in a UI, e.g: `0xFF0000`.</param>
        /// <param name="glyph"> A URL of an image that can be used to display this channel</param>
        public DisplayMetadata(string name, string color = null, string glyph = null)
        {
            Name = name;
            Color = color;
            Glyph = glyph;
        }

    }
}
