using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static GardifyModels.Models.GardenViewModels;
using System.Threading.Tasks;

namespace GardifyWebAPI.Controllers
{
    public class ProfileController : _BaseController
    {		
		// GET: Profile
		public async Task<ActionResult> Index()
        {
			string userId = User.Identity.GetUserId();
			ApplicationUser user = UserManager.FindById(userId);
			ProfileViewModels.ProfileViewModel viewModel = await getProfileViewModel(user);

			return View(viewModel);
        }

		public async Task<ActionResult> Public(string id)
		{

			if(String.IsNullOrEmpty(id))
			{
				IEnumerable<ApplicationUser> users = UserManager.Users.Where(u => !String.IsNullOrEmpty(u.ProfileUrl));
				ProfileViewModels.ProfileListViewModel listViewModel = new ProfileViewModels.ProfileListViewModel();

				foreach (ApplicationUser u in users)
				{
					ProfileViewModels.ProfileListViewModel.LinkHelper link = new ProfileViewModels.ProfileListViewModel.LinkHelper
					{
						UserName = u.UserName,
						UserPoints = u.Points,
						UserUrl = u.ProfileUrl
					};
					listViewModel.UserLinks.Add(link);
				}
				listViewModel.UserLinks = listViewModel.UserLinks.OrderByDescending(l => l.UserPoints).ToList();
				return View("PublicList", listViewModel);
			}

			ApplicationUser user = UserManager.Users.Where(u => u.ProfileUrl == id.ToLower()).FirstOrDefault();

			if(user == null)
			{
                throw new Exception("Seite konnte nicht gefunden werden.");//, HttpStatusCode.NotFound, "ProfileController.Public("+id+")");
            }

			ProfileViewModels.ProfileViewModel viewModel = await getProfileViewModel(user);

			return View(viewModel);
		}


		public async Task<ProfileViewModels.ProfileViewModel> getProfileViewModel(ApplicationUser user)
		{ 
			GardenController gc = new GardenController();

			ProfileViewModels.ProfileViewModel viewModel = new ProfileViewModels.ProfileViewModel();

			viewModel.UserName = user.UserName;
			viewModel.Points = user.Points;

            GardenIndexViewModel gardenViewModels = await gc.GetGardenIndexViewModel(Guid.Parse(user.Id));

			viewModel.Gardens = gardenViewModels;

			return viewModel;
		}
    }
}