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
    public class ForumAPI2Controller : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/ForumAPI2
        public ForumViewModels.ForumIndexViewModel GetForumHeaders()
        {
            ForumController fc = new ForumController();
            return fc.Index(1);
        }

        // GET: api/ForumAPI2/5
        [ResponseType(typeof(ForumHeader))]
        public IHttpActionResult GetForumHeader(int id)
        {
            ForumHeader forumHeader = db.ForumHeaders.Find(id);
            if (forumHeader == null)
            {
                return NotFound();
            }

            return Ok(forumHeader);
        }

        // PUT: api/ForumAPI2/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutForumHeader(int id, ForumHeader forumHeader)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != forumHeader.Id)
            {
                return BadRequest();
            }

            db.Entry(forumHeader).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ForumHeaderExists(id))
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

        // POST: api/ForumAPI2
        [ResponseType(typeof(ForumHeader))]
        public IHttpActionResult PostForumHeader(ForumHeader forumHeader)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.ForumHeaders.Add(forumHeader);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = forumHeader.Id }, forumHeader);
        }

        // DELETE: api/ForumAPI2/5
        [ResponseType(typeof(ForumHeader))]
        public IHttpActionResult DeleteForumHeader(int id)
        {
            ForumHeader forumHeader = db.ForumHeaders.Find(id);
            if (forumHeader == null)
            {
                return NotFound();
            }

            db.ForumHeaders.Remove(forumHeader);
            db.SaveChanges();

            return Ok(forumHeader);
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