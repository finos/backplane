/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.Config;
using Finos.Fdc3.Backplane.DTO;
using Finos.Fdc3.Backplane.DTO.Envelope;
using Finos.Fdc3.Backplane.MultiHost;
using Finos.Fdc3.Backplane.Utils;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Hubs
{
    /// <summary>
    /// SignalR Hubs API enables connected clients to call methods on the server. 
    /// The server defines methods that are called from the client and the client defines methods 
    /// that are called from the server. 
    /// SignalR takes care of everything required to make real-time client-to-server and server-to-client communication possible. 
    /// </summary>
    public class DesktopAgentsHub : Hub, IDesktopAgentHub
    {
        private readonly IHubContext<DesktopAgentsHub> _hubContext;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly INodesRepository _memberNodesRepo;
        private readonly ILogger<DesktopAgentsHub> _logger;
        private readonly IConfigRepository _configRepository;
        private readonly TimeSpan _httpTimeOutInMs;
        private readonly string _broadcastPath;

        public DesktopAgentsHub(ILogger<DesktopAgentsHub> logger,
            IHubContext<DesktopAgentsHub> hubContext,
             IHttpClientFactory httpClientFactory,
            INodesRepository memberNodesRepo, IConfigRepository configRepository)
        {
            _hubContext = hubContext;
            _httpClientFactory = httpClientFactory;
            _memberNodesRepo = memberNodesRepo;
            _logger = logger;
            _configRepository = configRepository;
            _httpTimeOutInMs = configRepository.HttpRequestTimeoutInMilliseconds;
            _broadcastPath = configRepository.BroadcastEndpoint;
        }

        /// <summary>
        /// Broadcast context to local connected client.
        /// It does not propagate message to member nodes.
        /// This is called when original source of message is other backplane.
        /// Multihost scenario:[Host A] DA1 => Backplane --REST-- [Host B] Backplane => DA2
        /// </summary>
        /// <param name="messageEnvelope">Message DTO</param>
        /// <returns></returns>
        public async Task BroadcastToLocalClients(MessageEnvelope messageEnvelope)
        {
            await HandleBroadcast(messageEnvelope, false);
        }

        /// <summary>
        /// Broadcast context to connected clients as well as to other member nodes of cluster over HTTP
        /// </summary>
        /// <param name="messageEnvelope">Message DTO</param>
        /// <returns></returns>
        public async Task Broadcast(MessageEnvelope messageEnvelope)
        {
            await HandleBroadcast(messageEnvelope, true);
        }

        /// <summary>
        /// Get FDC3 recommended 8 user channels.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Channel>> GetUserChannels()
        {
            return await Task.FromResult(_configRepository.Channels);
        }

        /// <summary>
        /// Broadcast context to connected clients and member backplane nodes of cluster.
        /// </summary>
        /// <param name="messageEnvelope"></param>
        /// <param name="isMessageOriginatedFromCurrentNode">if client called broadcast over this node or it was called on member node and propagated here</param>
        /// <returns></returns>
        private async Task HandleBroadcast(MessageEnvelope messageEnvelope, bool isMessageOriginatedFromCurrentNode = true)
        {
            if (messageEnvelope == null)
            {
                _logger.LogError($"ExceptionCode:{(int)ResponseCodes.BroadcastPayloadInvalid} invalid parameter");
                throw new HubException($"ExceptionCode:{(int)ResponseCodes.BroadcastPayloadInvalid} invalid parameter.") { HResult = (int)ResponseCodes.BroadcastPayloadInvalid };
            }

            _logger.LogInformation($"Call received from signalR client: {Context?.ConnectionId}, message: {JsonConvert.SerializeObject(messageEnvelope)}");
            //broadcast to local clients
            await SendMessageToClients(messageEnvelope, isMessageOriginatedFromCurrentNode, "OnMessage");
            // propagate message to other nodes of backplane cluster
            if (isMessageOriginatedFromCurrentNode)
            {
                await PostMessageToMemberBackplanes(messageEnvelope);
            }
        }

        private async Task SendMessageToClients(MessageEnvelope messageEnvelope, bool isMessageOriginatedFromCurrentNode, string remoteMethodName)
        {
            string messageJson = JsonConvert.SerializeObject(messageEnvelope);
            if (isMessageOriginatedFromCurrentNode)
            {
                _logger.LogInformation($"Sending data to all connected clients except original sender: {Context.ConnectionId}. Message: {messageJson}.");
                await _hubContext.Clients.AllExcept(Context.ConnectionId).SendAsync(remoteMethodName, messageEnvelope);
            }
            else
            {
                _logger.LogInformation($"Sending data to all connected clients. Message: {messageJson}.");
                await _hubContext.Clients.All.SendAsync(remoteMethodName, messageEnvelope);

            }
        }

        private async Task PostMessageToMemberBackplanes<T>(T messageContextDTO)
        {
            //send message to member backplanes
            foreach (Uri nodeUri in _memberNodesRepo.MemberNodes)
            {
                //http://nodeurl/api/backplane/broadcast/context
                await PostRequestAsync(nodeUri, _broadcastPath, messageContextDTO);
            }
        }

        private async Task PostRequestAsync<T>(Uri baseUri, string relativePath, T dto)
        {
            Uri uri = new Uri(baseUri, relativePath);
            try
            {

                //Any failure in broadcast to any one node should not stop broadcast to other node.
                HttpResponseMessage httpResponseMessage = await HttpUtils.PostAsync(_httpClientFactory, uri, dto, _httpTimeOutInMs);
                httpResponseMessage.EnsureSuccessStatusCode();
                _logger.LogInformation($"Successfully completed send context to backplane: {uri}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Failed to post message to member backplane at address: {uri}.{ex}");
            }

        }
    }
}
