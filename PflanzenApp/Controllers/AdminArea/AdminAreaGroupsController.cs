using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PflanzenApp.App_Code;
using GardifyModels.Models;

namespace PflanzenApp.Controllers.AdminArea
{
    public class AdminAreaGroupsController : Controller
    {
        private ApplicationDbContext ctx = new ApplicationDbContext();

        // GET: AdminAreaGroups
        public ActionResult Index()
        {
            return View("~/Views/AdminArea/AdminAreaGroups/Index.cshtml", ctx.Groups.ToList());
        }

        // GET: AdminAreaGroups/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = ctx.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/AdminArea/AdminAreaGroups/Details.cshtml", group);
        }

        // GET: AdminAreaGroups/Create
        public ActionResult Create()
        {
            return View("~/Views/AdminArea/AdminAreaGroups/Create.cshtml");
        }

        // POST: AdminAreaGroups/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,CreatedBy,CreatedDate,EditedBy,EditedDate,Deleted")] Group group)
        {
            ModelState.Remove("CreatedBy");
            if (ModelState.IsValid)
            {
                group.OnCreate(Utilities.GetUserId().ToString());
                ctx.Groups.Add(group);
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }

            return View("~/Views/AdminArea/AdminAreaGroups/Create.cshtml", group);
        }

        // GET: AdminAreaGroups/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = ctx.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/AdminArea/AdminAreaGroups/Edit.cshtml", group);
        }

        // POST: AdminAreaGroups/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,CreatedBy,CreatedDate,EditedBy,EditedDate,Deleted")] Group group)
        {
            if (ModelState.IsValid)
            {
                ctx.Entry(group).State = EntityState.Modified;
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("~/Views/AdminArea/AdminAreaGroups/Edit.cshtml", group);
        }

        // GET: AdminAreaGroups/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Group group = ctx.Groups.Find(id);
            if (group == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/AdminArea/AdminAreaGroups/Delete.cshtml", group);
        }

        // POST: AdminAreaGroups/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Group group = ctx.Groups.Find(id);
            ctx.Groups.Remove(group);
            ctx.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ctx.Dispose();
            }
            base.Dispose(disposing);
        }

        [NonAction]
        public bool DbRemoveGroupFromPlant(int plantId, int groupdId, bool saveChanges = true)
        {
            var checkPlant_sel = (from p in ctx.Plants
                                  where p.Id == plantId && !p.Deleted
                                  select p);

            // if plant not exists or plant tag is not in plant tags list return false
            if (checkPlant_sel == null || !checkPlant_sel.Any())
            {
                return false;
            }
            if (checkPlant_sel.FirstOrDefault().PlantGroups != null &&
                (checkPlant_sel.FirstOrDefault().PlantGroups.Where(t => t.Id == groupdId) == null ||
                !checkPlant_sel.FirstOrDefault().PlantGroups.Where(t => t.Id == groupdId).Any()))
            {
                return false;
            }

            checkPlant_sel.FirstOrDefault().PlantGroups.Remove(checkPlant_sel.FirstOrDefault().PlantGroups.Where(t => t.Id == groupdId).FirstOrDefault());
            if (saveChanges)
            {
                return ctx.SaveChanges() > 0 ? true : false;
            }
            return false;
        }

        [NonAction]
        public bool DbAddGroupToPlant(int plantId, int groupId, bool saveChanges = true)
        {
            var checkPlant_sel = (from p in ctx.Plants
                                  where p.Id == plantId && !p.Deleted
                                  select p);

            // if plant not exists or plant tag already in plant tags list return false
            if (checkPlant_sel == null || !checkPlant_sel.Any())
            {
                return false;
            }

            if (checkPlant_sel.FirstOrDefault().PlantGroups != null &&
                (checkPlant_sel.FirstOrDefault().PlantGroups.Where(t => t.Id == groupId) != null &&
                checkPlant_sel.FirstOrDefault().PlantGroups.Where(t => t.Id == groupId).Any()))
            {
                return false;
            }

            // if tag not exits return false
            var checkPlantTag_sel = (from t in ctx.Groups
                                     where t.Id == groupId && !t.Deleted
                                     select t);

            if (checkPlantTag_sel == null || !checkPlantTag_sel.Any())
            {
                return false;
            }

            checkPlant_sel.FirstOrDefault().PlantGroups.Add(checkPlantTag_sel.FirstOrDefault());
            if (saveChanges == true)
            {
                return ctx.SaveChanges() > 0 ? true : false;
            }
            return false;
        }
    }
}
