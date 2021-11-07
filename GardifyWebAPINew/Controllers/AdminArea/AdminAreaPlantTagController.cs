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

namespace GardifyWebAPI.Controllers
{
	//[CustomAuthorizeAttribute(Roles = "Admin")]
	public class AdminAreaPlantTagController : Controller
	{

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
