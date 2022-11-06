using AutoFixture;
using Finos.Fdc3.Backplane.Client.Transport;
using Finos.Fdc3.Backplane.DTO.FDC3;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.Test.Transport
{
    public class BackplaneTransportTest
    {
        private Fixture _fixture;
        private ISignalRConnection _connection;
        private ISignalRConnectionBuilder _signalRConnectionBuilder;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixture.Create();
            _connection = _fixture.Freeze<ISignalRConnection>();
            _connection.State.Returns(ConnectionState.Connected);
            _signalRConnectionBuilder = _fixture.Freeze<ISignalRConnectionBuilder>();
            _signalRConnectionBuilder.Build(default).ReturnsForAnyArgs(_connection);
        }

        [Test]
        public async Task ShouldCreateAndStartConnectionOnInitialize()
        {
            SignalRBackplaneTransport sut = _fixture.Create<SignalRBackplaneTransport>();
            await sut.ConnectAsync(default, default, default);
            _signalRConnectionBuilder.ReceivedWithAnyArgs().Build(default);
            await _connection.ReceivedWithAnyArgs().StartAsync();
        }

        [Test]

        public async Task BroadcastAsyncShouldThrowExceptionIfConnectionIsBroken()
        {
            SignalRBackplaneTransport sut = _fixture.Create<SignalRBackplaneTransport>();
            _connection.State.Returns(ConnectionState.Disconnected);
            await sut.ConnectAsync(default, default, default);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.BroadcastAsync(default, default));

        }

        [Test]
        public async Task ShouldCallGetSystemChannelOnHub()
        {
            SignalRBackplaneTransport sut = _fixture.Create<SignalRBackplaneTransport>();
            await sut.ConnectAsync(default, default, default);
            await sut.GetSystemChannelsAsync();
            await _connection.ReceivedWithAnyArgs().InvokeAsync<IEnumerable<Channel>>("GetSystemChannels", Arg.Any<CancellationToken>());

        }

        [Test]
        public async Task GetCurrentContextAsynShouldThrowExceptionIfConnectionIsBroken()
        {
            _connection.State.Returns(ConnectionState.Disconnected);
            SignalRBackplaneTransport sut = _fixture.Create<SignalRBackplaneTransport>();
            await sut.ConnectAsync(default, default, default);
            Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.GetCurrentContextAsync(default, default));

        }

        [Test]
        public async Task ShouldCallGetCurrentContextAsyncOnHub()
        {
            SignalRBackplaneTransport sut = _fixture.Create<SignalRBackplaneTransport>();
            await sut.ConnectAsync(default, default, default);
            await sut.GetCurrentContextAsync(default, default);
            await _connection.ReceivedWithAnyArgs().InvokeAsync<JObject>("GetCurrentContextForChannel", default, Arg.Any<CancellationToken>());

        }

    }
}
