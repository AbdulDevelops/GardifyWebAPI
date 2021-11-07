using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GardifyModels.Models;
using PflanzenApp.App_Code;
using Microsoft.AspNet.Identity;
using System.Collections.Generic;
using static GardifyModels.Models.UserPlantViewModels;
using System.Threading.Tasks;

namespace PflanzenApp.Controllers
{
    [CustomAuthorize]
    public class UserPlantController : _BaseController
    {

        // GET: UserPlants
        public ActionResult Index()
        {
            var userId = new Guid(User.Identity.GetUserId());

            var userProperty = new PropertyController().DbGetProperty(userId);
            if (userProperty == null)
            {
                return RedirectToAction("Create", "Property");
            }

            UserPlantIndexViewModel viewModel = GetUserPlantIndexViewModelByUserId(userId);
            return View(viewModel);
        }

        // GET: UserPlants/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            if (id == null)
            {
                return RedirectToError("Seite konnte nicht gefunden werden.", HttpStatusCode.NotFound, "UserPlantController.Details(" + id + ")");
            }
            UserPlantDetailsViewModel vm = GetUserPlantDetailsViewModel((int)id);

            ArticleController ac = new ArticleController();
            vm.Articles = ac.GetObjectArticles(ModelEnums.ArticleReferenceType.Plant, vm.PlantId);
            return View(vm);
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
                    SrcAttr = Utilities.GetBaseUrl() + "/Images/no-image.png",
                    Id = 0,
                    TitleAttr = "Kein Bild vorhanden"
                });
            }
            return ret;
        }

        // GET: UserPlants/Edit/5
        public ActionResult Edit(int? id)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var userPlant = DbGetUserPlant((int)id);
            if (userPlant == null)
            {
                return HttpNotFound();
            }
            UserPlantEditViewModel viewModel = new UserPlantEditViewModel();
            viewModel.Id = userPlant.Id;
            viewModel.Name = userPlant.Name;
            viewModel.Count = userPlant.Count;
            viewModel.Description = userPlant.Description;

            // get images to plant
            HelperClasses.DbResponse imageResponse = rc.DbGetUserPlantReferencedImages((int)id);
            if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            {
                viewModel.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
            }

            return View(viewModel);
        }

        // POST: UserPlants/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,count,description,isinpot")] UserPlantEditViewModel userPlant)
        {
            if (ModelState.IsValid)
            {
                DbEditUserPlant(userPlant);
                return RedirectToAction("Details", new { id = userPlant.Id });
            }
            return RedirectToAction("Edit", new { id = userPlant.Id });
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
        public ActionResult DeleteConfirmed(int id)
        {
            DbDeleteUserPlant(id);
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-user-plant-image")]
        public ActionResult UploadUserPlantImage(HttpPostedFileBase imageFile, int userPlantId, string imageTitle = null, string imageDescription = null)
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
                UploadAndRegisterFile(imageFile, userPlantId, (int)ModelEnums.ReferenceToModelClass.UserPlant, ModelEnums.FileReferenceType.UserPlantImage, imageTitle, imageDescription);
            }

            TempData["statusMessage"] = statusMessage;

            return RedirectToAction("Edit", new { id = userPlantId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("delete-user-plant-image")]
        public ActionResult DeleteUserPlantImage(int ImageRefId, int userPlantId)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            HelperClasses.DbResponse response = rc.DbDeleteFileReference(ImageRefId, User.Identity.GetUserName());
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Edit", new { id = userPlantId });
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ctx.Dispose();
            }
            base.Dispose(disposing);
        }

        #region DB

        [NonAction]
        public UserPlant DbGetUserPlant(int id)
        {
            var userPlant = (from up in ctx.UserPlants
                             where up.Id == id
                             && !up.Deleted
                             select up).FirstOrDefault();
            return userPlant;
        }
        [NonAction]
        public UserPlantIndexViewModel GetUserPlantIndexViewModelByGardenId(int gardenId)
        {
            var up = DbGetUserPlantsByGardenId(gardenId);
            List<UserPlantDetailsViewModel> plantList = new List<UserPlantDetailsViewModel>();
            foreach (var p in up)
            {
                UserPlantDetailsViewModel temp = GetUserPlantDetailsViewModel(p.Id);
                plantList.Add(temp);
            }

            return new UserPlantIndexViewModel { UserPlants = plantList };
        }
        [NonAction]
        public bool DbAddPlantToProperty(int plantId, int gardenId, Guid userId, string userName, int initialAgeInDays, int plantCount, bool IsInPot, IEnumerable<_TodoCheckedTemplateViewModel> todos)
        {
            TodoController tc = new TodoController();
            Plant plant = (from pl in ctx.Plants
                           where pl.Id == plantId
                           select pl).FirstOrDefault();

            if (plant != null)
            {
                var garden_sel = (from g in ctx.Gardens
                                  where g.Id == gardenId
                                  select g).FirstOrDefault();

                if (garden_sel != null)
                {
                    UserPlant newUserPlant = new UserPlant
                    {
                        PlantId = plant.Id,
                        Description = plant.Description,
                        Name = string.IsNullOrEmpty(plant.NameGerman) ? plant.NameLatin : plant.NameGerman,
                        Gardenid = garden_sel.Id,
                        InitialAgeInDays = initialAgeInDays > 0 ? initialAgeInDays : 0,
                        Count = plantCount > 0 ? plantCount : 1,
                        IsInPot = IsInPot

                    };
                    newUserPlant.OnCreate(userName);
                    ctx.UserPlants.Add(newUserPlant);
                    bool isOk = ctx.SaveChanges() > 0 ? true : false;
                    if (isOk && todos != null && todos.Count() > 0)
                    {
                        tc.DbAddTemplateTodos(ModelEnums.ReferenceToModelClass.Plant, plantId, ModelEnums.ReferenceToModelClass.UserPlant, newUserPlant.Id, todos);
                    }
                    return isOk;
                }
            }
            return false;
        }

        [NonAction]
        public UserPlant DbAddPlantToPropertyComplete(int plantId, int gardenId, Guid userId, string userName, int initialAgeInDays, int plantCount, bool IsInPot, bool async = true, UserPlantToUserListView[] uPlantTrigger = null)
        {
            TodoController tc = new TodoController();
            Plant plant = (from pl in ctx.Plants
                           where pl.Id == plantId
                           select pl).FirstOrDefault();

            if (plant != null)
            {
                var garden_sel = (from g in ctx.Gardens
                                  where g.Id == gardenId
                                  select g).FirstOrDefault();
                if (garden_sel != null)
                {

                    var addedPlant = new UserPlant();
                    if (uPlantTrigger != null)
                    {
                        foreach (var u in uPlantTrigger)
                        {
                            UserPlant userPlantExist = (from up in ctx.UserPlants
                                                        where up.Gardenid == gardenId && up.PlantId == plantId && !up.Deleted
                                                        select up).FirstOrDefault();
                            if (userPlantExist != null)
                            {
                                IEnumerable<UserPlantToUserList> userPlantToUserlist = (from ut in ctx.UserPlantToUserLists
                                                                                        where ut.PlantId == userPlantExist.Id && !ut.Deleted && ut.UserListId == u.UserListId
                                                                                        select ut);
                                if (userPlantToUserlist != null && userPlantToUserlist.Any())
                                {
                                    foreach (var upt in userPlantToUserlist)
                                    {
                                        upt.Count = upt.Count + plantCount;
                                    }

                                    ctx.SaveChanges();
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

                                addedPlant = userPlantExist;
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
                                UserPlant newlyCreated = ctx.UserPlants.Add(newUserPlant);
                                ctx.SaveChanges();
                                if (async)
                                {
                                    Task.Run(() => AddTodoTemplatesToUserPlant(newlyCreated, userId));
                                }
                                else
                                {
                                    AddTodoTemplatesToUserPlant(newlyCreated, userId);
                                }
                                addedPlant = newlyCreated;

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
            ctx.UserPlantToUserLists.Add(up);
            ctx.SaveChanges();
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
            tc.DbAddTemplateTodosComplete(ModelEnums.ReferenceToModelClass.Plant, up.PlantId, ModelEnums.ReferenceToModelClass.UserPlant, up.Id, todos, userId);
        }

        [NonAction]
        public void DbEditUserPlant(UserPlantEditViewModel plant)
        {
            var dbPlant = (from pln in ctx.UserPlants
                           where pln.Id == plant.Id && !pln.Deleted
                           select pln).FirstOrDefault();
            dbPlant.Count = plant.Count;
            dbPlant.Name = plant.Name;
            dbPlant.Description = plant.Description;
            dbPlant.IsInPot = plant.IsInPot;
            ctx.SaveChanges();
        }

        [NonAction]
        public void DbDeleteUserPlantGarden(int id, int gardenId)
        {
            var userPlants = (from u in ctx.UserPlants
                              where u.PlantId == id && !u.Deleted && u.Gardenid == gardenId
                              select u);
            if (userPlants != null && userPlants.Any())
            {
                foreach (var p in userPlants)
                {
                    var plantInUserList = (from up in ctx.UserPlantToUserLists
                                           where up.UserPlant.Id == p.Id && !up.Deleted
                                           select up);
                    foreach (var up in plantInUserList)
                    {
                        up.Deleted = true;
                    }
                    p.Deleted = true;
                    DbDeleteTodoFromUserPlant(p.Id);
                }
            }
            ctx.SaveChanges();
        }



        [NonAction]
        public void DbDeleteTodoFromUserPlant(int userPlantId)

        {
            var todos = ctx.Todoes.Where(t => !t.Deleted && !t.Ignored && t.ReferenceId == userPlantId);
            if (todos != null && todos.Any())
            {
                foreach (var t in todos)
                {
                    t.Ignored = true;
                    t.Deleted = true;
                }
                //var todoCyclic = plantDB.TodoCyclic.Join(todos, c => c.Id, t => t.CyclicId, (c, t) => c);
                var todoCyclic = (from t in ctx.TodoCyclic where t.ReferenceId == userPlantId select t);

                foreach (var todo in todoCyclic)
                {
                    todo.Deleted = true;
                }
                //plantDB.SaveChanges();
            }
        }

        [NonAction]
        public void DbDeleteUserPlant(int id)
        {
            var plant = (from g in ctx.UserPlants
                         where g.Id == id
                         select g).FirstOrDefault();
            plant.Deleted = true;
            ctx.SaveChanges();
        }

        [NonAction]
        public UserPlantIndexViewModel GetUserPlantIndexViewModelByUserId(Guid userid)
        {
            var up = DbGetUserPlantsByUserId(userid);
            List<UserPlantDetailsViewModel> plantList = new List<UserPlantDetailsViewModel>();
            foreach (var p in up)
            {
                UserPlantDetailsViewModel temp = GetUserPlantDetailsViewModel(p.Id);
                plantList.Add(temp);
            }

            return new UserPlantIndexViewModel { UserPlants = plantList };
        }

        private UserPlantDetailsViewModel GetUserPlantDetailsViewModel(int id)
        {
            VideoReferenceController vrc = new VideoReferenceController();
            FaqController fc = new FaqController();
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
                VideoReferenceList = vrc.GetVideoDetailsViewModelList(id, ModelEnums.ReferenceToModelClass.UserPlant),
                FaqList = fc.DbGetReferencedFaqList(ModelEnums.ReferenceToModelClass.UserPlant, id)
            };
            TodoController tc = new TodoController();
            plantVm.TodosOld = tc.GetTodoIndexViewModel(id, ModelEnums.ReferenceToModelClass.UserPlant);
            plantVm.Images = GetUserPlantHtmlImageViewModel(id);
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

        private UserPlant GetUserplant(int id)
        {
            var plant = (from pl in ctx.UserPlants
                         where pl.Id == id
                         && !pl.Deleted
                         select pl).FirstOrDefault();
            return plant;
        }

        [NonAction]
        public IEnumerable<UserPlant> DbGetUserPlantsByUserId(Guid userid)
        {
            GardenController gc = new GardenController();
            var gardens = gc.DbGetGardensByUserId(userid);
            var plants = (from ga in gardens
                          from pl in ctx.UserPlants
                          where ga.Id == pl.Gardenid
                          && !pl.Deleted
                          select pl);
            return plants;
        }

        [NonAction]
        public IEnumerable<UserPlant> DbGetUserPlantsByGardenId(int gardenId)
        {
            var plants = (from pln in ctx.UserPlants
                          where pln.Gardenid == gardenId
                          && !pln.Deleted
                          select pln);
            return plants;
        }
        #endregion
    }
}
