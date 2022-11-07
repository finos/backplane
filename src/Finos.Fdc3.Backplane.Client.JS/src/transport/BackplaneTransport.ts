/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */

import { Channel } from '@finos/fdc3';
import {
	HttpTransportType,
	HubConnection,
	HubConnectionBuilder,
	ILogger,
	JsonHubProtocol,
	LogLevel,
} from '@microsoft/signalr';
import { Context } from 'mocha';
import { InitializeParams } from '../API/initializeParams';
import { AppIdentifier, MessageEnvelope } from '../DTO/MessageEnvelope';

export class BackplaneClientTransport {
	private hubConnection: HubConnection | undefined;
	private logger: ILogger;
	private appIdentifier: AppIdentifier = { appId: '' };

	constructor(params: InitializeParams) {
		this.appIdentifier = params.appIdentifier;
		this.logger = params.logger ?? console;
	}

	/**
	 *
	 *
	 * @param {string} url
	 * @param {{ (msg: MessageEnvelope): void }} onMessage
	 * @param {{ (error?: Error): void }} onDisconnect
	 * @return {*}
	 * @memberof BackplaneClientTransport
	 */
	public async connect(
		url: string,
		onMessage: { (msg: MessageEnvelope): void },
		onDisconnect: { (error?: Error): void }
	) {
		this.hubConnection = await this.buildSignalRConnection(url);
		this.hubConnection?.on('OnMessage', onMessage);
		this.hubConnection?.onclose(onDisconnect);
		await this.hubConnection?.start();
		return this.appIdentifier;
	}

	/**
	 *
	 *
	 * @param {MessageEnvelope} msg
	 * @memberof BackplaneClientTransport
	 */
	public async broadcast(msg: MessageEnvelope) {
		await this.hubConnection?.invoke('Broadcast', msg);
	}

	public async getSystemChannels() {
		return await this.hubConnection?.invoke<Channel>('GetSystemChannels');
	}

	public async getCurrentContext(channelId: string, contextType?: string) {
		return await this.hubConnection?.invoke<Context>('GetCurrentContextForChannel', channelId);
	}

	public async Disconnect() {
		await this.hubConnection?.stop();
	}

	private async buildSignalRConnection(url: string) {
		this.logger.log(LogLevel.Information, `signalR: Building connection with url: ${url}`);
		var hubConnection = new HubConnectionBuilder()
			.withUrl(`${url}`, { skipNegotiation: true, transport: HttpTransportType.WebSockets })
			.configureLogging(this.logger)
			.withAutomaticReconnect()
			.withHubProtocol(new JsonHubProtocol())
			.build();
		return hubConnection;
	}
}
