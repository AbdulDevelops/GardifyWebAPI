using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using GardifyWebAPI.Services;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace GardifyWebAPI.Controllers
{
    public class NewsLetterController : ApiController
    {
        private ApplicationDbContext pl = new ApplicationDbContext();
        
        const string FRONTEND_URL = "https://gardify.de/newsletter-bestaetigt";

        const string NEWSLETTER_POST_KEY = "cf9210c4-3f71-4f5e-9311-e413f9a3e18f";
        // GET: api/NewsLetter
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/NewsLetter/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/NewsLetter
        public NewsLetter getEmailToConfirm(string userId, string token, string email)
        { 
            Guid userIdGuid = Guid.Parse(userId);
            var emailToConfirm = (from n in pl.NewsLetters where n.Email==email && !n.EmailConfirmed && n.UserId == userIdGuid select n).FirstOrDefault();
            return emailToConfirm;    
        }


        public async Task<List<NewsLetter>> getNewsletterUserList()
        {
            //if (key != NEWSLETTER_POST_KEY){
            //    return new List<NewsLetter>();
            //}
            var emailToConfirm = (from n in pl.NewsLetters where n.EmailConfirmed && !n.Deleted select n).ToList();
            return emailToConfirm;
        }

        public async Task<List<NewsLetter>> getAllUserList()
        {
            //if (key != NEWSLETTER_POST_KEY){
            //    return new List<NewsLetter>();
            //}

            var user = (from u in pl.Users where u.EmailConfirmed && !u.Email.Contains("Deleted") && !u.Email.Contains("UserDemo") select u).ToList().Select(u => new NewsLetter
            {
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Birthday = DateTime.Now,
                Gender = "Herr / Frau",
                UserId = Guid.Parse(u.Id),
                EmailConfirmed = !u.EmailConfirmed ? false : !u.NewsletterUnsubscribe


            });

            var emailToConfirm = (from n in pl.NewsLetters where n.EmailConfirmed && !n.Deleted select n).ToList();

            return user.Where(u => !emailToConfirm.Where(e => e.UserId == u.UserId || e.Email == u.Email).Any()).ToList();

        }

        public IHttpActionResult newsLetterSubscriberManual(NewsLetter newsLetter)
        {
            NewsLetter nl = new NewsLetter();
            var existUser = (from i in pl.Users where i.EmailConfirmed && !i.Deleted && i.Email == newsLetter.Email select i).FirstOrDefault();
            var userId = Guid.NewGuid();
            if(existUser != null)
            {
                userId = new Guid(existUser.Id);
            }
       
;
            var existEmail = (from e in pl.NewsLetters where !e.Deleted && e.Email == newsLetter.Email select e).FirstOrDefault();
            if (newsLetter != null && existEmail == null)
            {
                try
                {
                    nl.FirstName = newsLetter.FirstName;
                    nl.LastName = newsLetter.LastName;
                    nl.Email = newsLetter.Email;
                    nl.Gender = "Herr/Frau";
                    nl.Birthday = DateTime.Now;
                    nl.UserId = userId;
                    nl.EmailConfirmed = true;
                    nl.OnCreate(Utilities.GetUserName());
                    pl.NewsLetters.Add(nl);
                    bool ok = pl.SaveChanges() > 0 ? true : false;
                    if (ok == true)
                    {

                    }
                }

                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
            }
            else
            {
                return BadRequest("Ein Fehler ist aufgetreten: Sie haben sich bereits für den Newsletter angemeldet.");
            }
            return null;
        }
        public async Task<bool> newsLetterSubscriber(NewsLetter newsLetter,string code)
        {
            EmailSender es = new EmailSender(pl);
            var userId = Utilities.GetUserId() == Guid.Empty ? Guid.NewGuid() : Utilities.GetUserId();
            TemplateService ts = new TemplateService();
            NewsLetter nl = new NewsLetter();
            var uri = new Uri(FRONTEND_URL + $"?userId={userId}&code={HttpUtility.UrlEncode(code)}&email={newsLetter.Email}");
            string content = ts.RenderTemplateAsync("subscribe", new { UserName= newsLetter.FirstName , Action_Url = uri, UserId=userId }) ;
            var existEmail = checkedIfNewsletterAbo(newsLetter.Email);
            bool res = false;
            if (newsLetter != null && existEmail==null) {
                
                try
                {
                    nl.FirstName = newsLetter.FirstName;
                    nl.LastName = newsLetter.LastName;
                    nl.Email = newsLetter.Email;
                    nl.Gender = "Herr/Frau";
                    nl.Birthday = DateTime.Now;
                    nl.UserId = userId;
                    nl.EmailConfirmed = false;
                    nl.OnCreate(Utilities.GetUserName());
                    pl.NewsLetters.Add(nl);
                    bool ok= pl.SaveChanges()>0? true: false;
                   
                    if (ok == true) {
                         res = await es.SendEmail("NewsLetter Bestellung", content,"info@gardify.de", nl.Email, null);
                        
                    }
                }

                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
               
              //return Ok("Der Newsletter wurde erfolgreich bestellt.");
            }
            //else
            //{
            //    return BadRequest("Ein Fehler ist aufgetreten: Sie haben sich bereits für den Newsletter angemeldet.");
            //}
            return res;
        }
        public async Task<bool> newsLetterUnSubscriber(string userIdToRe=null)
        {


            TemplateService ts = new TemplateService();
            EmailSender es = new EmailSender(pl);
            var userId = userIdToRe != null ? new Guid(userIdToRe) : Utilities.GetUserId();
            var subscriberEmail = (from e in pl.NewsLetters where e.UserId == userId && !e.Deleted select e).FirstOrDefault();
            var res = false;
            var isExist = false;
            var username = "";
            var userEmail = "";

            if (subscriberEmail != null)
            {
                isExist = true;
                username = subscriberEmail.FirstName;
                userEmail = subscriberEmail.Email;
                subscriberEmail.Deleted = true;
            }

            var normalMail = (from e in pl.Users where e.Id == userIdToRe select e).FirstOrDefault();

            if (normalMail != null)
            {
                isExist = true;
                username = normalMail.FirstName;
                userEmail = normalMail.Email;

                normalMail.NewsletterUnsubscribe = true;

            }

            if (isExist)
            {
                try
                {
                    bool ok = pl.SaveChanges() > 0 ? true : false;
                    var agbUrl = new Uri("https://gardify.de/agb");
                    string content = ts.RenderTemplateAsync("unsubscribeConfirmation", new { UserName = username, UserId = userId });
                    if (ok == true)
                    {
                        res = await es.SendEmail("Abbestellung", content, "info@gardify.de", userEmail, null);
                    }
                }
                catch (DbEntityValidationException e) { throw; };
                return res;
            }
            else
            {
                return res;

            }

        }

        public async Task<bool> newsLetterGardifyUnSubscriber(string userIdToRe = null)
        {
            TemplateService ts = new TemplateService();
            EmailSender es = new EmailSender(pl);
            var userId = userIdToRe != null ? new Guid(userIdToRe) : Utilities.GetUserId();
            var existEmail = (from e in pl.Users where e.Id == userIdToRe select e).FirstOrDefault();
            var res = false;
            if (existEmail != null)
            {
                try
                {
                    //existEmail.Deleted = true;
                    existEmail.NewsletterUnsubscribe = true;
                    bool ok = pl.SaveChanges() > 0 ? true : false;
                    var agbUrl = new Uri("https://gardify.de/agb");
                    string content = ts.RenderTemplateAsync("unsubscribeConfirmation", new { UserName = existEmail.FirstName, UserId = userId });
                    if (ok == true)
                    {
                        //res = await es.sendemail("abbestellung", content, "info@gardify.de", existemail.email, null);
                    }
                }
                catch (DbEntityValidationException e) { throw; };
                return res;
            }
            else
            {
                return res;
            }

        }

        public bool newsLetterUnSubscriberNoMail(string userIdToRe = null)
        {
            TemplateService ts = new TemplateService();
            EmailSender es = new EmailSender(pl);
            var userId = userIdToRe != null ? new Guid(userIdToRe) : Utilities.GetUserId();
            var existEmail = (from e in pl.NewsLetters where e.UserId == userId && !e.Deleted select e).FirstOrDefault();
            if (existEmail != null)
            {
                try
                {
                    existEmail.Deleted = true;
                    bool ok = pl.SaveChanges() > 0 ? true : false;
                    var agbUrl = new Uri("https://gardify.de/agb");
                    string content = ts.RenderTemplateAsync("unsubscribeConfirmation", new { UserName = existEmail.FirstName, UserId = userId });
                    if (ok == true)
                    {
                        //var res = await es.SendEmail("Abbestellung", content, "info@gardify.de", existEmail.Email, null);
                    }
                }
                catch (DbEntityValidationException e) { throw; };
                return true;
            }
            else
            {
                return false;
            }

        }

        public NewsLetter checkedIfNewsletterAbo(string email)
        {
           var existEmail = (from e in pl.NewsLetters where !e.Deleted && e.Email == email select e).FirstOrDefault();
            return existEmail;
        }
        // PUT: api/NewsLetter/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/NewsLetter/5
        public void Delete(int id)
        {
        }
       

            
    }
}
