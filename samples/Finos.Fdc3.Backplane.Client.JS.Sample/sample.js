/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */

const backplaneClient= require("@finos/fdc3-backplane-client");
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
  var backplaneUrl= "http://localhost:4475";
  var backplaneClient1 = new backplaneClient.BackplaneClient({
    appIdentifier: {
      appId: "backplaneJSClient1",
    },
    url: backplaneUrl,
  });
  var backplaneClient2 = new backplaneClient.BackplaneClient({
    appIdentifier: {
      appId: "backplaneJSClient2",
    },
    url: backplaneUrl,
  });

  await backplaneClient1.connect(
    (msg) => {
      if (msg.type == backplaneClient.Fdc3Action.Broadcast) {
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
      if (msg.type == backplaneClient.Fdc3Action.Broadcast) {
        console.info(
          `Backplane Client2: Recived broadcast over channel: ${msg.payload.channelId}`
        );
      }
      console.info(JSON.stringify(msg));
    },
    (err) => {
      console.error(`Backplane Client2:Disconnected.${err}`);
    }
  );

  var userChannels = await backplaneClient1.getUserChannels();
  console.info(`User channels: ${JSON.stringify(userChannels)}`);

  await backplaneClient2.broadcast(instrument, "fdc3.channel.1");

  console.info(`Disconnecting client1`);
  await backplaneClient1.Disconnect();

  console.info(
    `Broadcasting context by client2, but this has no effect as client1 has disconnected`
  );
  await backplaneClient2.broadcast(instrument, "fdc3.channel.1");
  console.info("**DONE**");
}

main();
