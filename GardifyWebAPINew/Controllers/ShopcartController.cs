using Microsoft.AspNet.Identity;
using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using static GardifyModels.Models.ShopcartViewModels;

namespace GardifyWebAPI.Controllers
{
    public class ShopcartController : _BaseController
    {
        private readonly decimal SHIPPING_FLAT_RATE_DE = 4.95m;
        private readonly decimal SHIPPING_FLAT_RATE_AT = 9.90m;
        // GET: Shopcart
        public ShopcartEntryAndWishListViewModel Index()
        {
            ShopcartViewModels.ShopcartEntryAndWishListViewModel viewModel = new ShopcartViewModels.ShopcartEntryAndWishListViewModel();
            Guid userId = Utilities.GetUserId();
            
            if (userId == Guid.Empty)
            {
                return viewModel;
            }
            ShopcartEntriesListViewModel shopCartV = new ShopcartEntriesListViewModel()
            {
                EntriesList= getUserShopcartViewModels(userId)
            };

            SetFlatrateByUserCountry(shopCartV, userId);

            shopCartV.ComputeTotals();
            viewModel.ShopCartEntries = shopCartV;
            viewModel.WishListEntries = getUserShopcartWishListViewModels(userId);
            return viewModel;
        }

        public void SetFlatrateByUserCountry(ShopcartEntriesListViewModel cart, Guid userId)
        {
            var user = plantDB.Users.Where(u => u.Id == userId.ToString() && !u.Deleted).FirstOrDefault();
            switch (user.Country.ToLower())
            {
                case "deutschland":
                case "germany":
                case "de":
                    cart.ShippingFlatrate = SHIPPING_FLAT_RATE_DE; break;
                case "österreich":
                case "austria":
                case "at":
                    cart.ShippingFlatrate = SHIPPING_FLAT_RATE_AT; break;
                default:
                    cart.SetFlatrateByWeight(); break;
            }
        }

        public ActionResult getShopcartPartialView()
        {
            Guid userId = Utilities.GetUserId();
            ShopcartViewModels.ShopcartEntriesListViewModel viewModel = new ShopcartViewModels.ShopcartEntriesListViewModel();
            viewModel.EntriesList = getUserShopcartViewModels(userId);
            return PartialView("_Shopcart", viewModel);
        }

        public void ClearShopCart(Guid userId)
        {
            var toClear = (from s in plantDB.ShopCartEntry
                            where !s.Deleted && s.UserId == userId && s.IsInCart == true
                            select s);

            foreach (var art in toClear)
            {
                art.Deleted = true;
                art.IsInCart = false;
            }
            plantDB.SaveChanges();
        }

