using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace GardifyWebAPI.Controllers
{
    public class EventsAPIController: ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

        [HttpGet]
        [Route("api/EventsAPI")]
        public IEnumerable<EventViewModel> GetCurrentEvents()
        {
            var events = db.Events
                    .Where(ev => !ev.Deleted && ev.IsVisibleOnPage && ev.ValidFrom <= DateTime.Now && ev.ValidTo > DateTime.Now)
                    .OrderByDescending(ev => ev.Date)
                    .Select(ev => new EventViewModel()
                    {
                        Id = ev.Id,
                        Title = ev.Title,
                        Date = ev.Date,
                        Address = ev.Address,
                        Organizer = ev.Organizer,
                        Text = ev.Text
                    }).ToList();

            foreach (EventViewModel ev in events)
            {
                HelperClasses.DbResponse imageResponse = rc.DbGetEventReferencedImages(ev.Id);
                ev.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
            }

            return events;
        }

        [HttpGet]
        [Route("api/EventsAPI/{id}")]
        public EventViewModel GetEventById(int? id)
        {
            EventViewModel sel_event = db.Events
                    .Where(ev => !ev.Deleted && ev.IsVisibleOnPage && ev.Id == id)
                    .Select(ev => new EventViewModel()
                    {
                        Id = ev.Id,
                        Title = ev.Title,
                        Date = ev.Date,
                        Address = ev.Address,
                        Organizer = ev.Organizer,
                        Text = ev.Text
                    })
                    .FirstOrDefault();

            HelperClasses.DbResponse imageResponse = rc.DbGetEventReferencedImages(sel_event.Id);
            sel_event.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));

            return sel_event;
        }
    }
}