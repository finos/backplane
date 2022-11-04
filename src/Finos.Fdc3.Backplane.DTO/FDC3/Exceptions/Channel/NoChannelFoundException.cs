/**
	* SPDX-License-Identifier: Apache-2.0
	* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
	*/

using System;

namespace Finos.Fdc3.Backplane.DTO.FDC3.Exceptions.Channel
{
    /// <summary>
    /// Raised when specified channel is not found.
    /// </summary>
    [Serializable]
    public class NoChannelFoundException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the NoChannelFoundException class with a specified error
        /// </summary>
        /// <param name="message"></param>
        public NoChannelFoundException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the NoChannelFoundException class with a specified error and inner exception.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="innerException"></param>
        public NoChannelFoundException(string message, Exception innerException) : base(message, innerException)
        {

        }
    }

}
