/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.DTO.Envelope.Receive;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane
{
    /// <summary>
    /// SignalR hub
    /// </summary>
    public interface IDesktopAgentHub
    {
        /// <summary>
        /// Broadcast
        /// </summary>
        /// <param name="broadcastContextDTO"></param>
        /// <param name="isMessageOriginatedFromCurrentNode"></param>
        /// <returns></returns>
        Task Broadcast(MessageEnvelope broadcastContextDTO, bool isMessageOriginatedFromCurrentNode = false);
    }
}
