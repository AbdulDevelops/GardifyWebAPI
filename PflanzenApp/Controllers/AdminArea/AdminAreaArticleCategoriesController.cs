using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GardifyModels.Models;
using PflanzenApp.App_Code;

namespace PflanzenApp.Controllers.AdminArea
{
    [CustomAuthorizeAttribute(Roles = "Admin,Expert")]
    public class AdminAreaArticleCategoriesController : _BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: AdminAreaArticleCategories
        public ActionResult Index()
        {
            return View("~/Views/AdminArea/AdminAreaArticleCategories/Index.cshtml", db.ArticleCategories.ToList());
        }

        // GET: AdminAreaArticleCategories/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArticleCategory articleCategory = db.ArticleCategories.Find(id);
            if (articleCategory == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/AdminArea/AdminAreaArticleCategories/Details.cshtml", articleCategory);
        }

        // GET: AdminAreaArticleCategories/Create
        public ActionResult Create()
        {
            return View("~/Views/AdminArea/AdminAreaArticleCategories/Create.cshtml", new ArticleCategory());
        }

        // POST: AdminAreaArticleCategories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Title,IsGiftIdea")] ArticleCategory articleCategory)
        {
            if (!String.IsNullOrEmpty(articleCategory.Title))
            {
                articleCategory.OnCreate(Utilities.GetUserName());
                db.ArticleCategories.Add(articleCategory);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View("~/Views/AdminArea/AdminAreaArticleCategories/Create.cshtml", articleCategory);
        }

        // GET: AdminAreaArticleCategories/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArticleCategory articleCategory = db.ArticleCategories.Find(id);
            if (articleCategory == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/AdminArea/AdminAreaArticleCategories/Edit.cshtml", articleCategory);
        }

        // POST: AdminAreaArticleCategories/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Title,IsGiftIdea")] ArticleCategory articleCategory)
        {
            if (!String.IsNullOrEmpty(articleCategory.Title))
            {
                var art = db.ArticleCategories.Where(a => a.Id == articleCategory.Id && !a.Deleted).FirstOrDefault();
                if (art != null)
                {
                    art.Title = articleCategory.Title;
                    art.IsGiftIdea = articleCategory.IsGiftIdea;
                }
                art.OnEdit(Utilities.GetUserName());
                
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("~/Views/AdminArea/AdminAreaArticleCategories/Edit.cshtml", articleCategory);
        }

        // GET: AdminAreaArticleCategories/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ArticleCategory articleCategory = db.ArticleCategories.Find(id);
            if (articleCategory == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/AdminArea/AdminAreaArticleCategories/Delete.cshtml", articleCategory);
        }

        // POST: AdminAreaArticleCategories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ArticleCategory articleCategory = db.ArticleCategories.Find(id);
            db.ArticleCategories.Remove(articleCategory);
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
