using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static GardifyModels.Models._CustomValidations;

namespace GardifyModels.Models
{
    public class ArticleViewModels
    {
        public class ArticleListViewModel : _BaseViewModel
        {
            public IEnumerable<ArticleViewModel> ListEntries { get; set; }
            public _modalStatusMessageViewModel StatusMessage { get; set; }
            public IEnumerable<SelectListItem> CategoriesEntries { get; set; }
            public int SelectedCategoriesId { get; set; }
        }

        public class ArticleViewModel : _BaseViewModel
        {
            public ArticleViewModel()
            {
                ArticleImages = new List<_HtmlImageViewModel>();
                Name = "Artikel wurde entfernt";
                Description = "Artikel wurde entfernt";
            }
            [Required]
            [_Title]
            public string Name { get; set; }
            [Required]
            [_Description]
            public string Description { get; set; }
            public int Id { get; set; }
            [Required]
            [Range(0, float.MaxValue)]
            public decimal NormalPrice { get; set; }
            public string ArticleNr { get; set; }
            public string ProductLink { get; set; }
            public int Sort { get; set; }
            public string Label { get; set; }   // Bezeichnung
            public string Thumbnail { get; set; }
            public string PhotoLink { get; set; }
            public string ExpertTip { get; set; }
            public bool IsWishlisted { get; set; }
            public int PricePercentagePayableWithPoints { get; set; }
            public decimal PricePartAfterPoints { get; set; }
            public List<int> Categories { get; set; }
            public bool IsGiftIdea { get; set; }
            public int PointsToSpend { get; set; }
            public decimal PointsValue { get; set; }
            [Required]
            public bool IsAvailable { get; set; }
            public bool AllowPublishment { get; set; }
            public bool BulkArticle { get; set; }
            public decimal WeightInGrams { get; set; }
            public string MakerId { get; set; } // HerstellerId
            public List<_HtmlImageViewModel> ArticleImages { get; set; }
            public _modalStatusMessageViewModel StatusMessage { get; set; }
            public IEnumerable<ArticleReference> ArticleReferences { get; set; }
            public ArticleReference NewArticleReference { get; set; }
            public IEnumerable<TodoTemplate> TodotemplateReferenceList { get; set; }
            public IEnumerable<Plant> PlantReferenceList { get; set; }
            public string AffiliateLink { get; set; }
            public string EANCode { get; set; }
            public bool IsNotDeliverable { get; set; }
            public decimal VAT { get; set; }
            public string Brand { get; set; }
            public string HazardNotice { get; set; }
            public string Author { get; set; }
        }
        public class ArticleViewModelLite
        {
            public ArticleViewModelLite()
            {
                ArticleImages = new List<_HtmlImageViewModel>();
            }
            public int Id { get; set; }
            public string Name { get; set; }
            public string Description { get; set; }
            [Range(0, float.MaxValue)]
            public decimal NormalPrice { get; set; }
            public string ArticleNr { get; set; }
            public string ProductLink { get; set; }
            public string Label { get; set; }   // Bezeichnung
            public string Thumbnail { get; set; }
            public string PhotoLink { get; set; }
            public List<_HtmlImageViewModel> ArticleImages { get; set; }
        }
        
    }
}