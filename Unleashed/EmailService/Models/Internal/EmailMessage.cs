using System.Collections.Generic;
namespace EmailService.Models.Internal
{
    public class EmailMessage
    {
        public List<string> ToAddresses { get; set; } = new List<string>();
        public List<string> CcAddresses { get; set; } = new List<string>();
        public List<string> BccAddresses { get; set; } = new List<string>();

        public string? FromAddress { get; set; }

        public string Subject { get; set; }

        public string Body { get; set; }

        public bool IsHtml { get; set; } = true;

        public EmailMessage(string toAddress, string subject, string body)
        {
            ToAddresses.Add(toAddress);
            Subject = subject;
            Body = body;
        }

        public EmailMessage(List<string> toAddresses, string subject, string body)
        {
            ToAddresses = toAddresses;
            Subject = subject;
            Body = body;
        }
    }
}

