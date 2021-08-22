using System.Collections.Generic;
using System.Net.Mail;

namespace Lingua.Shared
{
    public class EmailMessage
    {
        public EmailMessage()
        {
            Attachments = new List<Attachment>();
        }

        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsHtml { get; set; }
        public IList<Attachment> Attachments { get; set; }
    }
}
