using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BioTech.Services
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _configuration;

        public EmailSender(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public async Task SendEmailAsync(string email, string subject, string htmlMessage)
        {
            using (var client = new SmtpClient())
            {
                client.Host = _configuration["Email:Host"];
                var message = new MailMessage(_configuration["Email:Email"], email)
                {
                    Body = htmlMessage,
                    IsBodyHtml = true,
                    Subject = subject
                };

                await client.SendMailAsync(message);
            }
        }
    }
}
