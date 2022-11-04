/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */

import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import polly from 'polly-js';
import { Logger } from 'ts-log';
import { BackplaneDiscoveryServiceClient } from '../discovery/BackplaneDiscoveryServiceClient';
import { createAndStartHubConnectionAsync } from '../utility/BackplaneTransportUtil';

export class BackplaneTransport {
	private backplaneServiceDiscovery: BackplaneDiscoveryServiceClient;
	private hubConnection: HubConnection | undefined;
	private onConnectHandlers: { (): void }[] = [];
	private logger: Logger;

	constructor(backplaneServiceDiscovery: BackplaneDiscoveryServiceClient, logger: Logger) {
		this.logger = logger;
		this.backplaneServiceDiscovery = backplaneServiceDiscovery;
	}

	async initializeAsync(retryCount: number, retryIntervalInMs: number): Promise<void> {
		this.logger.info(
			`Initializing backplane connection with retry count: ${retryCount} and retryIntervalInMs: ${retryIntervalInMs}`
		);
		await polly()
			.waitAndRetry(Array(retryCount).fill(retryIntervalInMs))
			.executeForPromise(
				async () =>
					(this.hubConnection = await createAndStartHubConnectionAsync(
						this.logger,
						this.backplaneServiceDiscovery,
						this.hubConnection,
						this.onConnectHandlers
					))
			);
	}

	registerOnConnect(onConnect: () => void): void {
		this.onConnectHandlers.push(onConnect);
		this.logger.info(`SignalR: Registered for callback on reconnect ..`);
	}

	getHubConnection(): HubConnection | undefined {
		return this.hubConnection;
	}
}
