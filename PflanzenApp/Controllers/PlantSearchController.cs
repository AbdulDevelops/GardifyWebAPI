using Microsoft.AspNet.Identity;
using PflanzenApp.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static GardifyModels.Models.ModelEnums;

namespace PflanzenApp.Controllers
{
    [CustomAuthorize]
    public class PlantSearchController : _BaseController
    {
        private int taxonomicTreeRootId = 1;
        PlantController pc = new PlantController();

        // GET: PlantSearch
        [AcceptVerbs("GET")]
        public ActionResult Index()
        {
            Guid userId = Utilities.GetUserId();
            return View(getSearchViewModel(userId));
        }

        public ActionResult Autocomplete(string term)
        {
            try
            {
                var german = (from pl in ctx.Plants
                              where (pl.NameGerman.Contains(term))
                              && !pl.Deleted && pl.Published
                              select new { label = pl.NameGerman });
                var latin = (from pl in ctx.Plants
                             where (pl.NameLatin.Contains(term))
                             && !pl.Deleted && pl.Published
                             select new { label = pl.NameLatin });
                var plants = pc.DbGetPublishedPlantList().ToList();
                var syn = (from pl in ctx.Synonym
                           where (pl.Text.Contains(term))
                           && !pl.Deleted
                           select new { label = pl.Text });
                var result = german.Concat(latin).Concat(syn);
                result = result.Take(10);
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json("{'ex':'" + ex + "'}");
            }
        }

        public IEnumerable<TodoTemplate> DbGetTodoTemplates(int referenceId, ReferenceToModelClass type)
        {
            var templates = (from t in ctx.TodoTemplate
                             where !t.Deleted
                             && t.ReferenceId == referenceId && t.ReferenceType == type
                             select t);
            return templates;
        }

