using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
	public class ShopcartViewModels
	{
		public class ShopcartEntriesListViewModel : _BaseViewModel
		{
            private readonly decimal BULK_ARTICLE_FLAT_RATE = 4.95m;
            private readonly decimal FREE_SHIPPING_MIN_RATE = 50m;

            public IEnumerable<ShopcartEntryViewModel> EntriesList { get; set; }
			public decimal TotalNormal { get; set; }
			public decimal TotalWithPoints { get; set; }
            public decimal ShippingCosts { get; set; }
            [JsonIgnore]
            public decimal ShippingFlatrate { get; set; }
            public decimal TotalPoints { get; set; }
			public _modalStatusMessageViewModel StatusMessage { get; set; }

			public void ComputeTotals()
			{
				if (EntriesList != null && EntriesList.Any())
				{
					TotalNormal = 0;
					TotalWithPoints = 0;
					TotalPoints = 0;

					foreach (ShopcartViewModels.ShopcartEntryViewModel entry in EntriesList)
					{
						TotalNormal += entry.ArticleView.NormalPrice * entry.Quantity;
						TotalWithPoints += entry.ArticleView.PricePartAfterPoints * entry.Quantity;
						TotalPoints += entry.ArticleView.PointsToSpend * entry.Quantity;
					}					
				}

                ComputeShippingCosts();
            }

            public void ComputeShippingCosts()
            {
                if (TotalNormal < FREE_SHIPPING_MIN_RATE)
                {
                    var total = ShippingFlatrate;

                    if (EntriesList != null && EntriesList.Any())
                    {
                        foreach (ShopcartViewModels.ShopcartEntryViewModel entry in EntriesList)
                        {
                            total += entry.ArticleView.BulkArticle ? (BULK_ARTICLE_FLAT_RATE * entry.Quantity) : 0;
                        }
                    }
                    this.ShippingCosts = total;
                } else
                {
                    this.ShippingCosts = 0;
                }
            }

            public void SetFlatrateByWeight()
            {
                decimal totalWeight = 0;

                if (EntriesList != null && EntriesList.Any())
                {
                    foreach (ShopcartViewModels.ShopcartEntryViewModel entry in EntriesList)
                    {
                        totalWeight += entry.ArticleView.WeightInGrams;
                    }
                }

                if (totalWeight <= 500)
                {
                    ShippingFlatrate = 7;
                }
                else if (totalWeight <= 1000)
                {
                    ShippingFlatrate = 10;
                }
                else if (totalWeight <= 2000)
                {
                    ShippingFlatrate = 19;
                }
                else if (totalWeight <= 4000)
                {
                    ShippingFlatrate = 38;
                }
                else // 10,00€ je weiteres kg
                {
                    ShippingFlatrate = 38;
                    var overMax = totalWeight - 4000;
                    ShippingFlatrate += (Math.Ceiling(overMax / 1000) * 10);
                }
            }
        }

		public class ShopcartEntryViewModel
		{
			public int Id { get; set; }
			public ArticleViewModels.ArticleViewModel ArticleView { get; set; }
			[Required]
			[Range(0, int.MaxValue)]
			public int Quantity { get; set; }
            public bool IsPreorder { get; set; }
        }
        public class WishListEntryViewModelLite
        {
            public int Id { get; set; }
            public ArticleViewModels.ArticleViewModelLite ArticleView { get; set; }
            [Required]
            [Range(0, int.MaxValue)]
            public int Quantity { get; set; }
        }
        public class ShopcartEntryAndWishListViewModel
        {
            public ShopcartEntriesListViewModel ShopCartEntries { get; set; }
            public IEnumerable<WishListEntryViewModelLite> WishListEntries { get; set; }
        }
        public class _modalArticleAddedToShopcartViewModel
		{
			public string Message { get; set; }
			public ModelEnums.ActionStatus Status { get; set; }
		}
	}
}