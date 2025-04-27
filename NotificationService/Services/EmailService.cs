using Microsoft.Extensions.Configuration;
using System.Net;
using System.Net.Mail;

namespace NotificationService.Services
{
   
    public class EmailService:IEmailService
    {
        private readonly IConfiguration _configuration;
        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }
        public bool SendEmail(MailMessage mailMessage)
        {
            bool result = false;
            try
            {
                using (var client = new SmtpClient(_configuration["Smtp:Host"], int.Parse(_configuration["Smtp:Port"])))
                {
                    client.EnableSsl = true;
                    client.Credentials = new NetworkCredential(_configuration["Smtp:Username"], _configuration["Smtp:Password"]);
                    client.Send(mailMessage);
                    result = true;
                }
            }
            catch (Exception ex)
            {
                result = false;
            }       
            return result;
        }
    }
}
