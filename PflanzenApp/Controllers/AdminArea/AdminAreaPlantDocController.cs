using GardifyModels.Models;
using Microsoft.AspNet.Identity;
using PflanzenApp.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using GardifyWebAPI;
using PflanzenApp.Services;
using System.Threading.Tasks;
using System.Data.Entity.Validation;
using RazorEngine.Compilation.ImpromptuInterface.Optimization;
using System.Net;

namespace PflanzenApp.Controllers.AdminArea
{
    public class AdminAreaPlantDocController : _BaseController
    {
        ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
   
        // GET: AdminAreaPlantDoc
        public ActionResult Index()
        {


            AdminPlantDocListViewModel viewModel = new AdminPlantDocListViewModel()
            {
                ListEntries = GetPlantDocQuestions()
            };

            return View("~/Views/AdminArea/AdminAreaPlantDoc/Index.cshtml",viewModel);
        }

      public IEnumerable<AdminPlantDocViewModel> GetPlantDocQuestions()
        {
            List<AdminPlantDocViewModel> viewList = new List<AdminPlantDocViewModel>();
            var listOfQuestions = getAllQuestion();
            AdminPlantDocViewModel vm = null;
            if (listOfQuestions != null)
            {
                foreach (var l in listOfQuestions)
                {
                    vm = new AdminPlantDocViewModel()
                    {
                        AdminAllowsPublishment = l.AdminAllowsPublishment,
                        Description = l.Description,
                        Headline = l.Headline,
                        IsOwnFoto = l.IsOwnFoto,
                        QuestionText = l.QuestionText,
                        Thema = l.Thema,
                        QuestionId = l.Id,
                        UserAllowsPublishment = l.UserAllowsPublishment,
                        QuestionTextUpdate = l.QuestionTextUpdate,
                        DescriptionUpdate = l.DescriptionUpdate,
                        ThemaUpdate = l.ThemaUpdate,
                        isEdited = l.isEdited,
                        PublishWithImages = l.PublishWithImages,
                        Editdate = l.CreatedDate,
                    };
                    var user = ctx.Users.Where(u => !u.Deleted && u.Id == l.QuestionAuthorId.ToString()).FirstOrDefault();

                    if (user != null)
                    {
                        vm.AuthorName = user.UserName.Split('@')[0];
                    }
                    HelperClasses.DbResponse imageResponse = rc.DbGetPlantDocReferencedImages(l.Id);
                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                    {
                        vm.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                    }
                    else
                    {
                        vm.Images.Add(new _HtmlImageViewModel
                        {
                            SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                            Id = 0,
                            TitleAttr = "kein Bild Vorhanden"
                        });
                    }

                    viewList.Add(vm);
                }
            }
            return viewList.OrderByDescending(q => q.Editdate);
        }
       
