using EmailService.Models.External;
using EmailService.Models.Internal;
using EmailService.Services.IServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace EmailService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SendEmailController : ControllerBase
    {

        private readonly ISendEmailService _emailService;
        private readonly ILogger<SendEmailController> _logger;

        // Your admin email. It's better to get this from IConfiguration (appsettings.json)
        private const string AdminEmailAddress = "admin@your-website.com";

        public SendEmailController(
            ISendEmailService emailService,
            ILogger<SendEmailController> logger)
        {
            _emailService = emailService;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> SendEmail()
        {
            EmailMessage message = new(
                "Phatpvce181515@fpt.edu.vn",
                "test",
                "test");
            await _emailService.SendEmailAsync(message);
            return Ok();
        }

        [HttpPost("send-confirm-register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SendConfirmationEmail([FromBody] ConfirmRegister request)
        {
            try
            {
                _logger.LogInformation("Attempting to send confirmation email to {Email}", request.Email);

                // 1. Construct the email body with the callback URL.
                //    Using a simple but clean HTML template is recommended.
                var emailBody = $@"
                <html>
                <body style='font-family: Arial, sans-serif; font-size: 16px; line-height: 1.6;'>
                    <h2>Welcome! Please Confirm Your Email Address</h2>
                    <p>Thank you for registering. Please click the button below to confirm your email address and activate your account.</p>
                    <a href='{request.CallbackUrl}' style='display: inline-block; padding: 12px 24px; font-size: 16px; color: #fff; background-color: #007bff; text-decoration: none; border-radius: 5px;'>
                        Confirm Email
                    </a>
                    <p>If you did not create an account, no further action is required.</p>
                </body>
                </html>";

                // 2. Create the EmailMessage object
                var emailMessage = new EmailMessage(
                    toAddress: request.Email,
                    subject: "Confirm Your Email Address",
                    body: emailBody
                );

                // 3. Call the service to send the email
                await _emailService.SendEmailAsync(emailMessage);

                _logger.LogInformation("Successfully sent confirmation email to {Email}", request.Email);

                return Ok(new { message = "Confirmation email sent successfully." });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while sending confirmation email to {Email}", request.Email);
                return StatusCode(500, new { message = "An internal error occurred while trying to send the email." });
            }
        }
    }    
}

