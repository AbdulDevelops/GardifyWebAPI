namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ShopCartEntry: _BaseEntity
    {
        public ShopCartEntry()
        {
            this.OrdersWithThisArticle = new HashSet<ShopOrder>();
        }

        [Required]
        public Guid UserId { get; set; }
        [Required]
        public int ArticleId { get; set; }
        [NotMapped]
        public Article Article { get; set; }
        [Required]
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        public bool IsWishlisted { get; set; }
        public bool IsInCart { get; set; }
        public bool IsPreorder { get; set; }

        public virtual ICollection<ShopOrder> OrdersWithThisArticle { get; set; }
    }
}
