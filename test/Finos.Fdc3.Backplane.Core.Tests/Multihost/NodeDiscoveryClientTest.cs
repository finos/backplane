using AutoFixture;
using Finos.Fdc3.Backplane.Core.MultiHost;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Core.Tests.Multihost
{
    internal class NodeDiscoveryClientTest
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixtures.Create();
        }



        [Test]
        public async Task DiscoverAsyncShouldNotReturnNull()
        {
            NodesDiscoveryClient sut = _fixture.Create<NodesDiscoveryClient>();
            IEnumerable<Uri> result = await sut.DiscoverAsync();
            Assert.IsNotNull(result);
        }
    }
}
