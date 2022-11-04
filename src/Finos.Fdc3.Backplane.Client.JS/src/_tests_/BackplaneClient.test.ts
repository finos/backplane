import chai, { assert, expect } from 'chai';
import { BackplaneClient } from '../API/BackplaneClient';
import spies from 'chai-spies';
import { Context } from '@finos/fdc3';

describe('BackplaneClient', () => {
	beforeEach(function () {
		chai.use(spies);
		chai.spy.on(console, ['info']);
	});

	afterEach(function () {
		chai.spy.restore(console);
	});

	it('brodcast function should be called correctly', () => {
		let backplaneClient = new BackplaneClient({ appId: 'test' }, console);
		let context = {
			name: 'testContextName',
			type: 'testContextType',
		};
		backplaneClient.broadcast(context);
		expect(console.info).to.have.been.called.with('DesktopAgentClient: broadcast');
	});

	it('addContextListener function should be called correctly', () => {
		let backplaneClient = new BackplaneClient({ appId: 'test' }, console);
		let contextTypeOrHandler = 'testContextTypeOrHandler';
		let contextHandler = (context: Context) => 'testContextHandler';
		backplaneClient.addContextListener(contextTypeOrHandler, contextHandler);
		expect(console.info).to.have.been.called.with('DesktopAgentClient: addContextListener');
	});

	it('getSystemChannels function should return a Promise object', () => {
		let backplaneClient = new BackplaneClient({ appId: 'test' });
		let result = backplaneClient.getSystemChannels();
		expect(result).instanceOf(Promise);
	});

	it('joinChannel should be called correctly', () => {
		let backplaneClient = new BackplaneClient({ appId: 'test' }, console);
		let channelId = 'testChannelId';
		backplaneClient.joinChannel(channelId);
		expect(console.info).to.have.been.called.with('DesktopAgentClient: joinChannel testChannelId');
	});

	it('getOrCreateChannel function should throw an Error', () => {
		let backplaneClient = new BackplaneClient({ appId: 'test' });
		let channelId = 'testChannelId';
		assert.throws(
			() => {
				backplaneClient.getOrCreateChannel(channelId);
			},
			Error,
			'Method not implemented.'
		);
	});

	it('getCurrentChannel function should return a Promise object', () => {
		let backplaneClient = new BackplaneClient({ appId: 'test' });
		let result = backplaneClient.getCurrentChannel();
		expect(result).instanceOf(Promise);
	});

	it('leaveCurrentChannel function should return a Promise object', () => {
		let backplaneClient = new BackplaneClient({ appId: 'test' });
		let result = backplaneClient.leaveCurrentChannel();
		expect(result).instanceOf(Promise);
	});

	it('initializeAsync function should return a Promise object', () => {
		let backplaneClient = new BackplaneClient({ appId: 'test' });
		let result = backplaneClient.initializeAsync(1, 1000);
		expect(result).instanceOf(Promise);
	});
});