        [ActionName("delete-answer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult deleteAnswerAsync(int id)
        {
            HelperClasses.DbResponse response;
            response = DbDeleteAnswer(id);
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            TempData["statusMessage"] = statusMessage;

            return RedirectToAction("Detail", new { questionId = ((PlantDocAnswer)response.ResponseObjects.FirstOrDefault()).PlantDocEntryId });
        }

        [ActionName("delete-question")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult deleteQuestionAsync(int id)
        {
            HelperClasses.DbResponse response = DbDeleteQuestion(id, User.Identity.GetUserName());
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            TempData["statusMessage"] = statusMessage;

            return RedirectToAction("Index");
        }

        [ActionName("sort-question-date-desc")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult sortQuestionByDatumDsc()
        {
            var list = GetPlantDocQuestions().OrderByDescending(o => o.Editdate).ToList();
            AdminPlantDocListViewModel viewModel = new AdminPlantDocListViewModel()
            {
                ListEntries = list
            };
            return View("~/Views/AdminArea/AdminAreaPlantDoc/Index.cshtml", viewModel);
        }
        [ActionName("sort-question-date-asc")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult sortQuestionByDatumAsc()
        {
            var list = GetPlantDocQuestions().OrderBy(o => o.Editdate).ToList();
            AdminPlantDocListViewModel viewModel = new AdminPlantDocListViewModel()
            {
                ListEntries = list
            };
            return View("~/Views/AdminArea/AdminAreaPlantDoc/Index.cshtml", viewModel);
        }
        public HelperClasses.DbResponse DbDeleteQuestion(int questionId, string userName)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            var question = getQuestionById(questionId);
            if (question != null)
            {
                question.Deleted = true;
                bool isOk = ctx.SaveChanges() > 0 ? true : false;

                if (isOk)
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.Deleted);
                    response.Status = ModelEnums.ActionStatus.Success;
                    response.ResponseObjects.Add(question);
                }
                else
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                    response.Status = ModelEnums.ActionStatus.Error;
                }
            }
            return response;
        }
        public HelperClasses.DbResponse DbDeleteAnswer(int answerId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            var answer = getAnswerById(answerId);
            if (answer != null)
            {
                answer.Deleted = true;
                bool isOk = ctx.SaveChanges() > 0 ? true : false;

                if (isOk)
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.Deleted);
                    response.Status = ModelEnums.ActionStatus.Success;
                    response.ResponseObjects.Add(answer);
                }
                else
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                    response.Status = ModelEnums.ActionStatus.Error;
                }
            }
            return response;
        }
        public PlantDocAnswer getAnswerById(int answerId)
        {
            var answer = ctx.PlantDocAnswers.Where(p => p.Id == answerId && !p.Deleted).FirstOrDefault();
            return answer;
        }
        public PlantDoc getQuestionById(int questionId)
        {
            var question = ctx.PlantDocs.Where(p => p.Id == questionId && !p.Deleted).FirstOrDefault();
            return question;
        }
        public ActionResult SetPostPublishedState(int id, bool published)
        {
            var question = ctx.PlantDocs.FirstOrDefault(p => !p.Deleted && p.Id == id);
            question.AdminAllowsPublishment = published;
            ctx.SaveChanges();
            return RedirectToAction("Index");
        }
        public ActionResult SetImagesPublishedState(int id, bool published)
        {
            var question = ctx.PlantDocs.FirstOrDefault(p => !p.Deleted && p.Id == id);
            question.PublishWithImages = published;
            ctx.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("addAnswer")]
        [ValidateInput(false)]

        public async Task<ActionResult> AddAnswerAsync(int questionId, string newAnswer, string thema, HttpPostedFileBase imageFile=null, string imageTitle = null, string imageDescription = null, string imageLicense = null, string imageAuthor = null)
        {
            PlantDoc question= new PlantDoc();
            if (!String.IsNullOrEmpty(thema))
            {
                question = ctx.PlantDocs.Where(q => q.Id == questionId && !q.Deleted).FirstOrDefault();
                question.Thema = thema;
            }

            PlantDocAnswer an = new PlantDocAnswer()
            {
                AnswerText = WebUtility.HtmlEncode(newAnswer),
                AuthorId = Utilities.GetUserId(),
                PlantDocEntryId = questionId
            };
            an.OnCreate(Utilities.GetUserName());
            PlantDocAnswer answer = ctx.PlantDocAnswers.Add(an);
            var res = ctx.SaveChanges();

            if (res > 0)
            {
                CreateStatEntry(StatisticEventTypes.SubmitAnswer, Utilities.GetUserId());
                UploadAnswerImage(imageFile, answer.Id, imageTitle, imageDescription, imageLicense, imageAuthor);
                TemplateService ts = new TemplateService();
                EmailSender es = new EmailSender(ctx);
                Guid userId = question.QuestionAuthorId;
                ApplicationUser user = await UserManager.FindByIdAsync(userId.ToString());
                try
                {
                    string content = ts.RenderTemplateAsync("PostAnswer", new { UserName = user.FirstName.Contains("Platzhalter") ? user.UserName.Split('@')[0] : user.FirstName });
          
                    var emailSent = await es.SendEmail("Antwort", content, "info@gardify.de", user.Email, null);

                }
                catch (DbEntityValidationException e) { throw; };
            }
            
            return RedirectToAction("Detail", new { questionId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("updateAnswer")]
        [ValidateInput(false)]

        public ActionResult UpdateAnswer(int answerId, string newAnswer)
        {
            PlantDocAnswer answerEntry = getAnswerById(answerId);
            if (newAnswer == null)
            {
                return RedirectToAction("Detail", new { questionId = answerEntry.PlantDocEntryId });
            }
            

            answerEntry.UpdatedAnswer = WebUtility.HtmlEncode(newAnswer);
            answerEntry.IsEdited = true;
            ctx.SaveChanges();
            return RedirectToAction("Detail", new { questionId = answerEntry.PlantDocEntryId });
        }
        public IEnumerable<PlantDoc> getAllQuestion()
        {
            var listOfQuestions = ctx.PlantDocs.Where(p => !p.Deleted).ToList();
            return listOfQuestions;
        }
        public ActionResult Detail(int questionId)
        {
            PlantDocDetailViewModel pdVm = new PlantDocDetailViewModel();
            var question = getQuestionById(questionId);

            if (question != null)
            {
                PlantDocViewModel plantDocView = new PlantDocViewModel()
                {
                    QuestionText = question.QuestionText,
                    UserAllowsPublishment = question.UserAllowsPublishment,
                    AdminAllowsPublishment = question.AdminAllowsPublishment,
                    IsOwnFoto = question.IsOwnFoto,
                    isEdited = question.isEdited,
                    Headline = question.Headline,
                    Thema = question.Thema,
                    Description = question.Description,
                    PublishDate = question.CreatedDate,
                    QuestionId = question.Id
                };
                HelperClasses.DbResponse imageResponse = rc.DbGetPlantDocReferencedImages(question.Id);
                if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                {
                    plantDocView.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                    plantDocView.mainImg = plantDocView.Images.Where(i => i.IsMainImg).FirstOrDefault();
                    
                }
                else
                {
                    plantDocView.Images.Add(new _HtmlImageViewModel
                    {
                        SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                        Id = 0,
                        TitleAttr = "kein Bild Vorhanden"
                    });
                }
                pdVm = new PlantDocDetailViewModel()
                {
                    PlantDocViewModel = plantDocView
                };
                pdVm.PlantDocAnswerList = getAllAnswerByPlantDocId(question.Id);
            }
            return View("~/Views/AdminArea/AdminAreaPlantDoc/Detail.cshtml", pdVm);
        }
        [ActionName("set-main-image")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult setImgAsMain(int imageRefId, int questionId)
        {
            
            rc.DbSetOtherImgAsNotMain(questionId, (int)ModelEnums.ReferenceToModelClass.PlantDocQuestion);
            rc.DbSetFileAsMain(imageRefId, User.Identity.GetUserName());
            return RedirectToAction("Detail", new { questionId });
        }
        [ActionName("delete-question-entry-image")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult deleteQuestionImage(int imageRefId, int questionId)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            HelperClasses.DbResponse response = rc.DbDeleteFileReference(imageRefId, User.Identity.GetUserName());
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Detail", new { questionId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("updateTheme")]
        public ActionResult updateTheme(int questionId, string thema)
        {
            var q = getQuestionById(questionId);

            if (q != null && !string.IsNullOrEmpty(thema))
            {
                q.Thema = thema;
            }

            ctx.SaveChanges();

            return RedirectToAction("Detail", new { questionId });
        }

        public IEnumerable<PlantDocAnswerViewModel> getAllAnswerByPlantDocId(int questionId)
        {
            List<PlantDocAnswerViewModel> listAnswers = new List<PlantDocAnswerViewModel>();
            PlantDocAnswerViewModel answerVm = new PlantDocAnswerViewModel();
            var answers = from a in ctx.PlantDocAnswers where a.PlantDocEntryId == questionId && !a.Deleted select a;
            if (answers != null)
            {
                foreach (var a in answers)
                {
                    answerVm = new PlantDocAnswerViewModel()
                    {
                       
                        Date = a.CreatedDate,
                        AutorName = a.CreatedBy,
                        AnswerId=a.Id,
                        IsEdited=a.IsEdited
                    };
                    if (answerVm.IsEdited)
                    {
                        answerVm.AnswerText = WebUtility.HtmlDecode(a.UpdatedAnswer);
                        answerVm.OriginalAnswer = WebUtility.HtmlDecode(a.AnswerText);
                    }
                    else
                    {
                        answerVm.AnswerText = WebUtility.HtmlDecode(a.AnswerText);
                    }
                    var user = ctx.Users.Where(u => !u.Deleted && u.Id == a.AuthorId.ToString()).FirstOrDefault();

                    if (user != null)
                    {
                        answerVm.AutorName = user.UserName.Split('@')[0];
                    }
                    HelperClasses.DbResponse imageResponse = rc.DbGetPlantDocAnswerReferencedImages(a.Id);
                    if (user.Id.Equals(Utilities.GetUserId().ToString())){
                        answerVm.EnableToEdit = true;
                       
                        if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                        {
                            answerVm.AnswerImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
                        }
                        else
                        {
                            answerVm.AnswerImages = null;
                        }
                    }
                    else
                    {
                        if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                        {
                            answerVm.AnswerImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                        }
                        else
                        {
                            answerVm.AnswerImages = null;
                        }
                    }
                   
                    listAnswers.Add(answerVm);
                }

            }
            return listAnswers;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-plantDocAnswer-image")]
        public void UploadAnswerImage(HttpPostedFileBase imageFile,int answerId, string imageTitle = null, string imageDescription = null, string imageLicense = null, string imageAuthor = null)
        {
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            if (imageDescription == null || imageDescription == "")
            {
                imageDescription = "Keine Beschreibung vorhanden.";
            }
            if (imageFile == null || imageFile.ContentLength <= 0 || imageFile.ContentLength > 4000000)
            {
                ViewBag.ErrorMessage = "Die Größe des Bildes sollte 4 MB nicht überschreiten";
                statusMessage.Messages.Add("Fehler beim Upload");
                statusMessage.Status = ModelEnums.ActionStatus.Error;
                //return
            }
            else
            {
                UploadAndRegisterFile(imageFile, answerId, (int)ModelEnums.ReferenceToModelClass.PlantDocAnswer, ModelEnums.FileReferenceType.PlantDocAnswerImage, imageTitle, imageDescription, imageLicense, imageAuthor);
            }

            TempData["statusMessage"] = statusMessage;

        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-plantDocQuestion-image")]
        public ActionResult UploadQuestionImage(HttpPostedFileBase imageFile, int questionId, string imageTitle = null, string imageDescription = null, string imageLicense = null, string imageAuthor = null)
        {
            
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            if (imageDescription == null || imageDescription == "")
            {
                imageDescription = "Admin_image Keine Beschreibung vorhanden.";
            }
            else
            {
                imageDescription="Admin_image" + imageDescription;
            }
            if (imageFile == null || imageFile.ContentLength <= 0 || imageFile.ContentLength > 4000000)
            {
                ViewBag.Message = "Die Größe des Bildes sollte 4 MB nicht überschreiten";
                statusMessage.Messages.Add("Fehler beim Upload");
                statusMessage.Status = ModelEnums.ActionStatus.Error;
                return Content(" <div class='alert alert - danger col - 12' role='alert'>Die Größe des Bildes sollte 4 MB nicht überschreiten </ div > ", "text/html");
            }
            else
            {
                UploadAndRegisterFile(imageFile, questionId, (int)ModelEnums.ReferenceToModelClass.PlantDocQuestion, ModelEnums.FileReferenceType.QuestionImage, imageTitle, imageDescription, imageLicense, imageAuthor);
                TempData["statusMessage"] = statusMessage;
                return RedirectToAction("Detail", new { questionId });
            }
            

        }

        [NonAction]
        public void CreateStatEntry(StatisticEventTypes type, Guid userId, int objectId = 0, EventObjectType objectType = 0, DateTime? date = null)
        {
            ApplicationUser user = ctx.Users.Find(userId.ToString());
            var ev = new StatisticEntry()
            {
                EventId = (int)type,
                UserId = userId,
                Date = date != null ? date.Value : DateTime.Now,
                ObjectId = objectId,
                ObjectType = objectType,
                DemoMode = (user != null && user.Email.StartsWith("UserDemo"))
            };
            ctx.StatisticEntries.Add(ev);
            ctx.SaveChanges();
        }
    }
}
