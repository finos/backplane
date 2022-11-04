/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Core.MultiHost
{
    /// <summary>
    /// This is interface to register backplane when it is hosted on any user desktop. Helps later in backplane member discovery.
    /// </summary>
    public interface INodeRegistrationClient
    {
        /// <summary>
        /// Current node uri
        /// </summary>
        Uri CurrentNodeUri { get; }
        Task<bool> RegisterAsync(Uri uri, CancellationToken ct = default);
    }
}
