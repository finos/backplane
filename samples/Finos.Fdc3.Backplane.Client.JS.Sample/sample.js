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
  var backplaneClient1 = new backplaneClient.BackplaneClient();
  var backplaneClient2 = new backplaneClient.BackplaneClient();

  await backplaneClient1.initialize(
    {
      appIdentifier: {
        appId: "Example_JS",
      },
      url: "http://localhost:49201/backplane/v1.0",
    },
    (msg) => {
      if (msg.type == Fdc3Action.Broadcast) {
        console.info(
          `Backplane Client1: Recived broadcast over channel: ${msg.payload.channelId}`
        );
      }
      console.info(JSON.stringify(msg));
    },
    (err) => {
      console.error(`Disconnected.${err}`);
    }
  );
  await backplaneClient2.initialize(
    {
      appIdentifier: {
        appId: "Example_JS",
      },
      url: "http://localhost:49201/backplane/v1.0",
    },
    (msg) => {
      `Backplane Client2: Recived broadcast over channel: ${msg.payload.channelId}`;
      console.info(JSON.stringify(msg));
    },
    (err) => {
      console.error(`Disconnected.${err}`);
    }
  );

  var systemChannels = await backplaneClient1.getSystemChannels();
  console.info(`System channels: ${JSON.stringify(systemChannels)}`);

  await backplaneClient2.broadcast(instrument, "group1");
  var context = await backplaneClient2.getCurrentContext("group1");
  console.info(`Current context: ${JSON.stringify(context)}`);
}

main();
