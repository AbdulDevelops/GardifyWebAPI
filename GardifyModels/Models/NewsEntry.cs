namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class NewsEntry : _BaseEntity
    {

        [Required]
        [StringLength(256)]
        public string Title { get; set; }
        public string SubTitle { get; set; }
        [Required]
        public string Theme { get; set; }
        public string Author { get; set; }
        public string Timing { get; set; }
        public string Tipp { get; set; }

        [Required]
        public string Text { get; set; }

        public DateTime Date { get; set; }

        public DateTime ValidFrom { get; set; }

        public DateTime ValidTo { get; set; }

        public bool IsVisibleOnPage { get; set; }

        public bool PremiumOnly { get; set; }
    }
   
}
