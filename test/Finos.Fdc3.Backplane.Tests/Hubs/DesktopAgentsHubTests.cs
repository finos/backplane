/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using AutoFixture;
using Finos.Fdc3.Backplane.Config;
using Finos.Fdc3.Backplane.DTO;
using Finos.Fdc3.Backplane.DTO.Envelope;
using Finos.Fdc3.Backplane.DTO.FDC3;
using Finos.Fdc3.Backplane.Hubs;
using Finos.Fdc3.Backplane.MultiHost;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Tests.Hubs
{
    [TestFixture]
    public class DesktopAgentsHubTests
    {
        private const string ConnectionId = "test-connection-id";
        private const string BroadcastEndpoint = "api/backplane/broadcast/context";

        private Fixture _fixture;
        private ILogger<DesktopAgentsHub> _logger;
        private IHubContext<DesktopAgentsHub> _hubContext;
        private IHubClients _hubClients;
        private IClientProxy _allExceptProxy;
        private IClientProxy _allProxy;
        private IHttpClientFactory _httpClientFactory;
        private INodesRepository _nodesRepository;
        private IConfigRepository _configRepository;
        private HubCallerContext _callerContext;
        private TestHttpMessageHandler _httpMessageHandler;
        private DesktopAgentsHub _sut;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixtures.Create();

            _logger = Substitute.For<ILogger<DesktopAgentsHub>>();

            _allExceptProxy = Substitute.For<IClientProxy>();
            _allProxy = Substitute.For<IClientProxy>();
            _hubClients = Substitute.For<IHubClients>();
            _hubClients.All.Returns(_allProxy);
            _hubClients.AllExcept(Arg.Any<IReadOnlyList<string>>()).Returns(_allExceptProxy);
            _hubContext = Substitute.For<IHubContext<DesktopAgentsHub>>();
            _hubContext.Clients.Returns(_hubClients);

            _httpMessageHandler = new TestHttpMessageHandler();
            _httpClientFactory = Substitute.For<IHttpClientFactory>();
            _httpClientFactory.CreateClient("Backplane").Returns(_ => new HttpClient(_httpMessageHandler, false));

            _nodesRepository = Substitute.For<INodesRepository>();
            _nodesRepository.MemberNodes.Returns(Enumerable.Empty<Uri>());

            _configRepository = Substitute.For<IConfigRepository>();
            _configRepository.HttpRequestTimeoutInMilliseconds.Returns(TimeSpan.FromMilliseconds(5000));
            _configRepository.BroadcastEndpoint.Returns(BroadcastEndpoint);
            _configRepository.Channels.Returns(_fixture.CreateMany<Channel>(8).ToList());

            _callerContext = Substitute.For<HubCallerContext>();
            _callerContext.ConnectionId.Returns(ConnectionId);

            _sut = new DesktopAgentsHub(_logger, _hubContext, _httpClientFactory, _nodesRepository, _configRepository)
            {
                Context = _callerContext
            };
        }

        [Test]
        public void Broadcast_ShouldThrowHubException_WhenPayloadIsNull()
        {
            //Arrange
            //Act
            HubException ex = Assert.ThrowsAsync<HubException>(() => _sut.Broadcast(null));
            //Assert
            Assert.That(ex.HResult, Is.EqualTo((int)ResponseCodes.BroadcastPayloadInvalid));
            Assert.That(_httpMessageHandler.Requests, Is.Empty);
        }

        [Test]
        public void BroadcastToLocalClients_ShouldThrowHubException_WhenPayloadIsNull()
        {
            //Arrange
            //Act
            HubException ex = Assert.ThrowsAsync<HubException>(() => _sut.BroadcastToLocalClients(null));
            //Assert
            Assert.That(ex.HResult, Is.EqualTo((int)ResponseCodes.BroadcastPayloadInvalid));
            Assert.That(_httpMessageHandler.Requests, Is.Empty);
        }

        [Test]
        public async Task Broadcast_ShouldSendToAllClientsExceptSender_AndPostToEachMemberNode()
        {
            //Arrange
            MessageEnvelope envelope = CreateEnvelope();
            Uri[] memberNodes = { new Uri("http://node1:4475/"), new Uri("http://node2:4476/"), new Uri("http://node3:4477/") };
            _nodesRepository.MemberNodes.Returns(memberNodes);
            //Act
            await _sut.Broadcast(envelope);
            //Assert
            _hubClients.Received(1).AllExcept(Arg.Is<IReadOnlyList<string>>(ids => ids.Count == 1 && ids[0] == ConnectionId));
            await _allExceptProxy.Received(1).SendCoreAsync("OnMessage", Arg.Is<object[]>(args => args.Length == 1 && ReferenceEquals(args[0], envelope)), Arg.Any<CancellationToken>());
            _ = _hubClients.DidNotReceive().All;
            await _allProxy.DidNotReceiveWithAnyArgs().SendCoreAsync(default, default, default);

            Assert.That(_httpMessageHandler.Requests, Has.Count.EqualTo(memberNodes.Length));
            foreach (Uri node in memberNodes)
            {
                Uri expected = new Uri(node, BroadcastEndpoint);
                Assert.That(_httpMessageHandler.Requests.Count(r => r.Method == HttpMethod.Post && r.RequestUri == expected), Is.EqualTo(1));
            }
        }

        [Test]
        public async Task BroadcastToLocalClients_ShouldSendToAllClients_AndNotPostToMemberNodes()
        {
            //Arrange
            MessageEnvelope envelope = CreateEnvelope();
            _nodesRepository.MemberNodes.Returns(new[] { new Uri("http://node1:4475/"), new Uri("http://node2:4476/") });
            //Act
            await _sut.BroadcastToLocalClients(envelope);
            //Assert
            _ = _hubClients.Received(1).All;
            await _allProxy.Received(1).SendCoreAsync("OnMessage", Arg.Is<object[]>(args => args.Length == 1 && ReferenceEquals(args[0], envelope)), Arg.Any<CancellationToken>());
            _hubClients.DidNotReceiveWithAnyArgs().AllExcept(default);
            await _allExceptProxy.DidNotReceiveWithAnyArgs().SendCoreAsync(default, default, default);
            Assert.That(_httpMessageHandler.Requests, Is.Empty);
        }

        [Test]
        public async Task Broadcast_ShouldContinueToNextNode_AndLogError_WhenPostToOneNodeFails()
        {
            //Arrange
            MessageEnvelope envelope = CreateEnvelope();
            Uri failingNode = new Uri("http://failing-node:4475/");
            Uri healthyNode = new Uri("http://healthy-node:4476/");
            Uri failingUri = new Uri(failingNode, BroadcastEndpoint);
            Uri healthyUri = new Uri(healthyNode, BroadcastEndpoint);
            _nodesRepository.MemberNodes.Returns(new[] { failingNode, healthyNode });
            _httpMessageHandler.Responder = request =>
            {
                if (request.RequestUri == failingUri)
                {
                    throw new HttpRequestException("Connection refused");
                }
                return new HttpResponseMessage(HttpStatusCode.OK);
            };
            //Act
            //Assert
            Assert.DoesNotThrowAsync(() => _sut.Broadcast(envelope));
            Assert.That(_httpMessageHandler.Requests.Count(r => r.RequestUri == failingUri), Is.EqualTo(1));
            Assert.That(_httpMessageHandler.Requests.Count(r => r.RequestUri == healthyUri), Is.EqualTo(1));

            // Known limitation: LogError is a static extension, so the underlying ILogger.Log<TState> call is
            // verified instead by matching the formatted state string against the failing node URI. Wording
            // changes to the LogError call in DesktopAgentsHub.PostRequestAsync require updating this matcher.
            Assert.That(CountErrorLogsContaining(failingUri.ToString()), Is.EqualTo(1));
            Assert.That(CountErrorLogsContaining(healthyUri.ToString()), Is.EqualTo(0));
            await Task.CompletedTask;
        }

        [Test]
        public async Task Broadcast_ShouldLogError_WhenNodeReturnsNonSuccessStatusCode()
        {
            //Arrange
            MessageEnvelope envelope = CreateEnvelope();
            Uri failingNode = new Uri("http://failing-node:4475/");
            Uri failingUri = new Uri(failingNode, BroadcastEndpoint);
            _nodesRepository.MemberNodes.Returns(new[] { failingNode });
            _httpMessageHandler.Responder = _ => new HttpResponseMessage(HttpStatusCode.InternalServerError);
            //Act
            await _sut.Broadcast(envelope);
            //Assert
            Assert.That(_httpMessageHandler.Requests, Has.Count.EqualTo(1));
            Assert.That(CountErrorLogsContaining(failingUri.ToString()), Is.EqualTo(1));
        }

        [Test]
        public async Task GetUserChannels_ShouldReturnChannelsFromConfigRepository()
        {
            //Arrange
            List<Channel> channels = _fixture.CreateMany<Channel>(8).ToList();
            _configRepository.Channels.Returns(channels);
            //Act
            IEnumerable<Channel> result = await _sut.GetUserChannels();
            //Assert
            Assert.That(result, Is.SameAs(channels));
            Assert.That(result.Count(), Is.EqualTo(8));
            Assert.That(result, Is.EqualTo(channels));
        }

        /// <summary>
        /// Counts ILogger.Log&lt;TState&gt; calls at Error level whose formatted state contains <paramref name="substring"/>.
        /// TState is the framework's internal FormattedLogValues, so the call cannot be matched via
        /// <c>Received().Log&lt;TState&gt;(...)</c> with a public TState; the recorded calls are inspected instead.
        /// </summary>
        private int CountErrorLogsContaining(string substring)
        {
            return _logger.ReceivedCalls()
                .Where(call => call.GetMethodInfo().Name == nameof(ILogger.Log))
                .Select(call => call.GetArguments())
                .Count(args => (LogLevel)args[0] == LogLevel.Error && args[2]?.ToString()?.Contains(substring) == true);
        }

        private static MessageEnvelope CreateEnvelope()
        {
            JObject context = JObject.Parse(@"{'type': 'fdc3.Instrument'}");
            return new MessageEnvelope()
            {
                Payload = new EnvelopeData() { ChannelId = "fdc3.channel.1", Context = context },
                Meta = new EnvelopeMeta() { RequestGuid = Guid.NewGuid().ToString(), Source = new AppIdentifier() { AppId = "Test" } }
            };
        }

        private sealed class TestHttpMessageHandler : HttpMessageHandler
        {
            public List<HttpRequestMessage> Requests { get; } = new List<HttpRequestMessage>();

            public Func<HttpRequestMessage, HttpResponseMessage> Responder { get; set; } = _ => new HttpResponseMessage(HttpStatusCode.OK);

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                Requests.Add(request);
                return Task.FromResult(Responder(request));
            }
        }
    }
}
