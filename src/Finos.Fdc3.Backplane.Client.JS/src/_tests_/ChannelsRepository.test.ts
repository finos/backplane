import chai, { expect } from 'chai';
import spies from 'chai-spies';
import { BackplaneDiscoveryServiceClient } from '../discovery/BackplaneDiscoveryServiceClient';
import { ChannelsRepository } from '../repository/ChannelsRepository';
import { ClientMiddleware } from '../service/ClientMiddleware';
import { BackplaneTransport } from '../transport/BackplaneTransport';

describe('ChannelsRepository', () => {
	beforeEach(function () {
		chai.use(spies);
		chai.spy.on(console, ['info']);
	});

	afterEach(function () {
		chai.spy.restore(console);
	});

	it('InitializeAsync should work correctly', () => {
		let backplaneTransport = new BackplaneTransport(new BackplaneDiscoveryServiceClient(console), console);

		let clientMiddleware = new ClientMiddleware({ appId: 'test' }, console);

		let channelsRepository = new ChannelsRepository(backplaneTransport, clientMiddleware, console);

		channelsRepository.InitializeAsync(1, 100);

		expect(console.info).to.have.been.called.with(`Fetching system channels from backplane ...`);
	});

	it('getSystemChannels to work correctly', () => {
		let backplaneTransport = new BackplaneTransport(new BackplaneDiscoveryServiceClient(console), console);

		let clientMiddleware = new ClientMiddleware({ appId: 'test' }, console);

		let channelsRepository = new ChannelsRepository(backplaneTransport, clientMiddleware, console);

		let result = channelsRepository.getSystemChannels();

		expect(result).instanceOf(Object);
	});
});
