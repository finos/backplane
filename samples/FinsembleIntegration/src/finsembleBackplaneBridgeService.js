/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */

/**
 *  Below is sample code snippet which demonstrate how finsemble and backplane are connected for message exchange.
 */
import { BackplaneClient } from "@finos/fdc3-backplane-client";
const Logger = FSBL.Clients.Logger;

/**
 * Invoked when FSBL has been injected and is ready to use.
 */
 const FSBLReady = async () => {
    // Finsemble's startup sequence is paused if this app has `waitForInitialization` set to true
    // Perform some initialization tasks, wait for events, etc, then publish ready
	if (window.fdc3) {
		setUpBridge();
	 } else {
		window.addEventListener("fdc3Ready", setUpBridge());
	 }
    FSBL.publishReady();
	Logger.log("Finsemble backplane bridge is ready");
 
    // Now Finsemble will continue its startup sequence
};

 const setUpBridge= async()=> {
    Logger.log("Fdc3 ready!. Setting up bridge...");
    var backplaneUrl= "http://localhost:49201/backplane/v1.0";
    try {
      this.bridgeServiceName = WindowClient.getWindowIdentifier().windowName;
      Logger.log(`Creating backplane client..`);
      var backplaneClient = new BackplaneClient({
        appIdentifier: {
          appId: bridgeServiceName,
        },
        url: backplaneUrl,
      });

      Logger.log(`Connecting with backplane at ${backplaneUrl}`);
      await backplaneClient.connect(
        (msg) => {
          // handle broadcast
          if (msg.type == Fdc3Action.Broadcast) {
            console.info(
              `Recived broadcast over channel: ${msg.payload.channelId}`
            );
            var channel = await window.fdc3.getOrCreateChannel(msg.payload.channelId);
            channel?.broadcast(msg.payload.context)
          }
          console.info(JSON.stringify(msg));
        },
        (err) => {
          console.error(`Disconnected from backplane.${err}`);
        }
      );
      subscribeToFinsembleBroadcast();
    } catch (err) {
      Logger.error(`Error setting up finsemble backplane bridge: ${err}`);
    }
  }

  const subscribeToFinsembleBroadcast = () => {
	Logger.log("Attaching to router for listening broadcast in finsemble..");
	const handler = (
		args,
		response
	) => {

		try {
			var channelId = response.data.channel;
			Logger.log(`Received broadcast context from source: ${response.data.source} on finsemble router: ${JSON.stringify(response.data)} over channel ${channelId}.Sending it to backplane`);
			console.log(response.data);
			backplaneClient.broadcast(response.data.context,channelId);
			Logger.log("Successfully sent broadcast context to backplane.");
		} catch (err) {
			Logger.error(`Failed to send broadcast context to backplane: ${err}`);
		}
	} 
	FSBL.Clients.RouterClient.addListener('FDC3.broadcast', handler);
}
