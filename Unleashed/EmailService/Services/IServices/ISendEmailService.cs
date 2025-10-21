using EmailService.Models.Internal;
using System.Net.Mail;

namespace EmailService.Services.IServices
{
    public interface ISendEmailService
    {
        Task SendEmailAsync(EmailMessage message);

    }
}
