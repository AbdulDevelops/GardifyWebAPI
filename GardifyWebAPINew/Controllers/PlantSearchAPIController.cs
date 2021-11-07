using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using Newtonsoft.Json;
using System.Data.Entity;
using static GardifyModels.Models.HelperClasses;
using static GardifyModels.Models.PlantViewModels;
using MoreLinq;
using MoreLinq.Extensions;

namespace GardifyWebAPI.Controllers
{
    public class PlantSearchAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/PlantSearchAPI/5
        [ResponseType(typeof(Plant))]
        [System.Web.Http.Route("api/PlantsearchAPI/{id}")]
        public IHttpActionResult GetPlantEntry(int id)
        {
            PlantSearchController pc = new PlantSearchController();
            var vm = pc.PlantDetails(id);
            return Ok(vm);
        }

        // GET: api/PlantSearchAPI/5
        [ResponseType(typeof(Plant))]
        [System.Web.Http.Route("api/PlantsearchAPI/getlite/{id}")]
        public IHttpActionResult GetPlantEntryLite(int id)
        {
            PlantSearchController pc = new PlantSearchController();
            var vm = pc.PlantDetailsLite(id);
            return Ok(vm);
        }

        [ResponseType(typeof(IEnumerable<Plant>))]
        [System.Web.Http.Route("api/PlantsearchAPI/FindSiblingById/{id}")]
        public IHttpActionResult GetPlantSiblings(int id)
        {
            PlantController pc = new PlantController();
            var vm = pc.DbGetSiblings(id);
            return Ok(vm);
        }

