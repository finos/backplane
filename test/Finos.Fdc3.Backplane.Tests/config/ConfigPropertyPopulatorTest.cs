using AutoFixture;
using Finos.Fdc3.Backplane.Config;
using Finos.Fdc3.Backplane.DTO.FDC3;
using Finos.Fdc3.Backplane.Models.Config;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using NUnit.Framework;

namespace Finos.Fdc3.Backplane.Tests.config
{
    internal class ConfigPropertyPopulatorTest
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixtures.Create();
            
        }
        [Test]
        public void ShouldPopulateConfigProperties()
        {

        }
    }
}
