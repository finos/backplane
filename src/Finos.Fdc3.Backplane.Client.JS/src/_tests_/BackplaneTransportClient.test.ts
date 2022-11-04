import chai, { expect } from 'chai';
import spies from 'chai-spies';
import { BackplaneDiscoveryServiceClient } from '../discovery/BackplaneDiscoveryServiceClient';
import { BackplaneTransport } from '../transport/BackplaneTransport';

describe('BackplaneTransportClient', () => {
	beforeEach(function () {
		chai.use(spies);
		chai.spy.on(console, ['info']);
	});

	afterEach(function () {
		chai.spy.restore(console);
	});

	it('registerOnConnect should work correctly', () => {
		let backplaneDiscoveryServiceClient = new BackplaneDiscoveryServiceClient(console);
		let backplaneTransport = new BackplaneTransport(backplaneDiscoveryServiceClient, console);
		let onConnect = () => {};
		backplaneTransport.registerOnConnect(onConnect);

		expect(console.info).to.have.been.called.with('SignalR: Registered for callback on reconnect ..');
	});

	it('getHubConnection should work correctly', () => {
		let backplaneDiscoveryServiceClient = new BackplaneDiscoveryServiceClient(console);
		let backplaneTransport = new BackplaneTransport(backplaneDiscoveryServiceClient, console);
		let result = backplaneTransport.getHubConnection();

		expect(result).to.be.undefined;
	});

	it('InitializeAsync should work correctly', () => {
		let backplaneDiscoveryServiceClient = new BackplaneDiscoveryServiceClient(console);
		let backplaneTransport = new BackplaneTransport(backplaneDiscoveryServiceClient, console);

		backplaneTransport.initializeAsync(1, 100);

		expect(console.info).to.have.been.called.with(
			'Initializing backplane connection with retry count: 1 and retryIntervalInMs: 100'
		);
	});
});
