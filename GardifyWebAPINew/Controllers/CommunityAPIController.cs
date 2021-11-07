using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;

using System.Web.Mvc;
using HttpGetAttribute = System.Web.Http.HttpGetAttribute;
using HttpPostAttribute = System.Web.Http.HttpPostAttribute;
using RouteAttribute = System.Web.Http.RouteAttribute;

namespace GardifyWebAPI.Controllers
{
    public class CommunityAPIController : ApiController
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
        private CommunityController cc = new CommunityController();
        // GET: CommunityAPI
        [HttpGet]
        [Route("api/CommunityAPI/getAllEntry")]
        public async Task<IEnumerable<CommunityPostViewModel>> GetAsync( bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            return await cc.IndexAsync( isIos, isAndroid, isWebPage);
        }

        [HttpGet]
        [Route("api/CommunityAPI/getAllEntryWithAnswers")]
        public async Task<IEnumerable<CommunityPostWithAnswerViewModel>> GetAllPostsWithRelatedAnswersAsync(bool isIos = false, bool isAndroid = false, bool isWebPage = false, int skip = 0, int take = int.MaxValue)
        {
            
            return await cc.GetListOfPostsWithRelatedAnswersAsync(isIos, isAndroid, isWebPage, skip, take);
        }

        [System.Web.Http.HttpPost]
        [Route("api/CommunityAPI/newEntry")]
        public async Task<IHttpActionResult> Post(CommunityPostEntryModel entrymodel, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
 
            if (entrymodel != null)
            {
                var userId = Utilities.GetUserId().ToString();
                ApplicationUser user = await UserManager.FindByIdAsync(userId);
                if (user == null || user.Email.Contains("UserDemo"))
                {
                    return Ok(0);
                }
                return Ok(await cc.PostQuestion(entrymodel, user, isIos, isAndroid, isWebPage));
            }
            else
            {
                return Ok(0);
            }

        }

        [HttpPost]
        [Route("api/CommunityAPI/answer")]
        public async Task<IHttpActionResult> PostAnswer(CommunityAnswerViewModel answerModel, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {


            var userId = Utilities.GetUserId().ToString();
            ApplicationUser user = await UserManager.FindByIdAsync(userId);

            if (answerModel != null && user != null && !user.Email.Contains("UserDemo"))
            {
                return Ok(cc.PostAnswer(answerModel, isIos, isAndroid, isWebPage));
            }
            else
            {
                return Ok(0);
            }

        }

        [HttpGet]
        [Route("api/CommunityAPI/{id}/getEntry")]
        public async Task<CommunityPostDetailViewModel> GetCommunityById(int id)
        {
            var userId = Utilities.GetUserId().ToString();
            ApplicationUser user = await UserManager.FindByIdAsync(userId);
            return await cc.DetailAsync(id, user);
        }

        [HttpPost]
        [Route("api/CommunityAPI/uploadQuestionImage")]
        public IHttpActionResult UploadRelatedQuestionImage()
        {
            var answerId = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);

            if (HttpContext.Current.Request.Files[0] != null)
            {
                var imageFile = HttpContext.Current.Request.Files[0];
                var imageTitle = HttpContext.Current.Request.Params["imageTitle"];
                HttpPostedFileBase filebase = new HttpPostedFileWrapper(imageFile);
                cc.UploadPostImage(filebase, imageFile, answerId, imageTitle);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

        [HttpPost]
        [Route("api/CommunityAPI/uploadAnswerImg")]
        public IHttpActionResult UploadRelatedAnswerImage()
        {
            var answerId = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);

            if (HttpContext.Current.Request.Files[0] != null)
            {
                var imageFile = HttpContext.Current.Request.Files[0];
                var imageTitle = HttpContext.Current.Request.Params["imageTitle"];
                HttpPostedFileBase filebase = new HttpPostedFileWrapper(imageFile);
                cc.UploadAnswerImage(filebase, imageFile, answerId, imageTitle);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }

    }
}