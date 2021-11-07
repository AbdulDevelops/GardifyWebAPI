using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static GardifyModels.Models.ShopcartViewModels;

namespace PflanzenApp.Controllers.AdminArea
{
    public class AdminAreaOrdersController : _BaseController
    {
        public readonly int PUBLISHER_NUMBER = 393;
        public readonly int PAGE_SIZE = 100;

        // GET: AdminAreaOrders
        public ActionResult Index(int page = 1)
        {
            var ac = new ArticleController();
            var viewModel = new List<OrderViewModel>();
            var orders = ctx.ShopOrders.Where(o => !o.Deleted && (!string.IsNullOrEmpty(o.TCResponseCode) || o.OrderConfirmed)).OrderByDescending(o => o.CreatedDate);

            var totalPages = Math.Max(1, (int)Math.Ceiling(Decimal.Divide(orders.Count(), PAGE_SIZE)));
            var firstPage = 1;
            var lastPage = totalPages;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;
            ViewBag.prevPage = Math.Max(page - 1, firstPage);
            ViewBag.nextPage = Math.Min(page + 1, lastPage);

            foreach (ShopOrder order in orders.Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE))
            {
                var temp = new OrderViewModel()
                {
                    ShopOrderId = order.Id,
                    OrderId = order.OrderId(PUBLISHER_NUMBER),
                    TCResponseCode = order.TCResponseCode,
                    Status = order.Status,
                    OrderAmount = order.OrderAmount + order.ShippingAmount,
                    TransactionId = order.TransactionId,
                    CustomerName = order.CustomerName,
                    Date = order.CreatedDate,
                    Comment = order.Comment
                };
                viewModel.Add(temp);
            }
            return View("~/Views/AdminArea/AdminAreaOrders/Index.cshtml", viewModel);
        }

        // GET: AdminAreaOrders/Details/5
        public ActionResult Details(int id)
        {
            var ac = new ArticleController();
            var order = ctx.ShopOrders.Where(o => !o.Deleted && o.Id == id).FirstOrDefault();
            var user = ctx.Users.Where(u => u.Id == order.UserId.ToString()).FirstOrDefault();
            var viewModel = new OrderViewModel()
            {
                ShopOrderId = order.Id,
                Email = user.Email,
                PaidWith = order.PaidWith,
                OrderId = order.OrderId(PUBLISHER_NUMBER),
                TransactionId = order.TransactionId,
                OrderAmount = order.OrderAmount,
                ShippingAmount = order.ShippingAmount,
                TCResponseCode = order.TCResponseCode,
                Date = order.CreatedDate,
                Status = order.Status,
                CustomerName = order.CustomerName,
                City = order.City,
                Street = order.Street,
                Country = order.Country,
                Comment = order.Comment,
                Zip = order.Zip,
                OrderedArticles = order.ArticlesInCart.Select(a => new ShopcartEntryViewModel()
                {
                    Id = a.Id,
                    Quantity = a.Quantity,
                    IsPreorder = a.IsPreorder,
                    //ArticleView = ac.GetArticleViewModel(a.Id)
                }).ToList()
            };
            return View("~/Views/AdminArea/AdminAreaOrders/Detail.cshtml", viewModel);
        }

        // GET: AdminAreaOrders/Details/5
        public ActionResult Edit(OrderViewModel viewModel)
        {
            var order = ctx.ShopOrders.Where(o => !o.Deleted && o.Id == viewModel.ShopOrderId).FirstOrDefault();

            switch (viewModel.Status)
            {
                case ModelEnums.OrderStatus.Shipped: order.ShippingDate = DateTime.Now; break;
                case ModelEnums.OrderStatus.Completed: order.CompletionDate = DateTime.Now; break;
            }
            order.Status = viewModel.Status;
            order.Comment = viewModel.Comment;
            ctx.SaveChanges();
            return View("~/Views/AdminArea/AdminAreaOrders/Detail.cshtml", viewModel);
        }
    }
}
