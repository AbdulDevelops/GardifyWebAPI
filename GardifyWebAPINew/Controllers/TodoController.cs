
using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using MoreLinq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GardifyModels.Models;
using static GardifyModels.Models.ModelEnums;
using static GardifyModels.Models.TodoViewModels;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;

namespace GardifyWebAPI.Controllers
{
    public class TodoController : _BaseController
    {
        public const int DAY_LIMIT = 365;
        public const int TAKE_AMOUNT = 10;
        private ApplicationDbContext db = new ApplicationDbContext();
       
        #region HTTP
        [Authorize]
        public async Task<TodoIndexViewModel> Index(string period, DateTime startDate = default(DateTime), int gid = 0)
        {
            if (startDate == default(DateTime))
            {
                startDate = DateTime.Now;
            }
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
                    case "2m":
                        endDate = startDate.AddMonths(2);
                        break;
                    case "yearEnd":
                        endDate = new DateTime(DateTime.Today.Year + 1, 1, 1);
                        endDate = endDate.AddDays(-1);
                        break;
                    default:
                        ViewBag.AlertMessage = "Zeitperiode konnte nicht erkannt werden.";
                        break;
                }
            }
            Guid userId = Utilities.GetUserId();
            var viewModel = await GetTodoIndexViewModel(userId, startDate, endDate, gid: gid);
            //viewModel.TodoList = viewModel.TodoList.Take(TAKE_AMOUNT);
            return viewModel;
        }
        [Authorize]
        public async Task<TodoIndexViewModel> IndexFinished(string period)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = GetEndDate(startDate, period);
            Guid userId = Utilities.GetUserId();
            var viewModel = await GetTodoIndexViewModel(userId, startDate, endDate, true);
            viewModel.TodoList = viewModel.TodoList.Where(v => v.Finished == true);
            viewModel.TodoList = viewModel.TodoList.Take(TAKE_AMOUNT);
            return viewModel;
        }

        public IEnumerable<Todo> GetTodos(Guid userId, ReferenceToModelClass referenceType)
        {
            Guid empty = Guid.Empty;
            return db.Todoes.Where(t => !t.Deleted && t.UserId == userId && t.UserId != empty && t.ReferenceType == referenceType);
        }

        public DateTime GetEndDate(DateTime startDate, string period)
        {
            var endDate = DateTime.Now.AddMonths(12);
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
                    case "2m":
                        endDate = startDate.AddMonths(2);
                        break;
                    default:
                        ViewBag.AlertMessage = "Zeitperiode konnte nicht erkannt werden.";
                        break;
                }
            }
            return endDate;
        }

        [Authorize]
        public TodoDetailsViewModel Details(int id)
        {
            var vm = GetTodoDetailsViewModel(id);
            ArticleController ac = new ArticleController();
            //vm.Articles = ac.GetObjectArticles(ModelEnums.ArticleReferenceType.Todotemplate, vm.)
            if (vm == null)
            {
                //TODO: Redirect to error
            }
            return vm;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<int> Create(TodoCreateViewModel vm, Guid userId)
        {
            if (ModelState.IsValid && userId != Guid.Empty)
            {
                var newId = DbAddTodo(vm, userId);

                DateTime startDate = new DateTime(vm.DateStart.Year, vm.DateStart.Month, 1);
                DateTime endDate = new DateTime(vm.DateEnd.Year + 1, 1, 1).AddDays(-1);
                DateTime endMonthDate = new DateTime(vm.DateStart.Year, vm.DateStart.Month + 1, 1).AddDays(-1);

                try
                {
                    await PopulateCyclicTodos(userId, startDate, endDate, certainCyclicId: newId);
                }
                catch
                {
                    Task.Run(() => PopulateCyclicTodos(userId, startDate, endDate, certainCyclicId: newId));
                }

                return newId;
            }
            return -1;
        }
        public void MarkIgnored(int id)
        {
            DbSetTodoIgnored(id);
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
        public async Task<TodoIndexViewModel> GetTodoIndexViewModel(Guid userId, bool includeFinished = false, int gid = 0)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(DAY_LIMIT);
            return await GetTodoIndexViewModel(userId, startDate, endDate, includeFinished, gid);
        }
        [NonAction]
        public async Task<TodoIndexViewModel> GetTodoIndexViewModel(Guid userId, DateTime startDate, DateTime endDate, bool includeFinished = true, int gid = 0)
        {
            var vm = await GetTodoIndexViewModel(startDate, endDate, userId, includeFinished, gid);
            return vm;
        }
        [NonAction]
        public async Task<TodoIndexViewModel> GetTodoIndexViewModel(int referenceId, ModelEnums.ReferenceToModelClass referenceType, Guid userId, bool includeFinished = false)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(DAY_LIMIT);
            return await GetTodoIndexViewModel(referenceId, referenceType, startDate, endDate, userId, includeFinished);
        }
        [NonAction]
        public async Task<TodoIndexViewModel> GetTodoIndexViewModel(int referenceId, ModelEnums.ReferenceToModelClass referenceType, DateTime startDate, DateTime endDate, Guid userId, bool includeFinished = false)
        {
            var vm = await GetTodoIndexViewModel(startDate, endDate, userId, includeFinished);
            vm.TodoList = vm.TodoList.Where(v => v.ReferenceId == referenceId
            && v.ReferenceType == referenceType);
            return vm;
        }
        [NonAction]
        public async Task<TodoIndexViewModel> GetTodoIndexViewModel(int gardenId, bool includeFinished = false, int take = int.MaxValue)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(DAY_LIMIT);
            var vm = await GetTodoIndexViewModel(gardenId, startDate, endDate, includeFinished);
            return new TodoIndexViewModel()
            {
                EndDate = vm.EndDate,
                StartDate = vm.StartDate,
                TodoList = vm.TodoList.Take(take)
            };
        }
        [NonAction]
        public async Task<TodoIndexViewModel> GetTodoIndexViewModel(int gardenId, DateTime startDate, DateTime endDate, bool includeFinished = false)
        {
            var todos = await DbGetTodosByGardenId(startDate, endDate, gardenId, includeFinished);
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
        public async Task<TodoIndexViewModel> GetTodoIndexViewModel(bool includeFinished = false)
        {
            DateTime startDate = DateTime.Now;
            DateTime endDate = DateTime.Now.AddDays(DAY_LIMIT);
            var userId = Utilities.GetUserId();
            return await GetTodoIndexViewModel(startDate, endDate, userId, includeFinished);
        }
        [NonAction]
        public async Task<TodoIndexViewModel> GetTodoIndexViewModel(DateTime startDate, DateTime endDate, Guid userId, bool includeFinished = true, int gid = 0)
        {
            VideoReferenceController vrc = new VideoReferenceController();
            var willPopulate = false;

            if (startDate.Year > DateTime.Now.Year)
            {
                willPopulate = true;
            }

            var todos = await DbGetTodos(startDate, endDate, includeFinished, populateCyclic: willPopulate, gardenId: gid);
            TodoIndexViewModel vm = new TodoIndexViewModel()
            {
                StartDate = startDate,
                EndDate = endDate
            };
            var empty = Guid.Empty;
            var todoIndexList = (from td in todos
                                 where td.UserId == userId && td.UserId != empty
                                 select td.Id);
            List<TodoDetailsViewModel> vmList = new List<TodoDetailsViewModel>();
            foreach (var todoIndex in todoIndexList)
            {
                vmList.Add(GetTodoDetailsViewModel(todoIndex));
            }
            vm.TodoList = vmList;
            //remove duplicate
            foreach (var todo in vm.TodoList)
            {
                ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
                HelperClasses.DbResponse imageResponse =null ;
                if (todo.ReferenceId == 0)
                {
                    imageResponse = todo.CyclicId.HasValue ? rc.DbGetTodoReferencedImages(todo.CyclicId.Value) : rc.DbGetTodoReferencedImages(todo.Id);
                }
                else
                {
                    //var relatedPlant = (from up in db.UserPlants where (up.Id == todo.ReferenceId) select up).FirstOrDefault();
                    //var relatedPlant = (from p in db.Plants where (p.Id == todo.ReferenceId) select p).FirstOrDefault();
                    //imageResponse = rc.DbGetPlantReferencedImages(relatedPlant.Id);
                    //todo.RelatedPlantId = relatedPlant.Id;
                    //todo.RelatedPlantName = relatedPlant.NameGerman;

                    var relatedTemplate = (from tp in db.TodoTemplate where (tp.Id == todo.RelatedTodoTemplateId) select tp).FirstOrDefault();
                    if(relatedTemplate == null)
                    {
                        var relatedPlant = (from up in db.UserPlants where (up.Id == todo.ReferenceId) select up).FirstOrDefault();
                        imageResponse = rc.DbGetPlantReferencedImages(relatedPlant.Id);
                        todo.RelatedPlantId = relatedPlant.Id;
                        todo.RelatedPlantName = relatedPlant.Name;
                    }
                    else
                    {
                        var relatedPlant = (from up in db.Plants where (up.Id == relatedTemplate.ReferenceId) select up).FirstOrDefault();
                        //imageResponse = rc.DbGetPlantReferencedImages(relatedPlant.Id);
                        //vm.RelatedPlantId = relatedPlant.Id;
                        //vm.RelatedPlantName = relatedPlant.Name;
                        //vm.ReferenceText = DbGetReferenceText(vm.ReferenceType, vm.ReferenceId);
                        imageResponse = rc.DbGetPlantReferencedImages(relatedPlant.Id);
                        todo.RelatedPlantId = relatedPlant.Id;
                        todo.RelatedPlantName = relatedPlant.Name;
                    }
                    

                }

                todo.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
            }
            return vm;

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("upload-todo-image")]
        public ActionResult UploadTodoImage(HttpPostedFileBase imageFile, int todoId, string imageTitle = null, string imageDescription = null)
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
                UploadAndRegisterFile(imageFile, todoId, (int)ModelEnums.ReferenceToModelClass.Todo, ModelEnums.FileReferenceType.DiaryEntryImage, imageTitle, imageDescription);
            }

            TempData["statusMessage"] = statusMessage;

            return RedirectToAction("EditEntry", new { id = todoId });
        }

        [NonAction]
        public TodoDetailsViewModel GetTodoDetailsViewModel(int id)
        {

            Todo userTodo = DbGetTodo(id);
            if (userTodo == null)
            {
                return null;
            }

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
                RelatedTodoTemplateId = userTodo.RelatedTodoTemplateId,
                Title = userTodo.Title,
                Notes = userTodo.Notes,
                Ignored = userTodo.Ignored,
                //CyclicDateEnd = plantDB.TodoCyclic.Where(cy => cy.Id == userTodo.CyclicId).Select(re => re.DateEnd).FirstOrDefault()
            };
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            HelperClasses.DbResponse imageResponse = null;
            if (vm.ReferenceId == 0)
            {
                imageResponse = vm.CyclicId.HasValue ? rc.DbGetTodoReferencedImages(vm.CyclicId.Value) : rc.DbGetTodoReferencedImages(vm.Id);
            }
            else
            {
                var relatedTemplate = (from tp in db.TodoTemplate where (tp.Id == vm.RelatedTodoTemplateId) select tp).FirstOrDefault();
                if(relatedTemplate != null)
                {
                    var relatedPlant = (from up in db.Plants where (up.Id == relatedTemplate.ReferenceId) select up).FirstOrDefault();
                    imageResponse = rc.DbGetPlantReferencedImages(relatedPlant.Id);
                    vm.RelatedPlantId = relatedPlant.Id;
                    vm.RelatedPlantName = relatedPlant.Name;
                    vm.ReferenceText = DbGetReferenceText(vm.ReferenceType, vm.ReferenceId);
                }
                
            }

            vm.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));

            VideoReferenceController vrc = new VideoReferenceController();
           
            vm.VideoReferenceList = vrc.GetVideoDetailsViewModelList(vm.Id, ModelEnums.ReferenceToModelClass.Todo);

            return vm;
        }
        #endregion

        #region DB
        [NonAction]
        public int DbAddTodo(TodoCreateViewModel vm, Guid userId, bool isUserTodo=true, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
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
                    RelatedTodoTemplateId=vm.RelatedTodoTemplateId
                   
                };
                td.OnCreate(Utilities.GetUserName());
                var added = db.Todoes.Add(td);
                db.SaveChanges();
                newId = added.Id;
                if (isUserTodo)
                {
                    var currentStatistic = StatisticEventTypes.TodoEntry;
                    new StatisticsController().CreateEntry(StatisticEventTypes.TodoEntry, userId);
                    
                        if (isIos)
                        {
                            new StatisticsController().CreateEntry(currentStatistic, userId, (int)StatisticEventTypes.ApiCallFromIos);
                        }
                        else if (isAndroid)
                        {
                            new StatisticsController().CreateEntry(currentStatistic, userId, (int)StatisticEventTypes.ApiCallFromAndroid);
                        }
                        else if (isWebPage)
                        {
                            new StatisticsController().CreateEntry(currentStatistic, userId, (int)StatisticEventTypes.ApiCallFromWebpage);
                        }
                        else
                        {
                            new StatisticsController().CreateEntry(currentStatistic, userId);
                        }
                }
              

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
                    RelatedTodoTemplateId=vm.RelatedTodoTemplateId
                };
                td.OnCreate(Utilities.GetUserName());
                var added = db.TodoCyclic.Add(td);
                db.SaveChanges();
                newId = added.Id;
                
                if (isUserTodo)
                {
                    var currentStatistic = StatisticEventTypes.CyclicTodoEntry;
                   
                   
                        if (isIos)
                        {
                            new StatisticsController().CreateEntry(currentStatistic, userId, (int)StatisticEventTypes.ApiCallFromIos);
                        }
                        else if (isAndroid)
                        {
                            new StatisticsController().CreateEntry(currentStatistic, userId, (int)StatisticEventTypes.ApiCallFromAndroid);
                        }
                        else if (isWebPage)
                        {
                            new StatisticsController().CreateEntry(currentStatistic, userId, (int)StatisticEventTypes.ApiCallFromWebpage);
                        }
                        else
                        {
                            new StatisticsController().CreateEntry(currentStatistic, userId);
                        }



                }
            }

            return newId;
        }
        [NonAction]
        public Todo DbGetTodo(int id)
        {
            var todo = (from td in db.Todoes
                        where !td.Deleted
                        && td.Id == id
                        select td).FirstOrDefault();
            return todo;
        }
        [NonAction]
        public async Task<IEnumerable<Todo>> DbGetTodos(DateTime startDate, DateTime endDate, bool includeFinished = false, bool populateCyclic = true, int gardenId = 0)
        {
            //RemoveDuplicateTodoCyclicEntries();
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return new List<Todo>();
            }
            #region Populate Cyclic Todos
            if (populateCyclic)
            {
                await PopulateCyclicTodos(userId, startDate, endDate);
                //PopulateCyclicTodosSync(userId, startDate, endDate);
                //Task.Run(() => PopulateCyclicTodos(userId, startDate, endDate));

                //try
                //{
                //    PopulateCyclicTodos(userId, startDate, endDate);
                //}
                //catch
                //{
                //    Task.Run(() => PopulateCyclicTodos(userId, startDate, endDate));
                //}
            }

            #endregion

            IQueryable<Todo> todos;
            if (gardenId > 0)
            {
                todos = (from up in db.UserPlantToUserLists
                         join td in db.Todoes on up.PlantId equals td.ReferenceId
                         where !up.Deleted && !td.Deleted && up.UserListId == gardenId
                         && (td.DateStart >= startDate && td.DateStart <= endDate)
                         && (td.DateEnd >= startDate || td.DateEnd <= endDate)
                         && !td.Ignored
                         && ((includeFinished == true) ? true : !td.Finished) && td.UserId==userId
                         select td);
            } else
            {
                todos = (from td in db.Todoes
                         where !td.Deleted
                         && (td.DateStart >= startDate && td.DateStart <= endDate)
                         && (td.DateEnd >= startDate || td.DateEnd <= endDate)
                         && !td.Ignored
                         && ((includeFinished == true) ? true : !td.Finished)&& td.UserId==userId
                         select td);
            }
            
            return todos.DistinctBy(t => new { t.ReferenceId, t.DateStart, t.Title });
        }

        public Todo DbGetTodosWithCyclic(DateTime startDate, DateTime endDate, int gardenId = 0, int cyclicId = 0, bool willToggle = false)
        {
            //RemoveDuplicateTodoCyclicEntries();
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return null;
            }
      

            IQueryable<Todo> todos;
            if (gardenId > 0)
            {
                todos = (from up in db.UserPlantToUserLists
                         join td in db.Todoes on up.PlantId equals td.ReferenceId
                         where !up.Deleted && !td.Deleted && up.UserListId == gardenId
                         && (td.DateStart >= startDate && td.DateStart <= endDate)
                         && (td.DateEnd >= startDate || td.DateEnd <= endDate)
                         && !td.Ignored
                         && td.CyclicId == cyclicId && td.UserId == userId
                         orderby td.DateStart
                         select td);
            }
            else
            {
                todos = (from td in db.Todoes
                         where !td.Deleted
                         //&& ((td.DateStart >= startDate && td.DateStart <= endDate) || (td.DateEnd >= startDate && td.DateEnd <= endDate))
                         //&& (td.DateEnd >= startDate || td.DateEnd <= endDate)
                         && !td.Ignored
                         && td.CyclicId == cyclicId && td.UserId == userId
                         orderby td.DateStart
                         select td);
            }

            if (!todos.Any())
            {
                return null;
            }

            if (willToggle)
            {
                var todo = todos.First();
                todo.Finished = !todo.Finished;

                db.SaveChanges();
            }

            return todos.First();
        }

        public async Task<bool> PopulateCyclicTodos(Guid userId, DateTime startDate, DateTime endDate, bool yearLong = false, int certainCyclicId = 0, bool onlyTemplate = false)
        {
            if (userId == Guid.Empty)
            {
                return true;
            }

            RemoveOrphanTodos(userId);
            //Task.Run(() => RemoveOrphanTodos(userId));
            var cyclics = db.TodoCyclic.Where(c => c.UserId == userId 
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
                DateTime cyclicEnd=DateTime.Now;
                if (cyclic.DateEnd.Month == 2 && cyclic.DateEnd.Day >28)
                {
                    if(DateTime.DaysInMonth(endDate.Year, cyclic.DateEnd.Month)<= cyclic.DateEnd.Day)
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
                        RelatedTodoTemplateId=cyclic.RelatedTodoTemplateId

                    };
                    newTodo.OnCreate(Utilities.GetUserName());
                    todoList.Add(newTodo);
                }
                else
                {
                    while (rStart >= cyclicStart && rStart < cyclicEnd)
                    {
                        DateTime currentStart = new DateTime(startDate.Year, cyclic.DateStart.Month, cyclic.DateStart.Day);
                        var hasDuplicates = db.Todoes.Where(t => t.UserId == cyclic.UserId && t.CyclicId == cyclic.Id && t.DateStart == rStart && !t.Deleted).FirstOrDefault();
                        if (hasDuplicates==null && currentStart <= rStart && rStart <= cyclicEnd)
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

            db.Todoes.AddRange(todoList);
            await db.SaveChangesAsync();

            return true;
        }

        public bool PopulateCyclicTodosSync(Guid userId, DateTime startDate, DateTime endDate, bool yearLong = false, int certainCyclicId = 0, bool onlyTemplate = false)
        {
            if (userId == Guid.Empty)
            {
                return true;
            }

            RemoveOrphanTodos(userId);
            //Task.Run(() => RemoveOrphanTodos(userId));
            var cyclics = db.TodoCyclic.Where(c => c.UserId == userId
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
                        var hasDuplicates = db.Todoes.Where(t => t.UserId == cyclic.UserId && t.CyclicId == cyclic.Id && t.DateStart == rStart && !t.Deleted).FirstOrDefault();
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

            db.Todoes.AddRange(todoList);
             db.SaveChanges();

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

        public void RemoveOrphanTodos(Guid userId)
        {
            var cyclics = (from t in db.TodoCyclic
                         join up in db.UserPlants on t.ReferenceId equals up.Id
                         where t.ReferenceType == ReferenceToModelClass.UserPlant && !t.Deleted && up.Deleted && t.UserId == userId
                         select t);
            var todos = (from t in db.Todoes
                         join tc in db.TodoCyclic on t.CyclicId equals tc.Id
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
            db.SaveChanges();
        }

        public void RemoveDuplicateTodoCyclicEntries()
        {
            var userId = Utilities.GetUserId();
            var empty = Guid.Empty;
            var todoCyclics = db.TodoCyclic.Where(t => t.UserId == userId && t.UserId != empty && !t.Deleted);
            List<TodoCyclic> duplicates = todoCyclics.GroupBy(x => x.Title)
                                                     .Where(g => g.Count() > 1)
                                                     .SelectMany(g => g.OrderBy(s => s.Id).Skip(1))
                                                     .ToList();
            var todos = db.Todoes.Where(t => t.UserId == userId && t.UserId != empty && !t.Deleted && t.CyclicId != null);
            List<Todo> duplicateTodos = todos.GroupBy(x => new { x.CyclicId, x.Index })
                                            .Where(x => x.Count() > 1)
                                            .SelectMany(t => t.OrderBy(s => s.Id).Skip(1))
                                            .ToList();
            if (duplicateTodos.Any())
            {
                foreach (var todo in duplicateTodos)
                {
                    todo.Deleted = true;
                    todo.Ignored = true;
                }
                db.SaveChanges();
            }
            if (duplicates.Any())
            {
                foreach (var todo in duplicates)
                {
                    List<Todo> todosToDelete = db.Todoes.Where(t => t.CyclicId == todo.Id).ToList();
                    foreach(var t in todosToDelete)
                    {
                        t.Deleted = true;
                        t.Ignored = true;
                    }
                    todo.Deleted = true;
                }
                db.SaveChanges();
            }
        }
        [NonAction]
        public async Task<IEnumerable<Todo>> DbGetTodosByUserId(DateTime startDate, DateTime endDate, Guid userId, bool includeFinished = false)
        {
            var todos = await DbGetTodos(startDate, endDate, includeFinished);
            return todos.Where(v => v.UserId == userId);
        }
        [NonAction]
        public async Task<IEnumerable<Todo>> DbGetTodosByGardenId(DateTime startDate, DateTime endDate, int gardenId, bool includeFinished = false)
        {
            UserPlantController gc = new UserPlantController();
            var gardenPlants = gc.DbGetUserPlantsByGardenId(gardenId);

            List<Todo> todoList = new List<Todo>();

            todoList.AddRange(await DbGetTodosByReference(startDate, endDate, ModelEnums.ReferenceToModelClass.Garden, gardenId));
            foreach (var plant in gardenPlants)
            {
                todoList.AddRange(await DbGetTodosByReference(startDate, endDate, ModelEnums.ReferenceToModelClass.UserPlant, plant.Id));
            }
            return todoList;
        }
        [NonAction]
        public async Task<IEnumerable<Todo>> DbGetTodosByReference(DateTime startDate, DateTime endDate, ModelEnums.ReferenceToModelClass referenceType, int referenceId, bool includeFinished = false)
        {
            var todos = await DbGetTodos(startDate, endDate, includeFinished);
            return todos.Where(v => v.ReferenceType == referenceType
            && v.ReferenceId == referenceId);
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
            return db.SaveChanges() > 0;
        }
        [NonAction]
        public void DbAddTemplateTodos(ModelEnums.ReferenceToModelClass tempRefType,
            int tempRefId,
            ModelEnums.ReferenceToModelClass newReferenceType,
            int newReferenceId,
            IEnumerable<_TodoCheckedTemplateViewModel> todos, Guid userId)
        {
            var templateIds = (from td in todos
                               where td.Checked
                               select td.TemplateId);

            var todoTemplatesPre = (from td in db.TodoTemplate
                                    where !td.Deleted
                                    && td.ReferenceId == tempRefId
                                    && td.ReferenceType == tempRefType
                                    select td).ToList();
            var todoTemplates = todoTemplatesPre.Where(v => templateIds.Contains(v.Id)).ToList();

            var existingCyclicTodos = db.TodoCyclic.Where(t => t.UserId == userId && (t.ReferenceType == ReferenceToModelClass.Plant || t.ReferenceType == ReferenceToModelClass.UserPlant) && !t.Deleted).ToList();
            todoTemplates = todoTemplates.Where(t => !existingCyclicTodos.Any(e => e.Description == t.Description && e.Title == t.Title && e.DateStart == t.DateStart && e.DateEnd == t.DateEnd && e.ReferenceId==t.ReferenceId)).ToList();
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
                    RelatedTodoTemplateId=template.Id
                    
                };
                DbAddTodo(vm, userId);
            }
            var endDate = new DateTime(DateTime.Now.Year+1, 1, 1);
            PopulateCyclicTodos(userId, DateTime.Now, endDate);
        }
        public bool resetUserTodosAsync(Guid userId)
        {
            
            if (userId != null)
            {
                DeletePlantsRelatedPopTodoAsync(userId);
                
                
            }
            return true;
        }
        public void DeletePlantsRelatedPopTodoAsync(Guid userId)
        {
            

            var editedTodoTemplates = GetEditedTemplateTodos().Select(temp=>temp.Id).ToArray();

            //var relateduserplants = userplants.Where(t=> editedTodoTemplates.Contains(t.Id));
            var relatedTodosWhithCyclicId = plantDB.Todoes.Where(t=> !t.Deleted && t.UserId == userId && t.ReferenceId != 0 && t.CyclicId != 0 && editedTodoTemplates.Contains(t.RelatedTodoTemplateId));

            if (relatedTodosWhithCyclicId != null && relatedTodosWhithCyclicId.Any())
            {
                foreach (Todo t in relatedTodosWhithCyclicId)
                {
                    t.Deleted = true;

                }
                plantDB.SaveChanges();
            }
            UpdateSingleTodosAfterRestting(userId, editedTodoTemplates);
            UpdateCyclicTodosAfterRestting(userId, editedTodoTemplates);
        }
        public void UpdateSingleTodosAfterRestting(Guid userId, int[] editedTodoTemplates)
        {
            var userSingleTodos = plantDB.Todoes.Where(t => !t.Deleted && t.UserId == userId && t.ReferenceId != 0 && editedTodoTemplates.Contains(t.RelatedTodoTemplateId));

            if (userSingleTodos != null && userSingleTodos.Any())
            {
                TodoTemplate temp = null;
                foreach (Todo t in userSingleTodos)
                {
                    temp = FindEditedTemplateTodoByID(t.RelatedTodoTemplateId);
                    if (temp != null)
                    {
                        t.Precision = temp.Precision;
                        t.ReferenceId = temp.ReferenceId;
                        t.Description = temp.Description;
                        t.DateStart = temp.DateStart;
                        t.DateEnd = temp.DateEnd;
                        t.Title = temp.Title;
                        t.ReferenceType = temp.ReferenceType;
                    }


                }
                plantDB.SaveChanges();
            }
        }
            public void UpdateCyclicTodosAfterRestting(Guid userId, int[] editedTodoTemplates)
        {
            //var userTodosCyclic = from ut in plantDB.TodoCyclic.Where(t => !t.Deleted && t.UserId == userId && t.ReferenceId != 0) join tt in GetEditedTemplateTodos() on new { ut.Title } equals new { tt.Title } select ut;
            var userTodosCyclic = plantDB.TodoCyclic.Where(t => !t.Deleted && t.UserId == userId && t.ReferenceId != 0 && editedTodoTemplates.Contains(t.RelatedTodoTemplateId));

            if (userTodosCyclic != null && userTodosCyclic.Any())
            {
                TodoTemplate temp = null;
                foreach (TodoCyclic t in userTodosCyclic)
                {
                    temp = FindEditedTemplateTodoByID(t.RelatedTodoTemplateId);
                    if (temp != null)
                    {
                        t.Cycle = temp.Cycle;
                        t.Precision = temp.Precision;
                        t.ReferenceId = temp.ReferenceId;
                        t.Description = temp.Description;
                        t.DateStart = temp.DateStart;
                        t.DateEnd = temp.DateEnd;
                        t.Title = temp.Title;
                        t.ReferenceType = temp.ReferenceType;
                    }


                }
                plantDB.SaveChanges();
                var endDate = new DateTime(DateTime.Now.Year + 1, 1, 1);
                PopulateCyclicTodos(userId, DateTime.Now, endDate);
            }
        }
        public IEnumerable<TodoTemplate> GetEditedTemplateTodos()
        {
            IEnumerable<TodoTemplate> todoTemplates = plantDB.TodoTemplate.Where(tt => tt.Edited && !tt.Deleted);
            return todoTemplates;
        }
        public TodoTemplate FindEditedTemplateTodoByID(int id)
        {
            TodoTemplate todoTemplate = plantDB.TodoTemplate.Where(tt => tt.Edited && !tt.Deleted && tt.Id==id).FirstOrDefault();
            return todoTemplate;
        }
        public void setResetTodoToTrueAsync(ApplicationUser user)
        {
            
            if (user != null)
            {
                user.ResetTodo = (int)MarkUserTodoReset.TodosReset;
                //IdentityResult res1 = UserManagerr.Update(user);
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
                    return (from db in db.Gardens
                            where db.Id == referenceId
                            select db.Name).FirstOrDefault();
                case ModelEnums.ReferenceToModelClass.UserPlant:
                    return (from db in db.UserPlants
                            where db.Id == referenceId
                            select db.Name).FirstOrDefault();
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
            db.SaveChanges();
        }
        [NonAction]
        public void DbSetTodoIgnored(int id)
        {
            var todo = DbGetTodo(id);
            if (todo != null)
            {
                todo.Ignored = true;
                todo.Deleted = true;
                todo.OnEdit(Utilities.GetUserName());
                db.SaveChanges();
            }
        }

        [NonAction]
        public void DbSetTodoFinished(int id)
        {
            var todo = DbGetTodo(id);
            todo.Finished = true;
            todo.OnEdit(Utilities.GetUserName());
            db.SaveChanges();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

    }
}
//self.kill()