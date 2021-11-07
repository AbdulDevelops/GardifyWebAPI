using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using GardifyWebAPI.Services;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using static GardifyModels.Models.ArticleViewModels;
using static GardifyModels.Models.ShopcartViewModels;

namespace GardifyWebAPI.Controllers
{
    public class OrdersAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private AspNetUserManager userManager;
        private ShopcartController scc = new ShopcartController();
        private ArticleController ac = new ArticleController();

        public readonly int PUBLISHER_NUMBER = 393;
        private readonly string TELECASH_API_KEY = "47B40C49-E524-47E6-AC4F-4ABC01A4F86C";
        private readonly string TELECASH_SECRET = "23620205";
        private readonly int MIN_ORDER_AMOUNT = 10;

        public AspNetUserManager UserManager
        {
            get
            {
                return userManager ?? Request.GetOwinContext().GetUserManager<AspNetUserManager>();
            }
            private set
            {
                userManager = value;
            }
        }

        [HttpGet]
        [Route("api/OrdersAPI/")]
        public IEnumerable<OrderViewModel> GetUserOrders()
        {
            var userId = Utilities.GetUserId();
            var res = new List<OrderViewModel>();
            var list = db.ShopOrders.Where(o => o.UserId == userId && (o.TCResponseCode == "Y" || o.OrderConfirmed)).OrderByDescending(o => o.CreatedDate);
            foreach (ShopOrder order in list)
            {
                var temp = new OrderViewModel()
                {
                    OrderId = order.OrderId(PUBLISHER_NUMBER),
                    OrderConfirmed = order.OrderConfirmed,
                    OrderAmount = order.OrderAmount,
                    ShippingAmount = order.ShippingAmount,
                    TransactionId = order.TransactionId,
                    CustomerName = order.CustomerName,
                    Street = order.Street,
                    HouseNr = order.HouseNr,
                    Date = order.CreatedDate,
                    City = order.City,
                    Zip = order.Zip,
                    Country = order.Country,
                    InvoiceCustomerName = order.InvoiceCustomerName,
                    InvoiceStreet = order.InvoiceStreet,
                    InvoiceHouseNr = order.InvoiceHouseNr,
                    InvoiceCity = order.InvoiceCity,
                    InvoiceZip = order.InvoiceZip,
                    InvoiceCountry = order.InvoiceCountry,
                    OrderedArticles = order.ArticlesInCart.Select(a => new ShopcartEntryViewModel()
                    {
                        Id = a.Id,
                        Quantity = a.Quantity,
                        IsPreorder = a.IsPreorder,
                        ArticleView = ac.GetArticleVMByCartEntry(a.Id, userId)
                    }).ToList()
                };
                res.Add(temp);
            }
            return res;
        }

        [HttpGet]
        [Route("api/OrdersAPI/{orderId}")]
        public IHttpActionResult GetUserOrder(string orderId)
        {
            var userId = Utilities.GetUserId();
            int intId = 0;

            Int32.TryParse(orderId.Split('-')[1], out intId);
            var order = db.ShopOrders.Where(o => o.UserId == userId && o.Id == intId && (o.TCResponseCode == "Y" || o.OrderConfirmed)).FirstOrDefault();
            if (order == null)
            {
                return BadRequest("Bestellung wurde nicht gefunden.");
            }

            var res = new OrderViewModel()
            {
                OrderId = orderId,
                OrderAmount = order.OrderAmount,
                TransactionId = order.TransactionId,
                ShippingAmount = order.ShippingAmount,
                OrderConfirmed = order.OrderConfirmed,
                CustomerName = order.CustomerName,
                Street = order.Street,
                HouseNr =order.HouseNr,
                Date = order.CreatedDate,
                City = order.City,
                Zip = order.Zip,
                Country = order.Country,
                InvoiceCustomerName = order.InvoiceCustomerName,
                InvoiceStreet = order.InvoiceStreet,
                InvoiceHouseNr = order.InvoiceHouseNr,
                InvoiceCity = order.InvoiceCity,
                InvoiceZip = order.InvoiceZip,
                InvoiceCountry = order.InvoiceCountry,
                OrderedArticles = order.ArticlesInCart.Select(a => new ShopcartEntryViewModel()
                {
                    Id = a.Id,
                    Quantity = a.Quantity,
                    IsPreorder = a.IsPreorder,
                    ArticleView = ac.GetArticleVMByCartEntry(a.Id, userId)
                }).ToList()
            };
            return Ok(res);
        }

