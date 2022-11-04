/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using System;

namespace Finos.Fdc3.Backplane.DTO.FDC3.Exceptions.Channel
{
    /// <summary>
    /// Raised when access to channel is denied.
    /// </summary>
    [Serializable]
    public class ChannelAccessDeniedException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the ChannelAccessDeniedException class with a specified error
        /// </summary>
        /// <param name="message"></param>
        public ChannelAccessDeniedException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the ChannelAccessDeniedException class with a specified error and inner exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public ChannelAccessDeniedException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }
}
