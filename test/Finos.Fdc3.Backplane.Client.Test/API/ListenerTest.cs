using Finos.Fdc3.Backplane.Client.API;
using NUnit.Framework;
using System.Threading.Tasks;

namespace Finos.Fdc3.Backplane.Client.Test.API
{
    public class ListenerTest
    {

        [Test]
        public async Task ShouldUnsubscribe()
        {
            bool unSubscribed = false;
            Listener sut = new Listener(async (token) => { unSubscribed = true; await Task.CompletedTask; });
            await sut.UnsubscribeAsync();
            Assert.IsTrue(unSubscribed);
        }
    }
}
