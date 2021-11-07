using System;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PflanzenApp.App_Code;
using GardifyModels.Models;

namespace PflanzenApp.Controllers.AdminArea
{
    public class AdminAreaParentGardenCategoriesController : Controller
    {
        private ApplicationDbContext ctx = new ApplicationDbContext();

        // GET: AdminAreaParentGardenCategories
        public ActionResult Index()
        {
            return View("~/Views/AdminArea/AdminAreaParentGardenCategories/Index.cshtml",ctx.ParentGardenCategories.ToList());
        }

        // GET: AdminAreaParentGardenCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ParentGardenCategory parentGardenCategory = ctx.ParentGardenCategories.Find(id);
            if (parentGardenCategory == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/AdminArea/AdminAreaParentGardenCategories/Details.cshtml", parentGardenCategory);
        }

        // GET: AdminAreaParentGardenCategories/Create
        public ActionResult Create()
        {
            return View("~/Views/AdminArea/AdminAreaParentGardenCategories/Create.cshtml");
        }

        // POST: AdminAreaParentGardenCategories/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ParentGardenCategory parentGardenCategory)
        {
            ModelState.Remove("CreatedBy");
            if (ModelState.IsValid)
            {
                parentGardenCategory.OnCreate(Utilities.GetUserId().ToString());
                parentGardenCategory.CreatedDate = DateTime.Now;
                //parentGardenCategory.OnCreate("");
                ctx.ParentGardenCategories.Add(parentGardenCategory);
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }

            return View("~/Views/AdminArea/AdminAreaParentGardenCategories/Create.cshtml", parentGardenCategory);
        }

        // GET: AdminAreaParentGardenCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ParentGardenCategory parentGardenCategory = ctx.ParentGardenCategories.Find(id);
            if (parentGardenCategory == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/AdminArea/AdminAreaParentGardenCategories/Edit.cshtml", parentGardenCategory);
        }

        // POST: AdminAreaParentGardenCategories/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,CreatedBy,CreatedDate,EditedBy,EditedDate,Deleted")] ParentGardenCategory parentGardenCategory)
        {
            if (ModelState.IsValid)
            {
                ctx.Entry(parentGardenCategory).State = EntityState.Modified;
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("~/Views/AdminArea/AdminAreaParentGardenCategories/Edit.cshtml", parentGardenCategory);
        }

        // GET: AdminAreaParentGardenCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ParentGardenCategory parentGardenCategory = ctx.ParentGardenCategories.Find(id);
            if (parentGardenCategory == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/AdminArea/AdminAreaParentGardenCategories/Delete.cshtml", parentGardenCategory);
        }

        // POST: AdminAreaParentGardenCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ParentGardenCategory parentGardenCategory = ctx.ParentGardenCategories.Find(id);
            ctx.ParentGardenCategories.Remove(parentGardenCategory);
            ctx.SaveChanges();
            return RedirectToAction("~/Views/AdminArea/AdminAreaParentGardenCategories/Delete.cshtml", "Index");
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
