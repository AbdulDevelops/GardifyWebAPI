using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GardifyWebAPI.Controllers
{
    public class SocialController : _BaseController
    {
        // GET: Social
        public ActionResult Index()
        {
            return View();
        }

		public ActionResult UserProfile(string profileUrl)
		{

			return View();
		}


		#region DB



		#endregion
	}
}