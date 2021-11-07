using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GardifyModels.Models;
using PflanzenApp.App_Code;
using Microsoft.AspNet.Identity;
using static GardifyModels.Models.ModelEnums;
using PflanzenApp.Services;
using System.Threading.Tasks;
using MetadataExtractor;
using XperiCode.JpegMetadata;
using System.IO;
using System.Drawing;
using System.Drawing.Imaging;
using RestSharp;

namespace PflanzenApp.Controllers.AdminArea
{
    [CustomAuthorizeAttribute(Roles = "Admin,Expert")]
    public class AdminAreaPlantController : _BaseController
    {
        public readonly int PAGE_SIZE = 60;
        //Get: PlantImages and Names
        public ActionResult Galerie(string filterBy=null)
        {
            ViewBag.FilterBy = filterBy;
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            List<_HtmlImageViewModel> imagesList = new List<_HtmlImageViewModel>();
            HelperClasses.DbResponse responseObject = rc.DbGetReferencesToFiles((int)ModelEnums.ReferenceToModelClass.Plant, filterBy);
            if (responseObject.ResponseObjects != null && responseObject.ResponseObjects.Any())
            {
                imagesList = Utilities.getHtmlImageObjectsFromDbImageResponse(responseObject, Url.Content("~/"));
            }
          
            return View("~/Views/AdminArea/AdminAreaPlant/Galerie.cshtml", new ImagesListViewModel { Images=imagesList } );

        }
        // GET: intern/plant
        public ActionResult Index(_modalStatusMessageViewModel statusMessage = null, bool notPublished = false, bool published = false, string name = null, string orderBy = null, string filterBy = null, int page = 1)
        {
            ViewBag.Name = name;
            ViewBag.OrderBy = orderBy;
            ViewBag.NotPublished = notPublished;
            ViewBag.Published = published;
            ViewBag.FilterBy = filterBy;
            PlantController pc = new PlantController();
            PlantCharacteristicController pcc = new PlantCharacteristicController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            PlantViewModels.PlantListViewModel viewModel = new PlantViewModels.PlantListViewModel();
            InternalCommentController icc = new InternalCommentController();
            AdminAreaTodoController tc = new AdminAreaTodoController();
            AlertController ac = new AlertController();



            var plantList = (from p in ctx.Plants
                            where !p.Deleted
                            orderby p.NameGerman
                            select p);

        


            if (!string.IsNullOrEmpty(name))
            {
                var searchText = name.Replace("-", "").Replace(" ", "").Replace("'", "").Trim().ToLower();
                // handle common plural cases ending in "en"
                searchText = searchText.EndsWith("en") ? searchText.Remove(searchText.Length - 1) : searchText;
                var byteText = System.Text.Encoding.GetEncoding("ISO-8859-8").GetBytes(name);
                var normalizedSearchText = System.Text.Encoding.UTF8.GetString(byteText);

                if(searchText != normalizedSearchText)
                {
                    var listPlant = plantList;
                }
                //var q = name.Replace("-", " ").ToLower();
                plantList = plantList
                    .Where(p => (p.NameGerman != null && p.NameGerman.Replace("-", "").Replace("'", "").Replace("‘", "").Replace("’", "").Replace(" ", "").ToLower().Contains(searchText))
                            || p.NameLatin.Replace("-", "").Replace("'", "").Replace("‘", "").Replace("’", "").Replace(" ", "").ToLower().Contains(searchText)
                            || (p.Synonym != null && p.Synonym.Replace("×", "").Replace("-", "").Replace("'", "").Replace("‘", "").Replace("’", "").Replace(" ", "").ToLower().Contains(searchText))
                            || p.Id.ToString() == searchText).OrderBy(p => p.NameGerman);
            }
            if (notPublished)
            {
                plantList = plantList.Where(p => !p.Published).OrderBy(n => n.NameGerman);
            }
            if (published)
            {
                plantList = plantList.Where(p => p.Published).OrderBy(n => n.NameGerman);
            }

            List<PlantViewModels.PlantViewModel> plantVms = new List<PlantViewModels.PlantViewModel>();

            if (plantList != null && plantList.Any())
            {
                foreach (Plant plant in plantList.Skip((page - 1) * PAGE_SIZE).Take(PAGE_SIZE))
                {
                    var comments = icc.DBGetInternalComments(ModelEnums.ReferenceToModelClass.Plant, plant.Id);
                    var todoTemplates = tc.DbGetTodoTemplates(plant.Id, ModelEnums.ReferenceToModelClass.Plant);
                    PlantViewModels.PlantViewModel entryView = new PlantViewModels.PlantViewModel
                    {
                        Id = plant.Id,
                        NameLatin = plant.NameLatin,
                        NameGerman = plant.NameGerman,
                        Description = plant.Description,
                        PlantTags = plant.PlantTags,
                        Published = plant.Published,
                        Comments = comments,
                        TodoTemplates = todoTemplates,
                        Family = plant.Familie,
                        Herkunft = plant.Herkunft,
                        Synonym = plant.Synonym
                    };

                    entryView.GardenCategory = ctx.GardenCategories.FirstOrDefault(c => c.Id == plant.GardenCategoryId);

                    entryView.PlantGroupsOld = ctx.Groups.Where(g => !g.Deleted && g.PlantsWithThisGroupd.Any(p => p.Id == plant.Id));

                    entryView.PlantCharacteristicsOld = pcc.DbGetPlantCharacteristicsByPlantId(plant.Id);

                    HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages(plant.Id);
                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                    {
                        entryView.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
                    }
                    else
                    {
                        entryView.Images.Add(new _HtmlImageViewModel
                        {
                            SrcAttr = Url.Content("~/Images/no-image.png"),
                            Id = 0,
                            TitleAttr = "Kein Bild vorhanden"
                        });
                    }

                    plantVms.Add(entryView);
                }
            }

            switch (filterBy)
            {
                case "comments":
                    plantVms = plantVms.Where(p => p.Comments != null && p.Comments.Any()).ToList();
                    break;
            }

            switch (orderBy)
            {
                case "BotanicName":
                    plantVms = plantVms.OrderBy(p => p.NameLatin).ToList();
                    break;
                case "GermanName":
                    plantVms = plantVms.OrderBy(p => p.NameGerman).ToList();
                    break;
                default: break;
            }
            viewModel.ListEntries = plantVms;

            var totalPages = Math.Max(1, (int)Math.Ceiling(Decimal.Divide(plantList.Count(), PAGE_SIZE)));

            if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(filterBy))
            {
                totalPages = Math.Max(1, (int)Math.Ceiling(Decimal.Divide(plantVms.Count(), PAGE_SIZE)));
            }
            
