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

namespace GardifyWebAPI.Controllers
{
    [RoutePrefix("api/UserListAPI")]
    public class UserListController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/UserListAPI/
        [HttpGet]
        [Route("")]
        public IEnumerable<UserListViewModel> GetUserLists()
        {
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return new List<UserListViewModel>();
            }
            var userProperty = new PropertyController().DbGetProperty(userId);
            var ownGarden = userProperty.Gardens.Where(g => !g.Deleted).FirstOrDefault();

            if (userProperty == null || ownGarden == null)
            {
                return new List<UserListViewModel>();
            }
            List<UserListViewModel> res = new List<UserListViewModel>();
            IEnumerable<UserList> uLists = db.UserLists.Where(ul => ul.GardenId == ownGarden.Id && !ul.Deleted);

            foreach (UserList list in uLists) {
                UserListViewModel temp = new UserListViewModel();
                temp.Description = list.Description;
                temp.GardenId = list.GardenId;
                temp.Name = list.Name;
                temp.Id = list.Id;
                res.Add(temp);
            }
            return res;
        }

        [HttpGet]
        [Route("plantlists/{plantId}")]
        public IEnumerable<UserListViewModel> GetUserPlantLists(int plantId)
        {
            // gets userlists and preselects them only if a userlist has UserPlant with PlantId
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return new List<UserListViewModel>();
            }
            var userProperty = new PropertyController().DbGetProperty(userId);
            var ownGarden = userProperty.Gardens.Where(g => !g.Deleted).FirstOrDefault();

            if (userProperty == null || ownGarden == null)
            {
                return new List<UserListViewModel>();
            }

            // userplants with same PlantId in req
            var sameIds = db.UserPlants.Where(p => p.PlantId == plantId && p.Gardenid == ownGarden.Id && !p.Deleted).Select(p => p.Id);

            List<UserListViewModel> res = new List<UserListViewModel>();
            IEnumerable<UserList> uLists = db.UserLists.Where(ul => ul.GardenId == ownGarden.Id && !ul.Deleted);

            foreach (UserList list in uLists)
            {
                var plantsInList = db.UserPlantToUserLists.Where(utu => utu.UserListId == list.Id && !utu.Deleted).Select(utu => utu.PlantId);
                UserListViewModel temp = new UserListViewModel();
                temp.ListSelected = plantsInList.Intersect(sameIds).Count() > 0;
                temp.Description = list.Description;
                temp.GardenId = list.GardenId;
                temp.Name = list.Name;
                temp.Id = list.Id;
                res.Add(temp);
            }
            return res;
        }

        // GET: api/UserListAPI/3/15
        [HttpGet]
        [Route("{gardenId}/{id}")]
        public UserListViewModel GetUserList(int gardenId, int id)
        {
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return new UserListViewModel();
            }
            var userProperty = new PropertyController().DbGetProperty(userId);
            bool ownGarden = userProperty.Gardens.Any(g => g.Id == gardenId);

            if (userProperty == null || !ownGarden)
            {
                return new UserListViewModel();
            }
            
            UserList list = db.UserLists.Where(ul => ul.Id == id && !ul.Deleted).FirstOrDefault();

            UserListViewModel res = new UserListViewModel();
            res.Description = list.Description;
            res.GardenId = list.GardenId;
            res.Name = list.Name;
            res.Id = list.Id;
           
            return res;
        }

        [HttpGet]
        [Route("{id}")]
        public UserListViewModel GetUserListFromId(int id)
        {
            //var userId = Utilities.GetUserId();
            //if (userId == Guid.Empty)
            //{
            //    return new UserListViewModel();
            //}
            //var userProperty = new PropertyController().DbGetProperty(userId);
            //bool ownGarden = userProperty.Gardens.Any(g => g.Id == gardenId);

            //if (userProperty == null || !ownGarden)
            //{
            //    return new UserListViewModel();
            //}

            UserList list = db.UserLists.Where(ul => ul.Id == id && !ul.Deleted).FirstOrDefault();

            UserListViewModel res = new UserListViewModel();
            res.Description = list.Description;
            res.GardenId = list.GardenId;
            res.Name = list.Name;
            res.Id = list.Id;

            return res;
        }

        // POST: api/UserListAPI
        [HttpPost]
        [Route("create")]
        public IHttpActionResult CreateUserList(UserList userList, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            if (userList == null || String.IsNullOrEmpty(userList.Name))
            {
                return BadRequest(ModelState);
            }
            userList.GardenId = new GardenController().DbGetGardensByUserId(Utilities.GetUserId()).FirstOrDefault().Id;
            userList.OnCreate(Utilities.GetUserName());
            db.UserLists.Add(userList);
            db.SaveChanges();
            var currentStatistic = StatisticEventTypes.NewGarden;
            
           
                if (isIos)
                {
                    new StatisticsController().CreateEntry(currentStatistic, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromIos);
                }
                else if (isAndroid)
                {
                    new StatisticsController().CreateEntry(currentStatistic, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromAndroid);
                }
                else if (isWebPage)
                {
                    new StatisticsController().CreateEntry(currentStatistic, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage);
                }

           
            List<UserListViewModel> res = new List<UserListViewModel>();
            IEnumerable<UserList> uLists = db.UserLists.Where(ul => ul.GardenId == userList.GardenId && !ul.Deleted);

            foreach (UserList list in uLists)
            {
                UserListViewModel temp = new UserListViewModel();
                temp.Description = list.Description;
                temp.GardenId = list.GardenId;
                temp.Name = list.Name;
                temp.Id = list.Id;
                res.Add(temp);
            }

            return Ok(res);
        }

       

        // PUT: api/UserListAPI/5
        [HttpPut]
        [Route("updatelist")]
        public IHttpActionResult UpdateUserList( UserListViewModel userList)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            UserList update = db.UserLists.Where(ul => ul.Id == userList.Id).FirstOrDefault();
           
            if(update!=null)
            {
                update.Name = userList.Name;
                update.Description = userList.Description;
            }


            db.Entry(update).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!UserListExists(update.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return Ok(new { message = "success" });
        }

        // Delete: api/UserListAPI/3/15
        [HttpDelete]
        [Route("{gardenId}/{id}")]
        public IHttpActionResult DeleteUserList(int gardenId, int id)
        {
            var userId = Utilities.GetUserId();
            UserPlantController pc = new UserPlantController();

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

            var list = db.UserLists.Where(ul => ul.Id == id).FirstOrDefault();
            if (list == null)
            {
                return BadRequest("Das geht nicht.");
            }
            var plantInUserList = (from p in db.UserPlantToUserLists
                                   where p.UserListId == list.Id
                                   select p);
            foreach (var p in plantInUserList)
            {
                p.Deleted = true;
                p.UserPlant.Deleted = true;
            }
         
            list.Deleted = true;
            db.SaveChanges();

            var cacheUniqueId = userId.ToString() + "_mygarden";
            if (userId == Guid.Empty)
            {
                return Ok();
            }

            string cachedFilePath = HttpContext.Current.Server.MapPath("~") + "\\gardenCache\\" + cacheUniqueId + ".txt";
            var cachedData = pc.GetCachedIndex(cachedFilePath);

            if (cachedData == null)
            {
                return Ok();

            }

            pc.DeleteIndexCache(cachedFilePath);


            return Ok();
        }

        private bool UserListExists(int id)
        {
            return db.UserLists.Count(e => e.Id == id) > 0;
        }
        public UserList getUserListById(int id)
        {
            var userList = (from list in db.UserLists
                            where list.Id == id
                            && !list.Deleted
                            select list).FirstOrDefault();
            return userList;
        }
       
    }
}
