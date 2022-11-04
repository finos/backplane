/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using AutoFixture;
using Finos.Fdc3.Backplane.Client.API;
using Finos.Fdc3.Backplane.Client.Broadcast.Channels;
using Finos.Fdc3.Backplane.Client.Discovery;
using Finos.Fdc3.Backplane.Client.Middleware;
using Finos.Fdc3.Backplane.DTO.FDC3;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.Test.API
{
    public class DesktopAgentBackplaneClientTest
    {

        private Fixture _fixture;
        private IClientMiddleware _clientService;
        private ChannelClient _channelInstance;
        private IChannel _channelMock;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixture.Create();
            _clientService = _fixture.Freeze<IClientMiddleware>();
            _channelInstance = new ChannelClient(_clientService, new Channel("channel1", "system"));
            _channelMock = _fixture.Freeze<IChannel>();
            _channelMock.Id.Returns("channel2");
            _clientService.GetSystemChannelsAsync().Returns(new[] { _channelInstance, _channelMock });
        }


        [Test]
        public void ShouldNotDiscoveryBackplaneOnObjectCreation()
        {
            IBackplaneDiscoveryServiceClient discoveryClient = _fixture.Freeze<IBackplaneDiscoveryServiceClient>();
            _fixture.Create<BackplaneClient>();
            discoveryClient.DidNotReceiveWithAnyArgs().DiscoverAsync(default);

        }

        [Test]
        public async Task ShouldSetInitializeFlagOnceOnly()
        {
            BackplaneClient sut = _fixture.Create<BackplaneClient>();
            AppIdentifier appIdentifier = new AppIdentifier() { AppId = "Test" };
            await sut.InitializeAsync(appIdentifier);
            await sut.InitializeAsync(appIdentifier);
            await _clientService.Received(1).InitializeAsync(appIdentifier, Arg.Any<int>(), Arg.Any<Func<int, TimeSpan>>());
        }

        [Test]
        public async Task ShouldNotBroadcastIfNoChannelIsSelected()
        {
            BackplaneClient sut = _fixture.Create<BackplaneClient>();
            await sut.InitializeAsync(Arg.Any<AppIdentifier>());
            await sut.BroadcastAsync(Arg.Any<IContext>());
            await _channelMock.DidNotReceive().BroadcastAsync(Arg.Any<IContext>());
        }

        [Test]
        public async Task ShouldAddContextListenerOnSelectedChannel()
        {
            BackplaneClient sut = _fixture.Create<BackplaneClient>();
            await sut.InitializeAsync(Arg.Any<AppIdentifier>());
            await sut.JoinChannelAsync("channel2");
            await sut.AddContextListenerAsync("fdc3.open", (ct) => { }, default);
            await _channelMock.Received(1).AddContextListenerAsync(Arg.Any<string>(), Arg.Any<Action<IContext>>(), default);

        }

        [Test]
        public async Task GetSystemChannelsShouldReturnPopulatedChannel()
        {
            BackplaneClient sut = _fixture.Create<BackplaneClient>();
            await sut.InitializeAsync(Arg.Any<AppIdentifier>());
            Assert.AreEqual(sut.GetSystemChannelsAsync().Result.Count(), 2);
        }

        [Test]
        public async Task LeaveCurrentChannelAsyncShouldSetSelectedChannelToNull()
        {
            BackplaneClient sut = _fixture.Create<BackplaneClient>();
            await sut.InitializeAsync(Arg.Any<AppIdentifier>());
            await sut.JoinChannelAsync("channel1");
            Assert.IsTrue(sut.GetCurrentChannelAsync().Result.Id == _channelInstance.Id);
            await sut.LeaveCurrentChannelAsync();
            Assert.IsNull(sut.GetCurrentChannelAsync().Result);
        }

    }
}