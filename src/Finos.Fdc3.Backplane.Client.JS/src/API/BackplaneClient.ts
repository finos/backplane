/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */

import { Channel, Context } from '@finos/fdc3';
import { AppIdentifier, Fdc3Action, MessageEnvelope } from '../DTO/MessageEnvelope';
import { BackplaneClientTransport } from '../transport/BackplaneTransport';
import { InitializeParams } from './initializeParams';
import { randomBytes } from 'crypto';

export class BackplaneClient {
  private backplaneClientService: BackplaneClientTransport;
  private appIdentifier: AppIdentifier = { appId: '' };
  private systemChannels: Channel[];

  constructor(initializeParams: InitializeParams) {
    this.backplaneClientService = new BackplaneClientTransport(initializeParams);
    this.systemChannels = [];
  }

  /**
   * Connect with backplane
   *
   * @param {{ (msg: MessageEnvelope): void }} onMessage
   * @param {{ (error?: Error): void }} onDisconnect
   * @return {*}
   * @memberof BackplaneClient
   */
  public async connect(onMessage: { (msg: MessageEnvelope): void }, onDisconnect: { (error?: Error): void }) {
    this.appIdentifier = await this.backplaneClientService.connect(onMessage, onDisconnect);
    this.systemChannels = await this.getSystemChannels();
    return this.appIdentifier;
  }

  /**
   * Broadcast context
   * @param {Context} context
   * @param {string} channelId
   * @memberof DesktopAgentBackplaneClient
   */
  public async broadcast(context: Context, channelId: string) {
    var message: MessageEnvelope = {
      type: Fdc3Action.Broadcast,
      payload: { channelId: channelId, context: context },
      meta: { source: this.appIdentifier, uniqueMessageId: randomBytes(20).toString('hex') },
    };
    await this.backplaneClientService.broadcast(message);
  }

  /**
   * Get system channels exposed by backplane
   * @memberof BackplaneClient
   */
  public getSystemChannels() {
    return this.systemChannels;
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
