using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GardifyWebAPI.Controllers
{
    public class DeviceController : _BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: Device
        public IEnumerable<AdminDeviceViewModel> Index()
        {
            HelperClasses.DbResponse response = dbgetAllDevices();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            List<AdminDeviceViewModel> devicesList = new List<AdminDeviceViewModel>();
            var deviceInRes = response.ResponseObjects;
            if(deviceInRes!=null && deviceInRes.Any())
            {
                foreach (AdminDevice d in deviceInRes)
                {
                    AdminDeviceViewModel viewModel = new AdminDeviceViewModel()
                    {
                        Id=d.Id,
                        Note = d.Note,
                        notifyForWind= d.notifyForWind,
                        notifyForFrost=d.notifyForFrost,
                        Name= d.Name,
                        isActive=d.isActive
                    };

                    HelperClasses.DbResponse imageResponse = rc.DbGetDeviceReferencedImages(d.Id);
                    viewModel.DevicesImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                    devicesList.Add(viewModel);
                }
            }
            return devicesList;
        }
        public void AddAdminDeviceToUserDevicesList(int[] adminDevicesId,Property userProperty)
        {
            foreach (var i in adminDevicesId)
            {
                var deviceToAdd = (from e in Index().ToList() where e.Id == i select e).First();
               
                if(deviceToAdd != null && userProperty != null)
                {
                    var userProps = userProperty.Gardens.First().Id;
                    Device existDevice = (from d in db.Devices where d.AdminDevId == deviceToAdd.Id && d.Gardenid == userProps && !d.Deleted select d).FirstOrDefault();
                    if (existDevice != null)
                    {
                        existDevice.Count += 1;
                    }
                    else
                    {
                        Device newdevice = new Device()
                        {
                            AdminDevId = deviceToAdd.Id,
                            Name = deviceToAdd.Name,
                            notifyForFrost = deviceToAdd.notifyForFrost,
                            notifyForWind = deviceToAdd.notifyForWind,
                            isActive = deviceToAdd.isActive,
                            Note = deviceToAdd.Note,
                            Date = DateTime.Now
                        };
                        newdevice.Gardenid = userProps;
                        newdevice.OnCreate(Utilities.GetUserName());
                        db.Devices.Add(newdevice);

                    }

                    db.SaveChanges();
                }
                
            }
        }
        public ActionResult uploadUserDeviceImages(HttpPostedFileBase imageFile, int deviceId, string imageTitle = null, string imageDescription = null)
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
                UploadAndRegisterFile(imageFile, deviceId, (int)ModelEnums.ReferenceToModelClass.UserDevice, ModelEnums.FileReferenceType.AdminDeviceImg, imageTitle, imageDescription);
            }

            TempData["statusMessage"] = statusMessage;

            return RedirectToAction("EditEntry", new { id = deviceId});
            
        }
        #region DB 
       [NonAction]
       public HelperClasses.DbResponse dbgetAllDevices()
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            var devices = from d in db.AdminDevices where !d.Deleted select d;
            if(devices!=null && devices.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.AddRange(devices);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.EmptyResult);
                response.Status = ModelEnums.ActionStatus.Error;
            }
            return response;
        }
        #endregion
    }

}