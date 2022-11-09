/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */

const {
  Fdc3Action,
} = require("../../src/Finos.Fdc3.Backplane.Client.JS/lib/DTO/MessageEnvelope");
var backplaneClient = require("../../src/Finos.Fdc3.Backplane.Client.JS/lib/index");
const instrument = {
  type: "fdc3.instrument",
  id: {
    ticker: "AAPL",
    ISIN: "US0378331005",
    FIGI: "BBG000B9XRY4",
  },
};

async function main(params) {
  console.log("***Setting up clients***");
  var backplaneClient1 = new backplaneClient.BackplaneClient({
    appIdentifier: {
      appId: "backplaneJSClient1",
    },
    url: "http://localhost:49201/backplane/v1.0",
  });
  var backplaneClient2 = new backplaneClient.BackplaneClient({
    appIdentifier: {
      appId: "backplaneJSClient2",
    },
    url: "http://localhost:49201/backplane/v1.0",
  });

  await backplaneClient1.connect(
    (msg) => {
      if (msg.type == Fdc3Action.Broadcast) {
        console.info(
          `Backplane Client1: Recived broadcast over channel: ${msg.payload.channelId}`
        );
      }
      console.info(JSON.stringify(msg));
    },
    (err) => {
      console.error(`Backplane Client1:Disconnected.${err}`);
    }
  );
  await backplaneClient2.connect(
    (msg) => {
      `Backplane Client2: Recived broadcast over channel: ${msg.payload.channelId}`;
      console.info(JSON.stringify(msg));
    },
    (err) => {
      console.error(`Backplane Client2:Disconnected.${err}`);
    }
  );

  var systemChannels = await backplaneClient1.getSystemChannels();
  console.info(`System channels: ${JSON.stringify(systemChannels)}`);

  await backplaneClient2.broadcast(instrument, "group1");

  console.info(`Disconnecting client1`);
  await backplaneClient1.Disconnect();

  console.info(
    `Broadcasting context by client2, but this has no effect as client1 has disconnected`
  );
  await backplaneClient2.broadcast(instrument, "group1");
  console.info("**DONE**");
}

main();
