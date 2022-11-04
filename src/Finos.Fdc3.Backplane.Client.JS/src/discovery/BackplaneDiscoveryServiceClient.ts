/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */

import { Logger } from 'ts-log';

export class BackplaneDiscoveryServiceClient {
	private logger: Logger;
	constructor(logger: Logger) {
		this.logger = logger;
	}
	//Below method can be extended to reach any persistent storage(database/service discovery) through a service call
	// and return locally hosted backolane url with port number. Currently hardcoded for demo purpose.
	discoverBackplaneAsync(): Promise<string> {
		this.logger.info(`Service discovery local url: ${'http://localhost:49201'}`);
		return new Promise<string>(resolve => {
			resolve('http://localhost:49201');
		});
	}
}
