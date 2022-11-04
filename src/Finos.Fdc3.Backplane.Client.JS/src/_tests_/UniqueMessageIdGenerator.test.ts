import { expect } from 'chai';
import { getUniqueMessageId } from '../utility/UniqueMessageIdGenerator';

describe('getUniqueMessageId', () => {
	it('should work correctly', () => {
		let result = getUniqueMessageId();

		expect(result).not.to.be.undefined;
	});
});
