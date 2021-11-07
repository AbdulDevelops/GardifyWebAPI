using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PflanzenApp.Controllers.AdminArea
{
    public class AdminAreaWarningsController : _BaseController
    {
        // GET: AdminAreaWarnings
        public ActionResult Index()
        {
            var pc = new PlantController();
            var ac = new AlertController();
            var plants = pc.DbGetPlantList();
            var plantsWithoutWarnings = 0;

            foreach (Plant p in plants)
            {
                p.Alerts = ac.DbGetAlertsByRelatedObjectId((int)p.Id, ModelEnums.ReferenceToModelClass.Plant);
                if (!p.Alerts.Any()) { plantsWithoutWarnings++; }
            }

            ViewBag.Count = plantsWithoutWarnings;

            return View("~/Views/AdminArea/AdminAreaWarnings/Index.cshtml", plants.ToList());
        }
    }
}
