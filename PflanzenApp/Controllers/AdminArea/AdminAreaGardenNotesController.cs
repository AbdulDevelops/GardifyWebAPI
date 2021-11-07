using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GardifyModels.Models;

namespace PflanzenApp.Controllers.AdminArea
{
    public class AdminAreaGardenNotesController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AdminAreaGardenNotes
        public ActionResult Index()
        {
            return View("~/Views/AdminArea/AdminAreaGardenNotes/Index.cshtml", db.GardenNotes.ToList());
        }

        // GET: AdminAreaGardenNotes/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GardenNote gardenNote = db.GardenNotes.Find(id);
            if (gardenNote == null)
            {
                return HttpNotFound();
            }
            return View(gardenNote);
        }

        // GET: AdminAreaGardenNotes/Create
        public ActionResult Create()
        {
            return View("~/Views/AdminArea/AdminAreaGardenNotes/Create.cshtml");
        }

        // POST: AdminAreaGardenNotes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(GardenNoteCreateModel gardenNote)
        {
            if (ModelState.IsValid)
            {

                GardenNote model = new GardenNote
                {
                    NoteTitle = gardenNote.NoteTitle
                };

                db.GardenNotes.Add(model);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View("~/Views/AdminArea/AdminAreaGardenNotes/Create.cshtml" , gardenNote);
        }

        // GET: AdminAreaGardenNotes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GardenNote gardenNote = db.GardenNotes.Find(id);
            if (gardenNote == null)
            {
                return HttpNotFound();
            }

            GardenNoteEditModel vm = new GardenNoteEditModel
            {
                Id = gardenNote.Id,
                NoteTitle = gardenNote.NoteTitle
            };
            return View("~/Views/AdminArea/AdminAreaGardenNotes/Edit.cshtml", vm);
        }

        // POST: AdminAreaGardenNotes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( GardenNoteEditModel gardenNote)
        {
            if (ModelState.IsValid)
            {
                var currentNote = db.GardenNotes.FirstOrDefault(n => n.Id == gardenNote.Id);
                if (currentNote == null)
                {
                    return View("~/Views/AdminArea/AdminAreaGardenNotes/Edit.cshtml", gardenNote);

                }
                currentNote.NoteTitle = gardenNote.NoteTitle;
                db.Entry(currentNote).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("~/Views/AdminArea/AdminAreaGardenNotes/Edit.cshtml", gardenNote);
        }

        // GET: AdminAreaGardenNotes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GardenNote gardenNote = db.GardenNotes.Find(id);
            if (gardenNote == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/AdminArea/AdminAreaGardenNotes/Delete.cshtml", gardenNote);
        }

        // POST: AdminAreaGardenNotes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GardenNote gardenNote = db.GardenNotes.Find(id);
            db.GardenNotes.Remove(gardenNote);
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
