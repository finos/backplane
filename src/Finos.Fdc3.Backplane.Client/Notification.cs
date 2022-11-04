/**
* SPDX-License-Identifier: Apache-2.0
* Copyright 2021 FINOS FDC3 contributors - see NOTICE file
*/
namespace Finos.Fdc3.Backplane.Client
{
    /// <summary>
    /// Notification object sent in notification stream, that can be subscribed to receive messages from desktop agent
    /// </summary>
    public class Notification
    {
        /// <summary>
        /// Type of notification. Info|Error|Warning
        /// </summary>
        public NotificationType NotificationType { get; }

        /// <summary>
        /// Notification message
        /// </summary>
        public string Message { get; }

        /// <summary>
        /// Notification object creator
        /// </summary>
        /// <param name="notificationType"></param>
        /// <param name="message"></param>
        public Notification(NotificationType notificationType, string message)
        {
            NotificationType = notificationType;
            Message = message;
        }
    }

    /// <summary>
    /// Notification Type
    /// </summary>
    public enum NotificationType
    {
        /// <summary>
        /// Information
        /// </summary>
        Info,
        /// <summary>
        /// Error
        /// </summary>
        Error,
        /// <summary>
        /// Warning
        /// </summary>
        Warning
    }
}
