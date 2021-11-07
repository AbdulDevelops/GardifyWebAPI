using System;
using System.Collections.Generic;

namespace GardifyNewsletter.Models
{
    public partial class NewsletterSpool
    {
        public int NewsletterSpoolId { get; set; }
        public string SenderDomain { get; set; }
        public int? NewsletterId { get; set; }
        public int? NewsletterDistributionListId { get; set; }
        public int? RecipientId { get; set; }
        public Guid? UserId { get; set; }
        public string RecipientEmail { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
        public string FromReplyTo { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool? Html { get; set; }
        public int? Port { get; set; }
        public bool? Subscriber { get; set; }

        public DateTime? Scheduled { get; set; }
        public string Credentials { get; set; }
        public DateTime? AddedToSpool { get; set; }
        public bool? Send { get; set; }
    }
}
