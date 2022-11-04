/**
 * SPDX-License-Identifier: Apache-2.0
 * Copyright 2021 FINOS FDC3 contributors - see NOTICE file
 */

import { Channel, Context, ContextHandler, DisplayMetadata, Listener } from '@finos/fdc3';
import { ClientMiddleware } from '../../service/ClientMiddleware';
export class Fdc3Channel implements Channel {
	id: string;
	type: string;
	displayMetadata?: DisplayMetadata | undefined;
	private clientMiddleware: ClientMiddleware;
	private currentContext: Context | null = null;

	constructor(id: string, type: string, desktopAgentService: ClientMiddleware, displayMetadata?: DisplayMetadata) {
		this.id = id;
		this.type = type;
		this.clientMiddleware = desktopAgentService;
		this.displayMetadata = displayMetadata;
	}

	broadcast(context: Context): void {
		this.currentContext = context;
		this.clientMiddleware.broadcastContext(context, this.id);
	}

	getCurrentContext(contextType?: string): Promise<Context | null> {
		return Promise.resolve(this.currentContext);
	}

	addContextListener(handler: ContextHandler): Listener;
	addContextListener(contextType: string | null, handler: ContextHandler): Listener;
	addContextListener(contextType: any, handler?: any): Listener {
		let contextListener: any = null;
		contextListener = this.clientMiddleware.addContextListener(contextType, handler, this.id);
		return contextListener;
	}
}
