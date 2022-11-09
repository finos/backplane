# Backplane Client JS

Javascript client which allows web based desktop agents to connect and communicate with backplane through API.

## Supported Operations

- Broadcast context

## Usage example

```ts
import { BackplaneClient } from "@finos/fdc3-backplane-client";

var backplaneClient = new backplaneClient.BackplaneClient({
  appIdentifier: {
    appId: "backplaneJSClient",
  },
  url: "http://localhost:49201/backplane/v1.0",
});

await backplaneClient.connect(
  //hook for receive message from backplane
  (msg) => {
    if (msg.type == Fdc3Action.Broadcast) {
      console.info(
        `Backplane Client: Recived broadcast over channel: ${msg.payload.channelId}`
      );
    }
    console.info(JSON.stringify(msg));
  },
  //hook on disconnection
  (err) => {
    console.error(`Backplane Client: Disconnected.${err}`);
  }
);
```

## Installation

1. Open folder '\src\Finos.Fdc3.Backplane.Client.JS' in vs code.
2. Run command 'yarn run buildPack' in new terminal.
3. npm package should be created in 'output\jsclient' folder at root.

   To access the APIs in your application, simply install '@finos/fdc3-backplane-client' npm package:

```sh

# npm
npm install file:/path/to/local/folder/@finos/fdc3-backplane-client

#yarn
yarn add file:/path/to/local/folder/@finos/fdc3-backplane-client

```

Use any of below to locally build and reference this package in your web application.

- [npm-link](https://docs.npmjs.com/cli/link)
- [yarn-link](https://classic.yarnpkg.com/en/docs/cli/link/)

## License

Copyright (C) 2022 Backplane open source project

Distributed under the [Apache License, Version 2.0](http://www.apache.org/licenses/LICENSE-2.0).

SPDX-License-Identifier: [Apache-2.0](https://spdx.org/licenses/Apache-2.0)
