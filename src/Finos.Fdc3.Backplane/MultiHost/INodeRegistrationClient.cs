/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using System;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.MultiHost
{
    /// <summary>
    /// Register backplane for discovery by other member nodes.
    /// </summary>
    public interface INodeRegistrationClient
    {
        /// <summary>
        /// Current node uri
        /// </summary>
        Uri CurrentNodeUri { get; }

        /// <summary>
        /// Register the uri of backplane
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        Task RegisterAsync(Uri uri, CancellationToken ct = default);
    }
}
