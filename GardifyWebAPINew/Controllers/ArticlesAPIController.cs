using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using Microsoft.VisualBasic.FileIO;
using static GardifyModels.Models.ArticleViewModels;

namespace GardifyWebAPI.Controllers
{
    public class ArticlesAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ArticleController ac = new ArticleController();

        // GET: api/ArticlesAPI
        [Route("api/ArticlesAPI/{skip}/{take}")]
        public ArticleViewModels.ArticleListViewModel GetArticles(int skip=0, int take = 8, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            // 8 is used on homepage
            var page = take != 8 ? GardifyPages.Shop : GardifyPages.Home;
           
                if (isIos)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromIos, (int)page, EventObjectType.PageName);
                }
                else if (isAndroid)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromAndroid, (int)page, EventObjectType.PageName);
                }
                else if (isWebPage)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage, (int)page, EventObjectType.PageName);
                }

            
            
            return ac.Index(skip * take,take);
        }

        // GET: api/ArticlesAPI/getall
        [Route("api/ArticlesAPI/getall")]
        public ArticleViewModels.ArticleListViewModel GetAllArticles()
        {
            // 8 is used on homepage
            return ac.GetAllArticles();
        }

        // GET: api/ArticlesAPI
        [Route("api/ArticlesAPI/count/{type}/{catId}/{searchText}")]
        public int GetArticlesCount(string type = null, int catId = 0, string searchText = null)
        {
            switch (type)
            {
                case "gift": return db.Articles.Where(art => art.AllowPublishment && art.ArticleCategories.Where(cat => cat.IsGiftIdea == true).Any()).Count();
                case "cat": return db.Articles.Where(art => art.AllowPublishment && art.ArticleCategories.Where(cat => cat.Id == catId).Any()).Count();
                case "search": return (from a in db.Articles
                                       where !a.Deleted && (a.Name.Contains(searchText) || a.Brand.Contains(searchText) || a.ArticleCategories.Any(c => c.Title.Contains(searchText))) && a.AllowPublishment
                                       select a).Count();
                default: return db.Articles.Where(a => !a.Deleted && a.AllowPublishment).Count();
            }
        }

        [HttpGet]
        [Route("api/ArticlesAPI/parse")]
        public List<Article> ParseCSV()
        {
            List<Article> res = new List<Article>();
            //var csv = System.IO.File.ReadAllLines(@"C:\Users\till9\source\repos\GardifyWebAPI\GardifyWebAPI\App_Data\products.csv", Encoding.UTF8);
            //foreach(var row in csv) 
            //{
            //    var elements = row.Split(';');
            //    var article = new Article()
            //    {
            //        Label = elements.ElementAt(0),
            //        IsAvailable = true,
            //        Name = elements.ElementAt(0),
            //        ArticleNr = elements.ElementAt(1),
            //        Price = decimal.Parse(elements.ElementAt(2), CultureInfo.InvariantCulture),
            //        Description = elements.ElementAt(3),
            //        ProductLink = elements.ElementAt(4),
            //        Thumbnail = elements.ElementAt(5),
            //        PhotoLink = elements.ElementAt(6),
            //    };
            //    article.OnCreate("Parser");
            //    res.Add(article);
            //}
            //db.Articles.AddRange(res);
            //db.SaveChanges();
            //return res;
            

            using (TextFieldParser parser = new TextFieldParser(@"C:\Users\till9\source\repos\GardifyWebAPI\GardifyWebAPI\App_Data\products.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(";");
                parser.ReadFields();    // skip first line
                while (!parser.EndOfData)
                {
                    string[] row = parser.ReadFields();
                    var article = new Article()
                    {
                        Label = row.ElementAt(0),
                        IsAvailable = true,
                        Name = row.ElementAt(0),
                        ArticleNr = row.ElementAt(1),
                        Price = decimal.Parse(row.ElementAt(2), CultureInfo.InvariantCulture),
                        Description = row.ElementAt(3),
                        ProductLink = row.ElementAt(4),
                        Thumbnail = row.ElementAt(5),
                        PhotoLink = row.ElementAt(6),
                    };
                    article.OnCreate("Parser");
                    res.Add(article);
                }
            }
            db.Articles.AddRange(res);
            db.SaveChanges();
            return res;
        }

        [HttpGet]
        [Route("api/ArticlesAPI/categories")]
        public List<ArticleCategoryViewModel> GetArticleCategories()
        {
            var list = db.ArticleCategories.Where(ac => !ac.Deleted);
            var res = new List<ArticleCategoryViewModel>();
            foreach (ArticleCategory cat in list)
            {
                var temp = new ArticleCategoryViewModel()
                {
                    Title = cat.Title,
                    IsGiftIdea = cat.IsGiftIdea,
                    Id = cat.Id
                };
                res.Add(temp);
            }
            return res;
        }
       
        //GET:api/LastViewedArticles
        [Route("api/ArticlesAPI/LastViewedArticles")]
        public IEnumerable<ArticleViewModels.ArticleViewModel> GetLastViewedArticles()
        {
            ArticleController ac = new ArticleController();
            return ac.getXLastViewedArt();
        }

        [HttpGet]
        [Route("api/ArticlesAPI/category/{id}/{skip}/{giftCat}")]
        public ArticleViewModels.ArticleListViewModel GetArticleByCategoryId(int id=0, int skip = 0, bool giftCat=false, int take = 8)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            var res = new List<ArticleViewModels.ArticleViewModel>();
            IQueryable<Article> list = null;
            if (giftCat==true)
            {
                list = db.Articles.Where(art => art.AllowPublishment && art.ArticleCategories.Where(cat => cat.IsGiftIdea == true).Any()).OrderBy(x => x.Id).Skip(skip * take).Take(take);
            }
            else
            if(id>0)
            {
                list = db.Articles.Where(art => art.AllowPublishment &&  art.ArticleCategories.Where(cat => cat.Id == id).Any()).OrderBy(x => x.Id).Skip(skip * take).Take(take);
            }
            
            foreach (Article article in list)
            {
                HelperClasses.DbResponse imageResponse = rc.DbGetArticleReferencedImages(article.Id);
                var temp = new ArticleViewModels.ArticleViewModel()
                {
                    Name = article.Name,
                    Description = article.Description,
                    NormalPrice = article.Price,
                    IsAvailable = article.IsAvailable,
                    Label = article.Label,
                    Thumbnail = article.Thumbnail,
                    ProductLink = article.ProductLink,
                    PhotoLink = article.PhotoLink,
                    IsNotDeliverable = article.IsNotDeliverable,
                    ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/")),
                    Id = article.Id,
                    ExpertTip = article.ExpertTip
                };
                res.Add(temp);
            }

            return new ArticleViewModels.ArticleListViewModel()
            {
                ListEntries = res
            };
        }

        //GET: api/SortedArticlesByPrice
        [Route("api/ArticlesAPI/SortedArticlesByPrice/{skip}")]
        public ArticleViewModels.ArticleListViewModel GetSortedArticlesByPrice(string sortType,int skip=0)
        {
            return ac.sortArticlesByPrice(sortType,skip * 8);
        }

        // GET: api/ArticlesAPI/5
        [Route("api/ArticlesAPI/articleById/{id}")]
        [ResponseType(typeof(Article))]
        public ArticleViewModel GetArticle(int id)
        {
            ArticleController ac = new ArticleController();
            var vm = ac.GetArticleById(id);


            return vm;
        }

        [Route("api/ArticlesAPI/articleByIdLite/{id}")]
        [ResponseType(typeof(Article))]
        public ArticleViewModel GetArticleLite(int id)
        {
            ArticleController ac = new ArticleController();
            var vm = ac.GetArticleByIdLite(id);


            return vm;
        }

        [HttpPost]
        [Route("api/ArticlesAPI/createLastViewedArt/{articleId}")]
        public IHttpActionResult CreateLastViewedArt(int articleId)
        {
            ArticleController ac = new ArticleController();
            var userId = Utilities.GetUserId();
            bool isRelevant = false;
            
            var previousArt = db.LastViewedArticles.Where(art => art.UserId == userId && art.Id != articleId)
                                            .OrderByDescending(p => p.Id).FirstOrDefault();
            if (previousArt != null)
            {
                double timeDiff = (DateTime.Now - previousArt.CreatedDate).TotalDays;
                isRelevant = timeDiff <= 1 ? true : false;
            }

            // add ref to previous article only if it exists and was viewed in the last 24 hours
            ac.DbCreateLastViewedArticle(articleId, isRelevant ? previousArt.ArticleId : 0);
            
            return Ok();
        }

        // GET: api/ArticlesAPI/5
        [Route("api/ArticlesAPI/search/{skip}")]
        public ArticleViewModels.ArticleListViewModel GetArticleSearchText(string searchText, int skip=0,int take=8)
        {
            ArticleController ac = new ArticleController();

            return ac.GetArticleBySearchText(searchText,skip * take,take);
        }

        [HttpGet]
        [Route("api/ArticlesAPI/viewedby/{artId}")]
        public List<ArticleViewModels.ArticleViewModel> GetLastViewedByOtherUsers(int artId, int take = 4)
        {
            var userId = Utilities.GetUserId();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            var test = (from last in db.LastViewedArticles
                        join arts in db.Articles on last.PreviousId equals arts.Id 
                        where last.ArticleId == artId && last.UserId != userId && arts.AllowPublishment && !arts.Deleted
                        select new { last.PreviousId, arts.Name, arts.Price, arts.Thumbnail })
                            .GroupBy(c => new
                            {
                                c.PreviousId,
                                c.Name,
                                c.Price,
                                c.Thumbnail
                            })
                            .Select(e => new ViewedByArticleVM
                            {
                                ArticleId = e.Key.PreviousId,
                                Thumbnail = e.Key.Thumbnail,
                                Price = e.Key.Price,
                                Name = e.Key.Name,
                                Count = e.Count()
                            }).Take(take)
                            .OrderByDescending(p => p.Count)
                            .ToList();

            var res = new List<ArticleViewModels.ArticleViewModel>();
            foreach (ViewedByArticleVM art in test.Where(a => a.ArticleId != artId))
            {
                HelperClasses.DbResponse imageResponse = rc.DbGetArticleReferencedImages(art.ArticleId);
                var temp = new ArticleViewModels.ArticleViewModel()
                {
                    NormalPrice = art.Price,
                    Thumbnail = art.Thumbnail,
                    Name = art.Name,
                    Id = art.ArticleId,
                    ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/")),
                };
                res.Add(temp);
            }

            return res;
        }

        [Route("api/ArticlesAPI/affiliate")]
        public List<ArticleViewModels.ArticleViewModel> GetAffiliateArticles()
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            var list = db.Articles.Where(a => !String.IsNullOrEmpty(a.AffiliateLink) && !a.Deleted).ToList();
            var res = new List<ArticleViewModels.ArticleViewModel>();
            foreach (Article art in list)
            {
                HelperClasses.DbResponse imageResponse = rc.DbGetArticleReferencedImages(art.Id);

                var temp = new ArticleViewModels.ArticleViewModel()
                {
                    AffiliateLink = art.AffiliateLink,
                    Name = art.Name,
                    ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"))
                };
                res.Add(temp);
            }
            return res;
        }

        [HttpGet]
        [Route("api/ArticlesAPI/purchases")]
        public List<PurchasedArticleViewModel> GetLastPurchases()
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            var userId = Utilities.GetUserId();
            var list = db.ShopOrders.Where(pa => pa.UserId.Equals(userId) && (pa.OrderConfirmed || pa.TCResponseCode == "Y")).OrderByDescending(a => a.CreatedDate).Take(4);
            var res = new List<PurchasedArticleViewModel>();
            if (list != null && list.Any())
            {
                foreach (ShopOrder art in list)
                {
                    var firstOrderArt = art.ArticlesInCart.FirstOrDefault();
                    if (firstOrderArt != null)
                    {
                        var firstArticleVM = ac.GetArticleVMByCartEntry(firstOrderArt.Id, userId);
                        var temp = new PurchasedArticleViewModel()
                        {
                            ArticleId = firstArticleVM.Id,
                            Name = firstArticleVM.Name,
                            PurchaseDate = firstOrderArt.CreatedDate,
                            Price = firstArticleVM.NormalPrice,
                            ArticleImages = firstArticleVM.ArticleImages
                        };
                        res.Add(temp);
                    }
                }
            }
            return res;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ArticleExists(int id)
        {
            return db.Articles.Count(e => e.Id == id) > 0;
        }
    }

    public class ViewedByArticleVM
    {
        public int Count { get; set; }
        public string Thumbnail { get; set; }
        public decimal Price { get; set; }
        public string Name { get; set; }
        public int ArticleId { get; set; }
    }

}