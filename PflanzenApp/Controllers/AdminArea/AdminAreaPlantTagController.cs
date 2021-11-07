using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using GardifyModels.Models;
using PflanzenApp.App_Code;
using Microsoft.AspNet.Identity;
using System.Web;

namespace PflanzenApp.Controllers
{
    [CustomAuthorizeAttribute(Roles = "Admin")]
	public class AdminAreaPlantTagController : _BaseController
	{
        private ApplicationDbContext db = new ApplicationDbContext();
        // GET: /intern/plant-tag
        public ActionResult Index(int? categoryId)
		{
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
            PlantCharacteristicCategoryController pccc = new PlantCharacteristicCategoryController();
			AdminAreaViewModels.PlantTagViewModel plantTagViewModel = new AdminAreaViewModels.PlantTagViewModel();
			plantTagViewModel.CategoryList = ptcc.DbGetPlantTagCategories();

			PlantTagCategory selectedCategory = null;
			if (categoryId != null)
			{
				selectedCategory = ptcc.DbGetPlantTagCategoryById((int)categoryId);
			}
			else
			{
				selectedCategory = plantTagViewModel.CategoryList.FirstOrDefault();
			}

			plantTagViewModel.SelectedCategory = selectedCategory;
			plantTagViewModel.SelectedListEntryId = selectedCategory.Id;
			plantTagViewModel.PlantTags = selectedCategory.TagsInThisCategory.OrderBy(t => t.Title).Where(t => !t.Deleted);
			plantTagViewModel.CharacteristicCategories = pccc.DbGetPlantCharacteristicCategories();

			return View("~/Views/AdminArea/AdminAreaPlantTag/Index.cshtml", plantTagViewModel);
        }

        public ActionResult SetCategory(int categoryId, int? parentId)
        {
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
            var cat = ptcc.DbGetPlantTagCategoryById(categoryId);
            cat.ParentId = parentId;
            ptcc.DbEditPlantTagCategory(cat);
            return RedirectToAction("OrderCategories");
        }

        public ActionResult SetTagCategory(int tagId, int categoryId)
        {
            PlantTagController ptc = new PlantTagController();
            var tag = ptc.DbGetPlantTagById(tagId);
            tag.CategoryId = categoryId;
            ptc.DbEditPlantTag(tag);
            return RedirectToAction("OrderTags");
        }

        public ActionResult OrderCategories()
        {
            PlantTagCategoryController pcc = new PlantTagCategoryController();
            var tagCats = pcc.DbGetPlantTagCategoriesParents();
            return View("~/Views/AdminArea/AdminAreaPlantTag/OrderCategories.cshtml", tagCats);
        }

        public ActionResult OrderTags()
        {
            PlantTagCategoryController pcc = new PlantTagCategoryController();
            var tagCats = pcc.DbGetPlantTagCategoriesParents();
            return View("~/Views/AdminArea/AdminAreaPlantTag/OrderTags.cshtml", tagCats);
        }

        // GET: intern/plant-tag/Create
        public ActionResult Create(int? categoryId)
		{
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
			AdminAreaViewModels.PlantTagViewModel plantTagViewModel = new AdminAreaViewModels.PlantTagViewModel();
			plantTagViewModel.CategoryList = ptcc.DbGetPlantTagCategories();

			PlantTagCategory selectedCategory = null;
			if (categoryId != null)
			{
				selectedCategory = ptcc.DbGetPlantTagCategoryById((int)categoryId);
			}
			else
			{
				selectedCategory = plantTagViewModel.CategoryList.FirstOrDefault();
			}

			plantTagViewModel.PlantTag = new PlantTag();
			plantTagViewModel.PlantTag.CategoryId = selectedCategory.Id;
			plantTagViewModel.SelectedListEntryId = selectedCategory.Id;

			return View("~/Views/AdminArea/AdminAreaPlantTag/Create.cshtml", plantTagViewModel);
		}

