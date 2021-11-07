using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web;
using static GardifyModels.Models.ModelEnums;

namespace GardifyWebAPI.Controllers
{
    [RoutePrefix("api/DeviceAPI")]
    public class DeviceAPIController : ApiController
    {
        
        private ApplicationDbContext db = new ApplicationDbContext();
        private DeviceController dc = new DeviceController();
        public Device dv = new Device();
        [HttpGet]
        [Route("AdminDevices")]
        public IEnumerable<AdminDeviceViewModel> GetAllDeviceFromAdmin()
        {
            return dc.Index();
        }
        [HttpGet]
        public IEnumerable<DeviceViewModel> GetDevices()
        {
            List<DeviceViewModel> res = new List<DeviceViewModel>();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            TodoController tc = new TodoController();
            var userId = Utilities.GetUserId();
            var userProperty = new PropertyController().DbGetProperty(userId);
            if (userProperty == null)
            {
                return res;
            }
            var userGardenId = userProperty.Gardens.ToList().First().Id;
            List<Device> dvs = this.db.Devices.Where(d => !d.Deleted && d.Gardenid==userGardenId).ToList();
            var todos = tc.GetTodos(Utilities.GetUserId(), ReferenceToModelClass.UserDevice);
            foreach (Device device in dvs)
            {
                DeviceViewModel vm = new DeviceViewModel()
                { Id = device.Id,
                    Name = device.Name,
                    Note = device.Note,
                    Date = device.Date,
                    isActive = device.isActive,
                    notifyForFrost = device.notifyForFrost,
                    notifyForWind = device.notifyForWind,
                    Gardenid = device.Gardenid,
                    AdminDevId=device.AdminDevId,
                    UserDevListId=device.UserDevListId,
                    Count=device.Count
                };
                if (todos != null && todos.Any())
                {
                    vm.Todos = todos.Where(r => r.ReferenceId == device.Id && r.ReferenceType == ReferenceToModelClass.UserDevice);
                }
                if (device.AdminDevId != null)
                {
                    HelperClasses.DbResponse imageResponse = rc.DbGetDeviceReferencedImages((int)vm.AdminDevId);
                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                    {
                        vm.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                    }
                    else
                    {

                        vm.EntryImages.Add(new _HtmlImageViewModel
                        {
                            SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                            Id = 0,
                            TitleAttr = "Kein Bild vorhanden"
                        });
                    }
                }
                else
                {
                    HelperClasses.DbResponse imageResponse = rc.DbGetUserDeviceReferencedImages((int)vm.Id);
                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                    {
                        vm.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                    }
                    else
                    {

                        vm.EntryImages.Add(new _HtmlImageViewModel
                        {
                            SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                            Id = 0,
                            TitleAttr = "Kein Bild vorhanden"
                        });
                    }
                }
                

                res.Add(vm);
            }
            return res;
        }


        [HttpGet]
        [Route("count")]

        public int GetDevicesCount()
        {
            List<DeviceViewModel> res = new List<DeviceViewModel>();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            TodoController tc = new TodoController();
            var userId = Utilities.GetUserId();
            var userProperty = new PropertyController().DbGetProperty(userId);
            if (userProperty == null)
            {
                return 0;
            }
            var userGardenId = userProperty.Gardens.ToList().First().Id;

            return this.db.Devices.Where(d => !d.Deleted && d.Gardenid == userGardenId).Count();
        }
        [HttpGet]
        [Route("{id}")]
        public DeviceViewModel GetDeviceById(int id)
        {
            Device d = this.db.Devices.Where(dv => !dv.Deleted && dv.Id == id).FirstOrDefault();
            return new DeviceViewModel()
            {
                Name = d.Name,
                Note = d.Note,
                Date = d.Date,
                isActive = d.isActive,
                notifyForFrost = d.notifyForFrost,
                notifyForWind = d.notifyForWind,
                Gardenid = d.Gardenid,
                AdminDevId=d.AdminDevId,
                UserDevListId = d.UserDevListId

            };
        }
        [HttpPost]
        [Route("postAdminDevice")]
        public IHttpActionResult PostAdminDevice(int []adminDeviceId)
        {
            var userId = Utilities.GetUserId();
            var userProperty = new PropertyController().DbGetProperty(userId);

            if (userProperty != null)
            {
                dc.AddAdminDeviceToUserDevicesList(adminDeviceId, userProperty);
            }

            return Ok(new { message = "success" });
        }
        [HttpPost]
        [Route("postDevice")]
        public IHttpActionResult PostDevice(Device device)
        {
        
            dv.Name = device.Name;
            dv.isActive = true;
            dv.Note = device.Note;
            dv.Date = device.Date;
            dv.notifyForFrost = device.notifyForFrost;
            dv.notifyForWind = device.notifyForWind;
            dv.Gardenid = device.Gardenid;
            dv.AdminDevId = device.AdminDevId;
            dv.OnCreate(Utilities.GetUserName());
            db.Devices.Add(dv);
            db.SaveChanges();
            
            return StatusCode(HttpStatusCode.Created);
        }
        [HttpPost]
        [Route("upload")]
        public IHttpActionResult UploadDeviceImage()
        {
            var lastDevice = (from t in db.Devices
                              where !t.Deleted
                              orderby t.CreatedDate descending
                              select t).First();
            if (HttpContext.Current.Request.Files[0] != null && lastDevice != null)
            {
                var imageFile = HttpContext.Current.Request.Files[0];
                var imageTitle = HttpContext.Current.Request.Params["imageTitle"];
                HttpPostedFileBase filebase = new HttpPostedFileWrapper(imageFile);
                dc.uploadUserDeviceImages(filebase, lastDevice.Id, imageTitle);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
        [HttpPut]
        [Route("update/{id}")]
        public IHttpActionResult UpdateDeviceById(int id, Device device)
        {
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            if (id != device.Id)
            {
                return BadRequest();
            }

            //db.Entry(device).State = EntityState.Modified;
            Device thisDevice = db.Devices.Find(id);
            thisDevice.Date = device.Date;
            thisDevice.notifyForFrost = device.notifyForFrost;
            thisDevice.notifyForWind = device.notifyForWind;
            thisDevice.Note = device.Note;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(device);
        }

        [HttpGet]
        [Route("flipNotification/{devId}/{forFrost}")]
        public IHttpActionResult UpdateDeviceNotification(int devId, bool forFrost)
        {
            Device dev = db.Devices.Find(devId);
            if (forFrost)
            {
                dev.notifyForFrost = !dev.notifyForFrost;
            } else
            {
                dev.notifyForWind = !dev.notifyForWind;
            }
            db.SaveChanges();
            return Ok();
        }

        [HttpPut]
        [Route("updateCount")]
        public IHttpActionResult UpdateDeviceCount(Device device)
        {
            Device existDevice = db.Devices.Find(device.Id);
            existDevice.Date = device.Date;
            existDevice.notifyForFrost = device.notifyForFrost;
            existDevice.notifyForWind = device.notifyForWind;
            existDevice.Note = device.Note;
            existDevice.Count = device.Count;
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DeviceExists(device.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return Ok(device);
        }
        [HttpDelete]
        [Route("{id}")]
        public IHttpActionResult DeleteDeviceById(int id)
        {
            Device device = db.Devices.Find(id);
            if (device == null)
            {
                return NotFound();
            }
            device.Deleted = true;
            db.SaveChanges();

            return Ok(device);
        }

        private bool DeviceExists(int id)
        {
            return db.Devices.Count(e => e.Id == id) > 0;
        }
    }
}
