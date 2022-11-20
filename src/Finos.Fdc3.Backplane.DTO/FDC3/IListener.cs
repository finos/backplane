/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.DTO.FDC3
{
    /// <summary>
    /// Object that provide handle to unsubscribe as part of add listener.
    /// </summary>
    public interface IListener
    {
        /// <summary>
        ///  Unsubscribe the listener object.
        /// </summary>
        Task UnsubscribeAsync(CancellationToken ct = default);
    }
}