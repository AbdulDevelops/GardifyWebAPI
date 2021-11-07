using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GardifyNewsletter.Models
{
    public partial class NewsletterComponents
    {
        public int NewsletterComponentId { get; set; }
        public string ApplicationId { get; set; }
        public int? BelongsToNewsletterId { get; set; }
        [ForeignKey("BelongsToNewsletterId")]
        public virtual Newsletter Newsletter { get; set; }
        /// <summary>
        /// Repurposed to become an enum instead of an unused table
        /// </summary>
        public ComponentType? NewsletterComponentTemplateId { get; set; }
        public string NewsletterComponentHeadline { get; set; }
        public string NewsletterComponentSubline { get; set; }
        public string NewsleterComponentText { get; set; }
        public string NewsletterPicLink { get; set; }
        public string NewsletterMoreLink { get; set; }

        public string CustomLinkText { get; set; }
        public bool? Active { get; set; }
        public bool? Deleted { get; set; }
        public int? Sort { get; set; }
        public DateTime? WrittenDate { get; set; }
        public string WrittenBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public string EditedBy { get; set; }
        public bool? NotEditable { get; set; }
        public int? NewsId { get; set; }
        public int? ShortArticleId { get; set; }
        public virtual NewsletterNewPlants NewsletterNewPlants { get; set; }


    }

    public enum ComponentType
    {

        [Display(Name = "Shopartikel mit Bild")]
        ArticleWithImage = 0,
        //[Display(Name = "Artikel ohne Bild")]
        //ArticleWithoutImage = 1,
        [Display(Name = "Trenner")]
        Divider = 2,
        [Display(Name = "Fußzeile")]
        Footer = 3,
        [Display(Name = "Freitext")]
        FreeText = 4,
        [Display(Name = "Freitext Kopfzeile")]
        FreeTextHeader = 5,
        [Display(Name = "Freitext ohne Bild")]
        FreeTextWithoutImage = 6,
        [Display(Name = "Kopfzeile")]
        Header = 7,
        //[Display(Name = "News Kopfzeile")]
        //NewsHeader = 8,
        [Display(Name = "Neue Pflanzen mit Bild")]
        NewPlantWithImage = 8,
        [Display(Name = "Pflanze mit Bild")]
        NewsWithImage = 9,
        //[Display(Name = "News ohne Bild")]
        //NewsWithoutImage = 10,
        [Display(Name = "Schlusssatz")]
        PostText = 11,
        [Display(Name = "Schlusssatz ohne Bild")]
        PostTextWithoutImage = 12,
        [Display(Name = "Einleitung")]
        PreText = 13,
        [Display(Name = "Einleitung ohne Bild")]
        PreTextWithoutImage = 14
    }
}
