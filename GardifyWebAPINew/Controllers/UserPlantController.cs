using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using Microsoft.AspNet.Identity;
using System.IO;
using System.Collections.Generic;
using Microsoft.AspNet.Identity.Owin;
using static GardifyModels.Models.UserPlantViewModels;
using static GardifyModels.Models.ModelEnums;
using System.Data.Entity;
using System.Threading.Tasks;
using System.Text.Json;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using ActionNameAttribute = System.Web.Mvc.ActionNameAttribute;
using NonActionAttribute = System.Web.Http.NonActionAttribute;
using System.Net.Http;
using Newtonsoft.Json;

namespace GardifyWebAPI.Controllers
{
    public class UserPlantController : _BaseController
    {
        private ImgResizerController imgResizer = new ImgResizerController();
        private PlantSearchController sc = new PlantSearchController();
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: UserPlants
        public IEnumerable<UserPlantDetailsViewModelCount> Index(bool withTags = true, bool withCyclicTodos = true, bool withImages = true, int listId=0, int skip = 0, int take = -1)
        {
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return new List<UserPlantDetailsViewModelCount>();
            }
            GardenController gc = new GardenController();
            var mainGarden = gc.DbGetGardensByUserId(userId).FirstOrDefault();

            IEnumerable<UserPlantDetailsViewModelCount> viewModel = GroupedUserplantToUserList(mainGarden.Id, withTags, withCyclicTodos, withImages,listId, skip, take);
            //var orderedPlantlist = viewModel.OrderBy(x => x.UserPlant.NameLatin.Contains("[k]") ? x.UserPlant.NameLatin.Split(new string[] { "[k]" }, StringSplitOptions.None)[1] : x.UserPlant.NameLatin).ToList();
            return viewModel;
        }

