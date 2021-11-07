using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using GardifyModels.Models;
using PflanzenApp.App_Code;
using PflanzenApp.Controllers.AdminArea;
using static GardifyModels.Models.InternalCommentViewModels;
using static GardifyModels.Models.ModelEnums;

namespace PflanzenApp.Controllers
{
    public class InternalCommentController : _BaseController
    {
        public bool Create(string internalComment, int id, ModelEnums.ReferenceToModelClass type)
        {
            InternalComment ic = new InternalComment
            {
                ReferenceId = id,
                ReferenceType = type,
                Text = internalComment,
                Finished = false,
                UserId = Utilities.GetUserId()
            };
            ic.OnCreate(Utilities.GetUserName());
            return DbAddInternalComment(ic);
        }


        //public List<InternalCommentViewModels.InternalCommentDetailsViewModel> GetInternalCommentDetailsViewModelsByRefferenceId(ModelEnums.ReferenceToModelClass type, int refId)
        //{
        //    List<InternalCommentViewModels.InternalCommentDetailsViewModel> list = new List<InternalCommentViewModels.InternalCommentDetailsViewModel>();
        //    var comments = DbGetInternalCommentByRefferenceId(type, refId);
        //    foreach(var com in comments)
        //    {
        //        list.Add(GetInternalCommentDetailsViewModel(com));
        //    }
        //    return list;
        //}

        //public InternalCommentViewModels.InternalCommentIndexViewModel GetInternalCommentIndexViewModelsByRefferenceId(ModelEnums.ReferenceToModelClass type, int refId)
        //{
        //    AdminAreaAccountController aaac = new AdminAreaAccountController();
        //    PlantController pc = new PlantController();
        //    string name;
        //    if (type == ModelEnums.ReferenceToModelClass.Plant)
        //    {
        //        name = pc.DbGetPlantName(refId);
        //    }
        //    else
        //    {
        //        /*Add other ReferenceObjects here!*/
        //        return null;
        //    }
        //    var list = GetInternalCommentDetailsViewModelsByRefferenceId(type, refId);
        //    InternalCommentViewModels.InternalCommentIndexViewModel vm = new InternalCommentViewModels.InternalCommentIndexViewModel
        //    {
        //        ReferenceName = name,
        //        ListOfComments = list
        //    };
        //    return vm;
        //}
        [NonAction]
        public bool MarkFinished(int id)
        {
            return DbSetInternalCommentFinished(id);
        }


        // GET: InternalComment
        public ActionResult Index()
        {
            InternalCommentIndexViewModel vm = GetInternalCommentIndexViewModel();
            return View(vm);
        }

        // GET: InternalComment/Details/5
        public ActionResult MarkFinished(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            bool succ = MarkFinished((int)id);
            if (!succ)
            {
                return HttpNotFound();
            }
            return RedirectToAction("Index");
        }

        // GET: InternalComment/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InternalComment internalComment = ctx.InternalComment.Find(id);
            if (internalComment == null)
            {
                return HttpNotFound();
            }
            return View(internalComment);
        }

        // GET: InternalComment/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: InternalComment/Create
        // Aktivieren Sie zum Schutz vor übermäßigem Senden von Angriffen die spezifischen Eigenschaften, mit denen eine Bindung erfolgen soll. Weitere Informationen 
        // finden Sie unter http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Text,ReferenceId,ReferenceType,CreatedBy,CreatedDate,EditedBy,EditedDate,Deleted")] InternalComment internalComment)
        {
            if (ModelState.IsValid)
            {
                ctx.InternalComment.Add(internalComment);
                ctx.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(internalComment);
        }

        // GET: InternalComment/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InternalComment internalComment = ctx.InternalComment.Find(id);
            if (internalComment == null)
            {
                return HttpNotFound();
            }
            return View(internalComment);
        }

