using System.Net.Mail;

namespace NotificationService.Services
{
    public interface IEmailService
    {
        bool SendEmail(MailMessage mailMessage);
    }
}