        [HttpPost]
        [Route("api/OrdersAPI/submit")]
        public async Task<IHttpActionResult> SubmitOrder(SubmitOrderViewModel vm, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            var userId = Utilities.GetUserId();
            var cart = scc.Index().ShopCartEntries;
            var orderAmount = cart.TotalNormal;
            ApplicationUser user = await UserManager.FindByIdAsync(userId.ToString());
            string resMsg = "";
            string[] allowedMethods = { "rechnung", "V", "M", "sofort", "paypal" };

            if (!ModelState.IsValid)
            {
                return BadRequest("Die Rechnungsadresse ist nicht vollständig");
            }

            if (!allowedMethods.Contains(vm.PaymentMethod))
            {
                return BadRequest("Zahlungsart ist ungültig");
            }

            if (user == null)
            {
                return BadRequest("Du bist noch nicht angemeldet");
            }

            if (cart.TotalNormal < MIN_ORDER_AMOUNT)
            {
                return BadRequest("Du hast den Mindestbestellwert von 10€ noch nicht erreicht");
            }

            // update placeholder props if address is not complete
            if (!user.CompleteSignup && !user.Email.Contains("UserDemo"))
            {
                try
                {
                    user.FirstName = user.FirstName.Contains("Platzhalter") ? vm.InvoiceCustomerName.Trim().Split(' ').FirstOrDefault() : user.FirstName;
                    user.LastName = user.LastName.Contains("Platzhalter") ? vm.InvoiceCustomerName.Trim().Split(' ').Skip(1).Aggregate((a, b) => a + " " + b) : user.LastName;
                    user.City = user.City.Contains("Platzhalter") ? vm.InvoiceCity : user.City;
                    user.Country = user.Country.Contains("Platzhalter") ? vm.InvoiceCountry : user.Country;
                    user.Street = user.Street.Contains("Platzhalter") ? vm.InvoiceStreet : user.Street;
                    user.HouseNr = user.Street.Contains("Platzhalter") ? vm.InvoiceHouseNr : user.HouseNr;
                    user.CompleteSignup = true;
                    var currentStatistic = StatisticEventTypes.RegisterConverted;
                    
                        if (isIos)
                        {
                            new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromIos);
                        }
                        else if (isAndroid)
                        {
                            new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromAndroid);
                        }
                        else if (isWebPage)
                        {
                            new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromWebpage);
                        }

                   
                   
