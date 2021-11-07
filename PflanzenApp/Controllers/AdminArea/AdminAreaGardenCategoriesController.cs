﻿using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PflanzenApp.App_Code;
using GardifyModels.Models;

namespace PflanzenApp.Controllers.AdminArea
{
    public class AdminAreaGardenCategoriesController : Controller
    {
        ApplicationDbContext ctx = new ApplicationDbContext();
        // GET: AdminAreaGardenCategories
        public ActionResult Index()
        {
            var gardenCategories = ctx.GardenCategories.Include(g => g.ParentCategory);
            return View("~/Views/AdminArea/AdminAreaGardenCategories/Index.cshtml", gardenCategories.ToList());
        }

        // GET: AdminAreaGardenCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GardenCategory gardenCategory = ctx.GardenCategories.Find(id);
            if (gardenCategory == null)
            {
                return HttpNotFound();
            }
            return View(gardenCategory);
        }

        // GET: AdminAreaGardenCategories/Create
        public ActionResult Create()
        {
            var selectList = new SelectList(ctx.ParentGardenCategories, "Id", "Name");
            ViewBag.ParentId = selectList;
            return View("~/Views/AdminArea/AdminAreaGardenCategories/Create.cshtml");
        }



        // POST: AdminAreaGardenCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,ParentId,CreatedBy,CreatedDate,EditedBy,EditedDate,Deleted")] GardenCategory gardenCategory)
        {
            ModelState.Remove("CreatedBy");
            if (ModelState.IsValid)
            {
                gardenCategory.OnCreate(Utilities.GetUserId().ToString());
                ctx.GardenCategories.Add(gardenCategory);
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.ParentId = new SelectList(ctx.ParentGardenCategories, "Id", "Name", gardenCategory.ParentId);
            return View("~/Views/AdminArea/AdminAreaGardenCategories/Create.cshtml", gardenCategory);
        }

        // GET: AdminAreaGardenCategories/Create
        public ActionResult CreateParent()
        {

            return View("~/Views/AdminArea/AdminAreaGardenCategories/CreateParent.cshtml", new ParentGardenCategory());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateParent([Bind(Include = "Name")] ParentGardenCategory parentGardenCategory)
        {
            ModelState.Remove("CreatedBy");
            if (ModelState.IsValid)
            {

                parentGardenCategory.OnCreate(Utilities.GetUserId().ToString());
                ctx.ParentGardenCategories.Add(parentGardenCategory);
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }


            return View("~/Views/AdminArea/AdminAreaGardenCategories/CreateParent.cshtml", parentGardenCategory);
        }


        // GET: AdminAreaGardenCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GardenCategory gardenCategory = ctx.GardenCategories.Find(id);
            if (gardenCategory == null)
            {
                return HttpNotFound();
            }
            ViewBag.ParentId = new SelectList(ctx.ParentGardenCategories, "Id", "Name", gardenCategory.ParentId);
            return View("~/Views/AdminArea/AdminAreaGardenCategories/Edit.cshtml", gardenCategory);
        }

        // POST: AdminAreaGardenCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,ParentId,CreatedBy,CreatedDate,EditedBy,EditedDate,Deleted")] GardenCategory gardenCategory)
        {
            if (ModelState.IsValid)
            {
                ctx.Entry(gardenCategory).State = EntityState.Modified;
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.ParentId = new SelectList(ctx.ParentGardenCategories, "Id", "Name", gardenCategory.ParentId);
            return View("~/Views/AdminArea/AdminAreaGardenCategories/Edit.cshtml", gardenCategory);
        }

        // GET: AdminAreaGardenCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            GardenCategory gardenCategory = ctx.GardenCategories.Find(id);
            if (gardenCategory == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/AdminArea/AdminAreaGardenCategories/Delete.cshtml", gardenCategory);
        }

        // POST: AdminAreaGardenCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            GardenCategory gardenCategory = ctx.GardenCategories.Find(id);
            ctx.GardenCategories.Remove(gardenCategory);
            ctx.SaveChanges();
            return RedirectToAction("~/Views/AdminArea/AdminAreaGardenCategories/Index.cshtml");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ctx.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
