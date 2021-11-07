using PflanzenApp.App_Code;
using GardifyModels.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using static GardifyModels.Models.TodoTemplateViewModels;
using static GardifyModels.Models.ModelEnums;
using System;
using Microsoft.Ajax.Utilities;
using System.Security.Policy;

namespace PflanzenApp.Controllers.AdminArea
{
    public class AdminAreaTodoController : _BaseController
    {
        // GET: AdminAreaTodo
        public ActionResult Index()
        {
            var vm = GetTodoTemplateIndexViewModel();
            return View("~/Views/AdminArea/AdminAreaTodo/Index.cshtml", vm);
        }
        public ActionResult Create(int selectedTaxonId = -1, int selectedPlant = -1, string selectedPlantName = "")
        {
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            TodoController tc = new TodoController();
            var refs = DbGetReferencedItems();
            var tax = ttc.DbGetAllNodes();
            var vm = new TodoTemplateViewModels.TodoTemplateCreateViewModel
            {
                InfoObjects = refs,
                TaxonomicTreeObjects = tax,
                SelectedPlantId = selectedPlant,
                SelectedPlantName = selectedPlantName,
                SelectedTaxonId = selectedTaxonId
            };
            return View("~/Views/AdminArea/AdminAreaTodo/Create.cshtml", vm);
        }

        [HttpPost]
        public ActionResult Create(TodoTemplateCreateViewModel vm, int? plantCallback, string articlesRef = "") //AIDSCODE
        {
            TaxonomicTreeController ttc = new TaxonomicTreeController();

            //Update dates to display correct data while ignoring the year and set the values from the range [Anfang, Mitte, Ende] as Day parameters
            if (vm.DateStartDay == 0 || vm.DateStartMonth == 0)
            {
                vm.InfoObjects = DbGetReferencedItems();
                vm.TaxonomicTreeObjects = ttc.DbGetAllNodes();
                return View("~/Views/AdminArea/AdminAreaTodo/Create.cshtml", vm);
            }
            else
            {
                vm.DateStart = new DateTime(DateTime.Now.Year, vm.DateStartMonth, vm.DateStartDay);
                if (vm.DateEndMonth != 0 && vm.DateEndDay != 0)
                {
                    vm.DateEnd = new DateTime(DateTime.Now.Year, vm.DateEndMonth, vm.DateEndDay);
                }
                else
                {
                    vm.DateEnd = vm.DateStart;
                }
            }
            if (ModelState.IsValid)
            {
                DbCreateTodoTemplate(vm, articlesRef);
                if (plantCallback != null)
                {
                    return RedirectToAction("Edit", "AdminAreaPlant", new { id = plantCallback });
                }
                return RedirectToAction("Index");
            }
            else
            {
                vm.InfoObjects = DbGetReferencedItems();
                vm.TaxonomicTreeObjects = ttc.DbGetAllNodes();
                ModelState.AddModelError(string.Empty, "Bitte geben Sie korrekte Werte ein.");
                if (plantCallback != null)
                {
                    return RedirectToAction("Edit", "AdminAreaPlant", new { id = plantCallback });
                }
                return View("~/Views/AdminArea/AdminAreaTodo/Create.cshtml", vm);
                //return RedirectToError("Das übergebene Model ist nicht korrekt", HttpStatusCode.InternalServerError, "AdminAreaTodoController.Create(" + vm.Title + ")");
            }
        }

        public ActionResult AddDropDown(int index)
        {
            TodoTemplateCreateViewModel vm = new TodoTemplateCreateViewModel();
            vm.Index = index;
            vm.InfoObjects = DbGetReferencedItems().Where(x => !string.IsNullOrEmpty(x.Name));
            return PartialView("~/Views/AdminArea/AdminAreaTodo/DropdownPartial.cshtml", vm);
        }

        public ActionResult BulkEdit(int id)
        {

            var bvm = GetBulkViewModel(id);

            if (bvm == null)
            {
                return RedirectToAction("Index");
            }

            return View("~/Views/AdminArea/AdminAreaTodo/BulkEdit.cshtml", bvm);

        }
        [HttpPost]

