using AutoFixture;
using Finos.Fdc3.Backplane.Client.Resilliency;
using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.Test.Resilliency
{
    public class RetryPolicyProviderTest
    {
        private Fixture _fixture;

        [SetUp]
        public void Setup()
        {
            _fixture = AutoFixture.Create();
        }

        [Test]
        public async Task ShouldReturnAsyncPolicyForRetryWithRetryCountAndInterval()
        {
            RetryPolicyProvider sut = _fixture.Create<RetryPolicyProvider>();
            Polly.IAsyncPolicy retryPolicy = sut.GetAsyncRetryPolicy<Exception>(3, (t) => TimeSpan.FromMilliseconds(10));
            int retryCounter = 0;
            await retryPolicy.ExecuteAsync(async () =>
            {
                retryCounter++;
                if (retryCounter < 3)
                {
                    throw new Exception("Exception simulation");
                }

                await Task.CompletedTask;
            });
            Assert.IsTrue(retryCounter == 3);
        }
    }
}
