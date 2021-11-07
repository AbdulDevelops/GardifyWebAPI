using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static GardifyModels.Models.ArticleViewModels;
using static GardifyModels.Models.ModelEnums;
using static GardifyModels.Models.ShopcartViewModels;

namespace GardifyModels.Models
{
    public class ShopOrder : _BaseEntity
    {
        public ShopOrder()
        {
            this.ArticlesInCart = new HashSet<ShopCartEntry>();
        }

        public bool PaymentConfirmed { get; set; }
        public bool OrderConfirmed { get; set; }
        public string PaidWith { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public int TransactionId { get; set; }
        public string TCResponseCode { get; set; }
        public OrderStatus Status { get; set; }
        public DateTime? ShippingDate { get; set; }
        public DateTime? CompletionDate { get; set; }
        public string Comment { get; set; }
        // Shipping
        public string CustomerName { get; set; }
        public string Street { get; set; }
        public string HouseNr { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        // Invoice
        public string InvoiceCustomerName { get; set; }
        public string InvoiceStreet { get; set; }
        public string InvoiceHouseNr { get; set; }
        public string InvoiceCity { get; set; }
        public string InvoiceZip { get; set; }
        public string InvoiceCountry { get; set; }
        public Guid UserId { get; set; }
        public virtual ICollection<ShopCartEntry> ArticlesInCart { get; set; }

        public string OrderId(int publisherNr)
        {
            return publisherNr.ToString() + "-" + Id.ToString();
        }
    }

    public class PurchasedArticleViewModel
    {
        [Required]
        public int ArticleId { get; set; }
        public string Description { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public DateTime PurchaseDate { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public List<_HtmlImageViewModel> ArticleImages { get; set; }
    }

    public class OrderViewModel
    {
        public OrderViewModel()
        {
            OrderedArticles = new List<ShopcartEntryViewModel>();
        }
        public int ShopOrderId { get; set; }
        public string OrderId { get; set; }    // pubNr-OrderId
        public bool OrderConfirmed { get; set; }
        public decimal OrderAmount { get; set; }
        public decimal ShippingAmount { get; set; }
        public int TransactionId { get; set; }
        public string TCResponseCode { get; set; }
        public OrderStatus Status { get; set; }
        public string Comment { get; set; }
        public string CustomerName { get; set; }
        public string Street { get; set; }
        public string HouseNr { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
        public string InvoiceCustomerName { get; set; }
        public string InvoiceStreet { get; set; }
        public string InvoiceHouseNr { get; set; }
        public string InvoiceCity { get; set; }
        public string InvoiceZip { get; set; }
        public string InvoiceCountry { get; set; }
        public DateTime Date { get; set; }
        public string PaidWith { get; set; }
        public string Email { get; set; }
        public List<ShopcartEntryViewModel> OrderedArticles { get; set; }
    }

    public class SubmitOrderViewModel
    {
        [Required]
        public string InvoiceCustomerName { get; set; }
        [Required]
        public string InvoiceStreet { get; set; }
        [Required]
        public string InvoiceHouseNr { get; set; }
        [Required]
        public string InvoiceCity { get; set; }
        [Required]
        public string InvoiceZip { get; set; }
        [Required]
        public string InvoiceCountry { get; set; }
        [Required]
        public string PaymentMethod { get; set; }   // V, M, paypal, sofort, rechnung
        // Shipping
        public string Name { get; set; }
        public string Street { get; set; }
        public string HouseNr { get; set; }
        public string City { get; set; }
        public string Zip { get; set; }
        public string Country { get; set; }
    }
}