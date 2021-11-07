using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace GardifyWebAPI.Controllers
{
    public class plantTagPropertyController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: plantTagProperty
        [HttpGet]
        [Route("api/plantTagProperty")]
        public IHttpActionResult Index()
        {

            var data = db.Database.SqlQuery<PlantSearchPropertyEntry>("select * from dbo.PlantSearchProperties").Select(p => new PlantSearchPropertyItem
            {
                plantId = p.id,
                nameGerman = p.nameGerman,
                nameLatin = p.nameLatin,
                Familie = p.Familie,
                Synonym = p.Synonym,
                TagProperty = p.TagProperty,
                HeightProperty = p.HeightProperty,
                BloomProperty = p.BloomProperty,
                GroupProperty = p.GroupProperty
            });
            var currentStorage = db.PlantSearchPropertyItems;

            db.PlantSearchPropertyItems.RemoveRange(currentStorage);
            db.PlantSearchPropertyItems.AddRange(data);

            var currentTemp = db.TempTableSearches;
            db.TempTableSearches.RemoveRange(currentTemp);
            db.SaveChanges();


            return Ok();
        }
    }
}