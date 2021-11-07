using GardifyWebAPI.App_Code;
using System;
using System.Web.Configuration;
using System.Web.Mvc;
using GardifyModels.Models;
using System.Web;
using System.Linq;

namespace GardifyWebAPI.Controllers.AdminArea
{
   // [CustomAuthorizeAttribute(Roles = "Admin")]
    public class AdminAreaUtilsController : Controller
    {
        // GET: intern/utils
        public ActionResult Index()
        {
            GardifyModels.Models.AdminAreaViewModels.UtilsViewModel utilsViewModel = new GardifyModels.Models.AdminAreaViewModels.UtilsViewModel();
            utilsViewModel.StatusMessage = new GardifyModels.Models.AdminAreaViewModels.UtilsViewModel.Message("Ok");
            PropertyController pc = new PropertyController();
            var up = pc.DbGetProperty(Utilities.GetUserId());

            WeatherHandler wh = new WeatherHandler();
            //WeatherForecast forecast = wh.getWeatherForecastByGeoCoords(up.Latitude, up.Longtitude);
            //WeatherHelpers.HourlyForecast weatherCurrentHour = forecast.Forecasts.Hourly.FirstOrDefault();
            //utilsViewModel.CurrentWeather = weatherCurrentHour;

            utilsViewModel.UseCustomData = bool.Parse(WebConfigurationManager.AppSettings["useCustomWeatherAndDate"]);
            utilsViewModel.CustomCurrentDate = DateTime.Parse(WebConfigurationManager.AppSettings["customDate"]);
            utilsViewModel.CustomCurrentTemperature = int.Parse(WebConfigurationManager.AppSettings["customTemperature"]);
            utilsViewModel.PointValue = decimal.Parse(WebConfigurationManager.AppSettings["pointValueInEuro"]);

            ApplicationDbContext appContext = new ApplicationDbContext();
            var roles = appContext.Roles.ToArray();

            ViewBag.message += string.Join(", ", roles.Select(v => v.Name));


            return View("~/Views/AdminArea/AdminAreaUtils/Index.cshtml", utilsViewModel);
        }

        // POST: intern/utils/migrate-db
        [HttpPost]
        [ActionName("migrate-db")]
        [ValidateAntiForgeryToken]
        public ActionResult MigrateDb()
        {

            GardifyModels.Models.AdminAreaViewModels.UtilsViewModel.Message statusMessage = new GardifyModels.Models.AdminAreaViewModels.UtilsViewModel.Message("nothing happens");
            return PartialView("~/Views/AdminArea/AdminAreaUtils/_statusMessage.cshtml", statusMessage);
        }

        [HttpPost]
        [ActionName("change-custom-settings")]
        [ValidateAntiForgeryToken]
        public ActionResult changeCustomSettings(GardifyModels.Models.AdminAreaViewModels.UtilsViewModel utilsViewModel)
        {
            WebConfigurationManager.AppSettings["useCustomWeatherAndDate"] = utilsViewModel.UseCustomData.ToString();
            WebConfigurationManager.AppSettings["customDate"] = utilsViewModel.CustomCurrentDate.ToString();
            WebConfigurationManager.AppSettings["customTemperature"] = utilsViewModel.CustomCurrentTemperature.ToString();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ActionName("import-plantus")]
        [ValidateAntiForgeryToken]
        public ActionResult importPlantusDB(GardifyModels.Models.AdminAreaViewModels.UtilsViewModel utilsViewModel)
        {
            //importPlantusDB();
            return RedirectToAction("Index");
        }

