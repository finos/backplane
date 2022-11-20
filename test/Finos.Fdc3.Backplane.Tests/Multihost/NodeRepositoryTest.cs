/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using AutoFixture;
using Finos.Fdc3.Backplane.MultiHost;
using NUnit.Framework;
using System;
using System.Linq;

namespace Finos.Fdc3.Backplane.Tests.Multihost
{
    internal class NodeRepositoryTest
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixtures.Create();
        }

        [Test]
        public void ShouldAddNode()
        {
            //Arrange
            Uri url = new Uri("http://url1");
            NodesRepository sut = _fixture.Create<NodesRepository>();
            //Act
            sut.AddNode(url);
            //Assert
            Assert.AreEqual(1, sut.MemberNodes.Count());
            Assert.AreEqual(url, sut.MemberNodes.ToList().First());
        }

        [Test]
        public void ShouldNotAddDuplicateNode()
        {
            //Arrange
            Uri url = new Uri("http://url1");
            NodesRepository sut = _fixture.Create<NodesRepository>();
            sut.AddNode(url);
            sut.AddNode(url);
            //Assert
            Assert.AreEqual(1, sut.MemberNodes.Count());
            Assert.AreEqual(url, sut.MemberNodes.ToList().First());
        }

        [Test]
        public void ShouldRemoveNode()
        {
            //Arrange
            Uri url = new Uri("http://url1");
            NodesRepository sut = _fixture.Create<NodesRepository>();
            //Act
            sut.AddNode(url);
            //Assert
            Assert.AreEqual(1, sut.MemberNodes.Count());
            Assert.AreEqual(url, sut.MemberNodes.ToList().First());
            sut.RemoveNode(url);
            Assert.AreEqual(0, sut.MemberNodes.Count());
        }

        [Test]
        public void MultipleRemoveShouldNotThrowException()
        {
            //Arrange
            Uri url = new Uri("http://url1");
            NodesRepository sut = _fixture.Create<NodesRepository>();
            //Act
            sut.AddNode(url);
            //Assert
            Assert.AreEqual(1, sut.MemberNodes.Count());
            Assert.AreEqual(url, sut.MemberNodes.ToList().First());
            sut.RemoveNode(url);
            sut.RemoveNode(url);
            sut.RemoveNode(url);
            Assert.AreEqual(0, sut.MemberNodes.Count());

        }


        [Test]
        public void MemberNodesShouldBeImmutable()
        {
            //Arrange
            Uri url1 = new Uri("http://url1");
            Uri url2 = new Uri("http://url2");
            Uri url_not_present = new Uri("http://notpresent");
            NodesRepository sut = _fixture.Create<NodesRepository>();
            sut.AddNode(url1);
            System.Collections.Generic.IEnumerable<Uri> ref1 = sut.MemberNodes;
            System.Collections.Generic.IEnumerable<Uri> ref2 = sut.MemberNodes;
            Assert.IsFalse(ReferenceEquals(ref2, ref1));
        }
    }
}
