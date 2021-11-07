using System;
using System.Collections.Generic;

namespace GardifyNewsletter.Models
{
    public partial class Newsletter
    {
        public int NewsletterId { get; set; }
        public string ApplicationId { get; set; }
        public int? NewsletterTemplateId { get; set; }
        public string NewsletterInternalName { get; set; }
        public string NewsletterDateShownOnNewsletter { get; set; }
        public string NewsletterHeaderText { get; set; }
        public string NewsletterMainPicLink { get; set; }
        public string NewsletterStatus { get; set; }
        public DateTime? NewsletterSentDate { get; set; }
        public string SenderEmail { get; set; }
        public string SenderName { get; set; }
        public string SenderReplyTo { get; set; }
        public string Subject { get; set; }
        public string NewsletterCompleteHtml { get; set; }
        public bool? Active { get; set; }
        public bool? Deleted { get; set; }
        public int? Sort { get; set; }
        public DateTime? WrittenDate { get; set; }
        public string WrittenBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public string EditedBy { get; set; }
        public bool? NotEditable { get; set; }
        public virtual ICollection<NewsletterComponents> NewsletterComponents { get; set; }
    }
}
