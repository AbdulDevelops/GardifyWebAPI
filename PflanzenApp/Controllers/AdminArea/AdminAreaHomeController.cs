using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PflanzenApp.Controllers.AdminArea
{
    public class AdminAreaHomeController : Controller
    {
        // GET: AdminAreaHome
        public ActionResult Index()
        {
            return View("~/Views/AdminArea/AdminAreaHome/Index.cshtml");
        }
    }
}