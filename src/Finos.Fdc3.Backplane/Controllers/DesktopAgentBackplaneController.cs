/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.DTO.Envelope;
using Finos.Fdc3.Backplane.MultiHost;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Controllers
{
    /// <summary>
    /// REST API for multihost communication. 
    /// </summary>
    [ApiController]
    [Route("backplane/api/v1")]
    public class DesktopAgentBackplaneController : ControllerBase
    {
        private readonly ILogger<DesktopAgentBackplaneController> _logger;
        private readonly IDesktopAgentHub _hub;
        private readonly INodesRepository _memberNodesRepository;

        public DesktopAgentBackplaneController(ILogger<DesktopAgentBackplaneController> logger, INodesRepository memberNodesRepository, IDesktopAgentHub hub)
        {
            _logger = logger;
            _hub = hub;
            _memberNodesRepository = memberNodesRepository;
        }

        /// <summary>
        /// Member node call this end point to send broadcast context to this node.
        /// </summary>
        /// <param name="message"> Input message</param>
        /// <returns>Response with appropriate status code.</returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("broadcast/context")]
        public async Task<IActionResult> BroadcastToLocalClients([FromBody]MessageEnvelope message)
        {
            if (message == null)
            {
                return await Task.FromResult(StatusCode(400, "Input parameter is missing"));
            }
            try
            {
                _logger.LogInformation($"Broadcast context request received : {JsonConvert.SerializeObject(message)}");
                return await Task.FromResult(Ok(_hub.BroadcastToLocalClients(message)));
            }
            catch (Exception ex)
            {
                _logger.LogError($"Internal server error.Error:{ex}");
                return await Task.FromResult(StatusCode(500, ex));
            }
        }

        /// <summary>
        /// This end point allows member nodes to register them with this node 
        /// </summary>
        /// <param name="memberNodeUri"></param>
        /// <returns></returns>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("addmembernode")]
        public async Task<IActionResult> AddMemberNode([FromBody]Uri memberNodeUri)
        {
            if (memberNodeUri == null)
            {
                return await Task.FromResult(StatusCode(400, "Input parameter is missing"));
            }
            _logger.LogDebug($"Received request from: {memberNodeUri} to add as member");

            try
            {
                _memberNodesRepository.AddNode(memberNodeUri);
                return await Task.FromResult(Ok());
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error processing request in addMemberNode.{ex}.");
                return await Task.FromResult(StatusCode(500, ex));
            }

        }

        /// <summary>
        /// PROD support helper, to remote shut down this node.
        /// </summary>
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost]
        [Route("shutdown")]
        public void Shutdown()
        {
            _logger.LogInformation("Received request for shutdown self process");
            System.Diagnostics.Process.GetCurrentProcess().Kill();

        }

    }

}
