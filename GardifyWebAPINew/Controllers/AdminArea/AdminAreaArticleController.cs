using Microsoft.AspNet.Identity;
using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using static GardifyModels.Models.ModelEnums;

namespace GardifyWebAPI.Controllers.AdminArea
{
   // [CustomAuthorizeAttribute(Roles = "Admin")]
    public class AdminAreaArticleController : _BaseController
    {
        private ArticleController ac = new ArticleController();
        private ReferenceToFileSystemObjectController or = new ReferenceToFileSystemObjectController();


        public ActionResult Index()
        {
            ArticleViewModels.ArticleListViewModel viewModel = new ArticleViewModels.ArticleListViewModel();
            viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;
            HelperClasses.DbResponse response = ac.DbGetAllArticles(false);

            List<ArticleViewModels.ArticleViewModel> articleEntries = new List<ArticleViewModels.ArticleViewModel>();

            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                foreach (Article entry in response.ResponseObjects)
                {
                    ArticleViewModels.ArticleViewModel entryView = new ArticleViewModels.ArticleViewModel
                    {
                        Id = entry.Id,
                        Name = entry.Name,
                        Description = entry.Description,
                        NormalPrice = entry.Price,
                        PricePercentagePayableWithPoints = entry.PricePercentagePayableWithPoints
                    };

                    if (entry.PricePercentagePayableWithPoints > 0)
                    {
                        decimal pointValueInEuro = -1;
                        decimal.TryParse(WebConfigurationManager.AppSettings["pointValueInEuro"], out pointValueInEuro);

                        if (pointValueInEuro > 0)
                        {

                            decimal euroToCoverWithPoints = (entry.Price / 100) * entry.PricePercentagePayableWithPoints;

                            if (euroToCoverWithPoints < pointValueInEuro)
                            {
                                euroToCoverWithPoints = pointValueInEuro;
                            }

                            int pointsToSpend = (int)Math.Ceiling(euroToCoverWithPoints / pointValueInEuro);
                            // recalculate price to cover with points
                            euroToCoverWithPoints = pointsToSpend * pointValueInEuro;

                            // we can pay with points only
                            if (euroToCoverWithPoints >= entry.Price)
                            {
                                entryView.PricePartAfterPoints = 0;
                                entryView.PointsValue = entry.Price;
                            }
                            else
                            {
                                entryView.PricePartAfterPoints = entry.Price - euroToCoverWithPoints;
                                entryView.PointsValue = euroToCoverWithPoints;
                            }
                            entryView.PointsToSpend = pointsToSpend;
                        }
                    }

                    HelperClasses.DbResponse imageResponse = or.DbGetArticleReferencedImages(entry.Id);

                    entryView.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));

