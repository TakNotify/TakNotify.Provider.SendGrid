// Copyright (c) Frandi Dwi 2020. All rights reserved.
// Licensed under the MIT License.
namespace TakNotify
{
    /// <summary>
    /// The options for the <see cref="SendGridProvider"/>
    /// </summary>
    public class SendGridOptions : NotificationProviderOptions
    {
        internal static string Parameter_ApiKey = $"{SendGridConstants.DefaultName}_{nameof(Apikey)}";

        /// <summary>
        /// Instantiate the <see cref="SendGridOptions"/>
        /// </summary>
        public SendGridOptions()
        {
            Parameters.Add(Parameter_ApiKey, "");
        }

        /// <summary>
        /// The SendGrid API key
        /// </summary>
        public string Apikey 
        {
            get => Parameters[Parameter_ApiKey].ToString();
            set => Parameters[Parameter_ApiKey] = value;
        }
    }
}
