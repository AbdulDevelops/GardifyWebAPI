﻿using Microsoft.AspNet.Identity;
using PflanzenApp.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RestSharp;
using Newtonsoft.Json;
using System.Net;
using System.Configuration;

namespace PflanzenApp.Controllers.AdminArea
{
    [CustomAuthorizeAttribute(Roles = "Admin")]
    public class AdminAreaNewsController : _BaseController
    {
        // GET: News
        public ActionResult Index()
        {
            NewsController nc = new NewsController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            NewsViewModels.NewsListViewModel viewModel = new NewsViewModels.NewsListViewModel();
            viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;

            HelperClasses.DbResponse response = nc.DbGetAllNewsEntries();

            List<NewsViewModels.NewsEntryViewModel> newsEntries = new List<NewsViewModels.NewsEntryViewModel>();

            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                foreach (NewsEntry entry in response.ResponseObjects)
                {
                    NewsViewModels.NewsEntryViewModel entryView = new NewsViewModels.NewsEntryViewModel
                    {
                        Id = entry.Id,
                        Title = entry.Title,
                        Text = WebUtility.HtmlDecode(entry.Text),
                        Date = entry.Date
                    };

                    HelperClasses.DbResponse imageResponse = rc.DbGetNewsEntryReferencedImages(entry.Id);

                    entryView.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));

