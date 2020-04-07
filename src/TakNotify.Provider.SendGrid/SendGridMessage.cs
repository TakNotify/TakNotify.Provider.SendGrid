// Copyright (c) Frandi Dwi 2020. All rights reserved.
// Licensed under the MIT License.
using System.Collections.Generic;
using System.Linq;

namespace TakNotify
{
    /// <summary>
    /// The wrapper of the message that will be sent with this provider
    /// </summary>
    public class SendGridMessage
    {
        internal static string Parameter_FromAddress = $"{SendGridConstants.DefaultName}_{nameof(FromAddress)}";
        internal static string Parameter_ToAddresses = $"{SendGridConstants.DefaultName}_{nameof(ToAddresses)}";
        internal static string Parameter_Subject = $"{SendGridConstants.DefaultName}_{nameof(Subject)}";
        internal static string Parameter_PlainContent = $"{SendGridConstants.DefaultName}_{nameof(PlainContent)}";
        internal static string Parameter_HtmlContent = $"{SendGridConstants.DefaultName}_{nameof(HtmlContent)}";

        /// <summary>
        /// Instantiate the <see cref="SendGridMessage"/>
        /// </summary>
        public SendGridMessage()
        {
            ToAddresses = new List<string>();
        }

        /// <summary>
        /// Instantiate the <see cref="SendGridMessage"/>
        /// </summary>
        /// <param name="parameters">The collection of message parameters</param>
        public SendGridMessage(MessageParameterCollection parameters)
        {
            if (parameters.ContainsKey(Parameter_FromAddress))
                FromAddress = parameters[Parameter_FromAddress];

            if (parameters.ContainsKey(Parameter_ToAddresses))
                ToAddresses = parameters[Parameter_ToAddresses].Split(',').ToList(); // parse csv
            else
                ToAddresses = new List<string>();

            if (parameters.ContainsKey(Parameter_Subject))
                Subject = parameters[Parameter_Subject];

            if (parameters.ContainsKey(Parameter_PlainContent))
                PlainContent = parameters[Parameter_PlainContent];

            if (parameters.ContainsKey(Parameter_HtmlContent))
                HtmlContent = parameters[Parameter_HtmlContent];
        }

        /// <summary>
        /// The <b>From</b> address
        /// </summary>
        public string FromAddress { get; set; }

        /// <summary>
        /// List of <b>To</b> addresses
        /// </summary>
        public List<string> ToAddresses { get; set; }

        /// <summary>
        /// The email subject
        /// </summary>
        public string Subject { get; set; }

        /// <summary>
        /// The email content in plain text
        /// </summary>
        public string PlainContent { get; set; }

        /// <summary>
        /// The email content in HTML format
        /// </summary>
        public string HtmlContent { get; set; }

        /// <summary>
        /// Convert this object into message parameters
        /// </summary>
        /// <returns></returns>
        public MessageParameterCollection ToParameters()
        {
            var parameters = new MessageParameterCollection();

            if (!string.IsNullOrEmpty(FromAddress))
                parameters.Add(Parameter_FromAddress, FromAddress);

            if (ToAddresses.Count > 0)
                parameters.Add(Parameter_ToAddresses, string.Join(",", ToAddresses));

            if (!string.IsNullOrEmpty(Subject))
                parameters.Add(Parameter_Subject, Subject);

            if (!string.IsNullOrEmpty(PlainContent))
                parameters.Add(Parameter_PlainContent, PlainContent);

            if (!string.IsNullOrEmpty(HtmlContent))
                parameters.Add(Parameter_HtmlContent, HtmlContent);

            return parameters;
        }
    }
}