            var firstPage = 1;
            var lastPage = totalPages;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;
            ViewBag.prevPage = Math.Max(page - 1, firstPage);
            ViewBag.nextPage = Math.Min(page + 1, lastPage);
            ViewBag.totalPages = totalPages;
            ViewBag.currentPage = page;
            viewModel.StatusMessage = statusMessage;
            //SaveImagesTags();
            return View("~/Views/AdminArea/AdminAreaPlant/Index.cshtml", viewModel);
        }
        public ActionResult SaveImagesTags(int plantId=0)
        {
            
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            HelperClasses.DbResponse imageResponse = null;
            if (plantId > 0)
            {
                imageResponse = rc.DbGetReferencesToFileByObjectId(plantId, (int)ModelEnums.ReferenceToModelClass.Plant);

            }
            else 
            {
                imageResponse = rc.DbGetAllReferencesToFile((int)ModelEnums.ReferenceToModelClass.Plant);

            }
            if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            {
                System.Net.WebClient client = new WebClient();
                string folderPath = Server.MapPath("~/ImagesFolder/");  //Create a Folder in your Root directory on your solution.
                var images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("https://gardify.de/intern/"));
                //var images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));

                foreach (var img in images)
                {
                    string imageUrl = img.SrcAttr;
                    Uri uri = null;
                    string lastSegment="";
                    if (imageUrl.Contains("https://gardify.de/intern"))
                    {
                         uri = new Uri(imageUrl);
                        lastSegment = uri.Segments.Last();
                    }
                    else
                    {
                        lastSegment= Path.GetFileName(imageUrl);
                    }
                    
                    string fileName = lastSegment;
                    string imagePath = folderPath + fileName;
                    
                    string ext = Path.GetExtension(imagePath).ToLower();
                    if (ext != ".jpg")
                    {
                        if (imageUrl.Contains("https://gardify.de/intern"))
                        {
                           
                            try
                            {
                                client.DownloadFile(new Uri(imageUrl), imagePath);
                            }
                            catch (WebException e)
                            {
                                var statusCode = ((HttpWebResponse)e.Response).StatusCode;

                                if (statusCode == HttpStatusCode.NotFound && System.IO.File.Exists(imagePath))
                                {
                                    System.IO.File.Delete(imagePath);
                                    //maybe log the occurence as well
                                }
                            }

                        }
                        string name = Path.GetFileNameWithoutExtension(imagePath);
                        string path = Path.GetDirectoryName(imagePath);
                        if (System.IO.File.Exists(imagePath))
                        {
                            Image otherExt = Image.FromFile(imagePath);
                            otherExt.Save(path + @"/" + name + ".jpg", ImageFormat.Jpeg);
                            otherExt.Dispose();
                            System.IO.File.Delete(imagePath);
                            imagePath = path + @"/" + name + ".jpg";
                        }
                        

                    }
                    else if(imageUrl.Contains("https://gardify.de/intern"))
                    {
                        try
                        {
                            client.DownloadFile(new Uri(imageUrl), imagePath);
                        }
                        catch (WebException e)
                        {
                            var statusCode = ((HttpWebResponse)e.Response).StatusCode;

                            if (statusCode == HttpStatusCode.NotFound && System.IO.File.Exists(imagePath))
                            {
                                System.IO.File.Delete(imagePath);
                                //maybe log the occurence as well
                            }
                        }

                    }
                    if (System.IO.File.Exists(imagePath))
                    {
                        var directories = ImageMetadataReader.ReadMetadata(imagePath);
                        JpegMetadataAdapter metaAdapter = new JpegMetadataAdapter(imagePath);
                        var outputString = "";
                        foreach (string item in metaAdapter.Metadata.Keywords)
                        {
                            outputString += string.Format("{0}", item);
                            outputString += ";";
                        }
                        TagToImage imagetag = new TagToImage()
                        {
                            ImageId = img.Id
                        };
                        imagetag.Tags = outputString;
                        foreach (var directory in directories)
                        {
                            foreach (var tag in directory.Tags)
                            {
                                if (tag.Name == "Copyright")
                                {
                                    Console.WriteLine($"[{directory.Name}] {tag.Name} = {tag.Description}");
                                    imagetag.Copyright = tag.Description;
                                    break;

                                }

                                if (directory.HasError)
                                {
                                    foreach (var error in directory.Errors)
                                        Console.WriteLine($"ERROR: {error}");
                                }

                            }

                        }
                        var existTag = (from t in ctx.TagToImages where t.ImageId == img.Id select t).FirstOrDefault();
                        if (existTag == null)
                        {
                            imagetag.OnCreate(Utilities.GetUserName());
                            var result = ctx.TagToImages.Add(imagetag);
                        }
                    }
                        
                    
                }
                ctx.SaveChanges();
            }
            

