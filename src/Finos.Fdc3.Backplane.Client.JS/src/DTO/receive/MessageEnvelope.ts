/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */

import { Context } from '@finos/fdc3';

export interface MessageEnvelope {
	type: Fdc3Action;
	payload: EnvelopeData;
	meta: EnvelopeMeta;
}

export enum Fdc3Action {
	Broadcast,
}

export interface EnvelopeData {
	channelId: string;
	context: Context;
}

export interface EnvelopeMeta {
	source: AppIdentifier;
	uniqueMessageId: string;
}

export interface AppIdentifier {
	readonly appId: string;
	readonly instanceId?: string;
	/** Field that represents the Desktop Agent that the app is available on.**/
	readonly desktopAgent?: string;
}
