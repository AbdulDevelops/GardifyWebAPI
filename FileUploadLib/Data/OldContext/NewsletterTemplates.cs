using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class NewsletterTemplates
    {
        public int NewsletterTemplateId { get; set; }
        public string NewsletterTemplateName { get; set; }
        public string NewsletterTemplateHeaderHtml { get; set; }
        public string NewsletterTemplateFooterHtml { get; set; }
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
