/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

using AutoFixture;
using Finos.Fdc3.Backplane.Client.Extensions;
using Finos.Fdc3.Backplane.Client.Transport;
using Finos.Fdc3.Backplane.DTO.FDC3;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Finos.Fdc3.Backplane.Client.Test.Extensions
{
    public class ServiceCollectionExtensionTest
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixture.Create();
        }

        [Test]
        public void ShouldRegisterAllRequiredServices()
        {
            ServiceCollection serviceCollection = new ServiceCollection();
            serviceCollection.AddLogging();
            InitializeParams initializeParam = new InitializeParams(new AppIdentifier() { AppId = "Test" });
            _fixture.Register(() => initializeParam);
            serviceCollection.ConfigureBackplaneClient(initializeParam, () => new Uri("http://test"));
            Assert.IsTrue(serviceCollection.Count() > 5);
            Assembly assembly = Assembly.Load("Finos.Fdc3.Backplane.Client");
            IEnumerable<Type> interfaces = assembly.GetTypes().Where(x => x.IsInterface);
            ServiceProvider container = serviceCollection.BuildServiceProvider();
            Assert.IsTrue(interfaces.Except(new[] { typeof(IBackplaneTransport) }).All(p => container.GetRequiredService(p) != null));
            Lazy<IBackplaneTransport> transport = container.GetService<Lazy<IBackplaneTransport>>();
            Assert.IsNotNull(transport);



        }
    }
}
