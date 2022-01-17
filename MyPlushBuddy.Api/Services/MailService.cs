using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace MyPlushBuddy.Api.Services
{
    public class MailService : IMailService
    {
        private readonly IConfiguration _configuration;

        public MailService(IConfiguration configuration)
        {
            _configuration = configuration ??
                throw new ArgumentNullException(nameof(configuration));
        }

        public void Send(string subject, string message)
        {
            // send mail - output to debug window
            Debug.WriteLine($"Mail from {_configuration["MailSettings:MailFromAddress"]} to {_configuration["MailSettings:MailToAddress"]}, with MailService.");
            Debug.WriteLine($"Subject: {subject}");
            Debug.WriteLine($"Message: {message}");
        }
    }
}
