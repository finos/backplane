using AutoFixture;
using Finos.Fdc3.Backplane.Client.Concurrency;
using Finos.Fdc3.Backplane.Client.Middleware;
using Finos.Fdc3.Backplane.Client.Transport;
using Finos.Fdc3.Backplane.DTO.Envelope;
using Finos.Fdc3.Backplane.DTO.Envelope.Receive;
using Finos.Fdc3.Backplane.DTO.FDC3;
using Newtonsoft.Json.Linq;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Reactive.Concurrency;
using System.Reactive.Subjects;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.Test.Services
{
    public class ClientMiddlewareTest
    {
        private Fixture _fixture;
        private IBackplaneTransport _desktopAgentTransport;
        private Subject<MessageEnvelope> _dataStream;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixture.Create();
            _desktopAgentTransport = _fixture.Freeze<IBackplaneTransport>();
            _dataStream = new Subject<MessageEnvelope>();
            ISchedulerProvider schedulerProvider = _fixture.Freeze<ISchedulerProvider>();
            schedulerProvider.TaskPool.Returns(Scheduler.Immediate);
        }


        [Test]
        public async Task ShouldAddContextListener()
        {
            _desktopAgentTransport.ReceiveDataStream.Returns(_dataStream);
            ClientMiddleware sut = _fixture.Create<ClientMiddleware>();
            string isin = "";
            string instrument = @"{
                type: 'fdc3.instrument',
                id: {
                    ticker: 'AAPL',
                    ISIN: 'US0378331005',
                    FIGI: 'BBG000B9XRY4',
                },
              }";
            IListener hook = await sut.AddContextListenerAsync("fdc3.instrument", (context) =>
              {
                  isin = context["id"]["ISIN"].ToString();
              }, "1");

            _dataStream.OnNext(new MessageEnvelope() { Payload = new EnvelopeData() { ChannelId = "1", Context = JObject.Parse(instrument) }, Meta = new EnvelopeMetadata() { Source = new AppIdentifier() { AppId = "Test" }, UniqueMessageId = Guid.NewGuid().ToString() } });

            Assert.IsTrue(isin == "US0378331005");
        }

        [Test]
        public async Task ShouldNotInvokeHandlerIfChannelIsDifferent()
        {
            _desktopAgentTransport.ReceiveDataStream.Returns(_dataStream);
            ClientMiddleware sut = _fixture.Create<ClientMiddleware>();
            string isin = "";
            string instrument = @"{
                type: 'fdc3.instrument',
                id: {
                    ticker: 'AAPL',
                    ISIN: 'US0378331005',
                    FIGI: 'BBG000B9XRY4',
                },
              }";
            IListener hook = await sut.AddContextListenerAsync("fdc3.instrument", (context) =>
            {
                isin = context["id"]["ISIN"].ToString();
            }, "2");

            _dataStream.OnNext(new MessageEnvelope() { Payload = new EnvelopeData() { ChannelId = "1", Context = JObject.Parse(instrument) }, Meta = new EnvelopeMetadata() { Source = new AppIdentifier() { AppId = "Test" }, UniqueMessageId = Guid.NewGuid().ToString() } });

            Assert.IsTrue(isin == "");
        }

        [Test]
        public async Task ShouldNotInvokeHandlerIfContextTypeIsNotSubscribed()
        {
            _desktopAgentTransport.ReceiveDataStream.Returns(_dataStream);
            ClientMiddleware sut = _fixture.Create<ClientMiddleware>();
            string isin = "";
            string instrument = @"{
                type: 'fdc3.instrument1',
                id: {
                    ticker: 'AAPL',
                    ISIN: 'US0378331005',
                    FIGI: 'BBG000B9XRY4',
                },
              }";
            IListener hook = await sut.AddContextListenerAsync("fdc3.instrument", (context) => { isin = context["id"]["ISIN"].ToString(); }, "1");

            _dataStream.OnNext(new MessageEnvelope() { Payload = new EnvelopeData() { ChannelId = "1", Context = JObject.Parse(instrument) }, Meta = new EnvelopeMetadata() { Source = new AppIdentifier() { AppId = "Test" }, UniqueMessageId = Guid.NewGuid().ToString() } });

            Assert.IsTrue(isin == "");
        }

        [Test]
        public async Task ShouldNotInvokeHandlerWhenUnsubscribed()
        {
            _desktopAgentTransport.ReceiveDataStream.Returns(_dataStream);
            ClientMiddleware sut = _fixture.Create<ClientMiddleware>();
            int receieveCounter = 0;
            string instrument = @"{
                type: 'fdc3.instrument',
                id: {
                    ticker: 'AAPL',
                    ISIN: 'US0378331005',
                    FIGI: 'BBG000B9XRY4',
                },
              }";
            IListener hook = await sut.AddContextListenerAsync("fdc3.instrument", (context) =>
            {
                receieveCounter++;
            }, "1");

            _dataStream.OnNext(new MessageEnvelope() { Payload = new EnvelopeData() { ChannelId = "1", Context = JObject.Parse(instrument) }, Meta = new EnvelopeMetadata() { Source = new AppIdentifier() { AppId = "Test" }, UniqueMessageId = Guid.NewGuid().ToString() } });
            _dataStream.OnNext(new MessageEnvelope() { Payload = new EnvelopeData() { ChannelId = "1", Context = JObject.Parse(instrument) }, Meta = new EnvelopeMetadata() { Source = new AppIdentifier() { AppId = "Test" }, UniqueMessageId = Guid.NewGuid().ToString() } });
            Assert.IsTrue(receieveCounter == 2);
            await hook.UnsubscribeAsync();
            _dataStream.OnNext(new MessageEnvelope() { Payload = new EnvelopeData() { ChannelId = "1", Context = JObject.Parse(instrument) }, Meta = new EnvelopeMetadata() { Source = new AppIdentifier() { AppId = "Test" }, UniqueMessageId = Guid.NewGuid().ToString() } });
            Assert.IsTrue(receieveCounter == 2);
        }

        [Test]
        public async Task ShouldNotInvokeHandlerForUnsubscribedOne()
        {
            _desktopAgentTransport.ReceiveDataStream.Returns(_dataStream);
            ClientMiddleware sut = _fixture.Create<ClientMiddleware>();
            int receieveCounterHook1 = 0;
            int receieveCounterHook2 = 0;
            string instrument = @"{
                type: 'fdc3.instrument',
                id: {
                    ticker: 'AAPL',
                    ISIN: 'US0378331005',
                    FIGI: 'BBG000B9XRY4',
                },
              }";
            IListener hook = await sut.AddContextListenerAsync("fdc3.instrument", (context) =>
            {
                receieveCounterHook1++;
            }, "1");

            IListener hook2 = await sut.AddContextListenerAsync("fdc3.instrument", (context) =>
            {
                receieveCounterHook2++;
            }, "1");

            _dataStream.OnNext(new MessageEnvelope() { Payload = new EnvelopeData() { ChannelId = "1", Context = JObject.Parse(instrument) }, Meta = new EnvelopeMetadata() { Source = new AppIdentifier() { AppId = "Test" }, UniqueMessageId = Guid.NewGuid().ToString() } });
            _dataStream.OnNext(new MessageEnvelope() { Payload = new EnvelopeData() { ChannelId = "1", Context = JObject.Parse(instrument) }, Meta = new EnvelopeMetadata() { Source = new AppIdentifier() { AppId = "Test" }, UniqueMessageId = Guid.NewGuid().ToString() } });
            Assert.IsTrue(receieveCounterHook1 == 2);
            Assert.IsTrue(receieveCounterHook2 == 2);
            await hook.UnsubscribeAsync();
            _dataStream.OnNext(new MessageEnvelope() { Payload = new EnvelopeData() { ChannelId = "1", Context = JObject.Parse(instrument) }, Meta = new EnvelopeMetadata() { Source = new AppIdentifier() { AppId = "Test" }, UniqueMessageId = Guid.NewGuid().ToString() } });
            Assert.IsTrue(receieveCounterHook1 == 2);
            Assert.IsTrue(receieveCounterHook2 == 3);
        }



        [Test]
        public async Task ShouldFetchSystemChannelFromBackplane()
        {
            _desktopAgentTransport.GetSystemChannelsAsync(default).ReturnsForAnyArgs(new List<Channel>() { new Channel("1", "system") });
            ClientMiddleware sut = _fixture.Create<ClientMiddleware>();
            await sut.GetSystemChannelsAsync();
            await _desktopAgentTransport.ReceivedWithAnyArgs(1).GetSystemChannelsAsync(default);
        }
    }
}
