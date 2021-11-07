using Microsoft.AspNet.Identity;
using PflanzenApp.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using static GardifyModels.Models.ModelEnums;
using System.Data.Entity;

namespace PflanzenApp.Controllers.AdminArea
{
    [CustomAuthorizeAttribute(Roles = "Admin")]
    public class AdminAreaArticleController : _BaseController
    {
        private ArticleController ac = new ArticleController();
        private ReferenceToFileSystemObjectController or = new ReferenceToFileSystemObjectController();


        public ActionResult Index(int? categoryId)
        {
            ArticleViewModels.ArticleListViewModel viewModel = new ArticleViewModels.ArticleListViewModel();
            viewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;
            HelperClasses.DbResponse response= new HelperClasses.DbResponse();
            if (categoryId != null)
            {
               response = ac.DbGetAllArticles(false,(int)categoryId);
                viewModel.SelectedCategoriesId = Int32.Parse(getArticlesCategoriesList().FirstOrDefault().Value);
            }
            else
            {
                response = ac.DbGetAllArticles(false);
            }
            
            List<ArticleViewModels.ArticleViewModel> articleEntries = new List<ArticleViewModels.ArticleViewModel>();

            if (response.ResponseObjects != null && response.ResponseObjects.Any())
            {
                foreach (Article entry in response.ResponseObjects)
                {
                    ArticleViewModels.ArticleViewModel entryView = new ArticleViewModels.ArticleViewModel
                    {
                        Id = entry.Id,
                        Name = entry.Name,
                        AllowPublishment = entry.AllowPublishment,
                        Description = entry.Description,
                        NormalPrice = entry.Price,
                        Sort = entry.Sort,
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

                    entryView.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));

                    articleEntries.Add(entryView);
                }
            }

            viewModel.ListEntries = articleEntries;
            viewModel.CategoriesEntries = getArticlesCategoriesList();
            
            return View("~/Views/AdminArea/AdminAreaArticle/Index.cshtml", viewModel);
        }

        public ActionResult ReSort()
        {
            var total = ctx.Articles.Where(a => !a.Deleted).OrderBy(a => a.Sort).ToList();

            for (var i = 0; i < total.Count(); i++)
            {
                total.ElementAt(i).Sort = i + 1;
                ctx.Entry(total.ElementAt(i)).State = EntityState.Modified;
            }
            ctx.SaveChanges();

            return RedirectToAction("Overview");
        }

        public ActionResult Overview(string searchText = null)
        {
            var viewModel = GetArticleForOverview(searchText);
            return View("~/Views/AdminArea/AdminAreaArticle/Overview.cshtml", viewModel);
        }

        [ActionName("UpdateArticleSort")]
        [HttpGet]
        public ActionResult UpdateArticleSort(int articleId, int newSort)
        {
            ApplicationDbContext db = new ApplicationDbContext();
            var toUpdate = (from a in db.Articles
                            where !a.Deleted && a.Id == articleId
                            select a).FirstOrDefault();

            toUpdate.Sort = newSort;
            db.SaveChanges();

            var list = (from a in db.Articles
                       where !a.Deleted
                       select a).OrderBy(a => a.Sort).ThenByDescending(a => a.EditedDate).ToList();

            for (var i = 0; i < list.Count(); i++)
            {
                list.ElementAt(i).Sort = i + 1;
            }

            db.SaveChanges();

            return RedirectToAction("Overview");
        }

        private ArticleViewModels.ArticleListViewModel GetArticleForOverview(string searchText = null)
        {
            var viewModel = new ArticleViewModels.ArticleListViewModel();
            List<ArticleViewModels.ArticleViewModel> res = new List<ArticleViewModels.ArticleViewModel>();
            var list = (from a in ctx.Articles
                        where !a.Deleted 
                        && (string.IsNullOrEmpty(searchText) ? true 
                        : (a.Name.ToLower().Contains(searchText) || a.Description.ToLower().Contains(searchText) || a.Id.ToString().Contains(searchText)))
                        select a).OrderBy(a => a.Sort).ThenByDescending(a => a.EditedDate);

            foreach (Article entry in list)
            {
                ArticleViewModels.ArticleViewModel entryView = new ArticleViewModels.ArticleViewModel
                {
                    Id = entry.Id,
                    Name = entry.Name,
                    Sort = entry.Sort,
                    IsNotDeliverable = entry.IsNotDeliverable,
                    AllowPublishment = entry.AllowPublishment,
                    NormalPrice = entry.Price,
                    IsAvailable=entry.IsAvailable,
                    PricePercentagePayableWithPoints = entry.PricePercentagePayableWithPoints
                };
                res.Add(entryView);
            }
            viewModel.ListEntries = res.OrderBy(a => a.Sort);
            return viewModel;
        }

