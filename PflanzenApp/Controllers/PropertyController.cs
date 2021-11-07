using System;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using GardifyModels.Models;
using PflanzenApp.App_Code;
using static GardifyModels.Models.PropertyViewModels;

namespace PflanzenApp.Controllers
{
    public class PropertyController : _BaseController
    {

        // GET: Property/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Property property = ctx.Property.Find(id);
            if (property == null)
            {
                return HttpNotFound();
            }
            return View(property);
        }

        // GET: Property/Create
        public ActionResult Create()
        {
            var userId = Utilities.GetUserId();
            var propertyId = DbGetPropertyId(userId);
            if (propertyId != null)
            {
                return RedirectToAction("Index", "Garden");
            }
            return View(new PropertyCreateViewModel { Country = "Deutschland" });
        }

        // POST: Property/Create
        // To protect from overposting attacks, please enable the specific Property you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "street,zip,city,country")] PropertyCreateViewModel property)
        {
            if (ModelState.IsValid)
            {
                DbCreateProperty(property.Street, property.Zip, property.City, property.Country);
                return RedirectToAction("Index", "Garden", null);
            }

            return View(property);
        }

        // GET: Property/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("Create", "Property");
            }
            Property property = ctx.Property.Find(id);
            if (property == null)
            {
                return RedirectToAction("Create", "Property");
            }
            return View(new PropertyEditViewModel
            {
                Country = property.Country,
                City = property.City,
                Zip = property.Zip,
                Id = property.Id,
                Street = property.Street
            });
        }

        // POST: Property/Edit/5
        // To protect from overposting attacks, please enable the specific Property you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,street,zip,city,country")] PropertyEditViewModel property)
        {
            if (ModelState.IsValid)
            {
                DbEditProperty(property);
                return RedirectToAction("Index", "Garden");
            }
            return View(property);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ctx.Dispose();
            }
            base.Dispose(disposing);
        }

        #region DB
        [NonAction]
        private void DbEditProperty(PropertyEditViewModel property)
        {
            var prop = (from p in ctx.Property
                        where p.Id == property.Id
                        select p).FirstOrDefault();
            prop.Street = property.Street;
            prop.Zip = property.Zip;
            prop.City = property.City;
            prop.Country = property.Country;
            prop.EditedBy = Utilities.GetUserName();
            prop.EditedDate = DateTime.Now;
            prop.UpdateCoordinates();

            ctx.SaveChanges();

        }
        [NonAction]
        private void DbCreateProperty(string street, string zip, string city, string country)
        {
            Property newProp = new Property();
            newProp.City = city;
            newProp.Country = country;
            newProp.CreatedBy = Utilities.GetUserName();
            newProp.CreatedDate = DateTime.Now;
            newProp.Deleted = false;
            newProp.EditedBy = Utilities.GetUserName();
            newProp.EditedDate = DateTime.Now;
            newProp.Street = street;
            newProp.UserId = Utilities.GetUserId();
            newProp.Zip = zip;
            newProp.UpdateCoordinates();

            ctx.Property.Add(newProp);
            ctx.SaveChanges();

        }

        [NonAction]
        public int? DbGetPropertyId(Guid userId)
        {
            var property = ctx.Property.Where(ct => ct.UserId == userId
                                            && !ct.Deleted).FirstOrDefault();
            if (property == null)
            {
                return null;
            }
            return property.Id;
        }

        public Property DbGetProperty(Guid userId)
        {
            var property = ctx.Property.Where(ct => ct.UserId == userId
                                            && !ct.Deleted).FirstOrDefault();
            return property;
        }
        #endregion
    }
}
