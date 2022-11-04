/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.DTO.Envelope.Send;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Core
{
    /// <summary>
    /// Interface for backplane Broadcast/RaiseIntent function.
    /// </summary>
    public interface IDesktopAgentHub
    {
        Task Broadcast(BroadcastContextEnvelope broadcastContextDTO, bool isMessageTobePropagatedToOtherNodes = false);
    }
}
