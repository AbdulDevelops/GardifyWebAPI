using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using static GardifyModels.Models.TodoViewModels;
using System.Data.Entity.Migrations;
using Microsoft.Ajax.Utilities;
using static GardifyModels.Models.ModelEnums;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;

namespace GardifyWebAPI.Controllers
{
    [RoutePrefix("api/TodoesAPI")]
    public class TodoesAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        private AspNetUserManager userManager;
        public AspNetUserManager UserManager
        {
            get
            {
                return userManager ?? Request.GetOwinContext().GetUserManager<AspNetUserManager>();
            }
            private set
            {
                userManager = value;
            }
        }
        // GET: api/TodoesAPI
        public async Task<TodoIndexViewModel> GetTodoes(string period = null, string startDate = "", int gid = 0, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), 0,(int)GardifyPages.Todo, EventObjectType.PageName);
               if (isIos)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromIos,(int)GardifyPages.Todo, EventObjectType.PageName);
                }
                else if (isAndroid)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromAndroid, (int)GardifyPages.Todo,  EventObjectType.PageName);
                }
                else if (isWebPage)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage, (int)GardifyPages.Todo,  EventObjectType.PageName);
                }


           
            TodoController tc = new TodoController();
            if (!DateTime.TryParse(startDate, out DateTime sDate))
            {
                sDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            }
            return await tc.Index(period, sDate, gid);
        }

        [ResponseType(typeof(int))]
        [Route("count")]
        public async System.Threading.Tasks.Task<TodoCountViewModel> GetTodoCountAsync(string period = null)
        {

            TodoController tc = new TodoController();
            Guid userId = Utilities.GetUserId();
            DateTime endDate = new DateTime(DateTime.Today.Year + 1, 1, 1).AddDays(-1);
            Task.Run(() => tc.PopulateCyclicTodos(userId, DateTime.Now, endDate, onlyTemplate: true));

            var startDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
            var count = new TodoCountViewModel();
            //GetTodoes(period: "yearEnd");

            var allTodos = db.Todoes.Where(t => t.UserId == userId && !t.Ignored && !t.Deleted && !t.Finished
                                            //&& t.DateStart.Month >= DateTime.Today.Month
                                            && t.DateStart.Year == DateTime.Today.Year);

            count.AllTodos = allTodos.Select(t => new { t.Title, t.DateStart }).Distinct().Count();
            count.AllTodosOfTheMonth = allTodos.Where(t =>t.DateStart.Month == DateTime.Today.Month ).Select(td => new { td.Title, td.DateStart, td.DateEnd, td.CyclicId }).Distinct().Count();

            

            return count;
        }

        

        [HttpPost]
        [Route("upload")]
        public IHttpActionResult UploadTodoImage()
        {
            if (Utilities.ActionAllowed(UserAction.NewTodoImage) == FeatureAccess.NotAllowed)
                return Unauthorized();

            var todoId = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);
            TodoController tc = new TodoController();
            if (HttpContext.Current.Request.Files[0] != null)
            {
                
                var x = 0;
                foreach(var file in HttpContext.Current.Request.Files)
                {
                    var imageTitleParamName = "imageTitle" + ((x == 0) ? "" : x.ToString());
                    var imageFile = HttpContext.Current.Request.Files[x];
                    var imageTitle = HttpContext.Current.Request.Params[imageTitleParamName];
                    HttpPostedFileBase filebase = new HttpPostedFileWrapper(imageFile);
                    tc.UploadTodoImage(filebase, todoId, imageTitle);
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        [Route("{imageId}/{todoId}/{isCyclic}")]
        [HttpDelete]
        public IHttpActionResult RemoveImageFromTodo(int imageId, int todoId, bool isCyclic)
        {
            var userId = Utilities.GetUserId();
            var sameOwner = true;
            var res = false;
            if (isCyclic)
            {
                var todoCyclic = db.TodoCyclic.Where(t => t.Id == todoId && t.UserId == userId);
                sameOwner = todoCyclic != null;

            }
            else
            {
                var todoCyclic = db.TodoCyclic.Where(t => t.Id == todoId && t.UserId == userId);
                sameOwner = todoCyclic != null;
            }
            if (!sameOwner)
                return BadRequest("Nicht erlaubt");

            //var entry = db.GardenAlbumFileToModules.Where(ga => ga.FileToModuleID == imageId && ga.GardenAlbumId == albumId).FirstOrDefault();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            res = rc.DbDeleteFile(imageId, todoId);

            //if (entry != null)
            //{
            //    db.GardenAlbumFileToModules.Remove(entry);
            //    db.SaveChanges();
            //}

            return Ok(res);
        }

        [HttpPost]
        [Route("fixCyclic")]
        public IHttpActionResult fixCyclic()
        {
            var cyclic = db.TodoCyclic.Where(t => t.GeneratedFromTemplate == true).ToList();

            var template = db.TodoTemplate.ToList();

            foreach (var cycle in cyclic)
            {
                var currentTemplate = template.FirstOrDefault(t => t.Title == cycle.Title);
                if (currentTemplate == null)
                {
                    continue;
                }

                if (currentTemplate.DateEnd.Month != cycle.DateEnd.Month )
                {
                    cycle.DateEnd = new DateTime(DateTime.Now.Year, currentTemplate.DateEnd.Month, cycle.DateEnd.Day); ;

                    db.TodoCyclic.AddOrUpdate(cycle);

                    db.SaveChanges();

                }
            }


            

            return StatusCode(HttpStatusCode.NoContent);
        }

        [ResponseType(typeof(Todo))]
        [Route("finished")]
        public async Task<TodoIndexViewModel> GetTodoFinished(string period = null)
        {
            TodoController tc = new TodoController();

            return await tc.IndexFinished(period);
        }


        // GET: api/TodoesAPI/5
        [ResponseType(typeof(Todo))]
        public TodoDetailsViewModel GetTodo(int id)
        {
            TodoController tc = new TodoController();

            return tc.Details(id);
        }

        [HttpGet]
        [Route("cyclic/{id}")]
        public TodoDetailsViewModel GetCyclicTodo(int id)
        {
            TodoController tc = new TodoController();
            var todo = (from td in db.TodoCyclic
                        where !td.Deleted
                        && td.Id == id
                        select td).FirstOrDefault();
            if (todo == null)
            {
                return null;
            }

            TodoDetailsViewModel vm = new TodoDetailsViewModel
            {
                Id = todo.Id,
                DateStart = todo.DateStart,
                DateEnd = todo.DateEnd,
                Deleted = todo.Ignored,
                Description = todo.Description,
                UserId = todo.UserId,
                ReferenceId = todo.ReferenceId,
                ReferenceType = todo.ReferenceType,
                Title = todo.Title,
                Ignored = todo.Ignored
            };

            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            HelperClasses.DbResponse imageResponse = null;
            if (todo.ReferenceId == 0)
            {
                imageResponse = rc.DbGetTodoReferencedImages(vm.Id);
            }
            else
            {
                var relatedPlantId = (from up in db.UserPlants where (up.Id == todo.ReferenceId) select up.PlantId).FirstOrDefault();
                imageResponse = rc.DbGetPlantReferencedImages(relatedPlantId);
            }
            vm.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));

            VideoReferenceController vrc = new VideoReferenceController();
            vm.ReferenceText = tc.DbGetReferenceText(vm.ReferenceType, vm.ReferenceId);
            vm.VideoReferenceList = vrc.GetVideoDetailsViewModelList(vm.Id, ModelEnums.ReferenceToModelClass.Todo);

            return vm;
        }

        [HttpPut]
        [Route("cyclicToggle/{id}/{gid}")]
        public IHttpActionResult ToggleCyclicTodo(int id, int gid = 0)
        {

            var sDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
            var eDate = new DateTime(DateTime.Today.Year + 1, DateTime.Today.Month, DateTime.Today.Day);
            TodoController tc = new TodoController();

            var selectedTodo = tc.DbGetTodosWithCyclic(sDate, eDate, gid, id, true);

            if (selectedTodo == null)
            {
                return BadRequest("Todo existiert nicht.");
            }

     

            return Ok();
        }

        [HttpGet]
        [Route("cyclicLatest/{id}/{gid}")]
        public TodoDetailsViewModel GetLatestCyclicTodo(int id, int gid = 0)
        {

            var sDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, DateTime.Today.Day);
            var eDate = new DateTime(DateTime.Today.Year + 1, DateTime.Today.Month, DateTime.Today.Day);
            TodoController tc = new TodoController();

            var todo = tc.DbGetTodosWithCyclic(sDate, eDate, gid, id);

            if (todo == null)
            {
                return null;
            }

            TodoDetailsViewModel vm = new TodoDetailsViewModel
            {
                Id = todo.Id,
                DateStart = todo.DateStart,
                DateEnd = todo.DateEnd,
                Deleted = todo.Ignored,
                Description = todo.Description,
                UserId = todo.UserId,
                ReferenceId = todo.ReferenceId,
                ReferenceType = todo.ReferenceType,
                Title = todo.Title,
                Ignored = todo.Ignored,
                Finished = todo.Finished
            };

            return vm;
        }

        [ResponseType(typeof(Todo))]
        [HttpPut]
        [Route("markfinished/{newState}/{taskId}/{cyclicId}")]
        public IHttpActionResult MarkTodoDone(int taskId, bool newState, double cyclicId=0)
        {
            TodoController tc = new TodoController();
            UserPlantController pc = new UserPlantController();

            var userId = Utilities.GetUserId();
            Guid empty = Guid.Empty;
            if (cyclicId>0) {
                var todoes = db.Todoes.Where(t =>t.UserId == userId && t.UserId != empty && !t.Deleted && t.CyclicId==cyclicId);
                if (todoes.Any())
                {
                    foreach (var td in todoes)
                    {
                        td.Finished = newState;
                        td.OnEdit(Utilities.GetUserName());
                    }
                }
                else
                {
                    return BadRequest("Todo existiert nicht.");
                }
                
            }
            else
            {
                var todo = db.Todoes.Where(t => t.Id == taskId && t.UserId == userId && t.UserId != empty && !t.Deleted).FirstOrDefault();
                if (todo == null)
                {
                    return BadRequest("Todo existiert nicht.");
                }
                todo.Finished = newState;
                todo.OnEdit(Utilities.GetUserName());
            }

            db.SaveChanges();


            var cacheUniqueId = userId.ToString() + "_mygarden";
            if (userId == Guid.Empty)
            {
                return Ok();
            }
            string cachedFilePath = HttpContext.Current.Server.MapPath("~") + "\\gardenCache\\" + cacheUniqueId + ".txt";
            var cachedData = pc.GetCachedIndex(cachedFilePath).ToList();

            if (cachedData == null)
            {
                return Ok();

            }
            foreach(var item in cachedData)
            {
                if (item.UserPlant.Todos.Any(t => t == null))
                {
                    continue;
                }
                if (item.UserPlant.Todos.Count() == 0)
                {
                    continue;
                }
                if (item.UserPlant.Todos.Any(t => t.CyclicId == taskId || t.Id == taskId))
                {
                    var todo = item.UserPlant.Todos.FirstOrDefault(t => t.CyclicId == taskId || t.Id == taskId);
                    var cyclic = item.UserPlant.CyclicTodos.FirstOrDefault(t => t.Id == todo.CyclicId);
                    cyclic.Finished = newState;
                }
            }

            pc.UpdateIndexCache(cachedData, cachedFilePath);


            return Ok();
        }

        [ResponseType(typeof(Todo))]
        [HttpPut]
        [Route("{todoId}/uploadTodo")]
        public IHttpActionResult PutTodoCyclic(int todoId, TodoDetailsViewModel todovm)
        {


            TodoController tc = new TodoController();
            if (todovm.CyclicId!=null)
            {
                Todo thisTodo = db.Todoes.Find(todoId);
                thisTodo.DateStart = todovm.DateStart;

                if (thisTodo != null)
                {
                    var cyclicId = thisTodo.CyclicId;
                    var alltodoWithThisCyclicId = (from t in db.Todoes where t.CyclicId == cyclicId select t).ToList();
                    foreach (var a in alltodoWithThisCyclicId)
                    {
                        a.Title = todovm.Title;
                        a.Finished = todovm.Finished;
                        a.Description = todovm.Description;
                        a.Deleted = todovm.Deleted ? todovm.Deleted : a.Deleted;
                        a.Notes = todovm.Notes;
                        a.Ignored = todovm.Ignored;
                        
                    }
                    var todoCyclic = (from t in db.TodoCyclic where t.Id == cyclicId select t).FirstOrDefault();
                    todoCyclic.Description = todovm.Description;
                    todoCyclic.Title = todovm.Title;
                    todoCyclic.Ignored = todovm.Ignored;
                }
            }
            else
            {
                var upTodo = db.Todoes.Where(t => t.Id == todoId).FirstOrDefault();
                if (upTodo != null)
                {
                    upTodo.Title = todovm.Title;
                    upTodo.Finished = todovm.Finished;
                    upTodo.Description = todovm.Description;
                    upTodo.DateStart = todovm.DateStart;
                    upTodo.DateEnd = todovm.DateEnd;
                    upTodo.Deleted = todovm.Deleted ? todovm.Deleted : upTodo.Deleted;
                    upTodo.Notes = todovm.Notes;
                    upTodo.Ignored = todovm.Ignored;
                }
                
            }
            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(todoId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        [ResponseType(typeof(Todo))]
        [HttpPut]
        [Route("{todoId}/updateCyclic")]
        public IHttpActionResult UpdateCyclicOnly(int todoId, TodoCyclicVM todovm)
        {
            TodoController tc = new TodoController();

            var alltodoWithThisCyclicId = (from t in db.Todoes where t.CyclicId == todoId select t).ToList();
            foreach (var a in alltodoWithThisCyclicId)
            {
                a.Title = string.IsNullOrEmpty(todovm.Title) ? a.Title : todovm.Title;
                a.Description = string.IsNullOrEmpty(todovm.Description) ? a.Description : todovm.Description;
                a.Ignored = todovm.Ignored;
            }

            var todoCyclic = (from t in db.TodoCyclic where t.Id == todoId select t).FirstOrDefault();
            todoCyclic.Description = string.IsNullOrEmpty(todovm.Description) ? todoCyclic.Description : todovm.Description;
            todoCyclic.Title = string.IsNullOrEmpty(todovm.Title) ? todoCyclic.Title : todovm.Title;
            todoCyclic.Ignored = todovm.Ignored;
            todoCyclic.DateStart = todovm.DateStart == null ? todoCyclic.DateStart : todovm.DateStart;
            todoCyclic.DateEnd = todovm.DateEnd == null ? todoCyclic.DateEnd : todovm.DateEnd;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TodoExists(todoId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/TodoesAPI
        [ResponseType(typeof(Todo))]
        [HttpPost]
        public async  Task<IHttpActionResult> PostTodo(TodoCreateViewModel vm)
        {
            if (Utilities.ActionAllowed(UserAction.NewTodo) == FeatureAccess.NotAllowed)
                return Unauthorized();

            TodoController tc = new TodoController();
            return Ok(await tc.Create(vm, Utilities.GetUserId()));
        }

        // DELETE: api/TodoesAPI/5/true
        [ResponseType(typeof(Todo))]
        [HttpDelete]
        [Route("{id}/{cyclic}")]
        public IHttpActionResult DeleteTodo(int id, bool cyclic)
        {
            TodoController tc = new TodoController();
            if (cyclic == true)
            {
                Todo todo = db.Todoes.Find(id);

                if (todo != null)
                {
                    var cyclicId = todo.CyclicId;
                    var alltodoWithThisCyclicId = from t in db.Todoes where t.CyclicId == cyclicId select t;
                    foreach (var a in alltodoWithThisCyclicId)
                    {
                        tc.MarkIgnored(a.Id);
                    }
                    
                    if (todo.ReferenceId == 0)
                    {
                         var todoCyclic = (from t in db.TodoCyclic where t.Id == todo.CyclicId select t).FirstOrDefault();
                        if (todoCyclic != null)
                        {
                            todoCyclic.Deleted = true;
                        }
                    }
                    else
                    {
                        var todoCyclic = (from t in db.TodoCyclic where t.Id == todo.CyclicId select t).FirstOrDefault();
                        if (todoCyclic != null)
                        {
                            todoCyclic.Deleted = true;
                        }
                        //var todoCyclic = (from t in db.TodoCyclic where t.ReferenceId == todo.ReferenceId select t);
                        //if (todoCyclic != null)
                        //{
                        //    foreach (var t in todoCyclic)
                        //    {
                        //        t.Deleted = true;
                        //    }

                        //}
                    }
                    
                    
                    db.SaveChanges();
                }
            }
            else
            {

                tc.MarkIgnored(id);
            }


            return Ok();
        }
        [HttpPut]
        [Route("resetTodos")]
        public async Task<IHttpActionResult> ResetTodosAsync()
        {
            var userId = Utilities.GetUserId();
            //var user = UserManager.FindById(userId.ToString());
            ApplicationUser user = await UserManager.FindByIdAsync(userId.ToString());
            TodoController tc = new TodoController();
             tc.resetUserTodosAsync(userId);
            if (user != null)
            {
                user.ResetTodo = (int)MarkUserTodoReset.TodosReset;
                IdentityResult res1 = UserManager.Update(user);
            }
            return Ok();
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TodoExists(int id)
        {
            return db.Todoes.Count(e => e.Id == id) > 0;
        }
    }
}