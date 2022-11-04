import chai, { assert, expect } from 'chai';
import spies from 'chai-spies';
import { BackplaneDiscoveryServiceClient } from '../discovery/BackplaneDiscoveryServiceClient';

describe('BackplaneDiscoveryServiceClient', () => {
	beforeEach(function () {
		chai.use(spies);
		chai.spy.on(console, ['info']);
	});

	afterEach(function () {
		chai.spy.restore(console);
	});

	it('discoverBackplaneAsync should work correctly', () => {
		let backplaneDiscoveryServiceClient = new BackplaneDiscoveryServiceClient(console);
		let result = backplaneDiscoveryServiceClient.discoverBackplaneAsync();
		expect(result).instanceOf(Promise);
		expect(console.info).to.have.been.called.with(`Service discovery local url: ${'http://localhost:49201/'}`);
	});
});
