/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using AutoFixture;
using Finos.Fdc3.Backplane.DTO.Envelope;
using Finos.Fdc3.Backplane.DTO.Envelope.Receive;
using Finos.Fdc3.Backplane.DTO.Envelope.Send;
using Finos.Fdc3.Backplane.DTO.FDC3;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System;

namespace Finos.Fdc3.Backplane.Tests
{
    public class MessageEnvelopeGeneratorTests
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixtures.Create();
        }

        [Test]
        public void Test_GenerateMessageEnvelope_returns_correctly_transformed_Broadcast_MessageEnvelope_object()
        {
            JObject jObject = JObject.Parse(@"{'type': 'fdc3.Instrument'}");
            string uniqueMessageid = Guid.NewGuid().ToString();
            BroadcastContextEnvelope context = _fixture.Build<BroadcastContextEnvelope>()
                .With(x => x.Context, new Context(jObject))
                .With(x => x.ChannelId, "group1")
                .With(x => x.Metadata, new EnvelopeMetadata() { UniqueMessageId = uniqueMessageid }).Create();

            MessageEnvelopeGenerator sut = _fixture.Create<MessageEnvelopeGenerator>();
            MessageEnvelope result = sut.GenerateMessageEnvelope(context);
            Assert.AreEqual(result.ActionType, Fdc3Action.Broadcast);
            Assert.AreEqual(result.Meta.UniqueMessageId, uniqueMessageid);
            Assert.AreEqual(result.Payload.ChannelId, "group1");
            Assert.AreEqual(result.Payload.Context, jObject);
        }
    }


}