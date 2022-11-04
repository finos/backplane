using Finos.Fdc3.Backplane.DTO.Envelope.Receive;
using Finos.Fdc3.Backplane.DTO.Envelope.Send;
/**
* SPDX-License-Identifier: Apache-2.0
* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
*/
namespace Finos.Fdc3.Backplane.Core
{
    /// <summary>
    /// Interface which converts input broadcast message to message envelop which can later be sent to other connected clients.
    /// </summary>
    public interface IMessageEnvelopeGenerator
    {
        MessageEnvelope GenerateMessageEnvelope(BroadcastContextEnvelope broadcastDto);
    }
}
