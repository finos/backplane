using AutoFixture;
using Finos.Fdc3.Backplane.Client.Discovery;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.Test.Discovery
{
    public class BackplaneDiscoveryServiceClientTest
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixture.Create();
        }


        [Test]
        public async Task ShouldThrowExceptionIfQueryServiceReturnsEmptyResponse()
        {
            BackplaneDiscoveryServiceClient sut = _fixture.Create<BackplaneDiscoveryServiceClient>();
            Uri uri = await sut.DiscoverAsync(default);
            Assert.AreEqual(uri, new Uri("http://localhost:49201"));
        }

    }

}
