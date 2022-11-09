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
import { Context } from '@finos/fdc3';
import { InitializeParams } from '../API/initializeParams';
import { AppIdentifier, MessageEnvelope } from '../DTO/MessageEnvelope';

export class BackplaneClientTransport {
	private hubConnection: HubConnection;
	private logger: ILogger;
	private appIdentifier: AppIdentifier = { appId: '' };

	constructor(initializeParams: InitializeParams) {
		this.appIdentifier = initializeParams.appIdentifier;
		this.logger = initializeParams.logger ?? console;
		this.hubConnection = this.buildSignalRConnection(initializeParams.url);
	}

	public async connect(onMessage: { (msg: MessageEnvelope): void }, onDisconnect: { (error?: Error): void }) {
		this.hubConnection?.on('OnMessage', onMessage);
		this.hubConnection?.onclose(onDisconnect);
		await this.hubConnection?.start();
		return this.appIdentifier;
	}

	public async broadcast(msg: MessageEnvelope) {
		this.logger.log(LogLevel.Information, `Broadcasting context: msg ${JSON.stringify(msg)}`);
		await this.hubConnection?.invoke('Broadcast', msg);
	}

	public async getSystemChannels() {
		return await this.hubConnection.invoke<Channel[]>('GetSystemChannels');
	}

	public async getCurrentContext(channelId: string, contextType?: string) {
		return await this.hubConnection?.invoke<Context>('GetCurrentContextForChannel', channelId);
	}

	public async Disconnect() {
		await this.hubConnection?.stop();
	}

	private buildSignalRConnection(url: string) {
		this.logger?.log(LogLevel.Information, `signalR: Building connection with url: ${url}`);
		var hubConnection = new HubConnectionBuilder()
			.withUrl(`${url}`, { skipNegotiation: true, transport: HttpTransportType.WebSockets })
			.configureLogging(this.logger)
			.withAutomaticReconnect()
			.withHubProtocol(new JsonHubProtocol())
			.build();
		return hubConnection;
	}
}
