using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using GardifyWebAPI.Services;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using static GardifyModels.Models.ModelEnums;

namespace GardifyWebAPI.Controllers
{
    public class PlantDocAPIController : ApiController
    {
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
        private ApplicationDbContext db = new ApplicationDbContext();
        private PlantDocController pd = new PlantDocController();
        public PlantDocAPIController()
        {
        }

        // GET: api/PlantDocAPI
        [HttpGet]
        [Route("api/plantDocAPI/getAllEntry")]
        public IEnumerable<PlantDocViewModel> Get(DateTime? readDate = null, bool isIos = false, bool isAndroid = false, bool isWebPage = false, bool getImage = true, bool showAnswer = true, int skip = 0, int take = -1)
        {
            return pd.Index(readDate, isIos, isAndroid, isWebPage, getImage, showAnswer, skip, take);
        }



        [HttpGet]
        [Route("api/plantDocAPI/getCurrentUserPosts")]
        public IEnumerable<PlantDocViewModel> GetPlantDocByUserId(DateTime? readDate = null)
        {
            var currentUser = Utilities.GetUserId();
            if (currentUser == Guid.Empty)
            {
                return new List<PlantDocViewModel>();
            }
            return pd.getPostByUserId(currentUser, readDate);

        }

        [HttpGet]
        [Route("api/plantDocAPI/{id}/getEntry")]
        public async Task<PlantDocDetailViewModel> GetPlantDocById(int id)
        {
            var userId = Utilities.GetUserId().ToString();
            ApplicationUser user = await UserManager.FindByIdAsync(userId);
            return await pd.DetailAsync(id, user);
        }
        [HttpGet]
        [Route("api/plantDocAPI/{id}/getPostById")]
        public PlantDoc GetPostById(int id)
        {
            return pd.getPostbyId(id);
        }

        [HttpGet]
        [Route("api/plantDocAPI/{id}/getAnswerById")]
        public PlantDocAnswer GetAnswerById(int id)
        {
            return pd.getAnswerById(id);
        }
        [HttpGet]
        [Route("api/plantDocAPI/count")]
        public int  GetPlantDocAnswerCount()
        {
            return  pd.totalNotReadedAnswers();
        }

        [HttpGet]
        [Route("api/plantDocAPI/notread")]
        public List<int> GetPlantDocNotReadCount()
        {
            return pd.getNotReadedAnswersId();
        }
        // POST: api/PlantDocAPI/
        [HttpPost]
        [Route("api/plantDocAPI/newEntry")]
        public async Task<IHttpActionResult> Post(PlantDocEntryModel entrymodel, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            if (Utilities.ActionAllowed(UserAction.PlantDoc) == FeatureAccess.NotAllowed)
                return Unauthorized();

            if (entrymodel != null)
            {
                var userId = Utilities.GetUserId().ToString();
                ApplicationUser user = await UserManager.FindByIdAsync(userId);
                if (user == null || user.Email.Contains("UserDemo"))
                {
                    return Ok(0);
                }
                return Ok(await pd.PostQuestion(entrymodel, user,  isIos ,  isAndroid ,  isWebPage));
            }
            else
            {
                return Ok(0);
            }

        }
        [HttpPost]
        [Route("api/plantDocAPI/answer")]
        public async Task<IHttpActionResult> PostAnswer(AnswerViewModel answerModel, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            if (Utilities.ActionAllowed(UserAction.PlantDoc) == FeatureAccess.NotAllowed)
                return Unauthorized();

            var userId = Utilities.GetUserId().ToString();
            ApplicationUser user = await UserManager.FindByIdAsync(userId);

            if (answerModel != null && user != null && !user.Email.Contains("UserDemo"))
            {
                return Ok(pd.PostAnswer(answerModel,  isIos,  isAndroid ,  isWebPage));
            }
            else
            {
                return Ok(0);
            }

        }

        [ResponseType(typeof(PlantDoc))]
        [HttpPut]
        [Route("api/plantDocAPI/{postId}/updatePost")]
        public async Task<IHttpActionResult> UpdatePostAsync(int postId, PlantDocViewModel model )
        {
            var userId = Utilities.GetUserId().ToString();
            ApplicationUser user = await UserManager.FindByIdAsync(userId);
            if (model != null && user != null)
            {
                pd.UpdatePostAsync(postId, model, user);
            }
            return Ok();
        }

        [ResponseType(typeof(PlantDoc))]
        [HttpPut]
        [Route("api/plantDocAPI/{answerId}/updateAnswer")]
        public IHttpActionResult updateAnswer(int answerId, PlantDocAnswerViewModel model)
        {
            var userId = Utilities.GetUserId();
            if (model != null && userId != Guid.Empty)
            {
                pd.UpdateAnswer(answerId, model, userId);
            }
            return Ok();
        }

        [HttpPost]
        [Route("api/plantDocAPI/upload")]
        public async Task<IHttpActionResult> UploadRelatedQuestImageAsync()
        {
            var userId = Utilities.GetUserId().ToString();
            ApplicationUser user = await UserManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest();
            }
            
            var questionId = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);

            var uploaded = new List<string>();
            for (var i = 0; i < HttpContext.Current.Request.Files.Count; i++)
            {
                var imageFile = HttpContext.Current.Request.Files[i];
                var imageTitle = HttpContext.Current.Request.Params["imageTitle" + i.ToString()];

                HttpPostedFileBase filebase = new HttpPostedFileWrapper(imageFile);
               
                var res = pd.UploadAndRegisterQuestImgFile(filebase, imageFile, questionId, (int)ModelEnums.ReferenceToModelClass.PlantDocQuestion, ModelEnums.FileReferenceType.QuestionImage, imageTitle);
                if (res != null)
                {
                    uploaded.Add(res.FullRelativePath);
                }

            }
            pd.uploadQuestionImgFull(user, questionId, uploaded);
            //if (HttpContext.Current.Request.Files[0] != null)
            //{
            //    var imageFile = HttpContext.Current.Request.Files[0];
            //    var imageTitle = HttpContext.Current.Request.Params["imageTitle"];
            //    HttpPostedFileBase filebase = new HttpPostedFileWrapper(imageFile);
            //    pd.UploadQuestImage(filebase, questionId, imageTitle);
            //}
            return StatusCode(HttpStatusCode.NoContent);
        }
        [HttpPost]
        [Route("api/plantDocAPI/uploadAnswerImg")]
        public IHttpActionResult UploadRelatedAnswerImage()
        {
            var answerId = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);

            if (HttpContext.Current.Request.Files[0] != null)
            {
                var imageFile = HttpContext.Current.Request.Files[0];
                var imageTitle = HttpContext.Current.Request.Params["imageTitle"];
                HttpPostedFileBase filebase = new HttpPostedFileWrapper(imageFile);
                pd.UploadAnswerImage(filebase, imageFile, answerId, imageTitle);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
        // PUT: api/PlantDocAPI/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/PlantDocAPI/5
        public void Delete(int id)
        {
        }
    }
}
