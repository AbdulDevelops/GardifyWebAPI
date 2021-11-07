using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GardifyModels.Models;
using Microsoft.AspNet.Identity;
using PflanzenApp.App_Code;
using System.Diagnostics;
using static GardifyModels.Models.GardenViewModels;
using static GardifyModels.Models.TodoViewModels;
using System.Threading;
using System.Web.Hosting;
using PflanzenApp.Controllers.AdminArea;
using System.IO;
using ExifLibrary;
using System.Drawing.Imaging;
using System.Drawing;
using MetadataExtractor.Formats.Jpeg;
using MetadataExtractor;
using MetadataExtractor.Formats.Exif;
using XperiCode.JpegMetadata;

namespace PflanzenApp.Controllers
{

    public class GardenController : _BaseController
    {
        // GET: Garden
        public ActionResult Index()
        {
            
            HostingEnvironment.QueueBackgroundWorkItem((Action<CancellationToken>)MyTaskAsMethod);

            var baseModel = new GardenIndexViewModel();
            baseModel.TodoList = new TodoViewModels.TodoIndexViewModel();
            baseModel.TodoList.TodoList = new List<TodoDetailsViewModel>();
            baseModel.Gardens = new List<GardenDetailsViewModel>();
            baseModel.WatchlistEntries = new List<WatchlistEntry>();
            return View(baseModel);
            WatchListController wc = new WatchListController();

            var userId = new Guid(User.Identity.GetUserId());

            var userProperty = new PropertyController().DbGetProperty(userId);
            if (userProperty == null)
            {
                return RedirectToAction("Create", "Property");
            }

            GardenIndexViewModel viewModel = GetGardenIndexViewModel(userId);
            TodoController tc = new TodoController();

            WeatherHandler wh = new WeatherHandler();
            WeatherForecast forecast = wh.getWeatherForecastByGeoCoords(userProperty.Latitude, userProperty.Longtitude);
            if (forecast != null)
            {
                WeatherHelpers.HourlyForecast weatherCurrentHour = forecast.Forecasts.Hourly.FirstOrDefault();
                viewModel.CurrentWeather = weatherCurrentHour;
            }

            viewModel.WatchlistEntries = wc.DbGetWatchlisEntriesByUserId(userId);

            //!!!!!!!!!!!!! WARNUNGEN //!!!!!!!!!!!!!
            Stopwatch watch = new Stopwatch();
            watch.Start();
            Debug.WriteLine("Warnung Anfang ");

            UserPlantController upc = new UserPlantController();
            AlertController ac = new AlertController();

            //Debug.WriteLine("Warnung controller geladen. " + watch.ElapsedMilliseconds);

            IEnumerable<UserPlant> userPlants = upc.DbGetUserPlantsByUserId(userId);
            //Debug.WriteLine("Warnung user plants geladen. " + watch.ElapsedMilliseconds);

            List<AlertViewModels.UserPlantAlertsViewModel> triggeredAlertViews = new List<AlertViewModels.UserPlantAlertsViewModel>();

            if (userPlants != null && userPlants.Any())
            {
                foreach (var userPlant in userPlants)
                {
                    long alertBeginnTime = watch.ElapsedMilliseconds;
                    AlertViewModels.UserPlantAlertsViewModel userPlantAlertViewModel;

                    long tmpTime = watch.ElapsedMilliseconds;
                    IEnumerable<Alert> triggeredPlantAlerts = ac.DbGetTriggeredAlertsByPlantId(userPlant.PlantId, forecast);
                    Debug.WriteLine("Aktive Alerts für " + userPlant.Name + " geladen in: " + (watch.ElapsedMilliseconds - tmpTime));

                    if (triggeredPlantAlerts != null && triggeredPlantAlerts.Any())
                    {
                        userPlantAlertViewModel = new AlertViewModels.UserPlantAlertsViewModel
                        {
                            Plant = userPlant.Plant,
                            UserPlantId = userPlant.Id,
                            UserPlantName = userPlant.Name
                        };
                        userPlantAlertViewModel.AlertViewModels.AddRange(ac.DbGetAlertViewModels(triggeredPlantAlerts));
                        triggeredAlertViews.Add(userPlantAlertViewModel);
                    }

                    Debug.WriteLine("Berechnung für " + userPlant.Name + " dauerte: " + (watch.ElapsedMilliseconds - alertBeginnTime));
                }
            }

            viewModel.Alerts = triggeredAlertViews;

            watch.Stop();
            Debug.WriteLine("Warnung Ende. " + watch.ElapsedMilliseconds);
            //!!!!!!!!!!! /WARNUNGEN //!!!!!!!!!!!!!

            return View(viewModel);
        }

        private static void MyTaskAsMethod(CancellationToken cancellationToken)
        {
            AdminAreaAccountController ac = new AdminAreaAccountController();
            var userList = ac.DbGetUserDetailsViewList();
            Debug.WriteLine("sleeping");
            //Thread.Sleep(1000);
        }

