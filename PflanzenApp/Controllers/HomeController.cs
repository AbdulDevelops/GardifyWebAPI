using GardifyModels.Models;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
{
    public class HomeController : _BaseController
	{
        public ActionResult Index()
        {
			ViewData.Model = new _BaseViewModel();
			if (User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Garden");
			} else
			{
				return RedirectToAction("Register", "Account");				
			}
		}

        public ActionResult Startpage()
        {
			PointController pc = new PointController();

			if (!User.Identity.IsAuthenticated)
			{
				return RedirectToAction("Index", "Garden");
			}

			HomeViewModels.StartPageViewModel viewModel = new HomeViewModels.StartPageViewModel();

			viewModel.bodyText = "Content!";

            return View(viewModel);
        }
    }
}