        [ResponseType(typeof(IEnumerable<Plant>))]
        [System.Web.Http.Route("api/PlantsearchAPI/families")]
        public IHttpActionResult GetPlantFamilies()
        {
            var fams = db.Plants.Where(p => !p.Deleted && p.Published)
                                .Select(p => p.Familie.Replace("[k]","").Replace("[/k]","").Replace("[","").Replace("]","").Trim())
                                .OrderBy(f => f).Distinct();
            return Ok(fams);
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/PlantsearchAPI/latest/{month}")]
        public IEnumerable<PlantViewModels.LatestPlantViewModel> GetLatestPlantsAuto(int month)
        {
            PlantSearchController psc = new PlantSearchController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            int currentYear = DateTime.Now.Year;
            DateTime timeData = new DateTime(currentYear, month, 1);
            if (DateTime.Now < timeData)
            {
                currentYear -= 1;
            }
            var plants = db.Plants
                         .Where(m => !m.Deleted && m.Published && m.PublishedDate.Month == month && m.PublishedDate.Year == currentYear)
                         .OrderBy(p => p.NameGerman);
            List<PlantViewModels.LatestPlantViewModel> res = new List<PlantViewModels.LatestPlantViewModel>();

            foreach (Plant mod in plants)
            {
                PlantViewModels.LatestPlantViewModel temp = new PlantViewModels.LatestPlantViewModel();
                var props = psc.GetPlantColorsAndBadgesById(mod.Id);
                temp.NameGerman = mod.NameGerman;
                temp.Id = mod.Id;
                temp.Description = mod.Description;
                temp.NameLatin = mod.NameLatin;
                temp.Images = new List<_HtmlImageViewModel>();
                temp.Synonym = mod.Synonym;
                temp.Colors = props.Colors;
                temp.Badges = props.Badges;
                temp.MonthAdded = mod.PublishedDate.Month - 1; // 0-indexed in JS

                HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages(mod.Id);
                if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                {
                    temp.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                }
                else
                {
                    temp.Images.Add(new _HtmlImageViewModel
                    {
                        SrcAttr = Utilities.GetAbsolutePath("~/Images/gardify_Pflanzenbild_Platzhalter.svg"),
                        Id = 0,
                        TitleAttr = "Kein Bild vorhanden"
                    });
                }
                res.Add(temp);
            }
            var orderedPlantsInRes = res.OrderBy(x => x.NameLatin.Contains("[k]") ? x.NameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.NameLatin);

            return orderedPlantsInRes;
        }

        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/PlantsearchAPI/latest/{year}/{month}")]
        public IEnumerable<PlantViewModels.LatestPlantViewModel> GetLatestPlants(int month, int year)
        {
            PlantSearchController psc = new PlantSearchController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            //int currentYear = DateTime.Now.Year;
            var plants = db.Plants
                         .Where(m => !m.Deleted && m.Published && m.PublishedDate.Month == month && m.PublishedDate.Year == year)
                         .OrderBy(p => p.NameGerman);
            List<PlantViewModels.LatestPlantViewModel> res = new List<PlantViewModels.LatestPlantViewModel>();

            foreach (Plant mod in plants)
            {
                PlantViewModels.LatestPlantViewModel temp = new PlantViewModels.LatestPlantViewModel();
                var props = psc.GetPlantColorsAndBadgesById(mod.Id);
                temp.NameGerman = mod.NameGerman;
                temp.Id = mod.Id;
                temp.Description = mod.Description;
                temp.NameLatin = mod.NameLatin;
                temp.Images = new List<_HtmlImageViewModel>();
                temp.Synonym = mod.Synonym;
                temp.Colors = props.Colors;
                temp.Badges = props.Badges;
                temp.MonthAdded = mod.PublishedDate.Month - 1; // 0-indexed in JS

                HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages(mod.Id);
                if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                {
                    temp.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                }
                else
                {
                    temp.Images.Add(new _HtmlImageViewModel
                    {
                        SrcAttr = Utilities.GetAbsolutePath("~/Images/gardify_Pflanzenbild_Platzhalter.svg"),
                        Id = 0,
                        TitleAttr = "Kein Bild vorhanden"
                    });
                }
                res.Add(temp);
            }
            var orderedPlantsInRes = res.OrderBy(x => x.NameLatin.Contains("[k]") ? x.NameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.NameLatin);

            return orderedPlantsInRes;
        }

        // GET: api/PlantSearchAPI
        [System.Web.Http.Route("api/PlantSearchAPI")]
        [ResponseType(typeof(SearchQuery))]
        public PlantSearchViewModel GetSearchResult(
            string searchText = "",
            int selectedcategoryId = -1,
            string cookieTags = "",
            string ecosTags = "",
            int selHmin = 0,
            int selHMax = 800,
            int selMaxMonth = 12,
            int selMinMonth = 1,
            int? groupId = null,
            int skip = 0,
            int take = int.MaxValue,
            string freezes = "",
            string family = "",
            int? gardenGroup = null,
            string colors = "",
            string leafColors = "",
            string autumnColors = "",
            string sortTags= "3",
            string excludes = "")
        {
            PlantSearchController pc = new PlantSearchController();
            var input = pc.Index(searchText, selectedcategoryId, cookieTags, ecosTags,selHmin, selHMax, selMaxMonth, selMinMonth, groupId, skip, take, freezes, colors, excludes, leafColors, autumnColors, family, gardenGroup);
            //var orderedPlantllist = input.PlantList.OrderBy(x => x.NameLatin.Replace("[k]", "")).ToList();
            var orderedPlantllist = input.PlantList.OrderBy(x => x.NameLatin.Contains("[k]") ? x.NameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.NameLatin);
            
            if (sortTags=="2")
            {
               orderedPlantllist = input.PlantList.OrderBy(x => x.CreatedDate);

            }
            if (sortTags == "1")
            {
                orderedPlantllist = input.PlantList.OrderByDescending(x => x.CreatedDate);

            }
            if (sortTags == "4")
            {
                orderedPlantllist = input.PlantList.OrderByDescending(x => x.NameLatin.Contains("[k]") ? x.NameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.NameLatin);

            }
            if (sortTags == "3")
            {
                orderedPlantllist = input.PlantList.OrderBy(x => x.NameLatin.Contains("[k]") ? x.NameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.NameLatin);

            }
            if(orderedPlantllist!=null && orderedPlantllist.Any())
            {
                input.Plants = pc.GetPlantViewModelLiteTodo(orderedPlantllist, Utilities.GetAbsolutePath("~/"), skip, take);

            }
            input.PlantList = null;
            if (!string.IsNullOrEmpty(searchText) && input.Plants!=null && input.Plants.Any())
            {
                input.ResultSortedByInput = input.Plants.OrderByDescending(name => getSimilarityScore(name, name.NameGerman, searchText)).Select(n => n);

            }
            else
            {
                input.ResultSortedByInput = input.Plants;
            }

            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            List<PlantViewModels.PlantViewModelLiteTodo> temp2 = new List<PlantViewModels.PlantViewModelLiteTodo>();

            // check user plants to see which plants he already has
            List<int> userPlants = new List<int>();
            var userId = Utilities.GetUserId();
            if (userId != Guid.Empty)
            {
                var userGarden = db.Property.Where(p => p.UserId == userId && !p.Deleted).FirstOrDefault().Gardens.FirstOrDefault();
                if (userGarden != null)
                {
                    userPlants = db.UserPlants.Where(up => up.Gardenid == userGarden.Id && !up.Deleted)
                                                .Select(p => p.PlantId).Distinct().ToList();
                }
            }
            if (input.Plants == null)
            {
                return input;
            }

            if (input.Plants.Count() == 0)
            {
                return input;
            }

            foreach (PlantViewModels.PlantViewModelLiteTodo mod in input.Plants)
            {
                PlantViewModels.PlantViewModelLiteTodo temp = new PlantViewModels.PlantViewModelLiteTodo();
                var props = pc.GetPlantColorsAndBadgesById(mod.Id);
                temp.NameGerman = mod.NameGerman;
                temp.Id = mod.Id;
                temp.IsInUserGarden = userPlants.Contains(mod.Id);
                temp.Description = mod.Description;
                temp.NameLatin = mod.NameLatin;
                temp.Images = new List<_HtmlImageViewModel>();
                temp.Todos = mod.Todos;
                temp.Synonym = mod.Synonym;
                temp.Colors = props.Colors;
                temp.Badges = props.Badges;

                HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages(mod.Id);
                if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                {
                    temp.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                }
                else
                {
                    temp.Images.Add(new _HtmlImageViewModel
                    {
                        SrcAttr = Utilities.GetAbsolutePath("~/Images/gardify_Pflanzenbild_Platzhalter.svg"),
                        Id = 0,
                        TitleAttr = "Kein Bild vorhanden"
                    });
                }
                temp2.Add(temp);
            }

            input.Plants = temp2;
            return input;
        }
        double getSimilarityScore(PlantViewModelLiteTodo plant,string name, string searchString)
        {
            var myplant = plant;
            double similarity = 0;
            if (name != null)
            {
                if (plant.NameGerman!=null && plant.NameGerman.Contains(searchString))
                {
                    similarity=similarity+( (double)searchString.Length / (double)plant.NameGerman.Length);
                }
                if (plant.NameLatin!=null && plant.NameLatin.Contains(searchString))
                {
                    similarity = similarity + ((double)searchString.Length / (double)plant.NameLatin.Length);
                }
                if (plant.Synonym!=null && plant.Synonym.Contains(searchString))
                {
                    similarity = similarity + ((double)searchString.Length / (double)plant.NameLatin.Length);
                }
                return similarity;
            }

            return 0;
        }
        // GET: api/PlantSearchAPI
        [System.Web.Http.Route("api/PlantSearchAPI/getall")]
        [ResponseType(typeof(SearchQuery))]
        public PlantSearchViewModel GetAllPlantResult(
            int skip = 0,
            int take = int.MaxValue)
        {
            PlantSearchController pc = new PlantSearchController();
            var input = pc.GetSearchViewModelLite();
            //var orderedPlantllist = input.PlantList.OrderBy(x => x.NameLatin.Replace("[k]", "")).ToList();
            var orderedPlantllist = input.PlantList.OrderBy(x => x.NameLatin.Contains("[k]") ? x.NameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.NameLatin).ToList();
            input.Plants = pc.GetPlantViewModelLiteTodo(orderedPlantllist, Utilities.GetAbsolutePath("~/"), skip, take);
            input.PlantList = null;
            List<PlantViewModels.PlantViewModelLiteTodo> temp2 = new List<PlantViewModels.PlantViewModelLiteTodo>();



            foreach (PlantViewModels.PlantViewModelLiteTodo mod in input.Plants)
            {
                PlantViewModels.PlantViewModelLiteTodo temp = new PlantViewModels.PlantViewModelLiteTodo();
                temp.NameGerman = mod.NameGerman;
                temp.Id = mod.Id;
     
                temp.NameLatin = mod.NameLatin;
 
                temp2.Add(temp);
            }

            input.Plants = temp2;
            return input;
        }

        [System.Web.Http.Route("api/PlantSearchAPI/getplantwithname")]
        [ResponseType(typeof(SearchQuery))]
        public List<string> GetAllPlantNameResult(string plantName)
        {
            PlantSearchController pc = new PlantSearchController();
            var input = pc.GetSearchNameList(plantName);
            //var orderedPlantllist = input.PlantList.OrderBy(x => x.NameLatin.Replace("[k]", "")).ToList();

            return input;
        }

        [System.Web.Http.Route("api/PlantSearchAPI/count")]
        [ResponseType(typeof(int))]
        public int GetSearchResultCount(string searchText = "", int selectedcategoryId = -1, string cookieTags = "", string ecosTags = "", int selHmin = 0, int selHMax = 800, int selMaxMonth = 12, int selMinMonth = 1, int? groupId = null, int skip = 0, int take = int.MaxValue,string family = "", string freezes = "", string colors = "", string excludes = "", string leafColors = "", string autumnColors = "", int? gardenGroup = null)
        {
            //var userId = Utilities.GetUserId();
            PlantSearchController psc = new PlantSearchController();
            var input = psc.Index(searchText, selectedcategoryId, cookieTags, ecosTags, selHmin, selHMax, selMaxMonth, selMinMonth, groupId, skip, take, freezes, colors, excludes, leafColors, autumnColors, family, gardenGroup);

            if (input.PlantProperties == null){
                return 0;
            }

            return input.PlantProperties.Count();


            //return (input.PlantList != null) ? input.PlantList.AsQueryable().Count() : 0;
        }

        public PlantSearchViewModel SearchResultLite(string searchText, int selectedcategoryId = -1)
        {
            PlantSearchController pc = new PlantSearchController();
            var input = pc.Index(searchText);

            //PlantViewModels.PlantViewModel temp = new PlantViewModels.PlantViewModel();
            List<PlantViewModels.PlantViewModelLiteTodo> temp2 = new List<PlantViewModels.PlantViewModelLiteTodo>();
            PlantSearchViewModel ret = new PlantSearchViewModel();
            ret = input;
            ret.TagsList = null;
            ret.CategoryList = null;
            ret.SubCategoryList = null;
            ret.PositiveFilterTagsList = null;
            if (input != null && input.Plants != null)
            {
                foreach (PlantViewModels.PlantViewModelLiteTodo mod in input.Plants)
                {
                    PlantViewModels.PlantViewModelLiteTodo temp = new PlantViewModels.PlantViewModelLiteTodo();
                    temp.NameGerman = mod.NameGerman;
                    temp.Id = mod.Id;
                    temp.NameLatin = mod.NameLatin;
                    temp.Description = mod.Description;

                    temp2.Add(temp);
                }
                ret.Plants = temp2;
            }
            return ret;
        }


        // DELETE: api/PlantSearchAPI/5
        [ResponseType(typeof(SearchQuery))]
        public IHttpActionResult DeleteSearchQuery(int id)
        {
            SearchQuery searchQuery = db.SearchQueries.Find(id);
            if (searchQuery == null)
            {
                return NotFound();
            }

            db.SearchQueries.Remove(searchQuery);
            db.SaveChanges();

            return Ok(searchQuery);
        }

        

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool SearchQueryExists(int id)
        {
            return db.SearchQueries.Count(e => e.Id == id) > 0;
        }

        public class PlantsCount
        {
            public int Count { get; set; }
        }
    }
}