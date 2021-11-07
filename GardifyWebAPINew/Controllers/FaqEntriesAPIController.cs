using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using GardifyModels.Models;
using static GardifyModels.Models.FaqViewModels;
using static GardifyModels.Models.HelperClasses;
using GardifyWebAPI.App_Code;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;

namespace GardifyWebAPI.Controllers
{
    [RoutePrefix("api/FaqEntriesAPI")]
    public class FaqEntriesAPIController : ApiController
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

        // GET: api/FaqEntriesAPI
        public FaqViewModels.FaqIndexViewModel GetFaqEntries()
        {
            FaqController fc = new FaqController();
            return fc.Index();
        }

        [ResponseType(typeof(FaqCreateViewModel))]
        [Route("create")]
        public IEnumerable<IReferencedObject> GetFaqCreate()
        {
            FaqController fc = new FaqController();
            return fc.Create();
        }

        // GET: api/FaqEntriesAPI/5
        [ResponseType(typeof(FaqDetailsViewModel))]
        public async Task<IHttpActionResult> GetFaqEntry(int id)
        {
            FaqController fc = new FaqController();
            FaqEntry faqEntry = db.FaqEntries.Find(id);
            if (faqEntry == null)
            {
                return NotFound();
            }

            var answerer = await UserManager.FindByIdAsync(faqEntry.AnswerAuthorId.ToString());
            int answererCount = db.FaqEntries.Where(e => e.AnswerAuthorId.ToString().Equals(answerer.Id)
                                                      || e.QuestionAuthorId.ToString().Equals(answerer.Id)).Count();
            int askerCount = db.FaqEntries.Where(e => e.AnswerAuthorId.ToString().Equals(faqEntry.QuestionAuthorId.ToString())
                                                   || e.QuestionAuthorId.ToString().Equals(faqEntry.QuestionAuthorId.ToString())).Count();

            FaqDetailsViewModel vm = new FaqDetailsViewModel()
            {
                QuestionText = faqEntry.QuestionText,
                AnswerText = faqEntry.AnswerText,
                Date = faqEntry.Date,
                Id = faqEntry.Id,
                CreatedBy = faqEntry.CreatedBy,
                AnsweredBy = answerer.FirstName,
                AnswererPlace = answerer.City,
                AnswererCount = answererCount,
                AskerCount = askerCount,
                Answers = fc.GetEntryAnswersByIdWithCount(faqEntry.Id)
            };

            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            HelperClasses.DbResponse imageResponse = rc.DbGetDiaryEntryReferencedImages(vm.Id);
            vm.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, System.Web.Hosting.HostingEnvironment.MapPath("~/"));

            return Ok(vm);
        }

        // PUT: api/FaqEntriesAPI/5
        [ResponseType(typeof(void))]
        public FaqViewModels.FaqIndexViewModel PutFaqEntry(int id, FaqEntry faqEntry)
        {
            FaqController fc = new FaqController();
            //fc.DbEditFaqEntry(faqEntry);
            return fc.Index();
        }

        public class CreateModel
        {
            public FaqCreateViewModel vm { get; set; }

            public HttpPostedFileBase imageFile { get; set; }

            //...other properties    
        }

        // POST: api/FaqEntriesAPI
        [HttpPost]
        [Route("api/FaqEntriesAPI")]
        public FaqViewModels.FaqIndexViewModel PostFaqEntry(CreateModel model)
        {
            FaqCreateViewModel vm = model.vm;
            HttpPostedFileBase imageFile = model.imageFile;
            FaqController fc = new FaqController();

            return fc.Create(vm, imageFile);
        }

        [HttpPost]
        [Route("api/FaqEntriesAPI/answer")]
        public IHttpActionResult PostFaqAnswer(FaqAnswer fa)
        {
            if (!ModelState.IsValid || !fa.AuthorId.Equals(Utilities.GetUserId().ToString()))
            {
                return BadRequest("Die Antwort ist nicht vollständig.");
            }
            fa.OnCreate(Utilities.GetUserName());
            db.FaqAnswers.Add(fa);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = fa.Id }, fa);
        }

        // DELETE: api/FaqEntriesAPI/5
        [ResponseType(typeof(FaqEntry))]
        public IHttpActionResult DeleteFaqEntry(int id)
        {
            FaqEntry faqEntry = db.FaqEntries.Find(id);
            if (faqEntry == null)
            {
                return NotFound();
            }

            db.FaqEntries.Remove(faqEntry);
            db.SaveChanges();

            return Ok(faqEntry);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool FaqEntryExists(int id)
        {
            return db.FaqEntries.Count(e => e.Id == id) > 0;
        }
    }
}