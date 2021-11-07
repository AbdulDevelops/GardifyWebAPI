using PflanzenApp.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using static GardifyModels.Models.TodoViewModels;
using static GardifyModels.Models.ModelEnums;
using System.Threading.Tasks;
using Microsoft.Ajax.Utilities;

namespace PflanzenApp.Controllers
{
    public class TodoController : _BaseController
    {
        public const int DAY_LIMIT = 365;
        public const int TAKE_AMOUNT = 10;
        #region HTTP
        [Authorize]
        public ActionResult Index(string period)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddMonths(12);
            if (!string.IsNullOrEmpty(period))
            {
                switch (period)
                {
                    case "week":
                        endDate = startDate.AddDays(7);
                        break;
                    case "month":
                        endDate = startDate.AddMonths(1);
                        break;
                    default:
                        ViewBag.AlertMessage = "Zeitperiode konnte nicht erkannt werden.";
                        break;
                }
            }
            Guid userId = Utilities.GetUserId();
            var viewModel = GetTodoIndexViewModel(userId, startDate, endDate);
            viewModel.TodoList = viewModel.TodoList.Take(TAKE_AMOUNT);
            return View(viewModel);
        }
        [Authorize]
        public ActionResult IndexFinished(string period)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddMonths(12);
            if (!string.IsNullOrEmpty(period))
            {
                switch (period)
                {
                    case "week":
                        endDate = startDate.AddDays(7);
                        break;
                    case "month":
                        endDate = startDate.AddMonths(1);
                        break;
                    default:
                        ViewBag.AlertMessage = "Zeitperiode konnte nicht erkannt werden.";
                        break;
                }
            }
            Guid userId = Utilities.GetUserId();
            var viewModel = GetTodoIndexViewModel(userId, startDate, endDate, true);
            viewModel.TodoList = viewModel.TodoList.Where(v => v.Finished == true);
            viewModel.TodoList = viewModel.TodoList.Take(TAKE_AMOUNT);
            return View(viewModel);
        }
        [Authorize]
        public ActionResult Details(int id)
        {
            var vm = GetTodoDetailsViewModel(id);
            ArticleController ac = new ArticleController();
            //vm.Articles = ac.GetObjectArticles(ModelEnums.ArticleReferenceType.Todotemplate, vm.)
            if (vm == null)
            {
                //TODO: Redirect to error
            }
            return View(vm);
        }
        [Authorize]
        public ActionResult Create()
        {
            TodoCreateViewModel vm = new TodoCreateViewModel();
            vm.InfoObjects = DbGetReferenceObjects(Utilities.GetUserId());
            vm.DateStart = DateTime.Now;
            vm.DateEnd = DateTime.Now;
            return View(vm);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TodoCreateViewModel vm)
        {
            PropertyController proc = new PropertyController();
            if (ModelState.IsValid)
            {
                DbAddTodo(vm);
                return RedirectToAction("Index");
            }
            else
            {
                vm.InfoObjects = DbGetReferenceObjects(Utilities.GetUserId());
                ModelState.AddModelError(string.Empty, "Bitte geben Sie korrekte Werte ein.");
                return View("Create", vm);
                //return RedirectToError("Das übergebene Model ist nicht gülitg.", HttpStatusCode.InternalServerError, "TodoController.Create(" + createView.Title + ")");
            }
        }
        public ActionResult MarkIgnored(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userid = Utilities.GetUserId();

                DbSetTodoIgnored(id);

                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Register", "Account");
            }
        }
        public ActionResult MarkFinished(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userid = Utilities.GetUserId();

                DbSetTodoFinished(id);

                return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
            }
            else
            {
                return RedirectToAction("Register", "Account");
            }
        }
        public ActionResult MarkDeleted(int id)
        {
            if (User.Identity.IsAuthenticated)
            {
                var userid = Utilities.GetUserId();

                DbSetTodoDeleted(id);

                return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);
            }
            else
            {
                return RedirectToAction("Register", "Account");
            }
        }
        [HttpPost]
        public ActionResult ajaxUpdateTodoDate(int id, DateTime date, bool dateStart)
        {
            bool success = DbUpdateTodo(id, date, dateStart);
            if (success)
            {
                return new HttpStatusCodeResult(200);
            }
            else
            {
                return new HttpStatusCodeResult(500);
            }
        }
        #endregion

        #region VM

        [NonAction]
        public TodoIndexViewModel GetTodoIndexViewModel(Guid userId, bool includeFinished = false)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(DAY_LIMIT);
            return GetTodoIndexViewModel(userId, startDate, endDate, includeFinished);
        }
        [NonAction]
        public TodoIndexViewModel GetTodoIndexViewModel(Guid userId, DateTime startDate, DateTime endDate, bool includeFinished = false)
        {
            var vm = GetTodoIndexViewModel(startDate, endDate, includeFinished);
            vm.TodoList = vm.TodoList.Where(v => v.UserId == userId);
            return vm;
        }
        [NonAction]
        public TodoIndexViewModel GetTodoIndexViewModel(int referenceId, ModelEnums.ReferenceToModelClass referenceType, bool includeFinished = false)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(DAY_LIMIT);
            return GetTodoIndexViewModel(referenceId, referenceType, startDate, endDate, includeFinished);
        }
        [NonAction]
        public TodoIndexViewModel GetTodoIndexViewModel(int referenceId, ModelEnums.ReferenceToModelClass referenceType, DateTime startDate, DateTime endDate, bool includeFinished = false)
        {
            var vm = GetTodoIndexViewModel(startDate, endDate, includeFinished);
            vm.TodoList = vm.TodoList.Where(v => v.ReferenceId == referenceId
            && v.ReferenceType == referenceType);
            return vm;
        }
        [NonAction]
        public TodoIndexViewModel GetTodoIndexViewModel(int gardenId, bool includeFinished = false, int take = int.MaxValue)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(DAY_LIMIT);
            var vm = GetTodoIndexViewModel(gardenId, startDate, endDate, includeFinished);
            return new TodoIndexViewModel()
            {
                EndDate = vm.EndDate,
                StartDate = vm.StartDate,
                TodoList = vm.TodoList.Take(take)
            };
        }
        [NonAction]
        public TodoIndexViewModel GetTodoIndexViewModel(int gardenId, DateTime startDate, DateTime endDate, bool includeFinished = false)
        {
            var todos = DbGetTodosByGardenId(startDate, endDate, gardenId, includeFinished);
            TodoIndexViewModel vm = new TodoIndexViewModel()
            {
                EndDate = endDate,
                StartDate = startDate
            };
            List<TodoDetailsViewModel> todoList = new List<TodoDetailsViewModel>();
            foreach (var todo in todos)
            {
                todoList.Add(GetTodoDetailsViewModel(todo.Id));
            }
            vm.TodoList = todoList;
            return vm;
        }
        [NonAction]
        public TodoIndexViewModel GetTodoIndexViewModel(bool includeFinished = false)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(DAY_LIMIT);
            return GetTodoIndexViewModel(startDate, endDate, includeFinished);
        }
        [NonAction]
        public TodoIndexViewModel GetTodoIndexViewModel(DateTime startDate, DateTime endDate, bool includeFinished = false)
        {
            VideoReferenceController vrc = new VideoReferenceController();
            var todos = DbGetTodos(startDate, endDate, includeFinished);
            TodoIndexViewModel vm = new TodoIndexViewModel()
            {
                StartDate = startDate,
                EndDate = endDate
            };
            var todoIndexList = (from td in todos
                                 where !td.Deleted
                                 && (td.DateStart >= startDate || td.DateEnd >= startDate)
                                 && (td.DateStart <= endDate || td.DateEnd <= endDate)
                                 select td.Id);
            List<TodoDetailsViewModel> vmList = new List<TodoDetailsViewModel>();
            foreach (var todoIndex in todoIndexList)
            {
                vmList.Add(GetTodoDetailsViewModel(todoIndex));
            }
            vm.TodoList = vmList;
            foreach (var todo in vm.TodoList)
            {
                todo.ReferenceText = DbGetReferenceText(todo.ReferenceType, todo.ReferenceId);
                todo.VideoReferenceList = vrc.GetVideoDetailsViewModelList(todo.ReferenceId, todo.ReferenceType);
            }
            return vm;
        }
        [NonAction]
        public TodoDetailsViewModel GetTodoDetailsViewModel(int id)
        {

            Todo userTodo = DbGetTodo(id);
            TodoDetailsViewModel vm = new TodoDetailsViewModel
            {
                Id = userTodo.Id,
                CyclicId = userTodo.CyclicId,
                DateStart = userTodo.DateStart,
                DateEnd = userTodo.DateEnd,
                Deleted = userTodo.Ignored,
                Description = userTodo.Description,
                Finished = userTodo.Finished,
                UserId = userTodo.UserId,
                ReferenceId = userTodo.ReferenceId,
                ReferenceType = userTodo.ReferenceType,
                Title = userTodo.Title
            };
            VideoReferenceController vrc = new VideoReferenceController();
            vm.ReferenceText = DbGetReferenceText(vm.ReferenceType, vm.ReferenceId);
            vm.VideoReferenceList = vrc.GetVideoDetailsViewModelList(vm.Id, ModelEnums.ReferenceToModelClass.Todo);

            return vm;
        }
        #endregion

        #region DB
        [NonAction]
        public void DbAddTodo(TodoCreateViewModel vm)
        {
            if (vm.Cycle == ModelEnums.TodoCycleType.Once)
            {
                //Create single todo
                Todo td = new Todo()
                {
                    Title = vm.Title,
                    Description = vm.Description,
                    DateStart = vm.DateStart,
                    DateEnd = vm.DateEnd,
                    Precision = ModelEnums.TodoPrecisionType.Date,
                    ReferenceType = vm.ReferenceType,
                    ReferenceId = vm.ReferenceId,
                    CyclicId = null,
                    Finished = false,
                    Ignored = false,
                    Index = null,
                    UserId = Utilities.GetUserId()
                };
                td.OnCreate(Utilities.GetUserName());
                ctx.Todoes.Add(td);
            }
            else
            {
                //Create cyclic todo
                TodoCyclic td = new TodoCyclic()
                {
                    Cycle = vm.Cycle,
                    Title = vm.Title,
                    Description = vm.Description,
                    DateStart = vm.DateStart,
                    DateEnd = vm.DateEnd,
                    Precision = ModelEnums.TodoPrecisionType.Date,
                    ReferenceType = vm.ReferenceType,
                    ReferenceId = vm.ReferenceId,
                    UserId = Utilities.GetUserId()
                };
                td.OnCreate(Utilities.GetUserName());
                ctx.TodoCyclic.Add(td);
            }
            ctx.SaveChanges();
        }

        [NonAction]
        public int DbAddTodoWithId(TodoCreateViewModel vm, Guid userId, bool isUserTodo = true, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            var newId = -1;
            if (vm.Cycle == ModelEnums.TodoCycleType.Once)
            {
                //Create single todo
                Todo td = new Todo()
                {
                    Title = vm.Title,
                    Description = vm.Description,
                    DateStart = vm.DateStart,
                    DateEnd = vm.DateStart,
                    Precision = ModelEnums.TodoPrecisionType.Date,
                    ReferenceType = vm.ReferenceType,
                    ReferenceId = vm.ReferenceId,
                    CyclicId = null,
                    Finished = false,
                    Ignored = false,
                    Index = null,
                    UserId = userId,
                    RelatedTodoTemplateId = vm.RelatedTodoTemplateId

                };
                td.OnCreate(Utilities.GetUserName());
                var added = ctx.Todoes.Add(td);
                ctx.SaveChanges();
                newId = added.Id;
       


            }
            else
            {
                //Create cyclic todo
                TodoCyclic td = new TodoCyclic()
                {
                    Cycle = vm.Cycle,
                    Title = vm.Title,
                    Description = vm.Description,
                    DateStart = vm.DateStart,
                    DateEnd = vm.DateEnd,
                    Precision = ModelEnums.TodoPrecisionType.Date,
                    ReferenceType = vm.ReferenceType,
                    ReferenceId = vm.ReferenceId,
                    UserId = userId,
                    GeneratedFromTemplate = vm.GeneratedFromTemplate,
                    RelatedTodoTemplateId = vm.RelatedTodoTemplateId
                };
                td.OnCreate(Utilities.GetUserName());
                var added = ctx.TodoCyclic.Add(td);
                ctx.SaveChanges();
                newId = added.Id;


            }

            return newId;
        }
        [NonAction]
        public Todo DbGetTodo(int id)
        {
            var todo = (from td in ctx.Todoes
                        where !td.Deleted
                        && td.Id == id
                        select td).FirstOrDefault();
            return todo;
        }
        [NonAction]
        public IEnumerable<Todo> DbGetTodos(DateTime startDate, DateTime endDate, bool includeFinished = false)
        {
            #region Populate Cyclic Todos
            foreach (var cyclic in ctx.TodoCyclic)
            {

                var possiblyEdited = (from td in ctx.Todoes
                                      where td.CyclicId == cyclic.Id
                                      && td.Deleted == false
                                      select td).ToList();
                int index = 0;
                DateTime rStart = cyclic.DateStart;
                TimeSpan difference = cyclic.DateEnd - cyclic.DateStart;
                List<Todo> todoList = new List<Todo>();
                while (rStart <= endDate)
                {
                    var hasAnomaly = !possiblyEdited.Any(v => v.Index == index);
                    if (rStart >= startDate && rStart <= endDate && hasAnomaly)
                    {
                        Todo newTodo = new Todo()
                        {
                            DateStart = rStart,
                            DateEnd = rStart + difference,
                            Finished = false,
                            Ignored = false,
                            Index = index,
                            Precision = cyclic.Precision,
                            ReferenceId = cyclic.ReferenceId,
                            ReferenceType = cyclic.ReferenceType,
                            Title = cyclic.Title,
                            UserId = cyclic.UserId,
                            Description = cyclic.Description,
                            CyclicId = cyclic.Id
                        };
                        newTodo.OnCreate(Utilities.GetUserName());
                        todoList.Add(newTodo);
                    }
                    switch (cyclic.Cycle)
                    {
                        case ModelEnums.TodoCycleType.Daily:
                            rStart = rStart.AddDays(1);
                            break;
                        case ModelEnums.TodoCycleType.Weekly:
                            rStart = rStart.AddDays(7);
                            break;
                        case ModelEnums.TodoCycleType.BiYearly:
                            rStart = rStart.AddYears(2);
                            break;
                        case ModelEnums.TodoCycleType.Monthly:
                            rStart = rStart.AddMonths(1);
                            break;
                        case ModelEnums.TodoCycleType.Yearly:
                            rStart = rStart.AddYears(1);
                            break;
                        default:
                            throw new Exception("DBHandler.getTodos unknown cycle");
                    }
                    index++;
                }
                ctx.Todoes.AddRange(todoList);
            }
            ctx.SaveChanges();
            #endregion
            var todos = (from td in ctx.Todoes
                         where !td.Deleted
                         && (td.DateStart >= startDate || td.DateEnd >= startDate)
                         && (td.DateEnd <= endDate || td.DateEnd <= endDate)
                         && !td.Ignored
                         && ((includeFinished == true) ? true : !td.Finished)
                         select td);
            return todos;
        }
        [NonAction]
        public IEnumerable<Todo> DbGetTodosByUserId(DateTime startDate, DateTime endDate, Guid userId, bool includeFinished = false)
        {
            var todos = DbGetTodos(startDate, endDate, includeFinished).Where(v => v.UserId == userId);
            return todos;
        }
        [NonAction]
        public IEnumerable<Todo> DbGetTodosByGardenId(DateTime startDate, DateTime endDate, int gardenId, bool includeFinished = false)
        {
            UserPlantController gc = new UserPlantController();
            var gardenPlants = gc.DbGetUserPlantsByGardenId(gardenId);

            List<Todo> todoList = new List<Todo>();

            todoList.AddRange(DbGetTodosByReference(startDate, endDate, ModelEnums.ReferenceToModelClass.Garden, gardenId));
            foreach (var plant in gardenPlants)
            {
                todoList.AddRange(DbGetTodosByReference(startDate, endDate, ModelEnums.ReferenceToModelClass.UserPlant, plant.Id));
            }
            return todoList;
        }
        [NonAction]
        public IEnumerable<Todo> DbGetTodosByReference(DateTime startDate, DateTime endDate, ModelEnums.ReferenceToModelClass referenceType, int referenceId, bool includeFinished = false)
        {
            var todos = DbGetTodos(startDate, endDate, includeFinished).Where(v => v.ReferenceType == referenceType
            && v.ReferenceId == referenceId);
            return todos;
        }
        [NonAction]
        public bool DbUpdateTodo(int id, DateTime date, bool dateStart)
        {
            var todo = DbGetTodo(id);
            if (dateStart)
            {
                todo.DateStart = date;
            }
            else
            {
                todo.DateEnd = date;
            }
            return ctx.SaveChanges() > 0;
        }
        [NonAction]
        public void DbAddTemplateTodos(ModelEnums.ReferenceToModelClass tempRefType,
            int tempRefId,
            ModelEnums.ReferenceToModelClass newReferenceType,
            int newReferenceId,
            IEnumerable<_TodoCheckedTemplateViewModel> todos)
        {
            var templateIds = (from td in todos
                               where td.Checked
                               select td.TemplateId);

            var todoTemplatesPre = (from td in ctx.TodoTemplate
                                    where !td.Deleted
                                    && td.ReferenceId == tempRefId
                                    && td.ReferenceType == tempRefType
                                    select td).ToList();
            var todoTemplates = todoTemplatesPre.Where(v => templateIds.Contains(v.Id));
            foreach (var template in todoTemplates)
            {
                TodoCreateViewModel vm = new TodoCreateViewModel()
                {
                    Cycle = template.Cycle,
                    DateStart = template.DateStart,
                    DateEnd = template.DateEnd,
                    Description = template.Description,
                    ReferenceId = newReferenceId,
                    ReferenceType = newReferenceType,
                    Title = template.Title
                };
                DbAddTodo(vm);
            }
        }

        [NonAction]
        public void DbAddTemplateTodosComplete(ModelEnums.ReferenceToModelClass tempRefType,
            int tempRefId,
            ModelEnums.ReferenceToModelClass newReferenceType,
            int newReferenceId,
            IEnumerable<_TodoCheckedTemplateViewModel> todos, Guid userId)
        {
            var templateIds = (from td in todos
                               where td.Checked
                               select td.TemplateId);

            var todoTemplatesPre = (from td in ctx.TodoTemplate
                                    where !td.Deleted
                                    && td.ReferenceId == tempRefId
                                    && td.ReferenceType == tempRefType
                                    select td).ToList();
            var todoTemplates = todoTemplatesPre.Where(v => templateIds.Contains(v.Id)).ToList();

            var existingCyclicTodos = ctx.TodoCyclic.Where(t => t.UserId == userId && (t.ReferenceType == ReferenceToModelClass.Plant || t.ReferenceType == ReferenceToModelClass.UserPlant) && !t.Deleted).ToList();
            todoTemplates = todoTemplates.Where(t => !existingCyclicTodos.Any(e => e.Description == t.Description && e.Title == t.Title && e.DateStart == t.DateStart && e.DateEnd == t.DateEnd && e.ReferenceId == t.ReferenceId)).ToList();
            foreach (var template in todoTemplates)
            {
                // set todo dates starting now
                var startAt = new DateTime(DateTime.Now.Year, template.DateStart.Month, template.DateStart.Day);
                var endAt = new DateTime(DateTime.Now.Year, template.DateEnd.Month, template.DateEnd.Day);

                // make sure end date is in the future
                if (template.DateEnd.DayOfYear < template.DateStart.DayOfYear)
                {
                    endAt = endAt.AddYears(1);
                }

                TodoCreateViewModel vm = new TodoCreateViewModel()
                {
                    Cycle = template.Cycle,
                    DateStart = startAt,
                    DateEnd = endAt,
                    Description = template.Description,
                    ReferenceId = newReferenceId,
                    ReferenceType = newReferenceType,
                    Title = template.Title,
                    GeneratedFromTemplate = true,
                    RelatedTodoTemplateId = template.Id

                };
                DbAddTodoWithId(vm, userId);
            }
            var endDate = new DateTime(DateTime.Now.Year + 1, 1, 1);
            PopulateCyclicTodos(userId, DateTime.Now, endDate);
        }

        public void RemoveOrphanTodos(Guid userId)
        {
            var cyclics = (from t in ctx.TodoCyclic
                           join up in ctx.UserPlants on t.ReferenceId equals up.Id
                           where t.ReferenceType == ReferenceToModelClass.UserPlant && !t.Deleted && up.Deleted && t.UserId == userId
                           select t);
            var todos = (from t in ctx.Todoes
                         join tc in ctx.TodoCyclic on t.CyclicId equals tc.Id
                         where (t.UserId == userId && tc.Deleted && !t.Deleted) || (t.UserId == userId && t.DateStart > tc.DateEnd)
                         select t);

            foreach (TodoCyclic t in cyclics.ToList())
            {
                t.Deleted = true;
            }
            foreach (Todo t in todos.ToList())
            {
                t.Deleted = true;
            }
            ctx.SaveChanges();
        }

        public async Task<bool> PopulateCyclicTodos(Guid userId, DateTime startDate, DateTime endDate, bool yearLong = false, int certainCyclicId = 0, bool onlyTemplate = false)
        {
            if (userId == Guid.Empty)
            {
                return true;
            }
            Task.Run(() => RemoveOrphanTodos(userId));
            var cyclics = ctx.TodoCyclic.Where(c => c.UserId == userId
                                                && !c.Deleted
                                                && !c.Ignored
                                                && ((!(c.DateEnd < startDate) && !(c.DateStart > endDate)) || c.GeneratedFromTemplate)
                                                && ((onlyTemplate && c.GeneratedFromTemplate) || !onlyTemplate)

                                                )

                .DistinctBy(t => t.Title).ToList();
            endDate = endDate.Subtract(TimeSpan.FromMinutes(1));
            List<Todo> todoList = new List<Todo>();
            foreach (var cyclic in cyclics)
            {
                if (certainCyclicId != 0 && certainCyclicId != cyclic.Id)
                {
                    continue;
                }
                DateTime rStart = cyclic.DateStart;
                DateTime cyclicStart = new DateTime(cyclic.CreatedDate.Year, cyclic.DateStart.Month, cyclic.DateStart.Day);
                DateTime cyclicEnd = DateTime.Now;
                if (cyclic.DateEnd.Month == 2 && cyclic.DateEnd.Day > 28)
                {
                    if (DateTime.DaysInMonth(endDate.Year, cyclic.DateEnd.Month) <= cyclic.DateEnd.Day)
                    {
                        cyclicEnd = new DateTime(endDate.Year, cyclic.DateEnd.Month, DateTime.DaysInMonth(endDate.Year, cyclic.DateEnd.Month));
                    }

                }
                else
                {
                    cyclicEnd = new DateTime(endDate.Year, cyclic.DateEnd.Month, cyclic.DateEnd.Day);

                }

                cyclicEnd = cyclicEnd.AddYears(Math.Abs(cyclic.DateStart.Year - cyclic.DateEnd.Year)); // incase cycle is in between years (ex. Nov -> Jan)

                // created by user, has no DateEnd
                if (cyclic.GeneratedFromTemplate)
                {
                    // cyclic is outside the timeframe, dont populate
                    if ((cyclic.DateStart.DayOfYear < startDate.DayOfYear) && (cyclic.DateEnd.DayOfYear < startDate.DayOfYear) && (cyclic.DateEnd.DayOfYear > cyclic.DateStart.DayOfYear) ||
                        (cyclic.DateStart.DayOfYear > endDate.DayOfYear) && (cyclic.DateEnd.DayOfYear > endDate.DayOfYear) && (cyclic.DateEnd.DayOfYear > cyclic.DateStart.DayOfYear))
                    {
                        continue;
                    }
                    // cyclic starts before timeframe
                    if (cyclic.DateStart < startDate && cyclic.DateEnd.DayOfYear > startDate.DayOfYear)
                    {
                        // fast forward until we're in timeframe
                        while (rStart < startDate)
                        {
                            rStart = AddCycle(rStart, cyclic.Cycle);
                        }

                        // no longer in timeframe, ignore
                        if (rStart > endDate)
                        {
                            continue;
                        }
                    }
                }

                int index = 0;
                TimeSpan difference = cyclic.DateEnd - cyclic.DateStart;


                //cyclic was edited and cyclic type == once
                if (cyclic.DateStart == cyclic.DateEnd)
                {
                    Todo newTodo = new Todo()
                    {
                        DateStart = cyclic.DateStart,
                        DateEnd = cyclic.DateEnd,
                        Finished = false,
                        Ignored = false,
                        Index = index,
                        Precision = cyclic.Precision,
                        ReferenceId = cyclic.ReferenceId,
                        ReferenceType = cyclic.ReferenceType,
                        Title = cyclic.Title,
                        UserId = cyclic.UserId,
                        Description = cyclic.Description,
                        CyclicId = cyclic.Id,
                        RelatedTodoTemplateId = cyclic.RelatedTodoTemplateId

                    };
                    newTodo.OnCreate(Utilities.GetUserName());
                    todoList.Add(newTodo);
                }
                else
                {
                    while (rStart >= cyclicStart && rStart < cyclicEnd)
                    {
                        DateTime currentStart = new DateTime(startDate.Year, cyclic.DateStart.Month, cyclic.DateStart.Day);
                        var hasDuplicates = ctx.Todoes.Where(t => t.UserId == cyclic.UserId && t.CyclicId == cyclic.Id && t.DateStart == rStart && !t.Deleted).FirstOrDefault();
                        if (hasDuplicates == null && currentStart <= rStart && rStart <= cyclicEnd)
                        {
                            Todo newTodo = new Todo()
                            {
                                DateStart = rStart,
                                //DateEnd = AddCycle(rStart, cyclic.Cycle), (todoCycle != 'todoEnd - todoStart' which is the timeframe a todo needs to be completed in)
                                DateEnd = rStart + difference,
                                Finished = false,
                                Ignored = false,
                                Index = index,
                                Precision = cyclic.Precision,
                                ReferenceId = cyclic.ReferenceId,
                                ReferenceType = cyclic.ReferenceType,
                                Title = cyclic.Title,
                                UserId = cyclic.UserId,
                                Description = cyclic.Description,
                                CyclicId = cyclic.Id,
                                RelatedTodoTemplateId = cyclic.RelatedTodoTemplateId
                            };
                            newTodo.OnCreate(Utilities.GetUserName());
                            todoList.Add(newTodo);
                        }

                        rStart = AddCycle(rStart, cyclic.Cycle);
                        index++;
                    }
                }
            }

            ctx.Todoes.AddRange(todoList);
            await ctx.SaveChangesAsync();

            return true;
        }

        public DateTime AddCycle(DateTime rStart, ModelEnums.TodoCycleType cycle)
        {
            switch (cycle)
            {
                case ModelEnums.TodoCycleType.Daily:
                    return rStart.AddDays(1);
                case ModelEnums.TodoCycleType.Weekly:
                    return rStart.AddDays(7);
                case ModelEnums.TodoCycleType.Monthly:
                    return rStart.AddMonths(1);
                case ModelEnums.TodoCycleType.Yearly:
                    return rStart.AddYears(1);
                case ModelEnums.TodoCycleType.BiYearly:
                    return rStart.AddYears(2);
                case ModelEnums.TodoCycleType.TriYearly:
                    return rStart.AddYears(3);
                case ModelEnums.TodoCycleType.QuadYearly:
                    return rStart.AddYears(4);
                case ModelEnums.TodoCycleType.PentYearly:
                    return rStart.AddYears(5);
                default:
                    throw new Exception("DBHandler.getTodos unknown cycle");
            }
        }

        [NonAction]
        public List<IReferencedObject> DbGetReferenceObjects(Guid userId)
        {
            List<IReferencedObject> items = new List<IReferencedObject>();
            UserPlantController uc = new UserPlantController();
            GardenController gc = new GardenController();
            List<UserPlant> userPlantList = new List<UserPlant>();
            //TODO: Implement UserTool stuff
            var uplants = uc.DbGetUserPlantsByUserId(userId);
            foreach (UserPlant plant in uplants)
            {
                UserPlant userPlant = plant;
                userPlant.Name = userPlant.Name + " (" + userPlant.Count + " in " + gc.getGardenName(userPlant.Gardenid) + ")";
                userPlantList.Add(userPlant);
            }
            var gards = gc.DbGetGardensByUserId(userId);
            items.AddRange(userPlantList);
            items.AddRange(gards);
            return items;
        }
        [NonAction]
        public string DbGetReferenceText(ModelEnums.ReferenceToModelClass referenceType, int referenceId)
        {
            switch (referenceType)
            {
                case ModelEnums.ReferenceToModelClass.Garden:
                    return (from ctx in ctx.Gardens
                            where ctx.Id == referenceId
                            select ctx.Name).FirstOrDefault();
                case ModelEnums.ReferenceToModelClass.UserPlant:
                    return (from ctx in ctx.UserPlants
                            where ctx.Id == referenceId
                            select ctx.Name).FirstOrDefault();
                default:
                    return "-";
            }
        }

        [NonAction]
        public void DbSetTodoDeleted(int id)
        {
            var todo = DbGetTodo(id);
            todo.Deleted = true;
            todo.OnEdit(Utilities.GetUserName());
            ctx.SaveChanges();
        }
        [NonAction]
        public void DbSetTodoIgnored(int id)
        {
            var todo = DbGetTodo(id);
            todo.Ignored = true;
            todo.OnEdit(Utilities.GetUserName());
            ctx.SaveChanges();
        }
        [NonAction]
        public void DbSetTodoFinished(int id)
        {
            var todo = DbGetTodo(id);
            todo.Finished = true;
            todo.OnEdit(Utilities.GetUserName());
            ctx.SaveChanges();
        }
        #endregion
    }
}
//self.kill()