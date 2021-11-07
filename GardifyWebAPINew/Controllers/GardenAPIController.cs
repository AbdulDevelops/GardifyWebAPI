using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using static GardifyModels.Models.GardenViewModels;
using static GardifyModels.Models.UserPlantViewModels;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using static GardifyModels.Models.ModelEnums;

namespace GardifyWebAPI.Controllers
{
    public class GardenAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ImgResizerController imgResizer = new ImgResizerController();
        private ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
        private AspNetUserManager userManager;

        public AspNetUserManager UserManager
        {
            get
            {
                return userManager ?? Request.GetOwinContext().GetUserManager<AspNetUserManager>();
            }
            private set
            {
                userManager = value;
            }
        }


        [HttpGet]
        [Route("api/GardenAPI/main")]
        public async Task<GardenDetailsViewModel> GetFirstUserGarden(bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            GardenController gc = new GardenController();
            TodoController tc = new TodoController();
            var userId = Utilities.GetUserId();
            var mainGarden = gc.DbGetGardensByUserId(userId).FirstOrDefault();

            if (mainGarden == null)
            {
                return new GardenDetailsViewModel();
            }
            
            GardenDetailsViewModel mainVm = gc.GetGardenDetailsViewModel(mainGarden, false);
            
            var user = await UserManager.FindByIdAsync(userId.ToString());

            // keep track of users activity in case RememberMe is checked
            user.LastLogin = DateTime.Now;
            UserManager.Update(user);

            
           
                if (isIos)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromIos, (int)GardifyPages.Garden, EventObjectType.PageName);
                }
                else if (isAndroid)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromAndroid, (int)GardifyPages.Garden, EventObjectType.PageName);
                }
                else if (isWebPage)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage, (int)GardifyPages.Garden, EventObjectType.PageName);
                }


          
            // temporary to seed all users with settings
            bool hasSettings = db.UsersSettings.Where(us => us.UserId == userId && !us.Deleted).FirstOrDefault() != null;
            if (!hasSettings)
            {
                var userSettings = new UserSettings()
                {
                    UserId = userId,
                    ActiveFrostAlert = true,
                    ActiveStormAlert = true,
                    FrostDegreeBuffer = 3,
                    AlertByEmail = true,
                    AlertByPush = false
                };
                userSettings.OnCreate("Register");
                db.UsersSettings.Add(userSettings);
              
                
              
            }
            
            db.SaveChanges();
            return mainVm;
        }

        public int getPlantCount()
        {
            GardenController gc = new GardenController();
            var userId = Utilities.GetUserId();

            var userGardens = gc.DbGetGardensByUserId(userId).ToList();
            var gardenFirst = userGardens.FirstOrDefault();

            if(gardenFirst == null)
            {
                return 0;
            }

            var userPlantToUserList = from u in db.UserPlantToUserLists where !u.Deleted && u.UserPlant.Gardenid == gardenFirst.Id select u;
            var groupedUserPlantToUserlist = userPlantToUserList.Where(gp => !gp.UserPlant.Deleted).GroupBy(g => g.UserPlant.PlantId).ToList();


            return groupedUserPlantToUserlist.Count();
        }

        [HttpGet]
        [Route("api/GardenAPI/count")]
        public UserPlantViewModels.UserPlantsCount GetUserPlantsCount()
        {
            GardenController gc = new GardenController();
            var gardenDetailsList = gc.IndexLite(false);
            if(gardenDetailsList == null || !gardenDetailsList.Any())
            {
                return new UserPlantViewModels.UserPlantsCount
                {
                    Sorts = 0,
                    Total = 0
                };
            }
            GardenDetailsViewModel garden = gardenDetailsList.ElementAt(0);
            UserPlantViewModels.UserPlantsCount count = new UserPlantViewModels.UserPlantsCount();
            var userPlantToUserList = from u in db.UserPlantToUserLists where !u.Deleted && u.UserPlant.Gardenid == garden.Id select u;
            var groupedUserPlantToUserlist = userPlantToUserList.Where(gp => !gp.UserPlant.Deleted).GroupBy(g =>  g.UserPlant.PlantId).ToList();
            var countPlants = 0;
            foreach (var g in groupedUserPlantToUserlist)
            {
                foreach (var p in g)
                {
                    countPlants = countPlants + p.Count;
                }
            }
            count.Sorts = groupedUserPlantToUserlist.Count();
            //count.Total = userPlantToUserList.Count();
            count.Total = countPlants;
            return count;
        }

        [HttpGet]
        [Route("api/GardenAPI/coords")]
        public PropertyViewModels.PropertyCoordsVM GetGardenCoords()
        {
            PropertyController pc = new PropertyController();
            var userId = Utilities.GetUserId();
            var prop = pc.DbGetProperty(userId);
            return new PropertyViewModels.PropertyCoordsVM()
            {
                Latitude = prop.Latitude,
                Longtitude = prop.Longtitude
            };
        }

        [HttpGet]
        [Route("api/GardenAPI/location")]
        public async Task<GardifyModels.Models.PropertyViewModels.PropertyCreateViewModel> GetGardenLocation()
        {
            PropertyController pc = new PropertyController();
            var userId = Utilities.GetUserId();
            var prop = pc.DbGetProperty(userId);
            //var user = await UserManager.FindByIdAsync(userId.ToString());
            if (prop == null)
            {
                new PropertyViewModels.PropertyCreateViewModel();
            }

            switch (prop.Country)
            {
                case "AT": prop.Country = "Österreich";
                        break;
                case "DE":
                    prop.Country = "Deutschland";
                    break;
                case "CH":
                    prop.Country = "Schweiz";
                    break;
                default: prop.Country = prop.Country;
                    break;
            }
            return new PropertyViewModels.PropertyCreateViewModel()
            {
                City = prop.City == "Platzhalter" ? "" : prop.City,
                Street = prop.Street == "Platzhalter" ? "" : prop.Street,
                Zip = prop.Zip,
                Country=prop.Country == "Platzhalter" ? "" : prop.Country
            };
            
        }

        [HttpPut]
        [Route("api/GardenAPI/location")]
        public async Task<IHttpActionResult> UpdateGardenLocationAsync(GardifyModels.Models.PropertyViewModels.UpdatePropertyViewModel vm)
        {
            PropertyController pc = new PropertyController();
            var userId = Utilities.GetUserId();
            var prop = db.Property.Where(p => !p.Deleted && p.UserId.Equals(userId)).FirstOrDefault();
            if (!ModelState.IsValid || prop == null)
            {
                return BadRequest("Unvollständige Eingaben.");
            }

            if (userId.Equals(prop.UserId))
            {
                prop.City = vm.City;
                prop.Zip = vm.Zip;
                prop.Street = vm.Street;
                prop.Country = vm.Country;
                prop.UpdateCoordinates();

                var res = db.SaveChanges();
                return Ok();
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("api/GardenAPI")]
        public IEnumerable<Garden> GetGardens()
        {
            GardenController gc = new GardenController();

            IEnumerable<Garden> gardens = gc.IndexLiteLite();

            return gardens;
        }

        [HttpGet]
        [Route("api/GardenAPI/details")]
        public IEnumerable<GardenDetailsViewModel> GetGardenDetails()
        {
            var watch = System.Diagnostics.Stopwatch.StartNew();
            GardenController gc = new GardenController();

            IEnumerable<GardenDetailsViewModel> input = gc.IndexLite();
            watch.Stop();
            var elapsedMs = watch.ElapsedMilliseconds;
            System.Diagnostics.Debug.WriteLine("garden indexing took " + elapsedMs);
            return input;
        }

        [HttpDelete]
        [Route("api/GardenAPI/deleteimg/{id}")]
        public IHttpActionResult DeleteGardenImage(int id)
        {
            nfilesEntities nf = new nfilesEntities();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            var res = rc.DbDeleteFile(id, (int)ModelEnums.ReferenceToModelClass.Garden);
            if (res)
            {
                return Ok();
            }
            return BadRequest("Etwas ist schiefgelaufen.");
        }
        
        // GET: api/GardenAPI/5
        [ResponseType(typeof(Garden))]
        [HttpGet]
        [Route("api/GardenAPI/{id}")]
        public GardenDetailsViewModel GetGarden(int id)
        {
            GardenController gc = new GardenController();
            return gc.Details(id);
        }

        // PUT: api/GardenAPI/sort
        [HttpPut]
        [Route("api/GardenAPI/sort")]
        public IHttpActionResult UpdateGardenSort(List<GardenImageSortViewModel> gardenSort)
        {
            var sortedImgs = gardenSort.OrderBy(r => r.Id);
            List<int> Ids = new List<int>();
            foreach (GardenImageSortViewModel vm in sortedImgs) {
                Ids.Add(vm.Id);
            }
            nfilesEntities nf = new nfilesEntities();
            // get images for sorting
            IEnumerable<FileToModule> ftms = nf.FileToModule.Where(ftm => Ids.Contains(ftm.FileToModuleID)).OrderBy(r => r.FileToModuleID).ToList();
            if (ftms != null && ftms.Any())
            {
                for (int i = 0; i < ftms.Count(); i++)
                {
                    var imgId = Ids.ElementAt(i);
                    // img to update sort
                    var imgFile = ftms.ElementAt(i);
                    if (imgFile.FileToModuleID == imgId)
                    {
                        imgFile.Sort = sortedImgs.ElementAt(i).Sort;
                        nf.Entry(imgFile).State = EntityState.Modified;
                    }
                }
            }
            
            nf.SaveChanges();
            return Ok();
        }

        // PUT: api/GardenAPI/5
        [HttpPut]
        [ResponseType(typeof(GardenEditViewModel))]
        [Route("api/GardenAPI/{id}")]
        public GardenEditViewModel PutGarden([System.Web.Mvc.Bind(Include = "id,name,wetness,phType,groundType,info,cardinalDirection,shadowStrength,inside,temperature,light,IsPrivate,MainImageId")] GardenEditViewModel gardenView)
        {
            GardenController gc = new GardenController();
            return gc.Edit(gardenView);
        }

        [HttpPost]
        [ResponseType(typeof(Garden))]
        [Route("api/GardenAPI/upload")]
        public IHttpActionResult UploadGardenImage()
        {
            if (Utilities.ActionAllowed(UserAction.NewGardenImage) == FeatureAccess.NotAllowed)
                return Unauthorized();

            HttpPostedFile imageFile = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0]: null;
            var imageMetadata = new ImageMetadataViewModel();

            imageMetadata.ImageId = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);
            imageMetadata.Title = HttpContext.Current.Request.Params["imageTitle"];
            imageMetadata.Description = HttpContext.Current.Request.Params["imageDescription"];
            imageMetadata.Note = HttpContext.Current.Request.Params["imageNote"];
            imageMetadata.Tags = HttpContext.Current.Request.Params["imageTags"];
            DateTime imageCreatedDate;
            if (!DateTime.TryParse(HttpContext.Current.Request.Params["imageCreatedDate"].Replace("_", "-"), out imageCreatedDate))
            {
                imageCreatedDate = DateTime.Now;
            }
            imageMetadata.UserCreatedDate = imageCreatedDate;
            
            HttpPostedFileBase filebase = new HttpPostedFileWrapper(imageFile);

            GardenController gc = new GardenController();
            
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            var isOk = gc.UploadGardenImage(filebase, imageFile, imageMetadata);
            //if (isOk)
            //{
            //    HelperClasses.DbResponse imageResponse = rc.DbGetGardenReferencedImages(id);
            //    string rootPath = HttpRuntime.AppDomainAppVirtualPath != "/" ? HttpRuntime.AppDomainAppVirtualPath + "/" : "/";
            //    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Count() == 1)
            //    {
            //        var garden = gc.DbGetGarden(id);
            //        var Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, rootPath);
            //        garden.MainImageId = Images.First().Id;
            //        garden.OnEdit(Utilities.GetUserName());
            //        //db.Entry(garden).State = EntityState.Modified;
            //        gc.SaveChanges();
            //    }
            //}
            return Ok(gc.Details(imageMetadata.ImageId));
        }

        // POST: api/GardenAPI
        [HttpPost]
        [Route("api/GardenAPI")]
        [ResponseType(typeof(GardenCreateViewModel))]
        public IEnumerable<GardenDetailsViewModel> PostGarden(GardenCreateViewModel gardenView)
        {
            GardenController gc = new GardenController();
            return gc.Create(gardenView);
        }

        // DELETE: api/GardenAPI/5
        [ResponseType(typeof(Garden))]
        [HttpDelete]
        public IHttpActionResult DeleteGarden(int id)
        {
            GardenController gc = new GardenController();
            gc.DeleteConfirmed(id);
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

        private bool GardenExists(int id)
        {
            return db.Gardens.Count(e => e.Id == id) > 0;
        }

        #region EcoElements

        [HttpGet]
        [Route("api/GardenAPI/ecoelements")]
        public IEnumerable<EcoElementViewModel> GetUserEcoElements()
        {
            var userId = Utilities.GetUserId();

            // will only generate checklist items if neccessary
            SeedUserEcoElements(userId);

            var elems = db.UserEcoElements.Where(ee => !ee.Deleted && ee.UserId==userId);
            var res = new List<EcoElementViewModel>();
            if(elems!=null&& elems.Any())
            {
                foreach (UserEcoElement ee in elems)
                {
                    var temp = new EcoElementViewModel()
                    {
                        Checked = ee.Checked,
                        Name = ee.EcoElement.Name,
                        Description = ee.EcoElement.Description,
                        Id = ee.EcoElementId,
                        EcoCount = ee.EcoCount
                    };
                    HelperClasses.DbResponse ecoImages = rc.DbGetEcoElementReferencedImages(temp.Id);
                    if (ecoImages.ResponseObjects != null && ecoImages.ResponseObjects.Any())
                    {
                        temp.EcoElementsImages = Utilities.getHtmlImageObjectsFromDbImageResponse(ecoImages, Utilities.GetAbsolutePath("~/"));
                    }
                    else
                    {
                        temp.EcoElementsImages.Add(new _HtmlImageViewModel
                        {
                            SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                            Id = 0,
                            TitleAttr = "Kein Bild vorhanden"
                        });
                    }
                    res.Add(temp);
                }
            }
           
            return res;
        }
        [HttpGet]
        [Route("api/GardenAPI/countCheckedEcoEl")]
        public int countCheckedEcoElemt()
        {
            var userId = Utilities.GetUserId();
            var count = db.UserEcoElements.Where(ee => !ee.Deleted && ee.Checked && ee.UserId==userId);
            return count.Count();
        }

        [HttpGet]
        [Route("api/GardenAPI/ratingTotalEcoEl")]
        public int ratingTotalEcoElemt(bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            var userId = Utilities.GetUserId();
           
            
                if (isIos)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromIos, (int)GardifyPages.BioScan, EventObjectType.PageName);
                }
                else if (isAndroid)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromAndroid, (int)GardifyPages.BioScan, EventObjectType.PageName);
                }
                else if (isWebPage)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage, (int)GardifyPages.BioScan, EventObjectType.PageName);
                }

            var allEco = db.UserEcoElements.Where(ee => !ee.Deleted && ee.UserId == userId);
            if (allEco.Count() == 0)
            {
                return 0;
            }
            var total = allEco.Where(ee => ee.Checked).Count() * 100.0 / allEco.Count();
            return (int)total;
        }
        [HttpPut]
        [Route("api/GardenAPI/updateEcoelements")]
        public IHttpActionResult PutUserEcoElement(EcoElementUpdateViewModel vm)
        {
            var userId = Utilities.GetUserId();
            var elem = db.UserEcoElements.Where(ee => !ee.Deleted && ee.UserId == userId && ee.EcoElementId == vm.Id).FirstOrDefault();
            if (elem != null)
            {
                elem.Checked = vm.Checked;
                db.SaveChanges();
            }
            
            return Ok(new { message = "success"});
        }


        [HttpPut]
        [Route("api/GardenAPI/updateEcoelementCounts")]
        public IHttpActionResult PutUserEcoElementCount(EcoElementCountUpdateViewModel vm)
        {
            var userId = Utilities.GetUserId();
            var elem = db.UserEcoElements.Where(ee => !ee.Deleted && ee.UserId == userId && ee.EcoElementId == vm.Id).FirstOrDefault();
            if (elem != null)
            {
                elem.EcoCount = vm.EcoCount;
                db.SaveChanges();
            }

            return Ok(new { message = "success" });
        }

        [NonAction]
        public void SeedUserEcoElements(Guid userId)
        {
            var elems = db.EcoElements.Where(ee => !ee.Deleted);
            var userElems = db.UserEcoElements.Where(uee => !uee.Deleted && uee.UserId==userId);
            var res = new List<UserEcoElement>();

            if (elems.Count() != userElems.Count())
            {
                foreach (EcoElement ee in elems)
                {
                    var temp = new UserEcoElement()
                    {
                        Checked = false,
                        UserId = userId,
                        EcoElement = ee,
                        EcoElementId = ee.Id
                    };

                    temp.OnCreate("System");
                    // only create it if not already there
                    if (!userElems.Any(ue => ue.EcoElementId == ee.Id))
                    {
                        res.Add(temp);
                    }
                }
                db.UserEcoElements.AddRange(res);
                db.SaveChanges();
            }
        }

        #endregion
    }
}