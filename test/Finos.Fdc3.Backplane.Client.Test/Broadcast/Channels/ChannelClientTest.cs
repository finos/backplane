/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using AutoFixture;
using Finos.Fdc3.Backplane.Client.Broadcast.Channels;
using Finos.Fdc3.Backplane.Client.Middleware;
using Finos.Fdc3.Backplane.DTO.FDC3;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.Test.Broadcast.Channels
{
    public class ChannelClientTest
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixture.Create();
        }

        [Test]
        public async Task ShouldAddContextListener()
        {
            IClientMiddleware clientMiddleware = _fixture.Freeze<IClientMiddleware>();
            _fixture.Create<ChannelClient>();
            ChannelClient sut = _fixture.Create<ChannelClient>();
            var hook=await sut.AddContextListenerAsync(default, default, default);
            Assert.IsTrue(hook != null);
        }

        [Test]
        public async Task ShouldBroadcastContext()
        {
            IClientMiddleware broadcastService = _fixture.Freeze<IClientMiddleware>();
            Channel channelDto = _fixture.Freeze<Channel>();
            Context context = new Context(JObject.Parse(@"{'type':'fdc3.open'}"));
            ChannelClient sut = _fixture.Create<ChannelClient>();
            await sut.BroadcastAsync(context);
            await broadcastService.Received().BroadcastAsync(Arg.Any<IContext>(), Arg.Any<string>(), Arg.Any<CancellationToken>());

        }

        [Test]
        public async Task ShouldQueryServiceForCurrentContext()
        {
            IClientMiddleware broadcastService = _fixture.Freeze<IClientMiddleware>();
            ChannelClient sut = _fixture.Create<ChannelClient>();
            await sut.GetCurrentContextAsync("fdc3.instrument", default);
            await broadcastService.Received().GetCurrentContextAsync(Arg.Any<string>(), Arg.Any<CancellationToken>());
        }

        [Test]
        public void ShouldReturnFormatOnCallingToString()
        {
            IClientMiddleware broadcastService = _fixture.Freeze<IClientMiddleware>();
            ChannelClient sut = new ChannelClient(broadcastService, new Channel("channel1", "system", new DisplayMetadata("channel1")));
            Assert.AreEqual(sut.ToString(), "channel1-system-channel1");
        }
    }
}
