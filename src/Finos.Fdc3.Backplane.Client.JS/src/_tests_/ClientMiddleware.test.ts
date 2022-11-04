import chai, { assert, expect } from 'chai';
import spies from 'chai-spies';
import { BackplaneDiscoveryServiceClient } from '../discovery/BackplaneDiscoveryServiceClient';
import { ClientMiddleware } from '../service/ClientMiddleware';
import { BackplaneTransport } from '../transport/BackplaneTransport';

describe('ClientMiddleware', () => {
	beforeEach(function () {
		chai.use(spies);
		chai.spy.on(console, ['info']);
	});

	afterEach(function () {
		chai.spy.restore(console);
	});

	it('broadcastContext should work correctly', () => {
		let backplaneTransport = new BackplaneTransport(new BackplaneDiscoveryServiceClient(console), console);
		let clientMiddleware = new ClientMiddleware({ appId: 'test' }, console);
		let context = {
			name: 'testContextName',
			type: 'testContextType',
		};
		let id = 'testId';
		assert.throws(
			() => {
				clientMiddleware.broadcastContext(context, id);
			},
			Error,
			'Invalid operation: Transport not initialized'
		);
	});

	it('addContextListener should work correctly', () => {
		let backplaneTransport = new BackplaneTransport(new BackplaneDiscoveryServiceClient(console), console);
		let clientMiddleware = new ClientMiddleware({ appId: 'test' }, console);
		let type = 'testType';
		let handler = () => {
			'testHandler';
		};
		let id = 'testId';
		let result = clientMiddleware.addContextListener(type, handler, id);
		expect(result).instanceOf(Object);
	});

	it('addContextListener unsubscribe should work correctly', () => {
		let backplaneTransport = new BackplaneTransport(new BackplaneDiscoveryServiceClient(console), console);
		let clientMiddleware = new ClientMiddleware({ appId: 'test' }, console);
		let type = 'testType';
		let handler = () => {
			'testHandler';
		};
		let id = 'testId';
		clientMiddleware.addContextListener(type, handler, id).unsubscribe();
		expect(console.info).to.have.been.called;
	});
});
