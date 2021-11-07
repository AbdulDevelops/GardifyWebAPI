using System;
using System.Collections.Generic;

namespace GardifyNewsletter.Models
{
    public partial class NewsletterComponentsTemplates
    {
        public int NewsletterComponentsTemplateId { get; set; }
        public string NewsletterComponentName { get; set; }
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
