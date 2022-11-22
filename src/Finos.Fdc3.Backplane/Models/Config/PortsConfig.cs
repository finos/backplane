/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/
namespace Finos.Fdc3.Backplane.Models.Config
{
    /// <summary>
    /// Backplane ports config model
    /// </summary>
    public class PortsConfig
    {
        /// <summary>
        /// Port range
        /// </summary>
        public int[] Ports { get; set; }
    }
}
