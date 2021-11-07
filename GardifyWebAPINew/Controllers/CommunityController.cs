using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GardifyWebAPI.Controllers
{
    public class CommunityController : _BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private ImgResizerController imgResizer = new ImgResizerController();
        ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
        // GET: Community
        public async Task<IEnumerable<CommunityPostViewModel>> IndexAsync(bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {



            if (isIos)
            {
                new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromIos, (int)GardifyPages.Community, EventObjectType.PageName);
            }
            else if (isAndroid)
            {
                new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromAndroid, (int)GardifyPages.Community, EventObjectType.PageName);
            }
            else if (isWebPage)
            {
                new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage, (int)GardifyPages.Community, EventObjectType.PageName);
            }

            return await GetAllPostsAsync();

        }

        public async Task<IEnumerable<CommunityPostViewModel>> GetAllPostsAsync()
        {
            CommunityPostViewModel vm = null;
            List<CommunityPostViewModel> listVm = new List<CommunityPostViewModel>();
            var listOfQuestions = from p in db.CommunityPosts
                                  where !p.Deleted
                                  orderby p.CreatedDate descending
                                  select p;
            if (listOfQuestions != null && listOfQuestions.Any())
            {

                //var userList = db.Users.ToList();

                foreach (var l in listOfQuestions)
                {
                    // this does not work yet
                    //ApplicationUser user = await UserManager.FindByIdAsync(l.QuestionAuthorId.ToString());
                    vm = new CommunityPostViewModel()
                    {
                        QuestionText = l.QuestionText,
                        //Description = l.Description,
                        //Headline = l.Headline,
                        Thema = l.Thema,
                        //IsOwnFoto = l.IsOwnFoto,
                        //UserAllowsPublishment = l.UserAllowsPublishment,
                        QuestionAuthorId = l.QuestionAuthorId,
                        //AutorName=user!=null? user.UserName: null,
                        PublishDate = l.CreatedDate,
                        QuestionId = l.Id,
                        IsOnlyExpert = l.IsOnlyExpert,
                        Images = new List<_HtmlImageViewModel>()
                        //isEdited = l.isEdited
                    };

                    //if (l.PublishWithImages)
                    //{
                    HelperClasses.DbResponse imageResponse = rc.DbGetCommunityPostReferencedImages(l.Id);
                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                    {
                        vm.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                    }
                    else
                    {
                        vm.Images.Add(getImagePlaceholder());
                    }
                    //}
                    //else
                    //{
                    //    var placeholder = getImagePlaceholder();
                    //    vm.Images.Add(placeholder);
                    //}

                    listVm.Add(vm);
                }
            }
            return listVm;
        }
        public async Task<IEnumerable<CommunityPostWithAnswerViewModel>> GetListOfPostsWithRelatedAnswersAsync(bool isIos = false, bool isAndroid = false, bool isWebPage = false, int skip = 0, int take = int.MaxValue)
        {
            ApplicationUser reqUser = null;
            var listOfPosts = await GetAllPostsAsync();
            List<CommunityPostWithAnswerViewModel> listVm = new List<CommunityPostWithAnswerViewModel>();

            foreach (var post in listOfPosts.Skip(skip).Take(take))
            {
                CommunityPostWithAnswerViewModel vm = new CommunityPostWithAnswerViewModel()
                {
                    Post = post,
                    AutorProfilUrl=GetUserProfilImg(post.QuestionAuthorId),
                    CommunityAnswerList = await getAnswersByQuestionIdAsync(post.QuestionId, reqUser)
                };
                listVm.Add(vm);
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

        [HttpPost]
        public async Task<int> PostQuestion(CommunityPostEntryModel entry, ApplicationUser user, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            int entryId = -1;
            try
            {
                // TODO: Add insert logic here
                if (entry != null && Utilities.GetUserId() != Guid.Empty)
                {
                    CommunityPost pl = new CommunityPost()
                    {
                        QuestionText = entry.QuestionText,
                        Thema = string.IsNullOrEmpty(entry.Thema) ? "" : entry.Thema,
                        QuestionAuthorId = Utilities.GetUserId(),
                        IsOnlyExpert = entry.IsOnlyExpert
                    };
                    pl.OnCreate(Utilities.GetUserName());
                    CommunityPost newEntry = db.CommunityPosts.Add(pl);

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
        public int PostAnswer(CommunityAnswerViewModel entry, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            int entryId = -1;
            try
            {
                // TODO: Add insert logic here
                if (entry != null && Utilities.GetUserId() != Guid.Empty)
                {
                    var question = db.CommunityPosts.FirstOrDefault(c => c.Id == entry.CommunityEntryId);

                    CommunityAnswer an = new CommunityAnswer()
                    {
                        AnswerText = entry.AnswerText,
                        AuthorId = Utilities.GetUserId(),
                        CommunityEntryId = entry.CommunityEntryId
                    };
                    an.OnCreate(Utilities.GetUserName());
                    CommunityAnswer newEntry = db.CommunityAnswers.Add(an);
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

        public async Task<CommunityPostDetailViewModel> DetailAsync(int id, ApplicationUser user)
        {
            CommunityPostDetailViewModel pdVm = new CommunityPostDetailViewModel();
            var question = (from p in db.CommunityPosts where p.Id == id && !p.Deleted select p).FirstOrDefault();

            if (question != null)
            {
                CommunityPostViewModel plantDocView;
                plantDocView = new CommunityPostViewModel()
                {
                    QuestionText = question.QuestionText,
                    //Description = l.Description,
                    //Headline = l.Headline,
                    Thema = question.Thema,
                    //IsOwnFoto = l.IsOwnFoto,
                    //UserAllowsPublishment = l.UserAllowsPublishment,
                    QuestionAuthorId = question.QuestionAuthorId,
                    PublishDate = question.CreatedDate,
                    QuestionId = question.Id,
                    IsOnlyExpert = question.IsOnlyExpert
                    //isEdited = l.isEdited
                };

                HelperClasses.DbResponse imageResponse = rc.DbGetCommunityPostReferencedImages(question.Id);
                if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                {
                    string rootPath = HttpRuntime.AppDomainAppVirtualPath != "/" ? HttpRuntime.AppDomainAppVirtualPath + "/" : "/";
                    plantDocView.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, rootPath);
                }
                else
                {
                    //plantDocView.Images.Add(getImagePlaceholder());
                }

                pdVm = new CommunityPostDetailViewModel()
                {
                    communityPostViewModel = plantDocView
                };
                pdVm.communityAnswerList = await getAnswersByQuestionIdAsync(question.Id, user);
            }
            return pdVm;
        }

        public List<_HtmlImageViewModel> GetUserProfilImg(Guid userId)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            List<_HtmlImageViewModel> output = new List<_HtmlImageViewModel>();

            var userProperty = new PropertyController().DbGetProperty(userId);
            var userPropertyId = userProperty.Id;
            HelperClasses.DbResponse imageResponse = rc.DbGetProfilImgReferencedImages(userPropertyId);

            if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            {
                output = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/")).OrderByDescending(r => r.InsertDate).ToList();
            }
            return output;
        }

        public async Task<IEnumerable<CommunityFullAnswerViewModel>> getAnswersByQuestionIdAsync(int id, ApplicationUser reqUser)
        {
            List<CommunityFullAnswerViewModel> listAnswers = new List<CommunityFullAnswerViewModel>();
            CommunityFullAnswerViewModel answerVm = new CommunityFullAnswerViewModel();
            var answers = from a in db.CommunityAnswers where a.CommunityEntryId == id && !a.Deleted select a;

            if (answers != null)
            {
                //var userList = db.Users.ToList();


                foreach (var a in answers)
                {
                    answerVm = new CommunityFullAnswerViewModel()
                    {
                        AnswerText = a.AnswerText,
                        Date = a.CreatedDate,
                        AutorName = a.CreatedBy,
                        AnswerId = a.Id,
                        IsFromAdmin = a.IsFromAdmin,
                        ProfilUrl = GetUserProfilImg(a.AuthorId)
                        //IsEdited = a.IsEdited
                    };
                    //if (a.IsEdited)
                    //{
                    //    answerVm = new PlantDocAnswerViewModel()
                    //    {
                    //        AnswerText = a.UpdatedAnswer,
                    //        Date = a.CreatedDate,
                    //        AutorName = a.CreatedBy,
                    //        AnswerId = a.Id,
                    //        IsEdited = a.IsEdited
                    //    };
                    //}
                    //else
                    //{
                    //    answerVm = new PlantDocAnswerViewModel()
                    //    {
                    //        AnswerText = a.AnswerText,
                    //        Date = a.CreatedDate,
                    //        AutorName = a.CreatedBy,
                    //        AnswerId = a.Id,
                    //        IsEdited = a.IsEdited
                    //    };
                    //}


                    var user = db.Users.Where(u => !u.Deleted && u.Id == a.AuthorId.ToString()).FirstOrDefault();

                    if (user != null)
                    {
                        answerVm.AutorName = user.UserName.StartsWith("Deleted-") ? "anonym" : user.UserName.Split('@')[0];
                        //ICollection<Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole> role = user.Roles;
                        //if (role.Any())
                        //{
                        //    answerVm.IsAdminAnswer = true;
                        //}
                    }
                    //if (a.AuthorId.Equals(Utilities.GetUserId()))
                    //{
                    //    answerVm.EnableToEdit = true;
                    //}
                    //else
                    //{
                    //    answerVm.EnableToEdit = false;
                    //}
                    HelperClasses.DbResponse imageResponse = rc.DbGetCommunityAnswerReferencedImages(a.Id);
                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                    {
                        answerVm.AnswerImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                    }
                    else
                    {
                        answerVm.AnswerImages = null;
                    }
                    listAnswers.Add(answerVm);
                    //if (Guid.Parse(reqUser.Id) == questionAuthorId)
                    //{
                    //    a.isAlreadyReaded = true;

                    //}
                }
                db.SaveChanges();
            }
            return listAnswers;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-post-image")]
        public ActionResult UploadPostImage(HttpPostedFileBase imageFile, HttpPostedFile imageFileSrc, int answerId, string imageTitle = null, string imageDescription = null)
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
                string relativePath = System.Web.Hosting.HostingEnvironment.MapPath("~/nfiles/CommunityPostsImages/");
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
                UploadAndRegisterFile(imageFile, answerId, (int)ModelEnums.ReferenceToModelClass.CommunityPost, ModelEnums.FileReferenceType.CommunityPost, imageTitle, imageDescription);
            }

            TempData["statusMessage"] = statusMessage;

            return RedirectToAction("EditEntry", new { id = answerId });
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
                string relativePath = System.Web.Hosting.HostingEnvironment.MapPath("~/nfiles/CommunityAnswersImages/");
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
                UploadAndRegisterFile(imageFile, answerId, (int)ModelEnums.ReferenceToModelClass.CommunityAnswer, ModelEnums.FileReferenceType.CommunityAnswer, imageTitle, imageDescription);
            }

            TempData["statusMessage"] = statusMessage;

            return RedirectToAction("EditEntry", new { id = answerId });
        }
    }
}