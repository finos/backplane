/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */

import { Context } from '@finos/fdc3';
import { AppIdentifier, Fdc3Action, MessageEnvelope } from '../DTO/MessageEnvelope';
import { BackplaneClientTransport } from '../transport/BackplaneTransport';
import { InitializeParams } from './initializeParams';
import { randomBytes } from 'crypto';

export class BackplaneClient {
	private backplaneClientService: BackplaneClientTransport | undefined;
	private appIdentifier: AppIdentifier = { appId: '' };

	public async initialize(
		initializeParams: InitializeParams,
		onMessage: { (msg: MessageEnvelope): void },
		onDisconnect: { (error?: Error): void }
	) {
		this.backplaneClientService = new BackplaneClientTransport(initializeParams);
		this.appIdentifier = await this.backplaneClientService.connect(initializeParams.url, onMessage, onDisconnect);
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
		await this.backplaneClientService?.broadcast(message);
	}

	/**
	 *
	 * @memberof BackplaneClient
	 */
	public async getSystemChannels() {
		return await this.backplaneClientService?.getSystemChannels();
	}

	public async getCurrentContext(channelId: string, contextType?: string) {
		return await this.backplaneClientService?.getCurrentContext(channelId, contextType);
	}

	public async Disconnect() {
		await this.backplaneClientService?.Disconnect();
	}
}
