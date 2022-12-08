# Backplane Client JS

Javascript client which allows web based desktop agents to connect and communicate with backplane through API.

## Supported Operations

- Broadcast context

## Usage example

```ts
import { BackplaneClient } from '@finos/fdc3-backplane-client';

const instrument = {
  type: "fdc3.instrument",
  id: {
    ticker: "AAPL",
    ISIN: "US0378331005",
    FIGI: "BBG000B9XRY4",
  },
};

var backplaneClient = new backplaneClient.BackplaneClient({
	appIdentifier: {
		appId: 'backplaneJSClient',
	},
	url: 'http://localhost:4475',
});

await backplaneClient.connect(
	//hook for receive message from backplane
	msg => {
		if (msg.type == Fdc3Action.Broadcast) {
			console.info(`Backplane Client: Recived broadcast over channel: ${msg.payload.channelId}`);
		}
		console.info(JSON.stringify(msg));
	},
	//hook on disconnection
	err => {
		console.error(`Backplane Client: Disconnected.${err}`);
	}
);
await backplaneClient.broadcast(instrument, "Channel 1");
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
