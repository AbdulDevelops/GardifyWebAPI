using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using GardifyWebAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GardifyWebAPI.Controllers
{
    public class PlantDocController : _BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ImgResizerController imgResizer = new ImgResizerController();
        ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

        // GET: PlantDoc
        public IEnumerable<PlantDocViewModel> Index(DateTime? readDate, bool isIos = false, bool isAndroid = false, bool isWebPage = false, bool getImage = true, bool showAnswer = true, int skip = 0, int take = -1)
        {
            PlantDocViewModel vm = null;


            if (isIos)
            {
                new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromIos, (int)GardifyPages.PlantDoc, EventObjectType.PageName);
            }
            else if (isAndroid)
            {
                new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromAndroid, (int)GardifyPages.PlantDoc, EventObjectType.PageName);
            }
            else if (isWebPage)
            {
                new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage, (int)GardifyPages.PlantDoc, EventObjectType.PageName);
            }


            List<PlantDocViewModel> listVm = new List<PlantDocViewModel>();
            var listOfQuestions = (from p in db.PlantDocs
                                   where !p.Deleted && p.UserAllowsPublishment && p.AdminAllowsPublishment
                                   orderby p.CreatedDate descending
                                   select p).Skip(skip).Take((take < 0)? int.MaxValue : take).ToList();
            if (listOfQuestions != null && listOfQuestions.Any())
            {
                foreach (var l in listOfQuestions.ToList())
                {
                    if (l.isEdited)
                    {
                        vm = new PlantDocViewModel()
                        {
                            QuestionText = l.QuestionText,
                            Description = l.DescriptionUpdate,
                            Headline = l.Headline,
                            Thema = l.ThemaUpdate,
                            IsOwnFoto = l.IsOwnFoto,
                            UserAllowsPublishment = l.UserAllowsPublishment,
                            QuestionAuthorId = l.QuestionAuthorId,
                            PublishDate = l.CreatedDate,
                            QuestionId = l.Id,
                            isEdited = l.isEdited
                        };
                    }
                    else
                    {
                        vm = new PlantDocViewModel()
                        {
                            QuestionText = l.QuestionText,
                            Description = l.Description,
                            Headline = l.Headline,
                            Thema = l.Thema,
                            IsOwnFoto = l.IsOwnFoto,
                            UserAllowsPublishment = l.UserAllowsPublishment,
                            QuestionAuthorId = l.QuestionAuthorId,
                            PublishDate = l.CreatedDate,
                            QuestionId = l.Id,
                            isEdited = l.isEdited
                        };
                    }
                    if (l.PublishWithImages && getImage)
                    {
                        HelperClasses.DbResponse imageResponse = rc.DbGetQuestionReferencedImages(l.Id);
                        if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                        {
                            vm.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                        }
                        else
                        {
                            vm.Images.Add(getImagePlaceholder());
                        }
                    }
                    else
                    {
                        vm.Images.Add(getImagePlaceholder());
                    }

                    //vm.TotalAnswers = (from a in db.PlantDocAnswers where !a.Deleted && a.PlantDocEntryId == l.Id select a).Count();
                    if (showAnswer)
                    {
                        var allAnswer = (from a in db.PlantDocAnswers where !a.Deleted && a.PlantDocEntryId == l.Id select a.CreatedDate).ToList();
                        vm.TotalAnswers = allAnswer.Count();
                        if (readDate == null)
                        {
                            vm.SeenAnswers = 0;
                        }
                        else
                        {
                            vm.SeenAnswers = allAnswer.Where(a => a > readDate).Count();

                        }
                    }

                    
                    listVm.Add(vm);
                }
            }
            return listVm;
        }
        public IEnumerable<PlantDocViewModel> getPostByUserId(Guid userId, DateTime? readDate)
        {
            PlantDocViewModel vm = null;
            List<PlantDocViewModel> listVm = new List<PlantDocViewModel>();
            var listOfQuestions = from p in db.PlantDocs where !p.Deleted && p.QuestionAuthorId == userId select p;
            if (listOfQuestions != null && listOfQuestions.Any())
            {
                foreach (var l in listOfQuestions)
                {
                    if (l.isEdited)
                    {
                        vm = new PlantDocViewModel()
                        {
                            Description = l.DescriptionUpdate,
                            Headline = l.Headline,
                            Thema = l.ThemaUpdate,
                            IsOwnFoto = l.IsOwnFoto,
                            UserAllowsPublishment = l.UserAllowsPublishment,
                            AdminAllowsPublishment = l.AdminAllowsPublishment,
                            QuestionAuthorId = l.QuestionAuthorId,
                            PublishDate = l.CreatedDate,
                            QuestionId = l.Id,
                            QuestionText = l.QuestionTextUpdate,
                            isEdited = l.isEdited
                        };
                    }
                    else
                    {
                        vm = new PlantDocViewModel()
                        {
                            Description = l.Description,
                            Headline = l.Headline,
                            Thema = l.Thema,
                            IsOwnFoto = l.IsOwnFoto,
                            UserAllowsPublishment = l.UserAllowsPublishment,
                            AdminAllowsPublishment = l.AdminAllowsPublishment,
                            QuestionAuthorId = l.QuestionAuthorId,
                            PublishDate = l.CreatedDate,
                            QuestionId = l.Id,
                            QuestionText = l.QuestionText,
                            isEdited = l.isEdited
                        };
                    }

                    if (l.PublishWithImages)
                    {
                        HelperClasses.DbResponse imageResponse = rc.DbGetQuestionReferencedImages(l.Id);
                        if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                        {
                            vm.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                        }
                        else
                        {
                            vm.Images.Add(getImagePlaceholder());
                        }
                    }
                    else
                    {
                        vm.Images.Add(getImagePlaceholder());
                    }

                    var allAnswer = (from a in db.PlantDocAnswers where !a.Deleted && a.PlantDocEntryId == l.Id select a);
                    vm.TotalAnswers = allAnswer.Count();
                    if (readDate == null)
                    {
                        vm.SeenAnswers = 0;
                    }
                    else
                    {
                        vm.SeenAnswers = allAnswer.Where(a => a.CreatedDate > readDate).Count();

                    }
                    listVm.Add(vm);
                }
            }
            return listVm;

        }
        public _HtmlImageViewModel getImagePlaceholder()
        {
            return new _HtmlImageViewModel
            {
                SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                Id = 0,
                TitleAttr = "kein Bild Vorhanden"
            };
        }
        public async Task<PlantDocDetailViewModel> DetailAsync(int id, ApplicationUser user)
        {
            PlantDocDetailViewModel pdVm = new PlantDocDetailViewModel();
            var question = (from p in db.PlantDocs where p.Id == id && !p.Deleted select p).FirstOrDefault();

            if (question != null)
            {
                PlantDocViewModel plantDocView;
                if (question.isEdited)
                {
                    plantDocView = new PlantDocViewModel()
                    {

                        QuestionText = question.QuestionTextUpdate,
                        Headline = question.Headline,
                        Thema = question.ThemaUpdate,
                        Description = question.DescriptionUpdate,
                        PublishDate = question.CreatedDate,
                        QuestionId = question.Id,
                        isEdited = question.isEdited
                    };
                }
                else
                {
                    plantDocView = new PlantDocViewModel()
                    {
                        QuestionText = question.QuestionText,
                        Headline = question.Headline,
                        Thema = question.Thema,
                        Description = question.Description,
                        PublishDate = question.CreatedDate,
                        QuestionId = question.Id,
                        isEdited = question.isEdited

                    };
                }

                if (question.PublishWithImages)
                {
                    HelperClasses.DbResponse imageResponse = rc.DbGetQuestionReferencedImages(question.Id);
                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                    {
                        string rootPath = HttpRuntime.AppDomainAppVirtualPath != "/" ? HttpRuntime.AppDomainAppVirtualPath + "/" : "/";
                        plantDocView.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, rootPath);
                    }
                    else
                    {
                        plantDocView.Images.Add(getImagePlaceholder());
                    }
                }
                else
                {
                    plantDocView.Images.Add(getImagePlaceholder());
                }
                pdVm = new PlantDocDetailViewModel()
                {
                    PlantDocViewModel = plantDocView
                };
                pdVm.PlantDocAnswerList = await getAnswersByQuestionIdAsync(question.Id, user, question.QuestionAuthorId);
            }
            return pdVm;
        }
        public async Task<IEnumerable<PlantDocAnswerViewModel>> getAnswersByQuestionIdAsync(int id, ApplicationUser reqUser, Guid questionAuthorId)
        {
            List<PlantDocAnswerViewModel> listAnswers = new List<PlantDocAnswerViewModel>();
            PlantDocAnswerViewModel answerVm = new PlantDocAnswerViewModel();
            var answers = from a in db.PlantDocAnswers where a.PlantDocEntryId == id && !a.Deleted select a;

            if (answers != null)
            {
                foreach (var a in answers)
                {
                    if (a.IsEdited)
                    {
                        StringWriter myWriter = new StringWriter();
                        HttpUtility.HtmlDecode(a.UpdatedAnswer, myWriter);
                        var answerText = myWriter.ToString();
                        answerVm = new PlantDocAnswerViewModel()
                        {
                            AnswerText = answerText,
                            Date = a.CreatedDate,
                            AutorName = a.CreatedBy,
                            AnswerId = a.Id,
                            IsEdited = a.IsEdited
                        };
                    }
                    else
                    {
                        StringWriter myWriter = new StringWriter();
                        HttpUtility.HtmlDecode(a.AnswerText, myWriter);
                        var answerText = myWriter.ToString();
                        answerVm = new PlantDocAnswerViewModel()
                        {
                            AnswerText = answerText,
                            Date = a.CreatedDate,
                            AutorName = a.CreatedBy,
                            AnswerId = a.Id,
                            IsEdited = a.IsEdited
                        };
                    }


                    var user = db.Users.Where(u => !u.Deleted && u.Id == a.AuthorId.ToString()).FirstOrDefault();

                    if (user != null)
                    {
                        answerVm.AutorName = user.UserName.StartsWith("Deleted-") ? "anonym" : user.UserName.Split('@')[0];
                        ICollection<Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole> role = user.Roles;
                        if (role.Any())
                        {
                            answerVm.IsAdminAnswer = true;
                        }
                    }
                    if (a.AuthorId.Equals(Utilities.GetUserId()))
                    {
                        answerVm.EnableToEdit = true;
                    }
                    else
                    {
                        answerVm.EnableToEdit = false;
                    }
                    HelperClasses.DbResponse imageResponse = rc.DbGetPlantDocAnswerReferencedImages(a.Id);
                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                    {
                        answerVm.AnswerImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                    }
                    else
                    {
                        answerVm.AnswerImages = null;
                    }
                    listAnswers.Add(answerVm);
                    if (Guid.Parse(reqUser.Id) == questionAuthorId)
                    {
                        a.isAlreadyReaded = true;

                    }
                }
                db.SaveChanges();
            }
            return listAnswers;
        }
        public int totalNotReadedAnswers()
        {
            var currentUserId = Utilities.GetUserId();
            int count = 0;
            var noReadedAnswersCountList = countAnswersToPlantDocByUserId(currentUserId);
            if (noReadedAnswersCountList != null)
            {
                foreach (var n in noReadedAnswersCountList)
                {
                    count = count + n.count;
                }
            }

            return count;
        }

        public List<int> getNotReadedAnswersId()
        {
            var currentUserId = Utilities.GetUserId();
            List<int> idList = new List<int>();
            var noReadedAnswersCountList = countAnswersToPlantDocByUserId(currentUserId);
            if (noReadedAnswersCountList != null)
            {
                foreach (var n in noReadedAnswersCountList)
                {
                    idList.Add(n.PlantDocEntryId);
                }
            }

            return idList;
        }
        public PlantDocAnswer AnswerDetail(int id)
        {
            PlantDocAnswer answer = db.PlantDocAnswers.Where(a => a.Id == id && !a.Deleted).FirstOrDefault();
            return answer;
        }
        public List<AnswersCountViewModel> countAnswersToPlantDocByUserId(Guid userId)
        {
            List<AnswersCountViewModel> noReadedAnswersCount = new List<AnswersCountViewModel>();
            var list = from p in db.PlantDocs where p.AdminAllowsPublishment && !p.Deleted && p.UserAllowsPublishment && p.QuestionAuthorId == userId select p;
            if (list != null)
            {
                foreach (var l in list)
                {
                    var answersFromQuestion = from a in db.PlantDocAnswers where !a.isAlreadyReaded && !a.Deleted && a.PlantDocEntryId == l.Id select a;
                    if (answersFromQuestion != null && answersFromQuestion.Any())
                    {
                        AnswersCountViewModel vm = new AnswersCountViewModel()
                        {
                            count = answersFromQuestion.Count(),
                            PlantDocEntryId = l.Id
                        };
                        noReadedAnswersCount.Add(vm);
                    }
                }

            }

            return noReadedAnswersCount;
        }
        public bool UpdatePostAsync(int postId, PlantDocViewModel model, ApplicationUser user)
        {
            const string Backend_URL = "https://gardify.de/intern/i/plant-doc/Detail";
            PlantDoc postEntry = getPostbyId(postId);

            // dont allow other users to edit posts they dont own
            if (postEntry == null || postEntry.QuestionAuthorId.ToString() != user.Id)
            {
                return false;
            }

            postEntry.QuestionTextUpdate = model.QuestionText;
            postEntry.ThemaUpdate = model.Thema;
            postEntry.UserAllowsPublishment = true;
            postEntry.DescriptionUpdate = model.Description;
            postEntry.isEdited = true;
            db.SaveChanges();
            EmailSender es = new EmailSender(db);
            TemplateService ts = new TemplateService();

            if (user != null && model.Thema != "TESTING")
            {
                var question = (from p in db.PlantDocs where p.Id == postId && !p.Deleted select p).FirstOrDefault();
                var actionLink = new Uri(Backend_URL + $" ? questionId ={ postId }");
                string content = ts.RenderTemplateAsync("UserMessage", new { Name = user.UserName, user.Email, Text = question.QuestionText, question.Id, ActionLink = actionLink });
                es.SendEmail("[Plant-Doc] Update Post: " + question.Thema, content, user.Email, "team@gardify.de");
            }
            return true;
        }
        public bool UpdateAnswer(int answerId, PlantDocAnswerViewModel model, Guid userId)
        {
            PlantDocAnswer answerEntry = getAnswerById(answerId);

            if (answerEntry == null || answerEntry.AuthorId != userId)
            {
                return false;
            }

            answerEntry.UpdatedAnswer = model.AnswerText;
            answerEntry.IsEdited = true;
            db.SaveChanges();
            return true;
        }

        // GET: PlantDoc/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: PlantDoc/Create
        [HttpPost]
        public async Task<int> PostQuestion(PlantDocEntryModel entry, ApplicationUser user, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            int entryId = -1;
            try
            {
                // TODO: Add insert logic here
                if (entry != null && Utilities.GetUserId() != Guid.Empty)
                {
                    PlantDoc pl = new PlantDoc()
                    {
                        QuestionText = entry.QuestionText,
                        UserAllowsPublishment = true,
                        IsOwnFoto = entry.IsOwnFoto,
                        Thema = string.IsNullOrEmpty(entry.Thema) ? "" : entry.Thema,
                        Description = string.IsNullOrEmpty(entry.Description) ? "" : entry.Description,
                        QuestionAuthorId = Utilities.GetUserId(),
                        Headline = "Headline"
                    };
                    pl.OnCreate(Utilities.GetUserName());
                    PlantDoc newEntry = db.PlantDocs.Add(pl);

                    var currentStatistic = StatisticEventTypes.SubmitQuestion;

                    if (isIos)
                    {
                        new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromIos);
                    }
                    else if (isAndroid)
                    {
                        new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromAndroid);
                    }
                    else if (isWebPage)
                    {
                        new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromWebpage);
                    }


                    db.SaveChanges();
                    entryId = newEntry.Id;
                }
                return entryId;
            }
            catch
            {
                return entryId;
            }
        }

        [HttpPost]
        public int PostAnswer(AnswerViewModel entry, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            int entryId = -1;
            try
            {
                // TODO: Add insert logic here
                if (entry != null && Utilities.GetUserId() != Guid.Empty)
                {
                    PlantDocAnswer an = new PlantDocAnswer()
                    {
                        AnswerText = entry.AnswerText,
                        AuthorId = Utilities.GetUserId(),
                        PlantDocEntryId = entry.PlantDocEntryId
                    };
                    an.OnCreate(Utilities.GetUserName());
                    PlantDocAnswer newEntry = db.PlantDocAnswers.Add(an);
                    db.SaveChanges();
                    entryId = newEntry.Id;

                    var currentStatistic = StatisticEventTypes.SubmitAnswer;

                    if (isIos)
                    {
                        new StatisticsController().CreateEntry(currentStatistic, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromIos);
                    }
                    else if (isAndroid)
                    {
                        new StatisticsController().CreateEntry(currentStatistic, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromAndroid);
                    }
                    else if (isWebPage)
                    {
                        new StatisticsController().CreateEntry(currentStatistic, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage);
                    }


                }
                return entryId;
            }
            catch
            {
                return entryId;
            }
        }
        public PlantDoc getPostbyId(int id)
        {
            var post = db.PlantDocs.Where(p => p.Id == id).FirstOrDefault();
            return post;
        }
        public PlantDocAnswer getAnswerById(int answerId)
        {
            var answer = db.PlantDocAnswers.Where(a => a.Id == answerId).FirstOrDefault();

            StringWriter myWriter = new StringWriter();
            HttpUtility.HtmlDecode(answer.AnswerText, myWriter);
            var decodedAnswer = myWriter.ToString();

            StringWriter myWriter2 = new StringWriter();
            HttpUtility.HtmlDecode(answer.UpdatedAnswer, myWriter2);
            var decodedUpdatedAnswer = myWriter2.ToString();

            answer.AnswerText = decodedAnswer;
            answer.UpdatedAnswer = decodedUpdatedAnswer;

            return answer;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult uploadQuestionImgFull(ApplicationUser user, int questionId, List<string> upload)
        {
            EmailSender es = new EmailSender(db);
            TemplateService ts = new TemplateService();
            const string Backend_URL = "https://gardify.de/intern/i/plant-doc/Detail";
            var question = (from p in db.PlantDocs where p.Id == questionId && !p.Deleted select p).FirstOrDefault();

            if (user != null && question != null && question.Thema != "TESTING")
            {
                var actionLink = new Uri(Backend_URL + $" ? questionId ={  questionId }");
                string content = ts.RenderTemplateAsync("UserMessage", new { Name = user.UserName, user.Email, Text = question.QuestionText, question.Id, ActionLink = actionLink });
                es.SendEmail("[Plant-Doc] Neue Frage: " + question.Thema, content, user.Email, "team@gardify.de", upload.ToArray());
            }

            return RedirectToAction("EditEntry", new { id = questionId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public Files UploadAndRegisterQuestImgFile(HttpPostedFileBase uploadedFile, HttpPostedFile imageFileSrc, int referencedObjectId, int moduleId, ModelEnums.FileReferenceType fileReferenceType, string fileTitle = null, string fileDescription = null)
        {
            string fileNameWithoutExtension = Utilities.stringToUri(System.IO.Path.GetFileNameWithoutExtension(uploadedFile.FileName));
            string extension = Path.GetExtension(uploadedFile.FileName).ToLower();
            string relativePath = System.Web.Hosting.HostingEnvironment.MapPath("~/nfiles/QuestionsImages/");
            string fullPath = Path.Combine(relativePath, fileNameWithoutExtension + extension);
            if (System.IO.File.Exists(fullPath))
            {
                int counter = 1;
                string tempFileName = "";
                do
                {
                    tempFileName = fileNameWithoutExtension + "_V" + counter.ToString();
                    fullPath = Path.Combine(relativePath, tempFileName + extension);
                    counter++;
                } while (System.IO.File.Exists(fullPath));

                fileNameWithoutExtension = tempFileName;
            }
            string savedFileName = fileNameWithoutExtension + extension;
            imgResizer.Upload(relativePath, imageFileSrc, savedFileName);
            var res = UploadAndRegisterFileFull(uploadedFile, referencedObjectId, moduleId, fileReferenceType, fileTitle, fileDescription);
            return res;
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-question-image")]
        public ActionResult UploadQuestImage(HttpPostedFileBase imageFile, int questionId, string imageTitle = null, string imageDescription = null)
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
                UploadAndRegisterFile(imageFile, questionId, (int)ModelEnums.ReferenceToModelClass.PlantDocQuestion, ModelEnums.FileReferenceType.QuestionImage, imageTitle, imageDescription);
            }

            TempData["statusMessage"] = statusMessage;

            return RedirectToAction("EditEntry", new { id = questionId });
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-answer-image")]
        public ActionResult UploadAnswerImage(HttpPostedFileBase imageFile, HttpPostedFile imageFileSrc, int answerId, string imageTitle = null, string imageDescription = null)
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
                string fileNameWithoutExtension = Utilities.stringToUri(System.IO.Path.GetFileNameWithoutExtension(imageFile.FileName));
                string extension = Path.GetExtension(imageFile.FileName).ToLower();
                string relativePath = System.Web.Hosting.HostingEnvironment.MapPath("~/nfiles/PlantDocAnswersImages/");
                string fullPath = Path.Combine(relativePath, fileNameWithoutExtension + extension);
                if (System.IO.File.Exists(fullPath))
                {
                    int counter = 1;
                    string tempFileName = "";
                    do
                    {
                        tempFileName = fileNameWithoutExtension + "_V" + counter.ToString();
                        fullPath = Path.Combine(relativePath, tempFileName + extension);
                        counter++;
                    } while (System.IO.File.Exists(fullPath));

                    fileNameWithoutExtension = tempFileName;
                }
                string savedFileName = fileNameWithoutExtension + extension;
                imgResizer.Upload(relativePath, imageFileSrc, savedFileName);
                UploadAndRegisterFile(imageFile, answerId, (int)ModelEnums.ReferenceToModelClass.PlantDocAnswer, ModelEnums.FileReferenceType.PlantDocAnswerImage, imageTitle, imageDescription);
            }

            TempData["statusMessage"] = statusMessage;

            return RedirectToAction("EditEntry", new { id = answerId });
        }



    }
}
