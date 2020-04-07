// Copyright (c) Frandi Dwi 2020. All rights reserved.
// Licensed under the MIT License.
namespace TakNotify
{
    /// <summary>
    /// The log messages
    /// </summary>
    public static class SendGridLogMessages
    {
        /// <summary>
        /// The message to display before sending email
        /// </summary>
        public const string Sending_Start = "Sending email {subject} to {toAddresses}";

        /// <summary>
        /// The message to display after sending email
        /// </summary>
        public const string Sending_End = "Email {subject} has been sent to {toAddresses}";
    }
}
