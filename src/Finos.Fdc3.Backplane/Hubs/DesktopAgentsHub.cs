/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.Config;
using Finos.Fdc3.Backplane.DTO;
using Finos.Fdc3.Backplane.DTO.Envelope;
using Finos.Fdc3.Backplane.DTO.FDC3;
using Finos.Fdc3.Backplane.MultiHost;
using Finos.Fdc3.Backplane.Utils;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Hubs
{
    /// <summary>
    /// SignalR hub
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
            _broadcastPath = Path.Combine(configRepository.HubEndpoint, configRepository.BroadcastEndpoint);
        }

        /// <summary>
        /// Invoked by member node. Hence original message source is not this node
        /// </summary>
        /// <param name="messageEnvelope"></param>
        /// <returns></returns>
        public async Task BroadcastToLocalClients(MessageEnvelope messageEnvelope)
        {
            await HandleBroadcast(messageEnvelope, false);
        }

        /// <summary>
        /// Invoked by local connected clients.Hence original message source is this node.
        /// This would also result in propagation of message to other member nodes
        /// </summary>
        /// <param name="messageEnvelope"></param>
        /// <returns></returns>
        public async Task Broadcast(MessageEnvelope messageEnvelope)
        {
            await HandleBroadcast(messageEnvelope, true);
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


        /// <summary>
        /// Provides system channels
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Channel>> GetSystemChannels()
        {
            return await Task.FromResult(_configRepository.Channels);
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
                //http://nodeurl/backplane/v1.0/broadcast/context
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
