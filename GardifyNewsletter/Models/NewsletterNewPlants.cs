using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace GardifyNewsletter.Models
{
    public class NewsletterNewPlants
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey("NewsletterComponents")]
        public int NewPlantComponentId { get; set; }
        public int? NewPlant1 { get; set; }
        public string NewPlant1SubHeadline { get; set; }
        public int? NewPlant2 { get; set; }
        public string NewPlant2SubHeadline { get; set; }
        public int? NewPlant3 { get; set; }
        public string NewPlant3SubHeadline { get; set; }
        public string NewPlantMonth { get; set; }

        public virtual NewsletterComponents NewsletterComponents { get; set; }

    }
}
