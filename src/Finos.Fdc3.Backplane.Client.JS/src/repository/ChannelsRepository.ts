/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */

import { Channel } from '@finos/fdc3';
import { Logger } from 'ts-log';
import { BackplaneTransport } from '../transport/BackplaneTransport';
import { ClientMiddleware } from '../service/ClientMiddleware';
import { Fdc3Channel } from '../broadcast/channels/ChannelClient';
import polly from 'polly-js';
import { HubConnection } from '@microsoft/signalr';

export class ChannelsRepository {
	private backplaneTransportClient: BackplaneTransport;
	clientMiddleware: ClientMiddleware;
	logger: Logger;
	systemChannel: Array<Fdc3Channel>;

	constructor(backplaneTransportClient: BackplaneTransport, clientMiddleware: ClientMiddleware, logger: Logger) {
		this.systemChannel = new Array<Fdc3Channel>();
		this.logger = logger;
		this.clientMiddleware = clientMiddleware;
		this.backplaneTransportClient = backplaneTransportClient;
	}

	async InitializeAsync(retryCount: number, retryIntervalinMs: number): Promise<void> {
		await polly()
			.waitAndRetry(Array(retryCount).fill(retryIntervalinMs))
			.executeForPromise(async () => {
				this.logger.info(`Fetching system channels from backplane ...`);
				let hubConn: HubConnection | undefined = this.backplaneTransportClient.getHubConnection();
				if (hubConn == undefined) throw Error('Invalid operation: Transport not initialized!');
				let channel: Channel[] = await hubConn.invoke<Array<Channel>>('GetSystemChannels');
				this.systemChannel = channel.map(x => new Fdc3Channel(x.id, x.type, this.clientMiddleware, x.displayMetadata));
				this.logger.info(`Successfully populated channels repository: ${this.systemChannel}`);
			});
	}

	getSystemChannels(): Fdc3Channel[] {
		return this.systemChannel;
	}
}
