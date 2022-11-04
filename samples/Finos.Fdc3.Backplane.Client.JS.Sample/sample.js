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
  var subscriber = new backplaneClient.BackplaneClient({
    appId: "Example_JS",
  });

  var publisher = new backplaneClient.BackplaneClient({
    appId: "Example_JS",
  });

  await subscriber.initializeAsync();
  await publisher.initializeAsync();

  var brdUnsubs1 = subscriber.addContextListener((ctx) =>
    console.log(
      `%cSubscriber1: Receieved context ${JSON.stringify(ctx)}`,
      "color:green"
    )
  );
  await subscriber.joinChannel("group1");
  publisher.broadcast(instrument);
}

main();
