using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using PflanzenApp.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
{
    [CustomAuthorize]
    public class _BaseController : Controller
    {
        private ApplicationUserManager _userManager;
        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
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

        protected ApplicationDbContext ctx = new ApplicationDbContext();
        public int SaveChanges()
        {
            return ctx.SaveChanges();
        }

        protected override void OnActionExecuted(ActionExecutedContext filterContext)
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
                (filterContext.Controller.ViewData.Model as _BaseViewModel).ShopcartCounter = sc.DbGetShopcartCountByUserID(userId);
                (filterContext.Controller.ViewData.Model as _BaseViewModel).Points = poc.GetUserPoints(userId);
                (filterContext.Controller.ViewData.Model as _BaseViewModel).NewMessages = 2; // NotImplemented
                // (filterContext.Controller.ViewData.Model as _BaseViewModel).PlantCount = upc.DbGetUserPlantsByUserId(userId).Count();
                (filterContext.Controller.ViewData.Model as _BaseViewModel).PlantCount = 0;
                // (filterContext.Controller.ViewData.Model as _BaseViewModel).CurrentTodoCount = tc.DbGetTodosByUserId(DateTime.Now, DateTime.Now.AddMonths(1), userId).Count();
                (filterContext.Controller.ViewData.Model as _BaseViewModel).CurrentTodoCount = 0;
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("get-add-plant-dialog")]
        public PartialViewResult getAddPlantToPropertyDialog(int plantId)
        {
            GardenController gc = new GardenController();
            ApplicationDbContext ctx = new ApplicationDbContext();
            if (plantId > 0)
            {
                IEnumerable<GardifyModels.Models.Plant> plants = (from plant in ctx.Plants
                                                                where plant.Id == plantId
                                                                select plant);

                if (plants != null && plants.Any() && User.Identity.IsAuthenticated)
                {
                    Guid userId = Utilities.GetUserId();

                    PropertyController pc = new PropertyController();
                    var property_sel = pc.DbGetProperty(userId);

                    if (property_sel != null)
                    {

                        var garden_sel = (from g in ctx.Gardens
                                          where g.PropertyId == property_sel.Id
                                          && g.Deleted == false
                                          select g);

                        var todoTemplates = (from t in ctx.TodoTemplate
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
                bool isOk = uc.DbAddPlantToProperty(vm.PlantId, vm.GardenId, Utilities.GetUserId(), User.Identity.GetUserName(), vm.InitialAgeInDays, vm.PlantCount, vm.IsInPot, vm.Todos);
                if (isOk)
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

                var property_sel = (from p in ctx.Property
                                    where p.UserId == userId
                                    select p);

                if (property_sel != null && property_sel.Any())
                {

                    var garden_sel = (from g in ctx.Gardens
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
                        ctx.UserPlants.Add(newRelation);
                        bool isOk = (ctx.SaveChanges() > 0) && wc.DbDeleteWatchlistEntryById(watchlistEntryId, userId) ? true : false;

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

        protected Files uploadFile(HttpPostedFileBase imageFile, ModelEnums.FileReferenceType fileReferenceType, string imageTitle = null, string imageDescription = null, string imageLicense = null, string imageAuthor = null)
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

                switch (extension)
                {
                    case ".jpg":
                        isExtensionOk = true;
                        break;
                    case ".jpeg":
                        isExtensionOk = true;
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

                        case ModelEnums.FileReferenceType.TagImage:
                            relativePath += FileSystemObject.TAGS_IMAGES_FOLDER;
                            break;

                        case ModelEnums.FileReferenceType.AdminDeviceImg:
                            relativePath += FileSystemObject.DEVICE_IMAGES_FOLDER;
                            break;
                        case ModelEnums.FileReferenceType.EcoElementImages:
                            relativePath += FileSystemObject.ECOELE_IMAGES_FOLDER;
                            break;
                        case ModelEnums.FileReferenceType.EventImage:
                            relativePath += FileSystemObject.EVENTS_IMAGES_FOLDER;
                            break;
                        case ModelEnums.FileReferenceType.LexiconTermImage:
                            relativePath += FileSystemObject.LEXICONTERMS_IMAGES_FOLDER;
                            break;
                        case ModelEnums.FileReferenceType.QuestionImage:
                            relativePath += FileSystemObject.QUESTION_IMAGES_FOLDER;
                            break;
                        case ModelEnums.FileReferenceType.PlantDocAnswerImage:
                            relativePath += FileSystemObject.PLANTDOC_ANSWER_IMAGES_FOLDER;
                            break;
                        case ModelEnums.FileReferenceType.NotSet:
                        default:

                            break;
                    }

                    string filePath = Server.MapPath(@"~/" + relativePath);
                    // upload file
                    string savedFileName = Utilities.tryToSaveUploadedFile(imageFile, filePath);

                    if (!string.IsNullOrEmpty(savedFileName))
                    {
                        newFile.FileA = savedFileName;
                        newFile.FilePath = relativePath;
                        newFile.TagsDE = imageTitle;
                        newFile.DescriptionDE = imageDescription;
                        newFile.ApplicationId = Utilities.GetApplicationId();
                        newFile.EditedBy = Utilities.GetUserName();
                        newFile.EditedDate = DateTime.Now;
                        newFile.WrittenBy = Utilities.GetUserName();
                        newFile.WrittenDate = DateTime.Now;
                        newFile.TestFile = false;
                        newFile.FileC = imageLicense;
                        newFile.FileD = imageAuthor;

                        try
                        {
                            ResizeImage(filePath + newFile.FileA, filePath + "250/" + newFile.FileA, 250, 0, 100);
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine("Failed for image " + newFile.FileA + "\n" + e.ToString());
                        }
                        return newFile;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }

        protected bool UploadAndRegisterFile(HttpPostedFileBase uploadedFile, int referencedObjectId, int moduleId, ModelEnums.FileReferenceType fileReferenceType, string fileTitle = null, string fileDescription = null, string imageLicense = null, string imageAuthor = null)
        {
            FileSystemObjectController fc = new FileSystemObjectController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            // upload file
            Files newFile = uploadFile(uploadedFile, fileReferenceType, fileTitle, fileDescription, imageLicense, imageAuthor);

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

                    try
                    {
                        string filePath = Server.MapPath(@"~/" + newFile.FilePath);
                        ResizeImage(filePath + newFile.FileA, filePath + "250/" + newFile.FileA, 250, 0, 100);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Failed for image " + newFile.FileA + "\n" + e.ToString());
                    }

                    return true;
                }
            }
            return false;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ctx.Dispose();
            }
            base.Dispose(disposing);
        }

        protected static void ResizeImage(string originalFile, string destinationFile, int finalWidth, int finalHeight, int imageQuality)
        {

            System.Drawing.Bitmap NewBMP;
            System.Drawing.Graphics graphicTemp;
            System.Drawing.Bitmap bmp = new System.Drawing.Bitmap(originalFile);

            int iWidth;
            int iHeight;
            if ((finalHeight == 0) && (finalWidth != 0))
            {
                iWidth = finalWidth;
                iHeight = (bmp.Size.Height * iWidth / bmp.Size.Width);
            }
            else if ((finalHeight != 0) && (finalWidth == 0))
            {
                iHeight = finalHeight;
                iWidth = (bmp.Size.Width * iHeight / bmp.Size.Height);
            }
            else
            {
                iWidth = finalWidth;
                iHeight = finalHeight;
            }

            NewBMP = new System.Drawing.Bitmap(iWidth, iHeight);
            graphicTemp = System.Drawing.Graphics.FromImage(NewBMP);
            graphicTemp.CompositingMode = System.Drawing.Drawing2D.CompositingMode.SourceOver;
            graphicTemp.CompositingQuality = System.Drawing.Drawing2D.CompositingQuality.HighQuality;
            graphicTemp.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
            graphicTemp.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphicTemp.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            graphicTemp.DrawImage(bmp, 0, 0, iWidth, iHeight);
            graphicTemp.Dispose();
            System.Drawing.Imaging.EncoderParameters encoderParams = new System.Drawing.Imaging.EncoderParameters();
            System.Drawing.Imaging.EncoderParameter encoderParam = new System.Drawing.Imaging.EncoderParameter(System.Drawing.Imaging.Encoder.Quality, imageQuality);
            encoderParams.Param[0] = encoderParam;
            System.Drawing.Imaging.ImageCodecInfo[] arrayICI = System.Drawing.Imaging.ImageCodecInfo.GetImageEncoders();
            for (int fwd = 0; fwd <= arrayICI.Length - 1; fwd++)
            {
                if (arrayICI[fwd].FormatDescription.Equals("JPEG"))
                {
                    NewBMP.Save(destinationFile, arrayICI[fwd], encoderParams);
                }
            }

            NewBMP.Dispose();
            bmp.Dispose();
        }
    }
}