        public IEnumerable<UserPlantDetailsViewModelCount> GetCachedIndex(string cachedFilePath, bool getShort = false)
        {
            string fileContents = "";
            UserPlantController pc = new UserPlantController();

            //return null;
            if (getShort)
            {
                cachedFilePath = cachedFilePath.Replace(".txt", ".json");
            }

            try
            {
                if (System.IO.File.Exists(cachedFilePath))
                {
                    var lastModified = System.IO.File.GetLastWriteTime(cachedFilePath);

                    if (new DateTime(lastModified.Year, lastModified.Month, lastModified.Day) < new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day).AddDays(-2))
                    {
                        //Task.Run(() => ReloadIndex(cachedFilePath));
                        DeleteIndexCache(cachedFilePath);
                        return null;
                    }


                    var fileContent = System.IO.File.ReadAllText(cachedFilePath);
                    UserPlantDetailsListObject outputText = JsonConvert.DeserializeObject<UserPlantDetailsListObject>(fileContent);
                    //var outputTemp = pc.Index(false, skip: skip, take: take);
                    return outputText.userPlants.OrderBy(u => u.UserPlant.NameLatin.Replace("[k]", ""));
                }
            }
            catch(Exception e)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(e.Message + e.InnerException),
                    ReasonPhrase = "Failed to save cache"
                };
                throw new HttpResponseException(resp);
            }


            return null;
            //var outputTemp = pc.Index(false, skip: skip, take: take);
            //var listObject = new UserPlantDetailsListObject
            //{
            //    userPlants = outputTemp
            //};

            //string jsonString = JsonSerializer.Serialize(listObject);
            //fileContents = jsonString;

            //TextWriter tw = new StreamWriter(cachedFilePath, true);
            //tw.WriteLine(fileContents);
            //tw.Close();



        }

        public void ReloadIndex(string cachedFilePath)
        {
            UserPlantController pc = new UserPlantController();

            var outputTemp = pc.Index(false, skip: 0, take: -1);
            pc.UpdateIndexCache(outputTemp, cachedFilePath);
        }

        public bool UpdateIndexCache(IEnumerable<UserPlantDetailsViewModelCount> indexData, string cachedFilePath)
        {
            string fileContents = "";
            string shortFileContents = "";

            var listObject = new UserPlantDetailsListObject
            {
                userPlants = indexData.OrderBy(d => d.UserPlant.Name)
            };


            var shortenedListObject = new UserPlantDetailsListObject
            {
                userPlants = indexData.OrderBy(d => d.UserPlant.Name).Take(20)
            };
            string jsonString = JsonConvert.SerializeObject(listObject);
            fileContents = jsonString;

            string shortJsonString = JsonConvert.SerializeObject(shortenedListObject);
            shortFileContents = shortJsonString;

            TextWriter tw = new StreamWriter(cachedFilePath.Replace(".json", ".txt"), false);
            tw.WriteLine(fileContents);
            tw.Close();

            TextWriter stw = new StreamWriter(cachedFilePath.Replace(".txt",".json"), false);
            stw.WriteLine(shortFileContents);
            stw.Close();

            return true;
        }

        public bool DeleteIndexCache(string cachedFilePath)
        {
            System.IO.File.Delete(cachedFilePath.Replace(".json", ".txt"));
            System.IO.File.Delete(cachedFilePath.Replace(".txt", ".json"));
            return true;
        }

        public bool CheckPlantIfInGarden(int plantId)
        {
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return false;
            }

            GardenController gc = new GardenController();
            var mainGarden = gc.DbGetGardensByUserId(userId).FirstOrDefault();

            IEnumerable<UserPlant> userPlantToUserList = null;



            userPlantToUserList = from u in plantDB.UserPlants where !u.Deleted && u.Gardenid == mainGarden.Id && u.PlantId == plantId select u;

            //var realPlantList = from u in plantDB.UserPlants where !u.Deleted

            if (userPlantToUserList.Any())
            {
                return true;

            }
            return false;
        }
        public IEnumerable<UserPlantDetailsViewModelCount> userPlantfilter(string ecotags = "", string freezes = "", string toxic = "")
        {
            UserPlantController pc = new UserPlantController();
            var userPlants = pc.Index(true);
            
            IEnumerable<PlantTag> ecoTags = sc.getPositiveFilterTags(ecotags);
            IEnumerable<PlantTag> freezTags = sc.getPositiveFilterTags(freezes).Distinct();
            IEnumerable<PlantTag> toxicTag = sc.getPositiveFilterTags(toxic);
            IEnumerable<UserPlantDetailsViewModelCount> userPlantList = userPlants;

            if (userPlantList != null)
            {
                if (ecotags != null && ecotags.Any())
                {
                    userPlantList = filterPlantEcologicaly(userPlantList, ecoTags);
                }
                if(freezTags!=null && freezTags.Any())
                {
                    userPlantList = filterPlantFreezly(userPlantList, freezTags);
                }
                if (toxicTag != null && toxicTag.Any())
                {
                    userPlantList = filterPlantToxicity(userPlantList, toxicTag);
                }
            }
            return userPlantList;
        }
        public IEnumerable<UserPlantDetailsViewModelCount> filterPlantEcologicaly(IEnumerable<UserPlantDetailsViewModelCount> userPlantsList, IEnumerable<PlantTag> ecoTags = null)
        {
            IEnumerable<UserPlantDetailsViewModelCount> ret = userPlantsList;
            if (ecoTags != null && ecoTags.Any())
            {
                if (ecoTags.Count() == 1)
                {
                    var ecoTagFirstEl = ecoTags.FirstOrDefault();
                    if (ecoTagFirstEl.Id == 322)
                    {
                        ret = from r in ret from e in ecoTags where r.UserPlant.PlantTag.Where(pt => e.Id == pt.Id || pt.Id == 320).Any() select r;
                    }
                    else
                    {
                        ret = from r in ret from e in ecoTags where r.UserPlant.PlantTag.Where(pt => e.Id == pt.Id).Any() select r;

                    }
                }
                else if ((ecoTags.Count() > 1))
                {
                    List<UserPlantDetailsViewModelCount> plantListInRes = new List<UserPlantDetailsViewModelCount>();
                    foreach (var r in ret)
                    {
                        var count = 0;
                        foreach (var e in ecoTags)
                        {
                            if (e.Id == 322)
                            {
                                if (r.UserPlant.PlantTag.Where(pt => e.Id == pt.Id || pt.Id == 320).Any())
                                {
                                    count += 1;
                                    if (count >= ecoTags.Count())
                                    {
                                        plantListInRes.Add(r);
                                    }
                                }
                            }
                            else
                            {
                                if (r.UserPlant.PlantTag.Where(pt => e.Id == pt.Id).Any())
                                {
                                    count += 1;
                                    if (count >= ecoTags.Count())
                                    {
                                        plantListInRes.Add(r);
                                    }
                                }
                            }
                        }
                    }
                    ret = plantListInRes;
                }
            }
            return ret;
        }
        
        public IEnumerable<UserPlantDetailsViewModelCount> filterPlantFreezly(IEnumerable<UserPlantDetailsViewModelCount> userPlantsList, IEnumerable<PlantTag> freezTags = null)
        {
            IEnumerable<UserPlantDetailsViewModelCount> ret = userPlantsList;
            if (freezTags != null && freezTags.Any())
            {
                ret = ret.Where(p => p.UserPlant.PlantTag.Where(t => freezTags.Contains(t)).Any());
            }
            return ret;
        }
        public IEnumerable<UserPlantDetailsViewModelCount> filterPlantToxicity(IEnumerable<UserPlantDetailsViewModelCount> userPlantsList, IEnumerable<PlantTag> toxicTag = null)
        {
            IEnumerable<UserPlantDetailsViewModelCount> ret = userPlantsList;
            if (toxicTag != null && toxicTag.Any())
            {
                List<UserPlantDetailsViewModelCount> plantListInRes = new List<UserPlantDetailsViewModelCount>();
                foreach (var r in ret)
                {
                    foreach (var f in toxicTag)
                    {
                        if (r.UserPlant.PlantTag.Where(pt => f.Id == pt.Id).Any())
                        {
                            plantListInRes.Add(r);
                        }
                    }
                }
                ret = plantListInRes;
            }
            return ret;
        }
        /// <summary>
        /// Wird für Warnungen benutzt: die IndexViewModel-Rückgabe hat KEINE FaqList/Todos
        /// </summary>
        public UserPlantIndexViewModel IndexLite()
        {
            var userId = Utilities.GetUserId();
            var userProperty = new PropertyController().DbGetProperty(userId);
            if (userProperty == null)
            {
                throw new Exception("No Plant available");
            }

            UserPlantIndexViewModel viewModel = GetUserPlantIndexViewModelByUserIdLite(userId);
            return viewModel;
        }

        public UserPlantIndexViewModel IndexLite(Guid userId)
        {
            var userProperty = new PropertyController().DbGetProperty(userId);
            if (userProperty == null)
            {
                throw new Exception("No Plant available");
            }

            UserPlantIndexViewModel viewModel = GetUserPlantIndexViewModelByUserIdLite(userId);
            return viewModel;
        }

        // GET: UserPlants/Details/5
        public UserPlantDetailsViewModel Details(int? id)
        {
            //if (id == null)
            //{
            //    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            //}
            if (id == null)
            {
                throw new Exception("Seite konnte nicht gefunden werden.");//, HttpStatusCode.NotFound, "UserPlantController.Details(" + id + ")");
            }
            UserPlantDetailsViewModel vm = GetUserPlantDetailsViewModel((int)id, true);
            if (vm != null)
            {
                ArticleController ac = new ArticleController();
                vm.Articles = ac.GetObjectArticles(ModelEnums.ArticleReferenceType.Plant, vm.PlantId);

            }
            return vm;
        }
        public bool MovePlantToUserList(MoveUserplantToAnotherListVM model)
        {
            bool isOk = false;
            if (model != null )
            {
                if (CheckIfUserlistExist(model.NewListId))
                {
                    UserPlantToUserList existUserplant = GetUserplantToUserList(model.UserplantId);
                    if (existUserplant != null)
                    {
                        existUserplant.UserListId = model.NewListId;
                        plantDB.Entry(existUserplant).State = EntityState.Modified;

                        isOk = plantDB.SaveChanges() > 0 ? true : false;

                    }
                }
            }
            return isOk;
        }
        public bool MoveAllPlantsToUserList(MoveAllUserplantsToAnotherListVM vm)
        {
            bool isOk = false;
            if (vm != null)
            {
                if (CheckIfUserlistExist(vm.NewListId))
                {
                    var existUserplantsInThisList = DbGetUserPlantToUserLists(vm.CurrentListId);
                    if (existUserplantsInThisList != null && existUserplantsInThisList.Any())
                    {
                        foreach(var plant in existUserplantsInThisList)
                        {
                            plant.UserListId = vm.NewListId;
                            plantDB.Entry(plant).State = EntityState.Modified;
                        }
                        

                    }

                    UserListController uc = new UserListController();
                    var userList = (from list in plantDB.UserLists
                                    where list.Id == vm.CurrentListId
                                    && !list.Deleted
                                    select list).FirstOrDefault();
                    if (userList != null)
                    {
                        userList.Deleted = true;
                        plantDB.Entry(userList).State = EntityState.Modified;
                    }
                    isOk = plantDB.SaveChanges() > 0 ? true : false;
                }
            }
            return isOk;
        }
        private List<_HtmlImageViewModel> GetUserPlantHtmlImageViewModel(int userPlantId)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            // get images to user plant
            List<_HtmlImageViewModel> ret = new List<_HtmlImageViewModel>();


            HelperClasses.DbResponse imageResponse = rc.DbGetUserPlantReferencedImages(userPlantId);
            if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            {
                ret = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetBaseUrl());
            }
            else
            {
                ret.Add(new _HtmlImageViewModel
                {
                    SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                    Id = 0,
                    TitleAttr = "Kein Bild vorhanden"
                });
            }
            return ret;
        }
        public int UserplantsToUserListCount(IEnumerable<UserPlantToUserList> userPlants)
        {
            var groupedUserPlantToUserlist = userPlants.GroupBy(g => g.UserPlant.PlantId).ToList();
            var countPlants = 0;
            foreach (var g in groupedUserPlantToUserlist)
            {
                foreach (var p in g)
                {
                    countPlants += p.UserPlant.Count;
                }
            }
            return countPlants;
        }
        // GET: UserPlants/Edit/5
        public UserPlantEditViewModel Edit(int? id)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            if (id == null)
            {
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                return null;
            }
            var userPlant = DbGetUserPlant((int)id);
            if (userPlant == null)
            {
                //return HttpNotFound();
                return null;
            }
            UserPlantEditViewModel viewModel = new UserPlantEditViewModel();
            viewModel.Id = userPlant.Id;
            viewModel.Name = userPlant.Name;
            viewModel.Count = userPlant.Count;
            viewModel.Description = userPlant.Description;
            viewModel.Notes = userPlant.Notes;

            // get images to plant
            HelperClasses.DbResponse imageResponse = rc.DbGetUserPlantReferencedImages((int)id);
            if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            {
                viewModel.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
            }

            return viewModel;
        }

        // POST: UserPlants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public UserPlantEditViewModel Edit([Bind(Include = "id,name,count,description,isinpot,notes")] UserPlantEditViewModel userPlant, bool editCount=false)
        {
            if (ModelState.IsValid)
            {
                if (editCount)
                {
                    DbEditUserPlantToUserList(userPlant);
                }
                else
                {
                    DbEditUserPlant(userPlant);
                }
                //return RedirectToAction("Details", new { id = userPlant.Id });
            }
            //return RedirectToAction("Edit", new { id = userPlant.Id });
            return userPlant;
        }

        // GET: UserPlants/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userPlant = DbGetUserPlant((int)id);

            if (userPlant == null)
            {
                return HttpNotFound();
            }
            UserPlantDeleteViewModel viewModel = new UserPlantDeleteViewModel();
            viewModel.Id = userPlant.Id;
            viewModel.Name = userPlant.Name;
            return View(viewModel);
        }

        // POST: UserPlants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id,int gardenId)
        {
            DbDeleteUserPlant(id, gardenId);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Files UploadScanImage(HttpPostedFileBase imageFile, HttpPostedFile imageFileSrc, int scanId, string imageTitle = null)
        {
            string fileNameWithoutExtension = Utilities.stringToUri(System.IO.Path.GetFileNameWithoutExtension(imageFile.FileName));
            string extension = Path.GetExtension(imageFile.FileName).ToLower();
            string relativePath = System.Web.Hosting.HostingEnvironment.MapPath("~/nfiles/ScanImages/");
            string fullPath = Path.Combine(relativePath, fileNameWithoutExtension + extension);
            if (System.IO.File.Exists(fullPath))
            {
                int counter = 1;
                string tempFileName = "";
                do
                {
                    tempFileName = fileNameWithoutExtension + "_V" + counter.ToString();
                    fullPath = Path.Combine(relativePath, tempFileName + extension);
                    counter++;
                } while (System.IO.File.Exists(fullPath));

                fileNameWithoutExtension = tempFileName;
            }
            string savedFileName = fileNameWithoutExtension + extension;
            imgResizer.Upload(relativePath, imageFileSrc, savedFileName);
            var res = UploadAndRegisterFileFull(imageFile, scanId, (int)ModelEnums.ReferenceToModelClass.ScanImage, ModelEnums.FileReferenceType.ScanImage, imageTitle);
            return res;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-user-plant-image")]
        public ActionResult UploadUserPlantImage(HttpPostedFileBase imageFile, HttpPostedFile imageFileSrc, int userPlantId, string imageTitle = null, string imageDescription = null)
        {
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();

            if (imageFile == null || imageFile.ContentLength <= 0)
            {
                statusMessage.Messages.Add("Fehler beim Upload");
                statusMessage.Status = ModelEnums.ActionStatus.Error;
                //return
            }
            else
            {
                imgResizer.Upload(System.Web.Hosting.HostingEnvironment.MapPath("~/nfiles/PlantImages/"), imageFileSrc);
                UploadAndRegisterFile(imageFile, userPlantId, (int)ModelEnums.ReferenceToModelClass.UserPlant, ModelEnums.FileReferenceType.UserPlantImage, imageTitle, imageDescription);
            }

            TempData["statusMessage"] = statusMessage;

            return RedirectToAction("Edit", new { id = userPlantId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Files UploadUserSuggestion(HttpPostedFileBase imageFile, string imageTitle = null, string plantName = "")
        {
            var res = UploadAndRegisterFileFull(imageFile, 0, (int)ModelEnums.ReferenceToModelClass.Suggestion, ModelEnums.FileReferenceType.Suggestion, imageTitle, plantName);
            return res;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public Files UploadNewPlantSuggestion(HttpPostedFileBase imageFile, int plantId, string imageTitle = null, string plantName = "")
        {
            var res = UploadAndRegisterFileFull(imageFile, plantId, (int)ModelEnums.ReferenceToModelClass.Plant, ModelEnums.FileReferenceType.PlantImage, imageTitle, plantName);
            return res;
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ActionName("delete-user-plant-image")]
        //public ActionResult DeleteUserPlantImage(int ImageRefId, int userPlantId)
        //{
        //    ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
        //    HelperClasses.DbResponse response = rc.DbDeleteFile(ImageRefId, User.Identity.GetUserName());
        //    _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
        //    statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
        //    statusMessage.Status = response.Status;
        //    TempData["statusMessage"] = statusMessage;
        //    return RedirectToAction("Edit", new { id = userPlantId });
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                plantDB.Dispose();
            }
            base.Dispose(disposing);
        }

        #region DB

        [NonAction]
        public UserPlant DbGetUserPlant(int id)
        {
            var userPlant = (from up in plantDB.UserPlants
                             where up.Id == id
                             && !up.Deleted
                             select up).FirstOrDefault();
            return userPlant;
        }
        //[NonAction]
        //public UserPlantIndexViewModel GetUserPlantIndexViewModelByGardenId(int gardenId)
        //{
        //    var up = DbGetUserPlantsByGardenId(gardenId);
        //    List<UserPlantDetailsViewModel> plantList = new List<UserPlantDetailsViewModel>();
        //    foreach (var p in up)
        //    {
        //        UserPlantDetailsViewModel temp = GetUserPlantDetailsViewModel(p.Id);
        //        plantList.Add(temp);
        //    }

        //    return new UserPlantIndexViewModel { UserPlants = plantList };
        //}

        [NonAction]
        public UserPlant DbAddPlantToProperty(int plantId, int gardenId, Guid userId, string userName, int initialAgeInDays, int plantCount, bool IsInPot, bool async = true, UserPlantToUserListView[] uPlantTrigger=null)
        {
            TodoController tc = new TodoController();
            Plant plant = (from pl in plantDB.Plants
                           where pl.Id == plantId
                           select pl).FirstOrDefault();

            if (plant != null)
            {
                var garden_sel = (from g in plantDB.Gardens
                                  where g.Id == gardenId
                                  select g).FirstOrDefault();
                if(garden_sel != null)
                {
                   
                    var addedPlant= new UserPlant();
                    if (uPlantTrigger != null)
                    {
                        foreach(var u in uPlantTrigger)
                        {
                            UserPlant userPlantExist = (from up in plantDB.UserPlants
                                                  where up.Gardenid == gardenId && up.PlantId == plantId && !up.Deleted 
                                                  select up).FirstOrDefault();
                            if (userPlantExist != null)
                            {
                               IEnumerable <UserPlantToUserList> userPlantToUserlist= (from ut in plantDB.UserPlantToUserLists
                                                                          where  ut.PlantId == userPlantExist.Id && !ut.Deleted && ut.UserListId==u.UserListId
                                                                          select ut);
                                if(userPlantToUserlist!=null && userPlantToUserlist.Any())
                                {
                                    foreach(var upt in userPlantToUserlist)
                                    {
                                        upt.Count = upt.Count + plantCount;
                                    }

                                    plantDB.SaveChanges();
                                }
                                else
                                {
                                    UserPlantToUserListView vm = new UserPlantToUserListView
                                    {
                                        UserListId = u.UserListId,
                                        UserPlantId = userPlantExist.Id,
                                        Count = plantCount
                                    };
                                    AddUserPlantIntoListVm(vm);
                                }
                               
                               addedPlant= userPlantExist;
                            }
                            else
                            {
                                UserPlant newUserPlant = new UserPlant
                                {
                                    PlantId = plant.Id,
                                    Description = plant.Description,
                                    Name = string.IsNullOrEmpty(plant.NameGerman) ? plant.NameLatin : plant.NameGerman,
                                    Gardenid = garden_sel.Id,
                                    InitialAgeInDays = initialAgeInDays > 0 ? initialAgeInDays : 0,
                                    Count = plantCount > 0 ? plantCount : 1,
                                    IsInPot = IsInPot,
                                    NameLatin = plant.NameLatin,
                                    Synonym = plant.Synonym,
                                    NotifyForWind = IsInPot ? true : false,
                                    NotifyForFrost = true
                                };
                                newUserPlant.OnCreate(userName);
                                UserPlant newlyCreated = plantDB.UserPlants.Add(newUserPlant);
                                plantDB.SaveChanges();
                                if (async)
                                {
                                    Task.Run(() => AddTodoTemplatesToUserPlant(newlyCreated, userId));
                                }
                                else
                                {
                                    AddTodoTemplatesToUserPlant(newlyCreated, userId);
                                }
                                addedPlant= newlyCreated;

                                UserPlantToUserListView vm = new UserPlantToUserListView
                                {
                                    UserListId = u.UserListId,
                                    UserPlantId = addedPlant.Id,
                                    Count = newlyCreated.Count
                                };
                                AddUserPlantIntoListVm(vm);
                            }
                            
                        }
                        return addedPlant;
                    }
                    
                }
            }
            return null;
        }
        public void AddUserPlantIntoListVm(UserPlantToUserListView vm)
        {
            UserPlantToUserList up;
           
                    up = new UserPlantToUserList
                    {
                        UserListId = vm.UserListId,
                        PlantId = vm.UserPlantId,
                        Count = vm.Count
                    };
                    up.OnCreate(Utilities.GetUserName());
                    db.UserPlantToUserLists.Add(up);
                    db.SaveChanges();
        }
        public void AddTodoTemplatesToUserPlant(UserPlant up, Guid userId)
        {
            TodoController tc = new TodoController();
            PlantSearchController psc = new PlantSearchController();
            var temps = psc.DbGetTodoTemplates(up.PlantId, ModelEnums.ReferenceToModelClass.Plant);
            var todos = new List<_TodoCheckedTemplateViewModel>();
            foreach (TodoTemplate todo in temps)
            {
                var temp = new _TodoCheckedTemplateViewModel();
                temp.TemplateId = todo.Id;
                temp.TemplateTitle = todo.Title;
                temp.Checked = true;
                todos.Add(temp);
            }
            tc.DbAddTemplateTodos(ModelEnums.ReferenceToModelClass.Plant, up.PlantId, ModelEnums.ReferenceToModelClass.UserPlant, up.Id, todos, userId);
        }

        [NonAction]
        public void DbEditUserPlant(UserPlantEditViewModel plant)
        {
            var dbPlant = (from pln in plantDB.UserPlants
                           where pln.Id == plant.Id && !pln.Deleted
                           select pln).FirstOrDefault();
            dbPlant.Count = plant.Count;
            dbPlant.Name = plant.Name;
            dbPlant.Notes = plant.Notes;
            dbPlant.Description = plant.Description;
            dbPlant.IsInPot = plant.IsInPot;
            plantDB.SaveChanges();
            
        }
        public void DbEditUserPlantToUserList(UserPlantEditViewModel plant)
        {
            UserPlantToUserList dbUserPlantToUserList = (from pl in plantDB.UserPlantToUserLists
                                                            where pl.PlantId == plant.Id && !pl.Deleted && pl.UserListId == plant.UserListId
                                                            select pl).FirstOrDefault();
            if (dbUserPlantToUserList != null)
            {
                dbUserPlantToUserList.Count = plant.Count;
            }
                
            plantDB.SaveChanges();
        }
        [NonAction]
        public void DbDeleteUserPlant(int id,int gardenId)
        {
            var userPlants = (from u in plantDB.UserPlants
                         where u.PlantId == id && !u.Deleted && u.Gardenid==gardenId
                         select u);
            if(userPlants != null && userPlants.Any())
            {
                foreach (var p in userPlants)
                {
                    var plantInUserList = (from up in plantDB.UserPlantToUserLists
                                           where up.UserPlant.Id == p.Id && !up.Deleted
                                           select up);
                    foreach(var up in plantInUserList)
                    {
                        up.Deleted = true;
                    }
                    p.Deleted = true;
                    DbDeleteTodoFromUserPlant(p.Id);
                }
            }
            plantDB.SaveChanges();
        }
        [NonAction]
        public void DbDeleteTodoFromUserPlant( int userPlantId)
        
        {
            var todos = plantDB.Todoes.Where(t => !t.Deleted && !t.Ignored && t.ReferenceId == userPlantId);
            if (todos != null && todos.Any())
            {
                foreach (var t in todos)
                {
                    t.Ignored = true;
                    t.Deleted = true;
                }
                //var todoCyclic = plantDB.TodoCyclic.Join(todos, c => c.Id, t => t.CyclicId, (c, t) => c);
                var todoCyclic = (from t in plantDB.TodoCyclic where t.ReferenceId == userPlantId select t);

                foreach (var todo in todoCyclic)
                {
                    todo.Deleted = true;
                }
                //plantDB.SaveChanges();
            }
        }
        [NonAction]
        public void DbDeleteUserPlantFromUserList(int userPlantId,int userListId)
        {
            var userPlantFromUserList = (from u in plantDB.UserPlantToUserLists
                                            where (u.PlantId == userPlantId && u.UserListId == userListId && !u.Deleted)
                                            select u).FirstOrDefault();
            var allrelatedUserplants= (from u in plantDB.UserPlantToUserLists
                                       where (u.PlantId == userPlantId && !u.Deleted)
                                       select u);
            if(allrelatedUserplants==null && !allrelatedUserplants.Any())
            {
                DbDeleteUserPlantFromAllUserLists(userPlantFromUserList.UserPlant.PlantId, userPlantFromUserList.UserPlant.Gardenid);
            }
            else
            {
                var up = (from u in plantDB.UserPlants
                          where (u.Id == userPlantId && !u.Deleted)
                          select u).FirstOrDefault();
                if (userPlantFromUserList != null && up != null)
                {
                    userPlantFromUserList.Deleted = true;
                    plantDB.SaveChanges();
                }
            }
           
        }
        [NonAction]
        public void DbDeleteUserPlantFromAllUserLists(int userPlantId, int gardenId)
        {
            var userPlantsFromUserList = (from u in plantDB.UserPlantToUserLists
                                         where (u.UserPlant.PlantId == userPlantId && !u.Deleted && u.UserPlant.Gardenid == gardenId)
                                         select u) ;

            var up = (from u in plantDB.UserPlants
                      where (u.PlantId == userPlantId && !u.Deleted && u.Gardenid==gardenId)
                      select u);

            if(up!=null && up.Any())
            {
                foreach (var u in up)
                {
                    u.Deleted = true;
                    DbDeleteTodoFromUserPlant(u.Id);
                }
            }

            if (userPlantsFromUserList != null && userPlantsFromUserList.Any())
            {
                foreach (var upl in userPlantsFromUserList)
                {
                    upl.Deleted = true;

                }
            }
               
            plantDB.SaveChanges();
        }
            [NonAction]
        public IEnumerable<UserPlantDetailsViewModelCount> GroupedUserplantToUserList(int gardenId, bool withTags, bool withCyclicTodos = true, bool withImages = true, int ListId=0, int skip= 0, int take= -1)
        {
            IEnumerable<UserPlantToUserList> userPlantToUserList =null;
            if (ListId > 0)
            {
                userPlantToUserList = (from u in plantDB.UserPlantToUserLists where !u.Deleted && u.UserPlant.Gardenid == gardenId && u.UserListId== ListId orderby u.UserPlant.NameLatin select u);

            }
            else
            {
                userPlantToUserList = (from u in plantDB.UserPlantToUserLists where !u.Deleted && u.UserPlant.Gardenid == gardenId orderby u.UserPlant.NameLatin select u);

            }
            //var allUserPlantsInGarden = from u in plantDB.UserPlants where !u.Deleted && u.Gardenid == gardenId select u;


            List<IGrouping<int, UserPlantToUserList>> groupedUserPlantToUserlist = new List<IGrouping<int, UserPlantToUserList>>();
            if (take == -1)
            {
                groupedUserPlantToUserlist = userPlantToUserList.Where(gp => !gp.UserPlant.Deleted).OrderBy(g => g.UserPlant.NameLatinClean).GroupBy(g => g.UserPlant.PlantId).ToList();

            }
            else
            {
                groupedUserPlantToUserlist = (List<IGrouping<int, UserPlantToUserList>>)userPlantToUserList.Where(gp => !gp.UserPlant.Deleted).OrderBy(g => g.UserPlant.NameLatinClean).GroupBy(g => g.UserPlant.PlantId).Skip(skip).Take(take).ToList();
            }
            List < UserPlantDetailsViewModelCount > userPlants = new List<UserPlantDetailsViewModelCount>();
           
            UserPlantDetailsViewModel userPlantVm=null;

            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return null;
            }

            List<Todo> allTodo = (from td in db.Todoes
                         where td.DateStart.Year == DateTime.Now.Year 
                         && !td.Deleted
                         && !td.Ignored 
                         && td.UserId == userId  
                         && td.ReferenceType == ReferenceToModelClass.UserPlant
                                         orderby td.DateStart
                         select td).ToList();

            List<TodoCyclic> allCyclic = plantDB.TodoCyclic.Where(r => r.UserId == userId && !r.Deleted && r.ReferenceType == ReferenceToModelClass.UserPlant).ToList();
                                            ;




            foreach (var g in groupedUserPlantToUserlist)
            {
                List<string> userPlantsListNames = new List<string>();
                List<int> userPlantsListIds = new List<int>();
                foreach (UserPlantToUserList p in g)
                {
                     userPlantVm = GetUserPlantDetailsViewModel(p.PlantId, withTags, withCyclicTodos, withImages, allTodo.ToList(), allCyclic);
                    
                    break;
                }
                var countItems = 0;
                foreach (UserPlantToUserList p in g)
                {
                    userPlantsListNames.Add(p.UserList.Name);
                    userPlantsListIds.Add(p.UserListId);
                    countItems += p.Count;
                }



                if (userPlantVm != null)
                {
                    userPlantVm.Count = countItems;

                    var noticeMessage = countItems + (countItems > 1 ? " Pflanzen eingepflegt am " : " Pflanze eingepflegt am ") + userPlantVm.DatePlanted.ToString("dd.MM.yyyy");

                    //userPlantVm.Notes = noticeMessage +((userPlantVm.Notes == null) ? "" : (", " + userPlantVm.Notes)) ;

                    userPlantVm.CreatedNotes = noticeMessage;

                    UserPlantDetailsViewModelCount uvm = new UserPlantDetailsViewModelCount
                    {
                        ListIds = userPlantsListIds,
                        UserPlant = userPlantVm,
                        ListNames = userPlantsListNames.Distinct().ToArray()
                    };

                    userPlants.Add(uvm);
                }

            }
            return userPlants;
        }
        //[NonAction]
        //public IEnumerable <UserPlantDetailsViewModelCount> GetUserPlantIndexViewModelByUserId(Guid userid)
        //{
        //    var up = DbGetUserPlantsByUserId(userid);
        //    List<UserPlantDetailsViewModel> plantList = new List<UserPlantDetailsViewModel>();
        //    List<UserPlantDetailsViewModelCount> userPlants = new List<UserPlantDetailsViewModelCount>();
        //    foreach (var p in up)
        //    {
        //        UserPlantDetailsViewModel temp = GetUserPlantDetailsViewModel(p.Id);
        //        plantList.Add(temp);
        //    }
        //    var groupedUserPlant = plantList.GroupBy(p => p.PlantId);
        //    foreach(var g in groupedUserPlant)
        //    {
        //        UserPlantDetailsViewModelCount uvm = new UserPlantDetailsViewModelCount
        //        {
        //            UserPlant=g.Select(grp => grp).First()
        //        };
        //        userPlants.Add(uvm);
        //    }
        //    return userPlants;
        //   // return new UserPlantIndexViewModel { UserPlants = plantList.GroupBy(p => p.PlantId).Select(grp => grp.First()).ToList() };
        //}

        [NonAction]
        public UserPlantIndexViewModel GetUserPlantIndexViewModelByUserIdLite(Guid userid)
        {
            var up = DbGetUserPlantsByUserId(userid);
            List<UserPlantDetailsViewModel> plantList = new List<UserPlantDetailsViewModel>();
            foreach (var p in up)
            {
                UserPlantDetailsViewModel temp = GetUserPlantDetailsViewModelLite(p.Id);
                plantList.Add(temp);
            }

            return new UserPlantIndexViewModel { UserPlants = plantList };
        }

        public IEnumerable<UserPlantLightViewModel> GetUserPlantLightViewModel(int gardenId)
        {
            TodoController tc = new TodoController();

            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            var plantsForGarden = DbGetUserPlantsByGardenId(gardenId);
            var todos = tc.GetTodos(Utilities.GetUserId(), ReferenceToModelClass.UserPlant);
            List<UserPlantLightViewModel> vms = new List<UserPlantLightViewModel>();
            foreach (var plant in plantsForGarden)
            {
                var vm = new UserPlantLightViewModel
                {
                    Id = plant.Id,
                    PlantId = plant.PlantId,
                    Name = plant.Name,
                    NameLatin = plant.NameLatin,
                    Count = plant.Count,
                    Age = GetUserplantAge(plant),
                    IsInPot = plant.IsInPot,
                    Gardenid = plant.Gardenid,
                    Notes = plant.Notes,
                    Description = plant.Description,
                    Badges = GetPlantBadges(plant.PlantId),
                };
                if (todos != null && todos.Any())
                {
                    vm.Todos = todos.Where(r => r.ReferenceId == plant.Id && r.ReferenceType == ReferenceToModelClass.UserPlant);
                }
                HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages((int)vm.PlantId);
                if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                {
                    vm.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                }
                else
                {

                    vm.Images.Add(new _HtmlImageViewModel
                    {
                        SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                        Id = 0,
                        TitleAttr = "Kein Bild vorhanden"
                    });
                }
                vms.Add(vm);
            }
            return vms;
        }

        // returns a list of tags that are filterable in garden (badges, frost, toxic..)
        private IEnumerable<PlantTagSearchLite> GetPlantBadges(int plantId)
        {
            var BADGES_IDS = new[] { 320, 321, 322, 445, 447, 530, 531,346 };
            var TOXIC_IDS = new[] { 262, 315, 561 };
            var FREEZE_LVLS_IDS = new[] { 294, 295, 293, 292, 285, 286, 287, 288, 289, 290, 291 };

            //var plant_sel = (from plant in plantDB.Plants
            //                where !plant.Deleted && plant.Id == plantId
            //                select plant).Include(p => p.PlantTags).FirstOrDefault();

            List<PlantTagSearchLite> badges = new List<PlantTagSearchLite>();


     
                var tag_sel = (from tag in plantDB.PlantTags where !tag.Deleted && tag.PlantsWithThisTag.Any(p => p.Id == plantId) select tag);
                var x = 0;
            



            if (tag_sel != null) 
            {
                badges = tag_sel
                    .Where(t => !t.Deleted && (BADGES_IDS.Contains(t.Id) || TOXIC_IDS.Contains(t.Id) || FREEZE_LVLS_IDS.Contains(t.Id)))
                    .Select(b => new PlantTagSearchLite() { Id = b.Id })
                    .ToList();
            }

            return badges;
        }
        public void addUserplantSynonym()
        {

        }
        public UserPlantDetailsViewModel GetUserPlantDetailsViewModel(int id, bool withTags = true, bool withCyclicTodos = true, bool withImages = true, List<Todo> todoList = null, List<TodoCyclic> cyclicList = null, bool bypassUser = false)
        {
            PlantTagController pt = new PlantTagController();
            TodoController tc = new TodoController();
            UserPlantDetailsViewModel plantVm = null;
            var plant = GetUserplant(id);
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty && !bypassUser)
            {
                return new UserPlantDetailsViewModel();
            }

            if (plant != null)
            {
                plantVm = new UserPlantDetailsViewModel
                {
                    Id = plant.Id,
                    Count = plant.Count,
                    Badges = GetPlantBadges(plant.PlantId),
                    Description = plant.Description,
                    Gardenid = plant.Gardenid,
                    Age = GetUserplantAge(plant),
                    Name = plant.Name,
                    CustomName = (plant.Count <= 1) ? plant.Name : "(" + plant.Count + ") " + plant.Name,
                    PlantId = plant.PlantId,
                    IsInPot = plant.IsInPot,
                    Notes = plant.Notes,
                    NameLatin = plant.NameLatin,
                    Synonym = plant.Synonym,
                    DatePlanted = plant.CreatedDate,
                    PlantTag = withTags ? pt.DBGetPlantTagsByPlantId(plant.PlantId) : null,
                    NotifyForFrost =plant.NotifyForFrost,
                    NotifyForWind=plant.NotifyForWind
                };

                if (withCyclicTodos)
                {
                    var cyclicTodoes = cyclicList.Where(r => r.ReferenceId == plant.Id)
                                            .Select(g => new TodoCyclicVM()
                                            {
                                                Title = g.Title,
                                                Id = g.Id,
                                                Description = g.Description,
                                                DateStart = g.DateStart,
                                                DateEnd = g.DateEnd,
                                                Ignored = g.Ignored

                                            });


                    //var cyclicTodoes = plantDB.TodoCyclic.Where(r => r.UserId == userId && r.ReferenceId == plant.Id && !r.Deleted && r.ReferenceType == ReferenceToModelClass.UserPlant)
                    //                       .Select(g => new TodoCyclicVM()
                    //                       {
                    //                           Title = g.Title,
                    //                           Id = g.Id,
                    //                           Description = g.Description,
                    //                           DateStart = g.DateStart,
                    //                           DateEnd = g.DateEnd,
                    //                           Ignored = g.Ignored

                    //                       });


                    //plantVm.CyclicTodos = cyclicTodoes.ToList();


                    var todo = new List<Todo>();
                    var todoCylicIsFinished = new List<TodoCyclicVM>();
                    var sDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
                    var eDate = new DateTime(DateTime.Today.Year + 1, DateTime.Today.Month, DateTime.Today.Day);
                    foreach (var td in cyclicTodoes.ToList())
                    {
                        //var singleTodo = tc.DbGetTodosWithCyclic(sDate, eDate, 0, td.Id);
                        var singleTodo = todoList.Where(st => st.CyclicId == td.Id && st.DateStart >= sDate).FirstOrDefault();
                        todo.Add(singleTodo);
                        if (singleTodo != null)
                        {
                            td.Finished = singleTodo.Finished;
                            todoCylicIsFinished.Add(td);

                        }
                    }
                    plantVm.CyclicTodos = todoCylicIsFinished.ToList();
                    plantVm.Todos = todo;
                }

                if (withImages)
                {
                    ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
                    HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages((int)plantVm.PlantId);
                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any() && plant.Plant.Published)
                    {
                        plantVm.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                    }
                    else
                    {
                        plantVm.Images.Add(new _HtmlImageViewModel
                        {
                            SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                            Id = 0,
                            TitleAttr = "Kein Bild vorhanden"
                        });
                    }
                }
            }
            return plantVm;
        }
        private UserPlantDetailsViewModel GetUserPlantByPlantIdDetailsViewModel(int id)
        {
            VideoReferenceController vrc = new VideoReferenceController();
            PlantTagController pt = new PlantTagController();
            FaqController fc = new FaqController();
            TodoController tc = new TodoController();
            UserPlantDetailsViewModel plantVm = null;
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            var plant = GetUserPlantByPlantId(id);
            var todos = tc.GetTodos(Utilities.GetUserId(), ReferenceToModelClass.UserPlant);
            if (plant != null)
            {
                plantVm = new UserPlantDetailsViewModel
                {
                    Id = plant.Id,
                    Count = plant.Count,
                    Badges = GetPlantBadges(plant.PlantId),
                    Description = plant.Description,
                    Gardenid = plant.Gardenid,
                    Age = GetUserplantAge(plant),
                    Name = plant.Name,
                    CustomName = (plant.Count <= 1) ? plant.Name : "(" + plant.Count + ") " + plant.Name,
                    PlantId = plant.PlantId,
                    IsInPot = plant.IsInPot,
                    Notes = plant.Notes,
                    NameLatin = plant.NameLatin,
                    //PlantTag = pt.DBGetPlantTagsByPlantId(plant.PlantId),
                };
                if (todos != null && todos.Any())
                {
                    plantVm.Todos = todos.Where(r => r.ReferenceId == plant.Id && r.ReferenceType == ReferenceToModelClass.UserPlant);
                }

                HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages((int)plantVm.PlantId);
                if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                {
                    plantVm.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                }
                else
                {

                    plantVm.Images.Add(new _HtmlImageViewModel
                    {
                        SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                        Id = 0,
                        TitleAttr = "Kein Bild vorhanden"
                    });
                }

            }
            return plantVm;

        }

        private UserPlantDetailsViewModel GetUserPlantDetailsViewModelLite(int id)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            PlantTagController pt = new PlantTagController();
            var plant = GetUserplant(id);
            var plantVm = new UserPlantDetailsViewModel
            {
                Id = plant.Id,
                Count = plant.Count,
                Description = plant.Description,
                Gardenid = plant.Gardenid,
                Age = GetUserplantAge(plant),
                Name = plant.Name,
                CustomName = (plant.Count <= 1) ? plant.Name : "(" + plant.Count + ") " + plant.Name,
                PlantId = plant.PlantId,
                IsInPot = plant.IsInPot,
                Notes = plant.Notes,
                NameLatin = plant.NameLatin,
                Synonym=plant.Synonym,
                PlantTag = pt.DBGetPlantTagsByPlantId(plant.PlantId),
                VideoReferenceList = null,
                FaqList = null
            };
            HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages((int)plantVm.PlantId);
            if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            {
                plantVm.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
            }
            else
            {

                plantVm.Images.Add(new _HtmlImageViewModel
                {
                    SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                    Id = 0,
                    TitleAttr = "Kein Bild vorhanden"
                });
            }
         
            return plantVm;
        }

        private string GetUserplantAge(UserPlant plant)
        {
            int existsFor = (DateTime.Now - plant.CreatedDate).Days;
            existsFor = existsFor + plant.InitialAgeInDays;
            int daysWithoutYears = existsFor % 365;
            int monthsWithoutYears = (int)Math.Floor((double)(daysWithoutYears / 30));
            int yearsWithoutDays = (int)Math.Floor((double)(existsFor / 365));

            string age = "";
            if (yearsWithoutDays == 1)
            {
                age = "~1 Jahr ";
                if (monthsWithoutYears > 0)
                {
                    age = age + "und ";
                }
            }
            else if (yearsWithoutDays > 1)
            {
                age = "~" + yearsWithoutDays + " Jahre ";
                if (monthsWithoutYears > 0)
                {
                    age = age + "und ";
                }
            }
            if (monthsWithoutYears == 1)
            {
                if (yearsWithoutDays == 0)
                {
                    age = age + "~";
                }
                age = age + "1 Monat ";
            }
            else if (monthsWithoutYears > 1)
            {
                if (yearsWithoutDays == 0)
                {
                    age = age + "~";
                }
                age = age + monthsWithoutYears + " Monate ";
            }
            if (monthsWithoutYears == 0 && yearsWithoutDays == 0)
            {
                age = daysWithoutYears + " Tage";
            }

            return age;
        }
        private UserPlant GetUserPlantByPlantId(int plantId)
        {
            var plant = (from pl in plantDB.UserPlants
                         where pl.PlantId == plantId
                         && !pl.Deleted
                         select pl).FirstOrDefault();
            
            return plant;
        }
        private UserPlant GetUserplant(int id)
        {
            var plant = (from pl in plantDB.UserPlants
                         where !pl.Deleted &&
                         pl.Id == id
                         select pl).FirstOrDefault();
            return plant;
        }
        public UserPlantToUserList GetUserplantToUserList(int userPlantId)
        {
            var userplantToUserList = (from pl in plantDB.UserPlantToUserLists
                         where pl.PlantId == userPlantId
                         && !pl.Deleted
                         select pl).FirstOrDefault();
            return userplantToUserList;
        }
        public bool CheckIfUserlistExist(int userlistId)
        {
            bool isOk = false;
            var userList = (from list in plantDB.UserLists
                                       where list.Id == userlistId
                                       && !list.Deleted
                                       select list).FirstOrDefault();
            if (userList != null)
            {
                isOk = true;
            }
            return isOk;
        }
        [NonAction]
        public IEnumerable<UserPlant> DbGetUserPlantsByUserId(Guid userid)
        {
            GardenController gc = new GardenController();
            var gardens = gc.DbGetGardensByUserId(userid);

            var plants = (from ga in gardens
                          from pl in plantDB.UserPlants
                          where ga.Id == pl.Gardenid
                          && !pl.Deleted
                          select pl);
            return plants;
        }

        [NonAction]
        public HashSet<UserPlantDetailsViewModel> DbGetUserPlantsFromLists(int gardenId)
        {
            List<UserList> uLists = plantDB.UserLists.Where(ul => ul.GardenId == gardenId && !ul.Deleted).ToList();
            HashSet<UserPlantDetailsViewModel> res = new HashSet<UserPlantDetailsViewModel>();

            foreach (UserList ul in uLists)
            {
                foreach (UserPlantToUserList upl in DbGetUserPlantToUserLists(ul.Id))
                {
                    res.Add(GetUserPlantDetailsViewModel(upl.UserPlant.Id, false, false, false, bypassUser: true));
                }
            }

            return res;
        }

        [NonAction]
        public IEnumerable<UserPlant> DbGetUserPlantsByGardenId(int gardenId)
        {
            var plants = (from pln in plantDB.UserPlants
                          where pln.Gardenid == gardenId
                          && !pln.Deleted
                          select pln);
            return plants;
        }
        [NonAction]
        public IEnumerable<UserPlantToUserList> DbGetUserPlantToUserLists(int listId)
        {
            var userPlantToUserList = from u in plantDB.UserPlantToUserLists
                                      where (!u.Deleted && u.UserListId == listId && !u.UserPlant.Deleted) select u;
            if (userPlantToUserList == null)
            {
                return new List<UserPlantToUserList>();
            }
            return userPlantToUserList;
        }
        public IEnumerable<UserPlantToUserListViewModel> DbGetGroupUserPlantToUserLists(int listId)
        {
            var userPlantToUserList = from u in plantDB.UserPlantToUserLists
                                      where (!u.Deleted && u.UserListId == listId && !u.UserPlant.Deleted) 
                                      select u;
            if (userPlantToUserList == null)
            {
                return new List<UserPlantToUserListViewModel>();
            }
            var listOfPlants = new List<UserPlantToUserListViewModel>();
            foreach (var u in userPlantToUserList.GroupBy(up => up.PlantId)
                        .Select(group => new UserPlantToUserListViewModel
                        {
                            PlantId = group.Key,
                            Count = group.Count(),
                            UserListId = group.Select(l => l.UserListId).FirstOrDefault()
                        })) 
            {
                listOfPlants.Add(u);
            }
            return listOfPlants;
        }
        #endregion
    }
}
