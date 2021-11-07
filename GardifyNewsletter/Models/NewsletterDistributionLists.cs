using System;
using System.Collections.Generic;

namespace GardifyNewsletter.Models
{
    public partial class NewsletterDistributionLists
    {
        public int NewsletterDistributionListId { get; set; }
        public string NewsletterDistributionListName { get; set; }
        public string ApplicationId { get; set; }
        public int? LanguageId { get; set; }
        public bool? Active { get; set; }
        public bool? Deleted { get; set; }
        public int? Sort { get; set; }
        public DateTime? WrittenDate { get; set; }
        public string WrittenBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public string EditedBy { get; set; }
        public bool? NotEditable { get; set; }
        public virtual ICollection<NewsletterRecipients> Recipients { get; set; }
    }
}
