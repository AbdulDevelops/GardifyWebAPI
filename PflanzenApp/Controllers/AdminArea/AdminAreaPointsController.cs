﻿using PflanzenApp.App_Code;
using GardifyModels.Models;
using System.Collections.Generic;
using System.Web.Mvc;

namespace PflanzenApp.Controllers.AdminArea
{
    [CustomAuthorizeAttribute(Roles = "Admin")]
	public class AdminAreaPointsController : Controller
    {
        // GET: AdminAreaPoints
        public ActionResult Index()
        {
            PointController poc = new PointController();
            IEnumerable<PointsPending> pointsPending = poc.DbGetPendingPoints();


            return View("~/Views/AdminArea/AdminAreaPoints/Index.cshtml", pointsPending);
        }

        [HttpGet]
        public ActionResult Approve(int index)
        {
            PointController poc = new PointController();
            poc.DbApprovePoints(index);
            return Redirect("/intern/points/");
        }

        [HttpGet]
        public ActionResult Decline(int index)
        {
            PointController poc = new PointController();
            poc.DbDeclinePoints(index); //TODO: Decline
            return Redirect("/intern/points/");
        }
    }
}