        // GET: Garden/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return RedirectToError("Seite konnte nicht gefunden werden.", HttpStatusCode.NotFound, "GardenController.Details(" + id + ")");
            }

            Garden garden = DbGetGarden((int)id);
            GardenDetailsViewModel viewModel = GetGardenDetailsViewModel((int)id);

            return View(viewModel);
        }

        private object getName(int plantId)
        {
            PlantController pc = new PlantController();
            return pc.DbGetPlantName(plantId);
        }

        // GET: Garden/Create
        [HttpGet]
        public ActionResult Create()
        {
            GardenCreateViewModel viewModel = new GardenCreateViewModel();
            return View(viewModel);
        }

        // POST: Garden/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GardenCreateViewModel gardenView)
        {
            if (ModelState.IsValid)
            {
                DbAddGarden(gardenView);
                return RedirectToAction("Index");
            }
            return View(gardenView);
        }

        // GET: Garden/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Garden garden = DbGetGarden((int)id);
            if (garden == null)
            {
                return RedirectToError("Seite konnte nicht gefunden werden.", HttpStatusCode.NotFound, "GardenController.Edit(" + id + ")");
            }

            GardenEditViewModel viewModel = new GardenEditViewModel
            {
                CardinalDirection = garden.CardinalDirection,
                GroundType = garden.GroundType,
                Description = garden.Description,
                Inside = garden.Inside,
                IsPrivate = garden.PrivacyLevel == ModelEnums.PrivacyLevel.Private,
                Light = garden.Light,
                Name = garden.Name,
                PhType = garden.PhType,
                ShadowStrength = garden.ShadowStrength,
                Temperature = garden.Temperature,
                Wetness = garden.Wetness,
                Images = GetGardenHtmlImageViewModel((int)id),
                Id = garden.Id
            };

            // remove placeholder image
            if (viewModel.Images.FirstOrDefault().Id == 0)
            {
                viewModel.Images = new List<_HtmlImageViewModel>();
            }

            return View(viewModel);
        }

        // POST: Garden/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,name,wetness,phType,groundType,info,cardinalDirection,shadowStrength,inside,temperature,light,IsPrivate")] GardenEditViewModel gardenView)
        {
            PropertyController proc = new PropertyController();
            if (ModelState.IsValid)
            {
                DbUpdateGarden(gardenView);
                return RedirectToAction("Details", new { id = gardenView.Id });
            }

            return View(gardenView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-garden-image")]
        public ActionResult UploadGardenImage(HttpPostedFileBase imageFile, int id, string imageTitle = null, string imageDescription = null)
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
                UploadAndRegisterFile(imageFile, id, (int)ModelEnums.ReferenceToModelClass.Garden, ModelEnums.FileReferenceType.GardenImage, imageTitle, imageDescription);
            }

            TempData["statusMessage"] = statusMessage;

            return RedirectToAction("Edit", new { id = id });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("delete-garden-image")]
        public ActionResult DeleteGardenImage(int ImageRefId, int gardenId)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            HelperClasses.DbResponse response = rc.DbDeleteFileReference(ImageRefId, User.Identity.GetUserName());
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Edit", new { id = gardenId });
        }

        // GET: Garden/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Garden garden = DbGetGarden((int)id);
            if (garden == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            GardenDeleteViewModel viewModel = new GardenDeleteViewModel();
            viewModel.Id = garden.Id;
            viewModel.Name = garden.Name;
            return View(viewModel);
        }

        // POST: Garden/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            DbDeleteGarden(id);
            return RedirectToAction("Index");
        }

        #region DB

        [NonAction]
        private List<_HtmlImageViewModel> GetGardenHtmlImageViewModel(int gardenId)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            // get images to user plant
            List<_HtmlImageViewModel> ret = new List<_HtmlImageViewModel>();


            HelperClasses.DbResponse imageResponse = rc.DbGetGardenReferencedImages(gardenId);
            if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            {
                ret = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
            }
            else
            {
                ret.Add(new _HtmlImageViewModel
                {
                    SrcAttr = Url.Content("~/Images/no-image.png"),
                    Id = 0,
                    TitleAttr = "Kein Bild vorhanden"
                });
            }
            return ret;
        }

        [NonAction]
        public Garden DbGetGarden(int id)
        {
            return (from g in ctx.Gardens
                    where g.Id == id
                    && !g.Deleted
                    select g).FirstOrDefault();
        }
        [NonAction]
        public String getGardenName(int id)
        {
            Garden garden = DbGetGarden(id);
            return garden.Name;
        }

        public IEnumerable<Garden> DbGetGardensByUserId(Guid userId, ModelEnums.PrivacyLevel privacyLevel = ModelEnums.PrivacyLevel.Public)
        {
            IEnumerable<Garden> ret = null;
            ret = (from p in ctx.Property
                   where p.UserId == userId
                   && !p.Deleted
                   join g in ctx.Gardens
                   on p.Id equals g.PropertyId
                   where !g.Deleted && g.PrivacyLevel == privacyLevel
                   select g);
            return ret;
        }

        [NonAction]
        public void DbAddGarden(GardenCreateViewModel gardenView)
        {
            PropertyController proc = new PropertyController();
            Guid userId = Utilities.GetUserId();
            int propertyId = (int)proc.DbGetPropertyId(userId);
            Garden garden = new Garden
            {
                CardinalDirection = gardenView.CardinalDirection,
                Description = gardenView.Description,
                Inside = gardenView.Inside,
                ShadowStrength = gardenView.ShadowStrength,
                GroundType = gardenView.GroundType,
                Light = gardenView.Light,
                Name = gardenView.Name,
                PhType = gardenView.PhType,
                PrivacyLevel = gardenView.IsPrivate ? ModelEnums.PrivacyLevel.Private : ModelEnums.PrivacyLevel.Public,
                Temperature = gardenView.Temperature,
                Wetness = gardenView.Wetness,
                PropertyId = propertyId,
                Deleted = false
            };
            garden.OnCreate(Utilities.GetUserName());

            ctx.Gardens.Add(garden);
            ctx.SaveChanges();
        }

        [NonAction]
        public void DbUpdateGarden(GardenEditViewModel garden)
        {
            var gardenToEdit = (from g in ctx.Gardens
                                where g.Id == garden.Id && !g.Deleted
                                select g).FirstOrDefault();

            gardenToEdit.CardinalDirection = garden.CardinalDirection;
            gardenToEdit.Description = garden.Description;
            gardenToEdit.Name = garden.Name;
            gardenToEdit.Wetness = garden.Wetness;
            gardenToEdit.PhType = garden.PhType;
            gardenToEdit.GroundType = garden.GroundType;
            gardenToEdit.Inside = garden.Inside;
            gardenToEdit.Light = garden.Light;
            gardenToEdit.ShadowStrength = garden.ShadowStrength;
            gardenToEdit.Temperature = garden.Temperature;
            gardenToEdit.PrivacyLevel = garden.IsPrivate ? ModelEnums.PrivacyLevel.Private : ModelEnums.PrivacyLevel.Public;
            gardenToEdit.OnEdit(Utilities.GetUserName());

            ctx.SaveChanges();
        }

        [NonAction]
        public void DbDeleteGarden(int id)
        {
            var garden = (from g in ctx.Gardens
                          where g.Id == id && !g.Deleted
                          select g).FirstOrDefault();
            garden.Deleted = true;
            ctx.SaveChanges();
        }

        [NonAction]
        public GardenDetailsViewModel GetGardenDetailsViewModel(int gardenId)
        {
            var garden = DbGetGarden(gardenId);

            TodoController tc = new TodoController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            UserPlantController uc = new UserPlantController();

            GardenDetailsViewModel ret = new GardenDetailsViewModel
            {
                Id = garden.Id,
                CardinalDirection = garden.CardinalDirection,
                GroundType = garden.GroundType,
                Description = garden.Description,
                Inside = garden.Inside,
                Light = garden.Light,
                Name = garden.Name,
                PhType = garden.PhType,
                IsPrivate = garden.PrivacyLevel == ModelEnums.PrivacyLevel.Private,
                ShadowStrength = garden.ShadowStrength,
                Temperature = garden.Temperature,
                Wetness = garden.Wetness,
                Plants = uc.GetUserPlantIndexViewModelByGardenId(gardenId),
                TodoList = tc.GetTodoIndexViewModel(gardenId, false, TodoController.TAKE_AMOUNT)
            };

            HelperClasses.DbResponse imageResponse = rc.DbGetGardenReferencedImages(garden.Id);
            string rootPath = HttpRuntime.AppDomainAppVirtualPath != "/" ? HttpRuntime.AppDomainAppVirtualPath + "/" : "/";
            if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            {
                ret.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, rootPath);
            }
            else
            {
                ret.Images.Add(new _HtmlImageViewModel
                {
                    SrcAttr = rootPath + "Images/no-image.png",
                    Id = 0,
                    TitleAttr = "Kein Bild vorhanden"
                });
            }
            return ret;
        }

        [NonAction]
        public GardenIndexViewModel GetGardenIndexViewModel(Guid userid)
        {
            var userGardens = DbGetGardensByUserId(userid);
            List<GardenDetailsViewModel> gardenList = new List<GardenDetailsViewModel>();
            foreach (var ug in userGardens)
            {
                GardenDetailsViewModel temp = GetGardenDetailsViewModel(ug.Id);
                gardenList.Add(temp);
            }
            TodoController tc = new TodoController();
            var userTodos = tc.GetTodoIndexViewModel(userid);
            userTodos.TodoList = userTodos.TodoList.Take(TodoController.TAKE_AMOUNT);
            var property = new PropertyController().DbGetProperty(userid);

            return new GardenIndexViewModel { Gardens = gardenList, TodoList = userTodos, City = property.City };
        }
        #endregion
    }
}
