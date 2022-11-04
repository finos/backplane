import { Context, ContextHandler } from '@finos/fdc3';
import { Logger } from 'ts-log';
import { AppIdentifier, MessageEnvelope } from '../DTO/receive/MessageEnvelope';
import { BroadcastContextEnvelope } from '../DTO/send/BroadcastContextEnvelope';
import { BackplaneTransport } from '../transport/BackplaneTransport';
import * as Utility from './UniqueMessageIdGenerator';
import { HubConnection } from '@microsoft/signalr';

export const buildBroadcastEnvelop = (
	appIdentifier: AppIdentifier,
	context: Context,
	channelId: string,
	logger: Logger
): string => {
	let dto: BroadcastContextEnvelope = {
		channelId: channelId,
		context: context,
		meta: { source: appIdentifier, uniqueMessageId: Utility.getUniqueMessageId() },
	};
	let jsonString = JSON.stringify(dto);
	logger.info(`BroadcastContextEnvelope: ${jsonString}`);
	return jsonString;
};

export const onHubConnection = (
	logger: Logger,
	backplaneTransport: BackplaneTransport,
	contextHandlers: Map<string, ContextHandler[]>
): void => {
	try {
		logger.info(`Registering on signalR hub for receiving broadcast`);
		let hubConnection: HubConnection | undefined = backplaneTransport.getHubConnection();
		if (hubConnection) {
			hubConnection.on('ReceiveBroadcastMessage', (messageEnvelop: MessageEnvelope) => {
				logger.info(
					`Received broadcast context message from backplane with message id: ${messageEnvelop.meta.uniqueMessageId}`
				);
				let fdc3Data = messageEnvelop.payload;
				handleContext(fdc3Data.context.type, fdc3Data.context, fdc3Data.channelId, logger, contextHandlers);
			});
			logger.info(`Successfully registered on signalR hub for receiving broadcast`);
		} else logger.error(`Invalid operation: Transport not initialized.`);
	} catch (err) {
		throw new Error(`Failed to register on hub for receiving message`);
	}
};

const handleContext = (
	contextType: string,
	context: Context,
	channelId: string,
	logger: Logger,
	contextHandlers: Map<string, ContextHandler[]>
): void => {
	if (!contextType) {
		logger.error(`Received invalid context ${context}. type property is mandatory..`);
		return;
	}
	logger.info(`Received context type ${contextType}. Invoking registered handlers`);
	let keyAllContextTypes: string = getBroadcastContextMapKey(undefined, channelId);
	let keySpecificContextType: string = getBroadcastContextMapKey(contextType, channelId);
	let listenersForAllContextType: ContextHandler[] | undefined = contextHandlers.get(keyAllContextTypes);
	let specificContextListeners: ContextHandler[] | undefined = contextHandlers.get(keySpecificContextType);
	if (listenersForAllContextType) invokeHandlersSafe(listenersForAllContextType, context, logger);
	if (specificContextListeners) invokeHandlersSafe(specificContextListeners, context, logger);
};

export const getBroadcastContextMapKey = (contextType: string | undefined, channel: string): string => {
	if (contextType == undefined) contextType = '*';
	return `${contextType.toLowerCase()}-${channel.toLowerCase()}`;
};

const invokeHandlersSafe = (handlerList: ContextHandler[] | undefined, context: Context, logger: Logger): void => {
	if (handlerList == undefined || handlerList.length == 0) {
		logger.warn(`no handlers registered for context type: ${context.type}`);
		return;
	}
	handlerList.forEach(x => {
		try {
			x(context);
		} catch (err) {
			logger.error(`An error occurred invoking handler: ${err}`);
		}
	});
};

export const unsubscribeUtil = (
	logger: Logger,
	item: ContextHandler[] | undefined,
	type: string,
	key: string,
	handler: ContextHandler
): void => {
	if (!item) {
		logger.error(`${type}: ${key} not found!`);
		return;
	}

	try {
		let itemIndexToDelete: number = item.findIndex(x => x == handler);
		if (itemIndexToDelete < 0) {
			logger.error('No handler found!');
			return;
		}
		item.splice(itemIndexToDelete, 1);
		logger.info(`Unsubscribe successful for ${type}: ${key}`);
	} catch (err) {
		logger.error(`An error occurred in unsubscribe: ${err}`);
	}
};
