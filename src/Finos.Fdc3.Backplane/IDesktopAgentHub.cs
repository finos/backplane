/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.DTO.Envelope.Send;
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
        /// <param name="isMessageTobePropagatedToOtherNodes"></param>
        /// <returns></returns>
        Task Broadcast(BroadcastContextEnvelope broadcastContextDTO, bool isMessageTobePropagatedToOtherNodes = false);
    }
}
