using AutoFixture;
using Finos.Fdc3.Backplane.Controllers;
using Finos.Fdc3.Backplane.DTO.Envelope;
using Finos.Fdc3.Backplane.DTO.Envelope.Send;
using Finos.Fdc3.Backplane.DTO.FDC3;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using System;

namespace Finos.Fdc3.Backplane.Tests.Controllers
{
    public class DesktopAgentBackplaneControllerTests
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixtures.Create();
        }


        [Test]
        public void Test_BroadcastToLocalClients_ShouldReturn_400_WhenParameterIsMissing()
        {
            //Arrange
            DesktopAgentBackplaneController sut = _fixture.Build<DesktopAgentBackplaneController>().OmitAutoProperties().Create();
            //Act
            ObjectResult result = sut.BroadcastToLocalClients(null).Result as ObjectResult;
            //Assert
            Assert.AreEqual(result.StatusCode, 400);
        }

        [Test]
        public void Test_BroadcastToLocalClients_ShouldReturn_200_WhenParameterIsPassed()
        {
            //Arrange
            JObject context = JObject.Parse(@"{'type': 'fdc3.Instrument'}");
            string uniqueMessageId = Guid.NewGuid().ToString();
            BroadcastContextEnvelope broadcastContext = new BroadcastContextEnvelope() { ChannelId = "abc", Metadata = new EnvelopeMetadata() { UniqueMessageId = uniqueMessageId, Source = new AppIdentifier() { AppId = "Test" } }, Context = context };
            DesktopAgentBackplaneController sut = _fixture.Build<DesktopAgentBackplaneController>().OmitAutoProperties().Create();
            //Act
            ObjectResult result = sut.BroadcastToLocalClients(broadcastContext).Result as ObjectResult;
            //Assert
            Assert.AreEqual(result.StatusCode, 200);
        }

        [Test]
        public void Test_BroadcastToLocalClients_ShouldReturn_500_WhenExceptionOccurs()
        {
            //Arrange
            JObject context = JObject.Parse(@"{'type': 'fdc3.Instrument'}");
            string uniqueMessageId = Guid.NewGuid().ToString();
            BroadcastContextEnvelope broadcastContext = new BroadcastContextEnvelope() { ChannelId = "abc", Metadata = new EnvelopeMetadata() { UniqueMessageId = uniqueMessageId, Source = new AppIdentifier() { AppId = "Test" } }, Context = context };
            IDesktopAgentHub userService = _fixture.Freeze<IDesktopAgentHub>();
            userService.Broadcast(broadcastContext, true).Returns(x => { throw new Exception("Exception Occured"); });
            DesktopAgentBackplaneController sut = _fixture.Build<DesktopAgentBackplaneController>().OmitAutoProperties().Create();
            //Act
            ObjectResult result = sut.BroadcastToLocalClients(broadcastContext).Result as ObjectResult;
            //Assert
            Assert.AreEqual(result.StatusCode, 500);
            Assert.AreEqual((result.Value as Exception).Message, "Exception Occured");
            Assert.Throws<Exception>(() => userService.Broadcast(broadcastContext, true));

        }
    }
}