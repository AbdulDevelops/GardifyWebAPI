using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Http.Results;
using System.Web.Mvc;
using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using GardifyModels.Models.GoogleAPIResponse;
using Newtonsoft.Json;
using MoreLinq;

namespace GardifyWebAPI.Controllers
{
    public class PlantSearchImageAPIController : ApiController
    {
        private UserPlantController upc = new UserPlantController();
        private ApplicationDbContext db = new ApplicationDbContext();
        private HttpClient client = new HttpClient();
        private const string GOOGLE_API_KEY = "AIzaSyAJJ3kNIPytdy47-pm2xt4aGy68FhD04LQ";
        private const string GSEARCH_ENGINE_ID = "013859794095342056622:zldfwmmctbi";
        private const string PLANTNET_API_KEY = "2a10JNOXg5LjKoSBQsOBDXy7ie";

        [System.Web.Http.HttpPost]
        public async System.Threading.Tasks.Task<IHttpActionResult> PostPlantSearchImage(bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            var request = Request;
            var address = "";
            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                address = ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }

            else if (HttpContext.Current != null)
            {
                address = HttpContext.Current.Request.UserHostAddress;
            }

            if (isIos)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromIos, (int)GardifyPages.Scanner,  EventObjectType.PageName, null, address);
                }
                else if (isAndroid)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromAndroid, (int)GardifyPages.Scanner,  EventObjectType.PageName, null, address);
                }
                else if (isWebPage)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage, (int)GardifyPages.Scanner,  EventObjectType.PageName, null, address);
                }
            else
            {
                new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage, (int)GardifyPages.Scanner, EventObjectType.PageName, null, address);

            }


            if (HttpContext.Current.Request.Files.Count > 0 &&
                HttpContext.Current.Request.Files[0].ContentType.Contains("image/") &&
                HttpContext.Current.Request.Files[0].ContentLength <= 25 * 1024 * 1024)
            {

                var img = HttpContext.Current.Request.Files[0];
                var userId = Utilities.GetUserId();

                HttpPostedFileBase filebase = new HttpPostedFileWrapper(img);
                var uploadedFile = upc.UploadScanImage(filebase, img, 0, img.FileName);

                var imageString = "";
                byte[] imageBytes;
                var folderPath = HttpContext.Current.Server.MapPath("~/nfiles/ScanImages/ImgMed");
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                var fullPath = Path.Combine(folderPath, uploadedFile.FileA);

                using (Image image = Image.FromFile(fullPath))
                {
                    using (MemoryStream m = new MemoryStream())
                    {
                        image.Save(m, image.RawFormat);
                        imageBytes = m.ToArray();

                        // Convert byte[] to Base64 String
                        imageString = Convert.ToBase64String(imageBytes);
                    }
                }

                ImageScan imgScan = new ImageScan();
                imgScan.UserId = userId;
                imgScan.ImageFilePath = fullPath;
                imgScan.ImageFileName = uploadedFile.FileA;
                imgScan.Date = DateTime.Now;

                imgScan.OnCreate(Utilities.GetUserName());
                ImageScan nis = db.ImageScans.Add(imgScan);
                db.SaveChanges();

                SearchResult res = new SearchResult();
                bool google = true; bool plantnet = true;

                if (plantnet)
                {
                    var plantNetPath = Path.Combine("https://gardifybackend.sslbeta.de/nfiles/ScanImages/ImgMed/", uploadedFile.FileA);

                    if (plantnet)
                    {
                        res.PnResults = await ScanWithPlantNet(plantNetPath);

                        if (res.PnResults == null)
                        {
                            return BadRequest("Es tut uns Leid, PlantNet ist gerade nicht erreichbar. Bitte versuche es später noch einmal!");

                        }
                        if (res.PnResults.results != null && res.PnResults.results.Any())
                        {
                            nis.PnName = !String.IsNullOrEmpty(res.PnResults.results.FirstOrDefault().species.commonNames.FirstOrDefault())
                                    ? res.PnResults.results.FirstOrDefault().species.commonNames.FirstOrDefault() : null;
                            nis.Family = res.PnResults.results.FirstOrDefault().species.family.scientificNameWithoutAuthor;

                            res.PnResults.results = res.PnResults.doubleResult;
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        res.PnResults = new PlantNetResult();
                    }
                }

                return Json(res);
            }
            else
            {
                return Json(new SearchResult());
            }
        }

        [System.Web.Http.NonAction]
        public string ExtractLabels(List<PlantViewModels.PlantViewModel> list)
        {
            var res = "";
            if (list != null && list.Any())
            {
                for (var i=0; i<list.Count; i++)
                {
                    res += list.ElementAt(i).Description;
                    if (i < list.Count-1)
                    {
                        res += ", ";
                    }
                }
            }
            return res;
        }

        [System.Web.Http.NonAction]
        private async System.Threading.Tasks.Task<SearchResult> ScanWithGoogleVision(string imageString)
        {
            GardifyModels.Models.GoogleAPIRequest.RootObject requestObject = new GardifyModels.Models.GoogleAPIRequest.RootObject
            {
                requests = new List<GardifyModels.Models.GoogleAPIRequest.Request>()
                {
                    new GardifyModels.Models.GoogleAPIRequest.Request
                    {
                        image = new GardifyModels.Models.GoogleAPIRequest.Image
                        {
                            content = imageString
                        },
                        features = new List<GardifyModels.Models.GoogleAPIRequest.Feature>()
                        {
                            new GardifyModels.Models.GoogleAPIRequest.Feature
                            {
                                maxResults = 50,
                                type = "LABEL_DETECTION"
                            },
                            new GardifyModels.Models.GoogleAPIRequest.Feature
                            {
                                maxResults = 10,
                                type = "WEB_DETECTION"
                            }
                        },
                        imageContext = new GardifyModels.Models.GoogleAPIRequest.ImageContext
                        {
                            languageHints = new List<string>()
                            {
                                "de"
                            }
                        }
                    }
                }
            };

            var jsonString = JsonConvert.SerializeObject(requestObject);
            var requestUrl = "https://vision.googleapis.com/v1/images:annotate?key=" + GOOGLE_API_KEY;
            string jsonResponse = "";

            var response = await client.PostAsync(
                requestUrl,
                new StringContent(jsonString, Encoding.UTF8, "application/json"));
            jsonResponse = await response.Content.ReadAsStringAsync();

            var responseObject = JsonConvert.DeserializeObject<GardifyModels.Models.GoogleAPIResponse.RootObject>(jsonResponse);
            var labels = responseObject.responses.First().labelAnnotations;
            var images = responseObject.responses.First().webDetection.visuallySimilarImages;

            // filter ignored words
            List<string> bw = db.BlacklistedWords.Select(x => x.Word).ToList();
            labels = labels.Where(x => !bw.Contains(x.description.ToLower())).ToList();

            // translate labels to german
            var translateUrl = "https://translation.googleapis.com/language/translate/v2?key=" + GOOGLE_API_KEY;
            var translateObject = new TranslateObject() { target = "de", q = new List<string>() };
            foreach (LabelAnnotation label in labels)
            {
                translateObject.q.Add(label.description);
            }
            var translateJson = JsonConvert.SerializeObject(translateObject);
            string translateJsonRes = "";

            var translateRes = await client.PostAsync(
                translateUrl,
                new StringContent(translateJson, Encoding.UTF8, "application/json"));

            translateJsonRes = await translateRes.Content.ReadAsStringAsync();

            var translateResObject = JsonConvert.DeserializeObject<TranslateResponse>(translateJsonRes);
            var translatedLabels = translateResObject.data.translations;

            for (int i = 0; i < labels.Count(); ++i)
            {
                labels.ElementAt(i).description = translatedLabels.ElementAt(i).translatedText;
            }

            PlantSearchAPIController pc = new PlantSearchAPIController();
            List<PlantViewModels.PlantViewModel> returnList = new List<PlantViewModels.PlantViewModel>();

            foreach (var label in labels)
            {
                IEnumerable<PlantViewModels.PlantViewModelLiteTodo> temp = pc.SearchResultLite(label.description.ToString()).Plants;

                if (temp != null)
                {
                    foreach (var plantFind in temp)
                    {
                        PlantViewModels.PlantViewModel newPlant = new PlantViewModels.PlantViewModel
                        {
                            Links = await GetLinks(plantFind.NameGerman),
                            NameGerman = plantFind.NameGerman,
                            NameLatin = plantFind.NameLatin,
                            Description = plantFind.Description,
                            Score = label.score,
                            Id = plantFind.Id
                        };
                        returnList.Add(newPlant);
                    }
                }
                else
                {
                    // keep suggested labels from google
                    PlantViewModels.PlantViewModel googleRes = new PlantViewModels.PlantViewModel
                    {
                        Links = await GetLinks(label.description),
                        Description = label.description,
                        Score = label.score,
                        Id = -1 // placeholder to differentiate results
                    };
                    returnList.Add(googleRes);
                }
            }

            return new SearchResult() { GPlants = returnList, GImages = images };
        }

        [System.Web.Http.NonAction]
        private async System.Threading.Tasks.Task<PlantNetResult> ScanWithPlantNet(string imageUrl)
        {
            var plantnetApi = "https://my-api.plantnet.org/v1/identify/all?";
            var customSearchApi = "https://www.googleapis.com/customsearch/v1?";

            // start identification
            var query = HttpUtility.ParseQueryString(string.Empty);
            query["images"] = imageUrl;
            query["organs"] = "flower";     // possible values: flower/bark/fruit/leaf/habit
            query["lang"] = "de";
            query["api-key"] = PLANTNET_API_KEY;
            string queryString = query.ToString();

            var idRes = await client.GetAsync(plantnetApi + queryString);
            var plantNames = new List<PnPlant>();
            var plantNamesOutput = new List<PnPlant>();

            if (idRes.IsSuccessStatusCode)
            {
                var idResponseString = await idRes.Content.ReadAsStringAsync();
                var idResponseObject = JsonConvert.DeserializeObject<PnResponse>(idResponseString);
                plantNames = idResponseObject.results;
            }
            else
            {
                return null;
            }

            PlantSearchController pc = new PlantSearchController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            List<PlantViewModels.PlantViewModel> plantsinDB = new List<PlantViewModels.PlantViewModel>();

            var gImagesQuery = HttpUtility.ParseQueryString(string.Empty);
            gImagesQuery["searchType"] = "image";
            gImagesQuery["fields"] = "items";
            gImagesQuery["cx"] = GSEARCH_ENGINE_ID;
            gImagesQuery["key"] = GOOGLE_API_KEY;


            if (plantNames != null && plantNames.Any())
            {
                // "spp." darf nicht übernommen werden
                plantNames = plantNames.Where(p => !p.species.scientificNameWithoutAuthor.Contains("spp.")).ToList();
                foreach (var plant in plantNames)
                {

                    // search for an image with google
                    gImagesQuery.Set("q", plant.species.scientificNameWithoutAuthor);
                    string gImagesQueryStr = gImagesQuery.ToString();
                    var csRes = await client.GetAsync(customSearchApi + gImagesQueryStr);

                    if (csRes.IsSuccessStatusCode)
                    {
                        var csResponseString = await csRes.Content.ReadAsStringAsync();
                        var customSearchResults = JsonConvert.DeserializeObject<CustomSearchResults>(csResponseString);
                        plant.images = customSearchResults.items;
                    }

                    // search in DB for matches
                    IEnumerable<Plant> plants = pc.Index(plant.species.genus.scientificNameWithoutAuthor, take: 10000).PlantList;

                   
                    List<Plant> sortedPlants = plants.Where(p => StringSimilarityScore(p.NameLatin, plant.species.scientificNameWithoutAuthor) > 0 ).OrderByDescending(p => StringSimilarityScore(p.NameLatin, plant.species.scientificNameWithoutAuthor)).Take(20).ToList();
                 
                    if (sortedPlants != null && sortedPlants.Any())
                    {
                        foreach (var plantFind in sortedPlants)
                        {
                            PlantViewModels.PlantViewModel newPlant = new PlantViewModels.PlantViewModel
                            {
                                NameGerman = plantFind.NameGerman,
                                NameLatin = plantFind.NameLatin,
                                Description = plantFind.Description,
                                Synonym = plantFind.Synonym,
                                Id = plantFind.Id
                            };

                            HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages(plantFind.Id);
                            if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                            {
                                newPlant.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                            }
                            else
                            {
                                newPlant.Images.Add(new _HtmlImageViewModel
                                {
                                    SrcAttr = Utilities.GetAbsolutePath("~/Images/gardify_Pflanzenbild_Platzhalter.svg"),
                                    Id = 0,
                                    TitleAttr = "Kein Bild vorhanden"
                                });
                            }
                            Plant p = db.Plants.Where(pl => pl.Id == plantFind.Id).FirstOrDefault();
                            //p.ImageScan_Id = imgScan.Id;
                            db.SaveChanges();
                            plantsinDB.Add(newPlant);
                        }
                        //plantNamesOutput.Remove(plant);
                    }
                   
                        plantNamesOutput.Add(plant);
                    
                }
            }


            return new PlantNetResult()
            {
                results = plantNames,
                doubleResult = plantNamesOutput,
                InDb = plantsinDB.DistinctBy(p => p.Id).ToList()
            };
        }

        [System.Web.Http.NonAction]
        private async System.Threading.Tasks.Task<string> GetLinks(string word)
        {
            word = word.Replace("[k]", "").Replace("[/k]", "").Replace("'", "");
            var wikiUrl = "https://de.wikipedia.org/w/api.php?action=opensearch&limit=1&namespace=0&format=json&search=" + word;
            var links = "";
            using (var client = new HttpClient())
            {
                var response = await client.GetAsync(wikiUrl);

                links = await response.Content.ReadAsStringAsync();

                return links;
            }
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/PlantSearchImageAPI/history")]
        public IEnumerable<ImageScanViewModel> ScanHistory()
        {
            var userId = Utilities.GetUserId();
            Guid empty = Guid.Empty;
            var res = new List<ImageScanViewModel>();
            var history = db.ImageScans.Where(e =>e.UserId == userId && e.UserId != empty).OrderBy(e => e.Date);
            foreach(ImageScan ims in history)
            {
                var temp = new ImageScanViewModel()
                {
                    Date = ims.Date,
                    GName = ims.GName,
                    PnName = ims.PnName,
                    Family = ims.Family,
                    ImageFileName = ims.ImageFileName,
                    ImageFilePath = ims.ImageFilePath
                };
                res.Add(temp);
            }
            return res;
        }

        double StringSimilarityScore(string name, string searchString)
        {
            string[] array = searchString.Split(' ');
            double len = 0;
            foreach (string value in array)
            {           
                if (name.Contains(value))
                {
                    len = len + (double)value.Length;
                }               
            }
            return len / (double)name.Length;       
        }
    }
}