        [NonAction]
        public List<PlantViewModels.PlantViewModel> plantsToPlantViewModels(IEnumerable<Plant> plants, string rootPath)
        {
            PlantCharacteristicController pcc = new PlantCharacteristicController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            List<PlantViewModels.PlantViewModel> viewModels = new List<PlantViewModels.PlantViewModel>();

            if (plants != null && plants.Any())
            {
                foreach (Plant plant in plants)
                {
                    PlantViewModels.PlantViewModel plantView = new PlantViewModels.PlantViewModel
                    {
                        Id = plant.Id,
                        NameLatin = plant.NameLatin,
                        NameGerman = plant.NameGerman,
                        Description = plant.Description,
                        PlantTags = plant.PlantTags,
                        PlantCharacteristicsOld = pcc.DbGetPlantCharacteristicsByPlantId(plant.Id)
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
                            SrcAttr = Utilities.GetBaseUrl() + "/Images/no-image.png",
                            Id = 0,
                            TitleAttr = "Kein Bild vorhanden"
                        });
                    }

                    viewModels.Add(plantView);
                }
            }
            return viewModels;
        }

        private IEnumerable<Plant> sortOutPlantsByCharacteristics(IEnumerable<Plant> plantList, IEnumerable<PlantCharacteristic> characteristicList)
        {
            if (plantList != null && plantList.Any() && characteristicList != null && characteristicList.Any())
            {
                List<Plant> ret = plantList.ToList();
                List<Plant> tmp = new List<Plant>();

                IEnumerable<IGrouping<int, PlantCharacteristic>> characteristicGroups = characteristicList.GroupBy(c => c.CategoryId);
                foreach (IGrouping<int, PlantCharacteristic> group in characteristicGroups)
                {
                    foreach (Plant plant in ret)
                    {
                        foreach (PlantCharacteristic characteristic in group)
                        {
                            if (validatePlantCharacteristic(plant, characteristic))
                            {
                                tmp.Add(plant);
                                break;
                            }
                        }
                    }
                    // process only with plants from past characteristic check
                    ret.Clear();
                    ret.AddRange(tmp);
                    tmp.Clear();
                }
                return ret;
            }
            return plantList;
        }

        private bool validatePlantCharacteristic(Plant plant, PlantCharacteristic inputCharacteristic)
        {
            var plantCharacteristics = plant.PlantCharacteristics.Where(c => c.CategoryId == inputCharacteristic.CategoryId);
            if (plantCharacteristics != null && plantCharacteristics.Any())
            {
                foreach (PlantCharacteristic characteristic in plantCharacteristics)
                {
                    bool check = false;
                    switch (inputCharacteristic.Category.CharacteristicValueType)
                    {
                        case ModelEnums.CharacteristicValueType.SingleNumber:
                        case ModelEnums.CharacteristicValueType.SingleMonth:
                        case ModelEnums.CharacteristicValueType.SingleLatinNumber:
                            if (inputCharacteristic.Max != null)
                            {
                                check = characteristic.Min >= inputCharacteristic.Min && characteristic.Min <= inputCharacteristic.Max;
                            }
                            else
                            {
                                check = characteristic.Min == inputCharacteristic.Min;
                            }
                            break;

                        case ModelEnums.CharacteristicValueType.NumberRange:
                        case ModelEnums.CharacteristicValueType.MonthRange:
                        case ModelEnums.CharacteristicValueType.LatinNumberRange:
                            if (inputCharacteristic.Max != null)
                            {
                                check = characteristic.Min <= inputCharacteristic.Max && characteristic.Max >= inputCharacteristic.Min;
                            }
                            else
                            {
                                check = inputCharacteristic.Min <= characteristic.Max && inputCharacteristic.Min >= characteristic.Min;
                            }
                            break;

                        default:
                            check = false;
                            break;
                    }

                    if (check)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private List<PlantSearchViewModel.CheckboxHelper> getFloweringMonthList(string checkedMonth)
        {
            List<PlantSearchViewModel.CheckboxHelper> ret = new List<PlantSearchViewModel.CheckboxHelper>();
            string[] checkedIndexes_str;
            if (String.IsNullOrEmpty(checkedMonth))
            {
                checkedIndexes_str = new string[0];
            }
            else
            {
                checkedIndexes_str = checkedMonth.Split(',');
            }


            PlantSearchViewModel.CheckboxHelper jan = new PlantSearchViewModel.CheckboxHelper
            {
                Value = 1,
                Checked = checkedIndexes_str.Contains("1") ? true : false,
                Text = "I (Jan)"
            };

            PlantSearchViewModel.CheckboxHelper feb = new PlantSearchViewModel.CheckboxHelper
            {
                Value = 2,
                Checked = checkedIndexes_str.Contains("2") ? true : false,
                Text = "II (Feb)"
            };

            PlantSearchViewModel.CheckboxHelper mrz = new PlantSearchViewModel.CheckboxHelper
            {
                Value = 3,
                Checked = checkedIndexes_str.Contains("3") ? true : false,
                Text = "III (Mrz)"
            };

            PlantSearchViewModel.CheckboxHelper apr = new PlantSearchViewModel.CheckboxHelper
            {
                Value = 4,
                Checked = checkedIndexes_str.Contains("4") ? true : false,
                Text = "IV (Apr)"
            };

            PlantSearchViewModel.CheckboxHelper mai = new PlantSearchViewModel.CheckboxHelper
            {
                Value = 5,
                Checked = checkedIndexes_str.Contains("5") ? true : false,
                Text = "V (Mai)"
            };

            PlantSearchViewModel.CheckboxHelper jun = new PlantSearchViewModel.CheckboxHelper
            {
                Value = 6,
                Checked = checkedIndexes_str.Contains("6") ? true : false,
                Text = "VI (Jun)"
            };

            PlantSearchViewModel.CheckboxHelper jul = new PlantSearchViewModel.CheckboxHelper
            {
                Value = 7,
                Checked = checkedIndexes_str.Contains("7") ? true : false,
                Text = "VII (Jul)"
            };

            PlantSearchViewModel.CheckboxHelper aug = new PlantSearchViewModel.CheckboxHelper
            {
                Value = 8,
                Checked = checkedIndexes_str.Contains("8") ? true : false,
                Text = "VIII (Aug)"
            };

            PlantSearchViewModel.CheckboxHelper sep = new PlantSearchViewModel.CheckboxHelper
            {
                Value = 9,
                Checked = checkedIndexes_str.Contains("9") ? true : false,
                Text = "IX (Sep)"
            };

            PlantSearchViewModel.CheckboxHelper okt = new PlantSearchViewModel.CheckboxHelper
            {
                Value = 10,
                Checked = checkedIndexes_str.Contains("10") ? true : false,
                Text = "X (Okt)"
            };

            PlantSearchViewModel.CheckboxHelper nov = new PlantSearchViewModel.CheckboxHelper
            {
                Value = 11,
                Checked = checkedIndexes_str.Contains("11") ? true : false,
                Text = "XI (Nov)",
                //Disabled = true
            };

            PlantSearchViewModel.CheckboxHelper dez = new PlantSearchViewModel.CheckboxHelper
            {
                Value = 12,
                Checked = checkedIndexes_str.Contains("12") ? true : false,
                Text = "XII (Dez)",
                //Disabled = true
            };

            ret.Add(jan);
            ret.Add(feb);
            ret.Add(mrz);
            ret.Add(apr);
            ret.Add(mai);
            ret.Add(jun);
            ret.Add(jul);
            ret.Add(aug);
            ret.Add(sep);
            ret.Add(okt);
            ret.Add(nov);
            ret.Add(dez);

            return ret;
        }

        private List<PlantTagCategory> getPossiblePlantTagCategoriesByPlantTags(IEnumerable<Plant> plantsToCheck, IEnumerable<PlantTag> positiveFilterTags)
        {
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
            List<PlantTagCategory> ret = new List<PlantTagCategory>();
            if (plantsToCheck != null)
            {
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
                                    PlantTagCategory tmp = ptcc.DbGetPlantTagCategoryById(tag.CategoryId);
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
            return ret;
        }

        private List<PlantTagCategory> sortPossibleCategoriesForView(List<PlantTagCategory> categories)
        {
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
            List<PlantTagCategory> ret = new List<PlantTagCategory>();

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
                        if (categories.Find(c => c.Id == cat.ParentId) != null)
                        {
                            PlantTagCategory parentCat = categories.Find(c => c.Id == cat.ParentId);
                            parentCat.Childs = new List<PlantTagCategory>();
                            parentCat.Childs.Add(cat);
                            ret.Add(parentCat);
                            // create new parent cat
                        }
                        else
                        {
                            PlantTagCategory tmp = ptcc.DbGetPlantTagCategoryById((int)cat.ParentId);
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

        private List<PlantTag> getPositiveFilterTags()
        {
            PlantTagController ptc = new PlantTagController();
            List<PlantTag> ret = new List<PlantTag>();
            string cookieTags = getValueFromCookie("positiveTags");
            if (!String.IsNullOrEmpty(cookieTags))
            {
                string[] ids_str = cookieTags.Split(',');

                for (int i = 0; i < ids_str.Length; i++)
                {
                    int tagId = -1;
                    if (int.TryParse(ids_str[i], out tagId))
                    {
                        PlantTag tag = ptc.DbGetPlantTagById(tagId);
                        if (tag != null && !ret.Contains(tag))
                        {
                            tag.Title = tag.Category.Title + ": " + tag.Title;
                            ret.Add(tag);
                        }
                    }
                }
                if (ret.Count > 1)
                {
                    ret = ret.OrderBy(t => t.Title).ToList();
                }
            }

            return ret;
        }

        private int getTaxonId()
        {
            int taxonId = -1;
            string cookieTaxonId = getValueFromCookie("taxonId");
            if (!string.IsNullOrEmpty(cookieTaxonId))
            {
                int.TryParse(cookieTaxonId, out taxonId);
            }
            return taxonId;
        }

        private string getSearchText()
        {
            string searchText = getValueFromCookie("searchText");
            return searchText;
        }

        private int getSelectedCategoryId()
        {
            int categoryId = -1;
            string cookieCategoryId = getValueFromCookie("categoryId");
            if (!string.IsNullOrEmpty(cookieCategoryId))
            {
                int.TryParse(cookieCategoryId, out categoryId);
            }
            return categoryId;
        }

        private int getSelectedSubCategoryId()
        {
            int subCategoryId = -1;
            string cookieSubCategoryId = getValueFromCookie("subCategoryId");
            if (!string.IsNullOrEmpty(cookieSubCategoryId))
            {
                int.TryParse(cookieSubCategoryId, out subCategoryId);
            }
            return subCategoryId;
        }

        private string getValueFromCookie(string key)
        {
            string ret = "";
            if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("filter"))
            {
                HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["filter"];

                if (!String.IsNullOrEmpty(cookie.Values[key]))
                {
                    ret = cookie.Values[key];
                }
            }
            return ret;
        }

        [AcceptVerbs("POST", "GET")]
        [ActionName("reset-search")]
        public ActionResult ResetSearch()
        {
            HttpCookie cookie = null;
            if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("filter"))
            {
                cookie = this.ControllerContext.HttpContext.Request.Cookies["filter"];
                cookie.Expires = DateTime.Now.AddDays(-1);
                this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
            }
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult PlantDetails(int? id)
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

                    PlantViewModels.PlantViewModel plantView = new PlantViewModels.PlantViewModel
                    {
                        Id = plant.Id,
                        NameLatin = plant.NameLatin,
                        NameGerman = plant.NameGerman,
                        Description = plant.Description,
                        PlantTags = plant.PlantTags,
                        GenusTaxon = genusTaxon,
                        //Articles = ac.GetObjectArticles(ModelEnums.ArticleReferenceType.Plant, plant.Id)
                    };

                    HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages(plant.Id);

                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                    {
                        plantView.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
                    }
                    else
                    {
                        plantView.Images.Add(new _HtmlImageViewModel
                        {
                            SrcAttr = Url.Content("~/Images/no-image.png"),
                            Id = 0,
                            TitleAttr = "Kein Bild vorhanden"
                        });
                    }

                    plantView.PlantCharacteristicsOld = pcc.DbGetPlantCharacteristicsByPlantId(plant.Id);
                    plantView.IsInUserGarden = false;

                    Guid userId = Guid.Parse(User.Identity.GetUserId());
                    lvc.CreateLastViewed(new LastViewed { PlantId = (int)id, UserId = userId, CreatedBy = User.Identity.GetUserName() });

                    return View(plantView);
                }
            }
            return RedirectToError("Seite konnte nicht gefunden werden.", HttpStatusCode.NotFound, "PlantSearchController.PlantDetails(" + id + ")");
        }

        private PlantSearchViewModel getSearchViewModel(Guid userId, int? gardenId = null)
        {
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            PlantController pc = new PlantController();
            PlantCharacteristicCategoryController pccc = new PlantCharacteristicCategoryController();
            PlantCharacteristicController pcc = new PlantCharacteristicController();
            SearchQueryController sqc = new SearchQueryController();

            // get search values from cookie
            int taxonId = getTaxonId();
            string searchText = getSearchText();
            List<PlantTag> positiveFilterTags = getPositiveFilterTags();
            int selectedCategoryId = getSelectedCategoryId();
            int selectedSubCategoryId = getSelectedSubCategoryId();

            // to view
            IEnumerable<Plant> plants = null;
            TaxonomicTree treeRoot = null;
            // TODO: get max and min values from plant list
            int heightMin = 0;
            int heightMax = 500;

            int selHmin = heightMin;
            int selHmax = heightMax;

            List<PlantTagCategory> categoryList = new List<PlantTagCategory>();
            List<PlantTagCategory> subCategoryList = new List<PlantTagCategory>();
            IEnumerable<PlantTag> tagsList = null;

            if (!string.IsNullOrEmpty(searchText)) // search by text
            {
                taxonId = -1;
                treeRoot = ttc.DbGetTreeNodeById(taxonomicTreeRootId);
                AdminArea.AdminAreaContentContributionController aaccc = new AdminArea.AdminAreaContentContributionController();
                IEnumerable<Plant> plantsFromSynonyms = aaccc.GetPlantsAssociatedWithSynonym(searchText);
                IEnumerable<Plant> plantsByText = pc.DbGetPlantsByText(searchText);
                plants = plantsFromSynonyms.Concat(plantsByText);
                plants = plants.Where(m => m.Published);
            }
            else if (taxonId > 0) // search by taxon
            {
                TaxonomicTree selectedTaxon = Utilities.CloneJson(ttc.DbGetTreeNodeById(taxonId));

                if (selectedTaxon != null)
                {
                    searchText = "";
                    selectedTaxon.IsParentOfOrSelectedTaxon = true;

                    treeRoot = ttc.DbGetParentTreeByDeepestNode_recursive(selectedTaxon);

                    plants = pc.DbGetChildrenTaxonPlantsByParentTaxonId(taxonId);
                    plants = plants.Where(m => m.Published);
                }
                else // default view
                {
                    treeRoot = ttc.DbGetTreeNodeById(taxonomicTreeRootId);
                    plants = pc.DbGetPublishedPlantList();
                }
            }
            else // default view
            {
                treeRoot = ttc.DbGetTreeNodeById(taxonomicTreeRootId);
                plants = pc.DbGetPublishedPlantList();
            }

            // sort out plants by positive filter tags
            if (positiveFilterTags != null)
            {
                plants = Utilities.sortOutPlantsByPositiveTagList(plants, positiveFilterTags);
                plants = plants.Where(m => m.Published);
            }

            List<PlantCharacteristic> filterCharacteristics = new List<PlantCharacteristic>();

            // add height filter
            string cookieSelHeightMin = getValueFromCookie("selHmin");
            string cookieSelHeightMax = getValueFromCookie("selHmax");

            int selHeightMin = -1;
            int selHeightMax = -1;
            if (int.TryParse(cookieSelHeightMin, out selHeightMin) && int.TryParse(cookieSelHeightMax, out selHeightMax))
            {
                selHmin = selHeightMin;
                selHmax = selHeightMax;
            }

            if (selHmin != heightMin || selHmax != heightMax)
            {
                PlantCharacteristic heightFilter = new PlantCharacteristic
                {
                    Category = pccc.DbGetPlantCharacteristicCategories().Where(c => c.Id == 1).FirstOrDefault(),
                    CategoryId = 1,
                    Min = selHmin,
                    Max = selHmax
                };

                filterCharacteristics.Add(heightFilter);
            }

            string selectedFloweringMonth_str = getValueFromCookie("fm");

            List<PlantSearchViewModel.CheckboxHelper> monthCheckboxes = new List<PlantSearchViewModel.CheckboxHelper>();
            monthCheckboxes = getFloweringMonthList(selectedFloweringMonth_str);

            // add flowering month filter
            if (!String.IsNullOrEmpty(selectedFloweringMonth_str))
            {
                var checkedMonth = monthCheckboxes.Where(m => m.Checked).Select(m => m.Value);
                if (checkedMonth != null && checkedMonth.Any())
                {
                    foreach (int month in checkedMonth)
                    {
                        PlantCharacteristic floweringMonthFilter = new PlantCharacteristic
                        {
                            Category = pccc.DbGetPlantCharacteristicCategories().Where(c => c.Id == 2).FirstOrDefault(),
                            CategoryId = 2,
                            Min = month
                        };

                        filterCharacteristics.Add(floweringMonthFilter);
                    }
                }
            }

            if (plants != null && plants.Any())
            {
                foreach (Plant plant in plants)
                {
                    plant.PlantCharacteristics = pcc.DbGetPlantCharacteristicsByPlantId(plant.Id);
                }

                plants = sortOutPlantsByCharacteristics(plants, filterCharacteristics);
                plants = plants.Where(m => m.Published);
            }

            // process possible tag categories and characteristics from sorted plant list

            // process plant tag categories
            List<PlantTagCategory> possibleCategories = getPossiblePlantTagCategoriesByPlantTags(plants, positiveFilterTags);

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

            IEnumerable<SearchQuery> searchQueries = sqc.DbGetSearchQueriesByUserId(userId);

            PlantSearchViewModel searchViewModel = new PlantSearchViewModel
            {
                CategoryId = selectedCategoryId,
                CategoryList = categoryList,
                SubCategoryId = selectedSubCategoryId,
                SubCategoryList = subCategoryList,
                SearchQueries = searchQueries,
                TagsList = tagsList,
                HeightMax = heightMax,
                HeightMin = heightMin,
                Input_search = searchText,
                MonthCheckboxes = monthCheckboxes,
                PlantsOld = plantsToPlantViewModels(plants, Url.Content("~/")),
                PositiveFilterTagsList = positiveFilterTags,
                SelHmax = selHmax,
                SelHmin = selHmin,
                TaxonId = taxonId,
                TreeRoot = treeRoot,
            };

            return searchViewModel;
        }

        public ActionResult ajaxGetPlantSearchResult()
        {
            Guid userId = Utilities.GetUserId();
            return PartialView("_searchPageContent", getSearchViewModel(userId));
        }

        public ActionResult ajaxGetPlantSearchControls()
        {
            Guid userId = Utilities.GetUserId();
            return PartialView("_searchControls", getSearchViewModel(userId));
        }
    }
}