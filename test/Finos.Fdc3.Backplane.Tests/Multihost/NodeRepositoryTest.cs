using AutoFixture;
using Finos.Fdc3.Backplane.MultiHost;
using Microsoft.Extensions.Configuration;
using NSubstitute;
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
        public void Test_MemberNodesRepository_AddOrUpdateActiveNode_AddsNodeToList()
        {
            //Arrange
            Uri url = new Uri("http://url1");
            IConfiguration configuration = _fixture.Freeze<IConfiguration>();
            configuration[Arg.Any<string>()].Returns("10");
            NodesRepository sut = _fixture.Create<NodesRepository>();
            //Act
            sut.CurrentNodeUri = url;
            sut.AddOrUpdateActiveNode(url);
            //Assert
            Assert.AreEqual(1, sut.MemberNodes.Count());
            Assert.AreEqual(url, sut.MemberNodes.ToList().First().Uri);
            Assert.AreEqual(url, sut.CurrentNodeUri);
        }

        [Test]
        public void Test_MemberNodesRepository_AddOrUpdateActiveNode_ActivatesDeactivatedNodePresent()
        {
            //Arrange
            Uri url = new Uri("http://url1");
            IConfiguration configuration = _fixture.Freeze<IConfiguration>();
            configuration[Arg.Any<string>()].Returns("10");
            NodesRepository sut = _fixture.Create<NodesRepository>();
            sut.AddOrUpdateDeactiveNode(url);
            //Act
            sut.CurrentNodeUri = url;
            sut.AddOrUpdateActiveNode(url);
            //Assert
            Assert.AreEqual(1, sut.MemberNodes.Count());
            Assert.AreEqual(url, sut.MemberNodes.ToList().First().Uri);
            Assert.IsTrue(sut.MemberNodes.First().IsActive);
            Assert.AreEqual(url, sut.CurrentNodeUri);
        }

        [Test]
        public void Test_MemberNodesRepository_DuplicateAddOrUpdateActiveNodeHandled()
        {
            //Arrange
            Uri url = new Uri("http://url1");
            IConfiguration configuration = _fixture.Freeze<IConfiguration>();
            configuration[Arg.Any<string>()].Returns("10");
            NodesRepository sut = _fixture.Create<NodesRepository>();
            //Act
            sut.AddOrUpdateActiveNode(url);
            sut.AddOrUpdateActiveNode(url);
            //Assert
            Assert.AreEqual(1, sut.MemberNodes.Count());
            Assert.AreEqual(url, sut.MemberNodes.ToList().First().Uri);
        }

        [Test]
        public void Test_MemberNodesRepository_AddOrUpdateDeactiveNode_WorksAsExpectedOnExistingNodeInList()
        {
            //Arrange
            Uri url = new Uri("http://url1");
            IConfiguration configuration = _fixture.Freeze<IConfiguration>();
            configuration[Arg.Any<string>()].Returns("10");
            NodesRepository sut = _fixture.Create<NodesRepository>();
            sut.AddOrUpdateActiveNode(url);
            //Act
            sut.AddOrUpdateDeactiveNode(url);
            //Assert
            Assert.IsTrue(!sut.MemberNodes.First().IsActive);
        }


        [Test]
        public void Test_MemberNodesRepository_AddOrUpdateDeactiveNode_AddsDeactiveNodeIfNotPresent()
        {
            //Arrange
            Uri url1 = new Uri("http://url1");
            Uri url2 = new Uri("http://url2");
            Uri url_not_present = new Uri("http://notpresent");
            IConfiguration configuration = _fixture.Freeze<IConfiguration>();
            configuration[Arg.Any<string>()].Returns("10");
            NodesRepository sut = _fixture.Create<NodesRepository>();
            sut.AddOrUpdateActiveNode(url1);
            sut.AddOrUpdateActiveNode(url2);
            //Act
            sut.AddOrUpdateDeactiveNode(url_not_present);
            var memberNodes = sut.MemberNodes.ToList();
            //Assert
            Assert.IsTrue(memberNodes[0].IsActive);
            Assert.IsTrue(memberNodes[1].IsActive);
            Assert.IsTrue(!memberNodes[2].IsActive);
            Assert.AreEqual(memberNodes.Count, 3);
        }
    }
}
