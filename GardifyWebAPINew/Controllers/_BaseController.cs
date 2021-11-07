using GardifyWebAPI;
using GardifyModels.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using GardifyWebAPI.App_Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;

namespace GardifyWebAPI.Controllers
{
    public class _BaseController : Controller
    {
        protected AspNetUserManager _userManager;
        public AspNetUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AspNetUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        public Error GetError(string errorMessage, HttpStatusCode? statusCode = null, string exception = null)
        {
            Error e = new Error()
            {
                ErrorMessage = errorMessage,
                Exception = exception,
                HttpStatusCode = statusCode
            };
            e.OnCreate(Utilities.GetUserName());
            return e;
        }

        public ActionResult RedirectToError(string errorMessage, HttpStatusCode? statusCode = null, string exception = null)
        {
            Error e = GetError(errorMessage, statusCode, exception);
            return RedirectToAction("Details", "Error", e);
        }

        protected ApplicationDbContext plantDB = new ApplicationDbContext();
        public int SaveChanges()
        {
            return plantDB.SaveChanges();
        }

        protected override async void OnActionExecuted(ActionExecutedContext filterContext)
        {
            ShopcartController sc = new ShopcartController();
            PointController poc = new PointController();
            UserPlantController upc = new UserPlantController();
            TodoController tc = new TodoController();

            base.OnActionExecuted(filterContext);

            if (filterContext.Controller.ViewData.Model == null)
            {
                filterContext.Controller.ViewData.Model = new _BaseViewModel();
            }
            if (User.Identity.IsAuthenticated && filterContext.Controller.ViewData.Model is _BaseViewModel)
            {
                Guid userId = Utilities.GetUserId();
                var todos = await tc.DbGetTodosByUserId(DateTime.Now, DateTime.Now.AddMonths(1), userId);

                (filterContext.Controller.ViewData.Model as _BaseViewModel).ShopcartCounter = sc.DbGetShopcartCountByUserID(userId);
                (filterContext.Controller.ViewData.Model as _BaseViewModel).Points = poc.GetUserPoints(userId);
                (filterContext.Controller.ViewData.Model as _BaseViewModel).NewMessages = 2; // NotImplemented
                (filterContext.Controller.ViewData.Model as _BaseViewModel).PlantCount = upc.DbGetUserPlantsByUserId(userId).Count();
                (filterContext.Controller.ViewData.Model as _BaseViewModel).CurrentTodoCount = todos.Count();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("get-add-plant-dialog")]
        public PartialViewResult getAddPlantToPropertyDialog(int plantId)
        {
            GardenController gc = new GardenController();
            ApplicationDbContext plantDB = new ApplicationDbContext();
            if (plantId > 0)
            {
                IEnumerable<GardifyModels.Models.Plant> plants = (from plant in plantDB.Plants
                                                                where plant.Id == plantId
                                                                select plant);

                if (plants != null && plants.Any() && User.Identity.IsAuthenticated)
                {
                    Guid userId = Utilities.GetUserId();

                    PropertyController pc = new PropertyController();
                    var property_sel = pc.DbGetProperty(userId);

                    if (property_sel != null)
                    {

                        var garden_sel = (from g in plantDB.Gardens
                                          where g.PropertyId == property_sel.Id
                                          && g.Deleted == false
                                          select g);

                        var todoTemplates = (from t in plantDB.TodoTemplate
                                             where t.ReferenceType == ModelEnums.ReferenceToModelClass.Plant
                                             && t.ReferenceId == plantId
                                             && !t.Deleted
                                             select t);
                        List<_TodoCheckedTemplateViewModel> checkedTodoTemplates = new List<_TodoCheckedTemplateViewModel>();
                        foreach (var template in todoTemplates)
                        {
                            _TodoCheckedTemplateViewModel mod = new _TodoCheckedTemplateViewModel
                            {
                                Checked = true,
                                TemplateId = template.Id,
                                TemplateTitle = template.Title
                            };
                            checkedTodoTemplates.Add(mod);
                        }

                        _modalAddPlantToGardenPropertyViewModel viewModel = new _modalAddPlantToGardenPropertyViewModel
                        {
                            PlantId = plantId,
                            Gardens = gc.DbGetGardensByUserId(userId),
                            PlantCount = 1,
                            InitialAgeInDays = 0,
                            Todos = checkedTodoTemplates,
                            IsInPot = false
                        };
                        return PartialView("_modalAddPlantToGarden", viewModel);
                    }
                }
            }
            return PartialView("_modalError");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("add-plant-to-property")]
        public PartialViewResult AddPlantToProperty(_modalAddPlantToGardenPropertyViewModel vm)
        {
            UserPlantController uc = new UserPlantController();
            if (vm.PlantId > 0 && User.Identity.IsAuthenticated)
            {
                UserPlant res = uc.DbAddPlantToProperty(vm.PlantId, vm.GardenId, Utilities.GetUserId(), User.Identity.GetUserName(), vm.InitialAgeInDays, vm.PlantCount, vm.IsInPot);
                if (res != null)
                {
                    return PartialView("_modalOk");
                }
            }
            return PartialView("_modalError");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("get-add-plant-to-watchlist-dialog")]
        public PartialViewResult getAddPlantToWatchlistDialog(int plantId)
        {
            PlantController pc = new PlantController();
            if (plantId > 0)
            {
                Plant plant = pc.DbGetPlantById(plantId);

                if (plant != null && User.Identity.IsAuthenticated)
                {
                    Guid userId = Utilities.GetUserId();

                    _modalAddPlantToWatchlistViewModel viewModel = new _modalAddPlantToWatchlistViewModel();
                    viewModel.PlantId = plantId;
                    viewModel.PlantCount = 1;
                    viewModel.NameGerman = plant.NameGerman;
                    viewModel.NameLatin = plant.NameLatin;
                    return PartialView("_modalAddPlantToWatchlist", viewModel);
                }
            }
            return PartialView("_modalError");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("add-plant-to-watchlist")]
        public PartialViewResult AddPlantToWatchlist(_modalAddPlantToWatchlistViewModel newEntryData)
        {
            PlantController pc = new PlantController();
            WatchListController wc = new WatchListController();
            if (newEntryData.PlantId > 0)
            {
                Plant plant = pc.DbGetPlantById(newEntryData.PlantId);

                if (plant != null && User.Identity.IsAuthenticated)
                {
                    Guid userId = Utilities.GetUserId();

                    WatchlistEntry newEntry = new WatchlistEntry();
                    newEntry.PlantId = plant.Id;
                    newEntry.UserId = userId;
                    newEntry.Count = newEntryData.PlantCount > 0 ? newEntryData.PlantCount : 1;

                    bool isOk = wc.DbCreateWatchlistEntry(newEntry);
                    if (isOk)
                    {
                        return PartialView("_modalOk");
                    }
                }
            }
            return PartialView("_modalError");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("move-plant-from-watchlist-to-garden")]
        public PartialViewResult MovePlantFromWatchlistToGarden(int watchlistEntryId, int plantId, int gardenId, int initialAgeInDays, int plantCount, bool isInPot)
        {
            PlantController pc = new PlantController();
            WatchListController wc = new WatchListController();
            Plant plant = pc.DbGetPlantById(plantId);

            if (plant != null && User.Identity.IsAuthenticated)
            {
                Guid userId = Utilities.GetUserId();

                var property_sel = (from p in plantDB.Property
                                    where p.UserId == userId
                                    select p);

                if (property_sel != null && property_sel.Any())
                {

                    var garden_sel = (from g in plantDB.Gardens
                                      where g.PropertyId == property_sel.FirstOrDefault().Id
                                      && g.Id == gardenId
                                      select g);

                    if (garden_sel != null && garden_sel.Any())
                    {
                        UserPlant newRelation = new UserPlant
                        {
                            PlantId = plantId,
                            Description = plant.Description,
                            Gardenid = garden_sel.FirstOrDefault().Id,
                            InitialAgeInDays = initialAgeInDays > 0 ? initialAgeInDays : 0,
                            Count = plantCount > 0 ? plantCount : 1,
                            IsInPot = isInPot
                        };
                        newRelation.OnCreate("Basecontroller");
                        plantDB.UserPlants.Add(newRelation);
                        bool isOk = (plantDB.SaveChanges() > 0) && wc.DbDeleteWatchlistEntryById(watchlistEntryId, userId) ? true : false;

                        if (isOk)
                        {
                            return PartialView("_modalOk");
                        }
                    }
                }
            }
            return PartialView("_modalError");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("remove-plant-from-watchlist")]
        public PartialViewResult RemovePlantFromWatchlist(int watchlistEntryId)
        {
            WatchListController wc = new WatchListController();
            if (watchlistEntryId > 0 && User.Identity.IsAuthenticated)
            {
                Guid userId = Utilities.GetUserId();
                wc.DbDeleteWatchlistEntryById(watchlistEntryId, userId);

                return PartialView("_Watchlist", wc.DbGetWatchlisEntriesByUserId(userId));
            }
            return null;
        }

        private RotateFlipType GetJPEGOrientationToFlipType(int jpegOrientation)
        {
            var rotateFlipType = RotateFlipType.RotateNoneFlipNone;
            switch (jpegOrientation)
            {
                case 1:
                    rotateFlipType = RotateFlipType.RotateNoneFlipNone;
                    break;
                case 2:
                    rotateFlipType = RotateFlipType.RotateNoneFlipX;
                    break;
                case 3:
                    rotateFlipType = RotateFlipType.Rotate180FlipNone;
                    break;
                case 4:
                    rotateFlipType = RotateFlipType.Rotate180FlipX;
                    break;
                case 5:
                    rotateFlipType = RotateFlipType.Rotate90FlipX;
                    break;
                case 6:
                    rotateFlipType = RotateFlipType.Rotate90FlipNone;
                    break;
                case 7:
                    rotateFlipType = RotateFlipType.Rotate270FlipX;
                    break;
                case 8:
                    rotateFlipType = RotateFlipType.Rotate270FlipNone;
                    break;
                default:
                    rotateFlipType = RotateFlipType.RotateNoneFlipNone;
                    break;
            }

            return rotateFlipType;
        }

        private bool RotateJPEGForEXIF(Image image)
        {
            if (image.RawFormat.Guid != ImageFormat.Jpeg.Guid)
            {
                return false;
            }

            try
            {
                var rotationProperty = image.GetPropertyItem(0x0112);
                if (rotationProperty == null)
                    return false;

                var flipType = GetJPEGOrientationToFlipType(rotationProperty.Value[0]);
                if (flipType == RotateFlipType.RotateNoneFlipNone)
                    return false;

                image.RotateFlip(flipType);
                image.RemovePropertyItem(0x0112);

                return true;
            } catch (ArgumentException e)
            {
                // rotation prop doesnt exist
                return false;
            }
        }

        private Stream PostProcessUploadedJPEG(Stream stream)
        {
            using (var image = Image.FromStream(stream))
            {
                var wasRotated = RotateJPEGForEXIF(image);
                if (!wasRotated)
                    return stream;

                var encoderParameters = new EncoderParameters();
                encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, (long)90);

                // There's no nicer way to get this
                ImageCodecInfo codec = null;
                foreach (var current in ImageCodecInfo.GetImageEncoders())
                {
                    if (current.MimeType == "image/jpeg")
                    {
                        codec = current;
                        break;
                    }
                }

                var modifiedStream = new MemoryStream();
                image.Save(modifiedStream, codec, encoderParameters);
                return modifiedStream;
            }
        }

