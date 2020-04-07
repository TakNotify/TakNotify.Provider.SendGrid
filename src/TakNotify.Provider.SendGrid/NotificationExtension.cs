// Copyright (c) Frandi Dwi 2020. All rights reserved.
// Licensed under the MIT License.
using System.Threading.Tasks;

namespace TakNotify
{
    /// <summary>
    /// The extension for <see cref="INotification"/> to send eamil with SendGrid provider
    /// </summary>
    public static class NotificationExtension
    {
        /// <summary>
        /// Send message with <see cref="SendGridProvider"/>
        /// </summary>
        /// <param name="notification">The notification object</param>
        /// <param name="message">The wrapper of the message that will be sent</param>
        /// <returns></returns>
        public static Task<NotificationResult> SendEmailWithSendGrid(this INotification notification, SendGridMessage message)
        {
            return notification.Send(SendGridConstants.DefaultName, message.ToParameters());
        }
    }
}