        // GET: InternalComment/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            InternalComment internalComment = ctx.InternalComment.Find(id);
            if (internalComment == null)
            {
                return HttpNotFound();
            }
            return View(internalComment);
        }

        // POST: InternalComment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            InternalComment internalComment = ctx.InternalComment.Find(id);
            ctx.InternalComment.Remove(internalComment);
            ctx.SaveChanges();
            return RedirectToAction("Index");
        }

        public IEnumerable<InternalComment> DBGetInternalComments(ReferenceToModelClass refType, int refId, bool includeFinished = false)
        {
            return DBGetInternalComments(includeFinished).Where(i => !i.Deleted && i.ReferenceType == refType && i.ReferenceId == refId);
        }
        #region VM
        public InternalCommentIndexViewModel GetInternalCommentIndexViewModel(ModelEnums.ReferenceToModelClass referenceType, int referenceId)
        {
            var vm = GetInternalCommentIndexViewModel();
            vm.ListOfComments = vm.ListOfComments.Where(v => v.ReferenceId == referenceId && v.ReferenceType == v.ReferenceType);
            return vm;
        }

        public InternalCommentIndexViewModel GetInternalCommentIndexViewModel()
        {
            var ics = DBGetInternalComments();
            return new InternalCommentIndexViewModel { ListOfComments = GetInternalCommentDetailsViewModels(ics) };
        }

        // Original logic taken from GetInternalCommentDetailsViewModel()
        public IEnumerable<InternalCommentDetailsViewModel> GetInternalCommentDetailsViewModels(IEnumerable<InternalComment> internalComments)
        {
            AdminAreaAccountController aaac = new AdminAreaAccountController();
            PlantController pc = new PlantController();
            var interalCommentDetailsViewModels = internalComments.Select(ic => new InternalCommentDetailsViewModel
            {
                Id = ic.Id,
                Finished = ic.Finished,
                ReferenceId = ic.ReferenceId,
                ReferenceType = ic.ReferenceType,
                // Technically requesting the usernames could be parallelized, but practically the internal comments
                // section probably won't ever have enough comments to slow it down noticeably
                UserName = aaac.DbGetUser(ic.UserId).UserName,
                Text = ic.Text,
            });

            foreach(var ic in interalCommentDetailsViewModels)
            {
                switch(ic.ReferenceType)
                {
                    case ModelEnums.ReferenceToModelClass.Plant:
                        ic.ReferenceName = pc.DbGetPlantName(ic.ReferenceId, true); //TODO: Wenn Kommentare für mehrere Typen supported werden Code anpassen
                        break;
                    case ModelEnums.ReferenceToModelClass.GeneralInternalComment:
                        ic.ReferenceName = "Allgemeiner Kommentar";
                        break;
                    default:
                        ic.ReferenceName = ic.ReferenceType.ToString();
                        break;
                }
            }

            return interalCommentDetailsViewModels;
        }

        public InternalCommentDetailsViewModel GetInternalCommentDetailsViewModel(InternalComment ic)
        {
            AdminAreaAccountController aaac = new AdminAreaAccountController();
            PlantController pc = new PlantController();
            if (ic.ReferenceType == ModelEnums.ReferenceToModelClass.Plant)
            {
                InternalCommentDetailsViewModel vm = new InternalCommentDetailsViewModel
                {
                    Id = ic.Id,
                    Finished = ic.Finished,
                    ReferenceId = ic.ReferenceId,
                    ReferenceType = ic.ReferenceType,
                    UserName = aaac.DbGetUser(ic.UserId).UserName,
                    ReferenceName = pc.DbGetPlantName(ic.ReferenceId, true), //TODO: Wenn Kommentare für mehrere Typen supported werden Code anpassen
                    Text = ic.Text
                };
                return vm;
            }
            else if (ic.ReferenceType == ModelEnums.ReferenceToModelClass.GeneralInternalComment)
            {
                InternalCommentDetailsViewModel vm = new InternalCommentDetailsViewModel
                {
                    Id = ic.Id,
                    Finished = ic.Finished,
                    ReferenceId = ic.ReferenceId,
                    ReferenceType = ic.ReferenceType,
                    UserName = aaac.DbGetUser(ic.UserId).UserName,
                    ReferenceName = "Allgemeiner Kommentar",
                    Text = ic.Text
                };
                return vm;
            }
            else
            {
                InternalCommentDetailsViewModel vm = new InternalCommentDetailsViewModel
                {
                    Id = ic.Id,
                    Finished = ic.Finished,
                    ReferenceType = ic.ReferenceType,
                    ReferenceName = ic.ReferenceType.ToString(),
                    Text = ic.Text,
                    ReferenceId = ic.ReferenceId,
                    UserName = aaac.DbGetUser(ic.UserId).UserName
                };
                return vm;
            }
        }
        #endregion
        #region DB
        [NonAction]
        public IEnumerable<InternalComment> DBGetInternalComments(bool includedFinished = false)
        {
            return ctx.InternalComment.Where(v => !v.Deleted && includedFinished ? true : !v.Finished);
        }
        [NonAction]
        public bool DbSetInternalCommentFinished(int id)
        {
            var com = (from c in ctx.InternalComment
                       where c.Id == id
                       && !c.Deleted
                       select c).FirstOrDefault();
            com.Finished = true;
            com.OnEdit(Utilities.GetUserName());
            return ctx.SaveChanges() > 0;
        }
        [NonAction]
        public bool DbAddInternalComment(InternalComment ic)
        {
            ctx.InternalComment.Add(ic);
            return (ctx.SaveChanges() > 0);
        }

        [NonAction]
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ctx.Dispose();
            }
            base.Dispose(disposing);
        }
        #endregion

    }
}
