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
3.  Nuget package is created in 'output' folder at root.

        PM> Install-Package Finos.Fdc3.Backplane.Client

## Usage example

```C#

//Register services in IOC container.
//Microsoft.Extensions.DependencyInjection.IServiceCollection

ServiceCollection services = new ServiceCollection();
services.ConfigureBackplaneClient();
services.AddLogging(configure => configure.AddConsole());


//Inject the interface IBackplaneClient
public class ApplicationExample
{
    private readonly IBackplaneClient _backplaneClient;
    public ApplicationExample(IBackplaneClient backplaneClient)
	{
	    _backplaneClient=backplaneClient;
	}

	public async Task InitializeAsync()
	{
	  await  _backplaneClient.InitializeAsync(new AppIdentifier() { AppId = "Backplane_Client" });
	  // Client must first join a channel to broadcast
	  var systemChannels = await _backplaneClient.GetSystemChannelsAsync();
	  // Join channel.1
	  await _backplaneClient.JoinChannelAsync("channel1");
	}

	public void BroadcastContext()
	{
	    // declare FDC3-compliant data
		var instrument = @"{
		type: 'fdc3.instrument',
		id: {
			ticker: 'AAPL',
			ISIN: 'US0378331005',
			FIGI: 'BBG000B9XRY4',
		},
		}";
		var contextJObject = JObject.Parse(instrument);
		//Listerners subscribed on 'channel.1' channel would receive this context.
		_backplaneClient.Broadcast(new Context(contextJObject));
	}


	public IListener AddContextListener()
	{
	    //listen 'fdc3.instrument' context on joined channel 'channel.1'
		IListener unsubscriber =_backplaneClient.AddContextListener("fdc3.instrument",(context)=>
		{
		  //handle received context 'fdc3.instrument'.
		});
	}
}

```

## License

Copyright (C) 2022 Backplane open source project

Distributed under the [Apache License, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0).

SPDX-License-Identifier: [Apache-2.0](https://spdx.org/licenses/Apache-2.0)
