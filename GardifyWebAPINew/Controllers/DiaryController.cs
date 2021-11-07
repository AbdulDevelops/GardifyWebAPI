using Microsoft.AspNet.Identity;
using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace GardifyWebAPI.Controllers
{
	public class DiaryController : _BaseController
    {
        private ImgResizerController imgResizer = new ImgResizerController();

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CreateEntry(DiaryViewModels.DiaryEntryViewModel entryData)
		{
			DiaryViewModels.DiaryEntryViewModel viewModel = entryData;

			_modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();

			if (ModelState.IsValid)
			{
				DiaryEntry newEntry = new DiaryEntry();
				newEntry.EntryObjectId = entryData.ObjectId;
				newEntry.Title = entryData.Title;
				newEntry.Description = entryData.Description;
				newEntry.Date = entryData.Date;
				newEntry.UserId = Utilities.GetUserId();
				newEntry.CreatedBy = User.Identity.GetUserName();
				newEntry.EntryOf = entryData.DiaryType;

				HelperClasses.DbResponse response = null;
				if (entryData.DiaryType == ModelEnums.ReferenceToModelClass.Garden || entryData.DiaryType == ModelEnums.ReferenceToModelClass.UserPlant)
				{
					response = DbCreateDiaryEntry(newEntry);
				} else
				{
					statusMessage.Messages.Add("DiaryType ist falsch");
					statusMessage.Status = response.Status;
					viewModel.StatusMessage = statusMessage;

					return View(viewModel);
				}

				if (response.Status == ModelEnums.ActionStatus.Success)
				{
					statusMessage.Messages.Add("Tagebucheintrag \"" + entryData.Title + "\" wurde erfolgreich erstellt. Sie können diesen Eintrag bearbeiten");
					statusMessage.Status = ModelEnums.ActionStatus.Success;

					viewModel.StatusMessage = statusMessage;
					return RedirectToAction("EditEntry", new { id = ((DiaryEntry)response.ResponseObjects.FirstOrDefault()).Id });
				}
				else
				{
					statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
					statusMessage.Status = response.Status;
					viewModel.StatusMessage = statusMessage;

					return View(viewModel);
				}
			}
			else
			{
				statusMessage.Messages.Add("Eingabe ist falsch");
				statusMessage.Status = ModelEnums.ActionStatus.Error;

				viewModel.StatusMessage = statusMessage;
				return View(viewModel);
			}
		}

		public ActionResult EditEntry(int id)
		{
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            Guid userId = Utilities.GetUserId();

			HelperClasses.DbResponse response = DbGetDiaryEntryById(id, userId);

			if (response.Status == ModelEnums.ActionStatus.Success)
			{
				DiaryEntry entryToEdit = (DiaryEntry)response.ResponseObjects.FirstOrDefault();

				DiaryViewModels.DiaryEntryViewModel viewModel = new DiaryViewModels.DiaryEntryViewModel {
					ObjectId = entryToEdit.EntryObjectId,
					Date = entryToEdit.Date,
					Title = entryToEdit.Title,
					Description = entryToEdit.Description,
					Id = entryToEdit.Id,
					DiaryType = entryToEdit.EntryOf
				};				

				HelperClasses.DbResponse imageResponse = rc.DbGetDiaryEntryReferencedImages(id);

                viewModel.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Server.MapPath("~/"));

                viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;

				return View(viewModel);
			}
			else
			{
                throw new Exception("Seite konnte nicht gefunden werden.");//, HttpStatusCode.NotFound, "DiaryController.EditEntry("+id+")");
            }
		}
		

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult EditEntry(DiaryViewModels.DiaryEntryViewModel entryData)
		{
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            Guid userId = Utilities.GetUserId();
			HelperClasses.DbResponse response = DbGetDiaryEntryById(entryData.Id, userId);

			if (response.Status == ModelEnums.ActionStatus.Success)
			{
				DiaryViewModels.DiaryEntryViewModel viewModel = entryData;

				_modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();

				if (ModelState.IsValid)
				{
					DiaryEntry entryToEdit = (DiaryEntry)response.ResponseObjects.FirstOrDefault();
					entryToEdit.Title = entryData.Title;
					entryToEdit.Description = entryData.Description;
					entryToEdit.Date = entryData.Date;
					entryToEdit.EditedBy = User.Identity.GetUserName();

					HelperClasses.DbResponse editResponse = DbEditDiaryEntry(entryToEdit);

					if (editResponse.Status == ModelEnums.ActionStatus.Success)
					{
						statusMessage.Messages.Add("Tagebucheintrag \"" + entryData.Title + "\" wurde erfolgreich verändert.");
						statusMessage.Status = ModelEnums.ActionStatus.Success;
						viewModel.StatusMessage = statusMessage;						
					}
					else
					{
						statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
						statusMessage.Status = response.Status;
						viewModel.StatusMessage = statusMessage;
					}
				}
				else
				{
					statusMessage.Messages.Add("Eingabe ist falsch");
					statusMessage.Status = ModelEnums.ActionStatus.Error;
					viewModel.StatusMessage = statusMessage;					
				}

				HelperClasses.DbResponse imageResponse = rc.DbGetDiaryEntryReferencedImages(entryData.Id);
                viewModel.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Server.MapPath("~/"));
                return View(viewModel);
			}
			else
			{
                throw new Exception("Seite konnte nicht gefunden werden.");//, HttpStatusCode.NotFound, "DiaryController.EditEntry("+entryData.Title+")");
            }
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("copy-diary-to-forum")]
		public ActionResult CopyDiaryEntryToForum(int diaryEntryId)
		{
			DiaryEntry entryToCopy = (from d in plantDB.DiaryEntry
									  where !d.Deleted && d.Id == diaryEntryId
									  select d).FirstOrDefault();

			if(entryToCopy != null)
			{
				Plant relatedPlant = (from up in plantDB.UserPlants
									  where !up.Deleted && up.Id == entryToCopy.EntryObjectId
									  join p in plantDB.Plants
									  on up.PlantId equals p.Id
									  where !p.Deleted
									  select p).FirstOrDefault();

				ForumHeader forumHeader = (from fh in plantDB.ForumHeaders									   
										   where !fh.Deleted && fh.RelatedObjectType == ModelEnums.ReferenceToModelClass.Plant && fh.RelatedObjectId == relatedPlant.Id
										   select fh).FirstOrDefault();

				if(forumHeader == null)
				{
					forumHeader = new ForumHeader
					{
						CreatedBy = "CREATED_BY_SYSTEM",
						RelatedObjectId = relatedPlant.Id,
						RelatedObjectType = ModelEnums.ReferenceToModelClass.Plant,
						IsThread = true,
						Title = "Thread für " + relatedPlant.NameGerman + " (" + relatedPlant.NameLatin + ")",
						AuthorId = Guid.Empty
					};
					forumHeader.OnCreate(forumHeader.CreatedBy);
					plantDB.ForumHeaders.Add(forumHeader);
					plantDB.SaveChanges();
				}

				ForumPost newPost = new ForumPost
				{
					Content = "<b>" + entryToCopy.Title + "</b><br/>" + entryToCopy.Description,
					AuthorId = entryToCopy.UserId,
					CreatedBy = entryToCopy.CreatedBy,
					ForumHeaderId = forumHeader.Id,
				};

				newPost.OnCreate(entryToCopy.CreatedBy);
				plantDB.ForumPost.Add(newPost);
				plantDB.SaveChanges();
				return RedirectToAction("Thread", "Forum", new { id = forumHeader.RelatedObjectId });
			}

            throw new Exception("Seite konnte nicht gefunden werden.");//, HttpStatusCode.NotFound, "DiaryController.CopyDiaryEntryToForum("+diaryEntryId+")");
        }

		[HttpPost]
		[ValidateAntiForgeryToken]
		[ActionName("upload-user-plant-diary-image")]
		public ActionResult UploadUserPlantDiaryImage(HttpPostedFileBase imageFile, HttpPostedFile imageFileSrc, int diaryEntryId, string imageTitle = null, string imageDescription = null)
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
                imgResizer.Upload(System.Web.Hosting.HostingEnvironment.MapPath("~/nfiles/DiaryImages/"), imageFileSrc);
                UploadAndRegisterFile(imageFile, diaryEntryId, (int)ModelEnums.ReferenceToModelClass.DiaryEntry, ModelEnums.FileReferenceType.DiaryEntryImage, imageTitle, imageDescription);
			}

			TempData["statusMessage"] = statusMessage;

			return RedirectToAction("EditEntry", new { id = diaryEntryId });
		}
		

		//[ActionName("delete-diary-entry-image")]
		//[HttpPost]
		//[ValidateAntiForgeryToken]
		//public ActionResult deleteDiaryEntryImage(int imageRefId, int entryId)
		//{
  //          ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
		//	HelperClasses.DbResponse response = rc.DbDeleteFile(imageRefId, User.Identity.GetUserName());
		//	_modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
		//	statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
		//	statusMessage.Status = response.Status;
		//	TempData["statusMessage"] = statusMessage;
		//	return RedirectToAction("EditEntry", new { id = entryId });
		//}

		public ActionResult DetailsEntry(int id)
		{
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            DiaryViewModels.DiaryEntryViewModel viewModel = new DiaryViewModels.DiaryEntryViewModel();
			Guid userId = Utilities.GetUserId();

			HelperClasses.DbResponse response = DbGetDiaryEntryById(id, userId);

			if (response.Status == ModelEnums.ActionStatus.Success)
			{
				if (response.ResponseObjects != null && response.ResponseObjects.Any())
				{
					DiaryEntry entry = (DiaryEntry)response.ResponseObjects.FirstOrDefault();

					viewModel.Id = entry.Id;
					viewModel.ObjectId = entry.EntryObjectId;
					viewModel.Title = entry.Title;
					viewModel.Description = entry.Description;
					viewModel.Date = entry.Date;

					HelperClasses.DbResponse imageResponse = rc.DbGetDiaryEntryReferencedImages(entry.Id);

                    viewModel.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Server.MapPath("~/"));
                }
			} else
			{
                throw new Exception("Seite konnte nicht gefunden werden.");//, HttpStatusCode.NotFound, "DiaryController.DetailsEntry("+id+")");
            }

			return View(viewModel);
		}

		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteEntry(int entryId, int objectId, ModelEnums.ReferenceToModelClass diaryType)
		{
			HelperClasses.DbResponse response = DbDeleteDiaryEntry(entryId, User.Identity.GetUserName());
			_modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
			statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
			statusMessage.Status = response.Status;
			TempData["statusMessage"] = statusMessage;

			if(diaryType == ModelEnums.ReferenceToModelClass.Garden)
			{
				return RedirectToAction("Details", "Garden", new { id = objectId });
			} else
			{
				return RedirectToAction("Details", "UserPlants", new { id = objectId });
			}
			
		}

        #region DB

        [NonAction]
        public HelperClasses.DbResponse DbCreateDiaryEntry(DiaryEntry entryData)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            DiaryEntry newEntry = new DiaryEntry();
            newEntry.Date = entryData.Date;
            newEntry.Title = entryData.Title;
            newEntry.Description = entryData.Description;
            newEntry.EntryOf = entryData.EntryOf;
            newEntry.EntryObjectId = entryData.EntryObjectId;
            newEntry.OnCreate(entryData.CreatedBy);
            newEntry.UserId = entryData.UserId;

            plantDB.DiaryEntry.Add(newEntry);

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

        [NonAction]
        public HelperClasses.DbResponse DbGetDiaryEntryById(int diaryEntryId, Guid userId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var entry_sel = (from d in plantDB.DiaryEntry where d.Id == diaryEntryId && d.UserId == userId && !d.Deleted select d);

            if (entry_sel != null && entry_sel.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(entry_sel.FirstOrDefault());
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetDiaryEntriesByObjectId(int objectId, Guid userId, int month, int year, ModelEnums.ReferenceToModelClass entryOf, int skip = 0, int take = int.MaxValue)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var entry_sel = (from d in plantDB.DiaryEntry
                             where d.EntryObjectId == objectId && d.EntryOf == entryOf && d.UserId == userId && !d.Deleted && d.Date.Month == month && d.Date.Year == year
                             orderby d.Date descending
                             select d);

            if (entry_sel != null && entry_sel.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.Status = ModelEnums.ActionStatus.Success;

                if (skip > 0 || take != 9999)
                {
                    response.ResponseObjects = entry_sel.Skip(skip).Take(take).ToList<object>();
                }
                else
                {
                    response.ResponseObjects = entry_sel.ToList<object>();
                }
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.EmptyResult);
                response.Status = ModelEnums.ActionStatus.Warning;
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbEditDiaryEntry(DiaryEntry entryData)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var entry_sel = (from d in plantDB.DiaryEntry where d.Id == entryData.Id && d.UserId == entryData.UserId && !d.Deleted select d);

            if (entry_sel != null && entry_sel.Any())
            {
                DiaryEntry entryToEdit = entry_sel.FirstOrDefault();

                entryToEdit.OnEdit(entryData.EditedBy);
                entryToEdit.Date = entryData.Date;
                entryToEdit.Title = entryData.Title;
                entryToEdit.Description = entryData.Description;

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
        public HelperClasses.DbResponse DbDeleteDiaryEntry(int entryId, string deletedBy)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var entry_sel = (from d in plantDB.DiaryEntry where d.Id == entryId && !d.Deleted select d);

            if (entry_sel != null && entry_sel.Any())
            {
                DiaryEntry entryToDelete = entry_sel.FirstOrDefault();

                entryToDelete.OnEdit(deletedBy);
                entryToDelete.Deleted = true;

                plantDB.Entry(entryToDelete).State = EntityState.Modified;
                bool isOk = plantDB.SaveChanges() > 0 ? true : false;

                if (isOk)
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.Deleted);
                    response.Status = ModelEnums.ActionStatus.Success;
                    response.ResponseObjects.Add(entryToDelete);
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
