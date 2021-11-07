using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using Microsoft.AspNet.Identity;
using System.IO;
using static GardifyModels.Models.FaqViewModels;
using static GardifyModels.Models.HelperClasses;

namespace GardifyWebAPI.Controllers
{
    public class FaqController : _BaseController
    {
        // GET: Faq
        public FaqIndexViewModel Index()
        {
            FaqIndexViewModel vm = GetFaqIndexViewModel(Utilities.GetUserId());
            //Guid userId = new Guid("64535925-4fd4-4e51-9001-b0e19f776379");
            //FaqIndexViewModel vm = GetFaqIndexViewModel(userId);

            return vm;
        }

        public ActionResult Edit(int id)
        {
            FaqEditViewModel vm = GetFaqEditViewModel(id);
            return View(vm);
        }

        // GET: Faq/Create
        public IEnumerable<IReferencedObject> Create()
        {
            FaqCreateViewModel vm = new FaqCreateViewModel();
            IEnumerable<IReferencedObject> ios = DbGetFaqInfoObjects(Utilities.GetUserId());
            //vm.InfoObjects = ios;
            return ios;
        }



        // POST: Faq/Create
        // Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
        // finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public FaqIndexViewModel Create(FaqCreateViewModel vm, HttpPostedFileBase imageFile)
        {
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            CreateFaq(vm, imageFile);
            return Index();
        }

        #region DB

        private IEnumerable<IReferencedObject> DbGetFaqInfoObjects(Guid userId)
        {
            UserPlantController uc = new UserPlantController();
            var ups = uc.DbGetUserPlantsByUserId(userId);
            return ups;
        }

        public IEnumerable<FaqEntry> GetFaqEntrysForUser(Guid userId)
        {
            var faqs = DbGetAllFaqEntries();
            List<FaqEntry> list = new List<FaqEntry>();
            foreach (var faq in faqs.ResponseObjects)
            {
                FaqEntry fe = (FaqEntry)faq;
                if (fe.UserAllowsPublishment && fe.AdminAllowsPublishment)
                {
                    list.Add(fe);
                }
                else
                {
                    if (fe.QuestionAuthorId == userId)
                    {
                        list.Add(fe);
                    }
                }
            }
            return list;
        }

        private FaqIndexViewModel GetFaqIndexViewModel(Guid userId)
        {
            FaqIndexViewModel vm = new FaqIndexViewModel();

            var faqList = GetFaqEntrysForUser(userId);

            List<FaqDetailsViewModel> faqEntries = new List<FaqDetailsViewModel>();

            foreach (FaqEntry entry in faqList)
            {
                FaqDetailsViewModel entryView = GetFaqDetailsViewModel(entry.Id);

                faqEntries.Add(entryView);
            }

            vm.FaqList = faqEntries;
            return vm;
        }

        private FaqEditViewModel GetFaqEditViewModel(int faqId)
        {
            FaqEntry faq = DbGetFaqEntryById(faqId);
            FaqEditViewModel vm = new FaqEditViewModel
            {
                QuestionText = faq.QuestionText,
                AnswerText = faq.AnswerText,
                Id = faq.Id,
                ReferenceId = faq.ReferenceId,
                ReferenceType = faq.ReferenceType,
                UserAllowsPublishment = faq.UserAllowsPublishment,
                AdminAllowsPublishment = faq.AdminAllowsPublishment
            };
            return vm;
        }

        public FaqDetailsViewModel GetFaqDetailsViewModel(int id)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            FaqEntry entry = (FaqEntry)(DbGetFaqEntry(id).ResponseObjects.FirstOrDefault());
            List<FaqAnswerViewModel> entryAnswers = GetEntryAnswersById(entry.Id);
            FaqDetailsViewModel entryView = new FaqDetailsViewModel
            {
                Id = entry.Id,
                QuestionText = entry.QuestionText,
                AnswerText = entry.AnswerText,
                Date = entry.Date,
                ReferenceId = entry.ReferenceId,
                ReferenceType = entry.ReferenceType,
                IsOpen = entry.IsOpen,
                UserAllowsPublishment = entry.UserAllowsPublishment,
                AdminAllowsPublishment = entry.AdminAllowsPublishment,
                CreatedBy = entry.CreatedBy,
                Answers = entryAnswers
            };

            DbResponse imageResponse = rc.DbGetFaqEntryReferencedImages(entry.Id);
            string rootPath = HttpRuntime.AppDomainAppVirtualPath != "/" ? HttpRuntime.AppDomainAppVirtualPath + "/" : "/";

            entryView.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, rootPath);

