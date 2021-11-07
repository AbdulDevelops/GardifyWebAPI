using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GardifyModels.Models;

namespace GardifyWebAPI.Controllers
{
    public class InstaPostEntriesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: InsatPostEntries
        public ActionResult Index()
        {
            var dbResult = db.InsatPostEntry.Where(p => !p.Deleted).OrderByDescending(p => p.Timestamp).Take(18).ToList();
            return View(dbResult.Select(p => new InsatPostEntryView(p)));
        }

        // GET: InsatPostEntries/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InsatPostEntry insatPostEntry = db.InsatPostEntry.Find(id);
            if (insatPostEntry == null)
            {
                return HttpNotFound();
            }
            return View(insatPostEntry);
        }

        // GET: InsatPostEntries/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InsatPostEntries/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,PostId,Caption,MediaType,MediaUrl,Username,Timestamp,ThumbnailUrl,RelatedLink,ChildrenId,CreatedBy,CreatedDate,EditedBy,EditedDate,Deleted")] InsatPostEntry insatPostEntry)
        {
            if (ModelState.IsValid)
            {
                insatPostEntry.OnCreate("System");

                db.InsatPostEntry.Add(insatPostEntry);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(insatPostEntry);
        }

        // GET: InsatPostEntries/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InsatPostEntry insatPostEntry = db.InsatPostEntry.Find(id);
            if (insatPostEntry == null)
            {
                return HttpNotFound();
            }
            return View(insatPostEntry);
        }

        // POST: InsatPostEntries/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,PostId,Caption,MediaType,MediaUrl,Username,Timestamp,ThumbnailUrl,RelatedLink,ChildrenId,CreatedBy,CreatedDate,EditedBy,EditedDate,Deleted")] InsatPostEntry insatPostEntry)
        {
            if (ModelState.IsValid)
            {
                db.Entry(insatPostEntry).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(insatPostEntry);
        }

        // GET: InsatPostEntries/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InsatPostEntry insatPostEntry = db.InsatPostEntry.Find(id);
            if (insatPostEntry == null)
            {
                return HttpNotFound();
            }
            return View(insatPostEntry);
        }

        // POST: InsatPostEntries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InsatPostEntry insatPostEntry = db.InsatPostEntry.Find(id);
            db.InsatPostEntry.Remove(insatPostEntry);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