		// POST: intern/plant-tag/Create
		// Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
		// finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Create([Bind(Include = "title,categoryId,tagImage")] PlantTag plantTag)
		{
            PlantTagController ptc = new PlantTagController();
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
			ModelState.Remove("plantTag.createdBy");
			if (ModelState.IsValid)
			{
				plantTag.CreatedBy = User.Identity.GetUserName();
				bool isOk = ptc.DbCreatePlantTag(plantTag);

				if (isOk)
				{
					return RedirectToAction("Index", new { categoryId = plantTag.CategoryId });
				}
			}

			AdminAreaViewModels.PlantTagViewModel plantTagViewModel = new AdminAreaViewModels.PlantTagViewModel();
			plantTagViewModel.PlantTag = plantTag;
			plantTagViewModel.CategoryList = ptcc.DbGetPlantTagCategories();

			return View("~/Views/AdminArea/AdminAreaPlantTag/Create.cshtml", plantTagViewModel);
		}

		// GET: intern/plant-tag/Edit/5
		public ActionResult Edit(int? id)
		{
            PlantTagController ptc = new PlantTagController();
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
			if (id == null)
			{
				return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
			}

			PlantTag plantTag = ptc.DbGetPlantTagById((int)id);
			if (plantTag == null)
			{
				return HttpNotFound();
			}

			AdminAreaViewModels.PlantTagViewModel plantTagViewModel = new AdminAreaViewModels.PlantTagViewModel();
			plantTagViewModel.PlantTag = plantTag;
			plantTagViewModel.CategoryList = ptcc.DbGetPlantTagCategories();
			plantTagViewModel.SelectedListEntryId = plantTag.CategoryId;
			return View("~/Views/AdminArea/AdminAreaPlantTag/Edit.cshtml", plantTagViewModel);
		}

		// POST: intern/plant-tag/Edit/5
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult Edit([Bind(Include = "id,categoryId,title,tagImage")] PlantTag plantTag)
		{
            PlantTagController ptc = new PlantTagController();
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
			ModelState.Remove("plantTag.createdBy");
			if (ModelState.IsValid)
			{
				plantTag.EditedBy = User.Identity.GetUserName();
				bool isOk = ptc.DbEditPlantTag(plantTag);

				if (isOk)
				{
					return RedirectToAction("Index", new { categoryId = plantTag.CategoryId });
				}
			}

			AdminAreaViewModels.PlantTagViewModel plantTagViewModel = new AdminAreaViewModels.PlantTagViewModel();
			plantTagViewModel.PlantTag = plantTag;
			plantTagViewModel.CategoryList = ptcc.DbGetPlantTagCategories();
			plantTagViewModel.SelectedListEntryId = plantTag.CategoryId;
			return View("~/Views/AdminArea/AdminAreaPlantTag/Edit.cshtml", plantTagViewModel);
		}

		// POST: intern/plant-tag/Delete/5
		[HttpPost, ActionName("Delete")]
		[ValidateAntiForgeryToken]
		public ActionResult DeleteConfirmed(int id, int categoryId)
        {
            PlantTagController ptc = new PlantTagController();
            ptc.DbDeletePlantTag(id, User.Identity.GetUserName());
			return RedirectToAction("Index", new { categoryId = categoryId });
		}

		#region PlantTagCategory

		// GET: intern/plant-tag/category-create
		[ActionName("category-create")]
		public ActionResult CategoryCreate()
		{
			return View("~/Views/AdminArea/AdminAreaPlantTag/CategoryCreate.cshtml");
		}

