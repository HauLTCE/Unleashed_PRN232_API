namespace EmailService.Models.Internal
{
    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUser { get; set; }
        public string SmtpPass { get; set; }

        /// <summary>
        /// The 'From' address to use if one isn't specified in the EmailMessage.
        /// </summary>
        public string DefaultFromAddress { get; set; }

        /// <summary>
        /// The 'From' name (e.g., "My App Support") to use.
        /// </summary>
        public string DefaultFromName { get; set; }
    }
}
