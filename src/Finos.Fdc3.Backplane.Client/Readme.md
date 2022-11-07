# Backplane Client .NET

Exposes API to send/receive FDC3 compliant messages over backplane for context sharing across apps and desktop agents.

## Supported Platforms

The library is written in .NET standard 2.0.
.NET Standard 2.0 is supported by all modern platforms and is the recommended way to support multiple platforms with one target.

## Supported Features

- Broadcast context

## Installation

Nuget package is generated as part of local build in output directory at root.

1.  Open 'Finos.Fdc3.Backplane.sln' at root in visual studio.
2.  Build Solution.
3.  Nuget package is created in 'output/netclient' folder at root.
4.  Add local path of nuget package in nuget.config.

        PM> Install-Package Finos.Fdc3.Backplane.Client

## Usage example

```C#
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

Uri? backplaneUrl = new Uri("http://localhost:49201/backplane/v1.0");
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

await backplaneClient1.InitializeAsync(new InitializeParams(backplaneUrl,
new AppIdentifier() { AppId = "Backplane_Client_1" }),
//hook for message receive
(msg) =>
{
    Console.WriteLine($"Backplane_Client_1: {JsonConvert.SerializeObject(msg)}");
},
//hook for disconnect
async (ex) => { await Task.CompletedTask; Console.WriteLine($"Client disconnected. {ex}");
 });


await backplaneClient2.InitializeAsync(new InitializeParams(backplaneUrl,
 new AppIdentifier() { AppId = "Backplane_Client_2" }),
 //hook for message receive
(msg) =>
{
    Console.WriteLine($"Backplane_Client_2: {JsonConvert.SerializeObject(msg)}");
},
//hook for disconnect
async (ex) =>
{ await Task.CompletedTask; Console.WriteLine($"Client disconnected. {ex}");
});

//get system channels
IEnumerable<Channel>? channels = await backplaneClient1.GetSystemChannelsAsync();
System.Console.WriteLine($"**Populated channels:{string.Join(", ", channels.Select(x => $"Id {x.Id} and Name: {x.DisplayMetadata.Name}"))}**{Environment.NewLine}");

/*
 * * BROADCAST WORK FLOW
*/
System.Console.WriteLine($"**Broadcasting context: fdc3.instrument**{Environment.NewLine}");
await backplaneClient2.BroadcastAsync(new Context(JObject.Parse(instrument)), "");

await backplaneClient1.DisposeAsync();
System.Console.WriteLine($"**Backplane_Client_1 disposed**{Environment.NewLine}");
System.Console.WriteLine($"**Broadcasting context: fdc3.instrument**{Environment.NewLine}. This will have no effect");
await backplaneClient2.BroadcastAsync(new Context(JObject.Parse(instrument)), "");
}

```

## License

Copyright (C) 2022 Backplane open source project

Distributed under the [Apache License, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0).

SPDX-License-Identifier: [Apache-2.0](https://spdx.org/licenses/Apache-2.0)
