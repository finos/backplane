/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */

import { BackplaneTransport } from '../transport/BackplaneTransport';
import { Logger } from 'ts-log';
import { Context, ContextHandler, Listener } from '@finos/fdc3';
import {
	buildBroadcastEnvelop,
	getBroadcastContextMapKey,
	onHubConnection,
	unsubscribeUtil,
} from '../utility/ClientMiddlewareUtils';
import { HubConnection } from '@microsoft/signalr';
import { AppIdentifier } from '../DTO/receive/MessageEnvelope';
import { BackplaneDiscoveryServiceClient } from '../discovery/BackplaneDiscoveryServiceClient';
import { ChannelsRepository } from '../repository/ChannelsRepository';
import { Fdc3Channel } from '../broadcast/channels/ChannelClient';

export class ClientMiddleware {
	private backplaneTransport: BackplaneTransport;
	private contextHandlers: Map<string, ContextHandler[]>;
	private logger: Logger;
	private appIdentifier: AppIdentifier;
	private channelsRepository: ChannelsRepository;

	constructor(appIdentifier: AppIdentifier, logger: Logger) {
		this.backplaneTransport = new BackplaneTransport(new BackplaneDiscoveryServiceClient(logger), logger);
		this.channelsRepository = new ChannelsRepository(this.backplaneTransport, this, logger);
		this.appIdentifier = appIdentifier;
		this.contextHandlers = new Map<string, ContextHandler[]>();
		this.backplaneTransport.registerOnConnect(() =>
			onHubConnection(this.logger, this.backplaneTransport, this.contextHandlers)
		);
		this.logger = logger;
	}

	getSystemChannels(): Fdc3Channel[] {
		return this.channelsRepository.getSystemChannels();
	}

	async initializeAsync(retryCount: number, retryIntervalInMs: number): Promise<void> {
		await this.backplaneTransport.initializeAsync(retryCount, retryIntervalInMs);
		await this.channelsRepository.InitializeAsync(retryCount, retryIntervalInMs);
	}

	broadcastContext(context: Context, channelId: string): void {
		let hubConn: HubConnection | undefined = this.backplaneTransport.getHubConnection();
		if (hubConn == undefined) throw Error('Invalid operation: Transport not initialized');
		hubConn.invoke(
			'Broadcast',
			JSON.parse(buildBroadcastEnvelop(this.appIdentifier, context, channelId, this.logger)),
			false
		);
	}

	addContextListener(contextType: string | undefined, handler: ContextHandler, channelId: string): Listener {
		this.logger.info(`Registering contextType: ${contextType} for listening from backplane`);
		let key: string = getBroadcastContextMapKey(contextType, channelId);
		if (this.contextHandlers.has(key)) {
			this.logger.info(`ContextType already found. Registering handler..`);
			this.contextHandlers.get(key)?.push(handler);
			this.logger.info(`Registered handler successfully: ${contextType}`);
		} else {
			this.contextHandlers.set(key, [handler]);
			this.logger.info(`Registered contextType and handler`);
		}

		return {
			unsubscribe: (): void => {
				this.logger.info(`Unsubscribe called for contextType: ${key}`);
				let item: ContextHandler[] | undefined = this.contextHandlers.get(key);
				unsubscribeUtil(this.logger, item, 'contextType', key, handler);
			},
		};
	}
}
