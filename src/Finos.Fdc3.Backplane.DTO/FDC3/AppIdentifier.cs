/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using Newtonsoft.Json;


namespace Finos.Fdc3.Backplane.DTO.FDC3
{
    /// <summary>
    /// App Identifier
    /// </summary>
    public class AppIdentifier
    {
        /// <summary>
        /// App identifier
        /// </summary>
        [JsonProperty("appId", Required = Required.Always)]
        public string AppId { get; set; }

        /// <summary>
        /// Instance id
        /// </summary>
        [JsonProperty("instanceId")]
        public string InstanceId { get; set; }

        /// <summary>
        /// Field that represents the Desktop Agent that the app is available on.
        /// </summary>
        [JsonProperty("desktopAgent")]
        public string DesktopAgent { get; set; }
    }
}