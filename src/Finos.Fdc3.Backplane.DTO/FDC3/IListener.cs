using System.Threading;
using System.Threading.Tasks;
/**
* SPDX-License-Identifier: Apache-2.0
* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
*/
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