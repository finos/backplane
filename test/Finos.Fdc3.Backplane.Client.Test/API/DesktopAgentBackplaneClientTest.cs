/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using AutoFixture;
using Finos.Fdc3.Backplane.Client.API;
using Finos.Fdc3.Backplane.Client.Transport;
using Finos.Fdc3.Backplane.DTO.FDC3;
using NSubstitute;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.Test.API
{
    public class DesktopAgentBackplaneClientTest
    {

        private Fixture _fixture;
        private IBackplaneTransport _backplaneTransport;


        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixture.Create();
            _backplaneTransport = _fixture.Freeze<IBackplaneTransport>();
            _backplaneTransport.GetSystemChannelsAsync().Returns(new Channel[] { new Channel("channel1", "system") });
        }


        [Test]
        public void ShouldNotConnectToBackplaneOnObjectCreation()
        {

            _fixture.Create<BackplaneClient>();
            _backplaneTransport.DidNotReceiveWithAnyArgs().ConnectAsync(default, default, default, default);

        }

        [Test]
        public async Task ShouldSetInitializeFlagOnceOnly()
        {
            BackplaneClient sut = _fixture.Create<BackplaneClient>();
            AppIdentifier appIdentifier = new AppIdentifier() { AppId = "Test" };
            await sut.InitializeAsync(new InitializeParams(new Uri("http://address"), new AppIdentifier() { AppId = "DA1" }), (x) => { }, async (ex) => { await Task.CompletedTask; });
            await sut.InitializeAsync(new InitializeParams(new Uri("http://address"), new AppIdentifier() { AppId = "DA1" }), (x) => { }, async (ex) => { await Task.CompletedTask; });
            await _backplaneTransport.ReceivedWithAnyArgs(1).ConnectAsync(default, default, default, default);
        }





        [Test]
        public async Task GetSystemChannelsShouldReturnPopulatedChannel()
        {
            BackplaneClient sut = _fixture.Create<BackplaneClient>();
            await sut.InitializeAsync(new InitializeParams(new Uri("http://address"), new AppIdentifier() { AppId = "DA1" }), (x) => { }, async (ex) => { await Task.CompletedTask; });
            Assert.AreEqual(sut.GetSystemChannelsAsync().Result.Count(), 1);
        }

    }
}