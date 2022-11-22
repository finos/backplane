/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/
    
using System;
using System.Threading;

namespace Finos.Fdc3.Backplane.Utils
{
    /// <summary>
    /// Utility functions related to hosting
    /// </summary>
    public interface IHostingUtils
    {
        // <summary>
        /// Replace uri with host name
        /// </summary>
        /// <param name="original"></param>
        /// <param name="newHostName"></param>
        /// <returns></returns>
        Uri ReplaceHost(string original, string newHostName);

        /// <summary>
        /// Wait for backplane to start and then registers url.
        /// </summary>
        /// <returns></returns>
        CancellationTokenRegistration RegisterBackplane();

        /// <summary>
        /// Even fired after successfull startup of backplane 
        /// </summary>
        event Action BackplaneStarted;
    }
}