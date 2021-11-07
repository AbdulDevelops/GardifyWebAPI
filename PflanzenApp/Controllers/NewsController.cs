using PflanzenApp.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
{
    [CustomAuthorize]
    public class NewsController : _BaseController
    {
        // GET: News
        public ActionResult Index()
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            NewsViewModels.NewsListViewModel viewModel = new NewsViewModels.NewsListViewModel();
            viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;

            HelperClasses.DbResponse response = DbGetVisibleOnPageNewsEntries();

            List<NewsViewModels.NewsEntryViewModel> newsEntries = new List<NewsViewModels.NewsEntryViewModel>();

            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                foreach (NewsEntry entry in response.ResponseObjects)
                {
                    NewsViewModels.NewsEntryViewModel entryView = new NewsViewModels.NewsEntryViewModel
                    {
                        Id = entry.Id,
                        Title = entry.Title,
                        Text = entry.Text,
                        Date = entry.Date
                    };

                    HelperClasses.DbResponse imageResponse = rc.DbGetNewsEntryReferencedImages(entry.Id);

                    entryView.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));

                    newsEntries.Add(entryView);
                }
            }

            viewModel.ListEntries = newsEntries;

            return View(viewModel);
        }
        //get Instaposts
        public NewsViewModels.InstaNewsViewModel GetInstaPosts()
        {
            var allPost = from p in ctx.InsatPostEntry
                          where !p.Deleted orderby (p.Timestamp)
                          select
                                p;
            List<NewsViewModels.InstaNewsEntryListModel> instaPosts = new List<NewsViewModels.InstaNewsEntryListModel>();
            IEnumerable<InsatPostEntry> orderedPosts = allPost.OrderByDescending(post => post.Timestamp).ToList();
            foreach (var post in orderedPosts)
            {
                NewsViewModels.InstaNewsEntryListModel vm = new NewsViewModels.InstaNewsEntryListModel()
                {
                    id = post.PostId,
                    caption = WebUtility.HtmlDecode(post.Caption),
                    media_type = post.MediaType,
                    media_url = post.MediaUrl,
                    timestamp = post.Timestamp,
                    thumbnail_url = post.ThumbnailUrl,
                    username=post.Username
                };
                instaPosts.Add(vm);
            }
            
            return new NewsViewModels.InstaNewsViewModel { data=instaPosts};
        }
        public ActionResult Details(int? id)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            if (id == null)
            {
                return RedirectToError("Seite konnte nicht gefunden werden.", HttpStatusCode.NotFound, "NewsController.Details("+id+")");
            }

            NewsViewModels.NewsEntryViewModel viewModel = new NewsViewModels.NewsEntryViewModel();
            viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;

            HelperClasses.DbResponse response = DbGetNewsEntryById((int)id);

            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                NewsEntry entry = response.ResponseObjects.FirstOrDefault() as NewsEntry;
                viewModel.Id = entry.Id;
                viewModel.Title = entry.Title;
                viewModel.Text = entry.Text;
                viewModel.Date = entry.Date;

                HelperClasses.DbResponse imageResponse = rc.DbGetNewsEntryReferencedImages(entry.Id);

                viewModel.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
            }
            return View(viewModel);
        }


        #region DB

        [NonAction]
        public HelperClasses.DbResponse DbCreateNewsEntry(NewsEntry newEntryData)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            NewsEntry newEntry = newEntryData;
            newEntry.OnCreate(newEntryData.CreatedBy);

            ctx.NewsEntry.Add(newEntry);

            bool isOk = ctx.SaveChanges() > 0 ? true : false;

            if (isOk)
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Created);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(newEntry);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetAllNewsEntries(int skip = 0, int take = int.MaxValue)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var entries_sel = (from e in ctx.NewsEntry
                               where !e.Deleted
                               orderby e.Date descending
                               select e);

            if (entries_sel != null && entries_sel.Any())
            {
                response.ResponseObjects = entries_sel.Skip(skip).Take(take).ToList<object>();
                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.Status = ModelEnums.ActionStatus.Success;
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.EmptyResult);
                response.Status = ModelEnums.ActionStatus.Warning;
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetVisibleOnPageNewsEntries(int skip = 0, int take = int.MaxValue)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            DateTime now = DateTime.Now;
            var entries_sel = (from e in ctx.NewsEntry
                               where !e.Deleted && e.IsVisibleOnPage == true &&
                               e.ValidFrom <= now && e.ValidTo > now
                               orderby e.Date descending
                               select e);

            if (entries_sel != null && entries_sel.Any())
            {
                response.ResponseObjects = entries_sel.Skip(skip).Take(take).ToList<object>();
                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.Status = ModelEnums.ActionStatus.Success;
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.EmptyResult);
                response.Status = ModelEnums.ActionStatus.Warning;
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetNewsEntryById(int entryId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var entry_sel = (from e in ctx.NewsEntry
                             where !e.Deleted && e.Id == entryId
                             select e);

            if (entry_sel != null && entry_sel.Any())
            {
                response.ResponseObjects.Add(entry_sel.FirstOrDefault());
                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.Status = ModelEnums.ActionStatus.Success;
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }
        [NonAction]
        public HelperClasses.DbResponse DbGetInstaPostEntryById(string entryId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var entry_sel = (from e in ctx.InsatPostEntry
                             where !e.Deleted && e.PostId.Equals(entryId)
                             select e);

            if (entry_sel != null && entry_sel.Any())
            {
                response.ResponseObjects.Add(entry_sel.FirstOrDefault());
                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.Status = ModelEnums.ActionStatus.Success;
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbEditNewsEntry(NewsEntry newData)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var entryToEdit_sel = (from e in ctx.NewsEntry
                                   where !e.Deleted && e.Id == newData.Id
                                   select e);

            var entryToEdit = entryToEdit_sel.FirstOrDefault();
            if (entryToEdit != null)
            {
                entryToEdit.Title = newData.Title;
                entryToEdit.Text = newData.Text;
                entryToEdit.IsVisibleOnPage = newData.IsVisibleOnPage;
                entryToEdit.Date = newData.Date;
                entryToEdit.ValidFrom = newData.ValidFrom;
                entryToEdit.ValidTo = newData.ValidTo;
                entryToEdit.SubTitle = newData.SubTitle;
                entryToEdit.Timing = newData.Timing;
                entryToEdit.Author = newData.Author;
                entryToEdit.Theme = newData.Theme;
                entryToEdit.Tipp = newData.Tipp;
                entryToEdit.OnEdit(newData.EditedBy);

                bool isOk = ctx.SaveChanges() > 0 ? true : false;

                if (isOk)
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.Edited);
                    response.Status = ModelEnums.ActionStatus.Success;
                    response.ResponseObjects.Add(entryToEdit);
                }
                else
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                    response.Status = ModelEnums.ActionStatus.Error;
                }
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }
        [NonAction]
        public HelperClasses.DbResponse DbEditPostEntry(InsatPostEntry newData)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var entryToEdit_sel = (from e in ctx.InsatPostEntry
                                   where !e.Deleted && e.PostId.Equals(newData.PostId)
                                   select e);

            var entryToEdit = entryToEdit_sel.FirstOrDefault();
            if (entryToEdit != null)
            {
                entryToEdit.PostId = newData.PostId;
                entryToEdit.Timestamp = newData.Timestamp;
                entryToEdit.ThumbnailUrl = newData.ThumbnailUrl;
                entryToEdit.Caption = newData.Caption;
                entryToEdit.RelatedLink = newData.RelatedLink;
                entryToEdit.MediaType = newData.MediaType;
                 entryToEdit.MediaUrl = newData.MediaUrl;
                entryToEdit.Username = newData.Username;
                entryToEdit.OnEdit(newData.EditedBy);
                entryToEdit.ChildrenId = newData.ChildrenId;

                bool isOk = ctx.SaveChanges() > 0 ? true : false;

                if (isOk)
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.Edited);
                    response.Status = ModelEnums.ActionStatus.Success;
                    response.ResponseObjects.Add(entryToEdit);
                }
                else
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                    response.Status = ModelEnums.ActionStatus.Error;
                }
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }
        [NonAction]
        public HelperClasses.DbResponse DbDeleteNewsEntry(int entryId, string deletedBy)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var entry_sel = (from e in ctx.NewsEntry
                             where !e.Deleted && e.Id == entryId
                             select e);

            if (entry_sel != null && entry_sel.Any())
            {
                entry_sel.FirstOrDefault().Deleted = true;
                entry_sel.FirstOrDefault().OnEdit(deletedBy);

                ctx.Entry(entry_sel.FirstOrDefault()).State = EntityState.Modified;

                bool isOk = ctx.SaveChanges() > 0 ? true : false;

                if (isOk)
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.Deleted);
                    response.Status = ModelEnums.ActionStatus.Success;
                    response.ResponseObjects.Add(entry_sel.FirstOrDefault());
                }
                else
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                    response.Status = ModelEnums.ActionStatus.Error;
                }
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }
        [NonAction]
        public HelperClasses.DbResponse DbDeleteInstaPostEntry(string entryId, string deletedBy)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var entry_sel = (from e in ctx.InsatPostEntry
                             where !e.Deleted && e.PostId.Equals(entryId) 
                             select e);

            if (entry_sel != null && entry_sel.Any())
            {
                entry_sel.FirstOrDefault().Deleted = true;
                entry_sel.FirstOrDefault().OnEdit(deletedBy);

                ctx.Entry(entry_sel.FirstOrDefault()).State = EntityState.Modified;

                bool isOk = ctx.SaveChanges() > 0 ? true : false;

                if (isOk)
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.Deleted);
                    response.Status = ModelEnums.ActionStatus.Success;
                    response.ResponseObjects.Add(entry_sel.FirstOrDefault());
                }
                else
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                    response.Status = ModelEnums.ActionStatus.Error;
                }
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }
        #endregion

    }
}