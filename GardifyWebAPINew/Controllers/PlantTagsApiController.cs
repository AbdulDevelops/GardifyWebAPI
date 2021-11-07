using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using GardifyModels.Models;

namespace GardifyWebAPI.Controllers
{
    public class PlantTagsApiController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/PlantTagsApi
        public IEnumerable<PlantTagSearchLite> GetPlantTags()
        {
            PlantTagController ptc = new PlantTagController();
            return ptc.DbGetPlantTagsSearchLite().ToList();
        }

        // GET: api/PlantTagsApi/5
        [ResponseType(typeof(IEnumerable<PlantTag>))]
        public IHttpActionResult GetPlantTag(int? id)
        {
            PlantTagController ptc = new PlantTagController();
            if (id == null)
            {
                return NotFound();
            }
            var tags = ptc.DbGetPlantTags().ToList();
            return Ok(tags.Where(p => p.PlantsWithThisTag.Count() > 0 && p.PlantsWithThisTag.Where(pl => pl.Id == (int)id).Count() > 0).Select(x => new { name = x.Title,x.CategoryId,x.Id }));
            
        }

        // GET: api/PlantTagsApi/cats
        [Route("api/PlantTagsApi/cats")]
        public IEnumerable<GardifyModels.Models.PlantTagCategory> GetTagCategories()
        {
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
            return ptcc.DbGetPlantTagCategoriesLite();
        }

        // PUT: api/PlantTagsApi/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutPlantTag(int id, PlantTag plantTag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != plantTag.Id)
            {
                return BadRequest();
            }

            db.Entry(plantTag).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PlantTagExists(id))
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

        // POST: api/PlantTagsApi
        [ResponseType(typeof(PlantTag))]
        public IHttpActionResult PostPlantTag(PlantTag plantTag)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.PlantTags.Add(plantTag);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = plantTag.Id }, plantTag);
        }

        // DELETE: api/PlantTagsApi/5
        [ResponseType(typeof(PlantTag))]
        public IHttpActionResult DeletePlantTag(int id)
        {
            PlantTag plantTag = db.PlantTags.Find(id);
            if (plantTag == null)
            {
                return NotFound();
            }

            db.PlantTags.Remove(plantTag);
            db.SaveChanges();

            return Ok(plantTag);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool PlantTagExists(int id)
        {
            return db.PlantTags.Count(e => e.Id == id) > 0;
        }
    }
}