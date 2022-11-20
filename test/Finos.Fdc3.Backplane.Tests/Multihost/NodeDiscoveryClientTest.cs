/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using AutoFixture;
using Finos.Fdc3.Backplane.MultiHost;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Tests.Multihost
{
    internal class NodeDiscoveryClientTest
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixtures.Create();
        }



        [Test]
        public async Task DiscoverAsyncShouldNotReturnNull()
        {
            NodesDiscoveryClient sut = _fixture.Create<NodesDiscoveryClient>();
            IEnumerable<Uri> result = await sut.DiscoverAsync();
            Assert.IsNotNull(result);
        }
    }
}