            return entryView;
        }

        public void CreateFaq(FaqCreateViewModel vm, HttpPostedFileBase imageFile)
        {
            FaqEntry newEntry = new FaqEntry
            {
                QuestionAuthorId = Utilities.GetUserId(),
                QuestionText = vm.QuestionText,
                Date = DateTime.Now,
                IsOpen = true,
                UserAllowsPublishment = vm.UserAllowsPublishment,
                AdminAllowsPublishment = false,
                ReferenceId = vm.ReferenceId,
                ReferenceType = vm.ReferenceType
            };
            newEntry.OnCreate(Utilities.GetUserName());
            int id = DbCreateFaqEntry(newEntry);
            if (imageFile != null)
            {
                bool isImageUploaded = UploadAndRegisterFile(imageFile, id, (int)ModelEnums.ReferenceToModelClass.FaqEntry, ModelEnums.FileReferenceType.FaqEntryImage, fileTitle: imageFile.FileName, fileDescription: imageFile.FileName);
            }
        }

        [NonAction]
        public int DbCreateFaqEntry(FaqEntry newEntryData)
        {
            FaqEntry newEntry = newEntryData;
            newEntry.OnCreate(newEntryData.CreatedBy);
            plantDB.FaqEntries.Add(newEntry);
            plantDB.SaveChanges();

            return newEntry.Id;
        }

        [NonAction]
        public FaqEntry DbGetFaqEntryById(int id)
        {
            var faq = (from f in plantDB.FaqEntries
                       where f.Id == id
                       select f).SingleOrDefault();
            return faq;
        }

        [NonAction]
        public DbResponse DbGetAllFaqEntries(int skip = 0, int take = int.MaxValue)
        {
            DbResponse response = new DbResponse();

            var entries_sel = (from e in plantDB.FaqEntries
                               where !e.Deleted
                               orderby e.Date ascending
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
        public DbResponse DbGetVisibleOnPageFaqEntries(int skip = 0, int take = int.MaxValue)
        {
            return DbGetFaqEntries(false, skip, take);
        }

        [NonAction]
        public DbResponse DbGetFaqEntries(bool isOpen, int skip = 0, int take = int.MaxValue)
        {
            DbResponse response = new DbResponse();

            var entries_sel = (from e in plantDB.FaqEntries
                               where !e.Deleted && e.IsOpen == isOpen
                               orderby e.Date ascending
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
        public DbResponse DbGetFaqEntry(int entryId)
        {
            DbResponse response = new DbResponse();

            var entry_sel = (from e in plantDB.FaqEntries
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
        public DbResponse DbEditFaqEntry(FaqEntry newData)
        {
            DbResponse response = new DbResponse();

            var entryToEdit_sel = (from e in plantDB.FaqEntries
                                   where !e.Deleted && e.Id == newData.Id
                                   select e);

            if (entryToEdit_sel != null && entryToEdit_sel.Any())
            {
                FaqEntry entryToEdit = entryToEdit_sel.FirstOrDefault();
                entryToEdit.IsOpen = newData.IsOpen;
                entryToEdit.UserAllowsPublishment = newData.UserAllowsPublishment;
                entryToEdit.AdminAllowsPublishment = newData.AdminAllowsPublishment;
                entryToEdit.Date = newData.Date;
                entryToEdit.OnEdit(newData.EditedBy);
                entryToEdit.QuestionText = newData.QuestionText;
                entryToEdit.AnswerText = newData.AnswerText;
                entryToEdit.AnswerAuthorId = newData.AnswerAuthorId;

                plantDB.Entry(entryToEdit).State = System.Data.Entity.EntityState.Modified;

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
        public DbResponse DbDeleteFaqEntry(int entryId, string deletedBy)
        {
            DbResponse response = new DbResponse();

            var entryToEdit_sel = (from e in plantDB.FaqEntries
                                   where !e.Deleted && e.Id == entryId
                                   select e);

            if (entryToEdit_sel != null && entryToEdit_sel.Any())
            {
                FaqEntry entryToDelete = entryToEdit_sel.FirstOrDefault();
                entryToDelete.Deleted = true;

                plantDB.Entry(entryToDelete).State = System.Data.Entity.EntityState.Modified;

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

        public FaqIndexViewModel DbGetReferencedFaqList(ModelEnums.ReferenceToModelClass refType, int refId)
        {
            var faqs = DbGetVisibleOnPageFaqEntries().ResponseObjects;
            List<FaqDetailsViewModel> responseList = new List<FaqDetailsViewModel>();
            foreach (FaqEntry faq in faqs)
            {
                if (faq.ReferenceType == refType && faq.ReferenceId == refId)
                {
                    responseList.Add(GetFaqDetailsViewModel(faq.Id));
                }
            }
            FaqIndexViewModel vm = new FaqIndexViewModel();
            vm.FaqList = responseList;
            return vm;
        }

        #endregion

        #region FaqAnswers
        public List<FaqAnswerViewModel> GetEntryAnswersById(int id)
        {
            var list = plantDB.FaqAnswers.Where(a => a.FaqEntryId == id && !a.Deleted).ToList();
            var res = new List<FaqAnswerViewModel>();
            foreach (FaqAnswer ans in list)
            {
                var temp = new FaqAnswerViewModel()
                {
                    AnsweredBy = ans.CreatedBy,
                    AnswerText = ans.AnswerText,
                    Date = ans.CreatedDate,
                    Images = null
                };
                res.Add(temp);
            }
            return res;
        }

        public List<FaqAnswerViewModel> GetEntryAnswersByIdWithCount(int id)
        {
            var list = plantDB.FaqAnswers.Where(a => a.FaqEntryId == id && !a.Deleted).ToList();
            var res = new List<FaqAnswerViewModel>();
            foreach (FaqAnswer ans in list)
            {
                var temp = new FaqAnswerViewModel()
                {
                    AnsweredBy = ans.CreatedBy,
                    AnswerText = ans.AnswerText,
                    Date = ans.CreatedDate,
                    Images = null,
                    UserPostsCount = GetUserPostsCount(ans.AuthorId)
                };
                res.Add(temp);
            }
            return res; 
        }

        public int GetUserPostsCount(Guid userId)
        {
            int answererCount = plantDB.FaqEntries.Where(e => e.AnswerAuthorId.ToString().Equals(userId.ToString())
                                                      || e.QuestionAuthorId.ToString().Equals(userId.ToString())).Count();
            return answererCount;
        }

        #endregion

    }
}
