using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Data.Entity;
using System.Web.Mvc;
using GardifyModels.Models;
using Microsoft.AspNet.Identity;
using GardifyWebAPI.App_Code;
using System.IO;
using static GardifyModels.Models.GardenViewModels;
using static GardifyModels.Models.TodoViewModels;
using System.Diagnostics;
using System.Threading.Tasks;
using static GardifyModels.Models.UserPlantViewModels;

namespace GardifyWebAPI.Controllers
{

    public class GardenController : _BaseController
    {
        private DeviceController dc = new DeviceController();
        private ImgResizerController imgResizer = new ImgResizerController();
        // GET: Garden
        //public IEnumerable<GardenDetailsViewModel> Index()
        //{
        //    WatchListController wc = new WatchListController();

        //    //var userId = new Guid(User.Identity.GetUserId());
        //    //var userId = new Guid("64535925-4fd4-4e51-9001-b0e19f776379");
        //    var userId = Utilities.GetUserId();
        //    var userProperty = new PropertyController().DbGetProperty(userId);
        //    if (userProperty == null)
        //    {
        //        return new List<GardenDetailsViewModel>();
        //        //return RedirectToAction("Create", "Property");
        //    }

        //    GardenIndexViewModel viewModel = GetGardenIndexViewModel(userId);
        //    TodoController tc = new TodoController();

        //    WeatherHandler wh = new WeatherHandler();
        //    WeatherForecast forecast = wh.getWeatherForecastByGeoCoords(userProperty.Latitude, userProperty.Longtitude);
        //    if (forecast != null)
        //    {
        //        WeatherHelpers.HourlyForecast weatherCurrentHour = forecast.Forecasts.Hourly.FirstOrDefault();
        //        viewModel.CurrentWeather = weatherCurrentHour;
        //    }

        //    viewModel.WatchlistEntries = wc.DbGetWatchlisEntriesByUserId(userId);

        //    //!!!!!!!!!!!!! WARNUNGEN //!!!!!!!!!!!!!
        //    Stopwatch watch = new Stopwatch();
        //    watch.Start();
        //    Debug.WriteLine("Warnung Anfang ");

        //    UserPlantController upc = new UserPlantController();
        //    AlertController ac = new AlertController();

        //    //Debug.WriteLine("Warnung controller geladen. " + watch.ElapsedMilliseconds);

        //    IEnumerable<UserPlant> userPlants = upc.DbGetUserPlantsByUserId(userId);
        //    //Debug.WriteLine("Warnung user plants geladen. " + watch.ElapsedMilliseconds);

        //    List<AlertViewModels.UserPlantAlertsViewModel> triggeredAlertViews = new List<AlertViewModels.UserPlantAlertsViewModel>();

        //    if (userPlants != null && userPlants.Any())
        //    {
        //        foreach (var userPlant in userPlants)
        //        {
        //            long alertBeginnTime = watch.ElapsedMilliseconds;
        //            AlertViewModels.UserPlantAlertsViewModel userPlantAlertViewModel;

        //            long tmpTime = watch.ElapsedMilliseconds;
        //            IEnumerable<Alert> triggeredPlantAlerts = ac.DbGetTriggeredAlertsByPlantId(userPlant.PlantId, forecast);
        //            Debug.WriteLine("Aktive Alerts für " + userPlant.Name + " geladen in: " + (watch.ElapsedMilliseconds - tmpTime));

        //            if (triggeredPlantAlerts != null && triggeredPlantAlerts.Any())
        //            {
        //                userPlantAlertViewModel = new AlertViewModels.UserPlantAlertsViewModel
        //                {
        //                    Plant = userPlant.Plant,
        //                    UserPlantId = userPlant.Id,
        //                    UserPlantName = userPlant.Name
        //                };
        //                userPlantAlertViewModel.AlertViewModels.AddRange(ac.DbGetAlertViewModels(triggeredPlantAlerts));
        //                triggeredAlertViews.Add(userPlantAlertViewModel);
        //            }

