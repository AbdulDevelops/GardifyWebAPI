using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace GardifyWebAPI.Controllers
{
    public class MaintenanceAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]

        // GET: MaintenanceAPI
        [Route("api/MaintenanceAPI")]

        public IHttpActionResult getstatus(bool forceMaintenance = false)
        {
            var data = db.MaintenanceSettings;

            if (forceMaintenance)
            {
                var output = new MaintenanceView
                {
                    IsNoticeExist = true,
                    Message = "Die Wartung ist im Gange"
                };
                return Ok(output);
            }

            if (!data.Any())
            {
                var output = new MaintenanceView
                {
                    IsNoticeExist = false,
                    Message = ""
                };
                return Ok(output);
            }


            var mtEntry = data.OrderByDescending(m => m.MaintenanceStart).First();
            if (mtEntry.IsRunning == false)
            {
                var output = new MaintenanceView
                {
                    IsNoticeExist = false,
                    Message = ""
                };
                return Ok(output);
            }

            if(DateTime.Now < mtEntry.MaintenanceStart)
            {

            var output = new MaintenanceView
            {
                IsNoticeExist = true,
                Message = "In " + getMinuteCountDown((int)(mtEntry.MaintenanceStart - DateTime.Now).TotalMinutes) + " Minuten findet eine Wartung statt. Bitte nutzen Sie den Dienst während dieser Zeit nicht."
            };
                return Ok(output);

            }
            else
            {
                var output = new MaintenanceView
                {
                    IsNoticeExist = true,
                    Message = "Die Wartung ist im Gange"
                };
                return Ok(output);
            }
        }


        public int getMinuteCountDown(int difference)
        {
            if (difference > 30)
            {
                return 60;
            }
            else if (difference > 15)
            {
                return 30;

            }
            else if (difference > 10)
            {
                return 15;

            }
            else if (difference > 5)
            {
                return 10;

            }
            else
            {
                return 5;
            }
        }
        [HttpPost]

        // GET: MaintenanceAPI
        [Route("api/MaintenanceAPI/startWarning")]

        public IHttpActionResult startwarning()
        {
            var data = db.MaintenanceSettings;

            if (data.Any())
            {
                var mtEntry = data.OrderByDescending(m => m.MaintenanceStart).First();

                if (mtEntry.IsRunning == true)
                {
                    return Ok("Maintenance in progress!");
                }
            }

           
            var newEntry = new MaintenanceSetting
            {
                MaintenanceNoticeStart = DateTime.Now,
                MaintenanceStart = DateTime.Now.AddMinutes(60),
                IsRunning = true,
                MaintenanceNote = ""
            };

            db.MaintenanceSettings.Add(newEntry);
            db.SaveChanges();

            return Ok("Maintenance Started");
        }

        [HttpPost]

        // GET: MaintenanceAPI
        [Route("api/MaintenanceAPI/stopWarning")]

        public IHttpActionResult stopWarning()
        {
            var mtEntry = db.MaintenanceSettings.OrderByDescending(m => m.MaintenanceStart).First();

            if (mtEntry.IsRunning == false)
            {
                return Ok("There is no running maintenance!");
            }
            mtEntry.IsRunning = false;

            

            db.SaveChanges();

            return Ok("Maintenance Stopped");
        }

        [HttpPost]

        // GET: MaintenanceAPI
        [Route("api/MaintenanceAPI/SetVersion")]

        public IHttpActionResult SetVersion(AppVersion model)
        {
            var versionDb = db.AppVersions.FirstOrDefault(v => v.VersionKey == model.VersionKey);

            if (versionDb == null)
            {
                AppVersion cm = new AppVersion
                {
                    VersionKey = model.VersionKey,
                    VersionId = model.VersionId
                };

                cm.OnCreate("system");
                db.AppVersions.Add(cm);
                db.SaveChanges();
                return Ok("Version set to " + model.VersionId);
            }


            versionDb.VersionId = model.VersionId;
            versionDb.OnEdit("system");

            db.SaveChanges();
            return Ok("Version set to " + model.VersionId);
        }


     


        [HttpGet]

        [Route("api/MaintenanceAPI/getVersion/{versionKey}")]

        public IHttpActionResult GetVersion(string versionKey)
        {
            var versionDb = db.AppVersions.FirstOrDefault(v => v.VersionKey == versionKey);

            if (versionDb == null)
            {

                return Ok(-1);
            }



            return Ok(versionDb.VersionId);
        }

        [HttpGet]

        [Route("api/MaintenanceAPI/getPlantTagCount")]

        public IHttpActionResult getPlantTagCount()
        {
            var filteredmodel = db.PlantTagCounts.Where(t => t.Count != null);

            var unfilteredmodel = db.PlantTagCounts.Where(t => t.Count == null).GroupBy(t => t.PlantTagId).ToList();

            foreach(var group in unfilteredmodel)
            {
                var tagId = group.First().PlantTagId;
                var vmcount = group.Count();

                if(filteredmodel.Any(f => f.PlantTagId == tagId))
                {
                    var dbFilter = filteredmodel.FirstOrDefault(t => t.PlantTagId == tagId);

                    dbFilter.Count += vmcount;

                    dbFilter.OnEdit("system");
                    db.SaveChanges();
                }
                else
                {
                    PlantTagCount newvm = new PlantTagCount
                    {
                        PlantTagId = tagId,
                        Count = vmcount
                    };
                    newvm.OnCreate("system");
                    db.PlantTagCounts.Add(newvm);

                    db.SaveChanges();
                }

                var outlist = group.ToList();

                db.PlantTagCounts.RemoveRange(outlist);
                db.SaveChanges();

            }

            var outputvm = db.PlantTagCounts.Where(t => t.Count != null);


            return Ok(outputvm);
        }

        //public Setting GetSetting(string key, string defaultValue, SettingType type)
        //{

        //    var setting = db.Settings.FirstOrDefault(s => s.Key == key);

        //    if (setting == null)
        //    {
        //        var newSetting = new Setting
        //        {
        //            Key = key,
        //            Value = defaultValue,
        //            Comment = "-",
        //            SettingType = type
        //        };

        //        newSetting.OnCreate("system");

        //        db.Settings.Add(newSetting);
        //        db.SaveChanges();

        //        return newSetting;
        //    }

        //    return setting;
        //}
    }
}
