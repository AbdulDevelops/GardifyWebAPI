using Microsoft.AspNet.Identity;
using PflanzenApp.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
{
    public class ShopcartController : _BaseController
    {
        // GET: Shopcart
        public ActionResult Index()
        {
            ShopcartViewModels.ShopcartEntriesListViewModel viewModel = new ShopcartViewModels.ShopcartEntriesListViewModel();
            Guid userId = Utilities.GetUserId();
            viewModel.EntriesList = getUserShopcartViewModels(userId);
            viewModel.ComputeTotals();

            return View(viewModel);
        }

        public ActionResult getShopcartPartialView()
        {
            Guid userId = Utilities.GetUserId();
            ShopcartViewModels.ShopcartEntriesListViewModel viewModel = new ShopcartViewModels.ShopcartEntriesListViewModel();
            viewModel.EntriesList = getUserShopcartViewModels(userId);
            return PartialView("_Shopcart", viewModel);
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
                            NormalPrice = entry.Article.Price,
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
                        entryView.ArticleView.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
                    }
                    else
                    {
                        entryView.ArticleView.ArticleImages.Add(new _HtmlImageViewModel
                        {
                            SrcAttr = Url.Content("~/Images/no-image.png"),
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
        public ActionResult addToShopcart(int articleId)
        {
            Guid userId = Utilities.GetUserId();
            string username = User.Identity.GetUserName();

            HelperClasses.DbResponse response = DbAddArticleToShopcart(articleId, userId, username, 1, true);

            ShopcartViewModels._modalArticleAddedToShopcartViewModel statusMessage = new ShopcartViewModels._modalArticleAddedToShopcartViewModel();
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
        public ActionResult changeQuantity(int articleId, int newQuantity = 1)
        {
            Guid userId = Utilities.GetUserId();
            string username = User.Identity.GetUserName();

            HelperClasses.DbResponse response = DbAddArticleToShopcart(articleId, userId, username, newQuantity);

            ShopcartViewModels.ShopcartEntriesListViewModel viewModel = new ShopcartViewModels.ShopcartEntriesListViewModel();

            viewModel.EntriesList = getUserShopcartViewModels(userId);
            viewModel.ComputeTotals();

            return PartialView("_Shopcart", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult deleteShopcartEntry(int entryId)
        {
            Guid userId = Utilities.GetUserId();
            string username = User.Identity.GetUserName();

            HelperClasses.DbResponse response = DbDeleteShopcartEntry(entryId, userId, username);

            ShopcartViewModels.ShopcartEntriesListViewModel viewModel = new ShopcartViewModels.ShopcartEntriesListViewModel();
            viewModel.EntriesList = getUserShopcartViewModels(userId);
            viewModel.ComputeTotals();

            return PartialView("_Shopcart", viewModel);
        }

        public ActionResult getShopcartCounter()
        {
            Guid userId = Utilities.GetUserId();
            string username = User.Identity.GetUserName();

            int counter = DbGetShopcartCountByUserID(userId);
            ViewData.Add("shopcartCounter", counter);
            return PartialView("_shopcartCounter");
        }

        #region DB
        [NonAction]
        public HelperClasses.DbResponse DbCreateShopcartEntry(ShopCartEntry newEntry)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            newEntry.OnCreate(newEntry.CreatedBy);
            ctx.ShopCartEntry.Add(newEntry);
            bool isOk = ctx.SaveChanges() > 0 ? true : false;

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
        public HelperClasses.DbResponse DbAddArticleToShopcart(int articleId, Guid userId, string username, int newQuantity = 1, bool addNewQuantityToExisting = false)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            HelperClasses.DbResponse existingEntryCheck = DbGetShopcartEntryByArticleId(articleId, userId);
            if (existingEntryCheck.Status == ModelEnums.ActionStatus.Success)
            {
                ShopCartEntry existingEntry = existingEntryCheck.ResponseObjects.FirstOrDefault() as ShopCartEntry;
                existingEntry.EditedBy = username;
                if (addNewQuantityToExisting)
                {
                    existingEntry.Quantity += newQuantity;
                }
                else
                {
                    existingEntry.Quantity = newQuantity;
                }

                response = DbShopcartEntryChangeQuantity(existingEntry);
            }
            else
            {
                ShopCartEntry newEntry = new ShopCartEntry
                {
                    ArticleId = articleId,
                    UserId = userId,
                    CreatedBy = username,
                    Quantity = newQuantity
                };
                response = DbCreateShopcartEntry(newEntry);
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetShopcartEntryById(int shopcartEntryId, Guid userId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var shopcart_sel = (from s in ctx.ShopCartEntry
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

            var shopcart_sel = (from s in ctx.ShopCartEntry
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

            shopcart_sel = (from s in ctx.ShopCartEntry
                            where !s.Deleted && s.UserId == userId
                            select s);

            if (shopcart_sel != null && shopcart_sel.Any())
            {
                if (appendArticle)
                {
                    foreach (ShopCartEntry entry in shopcart_sel)
                    {

                        var article_sel = (from a in ctx.Articles
                                           where entry.ArticleId == a.Id && !a.Deleted
                                           select a);
                        if (article_sel != null && article_sel.Any())
                        {
                            entry.Article = article_sel.FirstOrDefault();
                        }
                    }
                }

                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.AddRange(shopcart_sel);
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
            var shopcart_sel = (from s in ctx.ShopCartEntry
                                where !s.Deleted && s.UserId == userId
                                select s);
            if (shopcart_sel != null && shopcart_sel.Any())
            {
                return shopcart_sel.Count();
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

            var shopcart_sel = (from s in ctx.ShopCartEntry
                                where !s.Deleted && s.Id == newData.Id && s.UserId == newData.UserId
                                select s);

            if (shopcart_sel != null && shopcart_sel.Any())
            {
                shopcart_sel.FirstOrDefault().OnEdit(newData.EditedBy);
                shopcart_sel.FirstOrDefault().Quantity = newData.Quantity;

                ctx.Entry(shopcart_sel.FirstOrDefault()).State = EntityState.Modified;

                bool isOk = ctx.SaveChanges() > 0 ? true : false;

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

            var shopcart_sel = (from s in ctx.ShopCartEntry
                                where !s.Deleted && s.Id == entryId && s.UserId == userId
                                select s);

            if (shopcart_sel != null && shopcart_sel.Any())
            {
                shopcart_sel.FirstOrDefault().OnEdit(deletedBy);
                shopcart_sel.FirstOrDefault().Deleted = true;

                ctx.Entry(shopcart_sel.FirstOrDefault()).State = EntityState.Modified;

                bool isOk = ctx.SaveChanges() > 0 ? true : false;

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