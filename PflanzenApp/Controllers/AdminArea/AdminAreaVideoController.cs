using GardifyModels.Models;
using Microsoft.AspNet.Identity;
using PflanzenApp.App_Code;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PflanzenApp.Controllers.AdminArea
{
    [CustomAuthorizeAttribute(Roles = "Admin")]
    public class AdminAreaVideoController: _BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Events
        public ActionResult Index()
        {
            List<VideoEntryViewModel> viewModel = new List<VideoEntryViewModel>();

            var vids = db.VideoEntries.Where(ev => !ev.Deleted);

            if (vids != null && vids.Any())
            {
                foreach (VideoEntry vid in vids)
                {
                    VideoEntryViewModel eveView = new VideoEntryViewModel
                    {
                        Id = vid.Id,
                        Title = vid.Title,
                        Text = vid.Text,
                        SubTitle = vid.SubTitle,
                        YTLink = vid.YTLink
                    };

                    viewModel.Add(eveView);
                }
            }

            return View("~/Views/AdminArea/AdminAreaVideos/Index.cshtml", viewModel);
        }

        public ActionResult Create()
        {
            return View("~/Views/AdminArea/AdminAreaVideos/Create.cshtml", new VideoEntryViewModel());
        }

        // POST: events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(VideoEntryViewModel viewModel)
        {
            if (ModelState.IsValid && !string.IsNullOrEmpty(viewModel.Text))
            {
                var vidId = viewModel.YTLink.Split(new string[] { "?v=" }, StringSplitOptions.None)[1];
                var embedUrl = "https://www.youtube.com/embed/" + vidId;
                VideoEntry newEntry = new VideoEntry
                {
                    Title = viewModel.Title,
                    Text = viewModel.Text,
                    SubTitle = viewModel.SubTitle,
                    YTLink = embedUrl,
                    CreatedBy = User.Identity.GetUserName(),
                    Deleted = false
                };

                newEntry.OnCreate(User.Identity.GetUserName());
                db.VideoEntries.Add(newEntry);
                db.SaveChanges();
                viewModel.Id = newEntry.Id;
            }

            return View("~/Views/AdminArea/AdminAreaVideos/Edit.cshtml", viewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            VideoEntryViewModel viewModel = new VideoEntryViewModel();

            var eve = db.VideoEntries.Where(e => !e.Deleted && e.Id == id).FirstOrDefault();

            if (eve != null)
            {
                viewModel.Id = eve.Id;
                viewModel.Title = eve.Title;
                viewModel.SubTitle = eve.SubTitle;
                viewModel.Text = eve.Text;
                viewModel.YTLink = eve.YTLink;
            }
            return View("~/Views/AdminArea/AdminAreaVideos/Edit.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(VideoEntryViewModel viewModel)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            var eve = db.VideoEntries.Where(e => !e.Deleted && e.Id == viewModel.Id).FirstOrDefault();

            eve.Id = viewModel.Id;
            eve.Text = viewModel.SubTitle;
            eve.Title = viewModel.Title;
            eve.Text = viewModel.Text;
            eve.EditedBy = User.Identity.GetUserName();

            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            bool isOk = db.SaveChanges() > 0 ? true : false;
            if (isOk)
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Edited);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(eve);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                response.Status = ModelEnums.ActionStatus.Error;
            }
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            TempData["statusMessage"] = statusMessage;

            return View("~/Views/AdminArea/AdminAreaVideos/Edit.cshtml", viewModel);
        }

        [ActionName("delete-video-entry")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult deleteVideoEntry(int entryId)
        {
            var eve = db.VideoEntries.Where(ev => !ev.Deleted && ev.Id == entryId).FirstOrDefault();
            eve.Deleted = true;
            db.SaveChanges();
            return RedirectToAction("Index");
        }
    }
}