        //            Debug.WriteLine("Berechnung für " + userPlant.Name + " dauerte: " + (watch.ElapsedMilliseconds - alertBeginnTime));
        //        }
        //    }

        //    viewModel.Alerts = triggeredAlertViews;

        //    watch.Stop();
        //    Debug.WriteLine("Warnung Ende. " + watch.ElapsedMilliseconds);
        //    //!!!!!!!!!!! /WARNUNGEN //!!!!!!!!!!!!!

        //    return viewModel.Gardens;
        //}

        public IEnumerable<Garden> IndexLiteLite()
        {
            var userId = Utilities.GetUserId();
            var userGardens = DbGetGardensByUserId(userId);
            return userGardens;
        }

        public IEnumerable<GardenDetailsViewModel> IndexLite(bool withPlant = true)
        {
            var userId = Utilities.GetUserId();

            return GetUserGardensLite(userId, withPlant);
        }

        // GET: Garden/Details/5
        public GardenDetailsViewModel Details(int? id)
        {
            if (id == null)
            {
                throw new Exception("Seite konnte nicht gefunden werden.");//, HttpStatusCode.NotFound, "GardenController.Details(" + id + ")");
            }

            GardenDetailsViewModel viewModel = GetGardenDetailsViewModel((int)id);

            return viewModel;
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
        public IEnumerable<GardenDetailsViewModel> Create(GardenCreateViewModel gardenView)
        {
            if (ModelState.IsValid)
            {
                DbAddGarden(gardenView);
                return IndexLite();
            }
            return IndexLite();
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
                throw new Exception("Seite konnte nicht gefunden werden.");//, HttpStatusCode.NotFound, "GardenController.Edit(" + id + ")");
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
        public GardenEditViewModel Edit([Bind(Include = "id,name,wetness,phType,groundType,info,cardinalDirection,shadowStrength,inside,temperature,light,IsPrivate,MainImageId")] GardenEditViewModel gardenView)
        {
            PropertyController proc = new PropertyController();
            if (ModelState.IsValid)
            {
                DbUpdateGarden(gardenView);
                // return RedirectToAction("Details", new { id = gardenView.Id });
                return gardenView;
            }

            return gardenView;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-garden-image")]
        public bool UploadGardenImage(HttpPostedFileBase imageFile, HttpPostedFile imageFileSrc, ImageMetadataViewModel metadata)
        {
            if (imageFile == null || imageFile.ContentLength <= 0)
            {
                return false;
            }
            else if (imageFile != null)
            {
                string fileNameWithoutExtension = Utilities.stringToUri(System.IO.Path.GetFileNameWithoutExtension(imageFile.FileName));
                string extension = Path.GetExtension(imageFile.FileName).ToLower();
                string relativePath = System.Web.Hosting.HostingEnvironment.MapPath("~/nfiles/GardenImages/");
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
                UploadAndRegisterFile(imageFile, metadata.ImageId, (int)ModelEnums.ReferenceToModelClass.Garden, ModelEnums.FileReferenceType.GardenImage, metadata.Title, metadata.Description, metadata.Tags, metadata.UserCreatedDate, metadata.Note);
            }

            return true;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-album-image")]
        public bool UploadAlbumImage(HttpPostedFileBase imageFile, HttpPostedFile imageFileSrc, ImageMetadataViewModel metadata)
        {
            if (imageFile == null || imageFile.ContentLength <= 0)
            {
                return false;
            }
            else if (imageFile != null)
            {
                string fileNameWithoutExtension = Utilities.stringToUri(System.IO.Path.GetFileNameWithoutExtension(imageFile.FileName));
                string extension = Path.GetExtension(imageFile.FileName).ToLower();
                string relativePath = System.Web.Hosting.HostingEnvironment.MapPath("~/nfiles/AlbumImages/");
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
                UploadAndRegisterFile(imageFile, metadata.ImageId, (int)ModelEnums.ReferenceToModelClass.AlbumImage, ModelEnums.FileReferenceType.AlbumImage, metadata.Title, metadata.Description, metadata.Tags, metadata.UserCreatedDate, metadata.Note);
            }

            return true;
        }

        public int UploadAlbumImageWithId(HttpPostedFileBase imageFile, HttpPostedFile imageFileSrc, ImageMetadataViewModel metadata)
        {
            if (imageFile == null || imageFile.ContentLength <= 0)
            {
                return 0;
            }
 
            string fileNameWithoutExtension = Utilities.stringToUri(System.IO.Path.GetFileNameWithoutExtension(imageFile.FileName));
            string extension = Path.GetExtension(imageFile.FileName).ToLower();
            string relativePath = System.Web.Hosting.HostingEnvironment.MapPath("~/nfiles/AlbumImages/");
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
            var imageId = UploadAndRegisterFileWithId(imageFile, metadata.ImageId, (int)ModelEnums.ReferenceToModelClass.AlbumImage, ModelEnums.FileReferenceType.AlbumImage, metadata.Title, metadata.Description, metadata.Tags, metadata.UserCreatedDate, metadata.Note);
            return imageId;

            

        }

        public int UploadPresentationImageWithId(HttpPostedFileBase imageFile, HttpPostedFile imageFileSrc, ImageMetadataViewModel metadata)
        {
            if (imageFile == null || imageFile.ContentLength <= 0)
            {
                return 0;
            }

            string fileNameWithoutExtension = Utilities.stringToUri(System.IO.Path.GetFileNameWithoutExtension(imageFile.FileName));
            string extension = Path.GetExtension(imageFile.FileName).ToLower();
            string relativePath = System.Web.Hosting.HostingEnvironment.MapPath("~/nfiles/PresentationImages/");
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
            var imageId = UploadAndRegisterFileWithId(imageFile, metadata.ImageId, (int)ModelEnums.ReferenceToModelClass.PresentationImage, ModelEnums.FileReferenceType.PresentationImage, metadata.Title, metadata.Description, metadata.Tags, metadata.UserCreatedDate, metadata.Note);
            return imageId;



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
                ret = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
            }
            else
            {
                ret.Add(new _HtmlImageViewModel
                {
                    SrcAttr = Utilities.GetAbsolutePath("~/Images/gardify_Pflanzenbild_Platzhalter.svg"),
                    Id = 0,
                    TitleAttr = "Kein Bild vorhanden"
                });
            }
            return ret;
        }

        [NonAction]
        public Garden DbGetGarden(int id)
        {
            return (from g in plantDB.Gardens
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
            IEnumerable<Garden> ret = new List<Garden>();
            ret = (from p in plantDB.Property
                   where !p.Deleted && p.UserId == userId
                   join g in plantDB.Gardens
                   on p.Id equals g.PropertyId
                   where !g.Deleted && g.PrivacyLevel == privacyLevel
                   select g);

            if (ret == null || !ret.Any())
            {
                GardenViewModels.GardenCreateViewModel ng = new GardenViewModels.GardenCreateViewModel();
                ng.Description = "Meine Gartenbeschreibung";
                ng.Name = "Mein Garten";
                CreateGardenOnRegister(ng, userId.ToString(),false);
                ret = (from p in plantDB.Property
                       where !p.Deleted && p.UserId == userId
                       join g in plantDB.Gardens
                       on p.Id equals g.PropertyId
                       where !g.Deleted && g.PrivacyLevel == privacyLevel
                       select g);
            }
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

            plantDB.Gardens.Add(garden);
            plantDB.SaveChanges();
        }

        public void CreateGardenOnRegister(GardenCreateViewModel gardenView, string userId, bool demoMode)
        {
            PropertyController proc = new PropertyController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            int propertyId = (int)proc.DbGetPropertyId(new Guid(userId));
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
            plantDB.Gardens.Add(garden);
            plantDB.SaveChanges();
            CreateStandardUserLists(garden.Id);
            if (demoMode) { 
                autoAddPlantsToUserListsForDemo(garden.Id, userId);
                int[] adminDevicesId = { 11, 16, 12 };
                dc.AddAdminDeviceToUserDevicesList(adminDevicesId, proc.DbGetProperty(new Guid(userId)));
            }
            // FileIDs for placeholder images
            foreach (var fileId in new int[] { 2954, 2956 })
            {
                FileToModule ftm = new FileToModule
                {
                    Editable = true,
                    EditedBy = Utilities.GetUserName(),
                    EditedDate = DateTime.Now,
                    InsertedDate = DateTime.Now,
                    InsertedBy = Utilities.GetUserName(),
                    FileID = fileId,
                    DetailID = garden.Id,
                    AltText = "Platzhalter",
                    ApplicationId = Utilities.GetApplicationId()
                };
                ftm.Description = "Platzhalter";
                ftm.ModuleID = (int)ModelEnums.ReferenceToModelClass.Garden;

                var response = rc.DbCreateFileToModule(ftm);
            }
           
        }
        public void autoAddPlantsToUserListsForDemo(int gardenId, string userId)
        {
            //UserPlantsAPIController upc = new UserPlantsAPIController(); 
            var userlistList = from u in plantDB.UserLists where u.GardenId == gardenId && !u.Deleted select u;
            if(userlistList!=null && userlistList.Any())
            {
                UserPlantToUserList up = null;
                BorrowUserPlantViewModel b = null;
                foreach (var userlist in userlistList)
                {
                    UserPlantToUserListView[] uPlantTrigger =  { new UserPlantToUserListView
                    {
                        UserPlantId = 1,
                        UserListId = userlist.Id
                    }};
                    switch (userlist.Name)
                    {
                        case "Vorgarten" :
                            b = new BorrowUserPlantViewModel { GardenId = gardenId, PlantId = 100, InitialAgeInDays=0,Count=1,IsInPot=false,};

                            var userPlant1 = AddUserPlantForDemo(b, userId, uPlantTrigger);
                            up = new UserPlantToUserList
                            {
                                UserListId = userlist.Id,
                                PlantId = userPlant1.Id,
                            };
                            up.OnCreate(Utilities.GetUserName());
                            
                            break;
                        case "Topfpflanzen":
                            b = new BorrowUserPlantViewModel { GardenId = gardenId, PlantId = 243, InitialAgeInDays = 0, Count = 1, IsInPot = true, };

                            var userPlant2= AddUserPlantForDemo(b, userId, uPlantTrigger);
                            up = new UserPlantToUserList
                            {
                                UserListId = userlist.Id,
                                PlantId = userPlant2.Id,
                            };
                            up.OnCreate(Utilities.GetUserName());
                            break;
                        case "Hauptgarten":
                            b = new BorrowUserPlantViewModel { GardenId = gardenId, PlantId = 425, InitialAgeInDays = 0, Count = 1, IsInPot = false, };

                            
                            var userPlant3 = AddUserPlantForDemo(b, userId, uPlantTrigger);
                            up = new UserPlantToUserList
                            {
                                UserListId = userlist.Id,
                                PlantId = userPlant3.Id,
                            };
                            up.OnCreate(Utilities.GetUserName());
                            break;
                    }
                    //plantDB.UserPlantToUserLists.Add(up);
                  
                }
                plantDB.SaveChanges();
            }
        }
        public void CreateStandardUserLists(int gardenId)
        {
            //add Userlist 
            List<UserList> standardUserList = new List<UserList>
                {
                    new UserList{ Name="Hauptgarten", Description="No Description", GardenId= gardenId,  },
                    new UserList{ Name="Vorgarten", Description="No Description", GardenId= gardenId,  },
                    new UserList{ Name="Topfpflanzen", Description="No Description", GardenId= gardenId,  }
                };

            foreach (var a in standardUserList)
            {
                a.OnCreate(Utilities.GetUserName());
                plantDB.UserLists.Add(a);
            }
            plantDB.SaveChanges();
        }
        public UserPlant AddUserPlantForDemo(BorrowUserPlantViewModel up, string userIdDemoUser, UserPlantToUserListView[] uPlantTrigger)
        {
            var userId = new Guid(userIdDemoUser);
            var userProperty = new PropertyController().DbGetProperty(userId);
            var ownGarden = userProperty.Gardens.Where(g => !g.Deleted).FirstOrDefault();

            UserPlantController upc = new UserPlantController();
            var userName = Utilities.GetUserName();

          

            UserPlant res = upc.DbAddPlantToProperty(up.PlantId, ownGarden.Id, userId, userName, up.InitialAgeInDays, up.Count, up.IsInPot, false, uPlantTrigger);
            return res;
        }
        
        [NonAction]
        public void DbUpdateGarden(GardenEditViewModel garden)
        {
            var gardenToEdit = (from g in plantDB.Gardens
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
            gardenToEdit.MainImageId = garden.MainImageId;
            gardenToEdit.OnEdit(Utilities.GetUserName());

            plantDB.SaveChanges();
        }

        [NonAction]
        public void DbDeleteGarden(int id)
        {
            var garden = (from g in plantDB.Gardens
                          where g.Id == id && !g.Deleted
                          select g).FirstOrDefault();
            garden.Deleted = true;
            plantDB.SaveChanges();
        }

        [NonAction]
        public GardenDetailsViewModel GetGardenDetailsViewModel(Garden garden, bool withPlants = true)
        {
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
                MainImageId = garden.MainImageId,
                PlantsLight = withPlants ? uc.GetUserPlantLightViewModel(garden.Id) : null,
                //TodoList = tc.GetTodoIndexViewModel(gardenId, false, TodoController.TAKE_AMOUNT)
            };
            HelperClasses.DbResponse imageResponse = rc.DbGetGardenReferencedImages(garden.Id);
            string rootPath = HttpRuntime.AppDomainAppVirtualPath != "/" ? HttpRuntime.AppDomainAppVirtualPath + "/" : "/";
            if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            {
                ret.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, rootPath);
            }
            return ret;
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
                //Temperature = garden.Temperature,
               // Wetness = garden.Wetness,
                //PlantsLight = uc.GetUserPlantLightViewModel(gardenId),
                MainImageId = garden.MainImageId,
                //TodoList = tc.GetTodoIndexViewModel(gardenId, false)
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
                    SrcAttr = rootPath + "Images/gardify_Pflanzenbild_Platzhalter.svg",
                    Id = 0,
                    TitleAttr = "Kein Bild vorhanden"
                });
            }
            return ret;
        }