        public ActionResult BulkEdit(TodoTemplateViewModels.TodoTemplateEditViewModel vm)
        {
            var bvm = GetBulkViewModel(vm.Id);

            if (vm.DateStartDay == 0 || vm.DateStartMonth == 0)
            {

                return View("~/Views/AdminArea/AdminAreaTodo/BulkEdit.cshtml", bvm);
            }
            else
            {
                vm.DateStart = new DateTime(DateTime.Now.Year, vm.DateStartMonth, vm.DateStartDay);
                if (vm.DateEndMonth != 0 && vm.DateEndDay != 0)
                {
                    vm.DateEnd = new DateTime(DateTime.Now.Year, vm.DateEndMonth, vm.DateEndDay);
                }
                else
                {
                    vm.DateEnd = vm.DateStart;
                }
            }
            if (ModelState.IsValid)
            {
                foreach(var plantTemplate in bvm.BulkTodoTemplates)
                {
                    TodoTemplateViewModels.TodoTemplateEditViewModel newVm = new TodoTemplateEditViewModel
                    {
                        Title = vm.Title,
                        Description = vm.Description,
                        DateStart = vm.DateStart,
                        DateEnd = vm.DateEnd,
                        Cycle = vm.Cycle,
                        ReferenceType = vm.ReferenceType,
                        ReferenceId = new int[] { plantTemplate.plantId },
                        Id = plantTemplate.TemplateId
                    };

                    DbEditTodoTemplate(newVm);

                }

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToError("Das übergebene Model ist nicht korrekt", HttpStatusCode.InternalServerError, "AdminAreaTodoController.BulkEdit(" + vm.Title + ")");
            }


        }

        [HttpPost]
        public ActionResult BulkEditAdd(TodoTemplateViewModels.TodoTemplateBulkAddViewModel vm)
        {
            var bvm = GetBulkViewModel(vm.BaseId, false);

            foreach(var id in vm.ReferenceId)
            {
                if (bvm.BulkTodoTemplates.Any(t => t.plantId == id))
                {
                   continue;
                }

                int[] updatedReferenceId = { bvm.ReferenceId.First(), id };

                TodoTemplateViewModels.TodoTemplateEditViewModel addvm = new TodoTemplateEditViewModel
                {
                    Cycle = bvm.Cycle,
                    DateStart = bvm.DateStart,
                    DateEnd = bvm.DateEnd,
                    Title = bvm.Title,
                    Description = bvm.Description,
                    ReferenceId = updatedReferenceId,
                    CurrentTodoCount = bvm.CurrentTodoCount,
                    InfoObjects = bvm.InfoObjects,
                    NewMessages = bvm.NewMessages,
                    PlantCount = bvm.PlantCount,
                    Points = bvm.Points,
                    ReferenceType = bvm.ReferenceType,
                    ShopcartCounter = bvm.ShopcartCounter
                };



                DbEditTodoTemplate(addvm);
            }

            //if (bvm.BulkTodoTemplates.Any(t => t.plantId == vm.ReferenceId.First()))
            //{
            //    return RedirectToAction("BulkEdit", new { id = vm.BaseId });
            //}


           

            return RedirectToAction("BulkEdit", new { id = vm.BaseId });
        }

