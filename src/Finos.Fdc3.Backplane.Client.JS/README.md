# Backplane Client JS

Javascript client which allows web based desktop agents to connect and communicate with backplane through API.

## Supported Operations

- Broadcast context

### Example

```ts
import { DesktopAgentClient } from '@finos/fdc3-desktop-agent-backplane-client';

var backplaneClient = new backplaneClient.BackplaneClient({
	appId: 'Example_JS',
});

await backplaneClient.initializeAsync();
// declare FDC3-compliant data
const instrument = {
	type: 'fdc3.instrument',
	id: {
		ticker: 'AAPL',
		ISIN: 'US0378331005',
		FIGI: 'BBG000B9XRY4',
	},
};

// join the channel and broadcast data to subscribers
await backplaneClient.joinChannel('channel1');
backplaneClient.broadcast(instrument);

// set up a listener for incoming data
const listener = backplaneClient.addContextListener('fdc3.instrument', instrument => {
	// handle context received
});
```

### Installation

1. Open folder '\src\Finos.Fdc3.Backplane.Client.JS' in vs code.
2. Run command 'yarn prepare' in new terminal.
3. npm package should be created in 'output' folder at root.

   To access the APIs in your application, simply install fdc3-desktop-agent-backplane-client npm package:

```sh

# npm
npm install @finos/fdc3-desktop-agent-backplane-client

#yarn
yarn add @finos/fdc3-desktop-agent-backplane-client

```

Use any of below to locally build and reference this package in your web application.

- [npm-link](https://docs.npmjs.com/cli/link)
- [yarn-link](https://classic.yarnpkg.com/en/docs/cli/link/)

## License

Copyright (C) 2022 Backplane open source project

Distributed under the [Apache License, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0).

SPDX-License-Identifier: [Apache-2.0](https://spdx.org/licenses/Apache-2.0)
