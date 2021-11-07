using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GardifyWebAPI.Controllers
{
    public class HomeController : Controller
    {
        public async System.Threading.Tasks.Task<ActionResult> Index()
        {
            ViewBag.Title = "Home Page";
            //PlantSearchAPIController pc = new PlantSearchAPIController();
            //await pc.DummyFunctionAsync();
            return View();
        }
    }
}
