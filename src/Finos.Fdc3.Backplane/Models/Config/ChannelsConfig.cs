/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/
namespace Finos.Fdc3.Backplane.Models.Config
{
    /// <summary>
    /// Fdc3 channels model
    /// </summary>
    public class ChannelConfig
    {
        /// <summary>
        /// channel id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// channel name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// channel type
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// channel color
        /// </summary>
        public string Color { get; set; }

        /// <summary>
        /// channel glyph.
        /// </summary>
        public string Glyph { get; set; }
    }
}

