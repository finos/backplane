using AutoFixture;
using Finos.Fdc3.Backplane.Client.Discovery;
using Finos.Fdc3.Backplane.Client.Transport;
using Microsoft.Extensions.DependencyInjection;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.Test.Transport
{
    internal class SignalRConnectionFactoryTest
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixture.Create();
        }

        [Test]
        public async Task ShouldBuildSignalRConnectionButNotStart()
        {
            IServiceProvider provider = _fixture.Freeze<IServiceProvider>();
            IBackplaneDiscoveryServiceClient discoveryService = _fixture.Freeze<IBackplaneDiscoveryServiceClient>();
            discoveryService.DiscoverAsync(default).ReturnsForAnyArgs(new Uri("http://localhost:4500"));
            provider.GetService<IBackplaneDiscoveryServiceClient>().Returns(discoveryService);
            SignalRConnectionFactory sut = _fixture.Create<SignalRConnectionFactory>();
            IConnection hubConn = await sut.Create(default);
            Assert.IsTrue(hubConn.State == ConnectionState.Disconnected);
        }
    }
}
