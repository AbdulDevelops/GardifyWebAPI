using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using static GardifyModels.Models.DiaryViewModels;
using static GardifyModels.Models.HelperClasses;

namespace GardifyWebAPI.Controllers
{
    [Authorize]
    [RoutePrefix("api/DiaryAPI")]
    public class DiaryAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ImgResizerController imgResizer = new ImgResizerController();
        // GET: api/DiaryAPI
        [HttpGet]
        public DiaryListViewModel GetDiaryEntries(string m, string y)
        {
            GardenController gc = new GardenController();
            DiaryController dc = new DiaryController();
            DiaryListViewModel viewModel = new DiaryListViewModel();

            Guid userId = Utilities.GetUserId();
            var mainGarden = gc.DbGetGardensByUserId(userId).FirstOrDefault();
            
            if (mainGarden == null)
            {
                return viewModel;
            }

            viewModel.ObjectId = mainGarden.Id;
            viewModel.DiaryType = ModelEnums.ReferenceToModelClass.Garden;
            
            int month;
            int year;
            try
            {
                month = int.Parse(m);
                year = int.Parse(y);
            } catch(Exception)
            {
                return viewModel;
            }

            HelperClasses.DbResponse response = dc.DbGetDiaryEntriesByObjectId(mainGarden.Id, userId, month, year, viewModel.DiaryType);

            if (response == null || response.Status != ModelEnums.ActionStatus.Success)
            {
                return viewModel;
            }

            List<DiaryEntryViewModel> diaryEntries = new List<DiaryEntryViewModel>();

            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                foreach (DiaryEntry entry in response.ResponseObjects)
                {
                    DiaryEntryViewModel entryView = new DiaryEntryViewModel();
                    entryView.Id = entry.Id;
                    entryView.ObjectId = mainGarden.Id;
                    entryView.Title = entry.Title;
                    entryView.Description = entry.Description;
                    entryView.Date = entry.Date;
                    entryView.DiaryType = ModelEnums.ReferenceToModelClass.Garden;
                    ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
                    HelperClasses.DbResponse imageResponse = rc.DbGetDiaryEntryReferencedImages(entry.Id);

                    entryView.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, System.Web.Hosting.HostingEnvironment.MapPath("~/"));

                    diaryEntries.Add(entryView);
                }
            }

            viewModel.ListEntries = diaryEntries;
            return viewModel;
        }

        [HttpGet]
        [Route("BioScan")]
        public DiaryListViewModel GetBioScanEntries(string m, string y)
        {
            GardenController gc = new GardenController();
            DiaryController dc = new DiaryController();
            DiaryListViewModel viewModel = new DiaryListViewModel();

            Guid userId = Utilities.GetUserId();
            var mainGarden = gc.DbGetGardensByUserId(userId).FirstOrDefault();

            if (mainGarden == null)
            {
                return viewModel;
            }

            viewModel.ObjectId = mainGarden.Id;
            viewModel.DiaryType = ModelEnums.ReferenceToModelClass.BioScan;

            int month;
            int year;
            try
            {
                month = int.Parse(m);
                year = int.Parse(y);
            }
            catch (Exception)
            {
                return viewModel;
            }            

            HelperClasses.DbResponse response = dc.DbGetDiaryEntriesByObjectId(mainGarden.Id, userId, month, year, viewModel.DiaryType);

            if (response == null || response.Status != ModelEnums.ActionStatus.Success)
            {
                return viewModel;
            }

            List<DiaryEntryViewModel> diaryEntries = new List<DiaryEntryViewModel>();

            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                foreach (DiaryEntry entry in response.ResponseObjects)
                {
                    DiaryEntryViewModel entryView = new DiaryEntryViewModel();
                    entryView.Id = entry.Id;
                    entryView.ObjectId = mainGarden.Id;
                    entryView.Title = entry.Title;
                    entryView.Description = entry.Description;
                    entryView.Date = entry.Date;
                    entryView.DiaryType = ModelEnums.ReferenceToModelClass.BioScan;


                    diaryEntries.Add(entryView);
                }
            }

            viewModel.ListEntries = diaryEntries;
            return viewModel;
        }

        [HttpGet]
        [ResponseType(typeof(DiaryEntry))]
        public DiaryEntryViewModel GetDiary(int id)
        {
            var userId = Utilities.GetUserId();
            var res = db.DiaryEntry.Where(e => e.Id == id && !e.Deleted && e.UserId == userId).FirstOrDefault();

            if (res == null)
            {
                return new DiaryEntryViewModel();
            }

            var vm = new DiaryEntryViewModel()
            {
                Title = res.Title,
                Description = res.Description,
                ObjectId = res.EntryObjectId,
                Date = res.Date,
                Id = res.Id
            };

            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            HelperClasses.DbResponse imageResponse = rc.DbGetDiaryEntryReferencedImages(res.Id);
            vm.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, System.Web.Hosting.HostingEnvironment.MapPath("~/"));

            return vm;
        }

        [HttpPost]
        [ResponseType(typeof(DiaryEntry))]
        public IHttpActionResult PostDiary(DiaryEntry entry, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            if (entry != null && entry.EntryOf == ModelEnums.ReferenceToModelClass.BioScan)
            {
                new StatisticsController().CreateEntry(StatisticEventTypes.SaveBioScan, Utilities.GetUserId());
                
                    if (isIos)
                    {
                        new StatisticsController().CreateEntry(StatisticEventTypes.SaveBioScan, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromIos);
                    }
                    else if (isAndroid)
                    {
                        new StatisticsController().CreateEntry(StatisticEventTypes.SaveBioScan, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromAndroid);
                    }
                    else if (isWebPage)
                    {
                        new StatisticsController().CreateEntry(StatisticEventTypes.SaveBioScan, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage);
                    }

              
            }
            else if(entry != null && entry.EntryOf != ModelEnums.ReferenceToModelClass.BioScan)
            {

                
                    if (isIos)
                    {
                        new StatisticsController().CreateEntry(StatisticEventTypes.DiaryEntry, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromIos);
                    }
                    else if (isAndroid)
                    {
                        new StatisticsController().CreateEntry(StatisticEventTypes.DiaryEntry, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromAndroid);
                    }
                    else if (isWebPage)
                    {
                        new StatisticsController().CreateEntry(StatisticEventTypes.DiaryEntry, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage);
                    }

                
                else
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.DiaryEntry, Utilities.GetUserId());
                }
            }

            entry.OnCreate(Utilities.GetUserName());
            try
            {
                db.DiaryEntry.Add(entry);
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest();
                }
                else
                {
                    throw;
                }
            }
            
            return CreatedAtRoute("DefaultApi", new { id = entry.Id }, entry);
        }

        [HttpPost]
        [Route("upload")]
        public IHttpActionResult UploadDiaryImage()
        {
            var diaryId = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);
            DiaryController dc = new DiaryController();
            if (HttpContext.Current.Request.Files[0] != null)
            {
                var imageFile = HttpContext.Current.Request.Files[0];
                var imageTitle = HttpContext.Current.Request.Params["imageTitle"];
                HttpPostedFileBase filebase = new HttpPostedFileWrapper(imageFile);
                
                dc.UploadUserPlantDiaryImage(filebase,imageFile, diaryId, imageTitle);
               
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // PUT: api/DiaryAPI/5
        [HttpPut]
        public IHttpActionResult PutDiaryEntry(int id, DiaryEntryViewModel entry)
        {
            var userId = Utilities.GetUserId();
            var dbEntry = db.DiaryEntry.Where(d => d.Id == id && d.UserId == userId).FirstOrDefault();
            if (dbEntry == null || id != entry.Id)
            {
                return BadRequest();
            }

            dbEntry.Title = entry.Title;
            dbEntry.Date = entry.Date;
            dbEntry.Description = entry.Description;
            dbEntry.OnEdit(Utilities.GetUserName());
            
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiaryEntryExists(id))
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

        // DELETE: api/DiaryAPI/5
        [HttpDelete]
        public DbResponse DeleteDiaryEntry(int id)
        {
            string deletedBy = "testUser";
            DiaryController dc = new DiaryController();
            return dc.DbDeleteDiaryEntry(id, deletedBy);
        }

        private bool DiaryEntryExists(int id)
        {
            return db.DiaryEntry.Count(e => e.Id == id) > 0;
        }
    }
}
