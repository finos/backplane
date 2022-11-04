/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */

import { ClientMiddleware } from '../service/ClientMiddleware';
import { AppIntent, Channel, Context, ContextHandler, Listener } from '@finos/fdc3';
import { BackplaneDiscoveryServiceClient } from '../discovery/BackplaneDiscoveryServiceClient';
import { BackplaneTransport } from '../transport/BackplaneTransport';
import { Logger } from 'ts-log';
import { ChannelsRepository } from '../repository/ChannelsRepository';
import { Fdc3Channel } from '../broadcast/channels/ChannelClient';
import { AppIdentifier } from '../DTO/receive/MessageEnvelope';

interface ContextTypeAndHandler {
	contextTypeOrHandler: string | ContextHandler;
	handler?: ContextHandler;
	listener: Listener | null;
}

interface IntentAndHandler {
	intent: string;
	handler?: ContextHandler;
	listener: Listener | null;
}

export class BackplaneClient {
	private clientMiddleware: ClientMiddleware;

	private logger: Logger;
	private systemChannels: Array<Fdc3Channel>;
	private currentChannel: Fdc3Channel | undefined;
	private contextHandlers: { [key: string]: ContextTypeAndHandler } = {};
	private channelChanging = false;

	constructor(appIdentifier: AppIdentifier, logger?: Logger) {
		this.logger = logger ?? console;
		this.systemChannels = new Array<Fdc3Channel>();
		this.clientMiddleware = new ClientMiddleware(appIdentifier, this.logger);
	}

	async initializeAsync(retryCount?: number, retryIntervalInMs?: number): Promise<void> {
		retryCount = retryCount ?? 3;
		retryIntervalInMs = retryIntervalInMs ?? 3000;
		await this.clientMiddleware.initializeAsync(retryCount, retryIntervalInMs);
		this.systemChannels = this.clientMiddleware.getSystemChannels();
	}

	addContextListener(handler: ContextHandler): Listener;
	addContextListener(contextType: string, handler: ContextHandler): Listener;
	addContextListener(contextTypeOrHandler: string | ContextHandler, handler?: ContextHandler): Listener {
		this.logger.info('DesktopAgentClient: addContextListener', contextTypeOrHandler, handler);
		let contextListener = null;
		if (this.currentChannel) {
			if (typeof contextTypeOrHandler === 'string') {
				//type of context listener is specified
				// @ts-ignore
				contextListener = this.currentChannel.addContextListener(contextTypeOrHandler, handler);
			} else {
				contextListener = this.currentChannel.addContextListener(contextTypeOrHandler);
			}
		}

		const contextHandlerId = Date.now() + '_' + Math.random();
		this.contextHandlers[contextHandlerId] = {
			contextTypeOrHandler,
			handler,
			listener: contextListener,
		};

		return {
			unsubscribe: () => {
				this.contextHandlers[contextHandlerId].listener?.unsubscribe();
				delete this.contextHandlers[contextHandlerId];
			},
		};
	}

	broadcast(context: Context): void {
		this.logger.info('DesktopAgentClient: broadcast');
		if (this.currentChannel == undefined) {
			this.logger.warn('DesktopAgentClient: No channel joined. Broadcast has no effect');
			return;
		}
		this.currentChannel.broadcast(context);
	}

	getSystemChannels(): Promise<Channel[]> {
		this.logger.info('DesktopAgentClient: getSystemChannel');
		return Promise.resolve(this.systemChannels);
	}

	async joinChannel(channelId: string): Promise<void> {
		this.logger.info(`DesktopAgentClient: joinChannel ${channelId}`);
		if (this.currentChannel && this.currentChannel.id === channelId) return;
		if (this.channelChanging) {
			throw new Error('Currently in process of changing channels. Rejecting this request. ' + channelId);
		}
		let channel = this.systemChannels.find(x => x.id == channelId);
		if (channel == undefined) return Promise.reject('DesktopAgentClient: No such channel exist');
		// unsubscribe to everything that was already subscribed
		if (this.currentChannel) {
			this.channelChanging = true;
			await this.leaveCurrentChannel();
		}

		this.currentChannel = channel;
		const contextHandlerIds = Object.keys(this.contextHandlers);
		for (const contextHandlerId of contextHandlerIds) {
			const contextHandler = this.contextHandlers[contextHandlerId];
			let contextListener;
			if (typeof contextHandler.contextTypeOrHandler === 'string') {
				contextListener = this.currentChannel.addContextListener(
					contextHandler.contextTypeOrHandler,
					contextHandler.handler!
				);
			} else {
				contextListener = this.currentChannel.addContextListener(contextHandler.contextTypeOrHandler);
			}
			contextHandler.listener = contextListener;
		}

		if (this.channelChanging) {
			this.logger.log('done Changing channel to ', channelId);
			this.channelChanging = false;
		}
		return Promise.resolve();
	}

	getOrCreateChannel(channelId: string): Promise<Channel> {
		throw new Error('Method not implemented.');
	}

	getCurrentChannel(): Promise<Channel> {
		this.logger.info(`DesktopAgentClient: getCurrentChannel`);
		if (this.currentChannel == undefined) return Promise.reject('DesktopAgentClient: no channel joined');
		return Promise.resolve(this.currentChannel);
	}

	leaveCurrentChannel(): Promise<void> {
		this.logger.info('DesktopAgentClient: leaveCurrentChannel', this.currentChannel?.id);
		if (this.currentChannel) {
			this.currentChannel = undefined;
			const contextHandlerIds = Object.keys(this.contextHandlers);
			for (const contextHandlerId of contextHandlerIds) {
				const contextHandler = this.contextHandlers[contextHandlerId];
				contextHandler.listener?.unsubscribe();
				contextHandler.listener = null;
			}
		}
		return Promise.resolve();
	}
}
