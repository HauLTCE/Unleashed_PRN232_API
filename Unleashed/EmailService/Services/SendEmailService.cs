using EmailService.Models.Internal;
using EmailService.Services.IServices;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Options;
using MimeKit;

namespace EmailService.Services
{
    public class SendEmailService : ISendEmailService
    {
            private readonly ILogger<SendEmailService> _logger;
            private readonly EmailSettings _settings;

            // Inject ILogger and IOptions<EmailSettings>
            public SendEmailService(ILogger<SendEmailService> logger, IOptions<EmailSettings> settings)
            {
                _logger = logger;
                _settings = settings.Value; // Get the actual settings object
            }

            public async Task SendEmailAsync(EmailMessage message)
            {
                var mimeMessage = CreateMimeMessage(message);

                using (var client = new SmtpClient())
                {
                    try
                    {
                        _logger.LogInformation("Connecting to SMTP server {Server} on port {Port}...",
                            _settings.SmtpServer, _settings.SmtpPort);

                        // Connect to the SMTP server
                        // Note: Use SecureSocketOptions.StartTls for servers that upgrade connection
                        await client.ConnectAsync(_settings.SmtpServer, _settings.SmtpPort, SecureSocketOptions.Auto);

                        // Authenticate
                        await client.AuthenticateAsync(_settings.SmtpUser, _settings.SmtpPass);

                        _logger.LogInformation("Sending email to {To} with subject {Subject}...",
                            string.Join(", ", message.ToAddresses), message.Subject);

                        await client.SendAsync(mimeMessage);

                        _logger.LogInformation("Email sent successfully.");
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Failed to send email. SmtpServer: {Server}, User: {User}",
                            _settings.SmtpServer, _settings.SmtpUser);
                        throw; // Re-throw the exception so the calling service knows it failed
                    }
                    finally
                    {
                        // Always disconnect
                        if (client.IsConnected)
                        {
                            await client.DisconnectAsync(true);
                        }
                    }
                }
            }

            private MimeMessage CreateMimeMessage(EmailMessage message)
            {
                var mimeMessage = new MimeMessage();

                string fromAddress = message.FromAddress ?? _settings.DefaultFromAddress;

                if (string.IsNullOrEmpty(fromAddress))
                {
                throw new InvalidOperationException("Cannot send email: The 'From' address is not configured. Both message.FromAddress and settings.DefaultFromAddress are null.");
                }

                mimeMessage.From.Add(new MailboxAddress(_settings.DefaultFromName, fromAddress));

                // Set To
                mimeMessage.To.AddRange(message.ToAddresses.Select(MailboxAddress.Parse));

                // Set Cc (if any)
                if (message.CcAddresses.Any())
                {
                    mimeMessage.Cc.AddRange(message.CcAddresses.Select(MailboxAddress.Parse));
                }

                // Set Bcc (if any)
                if (message.BccAddresses.Any())
                {
                    mimeMessage.Bcc.AddRange(message.BccAddresses.Select(MailboxAddress.Parse));
                }

                // Set Subject
                mimeMessage.Subject = message.Subject;

                // Set Body
                var builder = new BodyBuilder();
                if (message.IsHtml)
                {
                    builder.HtmlBody = message.Body;
                }
                else
                {
                    builder.TextBody = message.Body;
                }
                mimeMessage.Body = builder.ToMessageBody();

                return mimeMessage;
            }
    }
}

