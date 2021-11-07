using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class NewsletterRecipients
    {
        public int NewsletterRecipientId { get; set; }
        public int? EntityId { get; set; }
        public int? LanguageId { get; set; }
        public string RecipientEmail { get; set; }
        public string RecipientName { get; set; }
        public string MailComesFromThisExternalList { get; set; }
        public string FlagPromotionKindOf { get; set; }
        public string ImportedBy { get; set; }
        public int? NewsletterDistributionListId { get; set; }
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
