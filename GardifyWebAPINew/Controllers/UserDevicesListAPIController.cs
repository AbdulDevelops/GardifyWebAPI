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

namespace GardifyWebAPI.Controllers
{
    [RoutePrefix("api/UserDevicesListAPI")]
    public class UserDevicesListAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        [HttpGet]
        [Route("{gardenId}")]
        // GET: api/UserDevicesListAPI
        public IEnumerable<UserDevicesListViewModel> GetUserDevicesLists(int gardenId)
        {
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return new List<UserDevicesListViewModel>();
            }
            var userProperty = new PropertyController().DbGetProperty(userId);
            bool ownGarden = userProperty.Gardens.Any(g => g.Id == gardenId);

            if (userProperty == null || !ownGarden)
            {
                return new List<UserDevicesListViewModel>();
            }
            List<UserDevicesListViewModel> res = new List<UserDevicesListViewModel>();
            IEnumerable<UserDevicesList> uDevicesLists = db.UserDevicesLists.Where(ul => ul.GardenId == gardenId && !ul.Deleted);
            foreach (UserDevicesList list in uDevicesLists)
            {
                UserDevicesListViewModel vm = new UserDevicesListViewModel();
                vm.Description = list.Description;
                vm.GardenId = list.GardenId;
                vm.Name = list.Name;
                vm.Id = list.Id;
                res.Add(vm);
            }
            return res;
        }
        [HttpGet]
        [Route("{gardenId}/{id}")]
        // GET: api/UserDevicesListAPI/5
        public UserDevicesListViewModel GetUserDeviceList(int gardenId, int id)
        {
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return new UserDevicesListViewModel();
            }
            var userProperty = new PropertyController().DbGetProperty(userId);
            bool ownGarden = userProperty.Gardens.Any(g => g.Id == gardenId);

            if (userProperty == null || !ownGarden)
            {
                return new UserDevicesListViewModel();
            }

            UserDevicesList list = db.UserDevicesLists.Where(ul => ul.Id == id && !ul.Deleted).FirstOrDefault();

            UserDevicesListViewModel res = new UserDevicesListViewModel();
            res.Description = list.Description;
            res.GardenId = list.GardenId;
            res.Name = list.Name;
            res.Id = list.Id;
            res.UserDevices = GetUserDevicesList(list.Id, list.GardenId);

            return res;
        }

        // POST: api/UserDevicesListAPI
        [HttpPost]
        [Route("create")]
        public IHttpActionResult CreateUserDevList(UserDevicesList userDevList)
        {
            if (userDevList == null || userDevList.GardenId <= 0 || String.IsNullOrEmpty(userDevList.Name))
            {
                return BadRequest(ModelState);
            }
            userDevList.OnCreate(Utilities.GetUserName());
            db.UserDevicesLists.Add(userDevList);
            db.SaveChanges();

            List<UserDevicesListViewModel> res = new List<UserDevicesListViewModel>();
            IEnumerable<UserDevicesList> uDevLists = db.UserDevicesLists.Where(ul => ul.GardenId == userDevList.GardenId && !ul.Deleted);

            foreach (UserDevicesList list in uDevLists)
            {
                UserDevicesListViewModel temp = new UserDevicesListViewModel();
                temp.Description = list.Description;
                temp.GardenId = list.GardenId;
                temp.Name = list.Name;
                temp.Id = list.Id;
                res.Add(temp);
            }

            return Ok(res);
        }
        // PUT: api/UserDevicesListAPI/5
        [HttpPut]
        [Route("update/{listId}")]
        public IHttpActionResult UpdateDeviceInUserList(int listId, Device userDev, bool shouldRemove)
        {
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }
            var userProperty = new PropertyController().DbGetProperty(userId);
            bool ownGarden = userProperty.Gardens.Any(g => g.Id == userDev.Gardenid);

            if (userProperty == null || !ownGarden)
            {
                return BadRequest("Das geht nicht.");
            }

            var plant = db.Devices.Where(up => up.Id == userDev.Id).FirstOrDefault();

            if (shouldRemove)
            {
                plant.UserDevListId = 0;
            }
            else
            {
                plant.UserDevListId = listId;
            }

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserDevListExists(listId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/UserDevicesListAPI/5
      
         [HttpPut]
        [Route("{id}")]
        public IHttpActionResult UpdateUserList(int id, UserDevicesList userDevList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != userDevList.Id)
            {
                return BadRequest();
            }

            db.Entry(userDevList).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserDevListExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // DELETE: api/UserDevicesListAPI/5
        [HttpDelete]
        [Route("{gardenId}/{id}")]
        public IHttpActionResult DeleteUserList(int gardenId, int id)
        {
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return Unauthorized();
            }
            var userProperty = new PropertyController().DbGetProperty(userId);
            bool ownGarden = userProperty.Gardens.Any(g => g.Id == gardenId);

            if (userProperty == null || !ownGarden)
            {
                return BadRequest("Das geht nicht.");
            }

            var list = db.UserDevicesLists.Where(ul => ul.Id == id).FirstOrDefault();
            if (list == null)
            {
                return BadRequest("Das geht nicht.");
            }

            // Delist plants in the list
            var listDevices = db.Devices.Where(up => up.UserDevListId == list.Id);
            foreach (Device dev in listDevices)
            {
                dev.UserDevListId = 0;
            }
            list.Deleted = true;
            db.SaveChanges();

            return Ok();
        }
            private bool UserDevListExists(int id)
        {
            return db.UserDevicesLists.Count(e => e.Id == id) > 0;
        }
        private IEnumerable<Device> GetUserDevicesList(int userListId, int gardenId)
        {
            var devices= db.Devices.Where(up => up.UserDevListId == userListId && !up.Deleted && up.Gardenid == gardenId);
            return devices;
        }
    }
}