            public ActionResult Edit(int id)
        {

            return RedirectToAction("BulkEdit", new { id = id });
            //setRelatedTodoTemplateIdToCyclicTodo();
            //setRelatedTodoTemplateIdToSingleTodo();
            var evm = GetEditViewModel(id);
            ViewBag.ArticleReferences = ctx.ArticleReference.Where(r => !r.Deleted && r.TodoTemplateId == id).ToList();
            ViewBag.ArticlesList = ctx.Articles.Where(r => !r.Deleted).ToList();
            return View("~/Views/AdminArea/AdminAreaTodo/Edit.cshtml", evm);
        }
        public void setRelatedTodoTemplateIdToCyclicTodo()
        {
            var todoCylics = from t in ctx.TodoCyclic where !t.Deleted && t.ReferenceId!=0 && t.GeneratedFromTemplate && t.RelatedTodoTemplateId == 0 select t;
            if(todoCylics!=null && todoCylics.Any())
            {
                foreach (TodoCyclic td in todoCylics.ToList())
                {
                    var templateInfo = FindTodoTemplateByID(td.Title);
                    if (templateInfo != null)
                    {
                        td.RelatedTodoTemplateId = templateInfo.Id;
                    }
                    else
                    {
                        td.RelatedTodoTemplateId = -1;
                    }
                    
                }
                ctx.SaveChanges();
            }
           
        }
        public void setRelatedTodoTemplateIdToSingleTodo()
        {
            var todo = (from t in ctx.Todoes where !t.Deleted && t.ReferenceId!=0 && t.RelatedTodoTemplateId == 0 select t);
            if (todo != null && todo.Any())
            {
                foreach (Todo td in todo.ToList())
                {
                    var templateInfo = FindTodoTemplateByID(td.Title);
                    if (templateInfo != null)
                    {
                        td.RelatedTodoTemplateId = templateInfo.Id;
                    }
                    else
                    {
                        td.RelatedTodoTemplateId = -1;
                    }
                }
                ctx.SaveChanges();
            }

        }
        public TodoTemplate FindTodoTemplateByID(string title)
        {
            TodoTemplate todoTemplate = ctx.TodoTemplate.Where(temp => !temp.Deleted && temp.Title.Equals(title)).FirstOrDefault();
            return todoTemplate;
        }
        [HttpPost]
        public ActionResult Edit(TodoTemplateViewModels.TodoTemplateEditViewModel vm, string addArticle = "", int articleId = 0)
        {
           
            if (!String.IsNullOrEmpty(addArticle))
            {
                return AddArticleReference(articleId, vm.Id);
            }
            //Update dates to display correct data while ignoring the year and set the values from the range [Anfang, Mitte, Ende] as Day parameters
            if (vm.DateStartDay == 0 || vm.DateStartMonth == 0)
            {
                var evm = GetEditViewModel(vm.Id);
                ViewBag.ArticleReferences = ctx.ArticleReference.Where(r => !r.Deleted && r.TodoTemplateId == vm.Id).ToList();
                ViewBag.ArticlesList = ctx.Articles.Where(r => !r.Deleted).ToList();
                //return RedirectToAction("edit", new { id = vm.Id });
                return View("~/Views/AdminArea/AdminAreaTodo/edit.cshtml", evm);
            }
            else
            {
                vm.DateStart = new DateTime(DateTime.Now.Year, vm.DateStartMonth, vm.DateStartDay);
                if (vm.DateEndMonth != 0 && vm.DateEndDay != 0)
                {
                    vm.DateEnd = new DateTime(DateTime.Now.Year, vm.DateEndMonth, vm.DateEndDay);
                }
                else
                {
                    vm.DateEnd = vm.DateStart;
                }
            }
            if (ModelState.IsValid)
            {
                DbEditTodoTemplate(vm);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToError("Das übergebene Model ist nicht korrekt", HttpStatusCode.InternalServerError, "AdminAreaTodoController.Edit(" + vm.Title + ")");
            }
        }

        public ActionResult DeleteArticleReference(int articleId, int articleReferenceId)
        {
            var toRemove = ctx.ArticleReference.Where(r => r.ArticleId == articleId && r.TodoTemplateId == articleReferenceId).FirstOrDefault();
            if (toRemove != null)
            {
                ctx.ArticleReference.Remove(toRemove);
                ctx.SaveChanges();
            }

            return RedirectToAction("edit", new { id = articleReferenceId });
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
                    DbRemoveOrphanTodos(vm.Id);

               
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToError("Das übergebene Model ist nicht korrekt", HttpStatusCode.InternalServerError, "AdminAreaTodoController.Delete(" + vm.Title + ")");
            }
        }

