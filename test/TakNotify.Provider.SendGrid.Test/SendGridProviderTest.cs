// Copyright (c) Frandi Dwi 2020. All rights reserved.
// Licensed under the MIT License.
using Microsoft.Extensions.Logging;
using Moq;
using SendGrid;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading;
using TakNotify.Test;
using Xunit;
using SGMail = SendGrid.Helpers.Mail;

namespace TakNotify.Provider.SendGrid.Test
{
    public class SendGridProviderTest
    {
        private readonly Mock<ILoggerFactory> _loggerFactory;
        private readonly Mock<ILogger<Notification>> _logger;
        private readonly Mock<ISendGridClient> _sendGridClient;

        public SendGridProviderTest()
        {
            _loggerFactory = new Mock<ILoggerFactory>();
            _logger = new Mock<ILogger<Notification>>();
            _sendGridClient = new Mock<ISendGridClient>();

            _loggerFactory.Setup(lf => lf.CreateLogger(It.IsAny<string>())).Returns(_logger.Object);
        }

        [Fact]
        public async void Send_Success()
        {
            _sendGridClient.Setup(client => client.SendEmailAsync(It.IsAny<SGMail.SendGridMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Response(System.Net.HttpStatusCode.Accepted, null, null));

            var message = new SendGridMessage
            {
                FromAddress = "sender@example.com",
                ToAddresses = new List<string> { "user@example.com" },
                Subject = "Test Email"
            };

            var provider = new SendGridProvider(_sendGridClient.Object, _loggerFactory.Object);

            var result = await provider.Send(message.ToParameters());

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);

            var startMessage = LoggerHelper.FormatLogValues(SendGridLogMessages.Sending_Start, message.Subject, message.ToAddresses);
            _logger.VerifyLog(LogLevel.Debug, startMessage);

            var endMessage = LoggerHelper.FormatLogValues(SendGridLogMessages.Sending_End, message.Subject, message.ToAddresses);
            _logger.VerifyLog(LogLevel.Debug, endMessage);
        }

        [Fact]
        public async void Send_Failed()
        {
            _sendGridClient.Setup(client => client.SendEmailAsync(It.IsAny<SGMail.SendGridMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Response(System.Net.HttpStatusCode.BadRequest, new StringContent("Error01"), null));

            var message = new SendGridMessage
            {
                FromAddress = "sender@example.com",
                ToAddresses = new List<string> { "user@example.com" },
                Subject = "Test Email"
            };

            var provider = new SendGridProvider(_sendGridClient.Object, _loggerFactory.Object);

            var result = await provider.Send(message.ToParameters());

            Assert.False(result.IsSuccess);
            Assert.NotEmpty(result.Errors);
            Assert.Equal("Error01", result.Errors[0]);

            var startMessage = LoggerHelper.FormatLogValues(SendGridLogMessages.Sending_Start, message.Subject, message.ToAddresses);
            _logger.VerifyLog(LogLevel.Debug, startMessage);

            var endMessage = LoggerHelper.FormatLogValues(SendGridLogMessages.Sending_End, message.Subject, message.ToAddresses);
            _logger.VerifyLog(LogLevel.Debug, endMessage, Times.Never());
        }

        [Fact]
        public async void Send_WithDefaultFromAddress_Success()
        {
            _sendGridClient.Setup(client => client.SendEmailAsync(It.IsAny<SGMail.SendGridMessage>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new Response(System.Net.HttpStatusCode.Accepted, null, null));

            var provider = new SendGridProvider(
                new SendGridOptions { DefaultFromAddress = "sender@example.com" },
                _sendGridClient.Object,
                _loggerFactory.Object);

            var message = new SendGridMessage
            {
                ToAddresses = new List<string> { "user@example.com" },
                Subject = "Test Email"
            };

            var result = await provider.Send(message.ToParameters());

            Assert.True(result.IsSuccess);
            Assert.Empty(result.Errors);
        }

        [Fact]
        public async void Send_WithoutFromAddress_ReturnError()
        {
            var provider = new SendGridProvider(_sendGridClient.Object, _loggerFactory.Object);

            var message = new SendGridMessage
            {
                ToAddresses = new List<string> { "user@example.com" },
                Subject = "Test Email"
            };

            var result = await provider.Send(message.ToParameters());

            Assert.False(result.IsSuccess);
            Assert.Equal("From Address should not be empty", result.Errors[0]);
        }
    }
}
