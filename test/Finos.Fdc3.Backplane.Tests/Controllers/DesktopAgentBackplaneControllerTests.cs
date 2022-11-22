/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using AutoFixture;
using Finos.Fdc3.Backplane.Controllers;
using Finos.Fdc3.Backplane.DTO.Envelope;
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
            BackplaneController sut = _fixture.Build<BackplaneController>().OmitAutoProperties().Create();
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
            MessageEnvelope broadcastContext = new MessageEnvelope() { Payload = new EnvelopeData() { ChannelId = "abc", Context = context }, Meta = new EnvelopeMeta() { RequestGuid = uniqueMessageId, Source = new AppIdentifier() { AppId = "Test" } } };
            BackplaneController sut = _fixture.Build<BackplaneController>().OmitAutoProperties().Create();
            //Act
            ObjectResult result = sut.BroadcastToLocalClients(broadcastContext).Result as ObjectResult;
            //Assert
            Assert.AreEqual(result.StatusCode, 200);
        }

        [Test]
        public void ShouldReturnErrorCodeIfExceptionOccurs()
        {
            //Arrange
            JObject context = JObject.Parse(@"{'type': 'fdc3.Instrument'}");
            string uniqueMessageId = Guid.NewGuid().ToString();
            MessageEnvelope broadcastContext = new MessageEnvelope() { Payload = new EnvelopeData() { ChannelId = "fdc3.channel.1", Context = context }, Meta = new EnvelopeMeta() { RequestGuid = uniqueMessageId, Source = new AppIdentifier() { AppId = "Test" } } };
            IDesktopAgentHub hub = _fixture.Freeze<IDesktopAgentHub>();
            hub.BroadcastToLocalClients(broadcastContext).ReturnsForAnyArgs(x => { throw new Exception("Exception Occured"); });
            BackplaneController sut = _fixture.Build<BackplaneController>().OmitAutoProperties().Create();
            //Act
            ObjectResult result = sut.BroadcastToLocalClients(broadcastContext).Result as ObjectResult;
            //Assert
            Assert.AreEqual(result.StatusCode, 500);
            Assert.AreEqual((result.Value as Exception).Message, "Exception Occured");
            Assert.Throws<Exception>(() => hub.BroadcastToLocalClients(broadcastContext));

        }
    }
}