                    newsEntries.Add(entryView);
                }
            }

            viewModel.ListEntries = newsEntries;

            return View("~/Views/AdminArea/AdminAreaNews/Index.cshtml", viewModel);
        }

        public ActionResult Create(NewsViewModels.NewsEntryViewModel viewModel = null)
        {
            if (viewModel == null || (viewModel.Date == DateTime.MinValue && viewModel.ValidFrom == DateTime.MinValue && viewModel.ValidTo == DateTime.MinValue))
            {
                viewModel = new NewsViewModels.NewsEntryViewModel();
                viewModel.Date = DateTime.Now;
                viewModel.ValidFrom = DateTime.Now;
                viewModel.ValidTo = DateTime.Now.AddYears(5);
            }
            return View("~/Views/AdminArea/AdminAreaNews/CreateNewsEntry.cshtml", viewModel);
        }

        // POST: news/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(NewsViewModels.NewsEntryViewModel viewModel, HttpPostedFileBase imageFile = null)
        {
            NewsController nc = new NewsController();
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            if (ModelState.IsValid && !string.IsNullOrEmpty(viewModel.Text))
            {
                NewsEntry newEntry = new NewsEntry
                {
                    Title = viewModel.Title,
                    SubTitle = viewModel.SubTitle,
                    Timing = viewModel.Timing,
                    Author = viewModel.Author,
                    Theme = viewModel.Theme,
                    Tipp = WebUtility.HtmlEncode(viewModel.Tipp),
                    Text = WebUtility.HtmlEncode(viewModel.Text),
                    ValidFrom = viewModel.ValidFrom,
                    ValidTo = viewModel.ValidTo,
                    CreatedBy = User.Identity.GetUserName(),
                    Date = DateTime.Now,
                    IsVisibleOnPage = viewModel.IsVisibleOnPage,
                    Deleted = false
                };

                HelperClasses.DbResponse response = nc.DbCreateNewsEntry(newEntry);

                if (response.Status == ModelEnums.ActionStatus.Success)
                {
                    statusMessage.Status = response.Status;
                    statusMessage.Messages.Add("Der Beitrag wurde erfolgreich erstellt. Sie können ihn jetzt bearbeiten");

                    if (imageFile != null)
                    {
                        bool isImageUploaded = UploadAndRegisterFile(imageFile, ((NewsEntry)response.ResponseObjects.FirstOrDefault()).Id, (int)ModelEnums.ReferenceToModelClass.NewsEntry, ModelEnums.FileReferenceType.NewsEntryImage, viewModel.Title);
                    }

                    TempData["statusMessage"] = statusMessage;
                    return RedirectToAction("Edit", new { id = ((NewsEntry)response.ResponseObjects.FirstOrDefault()).Id });
                }
                else
                {
                    statusMessage.Status = ModelEnums.ActionStatus.Error;
                    statusMessage.Messages.Add("Ein Fehler ist aufgetreten.");
                    viewModel.StatusMessage = statusMessage;
                }
            }
            statusMessage.Status = ModelEnums.ActionStatus.Error;
            statusMessage.Messages.Add("Bitte füllen Sie alle nötigen Angaben aus.");
            viewModel.StatusMessage = statusMessage;
            return View("~/Views/AdminArea/AdminAreaNews/CreateNewsEntry.cshtml", viewModel);

        }

        public ActionResult Edit(int? id)
        {
            NewsController nc = new NewsController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            if (id == null)
            {
                return HttpNotFound();
            }

            NewsViewModels.NewsEntryViewModel viewModel = new NewsViewModels.NewsEntryViewModel();
            viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;

            HelperClasses.DbResponse response = nc.DbGetNewsEntryById((int)id);

            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                NewsEntry entry = response.ResponseObjects.FirstOrDefault() as NewsEntry;
                viewModel.Id = entry.Id;
                viewModel.Title = entry.Title;
                viewModel.Text = WebUtility.HtmlDecode(entry.Text);
                viewModel.SubTitle = entry.SubTitle;
                viewModel.Timing = entry.Timing;
                viewModel.Author = entry.Author;
                viewModel.Theme = entry.Theme;
                viewModel.Tipp = WebUtility.HtmlDecode(entry.Tipp);
                viewModel.Date = entry.Date;
                viewModel.IsVisibleOnPage = entry.IsVisibleOnPage;
                viewModel.ValidFrom = entry.ValidFrom;
                viewModel.ValidTo = entry.ValidTo;

                HelperClasses.DbResponse imageResponse = rc.DbGetNewsEntryReferencedImages(entry.Id);

                viewModel.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
            }
            return View("~/Views/AdminArea/AdminAreaNews/EditNewsEntry.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(NewsViewModels.NewsEntryViewModel viewModel)
        {
            NewsController nc = new NewsController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            NewsEntry newData = new NewsEntry
            {
                Id = viewModel.Id,
                Date = viewModel.Date,
                IsVisibleOnPage = viewModel.IsVisibleOnPage,
                Text = WebUtility.HtmlEncode(viewModel.Text),
                SubTitle = viewModel.SubTitle,
                Timing = viewModel.Timing,
                Author = viewModel.Author,
                Theme = viewModel.Theme,
                Tipp = WebUtility.HtmlEncode(viewModel.Tipp),
                Title = viewModel.Title,
                ValidFrom = viewModel.ValidFrom,
                ValidTo = viewModel.ValidTo,
                EditedBy = User.Identity.GetUserName()
            };

            HelperClasses.DbResponse response = nc.DbEditNewsEntry(newData);

            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            viewModel.StatusMessage = statusMessage;

            HelperClasses.DbResponse imageResponse = rc.DbGetNewsEntryReferencedImages(viewModel.Id);
            viewModel.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));

            return View("~/Views/AdminArea/AdminAreaNews/EditNewsEntry.cshtml", viewModel);
        }

        [ActionName("delete-news-entry")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult deleteNewsEntry(int entryId)
        {
            NewsController nc = new NewsController();
            HelperClasses.DbResponse response = nc.DbDeleteNewsEntry(entryId, User.Identity.GetUserName());
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("edit-image-creds")]
        public ActionResult EditImageCreds(int imageId, int entryId, string imageAuthor = null, string imageLicense = null)
        {
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            try
            {
                nfilesEntities nfilesEntities = new nfilesEntities();
                var image = nfilesEntities.Files.FirstOrDefault(e => e.FileID == imageId);
                image.FileC = imageLicense;
                image.FileD = imageAuthor;
                nfilesEntities.SaveChanges();
                statusMessage.Messages = new string[] { "Bild geändert" }.ToList();
                statusMessage.Status = ModelEnums.ActionStatus.Success;
            }
            catch
            {
                statusMessage.Messages = new string[] { "Fehler beim Ändern" }.ToList();
                statusMessage.Status = ModelEnums.ActionStatus.Error;
            }
            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Edit", new { id = entryId });
        }

        [ActionName("delete-news-entry-image")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult deleteNewsEntryImage(int imageRefId, int entryId)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            HelperClasses.DbResponse response = rc.DbDeleteFileReference(imageRefId, User.Identity.GetUserName());
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Edit", new { id = entryId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-news-image")]
        public ActionResult UploadNewsImage(HttpPostedFileBase imageFile, int entryId, string imageTitle = null, string imageDescription = null, string imageLicense = null, string imageAuthor = null)
        {
            bool isOk = UploadAndRegisterFile(imageFile, entryId, (int)ModelEnums.ReferenceToModelClass.NewsEntry, ModelEnums.FileReferenceType.NewsEntryImage, imageTitle, imageDescription, imageLicense, imageAuthor);

            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();

            if (isOk)
            {
                statusMessage.Messages.Add("Das Bild wurde erfolgreich hochgeladen");
                statusMessage.Status = ModelEnums.ActionStatus.Success;
            }
            else
            {
                statusMessage.Messages.Add("Beim Hochgeladen ist ein Fehler aufgetreten");
                statusMessage.Status = ModelEnums.ActionStatus.Error;
            }

            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Edit", new { id = entryId });
        }
        //InstaPosts
        public ActionResult EditInstaPost(string id)
        {
            NewsController nc = new NewsController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            if (id == null)
            {
                return HttpNotFound();
            }

            NewsViewModels.InstaNewsEntryListModel viewModel = new NewsViewModels.InstaNewsEntryListModel();
            viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;

            HelperClasses.DbResponse response = nc.DbGetInstaPostEntryById(id);

            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                InsatPostEntry entry = response.ResponseObjects.FirstOrDefault() as InsatPostEntry;
                viewModel.id = entry.PostId;
                viewModel.caption = WebUtility.HtmlDecode(entry.Caption);
                viewModel.username = entry.Username;
                viewModel.timestamp = entry.Timestamp;
                viewModel.thumbnail_url = entry.ThumbnailUrl;
                viewModel.RelatedLink = entry.RelatedLink;
                viewModel.media_type = entry.MediaType;
                viewModel.media_url = entry.MediaUrl;
              
            }
            return View("~/Views/AdminArea/AdminAreaNews/EditInstaPostEntry.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditInstaPost(NewsViewModels.InstaNewsEntryListModel viewModel)
        {
            NewsController nc = new NewsController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            InsatPostEntry postData = new InsatPostEntry
            {
                PostId = viewModel.id,
                Timestamp = viewModel.timestamp,
                ThumbnailUrl = viewModel.thumbnail_url,
                Caption = WebUtility.HtmlEncode(viewModel.caption),
                RelatedLink = viewModel.RelatedLink,
                MediaType = viewModel.media_type,
                MediaUrl = viewModel.media_url,
                Username = viewModel.username,
                EditedBy = User.Identity.GetUserName()
            };

            HelperClasses.DbResponse response = nc.DbEditPostEntry(postData);

            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            viewModel.StatusMessage = statusMessage;

            return View("~/Views/AdminArea/AdminAreaNews/EditInstaPostEntry.cshtml", viewModel);
        }
        [ActionName("delete-instaPost-entry")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult deleteInstaPostEntry(string entryId)
        {
            NewsController nc = new NewsController();
            HelperClasses.DbResponse response = nc.DbDeleteInstaPostEntry(entryId, User.Identity.GetUserName());
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("InstaPosts");
        }

        

        public ActionResult GetInstaPost()
        {
            var codeToken = "IGQVJYcG1pS1FXaUN2QlJldUtuMDF5MG1JQVVvNHM1WVhHeW90M1pPSFB2Skh5R3NpNnBtekJlNWVhRjhkaWp3Q3ZABOHFCN1B4VkVUY0ZACS0U2VklDNk8yTmlTaDhPbXFDbW1DdEFB";

            var client = new RestClient("https://graph.instagram.com/me/media?fields=id,caption,children,media_type,media_url,username,thumbnail_url,timestamp&access_token=" + codeToken);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            NewsViewModels.InstaNewsViewModel output = new NewsViewModels.InstaNewsViewModel();
            NewsController nc = new NewsController();

            if (response.IsSuccessful)
            {
                output = JsonConvert.DeserializeObject<NewsViewModels.InstaNewsViewModel>(response.Content);


            }
            if ( output.data!=null && output.data.Any())
            {
                var clientNext = new RestClient(output.paging.next);
                clientNext.Timeout = -1;
                IRestResponse responseNext = clientNext.Execute(request);
                NewsViewModels.InstaNewsViewModel outputNext = new NewsViewModels.InstaNewsViewModel();
                outputNext = JsonConvert.DeserializeObject<NewsViewModels.InstaNewsViewModel>(responseNext.Content);

                output.data.AddRange(outputNext.data);

                foreach (var ins in output.data)
                {
                    InsatPostEntry instapost = new InsatPostEntry()
                    {
                        PostId = ins.id,
                        Caption = ins.caption,
                        MediaType = ins.media_type,
                        MediaUrl = ins.media_url,
                        Username = ins.username,
                        Timestamp = ins.timestamp,
                        ThumbnailUrl = ins.thumbnail_url,
                        ChildrenId = ins.children != null ? String.Join(",", ins.children.data.Select(d => d.id)) : null

                    };
                    if (!instapostexist(instapost.PostId))
                    {
                        ctx.InsatPostEntry.Add(instapost);
                    }
                    else
                    {
                        instaposteditraw(instapost);
                    }

                }
                ctx.SaveChanges();
            }
           
            output = nc.GetInstaPosts();
            return View("~/Views/AdminArea/AdminAreaNews/InstaPosts.cshtml", output);
        }
        public bool instapostexist(string postId)
        {
            var instaPost = (from p in ctx.InsatPostEntry where !p.Deleted && p.PostId.Equals(postId) select p).FirstOrDefault();
            if (instaPost != null)
            {
                return true;

            }
            else
            {
                return false;
            }
        }

        public bool instaposteditraw(InsatPostEntry entry)
        {
            NewsController nc = new NewsController();

            var instaPost = (from p in ctx.InsatPostEntry where !p.Deleted && p.PostId.Equals(entry.PostId) select p).FirstOrDefault();

            instaPost.ChildrenId = entry.ChildrenId;
            instaPost.MediaUrl = entry.MediaUrl;
            instaPost.ThumbnailUrl = entry.ThumbnailUrl;

            var res = nc.DbEditPostEntry(instaPost);

            return true;
        }
    }
   
}