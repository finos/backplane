/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.DTO.Envelope;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane
{
    /// <summary>
    /// SignalR hub
    /// </summary>
    public interface IDesktopAgentHub
    {
        /// <summary>
        /// Invoked by local connected clients.Hence original message source is this node.
        /// This would also result in propagation of message to other member nodes
        /// </summary>
        /// <param name="messageEnvelope"></param>
        /// <returns></returns>
        Task Broadcast(MessageEnvelope broadcastContextDTO);

        /// <summary>
        /// Invoked by member node. Hence original message source is not this node
        /// </summary>
        /// <param name="messageEnvelope"></param>
        /// <returns></returns>
        Task BroadcastToLocalClients(MessageEnvelope messageEnvelope);
    }
}