                    await UserManager.UpdateAsync(user);
                    resMsg = "Wir haben deine Adresse ergänzt.";
                }
                catch (Exception) { }
            }

            // create order in db, get order id
            var newOrder = new ShopOrder()
            {
                PaidWith = vm.PaymentMethod,
                OrderAmount = orderAmount,
                ShippingAmount = cart.ShippingCosts,
                CustomerName = string.IsNullOrEmpty(vm.Name) ? vm.InvoiceCustomerName : vm.Name,
                Street = string.IsNullOrEmpty(vm.Street) ? vm.InvoiceStreet : vm.Street,
                HouseNr = string.IsNullOrEmpty(vm.HouseNr) ? vm.HouseNr : vm.InvoiceHouseNr,
                City = string.IsNullOrEmpty(vm.City) ? vm.InvoiceCity : vm.City,
                Zip = string.IsNullOrEmpty(vm.Zip) ? vm.InvoiceZip : vm.Zip,
                Country = string.IsNullOrEmpty(vm.Country) ? vm.InvoiceCountry : vm.Country,
                InvoiceCustomerName = vm.InvoiceCustomerName,
                InvoiceStreet = vm.InvoiceStreet,
                InvoiceHouseNr = vm.InvoiceHouseNr,
                InvoiceCity = vm.InvoiceCity,
                InvoiceZip = vm.InvoiceZip,
                InvoiceCountry = vm.InvoiceCountry,
                UserId = userId,
                Status = ModelEnums.OrderStatus.New
            };

            foreach (ShopcartEntryViewModel avm in cart.EntriesList)
            {
                var art = db.ShopCartEntry.Where(a => a.Id == avm.Id).FirstOrDefault();
                newOrder.ArticlesInCart.Add(art);
            }

            db.ShopOrders.Add(newOrder);
            var added = db.SaveChanges();

            return Ok(new { OrderId = newOrder.Id, Msg = resMsg });
        }

        [HttpGet]
        [Route("api/OrdersAPI/checkout/{orderId}")]
        public FormData GetCheckout(int orderId)
        {
            ShopcartController sh = new ShopcartController();
            var userId = Utilities.GetUserId();

            var order = db.ShopOrders.Where(o => o.Id == orderId && o.UserId == userId).FirstOrDefault();
            var orderView = new FormData()
            {
                amount = order.OrderAmount + order.ShippingAmount,
                user = PUBLISHER_NUMBER,
                key = TELECASH_API_KEY,
                payment_method = order.PaidWith,
                orderid = order.OrderId(PUBLISHER_NUMBER),
                hash = HashOrder(order.OrderAmount + order.ShippingAmount),
                mobile = Mode.Ja,
                testmodus = Mode.Nein,
                success = "https://gardifybackend.sslbeta.de/api/OrdersAPI/paysuccess",
                abort = "https://gardifybackend.sslbeta.de/api/OrdersAPI/payfail",
                Cart = sh.Index().ShopCartEntries,
                Street = order.Street,
                Zip = order.Zip,
                CustomerName = order.CustomerName,
                City = order.City,
                Country = order.Country,
                InvoiceCustomerName = order.InvoiceCustomerName,
                InvoiceStreet = order.InvoiceStreet,
                InvoiceHouseNr = order.InvoiceHouseNr,
                InvoiceCity = order.InvoiceCity,
                InvoiceZip = order.InvoiceZip,
                InvoiceCountry = order.InvoiceCountry,
            };

            return orderView;
        }

        [HttpPut]
        [Route("api/OrdersAPI/update/{orderId}")]
        public IHttpActionResult UpdateOrder(UpdateOrderVM vm)
        {
            ShopcartController sh = new ShopcartController();
            var userId = Utilities.GetUserId();
            string[] allowedMethods = { "rechnung", "V", "M", "sofort", "paypal" };

            if (!allowedMethods.Contains(vm.PaidWith))
            {
                return BadRequest("Zahlungsart ist ungültig");
            }

            var order = db.ShopOrders.Where(o => o.Id == vm.OrderId && o.UserId == userId).FirstOrDefault();
            if (order == null)
            {
                return BadRequest("Bestellung wurde nicht gefunden.");
            }

            var cart = sh.Index();
            order.ShippingAmount = cart.ShopCartEntries.ShippingCosts;
            order.OrderAmount = cart.ShopCartEntries.TotalNormal;
            order.PaidWith = string.IsNullOrEmpty(vm.PaidWith) ? order.PaidWith : vm.PaidWith;
            db.SaveChanges();

            var orderView = new FormData()
            {
                amount = order.OrderAmount + order.ShippingAmount,
                user = PUBLISHER_NUMBER,
                key = TELECASH_API_KEY,
                payment_method = order.PaidWith,
                orderid = order.OrderId(PUBLISHER_NUMBER),
                hash = HashOrder(order.OrderAmount + order.ShippingAmount),
                mobile = Mode.Ja,
                testmodus = Mode.Nein,
                success = "https://gardifybackend.sslbeta.de/api/OrdersAPI/paysuccess",
                abort = "https://gardifybackend.sslbeta.de/api/OrdersAPI/payfail",
                Cart = cart.ShopCartEntries,
                Street = order.Street,
                Zip = order.Zip,
                CustomerName = order.CustomerName,
                City = order.City,
                Country = order.Country,
                InvoiceCustomerName = order.InvoiceCustomerName,
                InvoiceStreet = order.InvoiceStreet,
                InvoiceHouseNr = order.InvoiceHouseNr,
                InvoiceCity = order.InvoiceCity,
                InvoiceZip = order.InvoiceZip,
                InvoiceCountry = order.InvoiceCountry,
            };

            return Ok(orderView);
        }

        [HttpPost]
        [Route("api/OrdersAPI/payfail")]
        public HttpResponseMessage HandleFailedPayment(PaymentResponse vm)
        {
            string baseUrl = "https://gardify.de";
            int intId = 0;

            Int32.TryParse(vm.orderid.Split('-')[1], out intId);
            var order = db.ShopOrders.Where(o => o.Id == intId).FirstOrDefault();

            if (order != null)
            {
                // update shopOrder
                order.TransactionId = vm.transactionid;
                order.TCResponseCode = vm.responsecode;
                order.OrderConfirmed = true;

                if (order.TCResponseCode == "Y")
                {
                    order.PaymentConfirmed = true;
                }
                db.SaveChanges();
            }

            string endpoint = "/bestellung/fehler/" + vm.responsecode + "/" + vm.orderid;
            var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Found);
            httpResponseMessage.Headers.Location = new Uri(baseUrl + endpoint, UriKind.Absolute);
            return httpResponseMessage;
        }

        [HttpGet]
        [Route("api/OrdersAPI/paywithinvoice/{orderId}")]
        public async Task<IHttpActionResult> HandlePaidWithInvoiceAsync(int orderId, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            var order = db.ShopOrders.Where(o => o.Id == orderId).FirstOrDefault();
            if (order == null)
            {
                return BadRequest("Bestellung wurde nicht gefunden.");
            }

            if (!order.OrderConfirmed)
            {
                order.OrderConfirmed = true;
                db.SaveChanges();
                await NotifyByEmail(new PaymentResponse() { orderid = order.OrderId(PUBLISHER_NUMBER) }, order);
                scc.ClearShopCart(order.UserId);
            }

            
            var currentStatistic = StatisticEventTypes.OrderConfirmed;
            
                if (isIos)
                {
                    new StatisticsController().CreateEntry(currentStatistic, order.UserId, (int)StatisticEventTypes.ApiCallFromIos);
                }
                else if (isAndroid)
                {
                    new StatisticsController().CreateEntry(currentStatistic, order.UserId, (int)StatisticEventTypes.ApiCallFromAndroid);
                }
                else if (isWebPage)
                {
                    new StatisticsController().CreateEntry(currentStatistic, order.UserId, (int)StatisticEventTypes.ApiCallFromWebpage);
                }

           
            
            return Ok();
        }

        [HttpPost]
        [Route("api/OrdersAPI/paysuccess")]
        public async Task<HttpResponseMessage> HandleSuccessfulPayment(PaymentResponse vm, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            string baseUrl = "https://gardify.de"; 
            int intId = 0;

            Int32.TryParse(vm.orderid.Split('-')[1], out intId);
            var order = db.ShopOrders.Where(o => o.Id == intId).FirstOrDefault();

            if (order != null)
            {
                // update shopOrder
                order.TransactionId = vm.transactionid;
                order.TCResponseCode = vm.responsecode;
                order.OrderConfirmed = true;
                db.SaveChanges();

                string endpoint = "/bestellung/erfolgreich/" + vm.orderid;
                var httpResponseMessage = new HttpResponseMessage(HttpStatusCode.Found);
                httpResponseMessage.Headers.Location = new Uri(baseUrl + endpoint, UriKind.Absolute);

                if (!order.PaymentConfirmed)
                {
                    order.PaymentConfirmed = true;
                    db.SaveChanges();

                    // clear user cart
                    scc.ClearShopCart(order.UserId);
                    
                    await NotifyByEmail(vm, order);
                }

                var currentStatistic = StatisticEventTypes.OrderConfirmed;
               
                    if (isIos)
                    {
                        new StatisticsController().CreateEntry(currentStatistic, order.UserId, (int)StatisticEventTypes.ApiCallFromIos);
                    }
                    else if (isAndroid)
                    {
                        new StatisticsController().CreateEntry(currentStatistic, order.UserId, (int)StatisticEventTypes.ApiCallFromAndroid);
                    }
                    else if (isWebPage)
                    {
                        new StatisticsController().CreateEntry(currentStatistic, order.UserId, (int)StatisticEventTypes.ApiCallFromWebpage);
                    }

               
                return httpResponseMessage;
            }
            return new HttpResponseMessage(HttpStatusCode.Forbidden);
        }

        private string HashOrder(decimal amount)
        {
            string str = PUBLISHER_NUMBER.ToString() + amount.ToString("N", new CultureInfo("en-US")) + TELECASH_SECRET;
            byte[] tmpSource;
            byte[] tmpHash;

            //Create a byte array from source data.
            tmpSource = System.Text.UnicodeEncoding.UTF8.GetBytes(str);
            tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);
            return BitConverter.ToString(tmpHash).Replace("-", "").ToLower();
        }

        private async Task<bool> NotifyByEmail(PaymentResponse vm, ShopOrder order)
        {
            EmailSender es = new EmailSender(db);
            TemplateService ts = new TemplateService();
            var user = await UserManager.FindByIdAsync(order.UserId.ToString());
            string content = ts.RenderTemplateAsync("OrderConfirmation", new
            {
                UserName = user.DisplayName(),
                OrderId = vm.orderid,
                Articles = order.ArticlesInCart.Select(a => new ShopcartEntryViewModel()
                {
                    Id = a.Id,
                    Quantity = a.Quantity,
                    IsPreorder = a.IsPreorder,
                    ArticleView = ac.GetArticleVMByCartEntry(a.Id, new Guid(user.Id))
                }).ToList(),
                Order = order,
                TotalAmount = order.ShippingAmount + order.OrderAmount,
                User = user
            });
            // to user
            await es.SendEmail("Deine Bestellung bei gardify - " + vm.orderid, content, "shop@gardify.de", user.Email, null);
            // to admins
            await es.SendEmail("Bestellung " + vm.orderid, content, "shop@gardify.de", "info@bjvv.de", null);
            await es.SendEmail("Bestellung " + vm.orderid, content, "shop@gardify.de", "bjvv@lkg.eu", null);
            return true;
        }
    }

    public class FormData
    {
        public int user { get; set; }
        public string key { get; set; }
        public string payment_method { get; set; }
        public decimal amount { get; set; }     // Punkt als Trennzeichen
        public string orderid { get; set; }     // inklusive Verlagsnummer am Anfang
        public string hash { get; set; }
        public string success { get; set; }
        public Mode testmodus { get; set; }   // optional
        public Mode mobile { get; set; }     // optional
        public string abort { get; set; }   // optional
        public ShopcartEntriesListViewModel Cart { get; set; }
        public string Street { get; set; }
        public string Zip { get; set; }
        public string CustomerName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string InvoiceCustomerName { get; set; }
        public string InvoiceStreet { get; set; }
        public string InvoiceHouseNr { get; set; }
        public string InvoiceCity { get; set; }
        public string InvoiceZip { get; set; }
        public string InvoiceCountry { get; set; }
    }

    public class PaymentResponse
    {
        public int transactionid { get; set; }
        public string payment_method { get; set; }
        public decimal amount { get; set; }     // Punkt als Trennzeichen
        public string orderid { get; set; }
        public string responsecode { get; set; }
        public string errortext1 { get; set; }      // user-facing
        public string errortext2 { get; set; }      // internal
    }

    public class OrderSubmitMessage
    {
        public int OrderId { get; set; }
        public string Success { get; set; }
        public string Error { get; set; }
    }

    public class UpdateOrderVM
    {
        public int OrderId { get; set; }
        public string PaidWith { get; set; }
    }

    public enum Mode
    {
        Nein = 0, Ja = 1
    }
}
