using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using static GardifyModels.Models.ModelEnums;
using static GardifyModels.Models.UserPlantViewModels;
//using MoreLinq;
using System.Threading.Tasks;
using System.Runtime.Caching;
using System.IO;
using System.Text.Json;
using Microsoft.Ajax.Utilities;
//using MoreLinq.Extensions;

namespace GardifyWebAPI.Controllers
{

    public class UserPlantDetailsListObject
    {
        public IEnumerable<UserPlantDetailsViewModelCount> userPlants { get; set; }
    }

    [RoutePrefix("api/UserPlantsAPI")]
    public class UserPlantsAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/UserPlantsAPI
        [HttpGet]
        public IEnumerable<UserPlantDetailsViewModelCount> GetUserPlants(int skip = 0, int take = -1)
        {

            UserPlantController pc = new UserPlantController();
            ObjectCache cache = MemoryCache.Default;
            var userId = Utilities.GetUserId();
            var cacheUniqueId = userId.ToString() + "_mygarden";

            if (userId == Guid.Empty)
            {
                return new List<UserPlantDetailsViewModelCount>();
            }

            //if (skip == 0 && take == -1)
            //{
            string cachedFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/gardenCache/") + cacheUniqueId + ".txt";

            try
            {
                var result = pc.GetCachedIndex(cachedFilePath);
                if (result == null || !result.Any())
                {
                    var outputTemp = pc.Index(false, skip: 0, take: -1);
                    pc.UpdateIndexCache(outputTemp, cachedFilePath);

                    result = outputTemp;
                }
                if (skip == 0 && take == -1)
                {
                    return result;

                }
                else
                {
                    return result.Skip(skip).Take(take);

                }
            }
            catch (Exception e)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    Content = new StringContent(e.Message),
                    ReasonPhrase = "Failed to save cache"
                };
                throw new HttpResponseException(resp);
            }




            //}
            var output = pc.Index(false, skip: skip, take: take);


            return output;
        }

        [HttpGet]
        [Route("gardennote")]

        public IEnumerable<GardenNote> GetGardenNote()
        {

            var noteModel = db.GardenNotes.Where(n => !n.Deleted).ToList();

            return noteModel;

            //try
            //{
            //    var noteModel = db.GardenNotes.Where(n => !n.Deleted).ToList(); 

            //    var result = pc.GetCachedIndex(cachedFilePath);
            //    if (result == null)
            //    {
            //        var outputTemp = pc.Index(false, skip: 0, take: -1);
            //        pc.UpdateIndexCache(outputTemp, cachedFilePath);

            //        result = outputTemp;
            //    }

            //    return Ok("success in creating");
            //}
            //catch (Exception e)
            //{

            //    return BadRequest(e.ToString());


            //}



        }

        [HttpGet]
        [Route("cache")]

        public IHttpActionResult GetUserPlantsCache()
        {

            UserPlantController pc = new UserPlantController();
            var userId = Utilities.GetUserId();
            var cacheUniqueId = userId.ToString() + "_mygarden";

            if (userId == Guid.Empty)
            {
                return BadRequest("empty user Id");
            }
        

                try
                {
                string cachedFilePath = System.Web.Hosting.HostingEnvironment.MapPath("~/gardenCache/") + cacheUniqueId + ".txt";


                var result = pc.GetCachedIndex(cachedFilePath);
                    if (result == null)
                    {
                        var outputTemp = pc.Index(false, skip: 0, take: -1);
                        pc.UpdateIndexCache(outputTemp, cachedFilePath);

                        result = outputTemp;
                    }

                    return Ok("success in creating");
                }
                catch (Exception e)
                {

                    return BadRequest(e.ToString());

           
                }



        }
        [HttpPost]
        [Route("movePlant")]
        public IEnumerable<UserPlantDetailsViewModelCount> MoveUserplantToAnotherList(MoveUserplantToAnotherListVM model)
        {
            UserPlantController pc = new UserPlantController();
            UserListController uc = new UserListController();
           pc.MovePlantToUserList(model);
            var selectedGarden = uc.GetUserListFromId(model.NewListId);

            //var outputTemp = pc.Index(false);

            var userId = Utilities.GetUserId();
            var cacheUniqueId = userId.ToString() + "_mygarden";
            if (userId == Guid.Empty)
            {
                return pc.Index(false);
            }
            if (selectedGarden == null)
            {
                return pc.Index(false);

            }
            string cachedFilePath = HttpContext.Current.Server.MapPath("~") + "\\gardenCache\\" + cacheUniqueId + ".txt";


            //var cachedData = pc.GetCachedIndex(cachedFilePath);
            //if (cachedData == null)
            //{
            //    return pc.Index(false);
            //}
            //var filterCache = cachedData.Where(c => c.UserPlant.Id != model.UserplantId).ToList();

            //var changedUserList = cachedData.FirstOrDefault(c => c.UserPlant.Id == model.UserplantId);

            //List<int> listId = new List<int> { selectedGarden.Id };

            //changedUserList.ListIds = new List<int> { selectedGarden.Id};
            //changedUserList.ListNames = new List<string> { selectedGarden.Name };

            ////filterCache.Append(changedUserList);
            //filterCache.Add(changedUserList);

            //foreach (var item in changedUserList)
            //{
            //    item.
            //}

            //filterCache.Append()

            pc.DeleteIndexCache(cachedFilePath);


            return pc.Index(false);
        }
        [HttpPost]
        [Route("moveAllPlants")]
        public IEnumerable<UserPlantDetailsViewModelCount> MoveAllUserplantsToAnotherList(MoveAllUserplantsToAnotherListVM model)
        {
            UserPlantController pc = new UserPlantController();
            pc.MoveAllPlantsToUserList(model);
            var outputTemp = pc.Index(false);

            var userId = Utilities.GetUserId();
            var cacheUniqueId = userId.ToString() + "_mygarden";
            if (userId == Guid.Empty)
            {
                return outputTemp;
            }
            string cachedFilePath = HttpContext.Current.Server.MapPath("~") + "\\gardenCache\\" + cacheUniqueId + ".txt";
            pc.UpdateIndexCache(outputTemp, cachedFilePath);

            return outputTemp;
        }
        // GET: api/UserPlantsAPI/5
        [HttpGet]
        [Route("filterPlant")]
        public IEnumerable<UserPlantDetailsViewModelCount> filterUserPlant(string ecotags = "", string freezes = "",string toxic="")
        {
            UserPlantController pc = new UserPlantController();
            if (String.IsNullOrEmpty(ecotags) && String.IsNullOrEmpty(freezes) && String.IsNullOrEmpty(toxic))
            {
                return pc.Index(false);
            }
            return pc.userPlantfilter(ecotags, freezes,toxic);
        }

        [HttpGet]
        [Route("ratingPlant")]
        public IHttpActionResult ratingUserPlant()
        {
            if (Utilities.ActionAllowed(UserAction.EcoScan) == FeatureAccess.NotAllowed)
                return Unauthorized();

            UserPlantController pc = new UserPlantController();
            PlantSearchController psc = new PlantSearchController();

            var ecoList = psc.getEcoTagsList();

            var allPlants = pc.Index(false, false, false).Select(u => u.UserPlant).DistinctBy(p => p.PlantId).ToList();


            if (allPlants.Count() == 0)
            {
                return Ok(0);
            }

            var ecoPlants = allPlants.Where(p => p.Badges.Any(t => ecoList.Contains(t.Id))).ToList();

            var total = ecoPlants.Count() * 100.0 / allPlants.Count();

            return Ok((int)total);
        }

        [HttpGet]
        [Route("durationRatingPlant")]
        public IHttpActionResult durationRatingUserPlant()
        {
            if (Utilities.ActionAllowed(UserAction.EcoScan) == FeatureAccess.NotAllowed)
                return Unauthorized();

            UserPlantController pc = new UserPlantController();
            PlantSearchController psc = new PlantSearchController();

            var ecoList = psc.getEcoTagsList();

            var allPlants = pc.Index(false, false, false).Select(u => u.UserPlant).DistinctBy(p => p.PlantId);

            var ecoPlants = allPlants.Where(p => p.Badges.Any(t => ecoList.Contains(t.Id))).Select(p => p.PlantId).ToList();

            int[] durationCounter = new int[] { 0,0,0,0,0,0,0,0,0,0,0,0 };
            var charList = psc.getMultiplePlantMonthCharacteristic(ecoPlants);
   
            foreach (var character in charList)
            {
                // make the loop "wrap-around"
                var min = character[0] - 1;
                var max = character[1] - 1;
                if (max < min)
                {
                    max += 12;
                }
                for (var i = character[0] - 1; i <= character[1]; i++)
                {
                    durationCounter[i % 12] += 1;
                }
            }

            var total = durationCounter.Select((d, i) => new UserPlantFlowerDurationViewModel
            {
                Month = i + 1,
                PlantCount = d
            });

            return Ok(total);
        }

        // GET: api/UserPlantsAPI/5
        [ResponseType(typeof(UserPlant))]
        [HttpGet]
        [Route("{id}")]
        public UserPlantDetailsViewModel GetUserPlant(int id)
        {
            UserPlantController pc = new UserPlantController();

            return pc.Details(id);
        }

        // PUT: api/UserPlantsAPI/5
        [ResponseType(typeof(UserPlantEditViewModel))]
        [HttpPut]
        [Route("{editCount}")]
        public UserPlantEditViewModel PutUserPlant(UserPlantEditViewModel userPlant, bool editCount)
        {
            UserPlantController pc = new UserPlantController();

            var editedUserPlant = pc.Edit(userPlant, editCount);


            var userId = Utilities.GetUserId();
            var cacheUniqueId = userId.ToString() + "_mygarden";
            if (userId == Guid.Empty)
            {
                return editedUserPlant;
            }
            string cachedFilePath = HttpContext.Current.Server.MapPath("~") + "\\gardenCache\\" + cacheUniqueId + ".txt";

            var cachedData = pc.GetCachedIndex(cachedFilePath);

            if (cachedData == null)
            {
                return editedUserPlant;

            }
            var cachedDataFiltered = cachedData.Where(c => c.UserPlant.Id != userPlant.Id).ToList();
            var filterData = cachedData.FirstOrDefault(c => c.UserPlant.Id == userPlant.Id);

            //var cachedDataFiltered = cachedData.Where(c => c.UserPlant.Id != userPlantId);

            if (editCount)
            {
                filterData.UserPlant.Count = userPlant.Count;
            }
            else
            {
                filterData.UserPlant.Notes = userPlant.Notes;
                filterData.UserPlant.Count = userPlant.Count;
                filterData.UserPlant.Name = userPlant.Name;
                filterData.UserPlant.Description = userPlant.Description;
                filterData.UserPlant.IsInPot = userPlant.IsInPot;
            }

            cachedDataFiltered.Add(filterData);

            pc.UpdateIndexCache(cachedDataFiltered.AsEnumerable(), cachedFilePath);

            

            return editedUserPlant;
        }

        [HttpGet]
        [Route("deleteCache")]
        public IHttpActionResult deleteCache(string userId)
        {
            UserPlantController pc = new UserPlantController();

            var cacheUniqueId = userId + "_mygarden";
            if (userId == "")
            {
                return BadRequest("empty user Id");
            }
            string cachedFilePath = HttpContext.Current.Server.MapPath("~") + "\\gardenCache\\" + cacheUniqueId + ".txt";

            pc.DeleteIndexCache(cachedFilePath);

            return Ok();
        }

        [HttpPost]
        [Route("prop")]
        public async Task<HttpResponseMessage> AddUserPlantToProp(BorrowUserPlantViewModel up, bool checksub = false)
        {
            if (Utilities.ActionAllowed(UserAction.NewPlant) == FeatureAccess.NotAllowed)
                return Request.CreateResponse(HttpStatusCode.Unauthorized);

            UserPlantToUserListView[] uPlantTrigger = up.ArrayOfUserlist;
            UserPlantController pc = new UserPlantController();
            GardenAPIController gac = new GardenAPIController();
            MaintenanceAPIController mac = new MaintenanceAPIController();
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            if (checksub)
            {
                var plantCount = gac.getPlantCount();
                //var limitSetting = mac.GetSetting("PLANT_LIMIT", "25", SettingType.Integer);
                //int plantLimit = (limitSetting.actualType)limitSetting.Value;
                if(plantCount > 25)
                {
                    return Request.CreateResponse(HttpStatusCode.BadRequest, new { message = "Your Garden is limited to 25 plant, please purchase Gardify Plus" });

                }
            }

            var userProperty = new PropertyController().DbGetProperty(userId);
            var ownGarden = userProperty.Gardens.Where(g => !g.Deleted).FirstOrDefault();
            if (userProperty == null || ownGarden == null)
            {
                return Request.CreateResponse(HttpStatusCode.BadRequest);
            }

            UserPlantController upc = new UserPlantController();
            var userName = Utilities.GetUserName();
            UserPlant res = upc.DbAddPlantToProperty(up.PlantId, ownGarden.Id, userId, userName, up.InitialAgeInDays, up.Count, up.IsInPot,true, uPlantTrigger);
            if (res != null)
            {
                var user = db.Users.Where(u => !u.Deleted && u.Id == userId.ToString()).FirstOrDefault();
                // check for plant warnings
                var plant = new HashSet<UserPlantDetailsViewModel>();
                plant.Add(new UserPlantDetailsViewModel() { PlantId = res.PlantId, Name = res.Name });
                await new WarningController().WarnUser(user, db, null, userProperty, plant);


                //cache part


                var cacheUniqueId = userId.ToString() + "_mygarden";
                if (userId == Guid.Empty)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Id = res.Id });
                }

                string cachedFilePath = HttpContext.Current.Server.MapPath("~") + "\\gardenCache\\" + cacheUniqueId + ".txt";
                var cachedData = pc.GetCachedIndex(cachedFilePath);

                if (cachedData == null)
                {
                    return Request.CreateResponse(HttpStatusCode.OK, new { Id = res.Id });

                }

                //UserPlantDetailsViewModel newModel = new UserPlantDetailsViewModel
                //{
                //    Id = res.Id,
                //    Count = res.Count,
                //    //Badges = GetPlantBadges(plant.PlantId),
                //    Description = res.Description,
                //    Gardenid = res.Gardenid,
                //    //Age = GetUserplantAge(plant),
                //    Name = res.Name + " [Laden, bitte nachladen]",
                //    CustomName = (res.Count <= 1) ? res.Name : "(" + res.Count + ") " + res.Name,
                //    PlantId = res.PlantId,
                //    IsInPot = res.IsInPot,
                //    Notes = res.Notes,
                //    NameLatin = res.NameLatin,
                //    Synonym = res.Synonym,
                //    DatePlanted = res.CreatedDate,
                //    //PlantTag = withTags ? pt.DBGetPlantTagsByPlantId(plant.PlantId) : null,
                //    NotifyForFrost = res.NotifyForFrost,
                //    NotifyForWind = res.NotifyForWind
                //};

                //UserPlantDetailsViewModelCount uvm = new UserPlantDetailsViewModelCount
                //{
                //    ListIds = up.ArrayOfUserlist.Select(a => a.UserListId).ToList(),
                //    UserPlant = newModel,
                //    //ListNames = userPlantsListNames.Distinct().ToArray()
                //};

                //cachedData.Add(uvm);
                //pc.UpdateIndexCache(cachedData, cachedFilePath);


                //Task.Run(() => pc.ReloadIndex(cachedFilePath));
                pc.DeleteIndexCache(cachedFilePath);


                return Request.CreateResponse(HttpStatusCode.OK, new { Id = res.Id });
            }




            return Request.CreateResponse(HttpStatusCode.BadRequest);
        }

        //[HttpPost]
        //[Route("propRaw")]
        public UserPlant AddUserPlantRaw(BorrowUserPlantViewModel up, bool checksub = false)
        {
            //if (Utilities.ActionAllowed(UserAction.NewPlant) == FeatureAccess.NotAllowed)
            //    return null;

            UserPlantToUserListView[] uPlantTrigger = up.ArrayOfUserlist;
            GardenAPIController gac = new GardenAPIController();
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return null;
            }
            if (checksub)
            {
                var plantCount = gac.getPlantCount();
                if (plantCount > 25)
                {
                    return null;

                }
            }


            var userProperty = new PropertyController().DbGetProperty(userId);
            var ownGarden = userProperty.Gardens.Where(g => !g.Deleted).FirstOrDefault();
            if (userProperty == null || ownGarden == null)
            {
                return null;
            }

            UserPlantController upc = new UserPlantController();
            var userName = Utilities.GetUserName();
            UserPlant res = upc.DbAddPlantToProperty(up.PlantId, ownGarden.Id, userId, userName, up.InitialAgeInDays, up.Count, up.IsInPot, true, uPlantTrigger);
            return res;
        }

        [HttpPost]
        public async Task<HttpResponseMessage> PostUserPlant(UserPlant userPlant)
        {
            if (Utilities.ActionAllowed(UserAction.NewPlant) == FeatureAccess.NotAllowed)
                return Request.CreateResponse(HttpStatusCode.Unauthorized);

            PlantSearchController psc = new PlantSearchController();
            var searchResult = psc.Index(searchtext: userPlant.NameLatin);

            if( searchResult.PlantList.Count() > 0)
            {
                var plant = searchResult.PlantList.First();
                var up = new BorrowUserPlantViewModel
                {
                    Count = userPlant.Count,
                    InitialAgeInDays = userPlant.InitialAgeInDays,
                    IsInPot = userPlant.IsInPot,
                    PlantId = plant.Id
                };
                return await AddUserPlantToProp(up);
 
            }

            return Request.CreateResponse(HttpStatusCode.BadRequest);
            //userPlant.Gardenid = new GardenController().DbGetGardensByUserId(Utilities.GetUserId()).FirstOrDefault().Id;
            

            //db.UserPlants.Add(userPlant);
            //db.SaveChanges();

            //return Request.CreateResponse(HttpStatusCode.OK, new { userPlant.Id });
        }

        [HttpPost]
        [Route("userPlantToUserList")]
        public IHttpActionResult AddUserPlantIntoList(UserPlantToUserListView[] uPlantTrigger)
        {
            UserPlantController upc = new UserPlantController();
            foreach (var a in uPlantTrigger)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                else
                {
                    upc.AddUserPlantIntoListVm(a);
                }
            }

            return StatusCode(HttpStatusCode.Created);
        }

        [HttpPost]
        [Route("userPlantToUserListSingle")]
        public IHttpActionResult AddUserPlantIntoListSingle(UserPlantToUserListView uPlantTrigger)
        {
            UserPlantToUserList up;
      
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            else
            {
                up = new UserPlantToUserList
                {
                    UserListId = uPlantTrigger.UserListId,
                    PlantId = uPlantTrigger.UserPlantId
                };
                up.OnCreate(Utilities.GetUserName());
                db.UserPlantToUserLists.Add(up);
                db.SaveChanges();
            }


            return StatusCode(HttpStatusCode.Created);
        }
        [HttpGet]
        [Route("UserPlantByUserListId/{listId}")]
        public IEnumerable<UserPlantDetailsViewModelCount> getUserPlantsByUserListId(int listId)
        {
            UserPlantController pc = new UserPlantController();
            return pc.Index(false,true,true,listId);
            //List<UserPlantDetailsViewModelCount> userplt = new List<UserPlantDetailsViewModelCount>();
            //UserPlantController pc = new UserPlantController();
            //UserPlantDetailsViewModelCountList upcl = new UserPlantDetailsViewModelCountList();
            //var userPlantToUserList = pc.DbGetGroupUserPlantToUserLists(listId);
            //if (userPlantToUserList != null && userPlantToUserList.Any())
            //{
            //    foreach (var a in userPlantToUserList)
            //    {
            //        var plantId = a.PlantId;
            //        UserPlantDetailsViewModelCount uvm = new UserPlantDetailsViewModelCount
            //        {
            //            UserPlant = pc.GetUserPlantDetailsViewModel(plantId, false),
            //            ListId=a.UserListId,
            //            Count=a.Count
            //        };

            //        userplt.Add(uvm);
            //    }
                
            //    upcl.UserPlantInUserListCount = pc.UserplantsToUserListCount(pc.DbGetUserPlantToUserLists(listId));
            //    upcl.UserPlantsList = userplt;
            //}
            //return upcl;
        }

        [HttpGet]
        [Route("updateUserPlantNotification/{id}/{forFrost}")]
        public IHttpActionResult UpdateUserPlantNotificationById(int id, bool forFrost)
        {
            UserPlant thisPlant = db.UserPlants.Find(id);
            if (forFrost)
            {
                thisPlant.NotifyForFrost = !thisPlant.NotifyForFrost;
            }
            else
            {
                thisPlant.NotifyForWind = !thisPlant.NotifyForWind;
            }
            db.SaveChanges();
            return Ok();
        }

        [HttpPost]
        [ResponseType(typeof(UserPlant))]
        [Route("upload")]
        public UserPlantDetailsViewModel UploadUserPlantImage()
        {
            var id = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);
            UserPlantController upc = new UserPlantController();
            PointController pc = new PointController();
            ImgResizerController imgResizer = new ImgResizerController();
            if (HttpContext.Current.Request.Files[0] != null)
            {
                var imageFile = HttpContext.Current.Request.Files[0];
                Guid userId = new Guid(HttpContext.Current.Request.Params["userId"]);
                var imageTitle = HttpContext.Current.Request.Params["imageTitle"];
                var imageDescription = HttpContext.Current.Request.Params["imageDescription"];
                HttpPostedFileBase filebase = new HttpPostedFileWrapper(imageFile);
                upc.UploadUserPlantImage(filebase, imageFile, id, imageTitle, imageDescription);

                pc.DbAddPoints(userId, (int)PointsForAction.Photo, true, "Bild hochgeladen", "testUser");

            }

            return upc.Details(id);
        }

        // DELETE: api/UserPlantsAPI/5
        [HttpDelete]
        [Route("{id}/{gardenId}")]
        public IHttpActionResult DeleteUserPlant(int id, int gardenId)
        {
            UserPlantController pc = new UserPlantController();
            pc.DeleteConfirmed(id, gardenId);
            return Ok();
        }
        [HttpDelete]
        [Route("deleteUserPlantFromUserList/{userPlantId}/{userListId}")]
        public IHttpActionResult DeleteUserPlantFromUserList(int userPlantId,int userListId)
        {
            UserPlantController pc = new UserPlantController();
            pc.DbDeleteUserPlantFromUserList(userPlantId,userListId);
            return Ok();
        }
        [HttpDelete]
        [Route("deleteUserPlantFromAllUserList/{userPlantId}/{gardenId}")]
        public IHttpActionResult deleteUserPlantFromAllUserlists(int userPlantId, int gardenId)
        {
            UserPlantController pc = new UserPlantController();
            pc.DbDeleteUserPlantFromAllUserLists(userPlantId, gardenId);

            var userId = Utilities.GetUserId();
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
            var cachedDataFiltered = cachedData.Where(c => c.UserPlant.PlantId != userPlantId);
            //var cachedDataFiltered = cachedData.Where(c => c.UserPlant.Id != userPlantId);

            pc.UpdateIndexCache(cachedDataFiltered, cachedFilePath);
            return Ok();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool UserPlantExists(int id)
        {
            return db.UserPlants.Count(e => e.Id == id) > 0;
        }
    }
}