        public ActionResult Create(ArticleViewModels.ArticleViewModel viewModel)
        {
            if (viewModel == null)
            {
                viewModel = new ArticleViewModels.ArticleViewModel();
                viewModel.PricePercentagePayableWithPoints = 20;
                viewModel.Sort = 1;
            }
            ViewBag.ArticleCategories = getArticlesCategoriesList();
            return View("~/Views/AdminArea/AdminAreaArticle/CreateArticle.cshtml", viewModel);
        }

        // POST: news/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ArticleViewModels.ArticleViewModel viewModel, int[] articleCategories, HttpPostedFileBase imageFile = null, string imageLicense = null, string imageAuthor = null)
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
                    AllowPublishment = viewModel.AllowPublishment,
                    CreatedBy = User.Identity.GetUserName(),
                    Deleted = false,
                    ExpertTip = viewModel.ExpertTip,
                    WeightInGrams = viewModel.WeightInGrams,
                    BulkArticle = viewModel.BulkArticle,
                    MakerId = viewModel.MakerId
                };
                if (articleCategories != null && articleCategories.Any())
                {
                    foreach (int id in articleCategories)
                    {
                        var sel_category = (ArticleCategory)ac.DbGetArticleCategoryById(id).ResponseObjects.FirstOrDefault();
                        articleData.ArticleCategories.Add(sel_category);
                    }
                }

                HelperClasses.DbResponse response = ac.DbCreateArticle(articleData);

                if (response.Status == ModelEnums.ActionStatus.Success)
                {
                    statusMessage.Status = response.Status;
                    statusMessage.Messages.Add("Der Artikel wurde erfolgreich erstellt. Sie können ihn jetzt bearbeiten");

                    if (imageFile != null)
                    {
                        IncrementRestSorts(((Article)response.ResponseObjects.FirstOrDefault()).Id);
                        bool isImageUploaded = UploadAndRegisterFile(imageFile, ((Article)response.ResponseObjects.FirstOrDefault()).Id, (int)ModelEnums.ReferenceToModelClass.Article, ModelEnums.FileReferenceType.ArticleImage, viewModel.Name, null, imageLicense, imageAuthor);
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
            ViewBag.ArticleCategories = ac.DbGetArticleCategoriesSelectList();
            return View("~/Views/AdminArea/AdminAreaArticle/CreateArticle.cshtml", viewModel);
        }

        public void IncrementRestSorts(int articleId)
        {
            var restArticles = ctx.Articles.Where(a => !a.Deleted && a.Id != articleId).ToList();
            foreach (Article a in restArticles)
            {
                a.Sort++;
            }
            ctx.SaveChanges();
        }

        public IEnumerable<SelectListItem> getArticlesCategoriesList(int? id = -1)
        {
            var cats = ctx.ArticleCategories.Where(a => !a.Deleted);
            List<SelectListItem> catList = new List<SelectListItem>();
            foreach (var c in cats)
            {
                if (c.Id == id)
                {
                    catList.Add(new SelectListItem
                    {
                        Text = c.Title ,
                        Value = c.Id.ToString(),
                        Selected = true
                    });
                }
                else
                {
                    catList.Add(new SelectListItem
                    {
                        Text = c.Title,
                        Value = c.Id.ToString()
                    });
                }
            }
            return catList;
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
                vm.PhotoLink = article.PhotoLink;
                vm.Description = article.Description;
                vm.ExpertTip = article.ExpertTip;
                vm.NormalPrice = article.Price;
                vm.ProductLink = article.ProductLink;
                vm.IsAvailable = article.IsAvailable;
                vm.IsNotDeliverable = article.IsNotDeliverable;
                vm.Brand = article.Brand;
                vm.VAT = article.VAT;
                vm.Author = article.Author;
                vm.MakerId = article.MakerId;
                vm.WeightInGrams = article.WeightInGrams;
                vm.BulkArticle = article.BulkArticle;
                vm.EANCode = article.EANCode;
                vm.HazardNotice = article.HazardNotice;
                vm.AllowPublishment = article.AllowPublishment;
                vm.PricePercentagePayableWithPoints = article.PricePercentagePayableWithPoints;

                HelperClasses.DbResponse imageResponse = or.DbGetArticleReferencedImages(article.Id);

                vm.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
                ViewBag.ArticleCategories = ac.DbGetArticleCategoriesSelectionByArticleId(vm.Id);

                var articles = ctx.ArticleReference.Where(v => v.ArticleId == article.Id);
                foreach (var larticle in articles)
                {
                    if (larticle.ReferenceType == ArticleReferenceType.Plant)
                    {
                        larticle.Plant = ctx.Plants.Find(larticle.PlantId);
                    }
                    else if (larticle.ReferenceType == ArticleReferenceType.Todotemplate)
                    {
                        larticle.TodoTemplate = ctx.TodoTemplate.Find(larticle.TodoTemplateId);
                    }
                    else
                    {
                        throw new DivideByZeroException("Haha nope");
                    }
                }
                vm.ArticleReferences = articles;
                vm.PlantReferenceList = ctx.Plants;
                vm.TodotemplateReferenceList = ctx.TodoTemplate;
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

            ctx.ArticleReference.Add(newRef);
            ctx.SaveChanges();

            return RedirectToAction("edit", new { id = articleId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AddBulkReferences(int articleId, int referenceType, string bulkIds = null)
        {
            var ids = bulkIds.Split(',');
            var range = new List<ArticleReference>();

            foreach (string strId in ids)
            {
                ArticleReference newRef = new ArticleReference
                {
                    ArticleId = articleId,
                    ReferenceType = (ArticleReferenceType)referenceType
                };

                int objId;
                bool parsed = int.TryParse(strId, out objId);
                if (!parsed)
                {
                    continue;
                }
                if (newRef.ReferenceType == ArticleReferenceType.Plant)
                {
                    newRef.PlantId = objId;
                }
                else if (newRef.ReferenceType == ArticleReferenceType.Todotemplate)
                {
                    newRef.TodoTemplateId = objId;
                }
                else
                {
                    throw new DivideByZeroException("Pls enterz");
                }

                newRef.OnCreate("SYSTEM");
                range.Add(newRef);
            }
            ctx.ArticleReference.AddRange(range);
            ctx.SaveChanges();

            return RedirectToAction("edit", new { id = articleId });
        }

        public ActionResult DeleteArticleReference(int articleId, int articleReferenceId)
        {
            var toRemove = ctx.ArticleReference.Find(articleReferenceId);
            ctx.ArticleReference.Remove(toRemove);
            ctx.SaveChanges();

            return RedirectToAction("edit", new { id = articleId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ArticleViewModels.ArticleViewModel viewModel, int[] articleCategories)
        {
            Article newData = new Article
            {
                Id = viewModel.Id,
                AllowPublishment = viewModel.AllowPublishment,
                Name = viewModel.Name.Trim(),
                Thumbnail = viewModel.PhotoLink,
                PhotoLink = viewModel.PhotoLink,
                Description = viewModel.Description,
                ExpertTip = viewModel.ExpertTip,
                Price = viewModel.NormalPrice,
                PricePercentagePayableWithPoints = viewModel.PricePercentagePayableWithPoints,
                IsAvailable = viewModel.IsAvailable,
                EditedBy = User.Identity.GetUserName(),
                ProductLink = viewModel.ProductLink,
                IsNotDeliverable = viewModel.IsNotDeliverable,
                Brand = viewModel.Brand,
                VAT = viewModel.VAT,
                Author = viewModel.Author,
                EANCode = viewModel.EANCode,
                BulkArticle = viewModel.BulkArticle,
                WeightInGrams = viewModel.WeightInGrams,
                MakerId = viewModel.MakerId,
                HazardNotice = viewModel.HazardNotice,
                ArticleCategories = ac.DbGetArticleCategoriesBySelection(articleCategories)
            };

            HelperClasses.DbResponse response = ac.DbEditArticle(newData);

            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            viewModel.StatusMessage = statusMessage;

            HelperClasses.DbResponse imageResponse = or.DbGetArticleReferencedImages(viewModel.Id);
            viewModel.ArticleImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
            viewModel.PlantReferenceList = ctx.Plants;
            viewModel.TodotemplateReferenceList = ctx.TodoTemplate;
            ViewBag.ArticleCategories = ac.DbGetArticleCategoriesSelectionByArticleId(viewModel.Id);
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

        public ActionResult editArticleAllowPublishment(int? id, bool? allowPublishement)
        {
            HelperClasses.DbResponse response = ac.DbEditArticleAllowPublishment((int)id, User.Identity.GetUserName(), (bool)allowPublishement);
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Index");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("edit-image-creds")]
        public ActionResult EditImageCreds(int imageId, int objectId, string imageAuthor = null, string imageLicense = null)
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
            return RedirectToAction("Edit", new { id = objectId });
        }

        [ActionName("delete-article-image")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult deleteArticleImage(int imageId, int objectId)
        {
            HelperClasses.DbResponse response = or.DbDeleteFileReference(imageId, User.Identity.GetUserName());
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            statusMessage.Messages = Utilities.databaseMessagesToText(response.Messages);
            statusMessage.Status = response.Status;
            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("Edit", new { id = objectId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-article-image")]
        public ActionResult UploadArticleImage(HttpPostedFileBase imageFile, int objectId, string imageTitle = null, string imageDescription = null, string imageLicense = null, string imageAuthor = null)
        {
            bool isOk = UploadAndRegisterFile(imageFile, objectId, (int)ModelEnums.ReferenceToModelClass.Article, ModelEnums.FileReferenceType.ArticleImage, imageTitle, imageDescription, imageLicense, imageAuthor);

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
            return RedirectToAction("Edit", new { id = objectId });
        }
       
    }
}