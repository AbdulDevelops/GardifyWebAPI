using Microsoft.AspNet.Identity;
using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static GardifyModels.Models.FaqViewModels;

namespace GardifyWebAPI.Controllers.AdminArea
{
    //[CustomAuthorizeAttribute(Roles = "Admin")]
    public class AdminAreaFaqController : _BaseController
    {
        // GET: /intern/faq
        public ActionResult Index()
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            FaqController fc = new FaqController();
            FaqIndexViewModel viewModel = new FaqIndexViewModel();

            HelperClasses.DbResponse response = fc.DbGetAllFaqEntries();

            List<FaqDetailsViewModel> faqEntries = new List<FaqDetailsViewModel>();

            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                foreach (FaqEntry entry in response.ResponseObjects)
                {
                    FaqDetailsViewModel entryView = new FaqDetailsViewModel
                    {
                        Id = entry.Id,
                        QuestionText = entry.QuestionText,
                        AnswerText = entry.AnswerText,
                        Date = entry.Date,
                        ReferenceId = entry.ReferenceId,
                        ReferenceType = entry.ReferenceType,
                        UserAllowsPublishment = entry.UserAllowsPublishment,
                        AdminAllowsPublishment = entry.AdminAllowsPublishment,
                        IsOpen = entry.IsOpen
                    };

                    HelperClasses.DbResponse imageResponse = rc.DbGetFaqEntryReferencedImages(entry.Id);
                    entryView.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Server.MapPath("~/"));

                    faqEntries.Add(entryView);
                }
            }

            viewModel.FaqList = faqEntries;

            return View("~/Views/AdminArea/AdminAreaFaq/Index.cshtml", viewModel);
        }

        // Get intern/faq/Edit
        public ActionResult Edit(int id)
        {
            FaqController fc = new FaqController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            FaqEditViewModel viewModel = new FaqEditViewModel();

            HelperClasses.DbResponse response = fc.DbGetFaqEntry(id);
            if (response.Status == ModelEnums.ActionStatus.Success)
            {
                FaqEntry entryToEdit = response.ResponseObjects.FirstOrDefault() as FaqEntry;

                viewModel.Id = id;
                viewModel.QuestionText = entryToEdit.QuestionText;
                viewModel.AnswerText = entryToEdit.AnswerText;
                viewModel.UserAllowsPublishment = entryToEdit.UserAllowsPublishment;
                viewModel.AdminAllowsPublishment = entryToEdit.AdminAllowsPublishment;
                viewModel.ReferenceId = entryToEdit.ReferenceId;
                viewModel.ReferenceType = entryToEdit.ReferenceType;

                HelperClasses.DbResponse imageResponse = rc.DbGetFaqEntryReferencedImages(id);

                viewModel.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Server.MapPath("~/"));

            }
            else
            {
                _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
                statusMessage.Status = ModelEnums.ActionStatus.Error;
                statusMessage.Messages.Add("Ein Fehler ist aufgetreten");
            }

            return View("~/Views/AdminArea/AdminAreaFaq/Edit.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FaqEditViewModel vm)
        {
            FaqController fc = new FaqController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            FaqEditViewModel viewModel = vm;
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();

            if (!string.IsNullOrEmpty(vm.AnswerText))
            {
                HelperClasses.DbResponse response = fc.DbGetFaqEntry(vm.Id);
                if (response.Status == ModelEnums.ActionStatus.Success)
                {
                    FaqEntry entryToEdit = response.ResponseObjects.FirstOrDefault() as FaqEntry;

                    entryToEdit.QuestionText = vm.QuestionText;
                    entryToEdit.AnswerText = vm.AnswerText;
                    entryToEdit.AnswerAuthorId = Guid.Parse(User.Identity.GetUserId());
                    entryToEdit.IsOpen = false;
                    entryToEdit.AdminAllowsPublishment = vm.AdminAllowsPublishment;
                    entryToEdit.OnEdit(Utilities.GetUserName());
                    HelperClasses.DbResponse editResponse = fc.DbEditFaqEntry(entryToEdit);

                    if (editResponse.Status == ModelEnums.ActionStatus.Success)
                    {
                        statusMessage.Status = ModelEnums.ActionStatus.Success;
                        statusMessage.Messages.Add("Frage wurde erfolgreicht gespeichert");
                    }
                    else
                    {
                        statusMessage.Status = ModelEnums.ActionStatus.Error;
                        statusMessage.Messages.Add("Ein Fehler ist aufgetreten");
                        statusMessage.Messages.AddRange(Utilities.databaseMessagesToText(editResponse.Messages));
                    }

                    HelperClasses.DbResponse imageResponse = rc.DbGetFaqEntryReferencedImages(vm.Id);

                    viewModel.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Server.MapPath("~/"));
                }
                else
                {
                    statusMessage.Status = ModelEnums.ActionStatus.Error;
                    statusMessage.Messages.Add("Ein Fehler ist aufgetreten");
                }
            }
            else
            {
                statusMessage.Status = ModelEnums.ActionStatus.Error;
                statusMessage.Messages.Add("Bitte geben Sie eine Antwort ein");
            }
            return RedirectToAction("Index");
        }

    }
}