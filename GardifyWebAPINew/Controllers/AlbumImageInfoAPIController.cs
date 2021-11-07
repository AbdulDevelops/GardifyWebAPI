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
    public class AlbumImageInfoAPIController : ApiController
    {
        private AlbumImageInfoController aic = new AlbumImageInfoController();

        // GET: AlbumImageInfoAPI
        [Route("api/AlbumImageInfoAPI")]
        [HttpPost]
        public IHttpActionResult CreateEntry(AlbumImageInfoViewModel entry)
        {
            var userId = Utilities.GetUserId();

            if (userId == Guid.Empty)
                return BadRequest("Bitte melde dich zuerst an");
            if (!ModelState.IsValid)
                return BadRequest("Die Bewertung ist unvollständig");

            return Ok(aic.CreateEntry(entry, userId));
        }
    }

    public class AlbumImageInfoController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public AlbumImageInfo CreateEntry(AlbumImageInfoViewModel entry, Guid userId)
        {
            var image = new AlbumImageInfo()
            {
                UserId = userId,
                IsOwnImage = entry.IsOwnImage,
                ObjectId = entry.ObjectId,
            };

            var exist = db.AlbumImageInfos.Where(a => a.ObjectId == entry.ObjectId && a.UserId == userId).FirstOrDefault();

            if (exist != null)
            {
                exist.IsOwnImage = entry.IsOwnImage;
                exist.OnEdit(Utilities.GetUserName());

                image = exist;
            }
            else
            {
                image.OnCreate(Utilities.GetUserName());
                db.AlbumImageInfos.Add(image);

            }

            var res = db.SaveChanges();

            if (res <= 0)
                return null;

            return image;
        }

        public AlbumImageInfo GetEntry(int objectId, Guid userId)
        {
  

            var exist = db.AlbumImageInfos.Where(a => a.ObjectId == objectId && a.UserId == userId).FirstOrDefault();

            if (exist == null)
            {
                return null;
            }
       

            return exist;
        }
    }

}