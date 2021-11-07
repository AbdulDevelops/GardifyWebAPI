using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using GardifyWebAPI.Services;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;

namespace GardifyWebAPI.Controllers
{
    public class NewsLetterAPIController : ApiController
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

        // POST: api/NewsLetterAPI
        [ResponseType(typeof(NewsLetter))]
        [HttpPost]
        public async Task<IHttpActionResult> Post(NewsLetter nl)
        {
            NewsLetterController nlc = new NewsLetterController();
            var userId = Utilities.GetUserId();
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(userId.ToString());
            var result = await nlc.newsLetterSubscriber(nl, code);
            if (result)
            {
                return Ok("Der Newsletter wurde erfolgreich bestellt.");
            }
            else
            {
                return BadRequest("Sie haben sich bereits für den Newsletter angemeldet.");
                
            }
            
        }

        // POST: api/NewsLetterAPI
        [ResponseType(typeof(NewsLetter))]
        [HttpPost]
        [Route("api/newsletterapi/addmanual")]
        public IHttpActionResult PostNewSubscriber(NewsLetter nl)
        {
            NewsLetterController nlc = new NewsLetterController();
            nlc.newsLetterSubscriberManual(nl);
            return Ok();
        }

        // POST: api/NewsLetterAPI
        [ResponseType(typeof(NewsLetter))]
        [HttpPost]
        [Route("api/newsletterapi/addmanuals")]
        public IHttpActionResult PostNewSubscriberMultiple(NewsLetter[] nls)
        {
            NewsLetterController nlc = new NewsLetterController();
            foreach (var nl in nls)
            {
                nlc.newsLetterSubscriberManual(nl);

            }
            return Ok();
        }

        // GET: api/NewsLetterAPI/unsubscribe/
        [ResponseType(typeof(NewsLetter))]
        [HttpGet]
        [Route("api/newsletterapi/unsubscribe/{userId}")]
        public async Task<string> GetUnsuscribe(string userId)
        {
            NewsLetterController nlc = new NewsLetterController();
            await nlc.newsLetterUnSubscriber(userId);
            return "Dein Newsletter wurde abbestellt.";
        }

        [ResponseType(typeof(NewsLetter))]
        [HttpGet]
        [Route("api/newsletterapi/gardifyunsubscribe/{userId}")]
        public async Task<string> GetGardifyUnsuscribe(string userId)
        {
            NewsLetterController nlc = new NewsLetterController();
            await nlc.newsLetterGardifyUnSubscriber(userId);
            return "Dein Newsletter wurde abbestellt.";
        }

        [ResponseType(typeof(NewsLetter))]
        [HttpGet]
        [Route("api/newsletterapi/unsubscribenomail/{userId}")]
        public async Task<string> GetUnsuscribeNomail(string userId)
        {
            NewsLetterController nlc = new NewsLetterController();
            nlc.newsLetterUnSubscriberNoMail(userId);
            return "Dein Newsletter wurde abbestellt.";
        }


        [ResponseType(typeof(NewsLetter))]
        [HttpPost]
        [Route("api/newsletterapi/unsubscribe")]
        public async Task<IHttpActionResult> postUnsuscribe()
        {
            NewsLetterController nlc = new NewsLetterController();
             var result=await nlc.newsLetterUnSubscriber();
            if (result)
            {
                return Ok("Der Newsletter wurde erfolgreich abbestellt.");
            }
            else
            {
                return BadRequest("Ein Fehler ist aufgetreten: Sie haben sich bereits für den Newsletter abgemeldet.");

            }
        }



        [ResponseType(typeof(NewsLetter))]
        [HttpPost]
        [Route("api/newsletterapi/getalluser")]
        public async Task<List<NewsLetter>> getAllNewsletter()
        {
            NewsLetterController nlc = new NewsLetterController();
            return await nlc.getNewsletterUserList();
        }

        [ResponseType(typeof(NewsLetter))]
        [HttpPost]
        [Route("api/newsletterapi/getallgardifyuser")]
        public async Task<List<NewsLetter>> getAllGardifyUser()
        {
            NewsLetterController nlc = new NewsLetterController();
            return await nlc.getAllUserList();
        }
        // GET: api/NewsLetterAPI/confirm/
        [HttpGet]
        [Route("api/newsletterapi/confirm")]
        public async Task<IHttpActionResult> userConfirmEmailAsync(string userId, string code, string email)
        {
           
            NewsLetterController nlc = new NewsLetterController();
            if (userId == null || code== null)
            {
                return BadRequest("Ein Fehler ist aufgetreten: Ungültiges URL.");
            }
            else
            {
                if (code != null && userId != null)
                {
                    Guid userIdGuid = Guid.Parse(userId);
                    NewsLetter emailToConfirm = (from n in db.NewsLetters where n.Email == email && !n.EmailConfirmed && n.UserId == userIdGuid select n).FirstOrDefault();
                    try {
                        if (emailToConfirm != null)
                        {
                            emailToConfirm.EmailConfirmed = true;
                            db.SaveChanges();
                        }
                        else
                        {
                            return BadRequest("Ein Fehler ist aufgetreten.");
                        }
                    }
                    catch (InvalidOperationException ioe)
                    {
                        return BadRequest("Ein Fehler ist aufgetreten.");
                    }
                }
            }
           
            return Ok();
        }
        // PUT: api/NewsLetterAPI/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/NewsLetterAPI/5
        public void Delete(int id)
        {
        }
    }
}
