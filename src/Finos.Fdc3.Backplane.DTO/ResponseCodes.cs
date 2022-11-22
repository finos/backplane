/*
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2022 FINOS FDC3 contributors - see NOTICE file
	*/

namespace Finos.Fdc3.Backplane.DTO
{
    /// <summary>
    /// Backplane Response codes 
    /// </summary>
    public enum ResponseCodes
    {
        /// <summary>
        /// Multihost scenario, no member nodes connected
        /// </summary>
        NoMemberNodesConnected = 70000,

        /// <summary>
        /// Broadcast payload invalid
        /// </summary>
        BroadcastPayloadInvalid = 80000,
    }
}
