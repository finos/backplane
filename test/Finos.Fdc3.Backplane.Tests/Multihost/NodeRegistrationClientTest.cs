/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using AutoFixture;
using Finos.Fdc3.Backplane.MultiHost;
using NUnit.Framework;
using System;
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
            Uri uri = new Uri("http://abc.com/");
            await sut.RegisterAsync(uri);
            Assert.IsTrue(sut.CurrentNodeUri == uri);
        }


    }
}
