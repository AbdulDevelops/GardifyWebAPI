using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using static GardifyModels.Models.ModelEnums;
using static GardifyModels.Models.TodoTemplateViewModels;

namespace GardifyWebAPI.Controllers.AdminArea
{
    public class AdminAreaTodoController : _BaseController
    {
        // GET: AdminAreaTodo
        public ActionResult Index()
        {
            var vm = GetTodoTemplateIndexViewModel();
            return View("~/Views/AdminArea/AdminAreaTodo/Index.cshtml", vm);
        }
        public ActionResult Create()
        {
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            TodoController tc = new TodoController();
            var refs = DbGetReferencedItems();
            var tax = ttc.DbGetAllNodes();
            var vm = new TodoTemplateViewModels.TodoTemplateCreateViewModel
            {
                InfoObjects = refs,
                TaxonomicTreeObjects = tax
            };
            return View("~/Views/AdminArea/AdminAreaTodo/Create.cshtml", vm);
        }

        [HttpPost]
        public ActionResult Create(TodoTemplateCreateViewModel vm, int? plantCallback) //AIDSCODE
        {
            if (ModelState.IsValid)
            {
                DbCreateTodoTemplate(vm);
                if (plantCallback != null)
                {
                    return RedirectToAction("Edit", "AdminAreaPlant", new { id = plantCallback });
                }
                return RedirectToAction("Index");
            }
            else
            {
                vm.InfoObjects = DbGetReferencedItems();
                ModelState.AddModelError(string.Empty, "Bitte geben Sie korrekte Werte ein.");
                if (plantCallback != null)
                {
                    return RedirectToAction("Edit", "AdminAreaPlant", new { id = plantCallback });
                }
                return View("~/Views/AdminArea/AdminAreaTodo/Create.cshtml", vm);
                //throw new Exception("Das übergebene Model ist nicht korrekt", HttpStatusCode.InternalServerError, "AdminAreaTodoController.Create(" + vm.Title + ")");
            }
        }

        public ActionResult AddDropDown(int index)
        {
            TodoTemplateCreateViewModel vm = new TodoTemplateCreateViewModel();
            vm.Index = index;
            vm.InfoObjects = DbGetReferencedItems().Where(x => !string.IsNullOrEmpty(x.Name));
            return PartialView("~/Views/AdminArea/AdminAreaTodo/DropdownPartial.cshtml", vm);
        }

        public ActionResult Edit(int id)
        {
            var evm = GetEditViewModel(id);
            return View("~/Views/AdminArea/AdminAreaTodo/Edit.cshtml", evm);
        }

        [HttpPost]
        public ActionResult Edit(TodoTemplateViewModels.TodoTemplateEditViewModel vm)
        {
            if (ModelState.IsValid)
            {
                DbEditTodoTemplate(vm);
                return RedirectToAction("Index");
            }
            else
            {
                throw new Exception("Das übergebene Model ist nicht korrekt");//, HttpStatusCode.InternalServerError, "AdminAreaTodoController.Edit(" + vm.Title + ")");
            }
        }


        public ActionResult Delete(int id)
        {
            var dvm = GetDeleteViewModel(id);
            return View("~/Views/AdminArea/AdminAreaTodo/Delete.cshtml", dvm);
        }

        [HttpPost]
        public ActionResult Delete(TodoTemplateViewModels.TodoTemplateDeleteViewModel vm)
        {
            if (ModelState.IsValid)
            {
                DbDeleteTodoTemplate(vm);
                return RedirectToAction("Index");
            }
            else
            {
                throw new Exception("Das übergebene Model ist nicht korrekt");//, HttpStatusCode.InternalServerError, "AdminAreaTodoController.Delete(" + vm.Title + ")");
            }
        }

        #region DB

        private TodoTemplateViewModels.TodoTemplateDeleteViewModel GetDeleteViewModel(int id)
        {
            var mod = DbGetTodoTemplate(id);
            TodoTemplateViewModels.TodoTemplateDeleteViewModel vm = new TodoTemplateViewModels.TodoTemplateDeleteViewModel()
            {
                Id = mod.Id,
                Title = mod.Title
            };
            return vm;
        }

        private void DbDeleteTodoTemplate(TodoTemplateViewModels.TodoTemplateDeleteViewModel vm)
        {
            var todo = (from td in plantDB.TodoTemplate
                        where td.Deleted == false && td.Id == vm.Id
                        select td).FirstOrDefault();
            todo.Deleted = true;
            todo.OnEdit(Utilities.GetUserName());
            plantDB.SaveChanges();
        }

        private void DbEditTodoTemplate(TodoTemplateViewModels.TodoTemplateEditViewModel vm)
        {
            var todo = (from td in plantDB.TodoTemplate
                        where td.Deleted == false && td.Id == vm.Id
                        select td).FirstOrDefault();
            todo.Cycle = vm.Cycle;
            todo.DateStart = vm.DateStart;
            todo.DateEnd = vm.DateEnd;
            todo.ReferenceId = vm.ReferenceId[0];
            todo.ReferenceType = vm.ReferenceType;
            todo.Title = vm.Title;
            todo.Description = vm.Description;
            todo.OnEdit(Utilities.GetUserName());
            plantDB.SaveChanges();

            if (vm.ReferenceId.Count() > 1)
            {
                //TODO: Add TaxonomicTree properties to edit view
                TodoTemplateCreateViewModel todoTemplateCreateViewModel = new TodoTemplateCreateViewModel
                {
                    ReferenceId = vm.ReferenceId.Skip(1).ToArray(),
                    CurrentTodoCount = vm.CurrentTodoCount,
                    Cycle = vm.Cycle,
                    DateEnd = vm.DateEnd,
                    DateStart = vm.DateStart,
                    Description = vm.Description,
                    InfoObjects = vm.InfoObjects,
                    NewMessages = vm.NewMessages,
                    PlantCount = vm.PlantCount,
                    Points = vm.Points,
                    ReferenceType = vm.ReferenceType,
                    ShopcartCounter = vm.ShopcartCounter,
                    Title = vm.Title
                };
                DbCreateTodoTemplate(todoTemplateCreateViewModel);
            }
        }

