using PflanzenApp.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
{
    public class WatchListController : _BaseController
    {
        #region DB
        [NonAction]
        public bool DbCreateWatchlistEntry(WatchlistEntry newEntry)
        {
            // check existing entries
            var watchlistCheck_sel = (from w in ctx.WatchlistEntry
                                      where w.PlantId == newEntry.PlantId && w.UserId == newEntry.UserId
                                      select w);
            if (watchlistCheck_sel == null || !watchlistCheck_sel.Any())
            {
                newEntry.CreatedBy = Utilities.GetUserId().ToString();
                newEntry.EditedBy = Utilities.GetUserId().ToString();
                newEntry.CreatedDate = DateTime.Now;
                newEntry.EditedDate = DateTime.Now;
                ctx.WatchlistEntry.Add(newEntry);
                return ctx.SaveChanges() > 0 ? true : false;
            }

            return false;
        }

        [NonAction]
        public WatchlistEntry DbGetWatchlistEntryById(int id)
        {
            var wl_sel = (from w in ctx.WatchlistEntry where w.Id == id select w);
            if (wl_sel != null && wl_sel.Any())
            {
                return wl_sel.FirstOrDefault();
            }
            return null;
        }

        [NonAction]
        public IEnumerable<WatchlistEntry> DbGetWatchlisEntriesByUserId(Guid userId)
        {
            return (from w in ctx.WatchlistEntry where w.UserId == userId select w);
        }

        [NonAction]
        public bool DbDeleteWatchlistEntryById(int id, Guid userId)
        {
            var wl_sel = (from w in ctx.WatchlistEntry where w.Id == id && w.UserId == userId select w);
            if (wl_sel != null && wl_sel.Any())
            {
                ctx.WatchlistEntry.Remove(wl_sel.FirstOrDefault());
                return ctx.SaveChanges() > 0 ? true : false;
            }
            return false;
        }

        #endregion
    }
}