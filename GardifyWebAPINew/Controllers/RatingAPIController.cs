using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using static GardifyModels.Models.ModelEnums;

namespace GardifyWebAPI.Controllers
{
    public class RatingAPIController : ApiController
    {
        private RatingController rc = new RatingController();

        [Route("api/ratingAPI/")]
        [HttpPost]
        public IHttpActionResult CreateEntry(RatingEntryViewModel entry)
        {
            var userId = Utilities.GetUserId();

            if (userId == Guid.Empty)
                return BadRequest("Bitte melde dich zuerst an");
            if (!ModelState.IsValid)
                return BadRequest("Die Bewertung ist unvollständig");

            return Ok(rc.CreateEntry(entry, userId));
        }

        [Route("api/ratingAPI/")]
        [HttpPut]
        public IHttpActionResult UpdateEntry(RatingEntryViewModel entry)
        {
            var userId = Utilities.GetUserId();

            return Ok(rc.UpdateEntry(entry, userId));
        }

        [Route("api/ratingAPI/singleRating")]
        [HttpPut]
        public IHttpActionResult UpdateSingleRatingEntry(RatingEntryViewModel entry)
        {
            var userId = Utilities.GetUserId();

            return Ok(rc.UpdateSingleRatingEntry(entry, userId));
        }

        [Route("api/ratingAPI/")]
        [HttpDelete]
        public IHttpActionResult DeleteEntry([FromBody]int entryId)
        {
            var userId = Utilities.GetUserId();

            return Ok(rc.DeleteEntry(entryId, userId));
        }
    }

    public class RatingController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public decimal GetAverageRating(int objectId, RatableObject objectType)
        {
            var ratings = db.RatingEntries.Where(e => e.ObjectId == objectId && e.ObjectType == objectType && !e.Deleted && e.Rating > 0).Select(e => (int)e.Rating);

            if (ratings != null && ratings.Any())
            {
                var avg = (decimal)ratings.ToList().Average();
                return Math.Round(avg, 1);
            }

            return 0;
        }

        public RatingEntry CreateEntry(RatingEntryViewModel entry, Guid userId)
        {
            var rating = new RatingEntry()
            {
                UserId = userId,
                Rating = entry.Rating,
                Comment = entry.Comment,
                ObjectId = entry.ObjectId,
                ObjectType = entry.ObjectType,
            };

            rating.OnCreate(Utilities.GetUserName());
            db.RatingEntries.Add(rating);
            var res = db.SaveChanges();

            if (res <= 0)
                return null;

            return rating;
        }

        public RatingEntry UpdateEntry(RatingEntryViewModel entry, Guid userId)
        {
            var rating = db.RatingEntries.Where(e => e.UserId == userId && e.Id == entry.Id && !e.Deleted).FirstOrDefault();
            if (rating == null)
                return null;

            rating.Comment = string.IsNullOrEmpty(entry.Comment) ? rating.Comment : entry.Comment;
            rating.Rating = entry.Rating;
            rating.OnEdit(Utilities.GetUserName());
            db.SaveChanges();

            return rating;
        }

        public RatingEntry UpdateSingleRatingEntry(RatingEntryViewModel entry, Guid userId)
        {
            var rating = db.RatingEntries.Where(e => e.UserId == userId && e.ObjectId == entry.ObjectId && !e.Deleted);
            if (!rating.Any())
                return CreateEntry(entry, userId);

            if (rating.Count() > 1)
            {
                foreach (var rat in rating.Skip(1))
                {
                    rat.Deleted = true;
                }
            }

            rating.FirstOrDefault().Comment = string.IsNullOrEmpty(entry.Comment) ? rating.FirstOrDefault().Comment : entry.Comment;
            rating.FirstOrDefault().Rating = entry.Rating;
            rating.FirstOrDefault().OnEdit(Utilities.GetUserName());
            db.SaveChanges();

            return rating.FirstOrDefault();
        }

        public RatingEntry DeleteEntry(int entryId, Guid userId)
        {
            var rating = db.RatingEntries.Where(e => e.UserId == userId && e.Id == entryId).FirstOrDefault();
            if (rating == null)
                return null;

            rating.Deleted = true;

            db.SaveChanges();
            return rating;
        }
    }
}
