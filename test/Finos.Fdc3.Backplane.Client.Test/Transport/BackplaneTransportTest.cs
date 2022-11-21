/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using AutoFixture;
using Finos.Fdc3.Backplane.Client.Transport;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;
using System;

namespace Finos.Fdc3.Backplane.Client.Test.Transport
{
    public class BackplaneTransportTest
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixture.Create();
            _fixture.Freeze<HubConnection>();
            ServiceCollection serviceCollection = new ServiceCollection();
            ILogger<IBackplaneTransport> logger = _fixture.Freeze<ILogger<IBackplaneTransport>>();
            serviceCollection.AddSingleton<ILogger<IBackplaneTransport>>(logger);
            ServiceProvider provider = serviceCollection.BuildServiceProvider();
            provider.GetRequiredService<ILogger<IBackplaneTransport>>().Returns(logger);
            _fixture.Register<IServiceProvider>(() => provider);

        }

        [Test]
        public void GetSystemChannelsShouldThrowExceptionIfNotConnected()
        {
            SignalRBackplaneTransport sut = _fixture.Create<SignalRBackplaneTransport>();
            Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.GetUserChannelsAsync());
        }

        [Test]
        public void BroadcastContextShouldThrowExceptionIfNotConnected()
        {
            SignalRBackplaneTransport sut = _fixture.Create<SignalRBackplaneTransport>();
            Assert.ThrowsAsync<InvalidOperationException>(async () => await sut.BroadcastAsync(default, default));
        }


    }
}
