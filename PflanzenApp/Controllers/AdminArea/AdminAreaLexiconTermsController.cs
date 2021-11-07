using GardifyModels.Models;
using Microsoft.AspNet.Identity;
using PflanzenApp.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace PflanzenApp.Controllers.AdminArea
{
    [CustomAuthorizeAttribute(Roles = "Admin,Expert")]
    public class AdminAreaLexiconTermsController : _BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AdminAreaLexiconTerms
        public ActionResult Index()
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            var terms = db.LexiconTerms.ToList();
            var res = new List<LexiconTermVM>();

            foreach (LexiconTerm entry in terms)
            {
                LexiconTermVM entryView = new LexiconTermVM
                {
                    Id = entry.Id,
                    Name = entry.Name,
                    Description = entry.Description
                };

                res.Add(entryView);
            }
            return View("~/Views/AdminArea/AdminAreaLexiconTerms/Index.cshtml", res);
        }

        // GET: AdminAreaLexiconTerms/Create
        [HttpGet]
        public ActionResult Create()
        {
            return View("~/Views/AdminArea/AdminAreaLexiconTerms/Create.cshtml", new LexiconTermVM());
        }

        // POST: AdminAreaLexiconTerms/Create
        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Create(LexiconTermVM viewModel, HttpPostedFileBase imageFile = null)
        {
            if (!String.IsNullOrEmpty(viewModel.Name) && !String.IsNullOrEmpty(viewModel.Description))
            {
                var term = new LexiconTerm()
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description
                };
                term.OnCreate(Utilities.GetUserName());
                db.LexiconTerms.Add(term);
                var res = db.SaveChanges();

                if (imageFile != null && res > 0)
                {
                    bool isImageUploaded = UploadAndRegisterFile(imageFile, term.Id, (int)ModelEnums.ReferenceToModelClass.LexiconTerm, ModelEnums.FileReferenceType.LexiconTermImage, viewModel.Name);
                }
                viewModel.Id = term.Id;
                return View("~/Views/AdminArea/AdminAreaLexiconTerms/Edit.cshtml", viewModel);
            }

            return View("~/Views/AdminArea/AdminAreaLexiconTerms/Create.cshtml", viewModel);
        }

        // GET: AdminAreaLexiconTerms/Edit/5
        public ActionResult Edit(int? id)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LexiconTerm term = db.LexiconTerms.Find(id);
            if (term == null)
            {
                return HttpNotFound();
            }
            var viewModel = new LexiconTermVM()
            {
                Id = term.Id,
                Name = term.Name,
                Description = term.Description
            };
            HelperClasses.DbResponse imageResponse = rc.DbGetLexiconTermReferencedImages(term.Id);

            viewModel.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
            return View("~/Views/AdminArea/AdminAreaLexiconTerms/Edit.cshtml", viewModel);
        }

        // POST: AdminAreaLexiconTerms/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(LexiconTermVM viewModel)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            if (!String.IsNullOrEmpty(viewModel.Name) && !String.IsNullOrEmpty(viewModel.Description))
            {
                var art = db.LexiconTerms.Where(a => a.Id == viewModel.Id && !a.Deleted).FirstOrDefault();
                if (art != null)
                {
                    art.Name = viewModel.Name;
                    art.Description = viewModel.Description;
                }
                art.OnEdit(Utilities.GetUserName());
                viewModel.Id = art.Id;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            HelperClasses.DbResponse imageResponse = rc.DbGetLexiconTermReferencedImages(viewModel.Id);
            viewModel.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
            return View("~/Views/AdminArea/AdminAreaLexiconTerms/Edit.cshtml", viewModel);
        }

        // GET: AdminAreaLexiconTerms/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            LexiconTerm term = db.LexiconTerms.Find(id);
            if (term == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/AdminArea/AdminAreaLexiconTerms/Delete.cshtml", term);
        }

        // POST: AdminAreaLexiconTerms/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            LexiconTerm term = db.LexiconTerms.Find(id);
            db.LexiconTerms.Remove(term);
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

        [ActionName("delete-lexicon-image")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteLexiconTermImage(int imageRefId, int entryId)
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
        [ActionName("upload-lexicon-image")]
        public ActionResult UploadLexiconTermImage(HttpPostedFileBase imageFile, int entryId, string imageTitle = null, string imageDescription = null, string imageLicense = null, string imageAuthor = null)
        {
            bool isOk = UploadAndRegisterFile(imageFile, entryId, (int)ModelEnums.ReferenceToModelClass.LexiconTerm, ModelEnums.FileReferenceType.LexiconTermImage, imageTitle, imageDescription, imageLicense, imageAuthor);

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