        public IEnumerable<GardenDetailsViewModel> GetUserGardensLite(Guid userId, bool withPlant = true)
        {
            var userGardens = DbGetGardensByUserId(userId).ToList();
            List<GardenDetailsViewModel> gardenList = new List<GardenDetailsViewModel>();
            foreach (var ug in userGardens)
            {
                GardenDetailsViewModel temp = GetGardenDetailsViewModel(ug, withPlant);
                gardenList.Add(temp);
            }
            return gardenList;
        }

        [NonAction]
        public async Task<GardenIndexViewModel> GetGardenIndexViewModel(Guid userid)
        {
            var userGardens = DbGetGardensByUserId(userid);
            List<GardenDetailsViewModel> gardenList = new List<GardenDetailsViewModel>();
            foreach (var ug in userGardens)
            {
                GardenDetailsViewModel temp = GetGardenDetailsViewModel(ug);
                gardenList.Add(temp);
            }
            TodoController tc = new TodoController();
            var userTodos = await tc.GetTodoIndexViewModel(userid);
            userTodos.TodoList = userTodos.TodoList.Take(TodoController.TAKE_AMOUNT);
            var property = new PropertyController().DbGetProperty(userid);

            return new GardenIndexViewModel { Gardens = gardenList, TodoList = userTodos, City = property?.City };
        }
        #endregion
    }
}
