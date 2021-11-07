using System.Web.Mvc;

namespace PflanzenApp.Controllers
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