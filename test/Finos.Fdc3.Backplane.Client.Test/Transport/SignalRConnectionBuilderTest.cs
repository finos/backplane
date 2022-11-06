using AutoFixture;
using Finos.Fdc3.Backplane.Client.Transport;
using NUnit.Framework;
using System;

namespace Finos.Fdc3.Backplane.Client.Test.Transport
{
    internal class SignalRConnectionBuilderTest
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixture.Create();
        }

        [Test]
        public void ShouldBuildSignalRConnectionButNotStart()
        {
            IServiceProvider provider = _fixture.Freeze<IServiceProvider>();
            SignalRConnectionBuilder sut = _fixture.Create<SignalRConnectionBuilder>();
            ISignalRConnection hubConn = sut.Build(new Uri("http://localhost:4210"));
            Assert.IsTrue(hubConn.State == ConnectionState.Disconnected);
        }
    }
}
