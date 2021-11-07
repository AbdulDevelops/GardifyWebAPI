using GardifyModels.Models;
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
    public class AdminAreaEcoElementsController : _BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ReferenceToFileSystemObjectController or = new ReferenceToFileSystemObjectController();
        // Views
        public ActionResult Index()
        {
            
            IEnumerable<EcoElement> ecolist = db.EcoElements.ToList();
            List<AdminEcoElementViewModel> ecolistView = new List<AdminEcoElementViewModel>();
            foreach (var e in ecolist)
            {
               AdminEcoElementViewModel vm = new AdminEcoElementViewModel
                {
                    Id =e.Id,
                    Name=e.Name,
                    Description=e.Description
                };
                HelperClasses.DbResponse imageResponse = or.DbGetEcoElementReferencedImages(e.Id);

                vm.EcoElementsImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));

               ecolistView.Add(vm);
            }
            return View("~/Views/AdminArea/AdminAreaEcoElements/Index.cshtml", ecolistView.ToList());
        }

        public ActionResult Create()
        {
            return View("~/Views/AdminArea/AdminAreaEcoElements/Create.cshtml", new EcoElement());
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EcoElement ee = db.EcoElements.Find(id);
            if (ee == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/AdminArea/AdminAreaEcoElements/Edit.cshtml", ee);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            EcoElement ee = db.EcoElements.Find(id);
            if (ee == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/AdminArea/AdminAreaEcoElements/Delete.cshtml", ee);
        }

        //Actions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Name,Description")] EcoElement ee)
        {
            if (!String.IsNullOrEmpty(ee.Name))
            {
                ee.OnCreate(Utilities.GetUserName());
                db.EcoElements.Add(ee);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View("~/Views/AdminArea/AdminAreaEcoElements/Create.cshtml", ee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Description")] EcoElement ee)
        {
            if (!String.IsNullOrEmpty(ee.Name))
            {
                var art = db.EcoElements.Where(a => a.Id == ee.Id && !a.Deleted).FirstOrDefault();
                if (art != null)
                {
                    art.Name = ee.Name;
                    art.Description = ee.Description;
                }
                art.OnEdit(Utilities.GetUserName());

                db.SaveChanges();
                
                return RedirectToAction("Index");
            }
            return View("~/Views/AdminArea/AdminAreaEcoElements/Edit.cshtml", ee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            EcoElement ee = db.EcoElements.Find(id);
            db.EcoElements.Remove(ee);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-EcoElement-image")]
        public ActionResult UploadEcoElImage(HttpPostedFileBase imageFile, int objectId, string imageTitle = null, string imageDescription = null)
        {
            bool isOk = UploadAndRegisterFile(imageFile, objectId, (int)ModelEnums.ReferenceToModelClass.EcoElement, ModelEnums.FileReferenceType.EcoElementImages, imageTitle, imageDescription);

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
            return RedirectToAction("Edit", new { id = objectId });
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}