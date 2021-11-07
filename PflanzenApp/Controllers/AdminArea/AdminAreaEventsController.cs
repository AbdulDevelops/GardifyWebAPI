using GardifyModels.Models;
using Microsoft.AspNet.Identity;
using PflanzenApp.App_Code;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PflanzenApp.Controllers.AdminArea
{
    [CustomAuthorizeAttribute(Roles = "Admin")]
    public class AdminAreaEventsController : _BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Events
        public ActionResult Index()
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            EventsListViewModel viewModel = new EventsListViewModel();

            var events = db.Events.Where(ev => !ev.Deleted);

            if (events != null && events.Any())
            {
                foreach (Event eve in events)
                {
                    EventViewModel eveView = new EventViewModel
                    {
                        Id = eve.Id,
                        Title = eve.Title,
                        Text = eve.Text,
                        Date = eve.Date,
                        Address = eve.Address,
                        Organizer = eve.Organizer
                    };

                    HelperClasses.DbResponse imageResponse = rc.DbGetEventReferencedImages(eve.Id);
                    eveView.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));

                    viewModel.Events.Add(eveView);
                }
            }

            return View("~/Views/AdminArea/AdminAreaEvents/Index.cshtml", viewModel);
        }

        public ActionResult Create()
        {
            return View("~/Views/AdminArea/AdminAreaEvents/Create.cshtml", new EventViewModel());
        }

        // POST: events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(EventViewModel viewModel)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(viewModel.Text))
            {
                Event newEntry = new Event
                {
                    Title = viewModel.Title,
                    Text = viewModel.Text,
                    Address = viewModel.Address,
                    Organizer = viewModel.Organizer,
                    ValidFrom = viewModel.ValidFrom,
                    ValidTo = viewModel.ValidTo,
                    CreatedBy = User.Identity.GetUserName(),
                    Date = viewModel.Date,
                    IsVisibleOnPage = viewModel.IsVisibleOnPage,
                    Deleted = false
                };

                newEntry.OnCreate(User.Identity.GetUserName());
                db.Events.Add(newEntry);
                db.SaveChanges();
                viewModel.Id = newEntry.Id;
            }

            return View("~/Views/AdminArea/AdminAreaEvents/Edit.cshtml", viewModel);
        }

        public ActionResult Edit(int? id)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            if (id == null)
            {
                return HttpNotFound();
            }

            EventViewModel viewModel = new EventViewModel();

            var eve = db.Events.Where(e => !e.Deleted && e.Id == id).FirstOrDefault();

            if (eve != null)
            {
                viewModel.Id = eve.Id;
                viewModel.Title = eve.Title;
                viewModel.Text = eve.Text;
                viewModel.Address = eve.Address;
                viewModel.Organizer = eve.Organizer;
                viewModel.Date = eve.Date;
                viewModel.IsVisibleOnPage = eve.IsVisibleOnPage;
                viewModel.ValidFrom = eve.ValidFrom;
                viewModel.ValidTo = eve.ValidTo;

                HelperClasses.DbResponse imageResponse = rc.DbGetEventReferencedImages(eve.Id);
                viewModel.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
            }
            return View("~/Views/AdminArea/AdminAreaEvents/Edit.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EventViewModel viewModel)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            var eve = db.Events.Where(e => !e.Deleted && e.Id == viewModel.Id).FirstOrDefault();

            eve.Id = viewModel.Id;
            eve.Date = viewModel.Date;
            eve.IsVisibleOnPage = viewModel.IsVisibleOnPage;
            eve.Text = viewModel.Text;
            eve.Organizer = viewModel.Organizer;
            eve.Address = viewModel.Address;
            eve.Title = viewModel.Title;
            eve.ValidFrom = viewModel.ValidFrom;
            eve.ValidTo = viewModel.ValidTo;
            eve.EditedBy = User.Identity.GetUserName();

            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            bool isOk = db.SaveChanges() > 0 ? true : false;
            if (isOk)
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Edited);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(eve);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                response.Status = ModelEnums.ActionStatus.Error;
            }
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            TempData["statusMessage"] = statusMessage;

            HelperClasses.DbResponse imageResponse = rc.DbGetEventReferencedImages(viewModel.Id);
            viewModel.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));

            return View("~/Views/AdminArea/AdminAreaEvents/Edit.cshtml", viewModel);
        }

        [ActionName("delete-event")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult deleteEvent(int entryId)
        {
            var eve = db.Events.Where(ev => !ev.Deleted && ev.Id == entryId).FirstOrDefault();
            eve.Deleted = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("edit-image-creds")]
        public ActionResult EditImageCreds(int imageId, int entryId, string imageAuthor = null, string imageLicense = null)
        {
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            try
            {
                nfilesEntities nfilesEntities = new nfilesEntities();
                var image = nfilesEntities.Files.FirstOrDefault(e => e.FileID == imageId);
                image.FileC = imageLicense;
                image.FileD = imageAuthor;
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
            return RedirectToAction("Edit", new { id = entryId });
        }

        [ActionName("delete-event-image")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult deleteEventImage(int imageRefId, int entryId)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            HelperClasses.DbResponse response = rc.DbDeleteFileReference(imageRefId, User.Identity.GetUserName());
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Edit", new { id = entryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-event-image")]
        public ActionResult UploadEventImage(HttpPostedFileBase imageFile, int entryId, string imageTitle = null, string imageDescription = null, string imageLicense = null, string imageAuthor = null)
        {
            bool isOk = UploadAndRegisterFile(imageFile, entryId, (int)ModelEnums.ReferenceToModelClass.Event, ModelEnums.FileReferenceType.EventImage, imageTitle, imageDescription, imageLicense, imageAuthor);

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