            return RedirectToAction("Index");
        }
        public ActionResult SetPublishedState(int id, bool published)
        {
            var plant = ctx.Plants.FirstOrDefault(p => !p.Deleted && p.Id == id);
            plant.Published = published;
            ctx.SaveChanges();
            return RedirectToAction("Index");
        }

        public bool MergeAllUserPlant(int originalPlantId, int destinationPlantId)
        {
            PlantController pc = new PlantController();

            var plants = ctx.Plants.ToList();

            //delete Plant and all relation
            UserPlantController upc = new UserPlantController();

            var allUserPlant = ctx.UserPlants.Where(u => u.PlantId == originalPlantId);

            Plant originalPlant = plants.FirstOrDefault(p => p.Id == originalPlantId);
            Plant destinationPlant = plants.FirstOrDefault(p => p.Id == destinationPlantId);
            if(originalPlant == null || destinationPlant == null)
            {
                return false;
            }
            try
            {
                foreach (var user in allUserPlant)
                {
                    upc.DbDeleteUserPlantGarden(originalPlantId, user.Gardenid);


                    var userList = ctx.UserLists.FirstOrDefault(u => u.GardenId == user.Gardenid && !u.Deleted);
                    var userProperty = ctx.Property.FirstOrDefault(p => user.Garden.PropertyId == p.Id);

                    var newPlantList = new UserPlantToUserListView
                    {
                        Count = user.Count,
                        UserListId = userList.Id

                    };
                    UserPlantToUserListView[] uPlantTrigger = { newPlantList };



                    UserPlant res = upc.DbAddPlantToPropertyComplete(destinationPlantId, user.Gardenid, userProperty.UserId, user.CreatedBy, user.InitialAgeInDays, user.Count, user.IsInPot, true, uPlantTrigger);

                    var client = new RestClient("https://gardifybackend.sslbeta.de/api/userplantsapi/deletecache?userId=" + user.Garden.Property.UserId);
                    client.Timeout = -1;
                    var request = new RestRequest(Method.GET);
                    IRestResponse response = client.Execute(request);
                }


                //PlantSuggestMerge newProfile = new PlantSuggestMerge
                //{
                //    SuggestedPlantId = originalPlantId,
                //    ExistingPlantId = destinationPlantId
                //};

                //ctx.PlantSuggestMerges.Add(newProfile);
                //ctx.SaveChanges();
            }
            catch(Exception e)
            {
                var er = e;
                return false;
            }
           



            return true;
        }




        // GET: intern/plant/Create
        public ActionResult Create()
        {
            AdminAreaViewModels.PlantCreateViewModel viewModel = new AdminAreaViewModels.PlantCreateViewModel();
            AdminAreaContentContributionController aaccc = new AdminAreaContentContributionController();
            PlantController pc = new PlantController();
            var plants = pc.DbGetPlantList();
            List<IReferencedObject> list = new List<IReferencedObject>();
            list.AddRange(plants);
            viewModel.PlantList = list;
            ViewBag.GardenCategories = GetGardenCategoriesSelectList();

            return View("~/Views/AdminArea/AdminAreaPlant/Create.cshtml", viewModel);
        }

        // POST: intern/plant/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "nameLatin,nameGerman,description,SelectedPlantId, InternalComment, Herkunft")] AdminAreaViewModels.PlantCreateViewModel plantData, string gardenCategories, string articlesRef = "")
        {
            PlantController pc = new PlantController();
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            int? gardenCategoryId = null;
            if (gardenCategories != "-1")
            {
                int p = -1;
                int.TryParse(gardenCategories, out p);
                if (p != -1)
                {
                    gardenCategoryId = p;
                }
            }
            ViewBag.GardenCategories = GetGardenCategoriesSelectList(gardenCategoryId);
            if (ModelState.IsValid)
            {
                Plant newPlant = new Plant();
                newPlant.NameLatin = plantData.NameLatin;
                newPlant.NameGerman = plantData.NameGerman;
                newPlant.Herkunft = plantData.Herkunft;
                newPlant.GardenCategoryId = gardenCategoryId;
                newPlant.Description = plantData.Description == null ? "Keine Beschreibung verfügbar." : plantData.Description;
                newPlant.OnCreate(Utilities.GetUserName());

                HelperClasses.DbResponse response = pc.DbCreatePlant(newPlant);
                if (response.Status == ModelEnums.ActionStatus.Success)
                {
                    if (plantData.SelectedPlantId > 0)
                    {
                        copyCharacteristicsFromPlant(newPlant.Id, plantData.SelectedPlantId);
                        copyTagFromPlant(newPlant.Id, plantData.SelectedPlantId);
                        copyAlertsFromPlant(newPlant.Id, plantData.SelectedPlantId);
                    }
                    if (!string.IsNullOrEmpty(plantData.InternalComment))
                    {
                        InternalCommentController icc = new InternalCommentController();
                        if (!icc.Create(plantData.InternalComment, newPlant.Id, ModelEnums.ReferenceToModelClass.Plant))
                        {
                            statusMessage.Messages.Add("Interne Bemerkung konnte nicht erstellt werden.");
                        }
                    }
                    if (!String.IsNullOrEmpty(articlesRef))
                    {
                        foreach (string idStr in articlesRef.Split(','))
                        {
                            int articleId = 0;
                            if (Int32.TryParse(idStr, out articleId) && articleId > 0)
                            {
                                AddArticleReference(articleId, newPlant.Id);
                            }
                        }
                    }
                    statusMessage.Messages.Add("Pflanze \"" + newPlant.NameGerman + " (" + newPlant.NameLatin + ")\" wurde erfolgreich erstellt. Sie können diese Pflanze jetzt bearbeiten");
                    statusMessage.Status = ModelEnums.ActionStatus.Success;
                    TempData["statusMessage"] = statusMessage;
                    return RedirectToAction("Edit", new { id = ((Plant)response.ResponseObjects.FirstOrDefault()).Id });
                }
                else
                {
                    statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
                    statusMessage.Status = response.Status;
                    plantData.StatusMessage = statusMessage;
                    var plants = pc.DbGetPlantList();
                    List<IReferencedObject> list = new List<IReferencedObject>();
                    list.AddRange(plants);
                    plantData.PlantList = list;
                    return View("~/Views/AdminArea/AdminAreaPlant/Create.cshtml", plantData);
                }
            }

            statusMessage.Messages.Add("Eingabe ist falsch");
            statusMessage.Status = ModelEnums.ActionStatus.Error;
            plantData.StatusMessage = statusMessage;
            var plants2 = pc.DbGetPlantList();
            List<IReferencedObject> list2 = new List<IReferencedObject>();
            list2.AddRange(plants2);
            plantData.PlantList = list2;
            return View("~/Views/AdminArea/AdminAreaPlant/Create.cshtml", statusMessage);
        }

        [HttpPost]
        [ActionName("AddArticleReference")]
        public ActionResult AddArticleReference(int articleId, int plantId)
        {
            ArticleReference newRef = new ArticleReference
            {
                ArticleId = articleId,
                ReferenceType = ArticleReferenceType.Plant,
                PlantId = plantId
            };

            newRef.OnCreate("SYSTEM");

            ctx.ArticleReference.Add(newRef);
            ctx.SaveChanges();

            return RedirectToAction("edit", new { id = plantId });
        }

        public ActionResult DeleteArticleReference(int articleId, int articleReferenceId)
        {
            var toRemove = ctx.ArticleReference.Where(r => r.ArticleId == articleId && r.PlantId == articleReferenceId).FirstOrDefault();
            if (toRemove != null)
            {
                ctx.ArticleReference.Remove(toRemove);
                ctx.SaveChanges();
            }

            return RedirectToAction("edit", new { id = articleReferenceId });
        }

        public IEnumerable<SelectListItem> GetGardenCategoriesSelectList(int? selectedCategoryId = -1)
        {
            var categories = ctx.GardenCategories.Where(g => !g.Deleted);
            var list = categories.Select(c => new SelectListItem { Text = c.ParentCategory.Name + " - " + c.Name, Value = c.Id.ToString(), Selected = c.Id == selectedCategoryId }).OrderBy(c => c.Text);
            return list.Prepend(new SelectListItem { Text = "Keine Kategorie", Value = "-1" });
        }

        // GET: intern/plant/Edit/5
        public ActionResult Edit(int? id, bool saveimagetags=false)
        {
            if (saveimagetags == true)
            {
                SaveImagesTags();
            }
            PlantController pc = new PlantController();
            PlantCharacteristicController pcc = new PlantCharacteristicController();
            PlantCharacteristicCategoryController pccc = new PlantCharacteristicCategoryController();
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
            PlantTagController ptc = new PlantTagController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            AlertController ac = new AlertController();
            InternalCommentController icc = new InternalCommentController();



            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Plant plant = pc.DbGetPlantById((int)id);
            //if (plant.GardenCategoryId != null)
            //{
                ViewBag.GardenCategories = GetGardenCategoriesSelectList(plant.GardenCategoryId == null ? -1 : plant.GardenCategoryId);

            //}
            ViewBag.ArticlesList = ctx.Articles.Where(r => !r.Deleted);
            ViewBag.ArticleReferences = ctx.ArticleReference.Where(r => !r.Deleted && r.PlantId == id);
            ViewBag.PlantGroup = ctx.Groups.Where(g => !g.Deleted);
            if (plant == null)
            {
                return HttpNotFound();
            }

            


            AdminAreaViewModels.PlantViewModel plantViewModel = new AdminAreaViewModels.PlantViewModel();
            plantViewModel.ImportHistory = ctx.ImportHistory.Where(m => m.PlantId == plant.Id && !m.Deleted).FirstOrDefault();
            plantViewModel.Plant = plant;
            plantViewModel.Plant.PlantCharacteristics = pcc.DbGetPlantCharacteristicsByPlantId((int)id);
            plantViewModel.PlantTagsList = ptc.DbGetPlantTags();
            plantViewModel.PlantInternalComments = icc.GetInternalCommentIndexViewModel(ModelEnums.ReferenceToModelClass.Plant, plant.Id);
            var list_Tags = new List<PlantTag>();
            list_Tags = (from p in plantViewModel.Plant.PlantTags
                         orderby p.Category.Title ascending
                         select new PlantTag
                         {
                             Id = p.Id,
                             CategoryId = p.CategoryId,
                             Category = p.Category,
                             Title = /*p.Category.Title + ": " +*/ p.Title,
                             Selected = true
                         }).ToList();
            if (list_Tags.Count() != 0)
            {
                var listNotInPlant = plantViewModel.PlantTagsList.Where(m => !list_Tags.Any(v => v.Id == m.Id));
                var list_tagsByViewModel = (from p in listNotInPlant
                                            orderby p.Category.Title ascending
                                            select new PlantTag
                                            {
                                                Id = p.Id,
                                                CategoryId = p.CategoryId,
                                                Category = p.Category,
                                                //plantsWithThisTag = p.plantsWithThisTag,
                                                Title = /*p.Category.Title + ": " +*/ p.Title,
                                                Selected = false
                                            }).ToList();
                list_Tags.AddRange(list_tagsByViewModel);
                list_Tags = list_Tags.OrderBy(m => m.Category.Title).ToList();
                plantViewModel.PlantTagsList = list_Tags;
            }
            else
            {
                plantViewModel.PlantTagsList = (from p in plantViewModel.PlantTagsList
                                                orderby p.Category.Title ascending
                                                select new PlantTag
                                                {
                                                    Id = p.Id,
                                                    CategoryId = p.CategoryId,
                                                    Category = p.Category,
                                                    //plantsWithThisTag = p.plantsWithThisTag,
                                                    Title = /*p.Category.Title + ": " +*/ p.Title,
                                                    Selected = false
                                                }).ToList();
            }
            var list_CharacteristicCategory = new List<PlantCharacteristicCategory>();
            list_CharacteristicCategory.Add(new PlantCharacteristicCategory
            {
                Id = -1,
                Title = "Bitte ein Merkmal auswählen"
            });
            var list_CharacteristicCategoriesByViewModel = pccc.DbGetPlantCharacteristicCategories();
            list_CharacteristicCategory.AddRange(list_CharacteristicCategoriesByViewModel);

            plantViewModel.characteristicCategories = list_CharacteristicCategory;
            //Add Todo Stuff
            AdminAreaTodoController aatc = new AdminAreaTodoController();
            var refs = aatc.DbGetReferencedItems();
            var vm = new TodoTemplateViewModels.TodoTemplateCreateViewModel
            {
                InfoObjects = refs
            };
            vm.ReferenceId = new int[]{ plant.Id};
            vm.ReferenceType = ModelEnums.ReferenceToModelClass.Plant;
            plantViewModel.NewTodoTemplate = vm;

            var ttl = aatc.GetTodoTemplateIndexViewModelByPlantId(plant.Id);
            plantViewModel.TodoTemplateIndexViewModel = ttl;

            //Add Tax.Tree Stuff
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            var setNode = ttc.GetTreeNodeByPlantId(plant.Id);

            AdminAreaViewModels.TaxonomicTreeInsertViewModel ttivm = new AdminAreaViewModels.TaxonomicTreeInsertViewModel
            {
                AvailableTrees = ttc.DbGetAllLeafParents(),
                PlantId = plant.Id,
                TreeId = setNode == null ? -1 : setNode.ParentId
            };
            plantViewModel.TaxonomicTreeInsertViewModel = ttivm;

            HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages((int)id);

            plantViewModel.PlantImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));

            plantViewModel.Plant.Alerts = ac.DbGetAlertsByRelatedObjectId((int)id, ModelEnums.ReferenceToModelClass.Plant);

            plantViewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;

            return View("~/Views/AdminArea/AdminAreaPlant/Edit.cshtml", plantViewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-plant-image")]
        public ActionResult UploadPlantImage(HttpPostedFileBase imageFile, int plantId, string imageTitle = null, string imageDescription = null, string imageLicense = null, string imageAuthor = null)
        {
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            if (imageDescription == null || imageDescription == "")
            {
                imageDescription = "Keine Beschreibung vorhanden.";
            }
            if (imageFile == null || imageFile.ContentLength <= 0)
            {
                statusMessage.Messages.Add("Fehler beim Upload");
                statusMessage.Status = ModelEnums.ActionStatus.Error;
                //return
            }
            else
            {
                UploadAndRegisterFile(imageFile, plantId, (int)ModelEnums.ReferenceToModelClass.Plant, ModelEnums.FileReferenceType.PlantImage, imageTitle, imageDescription, imageLicense, imageAuthor);
                SaveImagesTags(plantId);
            }

            TempData["statusMessage"] = statusMessage;

            return RedirectToAction("Edit", new { id = plantId });
        }


        public ActionResult MarkFinished(int id, int plantId)
        {
            InternalCommentController icc = new InternalCommentController();
            icc.DbSetInternalCommentFinished(id);
            return RedirectToAction("Edit", new { id = plantId });
        }

        public ActionResult CreateGenerallyComment()
        {
            InternalCommentViewModels.InternalCommentCreateViewModel iccvm = new InternalCommentViewModels.InternalCommentCreateViewModel
            {
                ReferenceID = -1,
                ReferenceType = ModelEnums.ReferenceToModelClass.GeneralInternalComment,
                Text = ""
            };
            return View("~/Views/InternalComment/CreateGeneralComment.cshtml", iccvm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateGenerallyComment(InternalCommentViewModels.InternalCommentCreateViewModel vm)
        {
            InternalCommentController icc = new InternalCommentController();
            icc.Create(vm.Text, vm.ReferenceID, vm.ReferenceType);
            return RedirectToAction("Index", "InternalComment");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("delete-plant-image")]
        public ActionResult DeletePlantImage(int imageId, int plantId)
        {
            //dirty way:
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            try
            {
                nfilesEntities nfilesEntities = new nfilesEntities();
                var image = nfilesEntities.Files.FirstOrDefault(e => e.FileID == imageId);
                var ftm = nfilesEntities.FileToModule.FirstOrDefault(e => e.FileID == imageId);
                nfilesEntities.FileToModule.Remove(ftm);
                nfilesEntities.Files.Remove(image);
                nfilesEntities.SaveChanges();
                statusMessage.Messages = new string[] { "Bild gelöscht" }.ToList();
                statusMessage.Status = ModelEnums.ActionStatus.Success;
            }
            catch
            {
                statusMessage.Messages = new string[] { "Fehler beim löschen" }.ToList();
                statusMessage.Status = ModelEnums.ActionStatus.Error;
            }
            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Edit", new { id = plantId });
            //Old way:
            //ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            //HelperClasses.DbResponse response = rc.DbDeleteFileReference(imageId, User.Identity.GetUserName());
            //_modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            //statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            //statusMessage.Status = response.Status;
            //TempData["statusMessage"] = statusMessage;
            //return RedirectToAction("Edit", new { id = plantId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("edit-image-creds")]
        public ActionResult EditImageCreds(int imageId, int plantId, string imageAuthor = null, string imageLicense = null, string imageDescription = null)
        {
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            try
            {
                nfilesEntities nfilesEntities = new nfilesEntities();
                var image = nfilesEntities.Files.FirstOrDefault(e => e.FileID == imageId);
                image.FileC = imageLicense;
                image.FileD = imageAuthor;
                image.DescriptionDE = imageDescription;
                nfilesEntities.SaveChanges();
                statusMessage.Messages = new string[] { "Bild geändert" }.ToList();
                statusMessage.Status = ModelEnums.ActionStatus.Success;
            }
            catch
            {
                statusMessage.Messages = new string[] { "Fehler beim Ändern" }.ToList();
                statusMessage.Status = ModelEnums.ActionStatus.Error;
            }
            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Edit", new { id = plantId });
        }

        // POST: intern/plant/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Prefix = "plant")] Plant plantToEdit, string gardenCategories, bool notifyUsers = false)
        {
            PlantController pc = new PlantController();
            InternalCommentController icc = new InternalCommentController();
            ModelState.Remove("Plant.createdBy");
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            ViewBag.GardenCategories = GetGardenCategoriesSelectList();

            int? gardenCategoryId = null;
            if (gardenCategories != "-1")
            {
                int p = -1;
                int.TryParse(gardenCategories, out p);
                if (p != -1)
                {
                    gardenCategoryId = p;
                }
            }

            if (ModelState.IsValid)
            {
                plantToEdit.GardenCategoryId = gardenCategoryId;
                plantToEdit.EditedBy = User.Identity.GetUserName();
                HelperClasses.DbResponse response = pc.DbEditPlant(plantToEdit);
                statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
                statusMessage.Status = response.Status;
            }
            else
            {
                statusMessage.Messages.Add("Eingabe ist falsch");
                statusMessage.Status = ModelEnums.ActionStatus.Error;
            }

            if (statusMessage.Status == ActionStatus.Success && notifyUsers && plantToEdit.Published)
            {
                await NotifyUsersOfPlant(plantToEdit);
            }

            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Edit", new { id = plantToEdit.Id });
        }

        [NonAction]
        public async Task<bool> NotifyUsersOfPlant(Plant plant)
        {
            var relevantUsers = ctx.UserPlants.Where(up => up.PlantId == plant.Id && !up.Deleted).Select(up => up.Garden.Property.UserId).Distinct().ToList();
            foreach(var userId in relevantUsers)
            {
                var userSettings = ctx.UsersSettings.Where(us => us.UserId == userId && !us.Deleted).FirstOrDefault();
                if (userSettings == null || !userSettings.AlertByEmail || !userSettings.ActiveNewPlantAlert)
                {
                    continue;
                }

                var user = ctx.Users.Where(u => u.Id == userId.ToString() && !u.Deleted).FirstOrDefault();
                if (user == null || !user.EmailConfirmed)
                {
                    continue;
                }

                var url = $"https://gardify.de/pflanze/{plant.Id}/{plant.NameGerman}";
                TemplateService ts = new TemplateService();
                EmailSender es = new EmailSender(ctx);

                try
                {
                    string content = ts.RenderTemplateAsync("NewPlantAdded", new { UserName = user.FirstName.Contains("Platzhalter") ? user.UserName.Split('@')[0] : user.FirstName, Plant = plant.NameGerman, PlantUrl = url });
                    var res = await es.SendEmail("Neue Pflanze", content, "info@gardify.de", user.Email, null);
                }
                catch (Exception e) { return false; };
            }
            return true;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("addComment")]
        public ActionResult AddComment(string newInternalComment, int plantId)
        {
            InternalCommentController icc = new InternalCommentController();
            icc.Create(newInternalComment, plantId, ModelEnums.ReferenceToModelClass.Plant);
            return RedirectToAction("Edit", new { id = plantId });
        }

        // POST: intern/plant/delete-plant/
        [ActionName("delete-plant")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult deletePlant(int plantId)
        {
            PlantController pc = new PlantController();
            HelperClasses.DbResponse response = pc.DbDeletePlant(plantId, User.Identity.GetUserName());

            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel
            {
                Messages = Utilities.databaseMessagesToText(response.Messages),
                Status = response.Status
            };

            return Index(statusMessage);
        }

        public ActionResult DeletePlant(int plantId)
        {
            return View("~/Views/AdminArea/AdminAreaPlant/Delete.cshtml", new Plant() { Id = plantId });
        }

        // POST: intern/plant/add-tag/
        [ActionName("add-tag")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddTag(int[] checkedTags, Plant plant, PlantCharacteristicViewModel[] plantCharacteristics)
        {
            PlantTagController ptc = new PlantTagController();
            PlantCharacteristicController pcc = new PlantCharacteristicController();
            List<PlantCharacteristic> characteristics = new List<PlantCharacteristic>();
            foreach(var cha in plantCharacteristics)
            {
                if(cha.Min != null)
                {
                    characteristics.Add(new PlantCharacteristic
                    {
                        CategoryId = cha.CategoryId,
                        Min = (decimal)cha.Min,
                        Max = cha.Max,
                        PlantId = plant.Id
                    });
                }
            }
            var existingTags = ctx.Plants.FirstOrDefault(p => p.Id == plant.Id)?.PlantTags;
            if (existingTags != null && existingTags.Any())
            {
                foreach (var existingTag in existingTags)
                {
                    ptc.DbRemoveTagFromPlant(plant.Id, existingTag.Id);
                }
            }
            if (checkedTags != null && checkedTags.Any())
            {
                foreach (var checkedTag in checkedTags)
                {
                    ptc.DbAddTagToPlant(plant.Id, checkedTag);
                }
            }

            var existingCharacteristics = pcc.DbGetPlantCharacteristicsByPlantId(plant.Id).ToList();
            if(existingCharacteristics != null && existingCharacteristics.Any())
            {
                foreach(var cha in existingCharacteristics)
                {
                    pcc.DbDeletePlantCharacteristic(cha.Id, "System");
                }
            }
            if(characteristics != null && characteristics.Any())
            {
                foreach(var cha in characteristics)
                {
                    pcc.DbCreatePlantCharacteristic(cha);
                }
            }
            return RedirectToAction("Edit", new { id = plant.Id });
        }

        // POST: intern/plant/add-tag/
        [ActionName("add-group")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddGroup(int[] checkedGroups, Plant plant)
        {
            AdminAreaGroupsController ptc = new AdminAreaGroupsController();

            var existingGroups = ctx.Plants.FirstOrDefault(p => p.Id == plant.Id)?.PlantGroups;
            if (existingGroups != null && existingGroups.Any())
            {
                foreach (var existingTag in existingGroups)
                {
                    ptc.DbRemoveGroupFromPlant(plant.Id, existingTag.Id);
                }
            }
            if (checkedGroups != null && checkedGroups.Any())
            {
                foreach (var checkedTag in checkedGroups)
                {
                    ptc.DbAddGroupToPlant(plant.Id, checkedTag);
                }
            }
            return RedirectToAction("Edit", new { id = plant.Id });
        }

        // POST: intern/plant/remove-tag/
        [ActionName("remove-tag")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult removeTag(int plantId, int tagId)
        {
            PlantTagController ptc = new PlantTagController();
            ptc.DbRemoveTagFromPlant(plantId, tagId);
            return RedirectToAction("Edit", new { id = plantId });
        }

        // POST: intern/plant/add-characteristic/
        [ActionName("add-characteristic")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCharacteristic(PlantCharacteristic newCharacteristic)
        {
            if (newCharacteristic.Id == -1)
            {
                return RedirectToAction("Edit", new { id = newCharacteristic.PlantId });
            }
            PlantCharacteristicController pcc = new PlantCharacteristicController();
            newCharacteristic.CreatedBy = User.Identity.GetUserName();
            pcc.DbCreatePlantCharacteristic(newCharacteristic);
            return RedirectToAction("Edit", new { id = newCharacteristic.PlantId });
        }

        // POST: intern/plant/add-characteristic/
        [ActionName("add-todotemplate")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddCharacteristic(TodoTemplateViewModels.TodoTemplateCreateViewModel vm)
        {
            if (ModelState.IsValid)
            {
                AdminAreaTodoController aatc = new AdminAreaTodoController();
                aatc.DbCreateTodoTemplate(vm);
                return RedirectToAction("Edit", new { id = vm.ReferenceId });
            }
            else
            {
                return RedirectToAction("Edit", vm.ReferenceId);
            }
        }

        // POST: intern/plant/add-characteristic/
        [ActionName("set-taxonomictree")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SetTaxonomicTree(AdminAreaViewModels.TaxonomicTreeInsertViewModel vm)
        {
            if (ModelState.IsValid)
            {
                PlantController pc = new PlantController();
                var plant = pc.DbGetPlantById(vm.PlantId);

                TaxonomicTreeController ttc = new TaxonomicTreeController();
                //Check if old taxon exists
                var oldTaxon = ttc.GetTreeNodeByPlantId(vm.PlantId);
                if (oldTaxon != null)
                {
                    ttc.DbDeleteTaxonomicTreeNode(oldTaxon.Id, Utilities.GetUserName());
                }
                if (vm.TreeId != -1)
                {
                    var taxon = ttc.DbGetTreeNodeById(vm.TreeId);
                    TaxonomicTree node = new TaxonomicTree
                    {
                        ParentId = vm.TreeId,
                        PlantId = vm.PlantId,
                        TitleLatin = plant.NameLatin,
                        TitleGerman = plant.NameGerman,
                        Type = ModelEnums.NodeType.Leaf,
                        Taxon = ModelEnums.TaxonomicRank.Species,
                        RootID = taxon.RootID
                    };
                    node.OnCreate(Utilities.GetUserName());

                    ttc.DbCreateTaxonomicTreeNode(node);
                }
                return RedirectToAction("Edit", new { id = vm.PlantId });
            }
            else
            {
                return RedirectToError("Das übergebene Model ist nicht korrekt", HttpStatusCode.InternalServerError, "ModelState is invalid");
            }
        }
        // POST: intern/plant/remove-characteristic/
        [ActionName("remove-characteristic")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult removeCharacteristic(int plantId, int characteristicId)
        {
            PlantCharacteristicController pcc = new PlantCharacteristicController();
            pcc.DbDeletePlantCharacteristic(characteristicId, User.Identity.GetUserName());
            return RedirectToAction("Edit", new { id = plantId });
        }

        public bool copyCharacteristicsFromPlant(int ownId, int parentPlantId)
        {
            PlantController pc = new PlantController();
            PlantCharacteristicController pcc = new PlantCharacteristicController();
            Plant plant = pc.DbGetPlantById(ownId);
            var charas = DbCopyPlantCharacteristicsByPlantId(parentPlantId).ToList();
            bool x = true;
            foreach (var ch in charas)
            {
                PlantCharacteristic chara = new PlantCharacteristic
                {
                    Category = null,
                    Deleted = false,
                    CategoryId = ch.CategoryId,
                    CreatedBy = "CopyCharacteristicsFromPlant",
                    CreatedDate = DateTime.Now,
                    EditedBy = "CopyCharacteristicsFromPlant",
                    EditedDate = DateTime.Now,
                    Max = ch.Max,
                    Min = ch.Min,
                    PlantId = ownId,
                    Plant = null
                };
                x = x && pcc.DbCreatePlantCharacteristic(chara);
            }
            return x;
        }

        public bool copyTagFromPlant(int ownId, int parentPlantId)
        {
            PlantTagController ptc = new PlantTagController();
            var tags = ptc.DBGetPlantTagsByPlantId(parentPlantId);
            bool x = true;
            foreach (var tag in tags)
            {
                x = x && ptc.DbAddTagToPlant(ownId, tag.Id);
            }
            return x;
        }

        public bool copyAlertsFromPlant(int ownId, int parentPlantId)
        {
            AlertController ac = new AlertController();
            var alerts = ac.DbGetAlertsByRelatedObjectId(parentPlantId, ModelEnums.ReferenceToModelClass.Plant).ToList();
            bool x = true;
            foreach (var alert in alerts)
            {
                Alert al = new Alert
                {
                    ObjectType = alert.ObjectType,
                    RelatedObjectId = ownId,
                    Text = alert.Text,
                    Title = alert.Title,
                    Deleted = false,
                    CreatedBy = "copyAlertsFromPlant",
                    CreatedDate = DateTime.Now,
                    EditedBy = "copyAlertsFromPlant",
                    EditedDate = DateTime.Now
                };
                x = x && (ac.DbCreateAlert(al).Status == ModelEnums.ActionStatus.Success);
                var OldTriggers = ac.DbGetAlertTriggersByAlertId(alert.Id).ToList();
                var NewTriggers = ac.DbGetAlertTriggersByAlertId(al.Id).SingleOrDefault();
                foreach (var trig in OldTriggers)
                {
                    var conditions = trig.Conditions.ToList();
                    foreach (var condition in conditions)
                    {
                        AlertCondition cond = new AlertCondition
                        {
                            CreatedBy = "CopyAlertsFromPlant",
                            CreatedDate = DateTime.Now,
                            DateValue = condition.DateValue,
                            Deleted = false,
                            EditedBy = "CopyAlertsFromPlant",
                            EditedDate = DateTime.Now,
                            FloatValue = condition.FloatValue,
                            ReadableCondition = condition.ReadableCondition,
                            Trigger = NewTriggers,
                            TriggerId = NewTriggers.Id,
                            ValueType = condition.ValueType,
                            ComparisonOperator = condition.ComparisonOperator
                        };
                        ac.DbCreateAlertCondition(cond);
                    }
                }

            }
            return x;
        }

        #region DB

        //Characteristic (PlantCharacteristics)
        [NonAction]
        public IEnumerable<PlantCharacteristic> DbCopyPlantCharacteristicsByPlantId(int plantId)
        {
            PlantCharacteristicController pcc = new PlantCharacteristicController();
            var characteristics = pcc.DbGetPlantCharacteristicsByPlantId(plantId);
            return characteristics;
        }




        //Tags
        #endregion
    }
}
