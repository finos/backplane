using AutoFixture;
using Finos.Fdc3.Backplane.Core.MultiHost;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Core.Tests.Multihost
{
    internal class NodeRegistrationClientTest
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixtures.Create();
        }



        [Test]
        public async Task ShouldTakeUriAsParameterToRegister()
        {
            NodeRegistrationClient sut = _fixture.Create<NodeRegistrationClient>();
            await sut.RegisterAsync(new Uri("http://abc.com/"));
        }
    }
}
