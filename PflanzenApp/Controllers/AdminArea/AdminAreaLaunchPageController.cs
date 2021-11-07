using GardifyModels.Models;
using PflanzenApp.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PflanzenApp.Controllers.AdminArea
{
    [CustomAuthorizeAttribute(Roles = "Admin")]

    public class AdminAreaLaunchPageController : _BaseController
    {

        // GET: AdminAreaLaunchPage
        public ActionResult Index()
        {
            LaunchPageController lpc = new LaunchPageController();
            var launchContentList = lpc.getDbContent();

            LaunchPageViewModel vm = new LaunchPageViewModel
            {
                PageContents = launchContentList
            };

            return View("~/Views/AdminArea/AdminAreaLaunchPage/Index.cshtml", vm);
        }

        public ActionResult Create()
        {
            LaunchPageCreateModel lc = new LaunchPageCreateModel();

            var plantList = (from p in ctx.Plants
                             where !p.Deleted && p.Published
                             orderby p.NameGerman
                             select p);

            List<SelectListItem> plantViewList = plantList.Select(p => new SelectListItem
            {
                Text = p.NameGerman,
                Value = p.Id.ToString()
            }).ToList() ;

            ViewBag.PlantViewList = plantViewList;

            var newsList = (from p in ctx.NewsEntry
                             where !p.Deleted && p.IsVisibleOnPage
                             orderby p.Date descending
                             select p);

            List<SelectListItem> newsViewList = newsList.Select(n => new SelectListItem
            {
                Text = n.Theme,
                Value = n.Id.ToString()
            }).ToList();

            ViewBag.NewsViewList = newsViewList;




            return View("~/Views/AdminArea/AdminAreaLaunchPage/CreateLaunchEntry.cshtml", lc);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(LaunchPageCreateModel viewModel)
        {
            NewsController nc = new NewsController();
            LaunchPageController lpc = new LaunchPageController();
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            if (ModelState.IsValid && !string.IsNullOrEmpty(viewModel.PageContent))
            {

                LaunchPageContent newEntry = new LaunchPageContent
                {
                    PageContent = viewModel.PageContent
                };


                //HelperClasses.DbResponse response = nc.DbCreateNewsEntry(newEntry);

                ctx.LaunchPageContents.Add(newEntry);
                bool isOk = ctx.SaveChanges() > 0 ? true : false;


                if (isOk)
                {
        
                    return RedirectToAction("Index");
                }
                else
                {
                    //statusMessage.Status = ModelEnums.ActionStatus.Error;
                    //statusMessage.Messages.Add("Ein Fehler ist aufgetreten.");
                    //viewModel.StatusMessage = statusMessage;
                    return View("~/Views/AdminArea/AdminAreaLaunchPage/CreateLaunchEntry.cshtml", viewModel);

                }
            }
            //statusMessage.Status = ModelEnums.ActionStatus.Error;
            //statusMessage.Messages.Add("Bitte füllen Sie alle nötigen Angaben aus.");
            //viewModel.StatusMessage = statusMessage;
            return View("~/Views/AdminArea/AdminAreaLaunchPage/CreateLaunchEntry.cshtml", viewModel);

        }

        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return HttpNotFound();
            }

            var lc = (from e in ctx.LaunchPageContents
                             where !e.Deleted && e.Id == id
                             select e).FirstOrDefault();

            if (lc == null)
            {
                return HttpNotFound();

            }

            var plantList = (from p in ctx.Plants
                             where !p.Deleted && p.Published
                             orderby p.NameGerman
                             select p);

            List<SelectListItem> plantViewList = plantList.Select(p => new SelectListItem
            {
                Text = p.NameGerman,
                Value = p.Id.ToString()
            }).ToList();

            ViewBag.PlantViewList = plantViewList;

            var newsList = (from p in ctx.NewsEntry
                            where !p.Deleted && p.IsVisibleOnPage
                            orderby p.Date descending
                            select p);

            List<SelectListItem> newsViewList = newsList.Select(n => new SelectListItem
            {
                Text = n.Theme,
                Value = n.Id.ToString()
            }).ToList();

            ViewBag.NewsViewList = newsViewList;

            LaunchPageCreateModel viewModel = new LaunchPageCreateModel
            {
                PageContent = lc.PageContent
            };


            return View("~/Views/AdminArea/AdminAreaLaunchPage/edit.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(LaunchPageCreateModel viewModel)
        {
            NewsController nc = new NewsController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();


            var editEntry = (from e in ctx.LaunchPageContents
                                   where !e.Deleted && e.Id == viewModel.Id
                                   select e).FirstOrDefault();

            if (editEntry != null)
            {
                editEntry.PageContent = viewModel.PageContent;


                bool isOk = ctx.SaveChanges() > 0 ? true : false;

                return RedirectToAction("Index");

            }

            //HelperClasses.DbResponse response = nc.DbEditNewsEntry(newData);

            //_modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            //statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            //statusMessage.Status = response.Status;
            //viewModel.StatusMessage = statusMessage;

            //HelperClasses.DbResponse imageResponse = rc.DbGetNewsEntryReferencedImages(viewModel.Id);
            //viewModel.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
            //return View("~/Views/AdminArea/AdminAreaLaunchPage/Index.cshtml", vm);

            return View("~/Views/AdminArea/AdminAreaLaunchPage/edit.cshtml", viewModel);
        }
    }
}