                    articleEntries.Add(entryView);
                }
            }

            viewModel.ListEntries = articleEntries;

            return View("~/Views/AdminArea/AdminAreaArticle/Index.cshtml", viewModel);
        }

        public ActionResult Create(ArticleViewModels.ArticleViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new ArticleViewModels.ArticleViewModel();
                viewModel.PricePercentagePayableWithPoints = 20;
            }
            return View("~/Views/AdminArea/AdminAreaArticle/CreateArticle.cshtml", viewModel);
        }

        // POST: news/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ArticleViewModels.ArticleViewModel viewModel, HttpPostedFileBase imageFile = null)
        {
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            if (ModelState.IsValid && !string.IsNullOrEmpty(viewModel.Name) && !string.IsNullOrEmpty(viewModel.Description))
            {
                viewModel.Name = viewModel.Name.Trim();
                Article articleData = new Article
                {
                    Name = viewModel.Name,
                    Description = viewModel.Description,
                    Price = viewModel.NormalPrice,
                    PricePercentagePayableWithPoints = viewModel.PricePercentagePayableWithPoints,
                    IsAvailable = viewModel.IsAvailable,
                    CreatedBy = User.Identity.GetUserName(),
                    Deleted = false
                };

                HelperClasses.DbResponse response = ac.DbCreateArticle(articleData);

                if (response.Status == ModelEnums.ActionStatus.Success)
                {
                    statusMessage.Status = response.Status;
                    statusMessage.Messages.Add("Der Artikel wurde erfolgreich erstellt. Sie können ihn jetzt bearbeiten");

                    if (imageFile != null)
                    {
                        bool isImageUploaded = UploadAndRegisterFile(imageFile, ((Article)response.ResponseObjects.FirstOrDefault()).Id, (int)ModelEnums.ReferenceToModelClass.Article, ModelEnums.FileReferenceType.ArticleImage, viewModel.Name);
                    }

                    TempData["statusMessage"] = statusMessage;
                    return RedirectToAction("Edit", new { id = ((Article)response.ResponseObjects.FirstOrDefault()).Id });
                }
                else
                {
                    statusMessage.Status = ModelEnums.ActionStatus.Error;
                    statusMessage.Messages.Add("Ein Fehler ist aufgetreten.");
                    viewModel.StatusMessage = statusMessage;
                }
            }
            statusMessage.Status = ModelEnums.ActionStatus.Error;
            statusMessage.Messages.Add("Bitte geben Sie alle erforderlichen Eingaben an.");
            viewModel.StatusMessage = statusMessage;
            return View("~/Views/AdminArea/AdminAreaArticle/CreateArticle.cshtml", viewModel);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return HttpNotFound();
            }

            ArticleViewModels.ArticleViewModel vm = new ArticleViewModels.ArticleViewModel();
            vm.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;

            HelperClasses.DbResponse response = ac.DbGetArticleById((int)id, false);

            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                Article article = response.ResponseObjects.FirstOrDefault() as Article;
                vm.Id = article.Id;
                vm.Name = article.Name;
                vm.Description = article.Description;
                vm.NormalPrice = article.Price;
                vm.IsAvailable = article.IsAvailable;
                vm.PricePercentagePayableWithPoints = article.PricePercentagePayableWithPoints;

                HelperClasses.DbResponse imageResponse = or.DbGetArticleReferencedImages(article.Id);

                vm.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));

                var articles = plantDB.ArticleReference.Where(v => v.ArticleId == article.Id);
                foreach (var larticle in articles)
                {
                    if (larticle.ReferenceType == ArticleReferenceType.Plant)
                    {
                        larticle.Plant = plantDB.Plants.Find(larticle.PlantId);
                    }
                    else if (larticle.ReferenceType == ArticleReferenceType.Todotemplate)
                    {
                        larticle.TodoTemplate = plantDB.TodoTemplate.Find(larticle.TodoTemplateId);
                    }
                    else
                    {
                        throw new DivideByZeroException("Haha nope");
                    }
                }
                vm.ArticleReferences = articles;
                vm.PlantReferenceList = plantDB.Plants;
                vm.TodotemplateReferenceList = plantDB.TodoTemplate;
            }
            else
            {
                return HttpNotFound();
            }
            return View("~/Views/AdminArea/AdminAreaArticle/EditArticle.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddArticleReference(int articleId, int? plantId, int? todotemplateId, int referenceType)
        {
            ArticleReference newRef = new ArticleReference
            {
                ArticleId = articleId,
                ReferenceType = (ArticleReferenceType)referenceType
            };
            if (newRef.ReferenceType == ArticleReferenceType.Plant)
            {
                newRef.PlantId = plantId;
            }
            else if (newRef.ReferenceType == ArticleReferenceType.Todotemplate)
            {
                newRef.TodoTemplateId = todotemplateId;
            }
            else
            {
                throw new DivideByZeroException("Pls enterz");
            }

            newRef.OnCreate("SYSTEM");

            plantDB.ArticleReference.Add(newRef);
            plantDB.SaveChanges();

            return RedirectToAction("edit", new { id = articleId });
        }

        public ActionResult DeleteArticleReference(int articleId, int articleReferenceId)
        {
            var toRemove = plantDB.ArticleReference.Find(articleReferenceId);
            plantDB.ArticleReference.Remove(toRemove);
            plantDB.SaveChanges();

            return RedirectToAction("edit", new { id = articleId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ArticleViewModels.ArticleViewModel viewModel)
        {
            Article newData = new Article
            {
                Id = viewModel.Id,
                Name = viewModel.Name.Trim(),
                Description = viewModel.Description,
                Price = viewModel.NormalPrice,
                PricePercentagePayableWithPoints = viewModel.PricePercentagePayableWithPoints,
                IsAvailable = viewModel.IsAvailable,
                EditedBy = User.Identity.GetUserName()
            };

            HelperClasses.DbResponse response = ac.DbEditArticle(newData);

            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            viewModel.StatusMessage = statusMessage;

            HelperClasses.DbResponse imageResponse = or.DbGetArticleReferencedImages(viewModel.Id);
            viewModel.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));

            return View("~/Views/AdminArea/AdminAreaArticle/EditArticle.cshtml", viewModel);
        }

        [ActionName("delete-article")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult deleteArticle(int id)
        {
            HelperClasses.DbResponse response = ac.DbDeleteArticle(id, User.Identity.GetUserName());
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Index");
        }

        //[ActionName("delete-article-image")]
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult deleteArticleImage(int imageId, int objectId)
        //{
        //    HelperClasses.DbResponse response = or.DbDeleteFile(imageId, User.Identity.GetUserName());
        //    _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
        //    statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
        //    statusMessage.Status = response.Status;
        //    TempData["statusMessage"] = statusMessage;
        //    return RedirectToAction("Edit", new { id = objectId });
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-article-image")]
        public ActionResult UploadArticleImage(HttpPostedFileBase imageFile, HttpPostedFile imageFileSrc, int objectId, string imageTitle = null, string imageDescription = null)
        {
              ImgResizerController imgResizer = new ImgResizerController();
        bool isOk = UploadAndRegisterFile(imageFile, objectId, (int)ModelEnums.ReferenceToModelClass.Article, ModelEnums.FileReferenceType.ArticleImage, imageTitle, imageDescription);

            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();

            if (isOk)
            {
                imgResizer.Upload(System.Web.Hosting.HostingEnvironment.MapPath("~/nfiles/ArticleImages/"), imageFileSrc);
                statusMessage.Messages.Add("Das Bild wurde erfolgreich hochgeladen");
                statusMessage.Status = ModelEnums.ActionStatus.Success;
            }
            else
            {
                statusMessage.Messages.Add("Beim Hochgeladen ist ein Fehler aufgetreten");
                statusMessage.Status = ModelEnums.ActionStatus.Error;
            }

            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Edit", new { id = objectId });
        }
    }
}