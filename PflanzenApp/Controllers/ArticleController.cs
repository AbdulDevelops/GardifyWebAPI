using PflanzenApp.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Configuration;
using System.Web.Mvc;
using static GardifyModels.Models.ModelEnums;
using static GardifyModels.Models.ArticleViewModels;

namespace PflanzenApp.Controllers
{
    [CustomAuthorize]
    public class ArticleController : _BaseController
    {
        public ActionResult Index()
        {
            ArticleViewModels.ArticleListViewModel viewModel = new ArticleViewModels.ArticleListViewModel();
            viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;

            HelperClasses.DbResponse response = DbGetAllArticles();

            List<ArticleViewModels.ArticleViewModel> articleModels = new List<ArticleViewModels.ArticleViewModel>();



            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                foreach (Article art in response.ResponseObjects)
                {

                    ArticleViewModels.ArticleViewModel entryView = new ArticleViewModels.ArticleViewModel()
                    {
                        Id = art.Id,
                        Name = art.Name,
                        Description = art.Description,
                        NormalPrice = art.Price,
                    };

                    if (art.PricePercentagePayableWithPoints > 0)
                    {
                        decimal pointValueInEuro = -1;
                        decimal.TryParse(WebConfigurationManager.AppSettings["pointValueInEuro"], out pointValueInEuro);

                        if (pointValueInEuro > 0)
                        {

                            decimal euroToCoverWithPoints = (art.Price / 100) * art.PricePercentagePayableWithPoints;

                            if (euroToCoverWithPoints < pointValueInEuro)
                            {
                                euroToCoverWithPoints = pointValueInEuro;
                            }

                            int pointsToSpend = (int)Math.Ceiling(euroToCoverWithPoints / pointValueInEuro);
                            // recalculate price to cover with points
                            euroToCoverWithPoints = pointsToSpend * pointValueInEuro;

                            // we can pay with points only
                            if (euroToCoverWithPoints >= art.Price)
                            {
                                entryView.PricePartAfterPoints = 0;
                                entryView.PointsValue = art.Price;
                            }
                            else
                            {
                                entryView.PricePartAfterPoints = art.Price - euroToCoverWithPoints;
                                entryView.PointsValue = euroToCoverWithPoints;
                            }
                            entryView.PointsToSpend = pointsToSpend;
                        }
                    }

                    ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
                    HelperClasses.DbResponse imageResponse = rc.DbGetArticleReferencedImages(art.Id);

                    entryView.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));

