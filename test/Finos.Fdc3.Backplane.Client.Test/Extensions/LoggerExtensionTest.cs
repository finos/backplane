/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */
using AutoFixture;
using Finos.Fdc3.Backplane.Client.Extensions;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace Finos.Fdc3.Backplane.Client.Test.Extensions
{
    public class LoggerExtensionTest
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixture.Create();
        }

        [Test]
        public void ShouldCreateLogProviderForExistingLogger()
        {
            ILogger logger = _fixture.Freeze<ILogger>();
            ILoggerProvider logProvider = logger.AsLoggerProvider();
            Assert.AreSame(logger, logProvider.CreateLogger(default));
        }
    }
}
