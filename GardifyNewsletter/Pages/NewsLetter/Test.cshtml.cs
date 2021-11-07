using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GardifyNewsletter.Models;
using System.Net.Mail;
using System.Net;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authorization;
using System.Net.Http;
using System.Net.Http.Headers;

namespace GardifyNewsletter.Areas.Intern.Pages.Newsletter
{
    [Authorize(Roles = "Admin,Superadmin")]

    public class TestModel : PageModel
    {
        private readonly ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        public TestModel(ApplicationDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public SelectList NewsLetterList { get; set; }


        public void OnGet()
        {
            ViewData["SuccessMessage"] = TempData["SuccessMessage"];
            TempData["SuccessMessage"] = null;
            NewsLetterList = new SelectList(_context.Newsletter.OrderByDescending(n => n.WrittenDate), "NewsletterId", "NewsletterHeaderText");
        }

        public async Task<IActionResult> OnPostAsync(int newsLetterId, string recipient)
        {
            var newsLetter = _context.Newsletter.Find(newsLetterId);
            if (newsLetter == null)
            {
                return RedirectToPage("./Index");
            }

            await SendEmailAsync(newsLetter.SenderEmail, newsLetter.SenderName, recipient, newsLetter.SenderReplyTo, newsLetter.Subject, newsLetter.NewsletterCompleteHtml);
            TempData["SuccessMessage"] = "Testemail erfolgreich versandt.";
            return RedirectToPage("./Test");

        }

        /// <summary>
        /// This should be in a dedicated email service, but I had no time to implement that
        /// And it definitely shouldn't be both in Test.cshtml.cs AND SendNewsLetter.cshtml.cs
        /// </summary>
        /// <param name="fromEmail"></param>
        /// <param name="fromName"></param>
        /// <param name="to"></param>
        /// <param name="replyTo"></param>
        /// <param name="subject"></param>
        /// <param name="message"></param>
        /// <param name="files"></param>
        /// <param name="storeLocally"></param>
        /// <returns></returns>
        public async Task SendEmailAsync(string fromEmail, string fromName, string to, string replyTo, string subject, string message, IDictionary<string, string> files = null, bool storeLocally = false)
        {
            message = unsubscribeTest(message, to);

            // if replyTo empty then set fromEmail to replyTo
            if (replyTo == null)
            {
                replyTo = fromEmail;
            }


            try
            {
                MailMessage mail = new MailMessage
                {
                    From = new MailAddress(fromEmail, fromName),
                    Subject = subject,
                    Body = message,
                    IsBodyHtml = true
                };

                mail.ReplyToList.Add(replyTo);
                mail.To.Add(new MailAddress(to));

                if (files != null)
                {
                    foreach (var file in files)
                    {
                        var fileName = Path.Combine(_env.WebRootPath, "files", file.Key);
                        Attachment at = new Attachment(fileName)
                        {
                            Name = file.Value
                        };
                        at.ContentDisposition.FileName = file.Value;
                        mail.Attachments.Add(at);
                    }
                }

                using (SmtpClient smtp = new SmtpClient())
                {
                    if (!storeLocally)
                    {
                        smtp.EnableSsl = true;
                        smtp.Host = "mail.netzlab14.de";
                        smtp.Port = 587;
                        smtp.Credentials = new NetworkCredential("web@mail.netzlab14.de", "netzLab14!");
                    }
                    else
                    {
                        smtp.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
                        smtp.PickupDirectoryLocation = "C:\\EMail";
                    }
                    await smtp.SendMailAsync(mail);
                }
            }
            catch
            {
                await Task.CompletedTask;
            }
        }

        /**
         * Check if test email exists in database, attach UserId for unsubscribe test 
         **/
        private string unsubscribeTest(string message, string toEmail)
        {

            // getting recipients from api 
            string Post_Status = "https://gardifybackend.sslbeta.de/api/newsletterapi/getalluser";
            List<GardifyModels.Models.NewsLetter> NewsletterRecipientsLists = null;

            using (var client = new HttpClient())
            {
                var stringContent = new StringContent(string.Empty);
                stringContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
                var response = client.PostAsync(Post_Status, stringContent).Result;
                NewsletterRecipientsLists = response.Content.ReadAsAsync<List<GardifyModels.Models.NewsLetter>>().Result;
            }

            GardifyModels.Models.NewsLetter userDetail = NewsletterRecipientsLists.SingleOrDefault(x => x.Email.Equals(toEmail));

            if (userDetail!=null)
            {
                string newsletterUnsubscribe = "https://gardifybackend.sslbeta.de/api/newsletterapi/unsubscribe/";

                return message = message.Replace("%GUID%", newsletterUnsubscribe + userDetail.UserId.ToString());
            } else
            {
               return message;
            }

        }
    }
}
