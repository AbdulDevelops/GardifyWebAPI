using GardifyWebAPI.App_Code;
using GardifyModels.Models;
//using PflanzenApp.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using RestSharp;
using Newtonsoft.Json;

namespace GardifyWebAPI.Controllers
{
    public class NewsController : _BaseController
    {
        // GET: News
        public NewsViewModels.NewsListViewModel Index(int skip=0, int take = int.MaxValue)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            NewsViewModels.NewsListViewModel viewModel = new NewsViewModels.NewsListViewModel();
            viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;

            HelperClasses.DbResponse response = DbGetVisibleOnPageNewsEntries(skip, take);

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
                        Date = entry.Date,
                        Timing = entry.Timing,
                        Tipp = WebUtility.HtmlDecode(entry.Tipp),
                        SubTitle = entry.SubTitle,
                        Author = entry.Author,
                        Theme = entry.Theme
                    };

                    HelperClasses.DbResponse imageResponse = rc.DbGetNewsEntryReferencedImages(entry.Id);

                    entryView.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));

                    newsEntries.Add(entryView);
                }
            }

            viewModel.ListEntries = newsEntries;

            return viewModel;
        }

        public NewsViewModels.NewsEntryViewModel Details(int? id)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            if (id == null)
            {
                throw new Exception("Seite konnte nicht gefunden werden.");//, HttpStatusCode.NotFound, "NewsController.Details(" + id + ")");
            }

            NewsViewModels.NewsEntryViewModel viewModel = new NewsViewModels.NewsEntryViewModel();
            viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;

            HelperClasses.DbResponse response = DbGetNewsEntryById((int)id);

            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                NewsEntry entry = response.ResponseObjects.FirstOrDefault() as NewsEntry;
                viewModel.Id = entry.Id;
                viewModel.Title = entry.Title;
                viewModel.Text = WebUtility.HtmlDecode(entry.Text);
                viewModel.Date = entry.Date;
                viewModel.Tipp = WebUtility.HtmlDecode(entry.Tipp);
                viewModel.Author = entry.Author;
                viewModel.SubTitle = entry.SubTitle;
                viewModel.Timing = entry.Timing;
                viewModel.Theme = entry.Theme;

                HelperClasses.DbResponse imageResponse = rc.DbGetNewsEntryReferencedImages(entry.Id);

                viewModel.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
            }
            return viewModel;
        }


        #region DB

        [NonAction]
        public HelperClasses.DbResponse DbCreateNewsEntry(NewsEntry newEntryData)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            NewsEntry newEntry = newEntryData;

            newEntry.OnCreate(newEntryData.CreatedBy);

            plantDB.NewsEntry.Add(newEntry);

            bool isOk = plantDB.SaveChanges() > 0 ? true : false;

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

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-newsentry-image")]
        public ActionResult UploadNewsEntryImage(HttpPostedFileBase imageFile, int id, string imageTitle = null, string imageDescription = null)
        {
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();

            if (imageFile == null || imageFile.ContentLength <= 0)
            {
                statusMessage.Messages.Add("Fehler beim Upload");
                statusMessage.Status = ModelEnums.ActionStatus.Error;
                //return
            }
            else
            {
                UploadAndRegisterFile(imageFile, id, (int)ModelEnums.ReferenceToModelClass.NewsEntry, ModelEnums.FileReferenceType.NewsEntryImage, imageTitle, imageDescription);
            }

            TempData["statusMessage"] = statusMessage;

            return RedirectToAction("Edit", new { id = id });
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetAllNewsEntries(int skip = 0, int take = int.MaxValue)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var entries_sel = (from e in plantDB.NewsEntry
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
            var entries_sel = (from e in plantDB.NewsEntry
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


        public bool RefreshandSaveInstaPost()
        {
            var codeToken = "IGQVJYcG1pS1FXaUN2QlJldUtuMDF5MG1JQVVvNHM1WVhHeW90M1pPSFB2Skh5R3NpNnBtekJlNWVhRjhkaWp3Q3ZABOHFCN1B4VkVUY0ZACS0U2VklDNk8yTmlTaDhPbXFDbW1DdEFB";
            var client = new RestClient("https://graph.instagram.com/me/media?fields=id,caption,children,permalink,media_type,media_url,username,thumbnail_url,timestamp&access_token=" + codeToken);
            client.Timeout = -1;
            var request = new RestRequest(Method.GET);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);
            NewsViewModels.InstaNewsViewModel output = new NewsViewModels.InstaNewsViewModel();
            NewsController nc = new NewsController();
            var connectionStopped = false;

            if (response.IsSuccessful){           
                output = JsonConvert.DeserializeObject<NewsViewModels.InstaNewsViewModel>(response.Content);
            }
                if (output == null && output.data == null) {
                return false;
            }
            while (!(output.data == null ) && output.data.Any() && !(output.paging == null) && !String.IsNullOrEmpty(output.paging.next) && output.paging.next.Any() && !connectionStopped)
            {
                var clientNext = new RestClient(output.paging.next);
                clientNext.Timeout = -1;
                IRestResponse responseNext = clientNext.Execute(request);
                NewsViewModels.InstaNewsViewModel outputNext = new NewsViewModels.InstaNewsViewModel();
                outputNext = JsonConvert.DeserializeObject<NewsViewModels.InstaNewsViewModel>(responseNext.Content);
                if (outputNext?.data != null)
                {
                    output.data.AddRange(outputNext.data);
                    output.paging = outputNext.paging;

                }
                else
                {
                    connectionStopped = true;
                }
            }
            var instaPost = (from p in plantDB.InsatPostEntry where !p.Deleted select p).ToList();


            if (!(output.data == null) && output.data.Any()) { 
                foreach (var ins in output.data)
                {
                    InsatPostEntry instapost = new InsatPostEntry()
                    {
                        PostId = ins.id,
                        PermaLink = ins.permalink,
                        Caption = ins.caption,
                        MediaType = ins.media_type,
                        MediaUrl = ins.media_url,
                        Username = ins.username,
                        Timestamp = ins.timestamp,
                        ThumbnailUrl = ins.thumbnail_url,
                        ChildrenId = ins.children != null ? String.Join(",", ins.children.data.Select(d => d.id)) : null

                    };

                    if (!instaPost.Where(p => p.PostId == instapost.PostId).Any())
                    {
                        instapost.OnCreate("System");

                        plantDB.InsatPostEntry.Add(instapost);
                    }
                    else
                    {
                        //instaposteditraw(instapost);
                        var editedInsta = instaPost.Where(p => p.PostId == instapost.PostId).FirstOrDefault();
                        editedInsta.ChildrenId = instapost.ChildrenId;
                        editedInsta.MediaUrl = instapost.MediaUrl;
                        editedInsta.PermaLink = instapost.PermaLink;
                        editedInsta.ThumbnailUrl = instapost.ThumbnailUrl;
                        var res = nc.DbEditPostEntry(editedInsta);
                    }


                }
                plantDB.SaveChanges();

                return true;
            }
            return false;

        }
        public void GetAndSaveInstaPost()
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
            if (output == null)
            {
                return;
            }
            if (output.data == null)
            {
                return;
            }
            if (output.data.Any())
            {
                var clientNext = new RestClient(output.paging.next);
                clientNext.Timeout = -1;
                IRestResponse responseNext = clientNext.Execute(request);
                NewsViewModels.InstaNewsViewModel outputNext = new NewsViewModels.InstaNewsViewModel();
                outputNext = JsonConvert.DeserializeObject<NewsViewModels.InstaNewsViewModel>(responseNext.Content);
                if (outputNext?.data != null)
                {
                    output.data.AddRange(outputNext.data);

                }


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
                        plantDB.InsatPostEntry.Add(instapost);
                    }
                    else
                    {
                        instaposteditraw(instapost);
                    }

                }
                plantDB.SaveChanges();
            }
           
        }
        public bool instapostexist(string postId)
        {
            var instaPost = (from p in plantDB.InsatPostEntry where !p.Deleted && p.PostId.Equals(postId) select p).FirstOrDefault();
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

            var instaPost = (from p in plantDB.InsatPostEntry where !p.Deleted && p.PostId.Equals(entry.PostId) select p).FirstOrDefault();

            instaPost.ChildrenId = entry.ChildrenId;
            instaPost.MediaUrl = entry.MediaUrl;
            instaPost.ThumbnailUrl = entry.ThumbnailUrl;
            var res = nc.DbEditPostEntry(instaPost);

            return true;
        }
        public HelperClasses.DbResponse DbEditPostEntry(InsatPostEntry newData)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var entryToEdit_sel = (from e in plantDB.InsatPostEntry
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

                bool isOk = plantDB.SaveChanges() > 0 ? true : false;

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
        public NewsViewModels.InstaNewsViewModel GetInstaPosts(int skip = 0, int take = int.MaxValue)
        {
            //RefreshandSaveInstaPost();
            var allPost = (from p in plantDB.InsatPostEntry
                          where !p.Deleted
                          orderby p.Timestamp descending
                          select p).Skip(skip).Take(take);
            List<NewsViewModels.InstaNewsEntryListModel> instaPosts = new List<NewsViewModels.InstaNewsEntryListModel>();

            NewsViewModels.InstaNewsPagingModel dummyPage = new NewsViewModels.InstaNewsPagingModel { next = "", prev = "" };
            string pattern = @"#(\w+)";
            foreach (var post in allPost.ToList().Distinct())
            {

                NewsViewModels.InstaNewsEntryListModel vm = new NewsViewModels.InstaNewsEntryListModel()
                {
                    id = post.PostId,
                    caption = post.Caption != null ? Regex.Replace(WebUtility.HtmlDecode(post.Caption).Replace("(Link in Bio)", ""), pattern, ""): "",
                    captionRaw = post.Caption != null ? Regex.Replace(post.Caption.Replace("(Link in Bio)", ""), pattern, "") : "",
                    media_type = post.MediaType,
                    media_url = post.MediaUrl,
                    timestamp = post.Timestamp,
                    thumbnail_url = post.ThumbnailUrl,
                    username=post.Username,
                    childrenId = post.ChildrenId
                    
                };
                instaPosts.Add(vm);
            }

            return new NewsViewModels.InstaNewsViewModel { data = instaPosts, paging = dummyPage };
        }
        [NonAction]
        public HelperClasses.DbResponse DbGetNewsEntryById(int entryId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var entry_sel = (from e in plantDB.NewsEntry
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
        public HelperClasses.DbResponse DbEditNewsEntry(NewsEntry newData)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var entryToEdit_sel = (from e in plantDB.NewsEntry
                                   where !e.Deleted && e.Id == newData.Id
                                   select e);

            if (entryToEdit_sel != null && entryToEdit_sel.Any())
            {
                NewsEntry entryToEdit = entryToEdit_sel.FirstOrDefault();
                entryToEdit.Title = newData.Title;
                entryToEdit.Text = newData.Text;
                entryToEdit.IsVisibleOnPage = newData.IsVisibleOnPage;
                entryToEdit.Date = newData.Date;
                entryToEdit.OnEdit(newData.EditedBy);

                plantDB.Entry(entryToEdit).State = EntityState.Modified;

                bool isOk = plantDB.SaveChanges() > 0 ? true : false;

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

            var entry_sel = (from e in plantDB.NewsEntry
                             where !e.Deleted && e.Id == entryId
                             select e);

            if (entry_sel != null && entry_sel.Any())
            {
                entry_sel.FirstOrDefault().Deleted = true;
                entry_sel.FirstOrDefault().OnEdit(deletedBy);

                plantDB.Entry(entry_sel.FirstOrDefault()).State = EntityState.Modified;

                bool isOk = plantDB.SaveChanges() > 0 ? true : false;

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