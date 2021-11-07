using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using System.Xml;
using System.Text;
using System.Xml.Linq;
using static GardifyModels.Models.NewsViewModels;

namespace GardifyWebAPI.Controllers
{
    [RoutePrefix("api/SitemapAPI")]

    public class SitemapAPIController : ApiController
    {
        // GET: SitemapAPI
        [Route("sitemap")]
        [HttpGet]
        public String Index()
        {
            var sb = new StringBuilder();

            List<string> routeList = new List<string> {
                "",
            "home",
            "start",
            "kontakt",
            "todo",
            "newdiary",
            "newtodo",
            "warnungen",
            "shop",
            "suche",
            "neue-pflanzen",
            "news",
            "community",
            "datenschutz",
            "pflanzendoc",
            "wetter",
            "login",
            "register",
            "register/danke",
            "scanner",
            "scanresults",
            "scanverlauf",
            "warenkorb",
            "newsletter",
            "newsletter-bestaetigt",
            "thread",
            "premium",
            "profil",
            "info",
            "agb",
            "widerruf",
            "vonAbisZ",
            "impressum",
            "neuesthema",
            "einstellungen",
            "meingarten",
            "meingarten/ecolist",
            "new",
            "resetpassword",
            "videos",
            "faq",
            "glossar",
            "bestellungen",
            "oekoscan",
            "oekoscan-result",
            "team",
            "oekoscan-result"};

            PlantController pc = new PlantController();
            NewsController nc = new NewsController();

            var plantList = pc.DbGetPublishedPlantList().ToList();
            foreach (var plant in plantList)
            {
                if (plant.NameGerman == null)
                    continue;

                routeList.Add("pflanze/" + plant.Id.ToString() + "/" + Uri.EscapeUriString(plant.NameGerman));
                //routeList = { routeList, plant.Id.ToString() + "/" + plant.NameGerman};
            }

            List<NewsEntryViewModel> newsList = nc.Index().ListEntries.ToList();
            foreach (var news in newsList)
            {
                if (news.Title == null)
                    continue;
                routeList.Add("news/" + news.Id + "/" + Uri.EscapeUriString(news.Title));
            }
   


            using (XmlWriter writer = XmlWriter.Create("C:\\Users\\till9\\source\\repos\\GardifyWebAPI\\GardifyWebAPINew\\App_Data\\sitemap.xml"))
            {
                writer.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");





                foreach (var route in routeList.OrderBy(r => r))
                {
                    writer.WriteStartElement("url");
                    writer.WriteElementString("loc", "https://gardify.de/" +route);

                    if (route.Contains("pflanze"))
                    {
                        writer.WriteElementString("changefreq", "monthly");

                        writer.WriteElementString("priority", "0.9");

                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
                //writer.Close();
                writer.Flush();

            }




            return "done";
        }

        string FormatXml(string xml)
        {
            try
            {
                XDocument doc = XDocument.Parse(xml);
                return doc.ToString();
            }
            catch (Exception)
            {
                // Handle and throw if fatal exception here; don't just ignore them
                return xml;
            }
        }
    }
}