		// POST: intern/plant-tag/category-create
		[ActionName("category-create")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CategoryCreate(PlantTagCategory plantTagCategory)
		{
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
			ModelState.Remove("createdBy");
			if (ModelState.IsValid)
			{
                plantTagCategory.CreatedBy = User.Identity.GetUserName();
				ptcc.DbCreatePlantTagCategory(plantTagCategory);
				return RedirectToAction("Index");
			}
			return View("~/Views/AdminArea/AdminAreaPlantTag/CategoryCreate.cshtml", plantTagCategory);
		}

        // GET: intern/plant-tag/category-edit/5
        [ActionName("category-edit")]
        public ActionResult CategoryEdit(int? id)
        {
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlantTagCategory plantTagCategory = ptcc.DbGetPlantTagCategoryById((int)id);
            if (plantTagCategory == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/AdminArea/AdminAreaPlantTag/CategoryEdit.cshtml", plantTagCategory);
        }

		// POST: intern/plant-tag/category-edit
		[ActionName("category-edit")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CategoryEdit(PlantTagCategory plantTagCategory)
		{
            //add comment
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
            ModelState.Remove("createdBy");
			if (ModelState.IsValid)
			{
				plantTagCategory.EditedBy = User.Identity.GetUserName();
				ptcc.DbEditPlantTagCategory(plantTagCategory);
				return RedirectToAction("Index");
			}
			return View("~/Views/AdminArea/AdminAreaPlantTag/CategoryEdit.cshtml", plantTagCategory);
		}

        // POST: intern/plant-tag/category-delete/5
        [HttpPost, ActionName("category-delete")]
		[ValidateAntiForgeryToken]
		public ActionResult CategoryDelete(int id)
		{
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
            ptcc.DbDeletePlantTagCategory(id, User.Identity.GetUserName());
			return RedirectToAction("Index");
		}

        #endregion PlantTagCategory

        #region Super Categories
        [HttpPost]
        [ActionName("supercategory-add-group")]
        public ActionResult AddSuperCategoryGroup(int groupId, int catId)
        {
            var cat = db.PlantTagSuperCategory.Where(s => !s.Deleted && s.Id == catId).FirstOrDefault();
            var group = db.Groups.Where(g => !g.Deleted && g.Id == groupId).FirstOrDefault();
            cat.Groups.Add(group);

            db.SaveChanges();
            return RedirectToAction("supercategory-edit", new { id = catId });
        }

        public ActionResult DeleteSuperCategoryGroup(int groupId, int catId)
        {
            var cat = db.PlantTagSuperCategory.Where(s => !s.Deleted && s.Id == catId).FirstOrDefault();
            var group = cat.Groups.Where(g => !g.Deleted && g.Id == groupId).FirstOrDefault();
            cat.Groups.Remove(group);

            db.SaveChanges();
            return RedirectToAction("supercategory-edit", new { id = catId });
        }

        [HttpPost]
        [ActionName("supercategory-add-tag")]
        public ActionResult AddSuperCategoryTag(int tagId, int catId)
        {
            var cat = db.PlantTagSuperCategory.Where(s => !s.Deleted && s.Id == catId).FirstOrDefault();
            var tag = db.PlantTags.Where(g => !g.Deleted && g.Id == tagId).FirstOrDefault();
            cat.TagsInThisCategory.Add(tag);

            db.SaveChanges();
            return RedirectToAction("supercategory-edit", new { id = catId });
        }

        public ActionResult DeleteSuperCategoryTag(int tagId, int catId)
        {
            var cat = db.PlantTagSuperCategory.Where(s => !s.Deleted && s.Id == catId).FirstOrDefault();
            var tag = cat.TagsInThisCategory.Where(g => !g.Deleted && g.Id == tagId).FirstOrDefault();
            cat.TagsInThisCategory.Remove(tag);

            db.SaveChanges();
            return RedirectToAction("supercategory-edit", new { id = catId });
        }

        [HttpPost]
        [ActionName("supercategory-add-todo")]
        public ActionResult AddSuperCategoryTodo(int todoId, int catId)
        {
            var cat = db.PlantTagSuperCategory.Where(s => !s.Deleted && s.Id == catId).FirstOrDefault();
            var todo = db.TodoTemplate.Where(g => !g.Deleted && g.Id == todoId).FirstOrDefault();
            cat.Todos.Add(todo);

            db.SaveChanges();
            return RedirectToAction("supercategory-edit", new { id = catId });
        }

        public ActionResult DeleteSuperCategoryTodo(int todoId, int catId)
        {
            var cat = db.PlantTagSuperCategory.Where(s => !s.Deleted && s.Id == catId).FirstOrDefault();
            var todo = cat.Todos.Where(g => !g.Deleted && g.Id == todoId).FirstOrDefault();
            cat.Todos.Remove(todo);

            db.SaveChanges();
            return RedirectToAction("supercategory-edit", new { id = catId });
        }

        public ActionResult SuperCategoriesIndex()
        {
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
            var cats = ptcc.DbGetSuperCategories();
            return View("~/Views/AdminArea/AdminAreaPlantTag/SuperCategories.cshtml", cats.ToList());
        }

        // GET: intern/plant-tag/supercategory-create
        [ActionName("supercategory-create")]
        public ActionResult CreateSuperCategory()
        {
            return View("~/Views/AdminArea/AdminAreaPlantTag/CreateSuperCategory.cshtml", new PlantTagSuperCategory());
        }

        // POST: intern/plant-tag/supercategory-create
        [ActionName("supercategory-create")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSuperCategory(PlantTagSuperCategory plantTagCategory)
        {
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
            ModelState.Remove("createdBy");
            if (ModelState.IsValid)
            {
                plantTagCategory.CreatedBy = User.Identity.GetUserName();
                ptcc.DbCreatePlantTagSuperCategory(plantTagCategory);
                return RedirectToAction("supercategory-edit", new { plantTagCategory.Id });
            }
            return View("~/Views/AdminArea/AdminAreaPlantTag/CreateSuperCategory.cshtml", plantTagCategory);
        }

        // POST: intern/plant-tag/supercategory-edit
        [ActionName("supercategory-edit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditSuperCategory(PlantTagSuperCategory plantTagCategory)
        {
            //add comment
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
            ModelState.Remove("createdBy");
            if (ModelState.IsValid)
            {
                plantTagCategory.EditedBy = User.Identity.GetUserName();
                ptcc.DbEditPlantTagSuperCategory(plantTagCategory);
                return RedirectToAction("Index");
            }
            return View("~/Views/AdminArea/AdminAreaPlantTag/SuperCategoryEdit.cshtml", plantTagCategory);
        }

        // GET: intern/plant-tag/supercategory-edit/5
        [ActionName("supercategory-edit")]
        public ActionResult EditSuperCategory(int? id)
        {
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            PlantTagSuperCategory plantTagCategory = ptcc.DbGetPlantTagSuperCategoryById((int)id);

            ViewBag.Groups = db.Groups.Where(g => !g.Deleted).OrderBy(i => i.Name).ToList();
            ViewBag.Todos = db.TodoTemplate.Where(g => !g.Deleted).OrderBy(i => i.Title).ToList();
            ViewBag.Tags = db.PlantTags.Where(g => !g.Deleted).OrderBy(i => i.Title).ToList();
            ViewBag.Categories = db.PlantTagCategory.Where(g => !g.Deleted).OrderBy(i => i.Title).ToList();
            HelperClasses.DbResponse imageResponse = rc.DbGetPlantTagReferencedImages((int)id);
            ViewBag.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));

            if (plantTagCategory == null)
            {
                return HttpNotFound();
            }
            return View("~/Views/AdminArea/AdminAreaPlantTag/SuperCategoryEdit.cshtml", plantTagCategory);
        }

        // POST: intern/plant-tag/supercategory-delete/5
        [HttpPost, ActionName("supercategory-delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteSuperCategory(int id)
        {
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
            ptcc.DbDeletePlantTagSuperCategory(id, User.Identity.GetUserName());
            return RedirectToAction("SuperCategoriesIndex");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-supercat-image")]
        public ActionResult UploadSuperCatImage(HttpPostedFileBase imageFile, int catId, string imageTitle = null, string imageDescription = null, string imageLicense = null, string imageAuthor = null)
        {
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            if (imageDescription == null || imageDescription == "")
            {
                imageDescription = "Keine Beschreibung vorhanden.";
            }
            if (imageFile == null || imageFile.ContentLength <= 0)
            {
                statusMessage.Messages.Add("Fehler beim Upload");
                statusMessage.Status = ModelEnums.ActionStatus.Error;
                //return
            }
            else
            {
                UploadAndRegisterFile(imageFile, catId, (int)ModelEnums.ReferenceToModelClass.PlantTag, ModelEnums.FileReferenceType.TagImage, imageTitle, imageDescription, imageLicense, imageAuthor);
            }

            TempData["statusMessage"] = statusMessage;

            return RedirectToAction("supercategory-edit", new { id = catId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("delete-supercat-image")]
        public ActionResult DeleteSuperCatImage(int imageId, int catId)
        {
            //dirty way:
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            try
            {
                nfilesEntities nfilesEntities = new nfilesEntities();
                var image = nfilesEntities.Files.FirstOrDefault(e => e.FileID == imageId);
                var ftm = nfilesEntities.FileToModule.FirstOrDefault(e => e.FileID == imageId);
                nfilesEntities.FileToModule.Remove(ftm);
                nfilesEntities.Files.Remove(image);
                nfilesEntities.SaveChanges();
                statusMessage.Messages = new string[] { "Bild gelöscht" }.ToList();
                statusMessage.Status = ModelEnums.ActionStatus.Success;
            }
            catch
            {
                statusMessage.Messages = new string[] { "Fehler beim löschen" }.ToList();
                statusMessage.Status = ModelEnums.ActionStatus.Error;
            }
            TempData["statusMessage"] = statusMessage;
            return RedirectToAction("supercategory-edit", new { id = catId });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("edit-supercat-creds")]
        public ActionResult EditImageCreds(int imageId, int catId, string imageAuthor = null, string imageLicense = null)
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
            return RedirectToAction("supercategory-edit", new { id = catId });
        }

        #endregion Super Categories

        #region plant characteristic category

        // GET: intern/plant-tag/category-create
        [ActionName("characteristic-category-create")]
		public ActionResult CaracteristicCategoryCreate()
		{
			return View("~/Views/AdminArea/AdminAreaPlantTag/PlantCharacteristicCategoryCreate.cshtml");
		}

		// POST: intern/plant-tag/characteristic-category-create
		[ActionName("characteristic-category-create")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CaracteristicCategoryCreate(PlantCharacteristicCategory newCategory)
		{
            PlantCharacteristicCategoryController pccc = new PlantCharacteristicCategoryController();
            ModelState.Remove("createdBy");
			if (ModelState.IsValid)
			{
				newCategory.CreatedBy = User.Identity.GetUserName();
				pccc.DbCreatePlantCharacteristicCategory(newCategory);
				return RedirectToAction("Index");
			}
			return View("~/Views/AdminArea/AdminAreaPlantTag/PlantCharacteristicCategoryCreate.cshtml", newCategory);
		}


		[ActionName("characteristic-category-edit")]
		public ActionResult CharacteristicCategoryEdit(int id)
		{
            PlantCharacteristicCategoryController pccc = new PlantCharacteristicCategoryController();
            PlantCharacteristicCategory cat = pccc.DbGetPlantCharacteristicCategoryById(id);
			if(cat != null)
			{
				return View("~/Views/AdminArea/AdminAreaPlantTag/PlantCharacteristicCategoryEdit.cshtml", cat);

			}
			return RedirectToAction("Index");			
		}


		[ActionName("characteristic-category-edit")]
		[HttpPost]
		[ValidateAntiForgeryToken]
		public ActionResult CharacteristicCategoryEdit(PlantCharacteristicCategory categoryToEdit)
		{
            PlantCharacteristicCategoryController pccc = new PlantCharacteristicCategoryController();
            if (pccc.DbEditPlantCharacteristicCategory(categoryToEdit))
			{
				return RedirectToAction("Index");
			}
			return View("~/Views/AdminArea/AdminAreaPlantTag/PlantCharacteristicCategoryEdit.cshtml", categoryToEdit);
		}

		// POST: intern/plant-tag/characteristic-category-delete/5
		[HttpPost, ActionName("characteristic-category-delete")]
		[ValidateAntiForgeryToken]
		public ActionResult characteristicCategoryDelete(int id)
		{
            PlantCharacteristicCategoryController pccc = new PlantCharacteristicCategoryController();
            pccc.DbDeletePlantCharacteristicCategory(id, User.Identity.GetUserName());
			return RedirectToAction("Index");
		}

		#endregion plant characteristic category
	}
}
