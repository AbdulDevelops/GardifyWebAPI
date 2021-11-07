using Microsoft.AspNet.Identity;
using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GardifyWebAPI.Controllers.AdminArea
{
    // [CustomAuthorizeAttribute(Roles = "Admin")]
    public class AdminAreaNewsController : _BaseController
    {
        // GET: News
        public ActionResult Index()
        {
            NewsController nc = new NewsController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            NewsViewModels.NewsListViewModel viewModel = new NewsViewModels.NewsListViewModel();
            viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;

            HelperClasses.DbResponse response = nc.DbGetAllNewsEntries();

            List<NewsViewModels.NewsEntryViewModel> newsEntries = new List<NewsViewModels.NewsEntryViewModel>();

            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                foreach (NewsEntry entry in response.ResponseObjects)
                {
                    NewsViewModels.NewsEntryViewModel entryView = new NewsViewModels.NewsEntryViewModel
                    {
                        Id = entry.Id,
                        Title = entry.Title,
                        Text = entry.Text,
                        Date = entry.Date
                    };

                    HelperClasses.DbResponse imageResponse = rc.DbGetNewsEntryReferencedImages(entry.Id);

                    entryView.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));

                    newsEntries.Add(entryView);
                }
            }

            viewModel.ListEntries = newsEntries;

            return View("~/Views/AdminArea/AdminAreaNews/Index.cshtml", viewModel);
        }

        public ActionResult Create(NewsViewModels.NewsEntryViewModel viewModel = null)
        {
            if (viewModel == null || (viewModel.Date == DateTime.MinValue && viewModel.ValidFrom == DateTime.MinValue && viewModel.ValidTo == DateTime.MinValue))
            {
                viewModel = new NewsViewModels.NewsEntryViewModel();
                viewModel.Date = DateTime.Now;
                viewModel.ValidFrom = DateTime.Now;
                viewModel.ValidTo = DateTime.Now.AddYears(5);
            }
            return View("~/Views/AdminArea/AdminAreaNews/CreateNewsEntry.cshtml", viewModel);
        }

        // POST: news/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(NewsViewModels.NewsEntryViewModel viewModel, HttpPostedFileBase imageFile = null)
        {
            NewsController nc = new NewsController();
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            if (ModelState.IsValid && !string.IsNullOrEmpty(viewModel.Text))
            {
                NewsEntry newEntry = new NewsEntry
                {
                    Title = viewModel.Title,
                    Text = viewModel.Text,
                    ValidFrom = viewModel.ValidFrom,
                    ValidTo = viewModel.ValidTo,
                    CreatedBy = User.Identity.GetUserName(),
                    Date = DateTime.Now,
                    IsVisibleOnPage = viewModel.IsVisibleOnPage,
                    Deleted = false
                };

                HelperClasses.DbResponse response = nc.DbCreateNewsEntry(newEntry);

                if (response.Status == ModelEnums.ActionStatus.Success)
                {
                    statusMessage.Status = response.Status;
                    statusMessage.Messages.Add("Der Beitrag wurde erfolgreich erstellt. Sie können ihn jetzt bearbeiten");

                    if (imageFile != null)
                    {
                        bool isImageUploaded = UploadAndRegisterFile(imageFile, ((NewsEntry)response.ResponseObjects.FirstOrDefault()).Id, (int)ModelEnums.ReferenceToModelClass.NewsEntry, ModelEnums.FileReferenceType.NewsEntryImage, viewModel.Title);
                    }

                    TempData["statusMessage"] = statusMessage;
                    return RedirectToAction("Edit", new { id = ((NewsEntry)response.ResponseObjects.FirstOrDefault()).Id });
                }
                else
                {
                    statusMessage.Status = ModelEnums.ActionStatus.Error;
                    statusMessage.Messages.Add("Ein Fehler ist aufgetreten.");
                    viewModel.StatusMessage = statusMessage;
                }
            }
            statusMessage.Status = ModelEnums.ActionStatus.Error;
            statusMessage.Messages.Add("Bitte füllen Sie alle nötigen Angaben aus.");
            viewModel.StatusMessage = statusMessage;
            return View("~/Views/AdminArea/AdminAreaNews/CreateNewsEntry.cshtml", viewModel);

        }

        public ActionResult Edit(int? id)
        {
            NewsController nc = new NewsController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            if (id == null)
            {
                return HttpNotFound();
            }

            NewsViewModels.NewsEntryViewModel viewModel = new NewsViewModels.NewsEntryViewModel();
            viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;

            HelperClasses.DbResponse response = nc.DbGetNewsEntryById((int)id);

            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                NewsEntry entry = response.ResponseObjects.FirstOrDefault() as NewsEntry;
                viewModel.Id = entry.Id;
                viewModel.Title = entry.Title;
                viewModel.Text = entry.Text;
                viewModel.Date = entry.Date;
                viewModel.IsVisibleOnPage = entry.IsVisibleOnPage;
                viewModel.ValidFrom = entry.ValidFrom;
                viewModel.ValidTo = entry.ValidTo;

                HelperClasses.DbResponse imageResponse = rc.DbGetNewsEntryReferencedImages(entry.Id);

                viewModel.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
            }
            return View("~/Views/AdminArea/AdminAreaNews/EditNewsEntry.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(NewsViewModels.NewsEntryViewModel viewModel)
        {
            NewsController nc = new NewsController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            NewsEntry newData = new NewsEntry
            {
                Id = viewModel.Id,
                Date = viewModel.Date,
                IsVisibleOnPage = viewModel.IsVisibleOnPage,
                Text = viewModel.Text,
                Title = viewModel.Title,
                ValidFrom = viewModel.ValidFrom,
                ValidTo = viewModel.ValidTo,
                EditedBy = User.Identity.GetUserName()
            };

            HelperClasses.DbResponse response = nc.DbEditNewsEntry(newData);

            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            viewModel.StatusMessage = statusMessage;

            HelperClasses.DbResponse imageResponse = rc.DbGetNewsEntryReferencedImages(viewModel.Id);
            viewModel.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));

            return View("~/Views/AdminArea/AdminAreaNews/EditNewsEntry.cshtml", viewModel);
        }

        [ActionName("delete-news-entry")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult deleteNewsEntry(int entryId)
        {
            NewsController nc = new NewsController();
            HelperClasses.DbResponse response = nc.DbDeleteNewsEntry(entryId, User.Identity.GetUserName());
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Index");
        }

        //[ActionName("delete-news-entry-image")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult deleteNewsEntryImage(int imageRefId, int entryId)
        //{
        //    ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
        //    HelperClasses.DbResponse response = rc.DbDeleteFile(imageRefId, User.Identity.GetUserName());
        //    _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
        //    statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
        //    statusMessage.Status = response.Status;
        //    TempData["statusMessage"] = statusMessage;
        //    return RedirectToAction("Edit", new { id = entryId });
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-news-image")]
        public ActionResult UploadNewsImage(HttpPostedFileBase imageFile, int entryId, string imageTitle = null, string imageDescription = null)
        {
            bool isOk = UploadAndRegisterFile(imageFile, entryId, (int)ModelEnums.ReferenceToModelClass.NewsEntry, ModelEnums.FileReferenceType.NewsEntryImage, imageTitle, imageDescription);

            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();

            if (isOk)
            {
                statusMessage.Messages.Add("Das Bild wurde erfolgreich hochgeladen");
                statusMessage.Status = ModelEnums.ActionStatus.Success;
            }
            else
            {
                statusMessage.Messages.Add("Beim Hochgeladen ist ein Fehler aufgetreten");
                statusMessage.Status = ModelEnums.ActionStatus.Error;
            }

            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Edit", new { id = entryId });
        }
    }
}