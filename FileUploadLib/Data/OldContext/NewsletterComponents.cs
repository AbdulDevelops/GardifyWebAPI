using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class NewsletterComponents
    {
        public int NewsletterComponentId { get; set; }
        public int? EntityId { get; set; }
        public int? BelongsToNewsletterId { get; set; }
        public int? NewsletterComponentTemplateId { get; set; }
        public string NewsletterComponentHeadline { get; set; }
        public string NewsletterComponentSubline { get; set; }
        public string NewsleterComponentText { get; set; }
        public string NewsletterPicLink { get; set; }
        public string NewsletterMoreLink { get; set; }
        public bool? Active { get; set; }
        public bool? Deleted { get; set; }
        public int? Sort { get; set; }
        public DateTime? WrittenDate { get; set; }
        public string WrittenBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public string EditedBy { get; set; }
        public bool? NotEditable { get; set; }
    }
}
