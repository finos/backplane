/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.DTO.FDC3;
using System;

namespace Finos.Fdc3.Backplane.Client
{
    /// <summary>
    /// 
    /// </summary>
    public class InitializeParams
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="desktopAgentIdentifier"></param>
        public InitializeParams(Uri url, AppIdentifier desktopAgentIdentifier)
        {
            Url = url;
            AppIdentifier = desktopAgentIdentifier;
        }

        /// <summary>
        /// App/Desktop agent identifier
        /// </summary>
        public AppIdentifier AppIdentifier { get; }

        /// <summary>
        /// Backplane url to connect.
        /// </summary>
        public Uri Url { get; }

    }
}
