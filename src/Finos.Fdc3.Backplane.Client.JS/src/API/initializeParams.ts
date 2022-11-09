/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */

import { ILogger } from '@microsoft/signalr';
import { AppIdentifier } from '../DTO/MessageEnvelope';

export interface InitializeParams {
	/**
	 * App/desktop agent identifier
	 *
	 * @type {DesktopAgentIdentifier}
	 * @memberof InitializeOptions
	 */
	appIdentifier: AppIdentifier;

	/**
	 * Injected logger
	 *
	 * @type {Logger}
	 * @memberof InitializeOptions
	 */
	logger?: ILogger;

	/**
	 *
	 *
	 * @type {Url}
	 * @memberof InitializeParams
	 */
	url: string;
}
