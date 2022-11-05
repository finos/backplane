using AutoFixture;
using Finos.Fdc3.Backplane.Config;
using Finos.Fdc3.Backplane.MultiHost;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Tests.Multihost
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
        public async Task ShouldSetCurrentNodeUri()
        {
           
            NodeRegistrationClient sut = _fixture.Create<NodeRegistrationClient>();
            var uri= new Uri("http://abc.com/");
            await sut.RegisterAsync(uri);
            Assert.IsTrue(sut.CurrentNodeUri == uri);
        }

       
    }
}
