import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import polly from 'polly-js';
import { Logger } from 'ts-log';
import { BackplaneDiscoveryServiceClient } from '../discovery/BackplaneDiscoveryServiceClient';
import { LogForwarder } from '../logging/LogForwarder';

export const createAndStartHubConnectionAsync = async (
	logger: Logger,
	backplaneDiscoveryServiceClient: BackplaneDiscoveryServiceClient,
	hubConnection: HubConnection | undefined,
	onConnectHandlers: { (): void }[] = []
): Promise<HubConnection> => {
	logger.info(`SignalR: Building hub connection`);
	let hubConnectionNew: HubConnection | undefined = await buildSignalRConnection(
		logger,
		backplaneDiscoveryServiceClient,
		hubConnection
	);
	logger.info(`SignalR: Starting hub connection`);
	await hubConnectionNew.start();
	logger.info('SignalR: Hub Connected.');
	onConnectHandlers.forEach(async onConnectHandler => {
		try {
			onConnectHandler();
			logger.info(`Successfully invoked callback for onconnect over hub`);
		} catch (err) {
			logger.error(`Failed to invoke callback for onconnect over Hub: ${err}`);
		}
	});
	return hubConnectionNew;
};

const buildSignalRConnection = async (
	logger: Logger,
	backplaneDiscoveryServiceClient: BackplaneDiscoveryServiceClient,
	hubConnection: HubConnection | undefined,
	onConnectHandlers: { (): void }[] = []
): Promise<HubConnection> => {
	logger.info(`Discovering backplane from service discovery`);
	let backplaneUrlDiscovered: string = await backplaneDiscoveryServiceClient.discoverBackplaneAsync();
	let hubUrl: string = `${backplaneUrlDiscovered}/backplane/v1.0`;
	logger.info(`signalR: Building connection with url: ${hubUrl}`);
	hubConnection = new HubConnectionBuilder().withUrl(hubUrl).configureLogging(new LogForwarder(logger)).build();
	hubConnection.onreconnecting(error => logger.warn(`SignalR: Hub reconnecting...${error}.`));
	hubConnection.onclose(async error => {
		logger.error(`SignalR: hub connection closed.${error}.`);
		logger.info(`SignalR: Initiating reconnect ...`);
		createAndStartConnectionWithRetry(logger, backplaneDiscoveryServiceClient, hubConnection, onConnectHandlers);
	});
	return hubConnection;
};

const createAndStartConnectionWithRetry = async (
	logger: Logger,
	backplaneDiscoveryServiceClient: BackplaneDiscoveryServiceClient,
	hubConnection: HubConnection | undefined,
	onConnectHandlers: { (): void }[] = []
): Promise<void> => {
	//keep retrying connection for 24 hours.
	await polly()
		.waitAndRetry(Array(28800).fill(3000))
		.executeForPromise(
			async () =>
				await getExistingOrCreateNewHubConnection(
					logger,
					backplaneDiscoveryServiceClient,
					hubConnection,
					onConnectHandlers
				)
		);
};

const getExistingOrCreateNewHubConnection = async (
	logger: Logger,
	backplaneDiscoveryServiceClient: BackplaneDiscoveryServiceClient,
	hubConnection: HubConnection | undefined,
	onConnectHandlers: { (): void }[] = []
): Promise<HubConnection> => {
	if (hubConnection && hubConnection.state == 'Connected') {
		logger.info(`SignalR: Hub connection already established. Returning existing connection..`);
		return hubConnection;
	}
	logger.warn(`SignalR: Existing hub connection not found. Initiating new connection...`);
	return await createAndStartHubConnectionAsync(
		logger,
		backplaneDiscoveryServiceClient,
		hubConnection,
		onConnectHandlers
	);
};
