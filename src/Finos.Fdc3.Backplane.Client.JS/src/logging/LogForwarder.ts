/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */

import { ILogger, LogLevel } from '@microsoft/signalr';
import { Logger } from 'ts-log';

export class LogForwarder implements ILogger {
	externalLogger: Logger;
	constructor(extLogger: Logger) {
		this.externalLogger = extLogger;
	}

	log(logLevel: LogLevel, message: string): void {
		switch (logLevel) {
			case LogLevel.Error:
				this.externalLogger.error(message);
				break;
			case LogLevel.Warning:
				this.externalLogger.warn(message);
				break;
			case LogLevel.Debug:
				this.externalLogger.debug(message);
				break;
			case LogLevel.Trace:
				this.externalLogger.trace(message);
				break;
			case LogLevel.Information:
				this.externalLogger.info(message);
				break;
			default:
				break;
		}
	}
}
