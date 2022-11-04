import { assert, expect } from 'chai';
import { BackplaneDiscoveryServiceClient } from '../discovery/BackplaneDiscoveryServiceClient';
import { Fdc3Channel } from '../broadcast/channels/ChannelClient';
import { ClientMiddleware } from '../service/ClientMiddleware';
import { BackplaneTransport } from '../transport/BackplaneTransport';

describe('Fdc3Channel', () => {
	it('brodcast should work properly', () => {
		let id = 'testId';
		let type = 'testType';
		let clientMiddleware = new ClientMiddleware({ appId: 'test' }, console);
		let channel = new Fdc3Channel(id, type, clientMiddleware);
		let context = {
			name: 'testContextName',
			type: 'testContextType',
		};

		assert.throws(
			() => {
				channel.broadcast(context);
			},
			Error,
			'Invalid operation: Transport not initialized'
		);
	});

	it('getCurrentContext should work properly', () => {
		let id = 'testId';
		let type = 'testType';
		let clientMiddleware = new ClientMiddleware({ appId: 'test' }, console);
		let channel = new Fdc3Channel(id, type, clientMiddleware);
		expect(channel.getCurrentContext()).instanceOf(Promise);
	});

	it('addContextListener should work properly', () => {
		let id = 'testId';
		let type = 'testType';
		let clientMiddleware = new ClientMiddleware({ appId: 'test' }, console);
		let channel = new Fdc3Channel(id, type, clientMiddleware);
		let handler = () => {
			'testHandler';
		};
		let result = channel.addContextListener(type, handler);
		expect(result).instanceOf(Object);
	});
});
