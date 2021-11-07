using GardifyWebAPI.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace SiteMapGenerator
{
    class Generator
    {
        private readonly string BASE_PATH = "https://gardify.de";
        private readonly string[] STATIC_ROUTES = new string[] {
            "/",  "/agb", "/bonusinfo", "/faq", "/glossar", "/impressum", "/kontakt", "/login", "/register", "/new",
            "/news", "/register", "/scanner","/shop","/start", "/suche","/vonAbisZ"};
        private const string NEWS_URL_FORMAT = "/news/{0}";
        private const string PLANT_URL_FORMAT = "/pflanze/{0}";
        private const string ARTICLE_URL_FORMAT = "/artikel/{0}";

        private readonly decimal STATIC_PRIORITY = new decimal(1.0);
        private readonly decimal NEWS_PRIORITY = new decimal(0.9);
        private readonly decimal PLANT_PRIORITY = new decimal(0.8);

        private readonly NewsController newsController = new NewsController();
        private readonly PlantSearchController plantSearchController = new PlantSearchController();
        private readonly ArticleController articleController = new ArticleController();

        private string UrlJoin(string base_, string path)
        {
            var baseUri = new Uri(base_);
            return new Uri(baseUri, path).ToString();
        }

        public string Slugify(string s)
        {
            s = s.Replace("ö", "oe").Replace("ä", "ae")
                  .Replace("ü", "ue").Replace("ß", "ss").Trim();
            s = Regex.Replace(s, @"[^-\w\s]", "");
            s = Regex.Replace(s, @"\s+", "-");
            return s.ToLower();
        }

        public byte[] Generate()
        {
            var siteMap = new SiteMap();

            foreach (var staticRoute in STATIC_ROUTES)
            {
                var url = UrlJoin(BASE_PATH, staticRoute);
                var node = new SiteMapNode
                {
                    Location = url,
                    Priority = STATIC_PRIORITY
                };
                siteMap.Add(node);
            }
            foreach (var news in newsController.Index().ListEntries)
            {
                var newsUrl = UrlJoin(BASE_PATH, String.Format(NEWS_URL_FORMAT, news.Id));
                var node = new SiteMapNode
                {
                    Location = newsUrl,
                    Priority = NEWS_PRIORITY,
                    LastModified = news.Date
                };
                siteMap.Add(node);
            }
            foreach (var plant in plantSearchController.Index().PlantList)
            {
                var plantUrl = UrlJoin(BASE_PATH, String.Format(PLANT_URL_FORMAT, plant.Id));
                plantUrl += "/" + Slugify(plant.NameGerman);
                var node = new SiteMapNode
                {
                    Location = plantUrl,
                    Priority = PLANT_PRIORITY,
                    LastModified = plant.EditedDate
                };
                siteMap.Add(node);
            }
            foreach (var article in articleController.Index().ListEntries)
            {
                var articleUrl = UrlJoin(BASE_PATH, String.Format(ARTICLE_URL_FORMAT, article.Id));
                var node = new SiteMapNode { Location = articleUrl };
                siteMap.Add(node);
            }

            XmlSerializer serializer = new XmlSerializer(typeof(SiteMap));
            XmlSerializerNamespaces namespaces = new XmlSerializerNamespaces();
            namespaces.Add("", "http://www.sitemaps.org/schemas/sitemap/0.9");

            using (var memoryStream = new MemoryStream())
            using (var xmlWriter = new XmlTextWriter(memoryStream, Encoding.UTF8))
            {
                serializer.Serialize(xmlWriter, siteMap, namespaces);
                memoryStream.Seek(0, SeekOrigin.Begin);
                return memoryStream.ToArray();
            }
        }
    }
}