        protected Files uploadFile(HttpPostedFileBase imageFile, ModelEnums.FileReferenceType fileReferenceType, string imageTitle = null, string imageDescription = null, string imageTags = null, DateTime? dateCreated = null, string fileNote = null)
        {
            if (imageFile == null || imageFile.ContentLength <= 0)
            {
                return null;
            }
            else
            {
                string originalFileName = imageFile.FileName;
                string extension = Path.GetExtension(imageFile.FileName).ToLower();

                Files newFile = new Files();
                newFile.OriginalFileNameA = originalFileName;

                bool isExtensionOk = false;

                Stream data = imageFile.InputStream;
                switch (extension)
                {
                    case ".jpg":
                        isExtensionOk = true;
                        data = PostProcessUploadedJPEG(imageFile.InputStream);
                        break;
                    case ".jpeg":
                        isExtensionOk = true;
                        data = PostProcessUploadedJPEG(imageFile.InputStream);
                        break;
                    case ".png":
                        isExtensionOk = true;
                        break;
                    case ".gif":
                        isExtensionOk = true;
                        break;
                    case ".svg":
                        isExtensionOk = true;
                        break;
                    default:
                        isExtensionOk = false;
                        break;
                }

                if (!isExtensionOk)
                {
                    return null;
                }
                else
                {
                    string relativePath = FileSystemObject.NFILES_FOLDER;

                    switch (fileReferenceType)
                    {
                        case ModelEnums.FileReferenceType.ArticleImage:
                            relativePath += FileSystemObject.ARTICLE_IMAGES_FOLDER;
                            break;
                        case ModelEnums.FileReferenceType.DiaryEntryImage:
                            relativePath += FileSystemObject.DIARY_IMAGES_FOLDER;
                            break;
                        case ModelEnums.FileReferenceType.FaqEntryImage:
                            relativePath += FileSystemObject.FAQ_IMAGES_FOLDER;
                            break;
                        case ModelEnums.FileReferenceType.GardenImage:
                            relativePath += FileSystemObject.GARDEN_IMAGES_FOLDER;
                            break;
                        case ModelEnums.FileReferenceType.NewsEntryImage:
                            relativePath += FileSystemObject.NEWS_IMAGES_FOLDER;
                            break;
                        case ModelEnums.FileReferenceType.PlantImage:
                            relativePath += FileSystemObject.PLANT_IMAGES_FOLDER;
                            break;
                        case ModelEnums.FileReferenceType.ScanImage:
                            relativePath += FileSystemObject.SCAN_IMAGES_FOLDER;
                            break;
                        case ModelEnums.FileReferenceType.AdminDeviceImg:
                            relativePath += FileSystemObject.DEVICE_IMAGES_FOLDER;
                            break;
                        case ModelEnums.FileReferenceType.QuestionImage:
                            relativePath += FileSystemObject.QUESTION_IMAGES_FOLDER;
                            break;
                        case ModelEnums.FileReferenceType.PlantDocAnswerImage:
                            relativePath += FileSystemObject.PLANTDOC_ANSWER_IMAGES_FOLDER;
                            break;
                        case ModelEnums.FileReferenceType.UserProfileImage:
                            relativePath += FileSystemObject.USERPROFILES_IMAGES_FOLDER;
                            break;
                        case ModelEnums.FileReferenceType.AlbumImage:
                            relativePath += FileSystemObject.ALBUM_IMAGES_FOLDER;
                            break;

                        case ModelEnums.FileReferenceType.NotSet:
                        default:
                            break;
                    }
                    String destination = "";
                    if (Server == null)
                    {
                        destination = System.Web.Hosting.HostingEnvironment.MapPath(@"~/" + relativePath);
                    }
                    else
                    {
                        destination = Server.MapPath(@"~/" + relativePath);
                    }
                    // upload file
                    string savedFileName = Utilities.tryToSaveUploadedFile(data, imageFile.FileName, destination);

                    if (!string.IsNullOrEmpty(savedFileName))
                    {
                        newFile.FileA = savedFileName;
                        newFile.FilePath = relativePath;
                        newFile.TagsDE = imageTags ?? imageTitle;
                        newFile.DescriptionDE = imageDescription;
                        newFile.FileE = fileNote;
                        newFile.ApplicationId = Utilities.GetApplicationId();
                        newFile.EditedBy = Utilities.GetUserName();
                        newFile.EditedDate = DateTime.Now;
                        newFile.WrittenBy = Utilities.GetUserName();
                        newFile.WrittenDate = DateTime.Now;
                        newFile.UserCreatedDate = dateCreated == null ? DateTime.Now : dateCreated.Value;
                        newFile.TestFile = false;
                        return newFile;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        public Files UploadAndRegisterFileFull(HttpPostedFileBase uploadedFile, int referencedObjectId, int moduleId, ModelEnums.FileReferenceType fileReferenceType, string fileTitle = null, string fileDescription = null)
        {
            FileSystemObjectController fc = new FileSystemObjectController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            // upload file
            Files newFile = uploadFile(uploadedFile, fileReferenceType, fileTitle, fileDescription);

            // check if successfully uploaded
            if (newFile != null)
            {
                // register file in DB
                HelperClasses.DbResponse response = fc.DbCreateFile(newFile);

                if (response.Status != ModelEnums.ActionStatus.Success)
                {
                    return newFile;
                }
                else
                {
                    // create reference object <-> file
                    FileToModule ftm = new FileToModule
                    {
                        Editable = true,
                        EditedBy = Utilities.GetUserName(),
                        EditedDate = DateTime.Now,
                        InsertedDate = DateTime.Now,
                        InsertedBy = Utilities.GetUserName(),
                        FileID = ((Files)response.ResponseObjects.FirstOrDefault()).FileID,
                        DetailID = referencedObjectId,
                        AltText = fileTitle == null ? "NewImage" : fileTitle,
                        ApplicationId = Utilities.GetApplicationId()

                    };
                    ftm.Description = fileDescription == null ? ftm.AltText : fileDescription;
                    ftm.ModuleID = moduleId;

                    response = rc.DbCreateFileToModule(ftm);

                    return newFile;
                }
            }
            return newFile;
        }

        protected bool UploadAndRegisterFile(HttpPostedFileBase uploadedFile, int referencedObjectId, int moduleId, ModelEnums.FileReferenceType fileReferenceType, string fileTitle = null, string fileDescription = null, string fileTags = null, DateTime? imageCreatedDate = null, string fileNote = null)
        {
            FileSystemObjectController fc = new FileSystemObjectController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            // upload file
            Files newFile = uploadFile(uploadedFile, fileReferenceType, fileTitle, fileDescription, fileTags, imageCreatedDate, fileNote);

            // check if successfully uploaded
            if (newFile != null)
            {
                // register file in DB
                HelperClasses.DbResponse response = fc.DbCreateFile(newFile);

                if (response.Status != ModelEnums.ActionStatus.Success)
                {
                    return false;
                }
                else
                {
                    // create reference object <-> file
                    FileToModule ftm = new FileToModule
                    {
                        Editable = true,
                        EditedBy = Utilities.GetUserName(),
                        EditedDate = DateTime.Now,
                        InsertedDate = DateTime.Now,
                        InsertedBy = Utilities.GetUserName(),
                        FileID = ((Files)response.ResponseObjects.FirstOrDefault()).FileID,
                        DetailID = referencedObjectId,
                        AltText = fileTitle == null ? "NewImage" : fileTitle,
                        ApplicationId = Utilities.GetApplicationId()

                    };
                    ftm.Description = fileDescription == null ? ftm.AltText : fileDescription;
                    ftm.ModuleID = moduleId;

                    response = rc.DbCreateFileToModule(ftm);

                    return true;
                }
            }
            return false;
        }

        protected int UploadAndRegisterFileWithId(HttpPostedFileBase uploadedFile, int referencedObjectId, int moduleId, ModelEnums.FileReferenceType fileReferenceType, string fileTitle = null, string fileDescription = null, string fileTags = null, DateTime? imageCreatedDate = null, string fileNote = null)
        {
            FileSystemObjectController fc = new FileSystemObjectController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            // upload file
            Files newFile = uploadFile(uploadedFile, fileReferenceType, fileTitle, fileDescription, fileTags, imageCreatedDate, fileNote);

            // check if successfully uploaded
            if (newFile != null)
            {
                // register file in DB
                HelperClasses.DbResponse response = fc.DbCreateFile(newFile);

                if (response.Status != ModelEnums.ActionStatus.Success)
                {
                    return 0;
                }
                else
                {
                    // create reference object <-> file
                    FileToModule ftm = new FileToModule
                    {
                        Editable = true,
                        EditedBy = Utilities.GetUserName(),
                        EditedDate = DateTime.Now,
                        InsertedDate = DateTime.Now,
                        InsertedBy = Utilities.GetUserName(),
                        FileID = ((Files)response.ResponseObjects.FirstOrDefault()).FileID,
                        DetailID = referencedObjectId,
                        AltText = fileTitle == null ? "NewImage" : fileTitle,
                        ApplicationId = Utilities.GetApplicationId()

                    };
                    ftm.Description = fileDescription == null ? ftm.AltText : fileDescription;
                    ftm.ModuleID = moduleId;

                    response = rc.DbCreateFileToModule(ftm);

                    return ftm.FileToModuleID;
                }
            }
            return 0;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                plantDB.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}