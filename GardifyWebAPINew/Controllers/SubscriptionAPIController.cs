using GardifyModels.Models;
using System;
using System.Web.Http;

namespace GardifyWebAPI.Controllers
{
    [RoutePrefix("api/SubscriptionAPI")]
    public class SubscriptionAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpPost]
        [Route("saveGardifyPlusSubscription")]
        public IHttpActionResult saveGardifyPlusSubscription(Subscription sub)
        {
            Subscription userSub = db.Subscription.Find(sub.Id);
            if (userSub != null)
            {
                if (sub.IsGardifyPlusMonthly)
                {
                    userSub.AnnualStartDate = userSub.AnnualEndDate = DateTime.MinValue;
                    userSub.MonthlyStartDate = sub.MonthlyStartDate;
                    userSub.MonthlyEndDate = sub.MonthlyEndDate;
                }else if (sub.IsGardifyPlusAnnually)
                {
                    userSub.MonthlyEndDate = userSub.MonthlyStartDate = DateTime.MinValue;
                    userSub.AnnualStartDate = sub.AnnualStartDate;
                    userSub.MonthlyEndDate = sub.MonthlyEndDate;
                }
            }
            db.SaveChanges();
            return Ok();
        }
    }
}