                    articleModels.Add(entryView);
                }
            }

            viewModel.ListEntries = articleModels;

            return View(viewModel);
        }


        #region DB
        [NonAction]
        public HelperClasses.DbResponse DbCreateArticle(Article articleData)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            articleData.OnCreate(articleData.CreatedBy);

            ctx.Articles.Add(articleData);

            bool isOk = ctx.SaveChanges() > 0 ? true : false;

            if (isOk)
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Created);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(articleData);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetArticleCategoryById(int catId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var article_sel = (from a in ctx.ArticleCategories
                               where !a.Deleted && a.Id == catId
                               select a);

            if (article_sel != null && article_sel.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(article_sel.FirstOrDefault());
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }

        [NonAction]
        public ArticleViewModel GetArticleViewModel(int articleId)
        {
            var res = new ArticleViewModel();
            var art = ctx.Articles.Where(a => a.Id == articleId && !a.Deleted).FirstOrDefault();
            if (art != null)
            {
                res.Name = art.Name;
                res.NormalPrice = art.Price;
            }
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            HelperClasses.DbResponse imageResponse = rc.DbGetArticleReferencedImages(articleId);

            if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            {
                res.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
            }
            else
            {
                res.ArticleImages.Add(new _HtmlImageViewModel
                {
                    SrcAttr = Utilities.GetAbsolutePath("~/Images/gardify_Pflanzenbild_Platzhalter.svg"),
                    Id = 0,
                    TitleAttr = "Kein Bild vorhanden"
                });
            }
            return res;
        }

        [NonAction]
        public ICollection<ArticleCategory> DbGetArticleCategoriesBySelection(int[] ids)
        {
            var list = new HashSet<ArticleCategory>();
            if (ids != null && ids.Any())
            {
                foreach (int id in ids)
                {
                    var cat = DbGetArticleCategoryById(id);
                    if (cat.Status == ActionStatus.Success)
                    {
                        ArticleCategory temp = (ArticleCategory)cat.ResponseObjects.FirstOrDefault();
                        list.Add(temp);
                    }
                }
            }
            
            return list;
        }

        [NonAction]
        public IEnumerable<SelectListItem> DbGetArticleCategoriesSelectionByArticleId(int articleId)
        {
            var article = ctx.Articles.Where(g => !g.Deleted && g.Id == articleId).FirstOrDefault();
            var cats = ctx.ArticleCategories.Where(g => !g.Deleted);
            List<SelectListItem> list = new List<SelectListItem>();

            foreach (var c in cats)
            {
                if (article.ArticleCategories.Contains(c))
                {
                    list.Add(new SelectListItem
                    {
                        Text = c.Title,
                        Value = c.Id.ToString(),
                        Selected = true
                    });
                }
                else
                {
                    list.Add(new SelectListItem
                    {
                        Text = c.Title,
                        Value = c.Id.ToString(),
                        Selected = false
                    });
                }
            }

            return list;
        }

        [NonAction]
        public IEnumerable<SelectListItem> DbGetArticleCategoriesSelectList(int? id = -1)
        {
            var cats = ctx.ArticleCategories.Where(g => !g.Deleted);
            List<SelectListItem> list = new List<SelectListItem>();
            list.Add(new SelectListItem
            {
                Text = "Keine Kategorie",
                Value = "-1"
            });
            foreach (var c in cats)
            {
                if (c.Id == id)
                {
                    list.Add(new SelectListItem
                    {
                        Text = c.Title,
                        Value = c.Id.ToString(),
                        Selected = true
                    });
                }
                else
                {
                    list.Add(new SelectListItem
                    {
                        Text = c.Title,
                        Value = c.Id.ToString()
                    });
                }
            }
            return list;
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetArticleById(int articleId, bool availableOnly = true)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var article_sel = (from a in ctx.Articles
                               where !a.Deleted && a.Id == articleId && (availableOnly ? a.IsAvailable == true : true)
                               select a);

            if (article_sel != null && article_sel.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(article_sel.FirstOrDefault());
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetAllArticles(bool availableOnly = true,int categoryId=-1)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            IEnumerable<Article> article_sel = null;
            if (categoryId > 0)
            {
                 var cat_sel = (from a in ctx.ArticleCategories
                                where !a.Deleted && a.Id==categoryId
                                   select a).FirstOrDefault();
                article_sel = cat_sel.ArticlesInthisCategory;
            }
            else
            {
                article_sel = (from a in ctx.Articles
                               where !a.Deleted && (availableOnly ? a.IsAvailable == true : true)
                               select a).OrderBy(a => a.Sort);
            }
            if (article_sel != null && article_sel.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.AddRange(article_sel);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbEditArticle(Article newData)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var article_sel = (from a in ctx.Articles
                               where !a.Deleted && a.Id == newData.Id
                               select a);
            var artCats = (from a in ctx.ArticleCategories
                           where !a.Deleted
                           select a);

            if (article_sel != null && article_sel.Any())
            {
                // clear old categories
                foreach (ArticleCategory arc in artCats)
                {
                    article_sel.FirstOrDefault().ArticleCategories.Remove(arc);
                }
                // add new
                foreach (ArticleCategory arc in newData.ArticleCategories)
                {
                    article_sel.FirstOrDefault().ArticleCategories.Add(arc);
                }
                article_sel.FirstOrDefault().Name = newData.Name;
                article_sel.FirstOrDefault().Description = newData.Description;
                article_sel.FirstOrDefault().ExpertTip = newData.ExpertTip;
                article_sel.FirstOrDefault().Price = newData.Price;
                article_sel.FirstOrDefault().ProductLink = newData.ProductLink;
                article_sel.FirstOrDefault().IsAvailable = newData.IsAvailable;
                article_sel.FirstOrDefault().AllowPublishment = newData.AllowPublishment;
                article_sel.FirstOrDefault().PricePercentagePayableWithPoints = newData.PricePercentagePayableWithPoints;
                article_sel.FirstOrDefault().IsNotDeliverable = newData.IsNotDeliverable;
                article_sel.FirstOrDefault().Brand = newData.Brand;
                article_sel.FirstOrDefault().VAT = newData.VAT;
                article_sel.FirstOrDefault().Author = newData.Author;
                article_sel.FirstOrDefault().EANCode = newData.EANCode;
                article_sel.FirstOrDefault().HazardNotice = newData.HazardNotice;
                article_sel.FirstOrDefault().BulkArticle = newData.BulkArticle;
                article_sel.FirstOrDefault().WeightInGrams = newData.WeightInGrams;
                article_sel.FirstOrDefault().MakerId = newData.MakerId;
                article_sel.FirstOrDefault().Thumbnail = newData.PhotoLink;
                article_sel.FirstOrDefault().PhotoLink = newData.PhotoLink;
                article_sel.FirstOrDefault().OnEdit(newData.EditedBy);

                ctx.Entry(article_sel.FirstOrDefault()).State = EntityState.Modified;

                bool isOk = ctx.SaveChanges() > 0 ? true : false;

                if (isOk)
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.Edited);
                    response.Status = ModelEnums.ActionStatus.Success;
                    response.ResponseObjects.Add(article_sel.FirstOrDefault());
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
        public HelperClasses.DbResponse DbEditArticleAllowPublishment(int articleId, string editedBy, bool publish)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            var article_sel = (from a in ctx.Articles
                               where !a.Deleted && a.Id == articleId
                               select a);
            if (article_sel != null && article_sel.Any())
            {
                article_sel.FirstOrDefault().AllowPublishment = publish;
                article_sel.FirstOrDefault().OnEdit(editedBy);

                ctx.Entry(article_sel.FirstOrDefault()).State = EntityState.Modified;

                bool isOk = ctx.SaveChanges() > 0 ? true : false;

                if (isOk)
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.Deleted);
                    response.Status = ModelEnums.ActionStatus.Success;
                    response.ResponseObjects.Add(article_sel.FirstOrDefault());
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
        public HelperClasses.DbResponse DbDeleteArticle(int articleId, string deletedBy)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var article_sel = (from a in ctx.Articles
                               where !a.Deleted && a.Id == articleId
                               select a);

            if (article_sel != null && article_sel.Any())
            {
                article_sel.FirstOrDefault().Deleted = true;
                article_sel.FirstOrDefault().OnEdit(deletedBy);

                ctx.Entry(article_sel.FirstOrDefault()).State = EntityState.Modified;

                bool isOk = ctx.SaveChanges() > 0 ? true : false;

                if (isOk)
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.Deleted);
                    response.Status = ModelEnums.ActionStatus.Success;
                    response.ResponseObjects.Add(article_sel.FirstOrDefault());
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
        public IEnumerable<ArticleViewModelLite> GetObjectArticles(ArticleReferenceType type, int referenceId)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            IEnumerable<ArticleReference> articleRefs = null;
            List<ArticleViewModelLite> articlesList = new List<ArticleViewModelLite>();
            if (type == ArticleReferenceType.Plant)
            {
                articleRefs = ctx.ArticleReference.Where(v => v.ReferenceType == type && v.PlantId == referenceId);
            }
            else if (type == ArticleReferenceType.Todotemplate)
            {
                articleRefs = ctx.ArticleReference.Where(v => v.ReferenceType == type && v.TodoTemplateId == referenceId);
            }
            if (articleRefs != null && articleRefs.Any())
            {
                var articles = articleRefs.Select(v => v.Article);
                foreach (var a in articles)
                {
                    ArticleViewModelLite artVm = new ArticleViewModelLite()
                    {
                        Name = a.Name,
                        Description = a.Description,
                        Id = a.Id
                    };
                    HelperClasses.DbResponse imageResponse = rc.DbGetArticleReferencedImages(a.Id);
                    artVm.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                    articlesList.Add(artVm);
                }
            }

            return articlesList;
        }

        #endregion
    }
}