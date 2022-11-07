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
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
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
        private readonly string _broadcastEndpoint;
        private static readonly SemaphoreSlim _threadSafeCurrentContextAccessHandle = new SemaphoreSlim(1, 1);
        private static readonly ConcurrentDictionary<string, JObject> _currentContextStore = new ConcurrentDictionary<string, JObject>();
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
            _broadcastEndpoint = configRepository.AddNodeEndpoint;
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
                await PostMessageToMemberBackplanes(messageEnvelope, _broadcastEndpoint);
            }

            await _threadSafeCurrentContextAccessHandle.WaitAsync().ConfigureAwait(false);
            try
            {
                _currentContextStore.AddOrUpdate(messageEnvelope.Payload.ChannelId, messageEnvelope.Payload.Context, (channelId, ctx) => messageEnvelope.Payload.Context);
            }
            finally
            {
                _threadSafeCurrentContextAccessHandle.Release();
            }
            _logger.LogDebug($"Updated current context for channelId : {messageEnvelope.Payload.ChannelId}: {JsonConvert.SerializeObject(messageEnvelope.Payload.Context)}");
        }

        /// <summary>
        /// Get Context for Channel.
        /// </summary>
        /// <param name="channelId">Channel Id</param>
        /// <returns>Jobject Channel details</returns>
        public async Task<JObject> GetCurrentContextForChannel(string channelId)
        {
            await _threadSafeCurrentContextAccessHandle.WaitAsync().ConfigureAwait(false);
            try
            {
                JObject currentContextForChannel = null;
                _logger.LogDebug($"Received call to get current context for channel: {channelId}");

                if (_currentContextStore.TryGetValue(channelId, out JObject currentContext))
                {
                    currentContextForChannel = currentContext;
                }
                string ctxAsString = currentContextForChannel == null ? "NULL" : JsonConvert.SerializeObject(currentContextForChannel);
                _logger.LogDebug($"Returning current context for channel: {channelId}, context: ${ctxAsString}");
                return currentContextForChannel;
            }
            finally
            {
                _threadSafeCurrentContextAccessHandle.Release();
            }

        }

        /// <summary>
        /// Provides system channels
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<Channel>> GetSystemChannels()
        {
            return await Task.FromResult(_configRepository.ChannelsList);
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

        private async Task PostMessageToMemberBackplanes<T>(T messageContextDTO, string urlSuffix)
        {
            //send message to member backplanes
            foreach (Uri uri in _memberNodesRepo.MemberNodes)
            {
                await PostRequestAsync(uri, _broadcastEndpoint, messageContextDTO);
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
