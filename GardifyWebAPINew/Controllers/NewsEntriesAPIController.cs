using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using Newtonsoft.Json;
using RestSharp;
using static GardifyModels.Models.ModelEnums;

namespace GardifyWebAPI.Controllers
{
    public class NewsEntriesAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/NewsEntriesAPI
        public NewsViewModels.NewsListViewModel GetNewsEntries(int skip=0, int take = int.MaxValue, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            NewsController nc = new NewsController();
            var userId = Utilities.GetUserId();

            // 10 is used on homepage
            if (take != 10)
            {
               
                    if (isIos)
                    {
                        new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, userId, (int)StatisticEventTypes.ApiCallFromIos, (int)GardifyPages.News, EventObjectType.PageName);
                    }
                    else if (isAndroid)
                    {
                        new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, userId, (int)StatisticEventTypes.ApiCallFromAndroid, (int)GardifyPages.News, EventObjectType.PageName);
                    }
                    else if (isWebPage)
                    {
                        new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, userId, (int)StatisticEventTypes.ApiCallFromWebpage, (int)GardifyPages.News, EventObjectType.PageName);
                    }


               
            }
            return nc.Index(skip * take, take);
        }
        
        public NewsViewModels.NewsEntryViewModel GetNewsEntry(int? id)
        {
            NewsController nc = new NewsController();
            return nc.Details(id);
        }

        [HttpGet]
        [Route("api/NewsEntriesAPI/getInstaPost")]
        public NewsViewModels.InstaNewsViewModel getInstaPost(int skip = 0, int take = int.MaxValue)
        {
            NewsController nc = new NewsController();

            var output = nc.GetInstaPosts(skip * take, take);
            return output;

        }

        [HttpGet]
        [Route("api/NewsEntriesAPI/getRefreshInstaPost")]
        public IHttpActionResult getRefreshInstaPost()
        {
            NewsController nc = new NewsController();
            var res = nc.RefreshandSaveInstaPost();

            return Ok(res);
        }

        [HttpGet]
        [Route("api/NewsEntriesAPI/getInstaImage/{imageId}")]
        public List<string> getInstaImage(string imageId)
        {
            var codeToken = "IGQVJYcG1pS1FXaUN2QlJldUtuMDF5MG1JQVVvNHM1WVhHeW90M1pPSFB2Skh5R3NpNnBtekJlNWVhRjhkaWp3Q3ZABOHFCN1B4VkVUY0ZACS0U2VklDNk8yTmlTaDhPbXFDbW1DdEFB";

            var imageList = imageId.Split(',');
            List<string> imageUrlList = new List<string>();
            foreach(var image in imageList)
            {
                var client = new RestClient("https://graph.instagram.com/" + image + "?fields=id,media_type,media_url,username,thumbnail_url,timestamp&access_token=" + codeToken);
                client.Timeout = -1;
                var request = new RestRequest(Method.GET);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);
                NewsViewModels.InstaNewsEntrySingleModel output = new NewsViewModels.InstaNewsEntrySingleModel();
                NewsController nc = new NewsController();

                if (!response.IsSuccessful)
                {
                    continue;


                }

                output = JsonConvert.DeserializeObject<NewsViewModels.InstaNewsEntrySingleModel>(response.Content);
                if (output == null)
                {
                    continue;

                }
                imageUrlList.Add(output.media_url);
            }

            
            return imageUrlList;

        }
        public bool instapostexist(string postId)
        {
            var instaPost = (from p in db.InsatPostEntry where !p.Deleted && p.PostId.Equals(postId) select p).FirstOrDefault();
            if (instaPost != null)
            {
                return true;

            }
            else
            {
                return false;
            }
        }

        [HttpPost]
        [ResponseType(typeof(NewsEntry))]
        [Route("api/NewsEntriesAPI/upload")]
        public NewsViewModels.NewsListViewModel UploadNewsEntryImage()
        {
            var imageFile = HttpContext.Current.Request.Files[0] != null ? HttpContext.Current.Request.Files[0] : null;
            var id = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);
            var imageTitle = HttpContext.Current.Request.Params["imageTitle"];
            var imageDescription = HttpContext.Current.Request.Params["imageDescription"];
            HttpPostedFileBase filebase = new HttpPostedFileWrapper(imageFile);

            NewsController nc = new NewsController();
            nc.UploadNewsEntryImage(filebase, id, imageTitle, imageDescription);
            return nc.Index();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}