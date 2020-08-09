// Copyright (c) Frandi Dwi 2020. All rights reserved.
// Licensed under the MIT License.
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SendGrid;
using SendGrid.Helpers.Mail;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TakNotify
{
    /// <summary>
    /// The notification provider to send email via SendGrid
    /// </summary>
    public class SendGridProvider : NotificationProvider
    {
        private readonly ISendGridClient _sendGridClient;
        private readonly SendGridOptions _sendGridOptions;

        /// <summary>
        /// Instantiate the <see cref="SendGridProvider"/>
        /// </summary>
        /// <param name="sendGridOptions">The options for this provider</param>
        /// <param name="loggerFactory">The logger factory</param>
        public SendGridProvider(SendGridOptions sendGridOptions, ILoggerFactory loggerFactory)
            : base(sendGridOptions, loggerFactory)
        {
            _sendGridClient = new SendGridClient(sendGridOptions.Apikey);
            _sendGridOptions = sendGridOptions;
        }

        /// <summary>
        /// Instantiate the <see cref="SendGridProvider"/>
        /// </summary>
        /// <param name="sendGridClient">The <see cref="ISendGridClient"/> object</param>
        /// <param name="loggerFactory">The logger factory</param>
        public SendGridProvider(ISendGridClient sendGridClient, ILoggerFactory loggerFactory)
            : base(new NotificationProviderOptions(), loggerFactory)
        {
            _sendGridClient = sendGridClient;
        }

        /// <summary>
        /// Instantiate the <see cref="SendGridProvider"/>
        /// </summary>
        /// <param name="sendGridOptions">The options for this provider</param>
        /// <param name="loggerFactory">The logger factory</param>
        public SendGridProvider(IOptions<SendGridOptions> sendGridOptions, ILoggerFactory loggerFactory) 
            : base(sendGridOptions.Value, loggerFactory)
        {
            _sendGridClient = new SendGridClient(sendGridOptions.Value.Apikey);
            _sendGridOptions = sendGridOptions.Value;
        }

        /// <summary>
        /// Instantiate the <see cref="SendGridProvider"/>
        /// <para>Please be aware in using this constructor because it was originally intended for testing and it could cause
        /// unwanted behavior if it is used in the real application</para>
        /// </summary>
        /// <param name="sendGridClient">The <see cref="ISendGridClient"/> object</param>
        /// <param name="sendGridOptions">The options for this provider</param>
        /// <param name="loggerFactory">The logger factory</param>
        public SendGridProvider(SendGridOptions sendGridOptions, ISendGridClient sendGridClient, ILoggerFactory loggerFactory)
            : base(sendGridOptions, loggerFactory)
        {
            _sendGridClient = sendGridClient;
            _sendGridOptions = sendGridOptions;
        }

        /// <inheritdoc cref="NotificationProvider.Name"/>
        public override string Name => SendGridConstants.DefaultName;

        /// <inheritdoc cref="NotificationProvider.Send(MessageParameterCollection)"/>
        public override async Task<NotificationResult> Send(MessageParameterCollection messageParameters)
        {
            var sgMessage = new SendGridMessage(messageParameters);

            EmailAddress from = null;
            if (!string.IsNullOrEmpty(sgMessage.FromAddress))
                from = new EmailAddress(sgMessage.FromAddress);
            else if (_sendGridOptions != null && !string.IsNullOrEmpty(_sendGridOptions.DefaultFromAddress))
                from = new EmailAddress(_sendGridOptions.DefaultFromAddress);
            else
                return new NotificationResult(new List<string> { "From Address should not be empty" });

            var tos = sgMessage.ToAddresses.Select(to => new EmailAddress(to)).ToList();
            var subject = sgMessage.Subject;
            var plainTextContent = sgMessage.PlainContent;
            var htmlContent = sgMessage.HtmlContent;

            var message = MailHelper.CreateSingleEmailToMultipleRecipients(from, tos, subject, plainTextContent, htmlContent);

            Logger.LogDebug(SendGridLogMessages.Sending_Start, sgMessage.Subject, sgMessage.ToAddresses);

            var response = await _sendGridClient.SendEmailAsync(message);

            if (response.StatusCode == System.Net.HttpStatusCode.Accepted)
            {
                Logger.LogDebug(SendGridLogMessages.Sending_End, sgMessage.Subject, sgMessage.ToAddresses);

                return new NotificationResult(true);
            }
            
            var respBody = await response.Body.ReadAsStringAsync();
            return new NotificationResult(new List<string> { respBody });
        }
    }
}
