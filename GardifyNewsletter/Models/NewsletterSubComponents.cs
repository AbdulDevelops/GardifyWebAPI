using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GardifyNewsletter.Models
{
    public class NewsletterSubComponents
    {
        [Key]
        public int NewsletterSubComponentId { get; set; }

        public int? NewPlantArticleId { get; set; }

        [ForeignKey("NewsletterComponents")]
        public ComponentType? NewsletterComponentTemplateId { get; set; }
        public virtual NewsletterComponents newsletterComponents { get; set; }
    }
}