        /* ---------------------------------------------------- */
        private void importPlantusDB()
        {
            ApplicationDbContext db = new ApplicationDbContext();

            int plantusTreeRootId = 2768;
            TaxonomicTree plantusRootNode = (from t in db.TaxonomicTree where t.Id == plantusTreeRootId select t).FirstOrDefault();
            if (plantusRootNode.Childs == null)
            {
                plantusRootNode.Childs = new System.Collections.Generic.List<TaxonomicTree>();
            }

            int plantusIdCharacteristicId = 20;
            string sourceFile = HttpRuntime.AppDomainAppPath + "/plantusDb.csv";

            int counter = 0;
            string line;
            // Read the file and display it line by line.
            System.IO.StreamReader file = new System.IO.StreamReader(sourceFile);
            while ((line = file.ReadLine()) != null)
            {
                // 0 - RelID, 
                // 1 - Subart -Name
                // 2 - dt. Name
                // 3 - Abteilung
                // 4 - Unterabteilung
                // 5 - Klasse
                // 6 - Unterklasse
                // 7 - Ordnung
                // 8 - Familie
                // 9 - dt. Familienname
                // 10 - Gattung
                // 11 - Art
                string[] plantData = line.Split(';');

                //Phylum , Subphylum, ClassTaxon, SubclassTaxon, Order, Family, Genus, Species

                // Phylum - Abteilung
                string phylumName = plantData[3];
                TaxonomicTree phylum = null;
                if (plantusRootNode.Childs != null && plantusRootNode.Childs.Count > 0 && plantusRootNode.Childs.Find(c => c.TitleLatin == phylumName) != null)
                {
                    phylum = plantusRootNode.Childs.Find(c => c.TitleLatin == phylumName);
                }
                else
                {
                    phylum = new TaxonomicTree();
                    phylum.TitleLatin = phylumName;
                    phylum.RootID = plantusRootNode.Id;
                    phylum.ParentId = plantusRootNode.Id;
                    phylum.Taxon = ModelEnums.TaxonomicRank.Phylum;
                    phylum.Type = ModelEnums.NodeType.Node;

                    phylum.Childs = new System.Collections.Generic.List<TaxonomicTree>();
                    plantusRootNode.Childs.Add(phylum);
                    db.TaxonomicTree.Add(phylum);
                    db.SaveChanges();
                }

                // Subphylum - Unterabteilung
                string subphylumName = plantData[4];
                TaxonomicTree subphylum = null;
                if (phylum.Childs != null && phylum.Childs.Count > 0 && phylum.Childs.Find(c => c.TitleLatin == subphylumName) != null)
                {
                    subphylum = phylum.Childs.Find(c => c.TitleLatin == subphylumName);
                }
                else
                {
                    subphylum = new TaxonomicTree();
                    subphylum.TitleLatin = subphylumName;
                    subphylum.Taxon = ModelEnums.TaxonomicRank.Subphylum;
                    subphylum.ParentId = phylum.Id;

                    subphylum.Type = ModelEnums.NodeType.Node;
                    subphylum.RootID = plantusRootNode.Id;

                    subphylum.Childs = new System.Collections.Generic.List<TaxonomicTree>();
                    phylum.Childs.Add(subphylum);
                    db.TaxonomicTree.Add(subphylum);
                    db.SaveChanges();
                }

                // Class - Klasse
                string className = plantData[5];
                TaxonomicTree classTaxon = null;
                if (subphylum.Childs != null && subphylum.Childs.Count > 0 && subphylum.Childs.Find(c => c.TitleLatin == className) != null)
                {
                    classTaxon = subphylum.Childs.Find(c => c.TitleLatin == className);
                }
                else
                {
                    classTaxon = new TaxonomicTree();
                    classTaxon.TitleLatin = className;
                    classTaxon.Taxon = ModelEnums.TaxonomicRank.ClassTaxon;
                    classTaxon.ParentId = subphylum.Id;

                    classTaxon.Type = ModelEnums.NodeType.Node;
                    classTaxon.RootID = plantusRootNode.Id;

                    classTaxon.Childs = new System.Collections.Generic.List<TaxonomicTree>();
                    subphylum.Childs.Add(classTaxon);

                    db.TaxonomicTree.Add(classTaxon);
                    db.SaveChanges();
                }

                // Subclass - Unterklasse
                string subclassName = plantData[6];
                TaxonomicTree subclass = null;
                if (classTaxon.Childs != null && classTaxon.Childs.Count > 0 && classTaxon.Childs.Find(c => c.TitleLatin == subclassName) != null)
                {
                    subclass = classTaxon.Childs.Find(c => c.TitleLatin == subclassName);
                }
                else
                {
                    subclass = new TaxonomicTree();
                    subclass.TitleLatin = subclassName;
                    subclass.Taxon = ModelEnums.TaxonomicRank.SubclassTaxon;
                    subclass.ParentId = classTaxon.Id;

                    subclass.Type = ModelEnums.NodeType.Node;
                    subclass.RootID = plantusRootNode.Id;

                    subclass.Childs = new System.Collections.Generic.List<TaxonomicTree>();
                    classTaxon.Childs.Add(subclass);

                    db.TaxonomicTree.Add(subclass);
                    db.SaveChanges();
                }

                // Order - Ordnung
                string orderName = plantData[7];
                TaxonomicTree order = null;
                if (subclass.Childs != null && subclass.Childs.Count > 0 && subclass.Childs.Find(c => c.TitleLatin == orderName) != null)
                {
                    order = subclass.Childs.Find(c => c.TitleLatin == orderName);
                }
                else
                {
                    order = new TaxonomicTree();
                    order.TitleLatin = orderName;
                    order.Taxon = ModelEnums.TaxonomicRank.Order;
                    order.ParentId = subclass.Id;

                    order.Type = ModelEnums.NodeType.Node;
                    order.RootID = plantusRootNode.Id;

                    order.Childs = new System.Collections.Generic.List<TaxonomicTree>();
                    subclass.Childs.Add(order);

                    db.TaxonomicTree.Add(order);
                    db.SaveChanges();
                }

                // Family - Familie
                string familyName = plantData[8];
                TaxonomicTree family = null;
                if (order.Childs != null && order.Childs.Count > 0 && order.Childs.Find(c => c.TitleLatin == familyName) != null)
                {
                    family = order.Childs.Find(c => c.TitleLatin == familyName);
                }
                else
                {
                    family = new TaxonomicTree();
                    family.TitleLatin = familyName;
                    family.TitleGerman = plantData[9];
                    family.Taxon = ModelEnums.TaxonomicRank.Family;
                    family.ParentId = order.Id;

                    family.Type = ModelEnums.NodeType.Node;
                    family.RootID = plantusRootNode.Id;

                    family.Childs = new System.Collections.Generic.List<TaxonomicTree>();
                    order.Childs.Add(family);

                    db.TaxonomicTree.Add(family);
                    db.SaveChanges();
                }

                // Genus - Gattung
                string genusName = plantData[10];
                TaxonomicTree genus = null;
                if (family.Childs != null && family.Childs.Count > 0 && family.Childs.Find(c => c.TitleLatin == genusName) != null)
                {
                    genus = family.Childs.Find(c => c.TitleLatin == genusName);
                }
                else
                {
                    genus = new TaxonomicTree();
                    genus.TitleLatin = genusName;
                    genus.Taxon = ModelEnums.TaxonomicRank.Genus;
                    genus.ParentId = family.Id;

                    genus.Type = ModelEnums.NodeType.Node;
                    genus.RootID = plantusRootNode.Id;

                    genus.Childs = new System.Collections.Generic.List<TaxonomicTree>();
                    family.Childs.Add(genus);
                    db.TaxonomicTree.Add(genus);
                    db.SaveChanges();
                }

                Plant newPlant = new Plant();
                newPlant.NameLatin = plantData[10] + " " + plantData[11] + (!String.IsNullOrEmpty(plantData[1]) ? " '" + plantData[1] + "'" : "");
                newPlant.NameGerman = plantData[2];
                newPlant.Description = "importiert aus Plantus Datenbank";
                db.Plants.Add(newPlant);
                db.SaveChanges();

                PlantCharacteristic pc = new PlantCharacteristic();
                pc.CategoryId = plantusIdCharacteristicId;
                pc.Min = int.Parse(plantData[0]);
                pc.PlantId = newPlant.Id;
                db.PlantCharacteristic.Add(pc);

                // Species - Art
                string speciesName = plantData[10] + " " + plantData[11] + (!String.IsNullOrEmpty(plantData[1]) ? " '" + plantData[1] + "'" : "");
                TaxonomicTree species = new TaxonomicTree();
                species.TitleLatin = speciesName;
                species.TitleGerman = plantData[2];
                species.Taxon = ModelEnums.TaxonomicRank.Species;
                species.ParentId = genus.Id;
                species.PlantId = newPlant.Id;

                species.Type = ModelEnums.NodeType.Leaf;
                species.RootID = plantusRootNode.Id;

                genus.Childs.Add(genus);
                db.TaxonomicTree.Add(species);
                db.SaveChanges();

                counter++;
            }

            ViewBag.counter = counter;
            file.Close();
        }
    }
}
