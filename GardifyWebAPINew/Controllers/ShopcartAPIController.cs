using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using static GardifyModels.Models.ShopcartViewModels;

namespace GardifyWebAPI.Controllers
{
    public class ShopcartAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ShopcartController sh = new ShopcartController();

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/ShopcartAPI/shopCartEntries")]
        public ShopcartEntryAndWishListViewModel getShopEntries()
        {
            return sh.Index();
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/ShopcartAPI/shopCartCount")]
        public int getTotalShopCartEntries()
        {
            return sh.getShopcartCounter();
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/ShopcartAPI/wishlistEntriesCount")]
        public int getTotalWishlistEntries()
        {
            return sh.getWishlistEntriesCounter();
        }

        [System.Web.Http.HttpPut]
        [System.Web.Http.Route("api/ShopcartAPI/shopCartEntries/wish/{itemId}")]
        public ShopcartEntryAndWishListViewModel MoveToWishlist(int itemId)
        {
            var userId = Utilities.GetUserId();
            Guid empty = Guid.Empty;
            var art = (from s in db.ShopCartEntry
                       where !s.Deleted && s.ArticleId == itemId && s.UserId == userId && s.UserId != empty
                       select s).FirstOrDefault();
            if (art != null)
            {
                art.IsWishlisted = true;
                art.IsInCart = false;
                db.SaveChanges();
            }
            return sh.Index();
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/ShopcartAPI/addToShopCart/{articleId}/{fromWishlist}")]
        public IHttpActionResult addArticleTocart(int articleId, bool fromWishlist, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            var userId = Utilities.GetUserId();
            Guid empty = Guid.Empty;
            if (fromWishlist)
            {
                var art = (from s in db.ShopCartEntry
                           where !s.Deleted && s.ArticleId == articleId && s.UserId == userId && s.UserId != empty
                           select s).FirstOrDefault();
                if (art != null)
                {
                    if (art.IsInCart)
                    {
                        art.Quantity += 1;
                    }
                    else
                    {
                        art.Quantity = 1;
                    }
                    art.IsWishlisted = false;
                    art.IsInCart = true;
                    db.SaveChanges();
                }
                return Ok();
            }
            else
            {
                sh.addToShopcart(articleId, true);
            }

           
            
                if (isIos)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.AddToCart, userId, (int)StatisticEventTypes.ApiCallFromIos, articleId,  EventObjectType.Article);
                }
                else if (isAndroid)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.AddToCart, userId, (int)StatisticEventTypes.ApiCallFromAndroid, articleId,  EventObjectType.Article);
                }
                else if (isWebPage)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.AddToCart, userId, (int)StatisticEventTypes.ApiCallFromWebpage, articleId, EventObjectType.Article);
                }


          
            return Ok("Index");
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/ShopcartAPI/addToWishlist/{articleId}")]
        public IHttpActionResult addArticleToWishlist(int articleId)
        {
            sh.addToShopcart(articleId, false);
            return Ok("Index");
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/ShopcartAPI/changeQuantity/{articleId}/{increase}/{decrease}")]
        public ShopcartEntriesListViewModel changeQuantity(int articleId, bool increase, bool decrease)
        {
            return sh.changeQuantity(articleId, increase, decrease);
        }

        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/ShopcartAPI/deleteEntry/{articleId}")]
        public IHttpActionResult Delete(int articleId)
        {
            sh.deleteShopcartEntry(articleId);
            return Ok();
        }
    }
}
