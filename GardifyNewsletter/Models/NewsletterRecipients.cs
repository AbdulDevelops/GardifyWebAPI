using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace GardifyNewsletter.Models
{
    public partial class NewsletterRecipients
    {
        public int NewsletterRecipientId { get; set; }
        public string ApplicationId { get; set; }
        public int? LanguageId { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientName { get; set; }
        public string MailComesFromThisExternalList { get; set; }
        public string FlagPromotionKindOf { get; set; }
        public string ImportedBy { get; set; }
        public int? NewsletterDistributionListId { get; set; }
        [ForeignKey("NewsletterDistributionListId")]
        public virtual NewsletterDistributionLists NewsletterDistributionList { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public Guid? RegistrationCode { get; set; }
        public bool? Confirmed { get; set; }
        public bool? Deleted { get; set; }
        public bool? Active { get; set; }
        public DateTime? WrittenDate { get; set; }
        public string WrittenBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public string EditedBy { get; set; }
        public int? Sort { get; set; }
        public bool? Editable { get; set; }
    }
}
