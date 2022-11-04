import { Context } from '@finos/fdc3';
import { EnvelopeMeta } from '../receive/MessageEnvelope';

export interface BroadcastContextEnvelope {
	channelId: string;
	context: Context;
	meta: EnvelopeMeta;
}
