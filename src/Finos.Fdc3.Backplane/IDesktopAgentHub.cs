/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.DTO.Envelope;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane
{
    /// <summary>
    /// SignalR Hubs API enables connected clients to call methods on the server. 
    /// The server defines methods that are called from the client and the client defines methods 
    /// that are called from the server. 
    /// SignalR takes care of everything required to make real-time client-to-server and server-to-client communication possible. 
    /// </summary>
    public interface IDesktopAgentHub
    {
        /// <summary>
        /// Broadcast context to local connected client.
        /// It does not propagate message to member nodes.
        /// This is called when original source of message is other backplane.
        /// Multihost scenario:[Host A] DA1 => Backplane --REST-- [Host B] Backplane => DA2
        /// </summary>
        /// <param name="messageEnvelope">Message DTO</param>
        /// <returns></returns>
        Task Broadcast(MessageEnvelope broadcastContextDTO);

        /// <summary>
        /// Broadcast context to connected clients as well as to other member nodes of cluster over HTTP
        /// </summary>
        /// <param name="messageEnvelope">Message DTO</param>
        /// <returns></returns>
        Task BroadcastToLocalClients(MessageEnvelope messageEnvelope);
    }
}
