using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;

namespace GardifyWebAPI.Controllers
{
    public class StatsAPIController : ApiController
    {
        [Route("api/statsAPI/")]
        [HttpPost]
        public IHttpActionResult CreateEntry([FromBody]StatisticEventTypes type)
        {
            new StatisticsController().CreateEntry(type, Utilities.GetUserId());
            return Ok();
        }
    }

    public class StatisticsController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public void CreateEntry(StatisticEventTypes type, Guid userId, int apiCallEventId = 0,int objectId = 0, EventObjectType objectType = 0, DateTime? date = null, string possibleAddress = "")
        {
            ApplicationUser user = db.Users.Find(userId.ToString());



            var ev = new StatisticEntry() 
            {
                EventId = (int)type,
                UserId = userId,
                Date = date != null ? date.Value : DateTime.Now,
                ObjectId = objectId,
                ObjectType = objectType,
                DemoMode = (user != null && user.Email.StartsWith("UserDemo")),
                PossibleAddress = (user == null ? possibleAddress : null),
                ApiCallEventId= apiCallEventId
            };
         
            db.StatisticEntries.Add(ev);
            try
            {
                db.SaveChanges();

            }
            catch
            {

            }

        }
        public void SetDemoUser(Guid userId, bool demoMode)
        {
            ApplicationUser user = db.Users.Find(userId.ToString());
            var statEnt = db.StatisticEntries.Find(userId);
            if (statEnt != null)
            {
                statEnt.DemoMode = !statEnt.DemoMode;
                db.SaveChanges();
            }
        }
    }
}