        public ActionResult BulkDelete(int id)
        {
            var dvm = GetDeleteViewModel(id);
            return View("~/Views/AdminArea/AdminAreaTodo/BulkDelete.cshtml", dvm);
        }

        [HttpPost]
        public ActionResult BulkDelete(TodoTemplateViewModels.TodoTemplateDeleteViewModel vm)
        {
            var bvm = GetBulkViewModel(vm.Id);


            if (ModelState.IsValid)
            {

                foreach(var item in bvm.BulkTodoTemplates)
                {
                    TodoTemplateDeleteViewModel newvm = new TodoTemplateDeleteViewModel()
                    {
                        Id = item.TemplateId,
                        Title = item.Title
                    };
                    DbDeleteTodoTemplate(newvm);
                    DbRemoveOrphanTodos(newvm.Id);
                }
                


                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToError("Das übergebene Model ist nicht korrekt", HttpStatusCode.InternalServerError, "AdminAreaTodoController.Delete(" + vm.Title + ")");
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
            var todo = (from td in ctx.TodoTemplate
                        where td.Deleted == false && td.Id == vm.Id
                        select td).FirstOrDefault();

            if (todo == null)
            {
                return;
            }
            todo.Deleted = true;
            todo.OnEdit(Utilities.GetUserName());
            ctx.SaveChanges();

            if (todo.ReferenceType == ReferenceToModelClass.TaxonomicTree)
            {
                var templates = (from td in ctx.TodoTemplate
                                 where td.Deleted == false && td.TaxonomicReferenceTemplateId == vm.Id
                                 select td).ToList();

                foreach(var template in templates)
                {
                    todo.Deleted = true;
                    todo.OnEdit(Utilities.GetUserName());
                }
                ctx.SaveChanges();

            }
        }

        private void DbRemoveOrphanTodos(int todoTemplateId)
        {
            var template = ctx.TodoTemplate.Where(t => t.Id == todoTemplateId).FirstOrDefault();

            var plant = ctx.Plants.Where(t => t.Id == template.ReferenceId).FirstOrDefault();

            if (plant == null)
            {
                return;
            }

            var cyclics = (from t in ctx.TodoCyclic
                           join up in ctx.UserPlants on t.ReferenceId equals up.Id 
                           where t.ReferenceType == ReferenceToModelClass.UserPlant && !t.Deleted && up.PlantId == plant.Id && t.Title == template.Title
                           select t);

            var todos = (from t in ctx.Todoes
                         join tc in cyclics on t.CyclicId equals tc.Id
                         where !t.Deleted
                         select t);

            foreach (TodoCyclic t in cyclics)
            {
                t.Deleted = true;
            }
            foreach (Todo t in todos)
            {
                t.Deleted = true;
            }
            ctx.SaveChanges();
        }

        private void DbEditTodoTemplate(TodoTemplateViewModels.TodoTemplateEditViewModel vm)
        {
            var todoTemplate = (from td in ctx.TodoTemplate
                        where td.Deleted == false && td.Id == vm.Id
                        select td).FirstOrDefault();
            var oldTitle = todoTemplate!=null?todoTemplate.Title: "";
           

            if (todoTemplate != null && todoTemplate.ReferenceType != ReferenceToModelClass.TaxonomicTree)
            {



                    todoTemplate.Cycle = vm.Cycle;
                    todoTemplate.DateStart = vm.DateStart;
                    todoTemplate.DateEnd = vm.DateEnd;
                    todoTemplate.ReferenceId = vm.ReferenceId[0];
                    todoTemplate.ReferenceType = vm.ReferenceType;
                    todoTemplate.Title = vm.Title;
                    todoTemplate.Description = vm.Description;
                    todoTemplate.Edited = true;
                    todoTemplate.OnEdit(Utilities.GetUserName());
               
                //ctx.SaveChanges();
            }
            else if(todoTemplate != null && todoTemplate.ReferenceType == ReferenceToModelClass.TaxonomicTree)
            {
                todoTemplate.Cycle = vm.Cycle;
                todoTemplate.DateStart = vm.DateStart;
                todoTemplate.DateEnd = vm.DateEnd;
                todoTemplate.Title = vm.Title;
                todoTemplate.Description = vm.Description;
                todoTemplate.Edited = true;
                todoTemplate.OnEdit(Utilities.GetUserName());

                ctx.SaveChanges();
                AdviceUserAfterEditTodo();

                var templates = (from td in ctx.TodoTemplate
                                 where td.Deleted == false && td.TaxonomicReferenceTemplateId == vm.Id
                                 select td).ToList();

                foreach (var template in templates)
                {
                    template.Cycle = vm.Cycle;
                    template.DateStart = vm.DateStart;
                    template.DateEnd = vm.DateEnd;
                    template.ReferenceId = vm.ReferenceId[0];
                    template.Title = vm.Title;
                    template.Description = vm.Description;
                    template.Edited = true;
                    template.OnEdit(Utilities.GetUserName());

                }

                ctx.SaveChanges();

                return;
            }

            // update related user Cyclic-todos only if user confirm
            //var userCyclics = ctx.TodoCyclic.Where(cyc => !cyc.Deleted && cyc.Title == oldTitle);

            //if (userCyclics!=null && userCyclics.Any())
            //{
            //    foreach (TodoCyclic cyc in userCyclics)
            //    {
            //        cyc.Cycle = vm.Cycle;
            //        cyc.Description = vm.Description;
            //        cyc.Title = vm.Title;
            //        cyc.DateStart = vm.DateStart;
            //        cyc.DateEnd = vm.DateEnd;
            //    }
            //}

            //do not delete Populated user's Todos anymore
            //var userTodos = ctx.Todoes.Where(t => !t.Deleted && t.Title == oldTitle);
            //if (userTodos != null && userTodos.Any())
            //{
            //    foreach (Todo t in userTodos)
            //    {
            //        t.Deleted = true;

            //    }
            //}
            ctx.SaveChanges();
            AdviceUserAfterEditTodo();
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
        public void AdviceUserAfterEditTodo()
        {
            var users = UserManager.Users.Where(u => !u.Deleted).ToList();
            foreach (var user in users)
            {
                user.ResetTodo= (int)MarkUserTodoReset.TodosNotReset;
            }
            ctx.SaveChanges();
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

        private TodoTemplateViewModels.TodoTemplateBulkViewModel GetBulkViewModel(int id, bool getReferenced = true)
        {
            var todoTemplates = GetTodoTaxonomicViewList();
            var selectedTemplate = todoTemplates.FirstOrDefault(t => t.TemplateId == id);

            if (selectedTemplate == null)
            {
                return null;
            }

            var groupedTemplate = todoTemplates.Where(t =>
            t.cycle == selectedTemplate.cycle &&
            t.Title == selectedTemplate.Title &&
            t.DateEnd.Month == selectedTemplate.DateEnd.Month &&
            t.DateEnd.Day == selectedTemplate.DateEnd.Day &&
            t.DateStart.Month == selectedTemplate.DateStart.Month &&
            t.DateStart.Day == selectedTemplate.DateStart.Day &&
            t.Description == selectedTemplate.Description
            ).ToList();


            var mod = DbGetTodoTemplate(id);
            TodoTemplateViewModels.TodoTemplateBulkViewModel vm = new TodoTemplateViewModels.TodoTemplateBulkViewModel()
            {
                Cycle = mod.Cycle,
                Id = mod.Id,
                DateStart = mod.DateStart,
                DateEnd = mod.DateEnd,
                ReferenceType = mod.ReferenceType,
                Title = mod.Title,
                ReferenceId = new int[] { mod.ReferenceId },
                Description = mod.Description,
                InfoObjects = getReferenced ? DbGetReferencedItems() : null,
                BulkTodoTemplates = groupedTemplate
            };
            return vm;
        }


        public void DbCreateTodoTemplate(TodoTemplateCreateViewModel vm, string articlesRef = "")
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
                    ctx.TodoTemplate.Add(newTemplate);
                    if (!String.IsNullOrEmpty(articlesRef))
                    {
                        foreach (string idStr in articlesRef.Split(','))
                        {
                            int articleId = 0;
                            if (Int32.TryParse(idStr, out articleId) && articleId > 0)
                            {
                                AddArticleReference(articleId, newTemplate.Id);
                            }
                        }
                    }
                }
            }
            if (vm.ReferenceType == ReferenceToModelClass.TaxonomicTree)
            {
                TaxonomicTreeController ttc = new TaxonomicTreeController();
                var plants = ttc.DbGetAllPlantsUnderTree(vm.TaxonomicTreeId);
                var rootTemplate = new TodoTemplate()
                {
                    Cycle = vm.Cycle,
                    DateStart = vm.DateStart,
                    DateEnd = vm.DateEnd,
                    Description = vm.Description,
                    Precision = TodoPrecisionType.Date,
                    ReferenceId = -1,
                    ReferenceType = ReferenceToModelClass.TaxonomicTree,
                    Title = vm.Title,
                    TaxonomicTreeId = vm.TaxonomicTreeId
                };
                rootTemplate.OnCreate(Utilities.GetUserName());
                ctx.TodoTemplate.Add(rootTemplate);
                ctx.SaveChanges();


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
                        TaxonomicTreeId = vm.TaxonomicTreeId,
                        TaxonomicReferenceTemplateId = rootTemplate.Id
                    };
                    newTemplate.OnCreate(Utilities.GetUserName());
                    ctx.TodoTemplate.Add(newTemplate);
                }
            }
            ctx.SaveChanges();            
        }

        public ActionResult AddArticleReference(int articleId, int todotemplateId)
        {
            ArticleReference newRef = new ArticleReference
            {
                ArticleId = articleId,
                ReferenceType = ArticleReferenceType.Todotemplate,
                TodoTemplateId = todotemplateId
            };

            newRef.OnCreate("SYSTEM");

            ctx.ArticleReference.Add(newRef);
            ctx.SaveChanges();

            return RedirectToAction("edit", new { id = todotemplateId });
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
            //var vm = GetTodoTemplateIndexViewModel();

            TodoTemplateViewModels.TodoTemplateIndexViewModel output = new TodoTemplateIndexViewModel
            {
                TodoTemplates = null
            };
            output.TodoTemplates = ctx.TodoTemplate.Where(v => v.ReferenceId == id && v.ReferenceType == ModelEnums.ReferenceToModelClass.Plant && !v.Deleted).Select(t => new TodoTemplateViewModels.TodoTemplateDetailsViewModel
            {
                Cycle = t.Cycle,
                DateStart = t.DateStart,
                DateEnd = t.DateEnd,
                Description = t.Description,
                Title = t.Title,
                Id = t.Id,
            });
            return output;
        }

        public List<TaxonomicRelationTodoItemModel> GetTodoTaxonomicViewList()
        {
            return ctx.Database.SqlQuery<TaxonomicRelationTodoItemModel>("select * from dbo.TaxonomieRelationTodo").ToList();
        }

        public TodoTemplateViewModels.TodoTemplateIndexViewModel GetTodoTemplateIndexViewModel()
        {
            List<TodoTemplateViewModels.TodoTemplateDetailsViewModel> list = new List<TodoTemplateViewModels.TodoTemplateDetailsViewModel>();
            var todoTemplates = GetTodoTaxonomicViewList();
            //var grouped = todoTemplates.OrderBy(t => t.Title).GroupBy(t => new { t.Title, t.DateStart, t.DateEnd, t.Description, t.cycle }).ToList();
            var grouped = todoTemplates.OrderBy(t => t.Title).GroupBy(t => new { t.Title, t.DateStartMonthString, t.DateEndMonthString, t.Description, t.cycle }).ToList();

            var templateViewModels = grouped.Select(t => new TodoTemplateViewModels.TodoTemplateDetailsViewModel
            {
                Cycle = t.First().cycle,
                DateStart = t.First().DateStart,
                DateEnd = t.First().DateEnd,
                Description = t.First().Description,
                ReferenceName = string.Join(", ", t.Select(d => d.nameGerman)),
                Title = t.First().Title,
                Id = t.First().TemplateId,
            });
            return new TodoTemplateViewModels.TodoTemplateIndexViewModel { TodoTemplates = templateViewModels };
        }

        // Original logic taken from GetTodoTemplateDetailViewModel(). This is basically the same, just works much faster on larger queries
        public IEnumerable<TodoTemplateViewModels.TodoTemplateDetailsViewModel> GetTodoTemplateDetailViewModels(IEnumerable<int> ids)
        {
            var todoTemplates = (from t in ctx.TodoTemplate where !t.Deleted && ids.Contains(t.Id) select t);
            var plantReferenceIds = todoTemplates.Where(t => t.ReferenceType == ReferenceToModelClass.Plant).Select(t => t.ReferenceId);
            var plantReferenceIdToName = ctx.Plants.Where(p => !p.Deleted && plantReferenceIds.Contains(p.Id)).ToDictionary(p => p.Id, p => p.Name);
            var templateViewModels = todoTemplates.Select(t => new TodoTemplateViewModels.TodoTemplateDetailsViewModel
            {
                Cycle = t.Cycle,
                DateStart = t.DateStart,
                DateEnd = t.DateEnd,
                Description = t.Description,
                ReferenceId = t.ReferenceId,
                ReferenceType = t.ReferenceType,
                Title = t.Title,
                Id = t.Id,
            });
            
            foreach(var t in templateViewModels.Where(t => t.ReferenceType == ReferenceToModelClass.Plant))
            {
                string referenceName;
                plantReferenceIdToName.TryGetValue(t.ReferenceId, out referenceName);
                if (referenceName != null)
                    t.ReferenceName = referenceName;
            }

            return templateViewModels;
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
                newModel.ReferenceName = ctx.Plants.Where(p => !p.Deleted && p.Id == newModel.ReferenceId).FirstOrDefault().Name;
            }
            return newModel;
        }

        public IEnumerable<TodoTemplate> DbGetTodoTemplates()
        {
            var templates = (from t in ctx.TodoTemplate
                             where !t.Deleted
                             select t);
            return templates;
        }

        public IEnumerable<TodoTemplate> DbGetTodoTemplateWithTaxonId(int taxonId)
        {
            var templates = (from t in ctx.TodoTemplate
                             where !t.Deleted && t.ReferenceType == ReferenceToModelClass.TaxonomicTree && t.TaxonomicTreeId == taxonId
                             select t);
            return templates;
        }


        public void DbFixAllTaxom()
        {
            //var templates = (from t in ctx.TodoTemplate
            //                 where !t.Deleted
            //                 select t);
            var data = ctx.Database.SqlQuery<TaxonomicRelationTodoItemModel>("select * from dbo.TaxonomieRelationTodo").ToList();
            //var groupedTemplate = templates.GroupBy(t => new { t.Title, t.DateStart, t.DateEnd, t.Description, t.Cycle }).ToList();

            //var groupCount = groupedTemplate.Count();
            var grouped = data.GroupBy(t => new { t.Title, t.DateStart, t.DateEnd, t.Description, t.cycle}).ToList();
            var taxomList = data.OrderBy(t => t.Title);

            return;
        }

        public IEnumerable<TodoTemplate> DbGetTodoTemplates(int referenceId, ReferenceToModelClass type)
        {
            var templates = (from t in ctx.TodoTemplate
                             where !t.Deleted
                             && t.ReferenceId == referenceId && t.ReferenceType == type
                             select t);
            return templates;
        }

        public TodoTemplate DbGetTodoTemplate(int id)
        {
            var template = (from t in ctx.TodoTemplate
                            where !t.Deleted
                            && t.Id == id
                            select t).FirstOrDefault();
            return template;
        }
        #endregion
    }
}