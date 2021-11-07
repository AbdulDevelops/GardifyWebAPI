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

namespace PflanzenApp.Controllers.AdminArea
{
    public class AdminAreaDevicesController : _BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ReferenceToFileSystemObjectController or = new ReferenceToFileSystemObjectController();
        // GET: AdminAreaDevices
        public ActionResult Index()
        {
            AdminDeviceListViewModel viewModel = new AdminDeviceListViewModel();
            HelperClasses.DbResponse response = DbGetAllDevices();
            viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;
            List<AdminDeviceViewModel> devicesList = new List<AdminDeviceViewModel>();
            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {

                foreach (AdminDevice entry in response.ResponseObjects)
                {
                    AdminDeviceViewModel entryView = new AdminDeviceViewModel
                    {
                        Id = entry.Id,
                        Name = entry.Name,
                        isActive=entry.isActive,
                        notifyForFrost=entry.notifyForFrost,
                        notifyForWind=entry.notifyForWind,
                        Note=entry.Note,
                    };
                    HelperClasses.DbResponse imageResponse = or.DbGetDeviceReferencedImages(entry.Id);

                    entryView.DevicesImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));

                    devicesList.Add(entryView);
                }
               
            }
            viewModel.DevicesListEntries = devicesList;
            return View("~/Views/AdminArea/AdminAreaDevices/Index.cshtml", viewModel);
        }

        // GET: AdminAreaDevices/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminDevice adminDevice = db.AdminDevices.Find(id);
            if (adminDevice == null)
            {
                return HttpNotFound();
            }
            return View(adminDevice);
        }

        // GET: AdminAreaDevices/Create
        public ActionResult Create(AdminDeviceViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new AdminDeviceViewModel();
            }

            return View("~/Views/AdminArea/AdminAreaDevices/Create.cshtml", viewModel);
        }

        // POST: AdminAreaDevices/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( AdminDeviceViewModel adminDeviceView, HttpPostedFileBase imageFile = null)
        {
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            if (ModelState.IsValid )
            {
                AdminDevice device = new AdminDevice
                {
                    Note = adminDeviceView.Note,
                    isActive=adminDeviceView.isActive,
                    notifyForFrost = adminDeviceView.notifyForFrost,
                    notifyForWind = adminDeviceView.notifyForWind,
                    Name = adminDeviceView.Name,
                    CreatedBy = User.Identity.GetUserName(),
                    Date=DateTime.Now,
                    Deleted =false
                };
                HelperClasses.DbResponse response = DbCreateDevice(device);
                if (response.Status == ModelEnums.ActionStatus.Success)
                {
                    statusMessage.Status = response.Status;
                    statusMessage.Messages.Add("Der Artikel wurde erfolgreich erstellt. Sie können ihn jetzt bearbeiten");

                    if (imageFile != null)
                    {
                        bool isImageUploaded = UploadAndRegisterFile(imageFile,
                                                                        ((AdminDevice)response.ResponseObjects.FirstOrDefault()).Id,
                                                                         (int)ModelEnums.ReferenceToModelClass.AdminDevice,
                                                                         ModelEnums.FileReferenceType.AdminDeviceImg, 
                                                                         adminDeviceView.Name
                                                                         );
                    }

                    TempData["statusMessage"] = statusMessage;
                    return RedirectToAction("Edit", new { id = ((AdminDevice)response.ResponseObjects.FirstOrDefault()).Id });
                }
                else
                {
                    statusMessage.Status = ModelEnums.ActionStatus.Error;
                    statusMessage.Messages.Add("Ein Fehler ist aufgetreten.");
                }
            }
            else
            {
                statusMessage.Status = ModelEnums.ActionStatus.Error;
                statusMessage.Messages.Add("Bitte geben Sie alle erforderlichen Eingaben an.");
            }
            
            adminDeviceView.StatusMessage = statusMessage;
            return View("~/Views/AdminArea/AdminAreaDevices/Create.cshtml", adminDeviceView);
        }

        // GET: AdminAreaDevices/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminDeviceViewModel viewModel = new AdminDeviceViewModel();
            viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;
            HelperClasses.DbResponse response = DbGetDevicesById((int)id);
            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                AdminDevice device = response.ResponseObjects.FirstOrDefault() as AdminDevice;
                viewModel.Id = device.Id;
                viewModel.Name = device.Name;
                viewModel.notifyForFrost = device.notifyForFrost;
                viewModel.notifyForWind = device.notifyForWind;
                viewModel.Note = device.Note;
                viewModel.isActive = device.isActive;
                HelperClasses.DbResponse imageResponse = or.DbGetDeviceReferencedImages(device.Id);
                viewModel.DevicesImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
            }
            else
            {
                return HttpNotFound();
            }
            return View("~/Views/AdminArea/AdminAreaDevices/Edit.cshtml", viewModel);
        }

        // POST: AdminAreaDevices/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(AdminDeviceViewModel adminDeviceViewModel)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            if (ModelState.IsValid)
            {
                var selectedDevice = (from d in db.AdminDevices where !d.Deleted && d.Id == adminDeviceViewModel.Id select d).FirstOrDefault();
                if (selectedDevice != null)
                {
                    selectedDevice.Id = adminDeviceViewModel.Id;
                    selectedDevice.Name = adminDeviceViewModel.Name;
                    selectedDevice.notifyForFrost = adminDeviceViewModel.notifyForFrost;
                    selectedDevice.notifyForWind = adminDeviceViewModel.notifyForWind;
                    selectedDevice.Note = adminDeviceViewModel.Note;
                    selectedDevice.isActive = adminDeviceViewModel.isActive;
                    selectedDevice.EditedBy = User.Identity.GetUserName();

                    db.Entry(selectedDevice).State = EntityState.Modified;
                    bool isOk = db.SaveChanges() > 0 ? true : false;
                    if (isOk)
                    {
                        response.Messages.Add(ModelEnums.DatabaseMessage.Edited);
                        response.Status = ModelEnums.ActionStatus.Success;
                        response.ResponseObjects.Add(selectedDevice);
                    }
                    else
                    {
                        response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                        response.Status = ModelEnums.ActionStatus.Error;
                    }
                }
                else
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                    response.Status = ModelEnums.ActionStatus.Error;
                }
                _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
                statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
                statusMessage.Status = response.Status;
                adminDeviceViewModel.StatusMessage = statusMessage;
                HelperClasses.DbResponse imageResponse = or.DbGetDeviceReferencedImages(adminDeviceViewModel.Id);
                adminDeviceViewModel.DevicesImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
            }
            return View("~/Views/AdminArea/AdminAreaDevices/Edit.cshtml", adminDeviceViewModel);
        }


        [ActionName("delete-device-image")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult deleteDeviceImage(int imageId, int objectId)
        {
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            try
            {
                nfilesEntities nfilesEntities = new nfilesEntities();
                var image = nfilesEntities.Files.FirstOrDefault(e => e.FileID == imageId);
                var ftm = nfilesEntities.FileToModule.FirstOrDefault(e => e.FileID == imageId);
                nfilesEntities.FileToModule.Remove(ftm);
                nfilesEntities.Files.Remove(image);
                nfilesEntities.SaveChanges();
                statusMessage.Messages = new string[] { "Bild gelöscht" }.ToList();
                statusMessage.Status = ModelEnums.ActionStatus.Success;
            }
            catch
            {
                statusMessage.Messages = new string[] { "Fehler beim löschen" }.ToList();
                statusMessage.Status = ModelEnums.ActionStatus.Error;
            }
            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Edit", new { id = objectId });
           
        }

        [ActionName("delete-device")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult deleteDevice(int id, string deleteBy)
        {
            HelperClasses.DbResponse response = DbDeleteDevice(id, User.Identity.GetUserName());
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-device-image")]
        public ActionResult UploadDeviceImage(HttpPostedFileBase imageFile, int objectId, string imageTitle = null, string imageDescription = null)
        {
            bool isOk = UploadAndRegisterFile(imageFile, objectId, (int)ModelEnums.ReferenceToModelClass.AdminDevice, ModelEnums.FileReferenceType.AdminDeviceImg, imageTitle, imageDescription);

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
        #region DB

        [NonAction]
        public HelperClasses.DbResponse DbDeleteDevice(int deviceId,string deletedBy)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            var device = (from d in db.AdminDevices where d.Id == deviceId && !d.Deleted select d).FirstOrDefault();
            if (device != null)
            {
                device.Deleted = true;
                
               device.OnEdit(deletedBy);

                db.Entry(device).State = EntityState.Modified;
                foreach(Device d in db.Devices.Where(de => de.AdminDevId == device.Id))
                {
                    d.Deleted = true;
                }
                bool isOk = db.SaveChanges() > 0 ? true : false;

                if (isOk)
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.Deleted);
                    response.Status = ModelEnums.ActionStatus.Success;
                    response.ResponseObjects.Add(device);
                }
                else
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                    response.Status = ModelEnums.ActionStatus.Error;
                }
            }
            return response;
        }
        [NonAction]
        public HelperClasses.DbResponse DbGetDevicesById(int deviceId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            var device = from d in db.AdminDevices where !d.Deleted && d.Id == deviceId select d;
            if(device !=null && device.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(device.FirstOrDefault());
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
            }
            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbCreateDevice(AdminDevice deviceData)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            deviceData.OnCreate(deviceData.CreatedBy);

            ctx.AdminDevices.Add(deviceData);

            bool isOk = ctx.SaveChanges() > 0 ? true : false;

            if (isOk)
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Created);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(deviceData);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }
        [NonAction]
        public HelperClasses.DbResponse DbGetAllDevices()
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var devices = from dev in db.AdminDevices where dev.Deleted == false select dev;

            if (devices != null && devices.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.AddRange(devices);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }
        #endregion
    }
}
