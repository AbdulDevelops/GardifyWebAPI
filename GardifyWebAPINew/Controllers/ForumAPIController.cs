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
    [RoutePrefix("api/ForumAPI")]
    public class ForumAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ForumAPI
        public ForumViewModels.ForumIndexViewModel GetForumHeaders()
        {
            ForumController fc = new ForumController();
            return fc.Index(null);
        }

        // GET: api/ForumAPI/5
        [ResponseType(typeof(ForumHeader))]
        public ForumViewModels.ForumThreadViewModel GetForumHeader(int id)
        {
            ForumController fc = new ForumController();
            return fc.Thread(id);
        }

        // PUT: api/ForumAPI/5
        [ResponseType(typeof(void))]
        public ForumViewModels.ForumIndexViewModel PutForumHeader(int id)
        {
            ForumController fc = new ForumController();
            return fc.Index(1);
            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //if (id != forumHeader.Id)
            //{
            //    return BadRequest();
            //}

            //db.Entry(forumHeader).State = EntityState.Modified;

            //try
            //{
            //    db.SaveChanges();
            //}
            //catch (DbUpdateConcurrencyException)
            //{
            //    if (!ForumHeaderExists(id))
            //    {
            //        return NotFound();
            //    }
            //    else
            //    {
            //        throw;
            //    }
            //}

            //return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/ForumAPI
        [ResponseType(typeof(ForumHeader))]
        [HttpPost]
        public ForumViewModels.ForumThreadViewModel PostForumHeader(int plantId)
        {
            ForumController fc = new ForumController();
            return fc.PlantThread(plantId);

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //db.ForumHeaders.Add(forumHeader);
            //db.SaveChanges();

            //return CreatedAtRoute("DefaultApi", new { id = forumHeader.Id }, forumHeader);
        }

        [ResponseType(typeof(ForumHeader))]
        [Route("post")]
        public ForumViewModels.ForumThreadViewModel ForumPost(ForumViewModels.ForumPostViewModel newPostData, int plantId)
        {

            ForumController fc = new ForumController();
            return fc.CreatePlantPost(newPostData, plantId);

            //if (!ModelState.IsValid)
            //{
            //    return BadRequest(ModelState);
            //}

            //db.ForumHeaders.Add(forumHeader);
            //db.SaveChanges();

            //return CreatedAtRoute("DefaultApi", new { id = forumHeader.Id }, forumHeader);
        }

        // DELETE: api/ForumAPI/5
        [ResponseType(typeof(ForumHeader))]
        public ForumViewModels.ForumIndexViewModel DeleteForumHeader(int id)
        {
            //ForumHeader forumHeader = db.ForumHeaders.Find(id);
            //if (forumHeader == null)
            //{
            //    return NotFound();
            //}

            //db.ForumHeaders.Remove(forumHeader);
            //db.SaveChanges();

            //return Ok(forumHeader);

            ForumController fc = new ForumController();
            return fc.Index(1);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool ForumHeaderExists(int id)
        {
            return db.ForumHeaders.Count(e => e.Id == id) > 0;
        }
    }
}