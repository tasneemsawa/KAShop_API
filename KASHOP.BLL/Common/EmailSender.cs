using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using System;

namespace KASHOP.BLL.Common
{
    public class EmailSender : IEmailSender
    {
        private readonly IConfiguration _config;

        public EmailSender(IConfiguration config)
        {
            _config = config;
        }
        public Task SendEmailAsync(string email, string subject, string message)
        {
            var client = new SmtpClient(_config["EmailSettings:Host"], int.Parse(_config["EmailSettings:Port"]))//var client = new SmtpClient("smtp.gmail.com", 587)
            {
                EnableSsl = true,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(_config["EmailSettings:Email"], _config["EmailSettings:Password"])  //Credentials = new NetworkCredential("tasneem31997@gmail.com", "vpqv mlsu pbwm stxp")
            };

            return client.SendMailAsync(
                new MailMessage(from:_config["EmailSettings:Email"], //"tasneem31997@gmail.com",
                                to: email,
                                subject,
                                message
                                )
                { IsBodyHtml = true });
        }
    }
}