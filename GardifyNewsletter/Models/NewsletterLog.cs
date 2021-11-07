using System;
using System.Collections.Generic;

namespace GardifyNewsletter.Models
{
    public partial class NewsletterLog
    {
        public int NewsletterLogId { get; set; }
        public string NewsletterLogText { get; set; }
        public DateTime? NewsletterLogDatum { get; set; }
    }
}
