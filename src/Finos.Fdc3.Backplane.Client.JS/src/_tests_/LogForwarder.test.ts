import { LogLevel } from '@microsoft/signalr';
import chai, { expect } from 'chai';
import spies from 'chai-spies';
import { LogForwarder } from '../logging/LogForwarder';

describe('LogForwarder', () => {
	beforeEach(function () {
		chai.use(spies);
	});

	afterEach(function () {
		chai.spy.restore(console);
	});

	it('error message should display correctly', () => {
		chai.spy.on(console, ['error']);

		let lF = new LogForwarder(console);

		lF.log(LogLevel.Error, 'testError');

		expect(console.error).to.have.been.called.with('testError');
	});

	it('warning message should display correctly', () => {
		chai.spy.on(console, ['warn']);

		let lF = new LogForwarder(console);

		lF.log(LogLevel.Warning, 'testWarning');

		expect(console.warn).to.have.been.called.with('testWarning');
	});

	it('debug message should display correctly', () => {
		chai.spy.on(console, ['debug']);

		let lF = new LogForwarder(console);

		lF.log(LogLevel.Debug, 'testDebug');

		expect(console.debug).to.have.been.called.with('testDebug');
	});

	it('trace message should display correctly', () => {
		chai.spy.on(console, ['trace']);

		let lF = new LogForwarder(console);

		lF.log(LogLevel.Trace, 'testTrace');

		expect(console.trace).to.have.been.called.with('testTrace');
	});

	it('information message should display correctly', () => {
		chai.spy.on(console, ['info']);

		let lF = new LogForwarder(console);

		lF.log(LogLevel.Information, 'testInformation');

		expect(console.info).to.have.been.called.with('testInformation');
	});
});
