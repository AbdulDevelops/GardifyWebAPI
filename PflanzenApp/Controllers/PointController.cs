using Microsoft.AspNet.Identity;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
{
    public class PointController : _BaseController
    {

        #region DB
        [NonAction]
        public void DbAddPoints(Guid userid, int points, bool needsApproval, string origin, string createdBy)
        {
            if (needsApproval)
            {
                PointsPending newPoints = new PointsPending();
                newPoints.Approved = false;
                newPoints.UserId = userid;
                newPoints.RequiresApproval = true;
                newPoints.Points = points;
                newPoints.Origin = origin;
                newPoints.ApprovedDate = null;
                newPoints.ApprovedBy = null;
                newPoints.OnCreate(createdBy);

                ctx.PointsPending.Add(newPoints);
                ctx.SaveChanges();
            }
            else
            {
                PointsPending newPoints = new PointsPending();
                newPoints.Approved = true;
                newPoints.UserId = userid;
                newPoints.RequiresApproval = false;
                newPoints.Points = points;
                newPoints.ApprovedDate = DateTime.Now;
                newPoints.ApprovedBy = new Guid("00000000000000000000000000000000");
                newPoints.Origin = origin;
                newPoints.OnCreate(createdBy);

                ctx.PointsPending.Add(newPoints);
                ctx.SaveChanges();

                DbAddPoints(userid, points);
            }
        }

		[NonAction]
		public int GetUserPoints(Guid UserId)
		{
			ApplicationDbContext appDb = new ApplicationDbContext();

			int ret = 0;
			var points_sel = (from p in appDb.Users
							  where p.Id == UserId.ToString()
							  select p);
			if (points_sel != null && points_sel.Any())
			{
				ret = points_sel.FirstOrDefault().Points;
			}
			return ret;
		}

        [NonAction]
        public void DbAddPoints(Guid userid, int points)
        {
			ApplicationDbContext appDb = new ApplicationDbContext();

			var user_sel = (from p in appDb.Users
							where p.Id == userid.ToString()
							select p);

			if(user_sel != null && user_sel.Any())
			{
				user_sel.FirstOrDefault().Points += points;
				appDb.Entry(user_sel).State = System.Data.Entity.EntityState.Modified;
				appDb.SaveChanges();
			}
		}

        [NonAction]
        public IEnumerable<PointsPending> DbGetPendingPoints()
        {
            var data = (from p in ctx.PointsPending
                        where p.RequiresApproval == true
                        && p.Approved == false
                        select p);
            return data;
        }

        [NonAction]
        public void DbApprovePoints(int pendingId)
        {
            Guid userid = new Guid(User.Identity.GetUserId());
            var pending = (from p in ctx.PointsPending
                           where p.Id == pendingId
                           select p).FirstOrDefault();
            pending.Approved = true;
            pending.ApprovedBy = userid;
            pending.ApprovedDate = DateTime.Now;
            pending.OnEdit(User.Identity.GetUserName());

            DbAddPoints(pending.UserId, pending.Points);
        }
        [NonAction]
        public void DbDeclinePoints(int pendingId)
        {
            Guid userid = new Guid(User.Identity.GetUserId());
            var pending = (from p in ctx.PointsPending
                           where p.Id == pendingId
                           select p).FirstOrDefault();
            pending.Approved = false;
            pending.RequiresApproval = false;
            pending.ApprovedBy = userid;
            pending.ApprovedDate = DateTime.Now;
            pending.EditedDate = DateTime.Now;
            pending.EditedBy = User.Identity.GetUserName();
        }

        #endregion
    }
}