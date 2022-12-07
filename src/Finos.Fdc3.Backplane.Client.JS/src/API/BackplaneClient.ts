/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */

import { Channel, Context } from '@finos/fdc3';
import { AppIdentifier, Fdc3Action, MessageEnvelope } from '../DTO/MessageEnvelope';
import { BackplaneClientTransport } from '../transport/BackplaneTransport';
import { InitializeParams } from './initializeParams';
import uuid from 'uuid-random';

/**
 * Backplane client exposing API to connect and communicate with backplane.
 */
export class BackplaneClient {
  private backplaneClientService: BackplaneClientTransport;
  private appIdentifier: AppIdentifier = { appId: '' };
  private userChannels: Channel[];

  /**
   * Creates an instance of BackplaneClient.
   * @param {InitializeParams} initializeParams
   * @memberof BackplaneClient
   */
  constructor(initializeParams: InitializeParams) {
    this.backplaneClientService = new BackplaneClientTransport(initializeParams);
    this.userChannels = [];
  }

  /**
   * Connect with backplane
   *
   * @param {{ (msg: MessageEnvelope): void }} onMessage The handler that will be invoked when message is recieved by this client.
   * @param {{ (error?: Error): void }} onDisconnect The handler that will be invoked when the connection is closed.
   * @return {*}
   * @memberof BackplaneClient
   */
  public async connect(onMessage: { (msg: MessageEnvelope): void }, onDisconnect: { (error?: Error): void }) {
    this.appIdentifier = await this.backplaneClientService.connect(onMessage, onDisconnect);
    this.userChannels = await this.backplaneClientService.getUserChannels();
    return this.appIdentifier;
  }

  /**
   * Broadcasts a context on the specified channel.
   * @param {Context} context context
   * @param {string} channelId channelId
   * @memberof DesktopAgentBackplaneClient
   */
  public async broadcast(context: Context, channelId: string) {
    var message: MessageEnvelope = {
      type: Fdc3Action.Broadcast,
      payload: { channelId: channelId, context: context },
      meta: { source: this.appIdentifier, requestGuid: uuid() },
    };
    await this.backplaneClientService.broadcast(message);
  }

  /**
   * Get user channels exposed by backplane
   * @memberof BackplaneClient
   */
  public getUserChannels() {
    return this.userChannels;
  }

  /**
   * Disconnect client from backplane
   *
   * @memberof BackplaneClient
   */
  public async Disconnect() {
    await this.backplaneClientService.Disconnect();
  }
}
