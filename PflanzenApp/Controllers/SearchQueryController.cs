using Microsoft.AspNet.Identity;
using PflanzenApp.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
{
    public class SearchQueryController : _BaseController
    {
        // GET: SearchQuery
        public ActionResult Index()
        {
			Guid userId = Utilities.GetUserId();			
			return PartialView("_SearchQueryList", DbGetSearchQueriesByUserId(userId));
		}

        // POST: SearchQuery/Create
        [HttpPost]
		[ValidateAntiForgeryToken]
        public ActionResult Create(string name)
        {
			Guid userId = Utilities.GetUserId();

			if (this.ControllerContext.HttpContext.Request.Cookies.AllKeys.Contains("filter"))
			{
				HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies["filter"];
				SearchQuery newQuery = new SearchQuery
				{
					CookieString = cookie.Value,
					CreatedBy = User.Identity.GetUserName(),
					SearchName = name,
					UserId = userId
				};
				DbCreateSearchQuery(newQuery);
			}
			return PartialView("_SearchQueryList", DbGetSearchQueriesByUserId(userId));
		}        

        // POST: SearchQuery/Delete/5
        [HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Delete(int id)
        {
			Guid userId = Utilities.GetUserId();
			DbDeleteSearchQuery(id, userId);
			return PartialView("_SearchQueryList", DbGetSearchQueriesByUserId(userId));
		}

		#region DB

		[NonAction]
		public IEnumerable<SearchQuery> DbGetSearchQueriesByUserId(Guid userId)
		{
			return (from q in ctx.SearchQueries
					where !q.Deleted && q.UserId == userId
					orderby q.CreatedDate descending
					select q);
		}

		[NonAction]
		public bool DbCreateSearchQuery(SearchQuery newQueryData)
		{
			int maxQueries = 5;

			var query_sel = (from q in ctx.SearchQueries
					where !q.Deleted && q.UserId == newQueryData.UserId
					orderby q.CreatedDate descending
					select q);

			if(query_sel != null && query_sel.Any())
			{
				// falls der letzte eintrag den gleichen inhalt hat - ignorieren
				if (query_sel.FirstOrDefault().CookieString == newQueryData.CookieString)
				{
					return false;
				}

				// nur 5 queries sind erlaubt
				if (query_sel.Count() >= maxQueries)
				{					
					// ältesten eintrag löschen
					SearchQuery lastQuery = query_sel.ToList().LastOrDefault();
					ctx.Entry(lastQuery).State = System.Data.Entity.EntityState.Deleted;
				}				
			}

			SearchQuery newQuery = new SearchQuery
			{
				CookieString = newQueryData.CookieString,
				SearchName = newQueryData.SearchName,
				UserId = newQueryData.UserId
			};

			newQuery.OnCreate(newQueryData.CreatedBy);
			ctx.SearchQueries.Add(newQuery);

			return ctx.SaveChanges() > 0 ? true : false;
		}

		[NonAction]
		public bool DbDeleteSearchQuery(int queryId, Guid userId)
		{
			var query_sel = (from s in ctx.SearchQueries
							 where !s.Deleted && s.Id == queryId && s.UserId == userId
							 select s);

			if(query_sel != null && query_sel.Any())
			{
				ctx.Entry(query_sel.FirstOrDefault()).State = System.Data.Entity.EntityState.Deleted;
				return ctx.SaveChanges() > 0 ? true : false;
			}
			return false;
		}
		#endregion
	}
}
