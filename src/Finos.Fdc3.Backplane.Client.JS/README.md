![backplane logo](../../docs/resources/BackplaneIcon.png)

# Backplane Client JS

Javascript client which allows web based desktop agents to connect and communicate with backplane through API.

## Supported Operations

- Broadcast context.

## Usage example

```ts

const backplaneClient= require("@finos/fdc3-backplane-client");

const instrument = {
  type: "fdc3.instrument",
  id: {
    ticker: "AAPL",
    ISIN: "US0378331005",
    FIGI: "BBG000B9XRY4",
  },
};

var desktopAgentBackplaneClient = new backplaneClient.BackplaneClient({
	appIdentifier: {
		appId: 'desktopAgent',
	},
	url: 'http://localhost:4475',
});

await desktopAgentBackplaneClient.connect(
	//hook for receive message from backplane
	msg => {
		if (msg.type == backplaneClient.Fdc3Action.Broadcast) {
			console.info(`desktopAgent: Recived broadcast over channel: ${msg.payload.channelId}`);
		}
		console.info(JSON.stringify(msg));
	},
	//hook on disconnection
	err => {
		console.error(`desktopAgent: Backplane disconnected.${err}`);
	}
);
await desktopAgentBackplaneClient.broadcast(instrument, "Channel 1");
```

## Installation
To access the APIs in your application, simply install '@finos/fdc3-backplane-client' npm package:

```sh

# npm
npm install @finos/fdc3-backplane-client

#yarn
yarn add @finos/fdc3-backplane-client

```

Use any of below to locally build and reference this package in your web application.

- [npm-link](https://docs.npmjs.com/cli/link)
- [yarn-link](https://classic.yarnpkg.com/en/docs/cli/link/)

## License

Copyright (C) 2022 Backplane open source project

Distributed under the [Apache License, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0).

SPDX-License-Identifier: [Apache-2.0](https://spdx.org/licenses/Apache-2.0)
