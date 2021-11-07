using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using static GardifyModels.Models.ArticleViewModels;
using static GardifyModels.Models.HelperClasses;
using static GardifyModels.Models.ModelEnums;
using MoreLinq;

namespace GardifyWebAPI.Controllers
{
    public class ArticleController : _BaseController
    {
        public ArticleViewModels.ArticleListViewModel Index(int skip = 0, int take = 8)
        {
            ArticleViewModels.ArticleListViewModel viewModel = new ArticleViewModels.ArticleListViewModel();
            viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;

            HelperClasses.DbResponse response = DbGetAllArticles(false, skip, take);

            List<ArticleViewModels.ArticleViewModel> articleModels = new List<ArticleViewModels.ArticleViewModel>();



            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                foreach (Article art in response.ResponseObjects)
                {

                    ArticleViewModels.ArticleViewModel entryView = new ArticleViewModels.ArticleViewModel()
                    {
                        Id = art.Id,
                        Name = art.Name,
                        Label = art.Label,
                        PhotoLink = art.PhotoLink,
                        ProductLink = art.ProductLink,
                        Thumbnail = art.Thumbnail,
                        IsAvailable = art.IsAvailable,
                        ArticleNr = art.ArticleNr,
                        IsNotDeliverable = art.IsNotDeliverable,
                        Description = art.Description,
                        NormalPrice = art.Price,
                        AllowPublishment=art.AllowPublishment
                    };
                    var artInShopCartEntries = art.ShopCartEntries.Where(s => s.ArticleId == art.Id && !s.Deleted && s.UserId == Utilities.GetUserId()).FirstOrDefault();
                    entryView.IsWishlisted = artInShopCartEntries != null ? artInShopCartEntries.IsWishlisted : false;
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

                    entryView.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));

