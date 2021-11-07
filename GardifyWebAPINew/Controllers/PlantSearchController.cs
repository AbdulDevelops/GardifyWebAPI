using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static GardifyModels.Models.ModelEnums;
using static GardifyModels.Models.PlantViewModels;
using System.Web.UI.WebControls.WebParts;
using System.Data.SqlClient;
using System.Text;

namespace GardifyWebAPI.Controllers
{
    public class PlantSearchController : _BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        PlantController pc = new PlantController();


        // GET: PlantSearch
        [AcceptVerbs("GET")]
        public PlantSearchViewModel Index(string searchtext = "", int selectedcategoryId = -1, string cookieTags = "", string ecosTags="", int selHMin = 0, int selHMax = 800, int selMaxMonth = 12, int selMinMonth = 1, int? groupId = null, int skip = 0, int take = int.MaxValue, string freezes = "", string colors = "", string excludes = "", string leafColors = "", string autumnColors = "", string family = "", int? gardenGroup = null)
      {
            return getSearchViewModel(searchText: searchtext, selectedCategoryId: selectedcategoryId, cookieTags: cookieTags, ecosTags: ecosTags, cookieSelHeightMin: selHMin, cookieSelHeightMax: selHMax, cookieSelMaxMonth: selMaxMonth, cookieSelMinMonth: selMinMonth, groupId: groupId, skip: skip, take: take, freezes: freezes, colorsStr: colors, excludesStr: excludes, leafColorsStr: leafColors, autumnColorsStr: autumnColors, family: family, gardenGroup: gardenGroup);
        }

        //Get:PlantImage

        public void GetPlantimage(int id)
        {
            FileSystemObject file = new FileSystemObject();


            //plant.NameLatin.Replace("[k]", "").Replace("[/k]", "");
            var result = from plant in plantDB.Plants where (id == plant.Id) join files in plantDB.FileSystemObject on plant.NameLatin.Replace("[k]", "").Replace("[/k]", "") equals files.Title select new { files.FileName };



        }
        public ActionResult Autocomplete(string term)
        {
            try
            {
                var german = (from pl in plantDB.Plants
                              where (pl.NameGerman.Contains(term))
                              && !pl.Deleted && pl.Published
                              select new { label = pl.NameGerman });
                var latin = (from pl in plantDB.Plants
                             where (pl.NameLatin.Contains(term))
                             && !pl.Deleted && pl.Published
                             select new { label = pl.NameLatin });
                var plants = pc.DbGetPublishedPlantList().ToList();
                var syn = (from pl in plantDB.Synonym
                           where (pl.Text.Contains(term))
                           && !pl.Deleted
                           select new { label = pl.Text });
                var result = german.Concat(latin).Concat(syn);
                result = result.Take(10);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("{'ex':'Exception'}");
            }
        }

