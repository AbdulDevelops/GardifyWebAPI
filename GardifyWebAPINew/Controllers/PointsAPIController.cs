using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Results;

namespace GardifyWebAPI.Controllers
{
    public class PointsAPIController : ApiController
    {
        private readonly ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        [Route("api/PointsAPI/pending")]
        public IEnumerable<PointsPending> GetPendingPoints()
        {
            PointController pc = new PointController();
            return pc.DbGetPendingPoints();
        }

        [HttpGet]
        [Route("api/PointsAPI")]
        public PointsView GetUserPoints()
        {
            var userId = Utilities.GetUserId();
            PointController pc = new PointController();
            PointsView res = new PointsView { Points = pc.GetUserPoints(userId) };
            return res;
        }

        [HttpGet]
        [Route("api/PointsAPI/history/{userId}")]
        public IQueryable<BonusHistory> GetEarnedPoints(Guid userId)
        {
            return db.BonusHistories.Where(m => m.UserId == userId && m.PointType == (int)ModelEnums.BonusPointType.Earned);
        }

        [HttpGet]
        [Route("api/PointsAPI/historySpent/{userId}")]
        public IHttpActionResult GetSpentPoints(Guid userId)
        {
            return Ok(db.BonusHistories.Where(m => m.UserId == userId && m.PointType == (int)ModelEnums.BonusPointType.Spent));
        }

        [HttpGet]
        [Route("api/PointsAPI/add")]
        public IHttpActionResult AddPoints(Guid userId, int points, string origin, string createdBy)
        {
            PointController pc = new PointController();
            pc.DbAddPoints(userId, points, true, origin, createdBy);
            return Ok();
        }
    }

    public class PointsView
    {
        public int Points { get; set; }
    }
}
