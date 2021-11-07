using Microsoft.AspNet.Identity;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GardifyWebAPI.App_Code;

namespace GardifyWebAPI.Controllers
{
	public class LastViewedController : _BaseController
	{

		// GET: LastViewed
		public ActionResult Index()
		{
			LastViewedViewModels.LastViewedIndexViewModel viewModel = new LastViewedViewModels.LastViewedIndexViewModel();
			Guid userId = Guid.Parse(User.Identity.GetUserId());
			IEnumerable<Plant> userViewed = DbGetLastViewedPlantsByUserId(userId);
			IEnumerable<Plant> otherViewed = DbGetLastViewedPlantsByOthers(userId);

			PlantSearchController psc = new PlantSearchController();

			viewModel.UserLastViewed = psc.plantsToPlantViewModels(userViewed, Utilities.GetAbsolutePath("~/"));
			viewModel.OtherLastViewed = psc.plantsToPlantViewModels(otherViewed, Utilities.GetAbsolutePath("~/"));
			return View(viewModel);
		}
		
		#region DB

		[NonAction]
		public bool CreateLastViewed(LastViewed newEntry)
		{
			int maxEntries = 10;
			var lastViewed_sel = (from l in plantDB.LastViewed
								  where !l.Deleted && l.UserId == newEntry.UserId
								  orderby l.EditedDate descending
								  select l);

			if (lastViewed_sel != null && lastViewed_sel.Any())
			{
				LastViewed sameId = lastViewed_sel.Where(q => q.PlantId == newEntry.PlantId).FirstOrDefault();
				// falls ein eintrag die gleiche plantid hat - editeddate auf Now() setzen
				if (sameId != null)
				{
					sameId.OnEdit(newEntry.EditedBy);
				}
				else
				{
					newEntry.OnCreate(newEntry.CreatedBy);
					plantDB.LastViewed.Add(newEntry);
				}

				// nur 5 queries sind erlaubt
				if (lastViewed_sel.Count() >= maxEntries)
				{
					// ältesten eintrag löschen
					LastViewed lastQuery = lastViewed_sel.LastOrDefault();
					plantDB.Entry(lastQuery).State = System.Data.Entity.EntityState.Deleted;
				}
			}
			else
			{
				newEntry.OnCreate(newEntry.CreatedBy);
				plantDB.LastViewed.Add(newEntry);
			}

			return plantDB.SaveChanges() > 0 ? true : false;
		}

		public IEnumerable<Plant> DbGetLastViewedPlantsByUserId(Guid userId)
		{
			return (from l in plantDB.LastViewed
					where !l.Deleted && l.UserId == userId
					join p in plantDB.Plants
					on l.PlantId equals p.Id
					orderby l.EditedDate descending
					select p);
		}

		public IEnumerable<Plant> DbGetLastViewedPlantsByOthers(Guid userId)
		{
			return (from l in plantDB.LastViewed					
					where !l.Deleted && !(from ul in plantDB.LastViewed where !ul.Deleted && ul.UserId == userId select ul.PlantId).Contains(l.PlantId)
					group l by l.PlantId into gl
					orderby gl.Count() descending
					join p in plantDB.Plants
					on gl.Key equals p.Id
					select p);
		}

		#endregion
	}
}
