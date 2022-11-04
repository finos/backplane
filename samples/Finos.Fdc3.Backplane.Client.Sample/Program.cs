// See https://aka.ms/new-console-template for more information
using Finos.Fdc3.Backplane.Client.API;
using Finos.Fdc3.Backplane.Client.Extensions;
using Finos.Fdc3.Backplane.DTO.FDC3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;

System.Console.ForegroundColor = ConsoleColor.Green;
string? instrument = @"{
                type: 'fdc3.instrument',
                id: {
                    ticker: 'AAPL',
                    ISIN: 'US0378331005',
                    FIGI: 'BBG000B9XRY4',
                },
              }";

//set up DI
ServiceCollection services = new ServiceCollection();
services.ConfigureBackplaneClient();
services.AddLogging(configure => { configure.AddConsole(); configure.SetMinimumLevel(LogLevel.None); });
ServiceProvider container = services.BuildServiceProvider();

IBackplaneClient? backplaneClient1 = container.GetService<IBackplaneClient>();
System.Console.WriteLine($"**Initializing client Example_Publisher. Connecting with backplane**{Environment.NewLine}");
if (backplaneClient1 == null)
{
    Console.Error.WriteLine("Failed to resolve dependency.Exiting...");
    return;
}

IBackplaneClient? backplaneClient2 = container.GetService<IBackplaneClient>();
System.Console.WriteLine($"**Initializing client Example_Subscriber. Connecting with backplane**{Environment.NewLine}");
if (backplaneClient2 == null)
{
    Console.Error.WriteLine("Failed to resolve dependency.Exiting...");
    return;
}

await backplaneClient1.InitializeAsync(new AppIdentifier() { AppId = "Backplane_Client_1" });
await backplaneClient2.InitializeAsync(new AppIdentifier() { AppId = "Backplane_Client_2" });

//get system channels
IEnumerable<IChannel>? channels = await backplaneClient1.GetSystemChannelsAsync();
System.Console.WriteLine($"**Populated channels:{string.Join(", ", channels.Select(x => $"Id {x.Id} and Name: {x.DisplayMetadata.Name}"))}**{Environment.NewLine}");

/*
                * BROADCAST WORK FLOW
                * Broadcast is done on a channel.
                * To send receive context message through broadcast, publisher and subscriber must be on same channel.
                * Join Channel
                * AddContextListener
                * Broadcast.
                */
System.Console.WriteLine($"**Running Scenario for Broadcast**{Environment.NewLine}");


//subscription can be done earlier. Channel can be joined later.
System.Console.WriteLine($"**Subscribing context: fdc3.instrument**{Environment.NewLine}");
IListener? broadcastSubscription = await backplaneClient1.AddContextListenerAsync("fdc3.instrument", (ctx) =>
{
    System.Console.WriteLine($"Example_Subscriber1: Receieved context: {ctx}{Environment.NewLine}");
});

//Join Channel

Console.WriteLine($"Joining channel group1");
//subscriber on same channel.
await backplaneClient1.JoinChannelAsync("group1");
//publisher on same channel.
await backplaneClient2.JoinChannelAsync("group1");

System.Console.WriteLine($"**Broadcasting context: fdc3.instrument**{Environment.NewLine}");
await backplaneClient2.BroadcastAsync(new Context(JObject.Parse(instrument)));

Console.WriteLine("Enter to unsubscribe");
Console.ReadLine();
//Unsubscribe
System.Console.WriteLine($"Unsubscribing Example_Subscriber from context fdc3.instrument{Environment.NewLine}");
await broadcastSubscription.UnsubscribeAsync();

System.Console.WriteLine($"**Broadcasting context: fdc3.instrument.No subscriber now, so nothing would be printed**{Environment.NewLine}");
await backplaneClient2.BroadcastAsync(new Context(JObject.Parse(instrument)));

Console.ReadLine();