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

  console.log("***Setting up client***");
  var backplaneUrl= "http://localhost:4475";
  var desktopAgentConsole = new backplaneClient.BackplaneClient({
    appIdentifier: {
      appId: "desktopAgentConsole",
    },
    url: backplaneUrl,
  });
  

  await desktopAgentConsole.connect(
    (msg) => {
      if (msg.type == backplaneClient.Fdc3Action.Broadcast) {
        console.info(
          `desktopAgentConsole: Recived broadcast over channel: ${msg.payload.channelId}`
        );
      }
      console.info(JSON.stringify(msg));
    },
    (err) => {
      console.error(`desktopAgentConsole:Disconnected.${err}`);
    }
  );
  

  var userChannels = await desktopAgentConsole.getUserChannels();
  console.info(`User channels: ${JSON.stringify(userChannels)}`);

  await desktopAgentConsole.broadcast(instrument, "Channel 1");

  console.info(`Disconnecting desktopAgentConsole`);
  await desktopAgentConsole.Disconnect();

  console.info("**DONE**");
}

main();
