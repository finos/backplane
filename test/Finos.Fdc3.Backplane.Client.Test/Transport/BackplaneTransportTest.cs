using AutoFixture;
using Finos.Fdc3.Backplane.Client.Resilliency;
using Finos.Fdc3.Backplane.Client.Transport;
using Finos.Fdc3.Backplane.DTO.FDC3;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using Polly;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.Test.Transport
{
    public class BackplaneTransportTest
    {
        private Fixture _fixture;
        private IConnection _connection;
        private IConnectionFactory _connectionFactory;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixture.Create();
            _connection = _fixture.Freeze<IConnection>();
            _connection.State.Returns(ConnectionState.Connected);
            _connectionFactory = _fixture.Freeze<IConnectionFactory>();
            _connectionFactory.Create(default).ReturnsForAnyArgs(_connection);
            IRetryPolicyProvider retryPolicyProvider = _fixture.Freeze<IRetryPolicyProvider>();
            retryPolicyProvider.GetAsyncRetryPolicy<Exception>(Arg.Any<int>(), Arg.Any<Func<int, TimeSpan>>()).Returns(Policy.NoOpAsync());
        }

        [Test]
        public async Task ShouldCreateAndStartConnectionOnInitialize()
        {
            BackplaneTransport sut = _fixture.Create<BackplaneTransport>();
            await sut.InitializeConnectionAsync(1, (t) => TimeSpan.FromSeconds(1), CancellationToken.None);
            await _connectionFactory.ReceivedWithAnyArgs().Create(default);
        }

        [Test]

        public void BroadcastAsyncShouldThrowExceptionIfConnectionIsBroken()
        {
            BackplaneTransport sut = _fixture.Create<BackplaneTransport>();
            _connection.State.Returns(ConnectionState.Disconnected);
            sut.InitializeConnectionAsync(1, (t) => TimeSpan.FromSeconds(1), CancellationToken.None).Wait();
            Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.BroadcastAsync(default, default));

        }

        [Test]
        public async Task ShouldCallGetSystemChannelOnHub()
        {
            BackplaneTransport sut = _fixture.Create<BackplaneTransport>();
            await sut.InitializeConnectionAsync(1, (t) => TimeSpan.FromSeconds(1), CancellationToken.None);
            await sut.GetSystemChannelsAsync(default);
            await _connection.Received().InvokeAsync<IEnumerable<Channel>>("GetSystemChannels", Arg.Any<CancellationToken>());

        }

        [Test]
        public void GetCurrentContextAsynShouldThrowExceptionIfConnectionIsBroken()
        {
            _connection.State.Returns(ConnectionState.Disconnected);
            BackplaneTransport sut = _fixture.Create<BackplaneTransport>();
            sut.InitializeConnectionAsync(1, (t) => TimeSpan.FromSeconds(1), CancellationToken.None).Wait();
            Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.GetCurrentContextAsync(default, default));

        }

        [Test]
        public async Task ShouldCallGetCurrentContextAsyncOnHub()
        {
            BackplaneTransport sut = _fixture.Create<BackplaneTransport>();
            await sut.InitializeConnectionAsync(1, (t) => TimeSpan.FromSeconds(1), CancellationToken.None);
            await sut.GetCurrentContextAsync(default, default);
            await _connection.ReceivedWithAnyArgs().InvokeAsync<JObject>("GetCurrentContextForChannel", default, Arg.Any<CancellationToken>());

        }

        [Test]
        public async Task ShouldReinitiateConnectionOnClose()
        {
            BackplaneTransport sut = _fixture.Create<BackplaneTransport>();
            await sut.InitializeConnectionAsync(1, (t) => TimeSpan.FromSeconds(1), CancellationToken.None);
            _connection.Closed += Raise.Event<Func<Exception, Task>>(new Exception("Connection Closed"));
            await _connectionFactory.ReceivedWithAnyArgs(2).Create(default);
        }
    }
}
