/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using Finos.Fdc3.Backplane.DTO.Envelope.Receive;
using Finos.Fdc3.Backplane.DTO.Envelope.Send;

namespace Finos.Fdc3.Backplane
{
    /// <summary>
    /// Service which converts input broadcast message to message envelop which can later be sent to other connected clients. 
    /// </summary>
    public class MessageEnvelopeGenerator : IMessageEnvelopeGenerator
    {
        public MessageEnvelope GenerateMessageEnvelope(BroadcastContextEnvelope broadcastDto)
        {
            return new MessageEnvelope()
            {

                ActionType = Fdc3Action.Broadcast,
                Payload = new EnvelopeData()
                {
                    Context = broadcastDto.Context,
                    ChannelId = broadcastDto.ChannelId
                },
                Meta = broadcastDto.Metadata
            };
        }

    }
}