        private List<ShopcartViewModels.WishListEntryViewModelLite> getUserShopcartWishListViewModels(Guid userId)
        {
            List<ShopcartViewModels.WishListEntryViewModelLite> articlesInwishList = new List<ShopcartViewModels.WishListEntryViewModelLite>();
            HelperClasses.DbResponse shopResponse =DbGetArticlesInWishListByUserId(userId);
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            if (shopResponse.Status == ModelEnums.ActionStatus.Success && shopResponse.ResponseObjects != null)
            {
                foreach (ShopCartEntry entry in shopResponse.ResponseObjects)
                {
                    ShopcartViewModels.WishListEntryViewModelLite entryView = new ShopcartViewModels.WishListEntryViewModelLite
                    {
                        Id = entry.Id,
                        Quantity = entry.Quantity,
                        ArticleView = new ArticleViewModels.ArticleViewModelLite
                        {
                            Id = entry.Article.Id,
                            Description = entry.Article.Description,
                            Name = entry.Article.Name ,
                            NormalPrice = entry.Article.Price,
                            ProductLink = entry.Article.ProductLink,
                            Thumbnail=entry.Article.Thumbnail,
                            PhotoLink = entry.Article.PhotoLink
                            

                        }
                    };
                    HelperClasses.DbResponse imageResponse = rc.DbGetArticleReferencedImages(entryView.ArticleView.Id);

                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                    {
                        entryView.ArticleView.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                    }
                    else
                    {
                        entryView.ArticleView.ArticleImages.Add(new _HtmlImageViewModel
                        {
                            SrcAttr = Utilities.GetAbsolutePath("~/Images/gardify_Pflanzenbild_Platzhalter.svg"),
                            Id = 0,
                            TitleAttr = "Kein Bild vorhanden"
                        });
                    }

                    articlesInwishList.Add(entryView);
                }
            }
            return articlesInwishList;
        }
            private List<ShopcartViewModels.ShopcartEntryViewModel> getUserShopcartViewModels(Guid userId)
        {
            List<ShopcartViewModels.ShopcartEntryViewModel> entriesList = new List<ShopcartViewModels.ShopcartEntryViewModel>();
            HelperClasses.DbResponse cartResponse = DbGetShopcartEntriesByUserId(userId);
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            if (cartResponse.Status == ModelEnums.ActionStatus.Success && cartResponse.ResponseObjects != null)
            {
                foreach (ShopCartEntry entry in cartResponse.ResponseObjects)
                {
                    ShopcartViewModels.ShopcartEntryViewModel entryView = new ShopcartViewModels.ShopcartEntryViewModel
                    {
                        Id = entry.Id,
                        Quantity = entry.Quantity,
                        ArticleView = new ArticleViewModels.ArticleViewModel
                        {
                            Id = entry.Article.Id,
                            Description = entry.Article.Description,
                            Name = entry.Article.Name,
                            Thumbnail = entry.Article.Thumbnail,
                            PhotoLink = entry.Article.PhotoLink,
                            IsWishlisted = entry.IsWishlisted,
                            NormalPrice = entry.Article.Price,
                            VAT = entry.Article.VAT,
                            BulkArticle = entry.Article.BulkArticle,
                            WeightInGrams = entry.Article.WeightInGrams,
                            PricePercentagePayableWithPoints = entry.Article.PricePercentagePayableWithPoints
                        }
                    };

                    if (entryView.ArticleView.PricePercentagePayableWithPoints > 0)
                    {
                        decimal pointValueInEuro = -1;
                        decimal.TryParse(WebConfigurationManager.AppSettings["pointValueInEuro"], out pointValueInEuro);

                        if (pointValueInEuro > 0)
                        {

                            decimal euroToCoverWithPoints = (entryView.ArticleView.NormalPrice / 100) * entryView.ArticleView.PricePercentagePayableWithPoints;

                            if (euroToCoverWithPoints < pointValueInEuro)
                            {
                                euroToCoverWithPoints = pointValueInEuro;
                            }

                            int pointsToSpend = (int)Math.Ceiling(euroToCoverWithPoints / pointValueInEuro);
                            // recalculate price to cover with points
                            euroToCoverWithPoints = pointsToSpend * pointValueInEuro;

                            // we can pay with points only
                            if (euroToCoverWithPoints >= entryView.ArticleView.NormalPrice)
                            {
                                entryView.ArticleView.PricePartAfterPoints = 0;
                                entryView.ArticleView.PointsValue = entryView.ArticleView.NormalPrice;
                            }
                            else
                            {
                                entryView.ArticleView.PricePartAfterPoints = entryView.ArticleView.NormalPrice - euroToCoverWithPoints;
                                entryView.ArticleView.PointsValue = euroToCoverWithPoints;
                            }
                            entryView.ArticleView.PointsToSpend = pointsToSpend;
                        }
                    }

                    HelperClasses.DbResponse imageResponse = rc.DbGetArticleReferencedImages(entryView.ArticleView.Id);

                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                    {
                        entryView.ArticleView.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                    }
                    else
                    {
                        entryView.ArticleView.ArticleImages.Add(new _HtmlImageViewModel
                        {
                            SrcAttr = Utilities.GetAbsolutePath("~/Images/gardify_Pflanzenbild_Platzhalter.svg"),
                            Id = 0,
                            TitleAttr = "Kein Bild vorhanden"
                        });
                    }

                    entriesList.Add(entryView);
                }
            }
            return entriesList;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult addToShopcart(int articleId, bool addToCart = false)
        {
            Guid userId = Utilities.GetUserId();
            string username = Utilities.GetUserName();
            ShopcartViewModels._modalArticleAddedToShopcartViewModel statusMessage = new ShopcartViewModels._modalArticleAddedToShopcartViewModel();

            if (userId == Guid.Empty)
            {
                return PartialView("_modalArticleAddedToShopcart", statusMessage);
            }
            HelperClasses.DbResponse response;
            if (addToCart)
            {
                 response = DbAddArticleToShopcart(articleId, userId, username, 1, true, false, addToCart);

            }
            else
            {
                 response = DbAddArticleToWishList(articleId, userId, username, 1);

            }

            statusMessage.Status = response.Status;

            if (response.Status == ModelEnums.ActionStatus.Success)
            {
                statusMessage.Message = "Artikel wurde erfolgreich dem Warenkorb hinzugefügt";

            }
            else
            {
                statusMessage.Message = String.Join(",", response.Messages);
            }
            return PartialView("_modalArticleAddedToShopcart", statusMessage);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ShopcartEntriesListViewModel changeQuantity(int articleId, bool increase, bool decrease,int newQuantity = 1)
        {
            Guid userId = Utilities.GetUserId();
            string username = Utilities.GetUserName();

            if (userId != Guid.Empty)
            {
                if (increase)
                {
                    HelperClasses.DbResponse response = DbAddArticleToShopcart(articleId, userId, username, newQuantity, true, false);
                }
                else if (decrease)
                {
                    HelperClasses.DbResponse response = DbAddArticleToShopcart(articleId, userId, username, newQuantity, false, true);
                }
            }

            ShopcartViewModels.ShopcartEntriesListViewModel viewModel = new ShopcartViewModels.ShopcartEntriesListViewModel();

            viewModel.EntriesList = getUserShopcartViewModels(userId);
            SetFlatrateByUserCountry(viewModel, userId);
            viewModel.ComputeTotals();

            return viewModel;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult deleteShopcartEntry(int entryId)
        {
            Guid userId = Utilities.GetUserId();
            string username = Utilities.GetUserName();

            if (userId != Guid.Empty)
            {
                HelperClasses.DbResponse response = DbDeleteShopcartEntry(entryId, userId, username);
            }

            ShopcartViewModels.ShopcartEntriesListViewModel viewModel = new ShopcartViewModels.ShopcartEntriesListViewModel();
            viewModel.EntriesList = getUserShopcartViewModels(userId);
            viewModel.ComputeTotals();

            return PartialView("_Shopcart", viewModel);
        }

        public int getShopcartCounter()
        {
            Guid userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return 0;
            }
            int counter = DbGetShopcartCountByUserID(userId);
            return counter;
        }
        public int getWishlistEntriesCounter()
        {
            Guid userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return 0;
            }
            int counter = DbGetCountArtickelnFromWishlistByUserID(userId);
            return counter;
        }
        #region DB
        [NonAction]


        public HelperClasses.DbResponse DbCreateShopcartEntry(ShopCartEntry newEntry)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            newEntry.OnCreate(newEntry.CreatedBy);
            plantDB.ShopCartEntry.Add(newEntry);
            bool isOk = plantDB.SaveChanges() > 0 ? true : false;

            if (isOk)
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Created);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(newEntry);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }
        [NonAction]
        public HelperClasses.DbResponse DbAddArticleToWishList(int articleId, Guid userId, string username, int newQuantity = 1)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            HelperClasses.DbResponse existingEntryCheck = DbGetShopcartEntryByArticleId(articleId, userId);
            if ((existingEntryCheck.Status == ModelEnums.ActionStatus.Success) )
            {
                ShopCartEntry existingEntry = existingEntryCheck.ResponseObjects.FirstOrDefault() as ShopCartEntry;
                if (!existingEntry.IsWishlisted && existingEntry.IsInCart)
                {
                    existingEntry.IsInCart = false;
                    existingEntry.Quantity = 1;
                    existingEntry.IsWishlisted = true;
                    response = DbShopcartEntryChangeQuantity(existingEntry);
                }
                    
            }
            else
            {
                Article article = plantDB.Articles.Where(a => a.Id == articleId && !a.Deleted).FirstOrDefault();
                var shopcart_sel = (from s in plantDB.ShopCartEntry
                                    where !s.Deleted && s.ArticleId == articleId && s.UserId == userId && !s.IsInCart
                                    select s).FirstOrDefault();
                if (shopcart_sel == null)
                {
                    ShopCartEntry newEntry = new ShopCartEntry
                    {
                        ArticleId = articleId,
                        UserId = userId,
                        CreatedBy = username,
                        Quantity = newQuantity,
                        IsWishlisted = true,
                        IsPreorder = article == null ? false : article.IsNotDeliverable
                    };
                    response = DbCreateShopcartEntry(newEntry);
                }
                   
            }
                return response;
        }
        public HelperClasses.DbResponse DbAddArticleToShopcart(int articleId, Guid userId, string username, int newQuantity = 1, bool addNewQuantityToExisting = false, bool removeNewQuantityToExisting = false, bool isIncart = false)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            HelperClasses.DbResponse existingEntryCheck = DbGetShopcartEntryByArticleId(articleId, userId);
            if ((existingEntryCheck.Status == ModelEnums.ActionStatus.Success))
            {
                ShopCartEntry existingEntry = existingEntryCheck.ResponseObjects.FirstOrDefault() as ShopCartEntry;
               
                    existingEntry.EditedBy = username;
                    existingEntry.IsInCart = true;
                   
                    if (addNewQuantityToExisting)
                    {
                        existingEntry.Quantity += newQuantity;
                    }
                    else if (removeNewQuantityToExisting)
                    {
                        existingEntry.Quantity -= newQuantity;
                    }
                    else
                    {
                        existingEntry.Quantity = newQuantity;
                    }
                    if (existingEntry.IsWishlisted)
                    {
                        existingEntry.IsWishlisted = false;
                        existingEntry.Quantity = newQuantity;
                }
                response = DbShopcartEntryChangeQuantity(existingEntry);
               
               
            }
            else
            {
                Article article = plantDB.Articles.Where(a => a.Id == articleId && !a.Deleted).FirstOrDefault();
                ShopCartEntry newEntry = new ShopCartEntry
                {
                    ArticleId = articleId,
                    UserId = userId,
                    CreatedBy = username,
                    Quantity = newQuantity,
                    IsInCart = true,
                    IsPreorder = article == null ? false : article.IsNotDeliverable
                };
              
                response = DbCreateShopcartEntry(newEntry);
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetShopcartEntryById(int shopcartEntryId, Guid userId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var shopcart_sel = (from s in plantDB.ShopCartEntry
                                where !s.Deleted && s.Id == shopcartEntryId && s.UserId == userId
                                select s);

            if (shopcart_sel != null && shopcart_sel.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(shopcart_sel.FirstOrDefault());
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetShopcartEntryByArticleId(int articleId, Guid userId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var shopcart_sel = (from s in plantDB.ShopCartEntry
                                where !s.Deleted && s.ArticleId == articleId && s.UserId == userId
                                select s);

            if (shopcart_sel != null && shopcart_sel.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(shopcart_sel.FirstOrDefault());
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetShopcartEntriesByUserId(Guid userId, bool appendArticle = true)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            IEnumerable<ShopCartEntry> shopcart_sel = null;

            shopcart_sel = (from s in plantDB.ShopCartEntry
                            where !s.Deleted && s.UserId == userId && s.IsInCart == true
                            select s);
            List<ShopCartEntry> shopCartEntries = new List<ShopCartEntry>();
            if (shopcart_sel != null && shopcart_sel.Any())
            {
                if (appendArticle)
                {
                    foreach (ShopCartEntry entry in shopcart_sel)
                    {

                        var article_sel = (from a in plantDB.Articles
                                           where entry.ArticleId == a.Id && !a.Deleted
                                           select a);
                        if (article_sel != null && article_sel.Any())
                        {
                            entry.Article = article_sel.FirstOrDefault();
                            shopCartEntries.Add(entry);
                        }
                    }
                }

                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.AddRange( shopCartEntries);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }
        [NonAction]
        public HelperClasses.DbResponse DbGetArticlesInWishListByUserId(Guid userId, bool appendArticle = true)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            IEnumerable<ShopCartEntry> WishList_sel = null;

            WishList_sel = (from s in plantDB.ShopCartEntry
                            where !s.Deleted && s.UserId == userId && s.IsWishlisted == true
                            select s);
            List<ShopCartEntry> articlesInWishList = new List<ShopCartEntry>();
            if (WishList_sel != null && WishList_sel.Any())
            {
                if (appendArticle)
                {
                    foreach (ShopCartEntry entry in WishList_sel)
                    {

                        var article_sel = (from a in plantDB.Articles
                                           where entry.ArticleId == a.Id && !a.Deleted
                                           select a);
                        if (article_sel != null && article_sel.Any())
                        {
                            entry.Article = article_sel.FirstOrDefault();
                            articlesInWishList.Add(entry);
                        }
                    }
                }

                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.AddRange(articlesInWishList);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }
        [NonAction]
        public int DbGetShopcartCountByUserID(Guid userId)
        {
            var shopcart_sel = (from s in plantDB.ShopCartEntry
                                where !s.Deleted && s.UserId == userId && !s.IsWishlisted && s.IsInCart == true
                                select s);
            if (shopcart_sel != null && shopcart_sel.Any())
            {
                int totalNumberOfArt = 0;
                foreach(var a in shopcart_sel)
                {
                    totalNumberOfArt += a.Quantity;
                }
                return totalNumberOfArt;
            }
            return 0;
        }
        public int DbGetCountArtickelnFromWishlistByUserID(Guid userId)
        {
            var wishlist_sel = (from s in plantDB.ShopCartEntry
                                where !s.Deleted && s.UserId == userId && s.IsWishlisted == true
                                select s);
            if (wishlist_sel != null && wishlist_sel.Any())
            {
                int totalNumberOfArt = wishlist_sel.Count();
              
                return totalNumberOfArt;
            }
            return 0;
        }
        [NonAction]
        public HelperClasses.DbResponse DbShopcartEntryChangeQuantity(ShopCartEntry newData)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            if (newData.Quantity <= 0)
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.WrongQuantity);
                response.Status = ModelEnums.ActionStatus.Error;
                return response;
            }

            var shopcart_sel = (from s in plantDB.ShopCartEntry
                                where !s.Deleted && s.Id == newData.Id && s.UserId == newData.UserId
                                select s);

            if (shopcart_sel != null && shopcart_sel.Any())
            {
                shopcart_sel.FirstOrDefault().OnEdit(newData.EditedBy);
                shopcart_sel.FirstOrDefault().Quantity = newData.Quantity;
                plantDB.Entry(shopcart_sel.FirstOrDefault()).State = EntityState.Modified;

                bool isOk = plantDB.SaveChanges() > 0 ? true : false;

                if (isOk)
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.Edited);
                    response.Status = ModelEnums.ActionStatus.Success;
                    response.ResponseObjects.Add(shopcart_sel.FirstOrDefault());
                }
                else
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                    response.Status = ModelEnums.ActionStatus.Error;
                }
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbDeleteShopcartEntry(int entryId, Guid userId, string deletedBy)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var shopcart_sel = (from s in plantDB.ShopCartEntry
                                where !s.Deleted && s.ArticleId == entryId && s.UserId == userId
                                select s);

            if (shopcart_sel != null && shopcart_sel.Any())
            {
                shopcart_sel.FirstOrDefault().OnEdit(deletedBy);
                shopcart_sel.FirstOrDefault().Deleted = true;

                plantDB.Entry(shopcart_sel.FirstOrDefault()).State = EntityState.Modified;

                bool isOk = plantDB.SaveChanges() > 0 ? true : false;

                if (isOk)
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.Deleted);
                    response.Status = ModelEnums.ActionStatus.Success;
                    response.ResponseObjects.Add(shopcart_sel.FirstOrDefault());
                }
                else
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                    response.Status = ModelEnums.ActionStatus.Error;
                }
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }


        #endregion
    }
}