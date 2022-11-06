// See https://aka.ms/new-console-template for more information
using Finos.Fdc3.Backplane.Client;
using Finos.Fdc3.Backplane.Client.API;
using Finos.Fdc3.Backplane.Client.Extensions;
using Finos.Fdc3.Backplane.DTO.FDC3;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
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

var backplaneUrl = new Uri("http://localhost:49201/backplane/v1.0");
IBackplaneClient? backplaneClient1 = container.GetService<IBackplaneClient>();

System.Console.WriteLine($"**Initializing client Backplane_Client_1. Connecting with backplane**{Environment.NewLine}");
if (backplaneClient1 == null)
{
    Console.Error.WriteLine("Failed to resolve dependency.Exiting...");
    return;
}

IBackplaneClient? backplaneClient2 = container.GetService<IBackplaneClient>();
System.Console.WriteLine($"**Initializing client Backplane_Client_2. Connecting with backplane**{Environment.NewLine}");
if (backplaneClient2 == null)
{
    Console.Error.WriteLine("Failed to resolve dependency.Exiting...");
    return;
}

await backplaneClient1.InitializeAsync(new InitializeParams(backplaneUrl, new AppIdentifier() { AppId = "Backplane_Client_1" }), (msg) =>
{
    Console.WriteLine($"Backplane_Client_1: {JsonConvert.SerializeObject(msg)}");
}, async (ex) => { await Task.CompletedTask; Console.WriteLine($"Client disconnected. {ex}"); });


await backplaneClient2.InitializeAsync(new InitializeParams(backplaneUrl, new AppIdentifier() { AppId = "Backplane_Client_2" }), (msg) =>
{
    Console.WriteLine($"Backplane_Client_2: {JsonConvert.SerializeObject(msg)}");
}, async (ex) => { await Task.CompletedTask; Console.WriteLine($"Client disconnected. {ex}"); });

//get system channels
IEnumerable<Channel>? channels = await backplaneClient1.GetSystemChannelsAsync();
System.Console.WriteLine($"**Populated channels:{string.Join(", ", channels.Select(x => $"Id {x.Id} and Name: {x.DisplayMetadata.Name}"))}**{Environment.NewLine}");

/*
 * * BROADCAST WORK FLOW
*/
System.Console.WriteLine($"**Running Scenario for Broadcast**{Environment.NewLine}");

System.Console.WriteLine($"**Broadcasting context: fdc3.instrument**{Environment.NewLine}");
await backplaneClient2.BroadcastAsync(new Context(JObject.Parse(instrument)), "");

await backplaneClient1.DisposeAsync();
System.Console.WriteLine($"**Backplane_Client_1 disposed**{Environment.NewLine}");
System.Console.WriteLine($"**Broadcasting context: fdc3.instrument**{Environment.NewLine}. This will have no effect");
await backplaneClient2.BroadcastAsync(new Context(JObject.Parse(instrument)), "");


Console.WriteLine("**Done**");

Console.ReadLine();