        private TodoTemplateViewModels.TodoTemplateEditViewModel GetEditViewModel(int id)
        {
            var mod = DbGetTodoTemplate(id);
            TodoTemplateViewModels.TodoTemplateEditViewModel vm = new TodoTemplateViewModels.TodoTemplateEditViewModel()
            {
                Cycle = mod.Cycle,
                Id = mod.Id,
                DateStart = mod.DateStart,
                DateEnd = mod.DateEnd,
                ReferenceType = mod.ReferenceType,
                Title = mod.Title,
                ReferenceId = new int[] { mod.ReferenceId },
                Description = mod.Description,
                InfoObjects = DbGetReferencedItems()
            };
            return vm;
        }


        public void DbCreateTodoTemplate(TodoTemplateCreateViewModel vm)
        {
            if (vm.ReferenceType == ReferenceToModelClass.Plant)
            {
                foreach (var m in vm.ReferenceId.Where(x => x != -1))
                {
                    TodoTemplate newTemplate = new TodoTemplate()
                    {
                        Cycle = vm.Cycle,
                        DateStart = vm.DateStart,
                        DateEnd = vm.DateEnd,
                        Description = vm.Description,
                        Precision = TodoPrecisionType.Date,
                        ReferenceId = m,
                        ReferenceType = vm.ReferenceType,
                        Title = vm.Title
                    };
                    newTemplate.OnCreate(Utilities.GetUserName());
                    plantDB.TodoTemplate.Add(newTemplate);
                }
            }
            if (vm.ReferenceType == ReferenceToModelClass.TaxonomicTree)
            {
                TaxonomicTreeController ttc = new TaxonomicTreeController();
                var plants = ttc.DbGetAllPlantsUnderTree(vm.TaxonomicTreeId);
                foreach (var p in plants)
                {
                    TodoTemplate newTemplate = new TodoTemplate()
                    {
                        Cycle = vm.Cycle,
                        DateStart = vm.DateStart,
                        DateEnd = vm.DateEnd,
                        Description = vm.Description,
                        Precision = TodoPrecisionType.Date,
                        ReferenceId = p.Id,
                        ReferenceType = ReferenceToModelClass.Plant,
                        Title = vm.Title,
                        TaxonomicTreeId = vm.TaxonomicTreeId
                    };
                    newTemplate.OnCreate(Utilities.GetUserName());
                    plantDB.TodoTemplate.Add(newTemplate);
                }
            }
            plantDB.SaveChanges();
        }
        public List<IReferencedObject> DbGetReferencedItems()
        {
            List<IReferencedObject> items = new List<IReferencedObject>();
            PlantController pc = new PlantController();
            var plants = pc.DbGetPlantList();
            items.AddRange(plants);
            return items;
        }

        public TodoTemplateViewModels.TodoTemplateIndexViewModel GetTodoTemplateIndexViewModelByPlantId(int id)
        {
            var vm = GetTodoTemplateIndexViewModel();
            vm.TodoTemplates = vm.TodoTemplates.Where(v => v.ReferenceId == id && v.ReferenceType == ModelEnums.ReferenceToModelClass.Plant);
            return vm;
        }



        public TodoTemplateViewModels.TodoTemplateIndexViewModel GetTodoTemplateIndexViewModel()
        {
            List<TodoTemplateViewModels.TodoTemplateDetailsViewModel> list = new List<TodoTemplateViewModels.TodoTemplateDetailsViewModel>();
            var todos = DbGetTodoTemplates();
            foreach (var todo in todos)
            {
                list.Add(GetTodoTemplateDetailViewModel(todo.Id));
            }
            return new TodoTemplateViewModels.TodoTemplateIndexViewModel { TodoTemplates = list };
        }

        public TodoTemplateViewModels.TodoTemplateDetailsViewModel GetTodoTemplateDetailViewModel(int id)
        {
            var template = DbGetTodoTemplate(id);
            TodoTemplateViewModels.TodoTemplateDetailsViewModel newModel = new TodoTemplateViewModels.TodoTemplateDetailsViewModel()
            {
                Cycle = template.Cycle,
                DateStart = template.DateStart,
                DateEnd = template.DateEnd,
                Description = template.Description,
                ReferenceId = template.ReferenceId,
                ReferenceType = template.ReferenceType,
                Title = template.Title,
                Id = template.Id
            };
            if (newModel.ReferenceType == ReferenceToModelClass.Plant)
            {
                newModel.ReferenceName = plantDB.Plants.Where(p => !p.Deleted && p.Id == newModel.ReferenceId).FirstOrDefault().Name;
            }
            return newModel;
        }

        public IEnumerable<TodoTemplate> DbGetTodoTemplates()
        {
            var templates = (from t in plantDB.TodoTemplate
                             where !t.Deleted
                             select t);
            return templates;
        }

        public IEnumerable<TodoTemplate> DbGetTodoTemplates(int referenceId, ReferenceToModelClass type)
        {
            var templates = (from t in plantDB.TodoTemplate
                             where !t.Deleted
                             && t.ReferenceId == referenceId && t.ReferenceType == type
                             select t);
            return templates;
        }

        public TodoTemplate DbGetTodoTemplate(int id)
        {
            var template = (from t in plantDB.TodoTemplate
                            where !t.Deleted
                            && t.Id == id
                            select t).FirstOrDefault();
            return template;
        }
        #endregion
    }
}