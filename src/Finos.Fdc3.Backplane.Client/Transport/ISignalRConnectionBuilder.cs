/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using System;

namespace Finos.Fdc3.Backplane.Client.Transport
{
    /// <summary>
    /// SignalR connection builder
    /// </summary>
    public interface ISignalRConnectionBuilder
    {
        /// <summary>
        /// Build connection
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        ISignalRConnection Build(Uri uri);
    }
}
