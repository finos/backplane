/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */

/**
 *  Below is sample code snippet which demonstrate how finsemble and backplane are connected for message exchange.
 */
import { BackplaneClient, Fdc3Action } from "@finos/fdc3-backplane-client";
import { WindowClient } from "@finsemble/finsemble-core";

/**
 * Invoked when FSBL has been injected and is ready to use.
 */
 const FSBLReady = async () => {
  setUpBridge();
  // Finsemble's startup sequence is paused if this app has `waitForInitialization` set to true
  // Perform some initialization tasks, wait for events, etc, then publish ready
  FSBL.publishReady();

  // Now Finsemble will continue its startup sequence
};

if (window.FSBL && FSBL.addEventListener) {
  FSBL.addEventListener("onReady", FSBLReady);
} else {
  window.addEventListener("FSBLReady", FSBLReady);
}

 const setUpBridge= async()=> {
    var backplaneUrl= "http://localhost:4475";
    try {
      var backplaneClient = new BackplaneClient({
        appIdentifier: {
          appId: WindowClient.getWindowIdentifier().windowName,
        },
        url: backplaneUrl,
      });

      //connect with backplane.
      await backplaneClient.connect(
        //receive message from backplane
        async(msg) => {
          // handle broadcast
          if (msg.type == Fdc3Action.Broadcast) {
            console.info(
              `Recived broadcast over channel: ${msg.payload.channelId}`
            );
            // get the matching channel in finsemble and broadcast over that.
            var channel = await window.fdc3.getOrCreateChannel(msg.payload.channelId);
            channel?.broadcast(msg.payload.context)
          }
          console.info(JSON.stringify(msg));
        },
        (err) => {
          console.error(`Disconnected from backplane.${err}`);
        }
      );
      await subscribeToFinsembleBroadcast(backplaneClient);
    } catch (err) {
      console.error(`Disconnected from backplane.${err}`);
    }
  }

  //subscribe to broadcast in finsemble over user channels.
  const subscribeToFinsembleBroadcast = async (backplaneClient: BackplaneClient) => {
  var userChannels = await window.fdc3.getSystemChannels();
  //subscribe for each user channel to listen for all context types.
  userChannels.forEach(x=>
    {
      x.addContextListener(null,(ctx)=>	{
        backplaneClient.broadcast(ctx,x.id);
      })
    })
}