        [NonAction]
        public List<PlantViewModels.PlantViewModel> plantsToPlantViewModels(IEnumerable<Plant> plants, string rootPath, int skip = 0, int take = int.MaxValue)
        {
            PlantCharacteristicController pcc = new PlantCharacteristicController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            List<PlantViewModels.PlantViewModel> viewModels = new List<PlantViewModels.PlantViewModel>();

            if (plants != null && plants.Any())
            {
                var todoTemplates = DbGetTodoTemplates(ModelEnums.ReferenceToModelClass.Plant);

                foreach (Plant plant in plants.Skip(skip).Take(take))
                {
                    PlantViewModels.PlantViewModel plantView = new PlantViewModels.PlantViewModel
                    {
                        Id = plant.Id,
                        NameLatin = plant.NameLatin,
                        NameGerman = plant.NameGerman,
                        Description = plant.Description,
                    };

                    // get images to plants
                    HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages(plant.Id);
                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                    {
                        plantView.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, rootPath);
                    }
                    else
                    {

                        plantView.Images.Add(new _HtmlImageViewModel
                        {
                            SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                            Id = 0,
                            TitleAttr = "Kein Bild vorhanden"
                        });
                    }

                    viewModels.Add(plantView);
                }
            }
            return viewModels;
        }

        public List<PlantViewModelLiteTodo> GetPlantViewModelLiteTodo(IEnumerable<Plant> plants, string rootPath, int skip = 0, int take = int.MaxValue)
        {
            PlantCharacteristicController pcc = new PlantCharacteristicController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            List<PlantViewModels.PlantViewModelLiteTodo> viewModels = new List<PlantViewModels.PlantViewModelLiteTodo>();

            //if (skip < 0)
            //{
            //    skip = 0;
            //}
            //var plantsList = plants.Skip(skip).Take(take).ToList();

          
            var plantsList = plants;

            var imagePlaceholder = new _HtmlImageViewModel
            {
                SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                Id = 0,
                TitleAttr = "Kein Bild vorhanden"
            };



            foreach (Plant plant in plantsList)
            {
                PlantViewModels.PlantViewModelLiteTodo plantView = new PlantViewModels.PlantViewModelLiteTodo
                {
                    Id = plant.Id,
                    NameLatin = plant.NameLatin,
                    NameGerman = plant.NameGerman,
                    Description = plant.Description,
                    Synonym = plant.Synonym,
                    Images = new List<_HtmlImageViewModel>()
                };
                //plantView.Todos = todoTemplates.Where(t => t.ReferenceId == plant.Id).ToList();

                // get images to plants
                HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages(plant.Id);
                if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                {
                    plantView.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, rootPath);
                }
                else
                {

                    plantView.Images.Add(imagePlaceholder);
                }

                viewModels.Add(plantView);
            }
            //}
            return viewModels;
        }

        private IEnumerable<Plant> sortOutPlantsByCharacteristics(IEnumerable<Plant> plantList, IEnumerable<PlantCharacteristic> characteristicList)
        {
            if (plantList != null && plantList.Any() && characteristicList != null && characteristicList.Any())
            {
                plantList = plantList.Where(p => characteristicList.All(c => p.PlantCharacteristics.Where(r => 
                r.CategoryId == c.CategoryId && 
                ((r.Min <= c.Min && r.Max >= c.Max) ||
                (r.Min <= c.Min && r.Max >= c.Min) ||
                (r.Max >= c.Max && r.Min <= c.Max)
                ))
                .Any())).ToList();
            }
            return plantList;
        }

        private IEnumerable<PlantTagCategory> getPossiblePlantTagCategoriesByPlantTags(IEnumerable<Plant> plantsToCheck, IEnumerable<PlantTag> positiveFilterTags)
        {
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
            if ( positiveFilterTags != null && positiveFilterTags.Any())
            {
                //var tags = plantsToCheck.SelectMany(p => p.PlantTags).DistinctBy(t => t.Id);
                //var categoryIds = tags.Join(positiveFilterTags, planttag => planttag.Id, postag => postag.Id, (plat, post) => plat.CategoryId);
                //var categories = ptcc.DbGetPlantTagCategories().Join(categoryIds, t => t.Id, p => p, (t, p) => t);
                var categoryIds = positiveFilterTags.Select(t => t.CategoryId);
                var categories = ptcc.DbGetPlantTagCategories().Join(categoryIds, t => t.Id, p => p, (t, p) => t);
                return categories;
            }

            List<PlantTagCategory> ret = new List<PlantTagCategory>();
            //if (positiveFilterTags != null && positiveFilterTags.Any())
            //{
            //    if (plantsToCheck != null)
            //    {
            //        var plantTagCategories = ptcc.DbGetPlantTagCategories().ToList();
            //        foreach (var filter in positiveFilterTags)
            //        {

            //        }
            //        foreach (Plant plant in plantsToCheck)
            //        {
            //            if (plant.PlantTags != null)
            //            {
            //                foreach (PlantTag tag in plant.PlantTags)
            //                {
            //                    // exclude tags that are already in filter list and tags with values
            //                    if (positiveFilterTags != null && !positiveFilterTags.Where(t => t.Id == tag.Id).Any())
            //                    {
            //                        PlantTagCategory cat = ret.Find(c => c.Id == tag.CategoryId);

            //                        if (cat == null)
            //                        {
            //                            cat = new PlantTagCategory();
            //                            PlantTagCategory tmp = plantTagCategories.Where(p => p.Id == tag.CategoryId).FirstOrDefault();
            //                            cat.Id = tmp.Id;
            //                            cat.ParentId = tmp.ParentId;
            //                            cat.Title = tmp.Title;
            //                            cat.TagsInThisCategory = new List<PlantTag>();
            //                            cat.TagsInThisCategory.Add(tag);
            //                            ret.Add(cat);
            //                        }
            //                        else if (cat.TagsInThisCategory.Where(t => t.Id == tag.Id) == null || !cat.TagsInThisCategory.Where(t => t.Id == tag.Id).Any())
            //                        {
            //                            cat.TagsInThisCategory.Add(tag);
            //                        }
            //                        else
            //                        {
            //                            // tag is already in that category. do nothing
            //                        }
            //                    }
            //                }
            //            }
            //        }

            //        if (ret != null)
            //        {
            //            ret = ret.OrderBy(c => c.Title).ToList();
            //        }
            //    }
            //}
            return ret;
        }

        private IEnumerable<PlantTagCategory> getPossibleEmptyPlantTagCategoriesByPlantTags(IEnumerable<Plant> plantsToCheck, IEnumerable<PlantTag> positiveFilterTags)
        {
            PlantTagCategoryController ptcc = new PlantTagCategoryController();

            List<PlantTagCategory> ret = new List<PlantTagCategory>();
            if (positiveFilterTags != null && positiveFilterTags.Any())
            {
                if (plantsToCheck != null)
                {
                    var plantTagCategories = ptcc.DbGetPlantTagCategories().ToList();
                    foreach (var filter in positiveFilterTags)
                    {

                    }
                    foreach (Plant plant in plantsToCheck)
                    {
                        if (plant.PlantTags != null)
                        {
                            foreach (PlantTag tag in plant.PlantTags)
                            {
                                // exclude tags that are already in filter list and tags with values
                                if (positiveFilterTags != null && !positiveFilterTags.Where(t => t.Id == tag.Id).Any())
                                {
                                    PlantTagCategory cat = ret.Find(c => c.Id == tag.CategoryId);

                                    if (cat == null)
                                    {
                                        cat = new PlantTagCategory();
                                        PlantTagCategory tmp = plantTagCategories.Where(p => p.Id == tag.CategoryId).FirstOrDefault();
                                        cat.Id = tmp.Id;
                                        cat.ParentId = tmp.ParentId;
                                        cat.Title = tmp.Title;
                                        cat.TagsInThisCategory = new List<PlantTag>();
                                        cat.TagsInThisCategory.Add(tag);
                                        ret.Add(cat);
                                    }
                                    else if (cat.TagsInThisCategory.Where(t => t.Id == tag.Id) == null || !cat.TagsInThisCategory.Where(t => t.Id == tag.Id).Any())
                                    {
                                        cat.TagsInThisCategory.Add(tag);
                                    }
                                    else
                                    {
                                        // tag is already in that category. do nothing
                                    }
                                }
                            }
                        }
                    }

                    if (ret != null)
                    {
                        ret = ret.OrderBy(c => c.Title).ToList();
                    }
                }
            }
            return ret;
        }

        private IEnumerable<PlantTagCategory> getPossiblePlantTagCategoriesByPlantTagsOld(IEnumerable<Plant> plantsToCheck, IEnumerable<PlantTag> positiveFilterTags)
        {
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
            if (plantsToCheck != null && plantsToCheck.Any() && positiveFilterTags != null && positiveFilterTags.Any())
            {
                var tags = plantsToCheck.SelectMany(p => p.PlantTags).DistinctBy(t => t.Id);
                var categoryIds = tags.Join(positiveFilterTags, planttag => planttag.Id, postag => postag.Id, (plat, post) => plat.CategoryId);
                var categories = ptcc.DbGetPlantTagCategories().Join(categoryIds, t => t.Id, p => p, (t, p) => t);
                return categories;
            }

            List<PlantTagCategory> ret = new List<PlantTagCategory>();
            if (positiveFilterTags != null && positiveFilterTags.Any())
            {
                if (plantsToCheck != null)
                {
                    var plantTagCategories = ptcc.DbGetPlantTagCategories().ToList();
                    foreach (var filter in positiveFilterTags)
                    {

                    }
                    foreach (Plant plant in plantsToCheck)
                    {
                        if (plant.PlantTags != null)
                        {
                            foreach (PlantTag tag in plant.PlantTags)
                            {
                                // exclude tags that are already in filter list and tags with values
                                if (positiveFilterTags != null && !positiveFilterTags.Where(t => t.Id == tag.Id).Any())
                                {
                                    PlantTagCategory cat = ret.Find(c => c.Id == tag.CategoryId);

                                    if (cat == null)
                                    {
                                        cat = new PlantTagCategory();
                                        PlantTagCategory tmp = plantTagCategories.Where(p => p.Id == tag.CategoryId).FirstOrDefault();
                                        cat.Id = tmp.Id;
                                        cat.ParentId = tmp.ParentId;
                                        cat.Title = tmp.Title;
                                        cat.TagsInThisCategory = new List<PlantTag>();
                                        cat.TagsInThisCategory.Add(tag);
                                        ret.Add(cat);
                                    }
                                    else if (cat.TagsInThisCategory.Where(t => t.Id == tag.Id) == null || !cat.TagsInThisCategory.Where(t => t.Id == tag.Id).Any())
                                    {
                                        cat.TagsInThisCategory.Add(tag);
                                    }
                                    else
                                    {
                                        // tag is already in that category. do nothing
                                    }
                                }
                            }
                        }
                    }

                    if (ret != null)
                    {
                        ret = ret.OrderBy(c => c.Title).ToList();
                    }
                }
            }
            return ret;
        }




        private List<PlantTagCategory> sortPossibleCategoriesForView(IEnumerable<PlantTagCategory> categories)
        {
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
            List<PlantTagCategory> ret = new List<PlantTagCategory>();
            var plantTagCategories = ptcc.DbGetPlantTagCategories();

            foreach (PlantTagCategory cat in categories)
            {
                if (cat.ParentId > 0)
                {
                    if (ret.Find(c => c.Id == cat.ParentId) != null)
                    {
                        ret.Find(c => c.Id == cat.ParentId).Childs.Add(cat);
                    }
                    else
                    {
                        // search for parent cat in input list						
                        if (categories.FirstOrDefault(c => c.Id == cat.ParentId) != null)
                        {
                            PlantTagCategory parentCat = categories.FirstOrDefault(c => c.Id == cat.ParentId);
                            parentCat.Childs = new List<PlantTagCategory>();
                            parentCat.Childs.Add(cat);
                            ret.Add(parentCat);
                            // create new parent cat
                        }
                        else
                        {
                            PlantTagCategory tmp = plantTagCategories.FirstOrDefault(c => c.Id == (int)cat.ParentId);
                            PlantTagCategory parentCat = new PlantTagCategory();
                            parentCat.Id = tmp.Id;
                            parentCat.Title = tmp.Title;
                            parentCat.Childs = new List<PlantTagCategory>();
                            parentCat.Childs.Add(cat);
                            ret.Add(parentCat);
                        }
                    }
                }
                else if (ret.Find(c => c.Id == cat.Id) == null)
                {
                    cat.Childs = new List<PlantTagCategory>();
                    ret.Add(cat);
                }
            }

            return ret;
        }

        public IEnumerable<PlantTag> getPositiveFilterTags(string cookieTags)
        {
            PlantTagController ptc = new PlantTagController();
            List<int> tagIds = new List<int>();
            if (!String.IsNullOrEmpty(cookieTags))
            {
                string[] ids_str = cookieTags.Split(',');

                for (int i = 0; i < ids_str.Length; i++)
                {
                    int tagId = -1;
                    if (int.TryParse(ids_str[i], out tagId))
                    {
                        tagIds.Add(tagId);

            
                    }
                }

                //var sql = "SELECT dbo.PlantTags.id, dbo.PlantTagCategories.title, dbo.PlantTags.title AS tagTitle FROM dbo.PlantTags INNER JOIN dbo.PlantTagCategories ON dbo.PlantTags.categoryId = dbo.PlantTagCategories.id WHERE dbo.PlantTags.id IN (" + string.Join(",", tagIds.Select(t => "'" + t.ToString() + "'").ToArray()) + ")";



                //var plantTags = db.Database.SqlQuery<PlantTagView>(sql).Select(p => new PlantTag { Id = p.id, Title = p.tagTitle, Category = new PlantTagCategory { Title = p.title } }).ToList();

                var plantTags = tagIds.Select(t => new PlantTag { Id = t }).ToList();

                //var plantTags = plantDB.PlantTags.Include("PlantTagCategories").Where(p => !p.Deleted).Join(tagIds, t => t.Id, i => i, (tag, p) => tag).ToList();


                //foreach (var tag in plantTags)
                //{
                //    tag.Title = tag.Category.Title + ":" + tag.Title;
                //}
                //plantTags.OrderBy(t => t.Title);

                return plantTags;
            }
            return new List<PlantTag>();
        }

        public List<int> getTagList(string cookieTags)
        {
            List<int> tagIds = new List<int>();
            if (!String.IsNullOrEmpty(cookieTags))
            {
                string[] ids_str = cookieTags.Split(',');

                for (int i = 0; i < ids_str.Length; i++)
                {
                    int tagId = -1;
                    if (int.TryParse(ids_str[i], out tagId))
                    {
                        tagIds.Add(tagId);
                    }
                }
            }
            return tagIds;
        }

        public IEnumerable<Plant> sortOutPlantsByPositiveTagList(IEnumerable<Plant> plants, List<int> positiveList, List<int> freezeLvls = null, List<int> excludes = null, List<int> colors = null, List<int> leafColors = null)
        {
            if (plants == null || !plants.Any())
            {
                return plants.ToList();
            }

            if ((positiveList == null && freezeLvls == null && excludes == null && colors == null && leafColors == null) ||
                (!positiveList.Any() && !freezeLvls.Any() && !excludes.Any()) && !colors.Any() && !leafColors.Any())
            {
                return plants;
            }

            IEnumerable<Plant> ret = plants;

            if (positiveList != null)
            {
                foreach (var tagId in positiveList)
                {
                    var plant_sel = ret.Where(p => p.PlantTags.Where(t => t.Id == tagId).Any());

                    if (plant_sel == null || !plant_sel.Any())
                    {
                        return null;
                    }
                    ret = plant_sel;
                }
            }

            if (freezeLvls != null && freezeLvls.Count() > 0)
            {


                ret = ret.Where(p => p.PlantTags.Where(t => freezeLvls.Contains(t.Id)).Any());
            }




            if (excludes != null && excludes.Count() > 0)
            {
                ret = ret.Where(p => p.PlantTags.All(t => !excludes.Contains(t.Id)));
            }
            if (colors != null && colors.Any())
            {
                ret = ret.Where(p => p.PlantTags.Where(t => colors.Contains(t.Id)).Any());
            }
            if (leafColors != null && leafColors.Any())
            {
                ret = ret.Where(p => p.PlantTags.Where(t => leafColors.Contains(t.Id)).Any());
            }
            return ret;
        }

        [AcceptVerbs("POST", "GET")]
        [ActionName("reset-search")]
        public ActionResult ResetSearch()
        {
            resetFilterCookie(this.ControllerContext);
            return RedirectToAction("Index");
        }

        public void resetFilterCookie(ControllerContext context)
        {
            HttpCookie cookie = null;
            if (context.HttpContext.Request.Cookies.AllKeys.Contains("filter"))
            {
                cookie = context.HttpContext.Request.Cookies["filter"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                context.HttpContext.Response.Cookies.Add(cookie);
            }
        }

        public IEnumerable<TodoTemplate> DbGetTodoTemplates(ReferenceToModelClass type)
        {
            return plantDB.TodoTemplate.Where(t => !t.Deleted && t.ReferenceType == type);
        }

        public IEnumerable<TodoTemplate> DbGetTodoTemplates(int referenceId, ReferenceToModelClass type)
        {
            var templates = (from t in plantDB.TodoTemplate
                             where !t.Deleted
                             && t.ReferenceId == referenceId && t.ReferenceType == type
                             select t);
            return templates;
        }

        [AllowAnonymous]
        public PlantDetailsViewModelLite PlantDetails(int? id)
        {
            PlantController pc = new PlantController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            PlantCharacteristicController pcc = new PlantCharacteristicController();
            LastViewedController lvc = new LastViewedController();
            TaxonomicTreeController ttc = new TaxonomicTreeController();

            if (id != null)
            {
                Plant plant = pc.DbGetPlantPublishedById((int)id);

                if (plant != null)
                {
                    TaxonomicTree genusTaxon = ttc.DbGetGenusTaxonPlantId(plant.Id);
                    ArticleController ac = new ArticleController();
                    var todoTemplates = DbGetTodoTemplates(plant.Id, ModelEnums.ReferenceToModelClass.Plant);
                    PlantDetailsViewModelLite plantView = new PlantDetailsViewModelLite
                    {
                        Id = plant.Id,
                        NameLatin = plant.NameLatin,
                        NameGerman = plant.NameGerman,
                        Description = plant.Description,
                        PlantTags = plant.PlantTags.Select(t => new PlantTagLite { 
                            Id = t.Id,
                            CategoryId = t.CategoryId,
                            Title = t.Title,
                            TagImage = t.TagImage
                        }),
                        GenusTaxon = genusTaxon,
                        Synonym = plant.Synonym,
                        Family = plant.Familie,
                        Herkunft = plant.Herkunft,
                        TodoTemplates = todoTemplates,
                        PlantGroups = plant.PlantGroups.Select(p => new GroupSimplified(p)),
                        Articles = ac.GetObjectArticles(ModelEnums.ArticleReferenceType.Plant, plant.Id),
                        Colors = GetPlantColorsAndBadgesById(plant.Id).Colors
                    };

                    HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages(plant.Id);

                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                    {
                        plantView.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                    }
                    else
                    {
                        plantView.Images.Add(new _HtmlImageViewModel
                        {
                            SrcAttr = Utilities.GetAbsolutePath("~/Images/gardify_Pflanzenbild_Platzhalter.svg"),
                            Id = 0,
                            TitleAttr = "Kein Bild vorhanden"
                        });
                    }
                    var plantCharacteristics = pcc.DbGetPlantCharacteristicsByPlantId(plant.Id);
                    plantView.PlantCharacteristics = plantCharacteristics.Select(p => new PlantCharacteristicSimplified(p));
                //.Exclude(g => g.ParentCategory).Exclude(g => g.Plants)
                    if (plant.GardenCategoryId!=null)
                    {
                        var gardenCategoryData = plantDB.GardenCategories.FirstOrDefault(c => c.Id == plant.GardenCategoryId);

                        plantView.GardenCategory = new GardenCategory
                        {
                            Name = gardenCategoryData.Name,
                            Id = gardenCategoryData.Id,
                            ParentId = gardenCategoryData.ParentId
                        };
                    }

                    plantView.IsInUserGarden = false;


                  
                        UserPlantController upc = new UserPlantController();

                        plantView.IsInUserGarden = upc.CheckPlantIfInGarden(plant.Id);



                    return plantView;
                }
            }
            throw new Exception("Seite konnte nicht gefunden werden.");//, HttpStatusCode.NotFound, "PlantSearchController.PlantDetails(" + id + ")");
        }

        [AllowAnonymous]
        public PlantDetailsViewModelLite PlantDetailsLite(int? id)
        {
            PlantController pc = new PlantController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            if (id != null)
            {
                Plant plant = pc.DbGetPlantPublishedById((int)id);

                if (plant != null)
                {
                    //TaxonomicTree genusTaxon = ttc.DbGetGenusTaxonPlantId(plant.Id);
                    ArticleController ac = new ArticleController();
                    //var todoTemplates = DbGetTodoTemplates(plant.Id, ModelEnums.ReferenceToModelClass.Plant);
                    PlantDetailsViewModelLite plantView = new PlantDetailsViewModelLite
                    {
                        Id = plant.Id,
                        NameLatin = plant.NameLatin,
                        NameGerman = plant.NameGerman,
                        Description = plant.Description
                        //GenusTaxon = genusTaxon,
                        //Synonym = plant.Synonym,
                        //Family = plant.Familie,
                        //Herkunft = plant.Herkunft,
                        //TodoTemplates = todoTemplates,
                        //PlantGroups = plant.PlantGroups.Select(p => new GroupSimplified(p)),
                        //Articles = ac.GetObjectArticles(ModelEnums.ArticleReferenceType.Plant, plant.Id),
                        //Colors = GetPlantColorsAndBadgesById(plant.Id).Colors
                    };

                    HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages(plant.Id);

                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                    {
                        plantView.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                    }
                    else
                    {
                        plantView.Images.Add(new _HtmlImageViewModel
                        {
                            SrcAttr = Utilities.GetAbsolutePath("~/Images/gardify_Pflanzenbild_Platzhalter.svg"),
                            Id = 0,
                            TitleAttr = "Kein Bild vorhanden"
                        });
                    }
                    //var plantCharacteristics = pcc.DbGetPlantCharacteristicsByPlantId(plant.Id);
                    //plantView.PlantCharacteristics = plantCharacteristics.Select(p => new PlantCharacteristicSimplified(p));

                    //if (plant.GardenCategoryId != null)
                    //{
                    //    plantView.GardenCategory = plantDB.GardenCategories.FirstOrDefault(c => c.Id == plant.GardenCategoryId);
                    //}

                    plantView.IsInUserGarden = false;

                    return plantView;
                }
            }
            throw new Exception("Seite konnte nicht gefunden werden.");//, HttpStatusCode.NotFound, "PlantSearchController.PlantDetails(" + id + ")");
        }

        public int[] getEcoTagsList()
        {
            return new int[] { 320, 321, 322, 445, 447, 531, 530 };
        }

        public int[] getPlantMonthCharacteristic(int plantId)
        {
            var category = db.PlantCharacteristic.FirstOrDefault(pc => pc.PlantId == plantId && !pc.Deleted && pc.CategoryId == 22);

            if (category != null)
            {
                return new int[] { (int)category.Min, (int)category.Max };
            }

            return new int[] { 0, 0 };

        }

        public List<int[]> getMultiplePlantMonthCharacteristic(List<int> plantIds)
        {
            var category1 = db.PlantCharacteristic.Where(pc => !pc.Deleted && pc.CategoryId == 22 && plantIds.Contains(pc.PlantId) && pc.Max != null && pc.Min != null).ToList();
            var category = new List<int[]>();
            if (category1.Any())
            {
                category = category1.Select(c => new int[] { (int)c.Min, (int)c.Max }).ToList();

            }
            return category;
  

        }

        public PlantSearchViewModel GetSearchViewModelLite()
        {
            var plants = pc.DbGetPublishedPlantList();

            PlantSearchViewModel searchViewModel = new PlantSearchViewModel
            {
                CategoryId = 0,
                SubCategoryId = 0,
                Input_search = "",
                //MonthCheckboxes = monthCheckboxes,
                PlantList = plants
            };

            return searchViewModel;
        }

        private PlantSearchViewModel getSearchViewModel(
            int taxonId = -1,
            string searchText = "",
            int selectedCategoryId = -1,
            int selectedSubCategoryId = -1,
            string cookieTags = "",
            string ecosTags="",
            int cookieSelHeightMin = 0,
            int cookieSelHeightMax = 800,
            int cookieSelMaxMonth = 12,
            int cookieSelMinMonth = 1,
            string selectedFloweringMonth_str = "", int? gardenId = null,
            int? groupId = null,
            int skip = 0,
            int take = int.MaxValue,
            string freezes = "",
            string colorsStr = "",
            string excludesStr = "",
            string leafColorsStr = "",
            string autumnColorsStr = "",
            string family = "",
            int? gardenGroup = null)
        {
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            PlantController pc = new PlantController();
            PlantCharacteristicCategoryController pccc = new PlantCharacteristicCategoryController();
            PlantCharacteristicController pcc = new PlantCharacteristicController();
            SearchQueryController sqc = new SearchQueryController();

            IEnumerable<PlantTag> freezeLvlsReal = getPositiveFilterTags(freezes);
            IEnumerable<PlantTag> colorsReal = getPositiveFilterTags(colorsStr);
            IEnumerable<PlantTag> leafColorsReal = getPositiveFilterTags(leafColorsStr);
            IEnumerable<PlantTag> autumnColorsReal = getPositiveFilterTags(autumnColorsStr);
            IEnumerable<PlantTag> excludesReal = getPositiveFilterTags(excludesStr);
            IEnumerable<PlantTag> positiveFilterTagsReal = getPositiveFilterTags(cookieTags);
            IEnumerable<PlantTag> positiveFilterEcoTagsReal = getPositiveFilterTags(ecosTags);

            //var searchQueryCode = "_co_" + (cookieTags != null ? string.Join("_", cookieTags.OrderBy(t => t)) : "") +
            //    "_eco_" + (ecosTags != null ?  ecosTags.Split(',').OrderBy(t => t) : "") +
            //    "_ex_" + (excludesStr != null ?  excludesStr.OrderBy(t => t) : "") +
            //    "_acl_" + (autumnColorsStr != null ?  autumnColorsStr.OrderBy(t => t) : "") +
            //    "_lcl_" + (leafColorsStr != null ?  leafColorsStr.OrderBy(t => t) : "") +
            //    "_cl_" + (colorsStr != null ?  colorsStr.OrderBy(t => t) : "") +
            //    "_fr_" + (freezes != null ?  freezes.OrderBy(t => t) : "");
            var searchQueryCode = "_co_" + (cookieTags != null ? string.Join("_", cookieTags.Split(',').OrderBy(t => t)) : "") +
                 "_eco_" + (ecosTags != null ? string.Join("_", ecosTags.Split(',').OrderBy(t => t)) : "") +
                  "_ex_" + (excludesStr != null ? string.Join("_", excludesStr.Split(',').OrderBy(t => t)) : "") +
                   "_acl_" + (autumnColorsStr != null ? string.Join("_", autumnColorsStr.Split(',').OrderBy(t => t)) : "") +
                    "_lcl_" + (leafColorsStr != null ? string.Join("_", leafColorsStr.Split(',').OrderBy(t => t)) : "") +
                     "_cl_" + (colorsStr != null ? string.Join("_", colorsStr.Split(',').OrderBy(t => t)) : "") +
                      "_fr_" + (freezes != null ? string.Join("_", freezes.Split(',').OrderBy(t => t)) : "");

            //foreach(var item in coo)

            IEnumerable<PlantTag> allTag = new List<PlantTag>();
            if (freezeLvlsReal.Any())
            {
                allTag = allTag.Concat(freezeLvlsReal);
            }
            if (colorsReal.Any())
            {
                allTag = allTag.Concat(colorsReal);
            }
            if (leafColorsReal.Any())
            {
                allTag = allTag.Concat(leafColorsReal);
            }
            if (autumnColorsReal.Any())
            {
                allTag = allTag.Concat(autumnColorsReal);
            }
            if (excludesReal.Any())
            {
                allTag = allTag.Concat(excludesReal);
            }
            if (positiveFilterTagsReal.Any())
            {
                allTag = allTag.Concat(positiveFilterTagsReal);
            }
            if (positiveFilterEcoTagsReal.Any())
            {
                allTag = allTag.Concat(positiveFilterEcoTagsReal);
            }

            var countvm = allTag.Select(t => new PlantTagCount
            {
                PlantTagId = t.Id,
                PlantTagIdentifier = t.CategoryId.ToString()
            });

            if (countvm.Any())
            {
                foreach(var vm in countvm)
                {
                    vm.OnCreate("system");
                    db.PlantTagCounts.Add(vm);
                }
               
                
                db.SaveChanges();
            }

            // keep track of applied filters when rebuilding seach form from a saved query
            List < AppliedFilterVM> appliedFilters = new List<AppliedFilterVM>();
            if (!string.IsNullOrEmpty(family))
            {
                appliedFilters.Add(new AppliedFilterVM() { pos = "family", t = family.Replace("[k]", "").Replace("[/k]", "").Trim() });
            }
            if (groupId != null)
            {
                var group = db.Groups.Find(groupId);
                appliedFilters.Add(new AppliedFilterVM() { pos = "groups", t = group.Name });
            }
            appliedFilters.AddRange(freezeLvlsReal.Select(f => new AppliedFilterVM() { pos = "freezeLevels", t = "Frosthärte" }));
            appliedFilters.AddRange(colorsReal.Select(f => new AppliedFilterVM() { pos = "colors", t = f.Title }));
            appliedFilters.AddRange(leafColorsReal.Select(f => new AppliedFilterVM() { pos = "leafClr", t = f.Title }));
            appliedFilters.AddRange(autumnColorsReal.Select(f => new AppliedFilterVM() { pos = "autumnClr", t = f.Title }));
            appliedFilters.AddRange(excludesReal.Select(f => new AppliedFilterVM() { pos = "exclusions", t = f.Title }));
            //appliedFilters.AddRange(positiveFilterTagsReal.Select(f => new AppliedFilterVM() { pos = TagPosition(f), t = f.Title }));
            //appliedFilters.AddRange(positiveFilterEcoTagsReal.Select(f => new AppliedFilterVM() { pos = TagPosition(f), t = f.Title }));
            appliedFilters.AddRange(positiveFilterTagsReal.Select(f => new AppliedFilterVM() { pos = "tags", t = f.Title }));
            appliedFilters.AddRange(positiveFilterEcoTagsReal.Select(f => new AppliedFilterVM() { pos = "ecoTags", t = f.Title }));
            
            //if (positiveFilterTagsReal.Any())
            //{
            //   if (positiveFilterTagsReal.Where(t => t.Id == 322).Any() && !positiveFilterTagsReal.Where(t => t.Id == 320).Any())
            //    {
            //        positiveFilterTagsReal = getPositiveFilterTags(cookieTags + ",320");
            //    }

            //}

            int[] orList = new int[] { };
            int[] andList = new int[] {320, 321, 322, 445, 447, 531, 346, 530};
            int[][] specialConnection = new int[1][]{ new int[]{ 320, 322 } };

            List<IEnumerable<PlantTag>> tagLists = new List<IEnumerable<PlantTag>>();
            tagLists.Add(freezeLvlsReal); tagLists.Add(colorsReal); tagLists.Add(leafColorsReal); tagLists.Add(autumnColorsReal);
            List<IEnumerable<PlantTag>> excludetagLists = new List<IEnumerable<PlantTag>>();
            excludetagLists.Add(excludesReal);


            var tags = tagLists.SelectMany(t => t);
            var extags = excludetagLists.SelectMany(t => t);
            // to view
            IEnumerable<Plant> plants = null;

            IEnumerable<PlantSearchPropertyView> plantProperty = null;
            //TaxonomicTree treeRoot = null;

            int selHmin = 0;
            int selHmax = 800;
            int selMaxMonth = 12;
            int selMinMonth = 1;

            List<PlantTagCategory> categoryList = new List<PlantTagCategory>();
            List<PlantTagCategory> subCategoryList = new List<PlantTagCategory>();
            IEnumerable<PlantTag> tagsList = null;

            GardenCategory selGardenGroup = null;

            var existingQuery = db.TempTableSearches.FirstOrDefault(t => t.SearchQuery == searchQueryCode);
            if (existingQuery == null || (searchQueryCode == "_co__eco__ex__acl__lcl__cl__fr_"))
            {
               

                var firstRequest = false;
                var initialRequest = "SELECT * FROM [pflanzenApp08].[dbo].[PlantSearchPropertyItems] ";
                if (positiveFilterTagsReal.Any())
                {
                    foreach (var tag in positiveFilterTagsReal)
                    {
                        var subRequest = "";
                        if (!firstRequest)
                        {
                            subRequest = "WHERE TagProperty LIKE '%" + tag.Id.ToString() + "%'";
                            firstRequest = true;
                        }
                        else
                        {
                            subRequest = "AND TagProperty LIKE '%" + tag.Id.ToString() + "%'";
                        }

                        initialRequest += subRequest;
                    }
                }
                if (positiveFilterEcoTagsReal.Any())
                {


                    var orTags = positiveFilterEcoTagsReal.Where(p => orList.Contains(p.Id));

                    var andTags = positiveFilterEcoTagsReal.Where(p => !orList.Contains(p.Id));

                    foreach (var tag in andTags)
                    {
                        var subRequest = "";
                        if (!firstRequest)
                        {
                            subRequest = "WHERE TagProperty LIKE '%" + tag.Id.ToString() + "%'";
                            firstRequest = true;
                        }
                        else
                        {
                            subRequest = "AND TagProperty LIKE '%" + tag.Id.ToString() + "%'";
                        }

                        initialRequest += subRequest;
                    }
                    var firstOr = false;

                    foreach (var tag in orTags)
                    {
                        var subRequest = "";
                        if (!firstRequest )
                        {
                            subRequest = "WHERE (TagProperty LIKE '%" + tag.Id.ToString() + "%'";
                            firstRequest = true;
                            firstOr = true;
                        }
                        else if (!firstOr)
                        {
                            subRequest = "AND (TagProperty LIKE '%" + tag.Id.ToString() + "%'";
                            firstOr = true;
                        }
                        else
                        {
                            subRequest = "OR TagProperty LIKE '%" + tag.Id.ToString() + "%'";
                        }

                        

                        initialRequest += subRequest;
                    }
                    if (firstOr)
                    {
                        initialRequest += ") ";
                    }

                }

                if (tags.Any() || extags.Any())
                {

                    foreach (var tag in tagLists)
                    {

                        var firstTagList = false;

                        var subRequest = "";

                        if (tag.Count() <= 0)
                        {
                            continue;
                        }

                        foreach(var itemTag in tag)
                        {


                            if (!firstRequest)
                            {
                                subRequest += " WHERE (TagProperty LIKE '%" + itemTag.Id.ToString() + "%'";
                                firstRequest = true;
                                firstTagList = true;
                            }
                            else if (!firstTagList)
                            {
                                subRequest += " AND (TagProperty LIKE '%" + itemTag.Id.ToString() + "%'";
                                firstTagList = true;
                            }
                            else
                            {
                                subRequest += " OR TagProperty LIKE '%" + itemTag.Id.ToString() + "%'";
                            }

                        }

                        if (firstTagList)
                        {
                            subRequest += ") ";
                        }
                        initialRequest += subRequest;


                        //plantProperty = plantProperty.Where(p => tag.Any(t => p.TagProperty.Contains(t.Id.ToString())));
                    }

                    foreach (var tag in excludetagLists)
                    {
                        if (tag.Count() > 0)
                        {
                            var subRequest = "";

                            foreach (var itemTag in tag)
                            {
                                if (!firstRequest)
                                {
                                    subRequest += " WHERE TagProperty NOT LIKE '%" + itemTag.Id.ToString() + "%'";
                                    firstRequest = true;
                                }
                                else
                                {
                                    subRequest += " AND TagProperty NOT LIKE '%" + itemTag.Id.ToString() + "%'";
                                }
                            }

                            initialRequest += subRequest;

                        }
                    }
                }

                var result = db.Set<PlantSearchPropertyItem>().SqlQuery(initialRequest);

                plantProperty = result.Select(p => new PlantSearchPropertyView(p)).OrderBy(x => x.nameLatin.Contains("[k]") ? x.nameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.nameLatin);


                plantProperty = plantProperty.ToList();
            if (searchQueryCode != "_co__eco__ex__acl__lcl__cl__fr_")
            {
                var searchResultId = string.Join(",", plantProperty.Select(p => p.id).ToArray());
                TempTableSearch newTempQuery = new TempTableSearch
                {
                    SearchQuery = searchQueryCode,
                    SearchResult = searchResultId,
                    SearchDate = DateTime.Now
                };

                db.TempTableSearches.Add(newTempQuery);
                db.SaveChanges();
            }


        }
            else
            {
                plantProperty = db.PlantSearchPropertyItems.ToList().Select(p => new PlantSearchPropertyView(p)).OrderBy(x => x.nameLatin.Contains("[k]") ? x.nameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.nameLatin);

                var storedId = existingQuery.SearchResult.Split(',').ToList().Select(s => int.Parse(s)).ToArray();
                var values = new StringBuilder();
                values.AppendFormat("{0}", storedId[0].ToString());
                for (int i = 1; i < storedId.Length; i++)
                    values.AppendFormat(", {0}", storedId[i]);

                var sql = string.Format(
                                "SELECT * FROM [pflanzenApp08].[dbo].[PlantSearchPropertyItems] WHERE [plantId] IN ({0})",
                                values);

                var result = db.Set<PlantSearchPropertyItem>().SqlQuery(sql);

                plantProperty = result.Select(p => new PlantSearchPropertyView(p)).OrderBy(x => x.nameLatin.Contains("[k]") ? x.nameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.nameLatin);


                //plantProperty = plantProperty.Where(p => storedId.Contains(p.id)).ToList();
            }








            if (!string.IsNullOrEmpty(searchText)) // search by text
            {
                appliedFilters.Add(new AppliedFilterVM() { pos = "searchText", t = searchText });
                taxonId = -1;
                //treeRoot = ttc.DbGetTreeNodeById(taxonomicTreeRootId);
                AdminArea.AdminAreaContentContributionController aaccc = new AdminArea.AdminAreaContentContributionController();
                //IEnumerable<Plant> plantsFromSynonyms = aaccc.GetPlantsAssociatedWithSynonym(searchText);
                plants = pc.DbGetPlantsByText(searchText).OrderBy(x => x.NameLatin.Contains("[k]") ? x.NameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.NameLatin);


                searchText = searchText.Replace("-", "").Replace(" ", "").Replace("'", "").Trim().ToLower();
                // handle common plural cases ending in "en"
                searchText = searchText.EndsWith("en") ? searchText.Remove(searchText.Length - 1) : searchText;

                var byteText = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(searchText);
                var normalizedSearchText = System.Text.Encoding.UTF8.GetString(byteText);


                plantProperty = plantProperty.Where(s => (s.nameLatin.Replace("-", "").Replace("'", "").Replace("‘", "").Replace("’", "").Replace(" ", "").ToLower().Contains(searchText)
                        || (s.nameGermanNorm != null && s.nameGermanNorm.Replace("-", "").Replace("'", "").Replace("‘","").Replace("’", "").Replace(" ", "").ToLower().Contains(normalizedSearchText))
                        || s.nameLatin.Replace("×", "").Replace("'", "").Replace(" ", "").Replace("‘", "").Replace("’", "").ToLower().Contains(searchText)
                        || (s.Synonym != null && s.Synonym.Replace("×", "").Replace("-", "").Replace("'", "").Replace("‘", "").Replace("’", "").Replace(" ", "").ToLower().Contains(searchText))
                        ));

            }
            else if (taxonId > 0) // search by taxon
            {
                TaxonomicTree selectedTaxon = Utilities.CloneJson(ttc.DbGetTreeNodeById(taxonId));

                if (selectedTaxon != null)
                {
                    searchText = "";
                    selectedTaxon.IsParentOfOrSelectedTaxon = true;

                    //treeRoot = ttc.DbGetParentTreeByDeepestNode_recursive(selectedTaxon);

                    plants = pc.DbGetChildrenTaxonPlantsByParentTaxonId(taxonId).OrderBy(x => x.NameLatin.Contains("[k]") ? x.NameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.NameLatin).ToList();
                    plants = plants.Where(m => m.Published);
                    plantProperty = plantProperty.Where(s => plants.Any(p => p.Id == s.id));

                }
                else // default view
                {

                    //plants = pc.DbGetPublishedPlantList().OrderBy(x => x.NameLatin.Contains("[k]") ? x.NameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.NameLatin).ToList();



                    //if (positiveFilterTagsReal.Any() || positiveFilterEcoTagsReal.Any())
                    //{
                    //    plants = GetInitialFilterPlants(positiveFilterTagsReal, positiveFilterEcoTagsReal, orList, andList, selGardenGroup);


                    //}
                    //else
                    //{

                        plants = pc.DbGetPublishedPlantList().OrderBy(x => x.NameLatin.Contains("[k]") ? x.NameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.NameLatin).ToList();



                    //}

                }
            }
            else 
            {

                //plants = pc.DbGetPublishedPlantList().OrderBy(x => x.NameLatin.Contains("[k]") ? x.NameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.NameLatin).ToList();


                //if (positiveFilterTagsReal.Any() || positiveFilterEcoTagsReal.Any())
                //{
                //    plants = GetInitialFilterPlants(positiveFilterTagsReal, positiveFilterEcoTagsReal, orList, andList, selGardenGroup);


                //}
                //else
                //{

                    plants = pc.DbGetPublishedPlantList().OrderBy(x => x.NameLatin.Contains("[k]") ? x.NameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.NameLatin).ToList();



                //}


            }

            if (gardenGroup != null)
            {
                //Get plants for selected gardenGroup id
                selGardenGroup = db.GardenCategories.Where(p => p.Id == gardenGroup.Value).FirstOrDefault();

                plantProperty = plantProperty.Where(p => selGardenGroup.Plants.Any(s => s.Id == p.id));
            }


            List<PlantCharacteristic> filterCharacteristics = new List<PlantCharacteristic>();

            int heightMin = 0;
            int heightMax = 800;
            int monthMin = 1;
            int monthMax = 12;
            selMaxMonth = cookieSelMaxMonth;
            selMinMonth = cookieSelMinMonth;
            selHmin = cookieSelHeightMin;
            selHmax = cookieSelHeightMax;

            plantProperty = plantProperty.ToList();

            if (selHmin != heightMin || selHmax != heightMax)
            {
                appliedFilters.Add(new AppliedFilterVM() { pos = "growthRange", t = $"{selHmin} cm - {selHmax} cm" });
                plantProperty = plantProperty.Where(p => ((p.HeightPropertyMin <= selHmin && p.HeightPropertyMax >= selHmax) ||
                (p.HeightPropertyMin <= selHmin && p.HeightPropertyMax >= selHmin) ||
                (p.HeightPropertyMax >= selHmax && p.HeightPropertyMin <= selHmax) ||
                (p.HeightPropertyMin > selHmin && p.HeightPropertyMax < selHmax)
                ));

                PlantCharacteristic heightFilter = new PlantCharacteristic
                {
                    Category = pccc.DbGetPlantCharacteristicCategories().Where(c => c.Id == 24).FirstOrDefault(),
                    CategoryId = 24,
                    Min = selHmin,
                    Max = selHmax
                };

                filterCharacteristics.Add(heightFilter);
            }
            if (selMinMonth != monthMin || selMaxMonth != monthMax)
            {
                appliedFilters.Add(new AppliedFilterVM() { pos = "monthRange", t = $"Blühdauer Monat  {selMinMonth} bis {selMaxMonth}" });
                plantProperty = plantProperty.Where(p => (p.BloomPropertyMax >= p.BloomPropertyMin && (selMinMonth >= p.BloomPropertyMin && selMaxMonth <= p.BloomPropertyMax)) ||
                             (p.BloomPropertyMax < p.BloomPropertyMin && (selMinMonth <= p.BloomPropertyMin && p.BloomPropertyMax >= selMaxMonth)));

            }

            var isExist = false;

            if (plantProperty != null && plantProperty.Count()>0)
            {
                isExist = true;
                if ( groupId != null)
                {
                    //Get plants for selected group id
                    int groupIdVal = (int)groupId;
                    plantProperty = plantProperty.Where(p => p.GroupProperty != null).Where(p =>  p.GroupPropertyArray.ToList().Contains(groupIdVal));
                    plants = plants.Where(p => p.PlantGroups.Any(g => g.Id == groupId));
                }

                

                if ( !String.IsNullOrEmpty(family))
                {
                    var ignored = new string[] { "einige", "Autoren:", "früher" };
                    var famQuery = family.Replace("[k]", "").Replace("[/k]", "").Trim().ToLower();
                    var punc = famQuery.Where(Char.IsPunctuation).Distinct().ToArray();
                    var familyNames = famQuery.Split().Where(x => !ignored.Contains(x)).Select(x => x.Trim(punc)).ToArray();
                    var hasTwoNames = familyNames.Count() > 1;

                    //Get plants for selected family(s)
                    if (hasTwoNames)
                    {
                       // plants = plants.Where(
                       //p => p.Familie != null && p.Familie.ToLower().Contains(familyNames[0]) && p.Familie.ToLower().Contains(familyNames[1]));

                        plantProperty = plantProperty.Where(
                       p => p.Familie != null && p.Familie.ToLower().Contains(familyNames[0]) && p.Familie.ToLower().Contains(familyNames[1]));

                    }
                    else
                    {

                        plantProperty = plantProperty.Where(p => p.Familie != null && p.Familie.ToLower().Equals(familyNames[0]));
                    }
                }

                //if (filterCharacteristics.Any())
                //{
                //    plants = plants.Where(m => m.Published);
                //    var characteristics = pcc.DbGetPlantCharacteristics().ToList();
                //    foreach (Plant plant in plants)
                //    {
                //        plant.PlantCharacteristics = characteristics.Where(c => c.PlantId == plant.Id);
                //    }
                //    plants = sortOutPlantsByCharacteristics(plants, filterCharacteristics);
                //}

            }



            //var currentPlantList = plants.ToList();
            var currentPropertyList = plantProperty.ToList();

            if (take == int.MaxValue)
            {

                //outputPlant = outputPlant.Skip(skip).Take(take);

                PlantSearchViewModel searchViewModel2 = new PlantSearchViewModel
                {
                    CategoryId = selectedCategoryId,
                    CategoryList = categoryList,
                    SubCategoryId = selectedSubCategoryId,
                    SubCategoryList = subCategoryList,
                    TagsList = tagsList,
                    HeightMax = cookieSelHeightMax, //heightMax,
                    HeightMin = cookieSelHeightMin, //heightMin,
                    Input_search = searchText,
                    PlantProperties = currentPropertyList,
                    //MonthCheckboxes = monthCheckboxes,
                    //PlantList = plants,
                    SuperCategories = null,
                    SelHmax = selHmax,
                    SelHmin = selHmin,
                    AppliedFilters = appliedFilters
                };

                return searchViewModel2;
            }

            if (currentPropertyList.Any())
            {
                var plantSql = "SELECT * FROM [pflanzenApp08].[dbo].[Plants] WHERE id IN (" + string.Join(",", currentPropertyList.Select(p => "'" + p.id.ToString() + "'").ToArray()) + ")";

                var result = db.Set<Plant>().SqlQuery(plantSql);

                plants = result.OrderBy(x => x.NameLatin.Contains("[k]") ? x.NameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.NameLatin).ToList();

            }
            else
            {
                plants = null;
            }

            //plants = plants.Where(p => currentPropertyList.Any(t => p.Id == t.id)).ToList();


            // process possible tag categories and characteristics from sorted plant list

            // process plant tag categories
            IEnumerable<PlantTagCategory> possibleCategories = null;

            if (isExist)
            {
                possibleCategories = getPossiblePlantTagCategoriesByPlantTags(plants, positiveFilterTagsReal);
            }
            else
            {
                possibleCategories = getPossibleEmptyPlantTagCategoriesByPlantTags(plants, positiveFilterTagsReal);
            }



            categoryList = sortPossibleCategoriesForView(possibleCategories);

            // process subcategories and tag list
            if (categoryList != null && categoryList.Any())
            {
                PlantTagCategory activeCategory = null;
                if (selectedCategoryId > 0 && categoryList.Where(c => c.Id == selectedCategoryId).Any())
                {
                    activeCategory = categoryList.Where(c => c.Id == selectedCategoryId).FirstOrDefault();
                }
                else
                {
                    activeCategory = categoryList.FirstOrDefault();
                }

                if (activeCategory != null && activeCategory.Childs != null && activeCategory.Childs.Any())
                {
                    subCategoryList.Add(new PlantTagCategory { Id = -1, Title = "----" });
                    subCategoryList.AddRange(activeCategory.Childs);
                }

                if (selectedSubCategoryId > 0 && subCategoryList != null && subCategoryList.Where(c => c.Id == selectedSubCategoryId).Any())
                {
                    tagsList = subCategoryList.Find(c => c.Id == selectedSubCategoryId).TagsInThisCategory;
                }
                else if (activeCategory != null && activeCategory.TagsInThisCategory != null)
                {
                    tagsList = activeCategory.TagsInThisCategory;
                }

                selectedCategoryId = activeCategory != null ? activeCategory.Id : 0;
            }

            var cats = pc.DbGetSuperCategoriesByText(searchText);
            var catsViewModel = new List<PlantTagSuperCategoryViewModel>();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            foreach (var supercat in cats)
            {
                var temp = new PlantTagSuperCategoryViewModel()
                {
                    NameGerman = supercat.NameGerman,
                    NameLatin = supercat.NameLatin,
                    Description = supercat.Description,
                    Synonym = supercat.Synonym
                };
                HelperClasses.DbResponse imageResponse = rc.DbGetPlantTagReferencedImages(supercat.Id);
                if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                {
                    temp.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                }
                else
                {

                    temp.Images.Add(new _HtmlImageViewModel
                    {
                        SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                        Id = 0,
                        TitleAttr = "Kein Bild vorhanden"
                    });
                }
                catsViewModel.Add(temp);
            }

            if (skip < 0)
            {
                skip = 0;
            }

            if (plants == null)
            {
                PlantSearchViewModel searchViewModelNull = new PlantSearchViewModel
                {
                    CategoryId = selectedCategoryId,
                    CategoryList = categoryList,
                    SubCategoryId = selectedSubCategoryId,
                    SubCategoryList = subCategoryList,
                    TagsList = tagsList,
                    HeightMax = cookieSelHeightMax, //heightMax,
                    HeightMin = cookieSelHeightMin, //heightMin,
                    Input_search = searchText,
                    //MonthCheckboxes = monthCheckboxes,
                    PlantList = new List<Plant>(),
                    SuperCategories = catsViewModel,
                    SelHmax = selHmax,
                    SelHmin = selHmin,
                    AppliedFilters = appliedFilters
                };

                return searchViewModelNull;
            }
          
            var outputPlant = plants.Skip(skip).Take(take).ToList();
          
            //outputPlant = outputPlant.Skip(skip).Take(take);

            PlantSearchViewModel searchViewModel = new PlantSearchViewModel
            {
                CategoryId = selectedCategoryId,
                CategoryList = categoryList,
                SubCategoryId = selectedSubCategoryId,
                SubCategoryList = subCategoryList,
                TagsList = tagsList,
                HeightMax = cookieSelHeightMax, //heightMax,
                HeightMin = cookieSelHeightMin, //heightMin,
                Input_search = searchText,
                //MonthCheckboxes = monthCheckboxes,
                PlantList = outputPlant,
                SuperCategories = catsViewModel,
                SelHmax = selHmax,
                SelHmin = selHmin,
                AppliedFilters = appliedFilters
            };

            return searchViewModel;
        }

        private static IEnumerable<Plant> FilterMorePlantsWithText(IEnumerable<PlantTag> positiveFilterTagsReal, IEnumerable<PlantTag> positiveFilterEcoTagsReal, int[] orList, int[][] specialList, IEnumerable<Plant> plants)
        {
            if (positiveFilterTagsReal.Any())
            {
                foreach (var filter in positiveFilterTagsReal)
                {
                    plants = plants.Where(p => p.PlantTags.Where(t => t.Id == filter.Id).Any());
                }
            }
            //if (positiveFilterEcoTagsReal.Any())
            //{
            //    foreach (var filter in positiveFilterEcoTagsReal.Where(t => !orList.Contains(t.Id)))
            //    {
            //        plants = plants.Where(p => p.PlantTags.Where(t => t.Id == filter.Id).Any());
            //    }
            //}
            if (positiveFilterEcoTagsReal.Where(t => orList.Contains(t.Id)).Any())
            {
                plants = plants.Where(p => p.PlantTags.Where(t => positiveFilterTagsReal.Where(f => orList.Contains(f.Id)).Contains(t)).Any());
            }

            
            //foreach (var special in specialList)
            //{
            //    if (positiveFilterTagsReal.Where(p => special.Contains(p.Id)).Any())
            //    {
            //        foreach (var s in special)
            //        {
            //            plants = plants.Where(p => p.PlantTags.Where(t => t.Id == s).Any());
            //        }
            //    }
            //}

            return plants;
        }

        private string TagPosition(PlantTag tag)
        {
            if (getEcoTagsList().Contains(tag.Id))
            {
                return "ecoTags";
            }

            string res;
            var map = new Dictionary<string, string>()
            {
                { "Licht", "places" },
                { "Blüten", "blossoms" },
                { "Blütengröße", "blossomsSize" },
                { "Früchte", "fruitType" },
                { "Boden", "groundType" },
                { "Düngung", "fertil" },
                { "Verwendung", "usage" },
                { "Schnitt", "trim" },
                { "Vermehrung", "breeding" },
                { "Wuchs", "growthType" },
                { "Nutzpflanzen", "cropPlant" },
                { "Wasserbedarf", "waterReq" },
                { "Besonderheiten", "specifics" },
                { "Dekoaspekte", "deco" },
                { "Laubrythmus", "foliage" },
                { "Blütenform", "flShape" },
                { "Fruchtfarbe", "fruitClr" },
                { "Blütenstand", "blossomStat" },
                { "Blattform", "leafShape" },
                { "Blattrand", "leafMargin" },
                { "Blattstellung", "leafPos" }
            };
            if (map.TryGetValue(tag.Category.Title, out res))
            {
                return res;
            }
            return null;
        }

        public List<string> GetSearchNameList(string plantName)
        {
            var plants = pc.DbGetPublishedPlantNameList(plantName);

            var nameSection = plantName.Split(' ');
            if (nameSection.Count() > 1)
            {
                plants.AddRange(pc.DbGetPublishedPlantDoubleNameList(nameSection[0], nameSection[1]));

                plants = plants.Distinct().ToList();
            }

            return plants;
        }


        private static IEnumerable<Plant> FilterMorePlants(IEnumerable<PlantTag> positiveFilterTagsReal, IEnumerable<PlantTag> positiveFilterEcoTagsReal, int[] orList, int[][] specialList, IEnumerable<Plant> plants)
        {
            if (positiveFilterTagsReal.Any())
            {
                IEnumerable<int> andPositiveTagsPlant = null;
                foreach (var filter in positiveFilterTagsReal)
                {
                    if (andPositiveTagsPlant == null)
                    {
                        andPositiveTagsPlant = filter.PlantsWithThisTag.Where(p => p.Published).Select(p => p.Id);
                        continue;
                    }

                    var currentTags = filter.PlantsWithThisTag.Where(p => p.Published).Select(p => p.Id);

                        andPositiveTagsPlant = andPositiveTagsPlant.Where(a => currentTags.Contains(a));
                }

                if (andPositiveTagsPlant != null)
                {
                    if (andPositiveTagsPlant.Any())
                    {
                        plants = plants.Where(p => andPositiveTagsPlant.Contains(p.Id));

                    }
                }
                //foreach (var filter in positiveFilterTagsReal)
                //{
                //    plants = plants.Where(p => p.PlantTags.Where(t => t.Id == filter.Id).Any());
                //}
                //plants = plants.Where(p => p.PlantTags.All(t => positiveFilterTagsReal.Contains(t)));

            }
            if (positiveFilterEcoTagsReal.Any())
            {

                IEnumerable<int> ecoFilterTagsPlant = null;

                var orTags = positiveFilterEcoTagsReal.Where(p => orList.Contains(p.Id));

                var andTags = positiveFilterEcoTagsReal.Where(p => !orList.Contains(p.Id));

                ecoFilterTagsPlant = orTags.SelectMany(t => t.PlantsWithThisTag).Where(p => p.Published).Select(p => p.Id).Distinct();

                IEnumerable<int> andEcoFilterTagsPlant = null;

                foreach(var andF in andTags)
                {
                    if (andEcoFilterTagsPlant == null)
                    {
                        andEcoFilterTagsPlant = andF.PlantsWithThisTag.Where(p => p.Published).Select(p => p.Id);
                        continue;
                    }

                    var currentTags = andF.PlantsWithThisTag.Where(p => p.Published).Select(p => p.Id);

                    andEcoFilterTagsPlant = andEcoFilterTagsPlant.Where(a => currentTags.Contains(a));
                }

                if (andEcoFilterTagsPlant != null)
                {
                    if (andEcoFilterTagsPlant.Any())
                    {
                        plants = plants.Where(p => andEcoFilterTagsPlant.Contains(p.Id));

                    }
                }
                
                if (ecoFilterTagsPlant != null)
                {
                    if (ecoFilterTagsPlant.Any())
                    {
                        plants = plants.Where(p => ecoFilterTagsPlant.Contains(p.Id));

                    }
                }

            }



            return plants;
        }

        private static IEnumerable<PlantSearchPropertyView> FilterMorePlantsProperty(IEnumerable<PlantTag> positiveFilterTagsReal, IEnumerable<PlantTag> positiveFilterEcoTagsReal, int[] orList, int[][] specialList, IEnumerable<PlantSearchPropertyView> plants)
        {
            if (positiveFilterTagsReal.Any())
            {
                foreach (var filter in positiveFilterTagsReal)
                {


                    plants = plants.Where(p => p.TagProperty.Contains(filter.Id.ToString()));
                }

    


            }
            if (positiveFilterEcoTagsReal.Any())
            {


                var orTags = positiveFilterEcoTagsReal.Where(p => orList.Contains(p.Id));

                var andTags = positiveFilterEcoTagsReal.Where(p => !orList.Contains(p.Id));

                plants = plants.Where(p => orTags.Any(t => p.TagProperty.Contains(t.Id.ToString())));
      

                foreach (var andF in andTags)
                {

                    plants = plants.Where(p => p.TagProperty.Contains(andF.Id.ToString()));



                }

   

            }



            return plants;
        }

        //private static IEnumerable<Plant> GetInitialFilterPlants(IEnumerable<PlantTag> positiveFilterTagsReal, IEnumerable<PlantTag> positiveFilterEcoTagsReal, int[] orList, int[] andList, GardenCategory selGardenGroup)
        //{
        //    IEnumerable<Plant> plants;
        //    IEnumerable<PlantTag> positiveFilterTagsRealconcat= new List<PlantTag>() ;
        //    if (selGardenGroup != null)
        //    {
        //        plants = selGardenGroup.Plants.Where(p => p.Published).ToList();
        //        return plants.OrderBy(x => x.NameLatin.Contains("[k]") ? x.NameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.NameLatin);
        //    }

        //    if (positiveFilterTagsReal.Any() || positiveFilterEcoTagsReal.Any())
        //    {
        //        positiveFilterTagsRealconcat = positiveFilterTagsReal.Concat(positiveFilterEcoTagsReal);
        //    }
        //    if (positiveFilterTagsRealconcat.Where(t => andList.Contains(t.Id)).Any())
        //    {
        //        plants = positiveFilterTagsRealconcat
        //            .Where(t => andList.Contains(t.Id))
        //            .OrderBy(p => p.PlantsWithThisTag.Count())
        //            .First()
        //            .PlantsWithThisTag
        //            .Where(p => p.Published);
        //    }
        //    else if (positiveFilterTagsRealconcat.Where(t => orList.Contains(t.Id)).Any())
        //    {
        //        plants = positiveFilterTagsRealconcat.SelectMany(t => t.PlantsWithThisTag)
        //            .Where(p => p.Published).Distinct();
        //    }
        //    else
        //    {
        //        plants = positiveFilterTagsRealconcat.OrderBy(p => p.PlantsWithThisTag.Count()).First().PlantsWithThisTag.Where(p => p.Published);
        //    }

        //    return plants.OrderBy(x => x.NameLatin.Contains("[k]") ? x.NameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.NameLatin);
        //}

        public dynamic GetPlantColorsAndBadgesById(int plantId)
        {
            var BADGES_IDS = getEcoTagsList();
            List<string> listOfColors = new List<string>();
            string color;
            IEnumerable<string> listOfColorsDistincts = null;
            var sel = (from plant in db.Plants
                       where !plant.Deleted && plant.Id == plantId
                       select plant).Include(p => p.PlantTags).FirstOrDefault();

            var badges = sel.PlantTags
                    .Where(t => !t.Deleted && (BADGES_IDS.Contains(t.Id)))
                    .Select(b => new PlantTagSearchLite() { Id = b.Id });

            var colorsSelected = sel.PlantTags.Where(t => t.CategoryId == 64).Select(t => t.Title);
            if (colorsSelected != null && colorsSelected.Any())
            {
                foreach (var c in colorsSelected)
                {
                    if (c.Contains('-'))
                    {
                        color = c;
                        string[] elements = color.Split('-');
                        foreach (var e in elements)
                        {
                            listOfColors.Add(e);
                        }
                    }
                    else
                    {
                        listOfColors.Add(c);
                    }

                }
                listOfColorsDistincts = listOfColors.Distinct();
            }

            return new { Colors = listOfColorsDistincts, Badges = badges };
        }

        public ActionResult ajaxGetPlantSearchResult()
        {
            return PartialView("_searchPageContent", getSearchViewModel());
        }

        public ActionResult ajaxGetPlantSearchControls()
        {
            return PartialView("_searchControls", getSearchViewModel());
        }
    }
}