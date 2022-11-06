using AutoFixture;
using Finos.Fdc3.Backplane.Client.Extensions;
using Finos.Fdc3.Backplane.Client.Transport;
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
            serviceCollection.ConfigureBackplaneClient();
            Assert.IsTrue(serviceCollection.Count() > 5);
            Assembly assembly = Assembly.Load("Finos.Fdc3.Backplane.Client");
            IEnumerable<Type> interfaces = assembly.GetTypes().Where(x => x.IsInterface);
            ServiceProvider container = serviceCollection.BuildServiceProvider();
            Assert.IsTrue(interfaces.Except(new[] { typeof(ISignalRConnection) }).All(p => container.GetService(p) != null));



        }
    }
}
