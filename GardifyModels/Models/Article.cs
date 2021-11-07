namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Article : _BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Article()
        {
            ArticleReferences = new HashSet<ArticleReference>();
            ArticleCategories = new HashSet<ArticleCategory>();
            ArticleShopOrders = new HashSet<ShopOrder>();
            ShopCartEntries = new HashSet<ShopCartEntry>();
        }

        public string ArticleNr { get; set; }
        public string ProductLink { get; set; }
        public string Label { get; set; }   // Bezeichnung
        public string Thumbnail { get; set; }
        public string PhotoLink { get; set; }

        public string EANCode { get; set; }
        public bool IsNotDeliverable { get; set; }
        public decimal VAT { get; set; }
        public string Brand { get; set; }
        public string HazardNotice { get; set; }
        public string Author { get; set; }
        public bool BulkArticle { get; set; }   // Sperrgut
        public decimal WeightInGrams { get; set; }
        public string MakerId { get; set; } // HerstellerId

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        [Required]
        public string Description { get; set; } // Beschreibung

        public decimal Price { get; set; }

        public bool IsAvailable { get; set; }
        public bool AllowPublishment { get; set; }
        public string AffiliateLink { get; set; }
        public int Sort { get; set; }
        public string ExpertTip { get; set; }

        public int PricePercentagePayableWithPoints { get; set; }

        public bool PremiumOnly { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArticleReference> ArticleReferences { get; set; }

        public virtual ICollection<ArticleCategory> ArticleCategories { get; set; }
        public virtual ICollection<ShopOrder> ArticleShopOrders { get; set; }
        public virtual ICollection<ShopCartEntry> ShopCartEntries { get; set; }
        public static explicit operator Article(List<object> v)
        {
            throw new NotImplementedException();
        }
    }
}