                    articleModels.Add(entryView);
                }
            }

            viewModel.ListEntries = articleModels;

            return viewModel;
        }

        public ArticleViewModels.ArticleListViewModel GetAllArticles()
        {
            ArticleViewModels.ArticleListViewModel viewModel = new ArticleViewModels.ArticleListViewModel();
            viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;

            HelperClasses.DbResponse response = DbGetAllArticles(false, 0, int.MaxValue);

            List<ArticleViewModels.ArticleViewModel> articleModels = new List<ArticleViewModels.ArticleViewModel>();



            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                foreach (Article art in response.ResponseObjects)
                {

                    ArticleViewModels.ArticleViewModel entryView = new ArticleViewModels.ArticleViewModel()
                    {
                        Id = art.Id,
                        Name = art.Name
                    };
               

                    articleModels.Add(entryView);
                }
            }

            viewModel.ListEntries = articleModels;

            return viewModel;
        }
        public ArticleViewModel GetArticleViewModel(int articleId)
        {
            var res = new ArticleViewModel();
            var art = plantDB.Articles.Where(a => a.Id == articleId && !a.Deleted).FirstOrDefault();
            if (art != null)
            {
                res.Name = art.Name;
                res.NormalPrice = art.Price;
            }
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            HelperClasses.DbResponse imageResponse = rc.DbGetArticleReferencedImages(articleId);

            if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            {
                res.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
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

        public ArticleViewModel GetArticleVMByCartEntry(int entryId, Guid userId)
        {
            var res = new ArticleViewModel();
            var entry = plantDB.ShopCartEntry.Where(e => e.Id == entryId && e.UserId == userId).FirstOrDefault();
            if (entry == null)
            {
                return res;
            }
            var art = plantDB.Articles.Where(a => a.Id == entry.ArticleId && !a.Deleted).FirstOrDefault();
            if (art != null)
            {
                res.Id = art.Id;
                res.Name = art.Name;
                res.NormalPrice = art.Price;
                res.EANCode = art.EANCode;
                res.MakerId = art.MakerId;
            }
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            HelperClasses.DbResponse imageResponse = rc.DbGetArticleReferencedImages(entry.ArticleId);

            if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            {
                res.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
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

        public ArticleViewModels.ArticleListViewModel sortArticlesByPrice(string sort,int skip,int take=8)
        {
            ArticleViewModels.ArticleListViewModel viewModel = new ArticleViewModels.ArticleListViewModel();
            viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;

            HelperClasses.DbResponse response = DbSortArticleByPrice(sort);

            List<ArticleViewModels.ArticleViewModel> sortedArticleModels = new List<ArticleViewModels.ArticleViewModel>();



            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                foreach (Article art in response.ResponseObjects.Skip(skip).Take(take))
                {

                    ArticleViewModels.ArticleViewModel entryView = new ArticleViewModels.ArticleViewModel()
                    {
                        Id = art.Id,
                        Name = art.Name,
                        Description = art.Description,
                        NormalPrice = art.Price,
                        Thumbnail = art.Thumbnail,
                        PhotoLink = art.PhotoLink,
                        ProductLink = art.ProductLink,
                        IsNotDeliverable = art.IsNotDeliverable,
                        Label = art.Label
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

                    entryView.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));

                    sortedArticleModels.Add(entryView);
                }
            }

            viewModel.ListEntries = sortedArticleModels;

            return viewModel;
        }
        public IEnumerable<ArticleViewModels.ArticleViewModel> getXLastViewedArt()
        {
            Guid userId = Utilities.GetUserId();
            List<ArticleViewModels.ArticleViewModel> lastWList = new List<ArticleViewModels.ArticleViewModel>();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            HelperClasses.DbResponse lastViewedArtList = DbGetXLastviewedArticles(userId);
            if (lastViewedArtList.ResponseObjects != null && lastViewedArtList.ResponseObjects.Any())
            {
                foreach (LastViewedArticle l in lastViewedArtList.ResponseObjects)
                {
                    ArticleViewModels.ArticleViewModel vm = new ArticleViewModels.ArticleViewModel()
                    {
                        Name = l.Article.Name,
                        Id = l.ArticleId,
                        PhotoLink = l.Article.PhotoLink,
                        Thumbnail = l.Article.Thumbnail,
                        NormalPrice = l.Article.Price,
                    };
                    HelperClasses.DbResponse imageResponse = rc.DbGetArticleReferencedImages(l.ArticleId);

                    vm.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                    lastWList.Add(vm);
                }

            }
            return lastWList;
        }
        
        public ArticleViewModel GetArticleById(int Id)
        {
            ArticleViewModel viewModel = new ArticleViewModel();


            HelperClasses.DbResponse response = DbGetArticleById(Id);


            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                //if (response.ResponseObjects[0].GetType() == new Article().GetType())
                //{

                //}
                Article art = (Article)response.ResponseObjects[0];

                ArticleViewModel entryView = new ArticleViewModel()
                {
                    Id = art.Id,
                    Name = art.Name,
                    Label = art.Label,
                    PhotoLink = art.PhotoLink,
                    ProductLink = art.ProductLink,
                    Thumbnail = art.Thumbnail,
                    IsAvailable = art.IsAvailable,
                    ArticleNr = art.ArticleNr,
                    Description = art.Description,
                    ExpertTip = art.ExpertTip,
                    NormalPrice = art.Price,
                    IsNotDeliverable = art.IsNotDeliverable,
                    Brand = art.Brand,
                    VAT = art.VAT,
                    Author = art.Author,
                    EANCode = art.EANCode,
                    HazardNotice = art.HazardNotice,
                    WeightInGrams = art.WeightInGrams,
                    BulkArticle = art.BulkArticle,
                    MakerId = art.MakerId,
                    Categories = art.ArticleCategories.Select(c => c.Id).ToList()
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

                entryView.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                viewModel = entryView;
                return viewModel;
            }
            return null;

        }

        public ArticleViewModel GetArticleByIdLite(int Id)
        {
            ArticleViewModel viewModel = new ArticleViewModel();


            HelperClasses.DbResponse response = DbGetArticleById(Id);


            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                //if (response.ResponseObjects[0].GetType() == new Article().GetType())
                //{

                //}
                Article art = (Article)response.ResponseObjects[0];

                ArticleViewModel entryView = new ArticleViewModel()
                {
                    Id = art.Id,
                    Name = art.Name,
                    Label = art.Label
                };
                ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
                HelperClasses.DbResponse imageResponse = rc.DbGetArticleReferencedImages(art.Id);

                entryView.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                viewModel = entryView;
                return viewModel;
            }
            return null;

        }

        public ArticleViewModels.ArticleListViewModel GetArticleBySearchText(string searchText,int skip=0,int take=int.MaxValue)
        {
            ArticleViewModels.ArticleListViewModel viewModel = new ArticleViewModels.ArticleListViewModel();
            viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;

            HelperClasses.DbResponse response = DbGetArticleBySearchText(searchText, skip, take);

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
                        Thumbnail=art.Thumbnail,
                        PhotoLink=art.PhotoLink,
                        IsNotDeliverable = art.IsNotDeliverable,
                        Label =art.Label
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

                    entryView.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));

                    articleModels.Add(entryView);
                }
            }

            viewModel.ListEntries = articleModels;

            return viewModel;
        }


        #region DB

        [NonAction]
        public HelperClasses.DbResponse DbCreateArticle(Article articleData)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            articleData.OnCreate(articleData.CreatedBy);

            plantDB.Articles.Add(articleData);


            bool isOk = plantDB.SaveChanges() > 0 ? true : false;

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
        public HelperClasses.DbResponse DbCreateLastViewedArticle(int articleId, int previousId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            LastViewedArticle lV = new LastViewedArticle()
            {
                PreviousId = previousId,
                ArticleId = articleId,
                UserId = Utilities.GetUserId()
            };
            lV.OnCreate(Utilities.GetUserName());
            plantDB.LastViewedArticles.Add(lV);
            bool isOk = plantDB.SaveChanges() > 0 ? true : false;
            if (isOk)
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Created);
            }
            return response;
        }
        [NonAction]
        public HelperClasses.DbResponse DbGetArticleById(int articleId, bool availableOnly = true)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            //richtige lösung
            /*var article_sel = (from a in plantDB.Articles
                               where !a.Deleted && a.Id == articleId && (availableOnly ? a.IsAvailable == true : true)
                               select a);*/

            //NUR FÜR Test
            var article_sel = (from a in plantDB.Articles
                               where !a.Deleted && a.Id == articleId && a.AllowPublishment
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
        public HelperClasses.DbResponse DbGetByOtherUsersViewedArticles(Guid userId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            IQueryable<LastViewedArticle> byOtherUserViewedArtList = null;
            var byCurrentUserviewedArticle = (from art in plantDB.LastViewedArticles.OrderByDescending(o => o.Id)
                                     where (art.UserId == userId && art.Deleted == false)
                                     select art).First();
            DateTime date;
            int articleId;
            
            if (byCurrentUserviewedArticle != null)
            {
                date = byCurrentUserviewedArticle.CreatedDate;
                articleId = byCurrentUserviewedArticle.ArticleId;
                var byOtherUsersViewedArt = (from lArt in plantDB.LastViewedArticles
                                              where (lArt.CreatedDate < date && lArt.ArticleId == articleId && userId!=lArt.UserId && lArt.Deleted == false)
                                              select lArt);
                if(byOtherUsersViewedArt!=null && byOtherUsersViewedArt.Any())
                {
                     byOtherUserViewedArtList = (from l in plantDB.LastViewedArticles
                                                      where (userId != l.UserId && l.ArticleId != byCurrentUserviewedArticle.ArticleId && l.Deleted==false)
                                                      select l).Distinct();
                }
            }
            if (byOtherUserViewedArtList != null && byOtherUserViewedArtList.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.ResponseObjects.AddRange(byOtherUserViewedArtList);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
            }
            return response;
        }
        [NonAction]
        public HelperClasses.DbResponse DbGetXLastviewedArticles(Guid userId, int count=4)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            var lastViewedArtList = (from art in plantDB.LastViewedArticles
                                     where (art.UserId == userId && !art.Deleted && art.Article.AllowPublishment && !art.Article.Deleted)
                                     select art)
                                    .OrderByDescending(o => o.Id)
                                    .DistinctBy(o => o.ArticleId)
                                    .Skip(1)
                                    .Take(count);
            
            if (lastViewedArtList != null && lastViewedArtList.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.ResponseObjects.AddRange(lastViewedArtList);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
            }
            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetArticleBySearchText(string searchText, int skip = 0, int take = int.MaxValue)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            //richtige lösung
            /*var article_sel = (from a in plantDB.Articles
                               where !a.Deleted && a.Id == articleId && (availableOnly ? a.IsAvailable == true : true)
                               select a);*/

            //NUR FÜR Test
            var article_sel = (from a in plantDB.Articles
                               where !a.Deleted && (a.Name.Contains(searchText) || a.Brand.Contains(searchText) || a.ArticleCategories.Any(c => c.Title.Contains(searchText))) && a.AllowPublishment
                               select a).OrderBy(a => a.Id).Skip(skip).Take(take);

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
        public HelperClasses.DbResponse DbGetAllArticles(bool availableOnly = true, int skip = 0, int take = int.MaxValue)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            //RICHTIGE LÖSUNG
            /*var article_sel = (from a in plantDB.Articles
                               where !a.Deleted && (availableOnly ? a.IsAvailable == true : false)
                               select a);*/

            //Nur für Test
            var article_sel = (from a in plantDB.Articles
                               where !a.Deleted && a.AllowPublishment
                               select a).OrderBy(a => a.Sort).Skip(skip).Take(take);

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
        public DbResponse DbSortArticleByPrice(string sort)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            //RICHTIGE LÖSUNG
            /*var article_sel = (from a in plantDB.Articles
                               where !a.Deleted && (availableOnly ? a.IsAvailable == true : false)
                               select a);*/

            //Nur für Test
            IOrderedQueryable<Article> article_sel = null;

            switch (sort)
            {
                case "ascending":
                    article_sel = (from a in plantDB.Articles
                                   where !a.Deleted && a.AllowPublishment
                                   orderby a.Price ascending
                                   select a);
                    break;
                case "descending":
                    article_sel = (from a in plantDB.Articles
                                   where !a.Deleted && a.AllowPublishment
                                   orderby a.Price descending
                                   select a);
                    break;
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

            var article_sel = (from a in plantDB.Articles
                               where !a.Deleted && a.Id == newData.Id
                               select a);

            if (article_sel != null && article_sel.Any())
            {
                article_sel.FirstOrDefault().Name = newData.Name;
                article_sel.FirstOrDefault().Description = newData.Description;
                article_sel.FirstOrDefault().Price = newData.Price;
                article_sel.FirstOrDefault().IsAvailable = newData.IsAvailable;
                article_sel.FirstOrDefault().PricePercentagePayableWithPoints = newData.PricePercentagePayableWithPoints;
                article_sel.FirstOrDefault().OnEdit(newData.EditedBy);

                plantDB.Entry(article_sel.FirstOrDefault()).State = EntityState.Modified;

                bool isOk = plantDB.SaveChanges() > 0 ? true : false;

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
        public HelperClasses.DbResponse DbDeleteArticle(int articleId, string deletedBy)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var article_sel = (from a in plantDB.Articles
                               where !a.Deleted && a.Id == articleId
                               select a);

            if (article_sel != null && article_sel.Any())
            {
                article_sel.FirstOrDefault().Deleted = true;
                article_sel.FirstOrDefault().OnEdit(deletedBy);

                plantDB.Entry(article_sel.FirstOrDefault()).State = EntityState.Modified;

                bool isOk = plantDB.SaveChanges() > 0 ? true : false;

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
                articleRefs = plantDB.ArticleReference.Where(v => v.ReferenceType == type && v.PlantId == referenceId && v.Article.AllowPublishment);
            }
            else if (type == ArticleReferenceType.Todotemplate)
            {
                articleRefs = plantDB.ArticleReference.Where(v => v.ReferenceType == type && v.TodoTemplateId == referenceId && v.Article.AllowPublishment);
            }
            if (articleRefs != null && articleRefs.Any())
            {
                var articles = articleRefs.Select(v => v.Article).Where(a=>a.AllowPublishment);
                foreach (var a in articles)
                {
                   
                    ArticleViewModelLite artVm = new ArticleViewModelLite()
                    {
                        Name = a.Name,
                        Description = a.Description,
                        Label = a.Label,
                        Thumbnail = a.Thumbnail,
                        PhotoLink = a.PhotoLink,
                        ProductLink = a.ProductLink,
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