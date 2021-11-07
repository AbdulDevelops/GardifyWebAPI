using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using GardifyNewsletter.Models;
using MoreLinq;
using System.Net.Mail;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization;

namespace GardifyNewsletter.Areas.Intern.Pages.Newsletter
{
    [Authorize(Roles = "Admin,Superadmin")]

    public class SendModel : PageModel
    {
        private readonly GardifyNewsletter.Models.ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        public SendModel(GardifyNewsletter.Models.ApplicationDbContext context, IHostingEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public SelectList NewsLetterList { get; set; }

        public IList<NewsletterSpool> NewsletterSpool { get; set; }
        public IList<NewsletterSpoolArchive> SpoolArchiveLastHour { get; set; }
        [BindProperty]
        public string SelectedSector { get; set; }
        public async Task OnGetAsync(string selectedSector)
        {
            SelectedSector = selectedSector;

            // entities dropdown
            NewsLetterList = new SelectList(_context.Newsletter
                .Where(l => l.ApplicationId.Equals(Program.APPLICATION_ID, StringComparison.InvariantCultureIgnoreCase))
                .OrderByDescending(n => n.WrittenDate), "NewsletterId", "NewsletterHeaderText");


            NewsletterSpool = await _context.NewsletterSpool.ToListAsync();
            var lastHour = DateTime.Now.AddHours(-1);
            SpoolArchiveLastHour = await _context.NewsletterSpoolArchive.Where(a => a.SendedDate >= lastHour).ToListAsync();

        }

        public IActionResult OnPostAddToSpool(int newsletterId, string selectedSector)
        {
            var newsletter = _context.Newsletter.Find(newsletterId);
            if (newsletter == null)
            {
                return RedirectToPage("./Index");
            }

            string Post_StatusNonSub = "https://gardifybackend.sslbeta.de/api/newsletterapi/getallgardifyuser";
            string Post_StatusSub = "https://gardifybackend.sslbeta.de/api/newsletterapi/getalluser";


            IEnumerable<GardifyModels.Models.NewsLetter> ApiResult;

            if (SelectedSector != null && SelectedSector.Contains("non-sub"))
            {
                ApiResult = apiCall(Post_StatusNonSub);
            }
            else if (SelectedSector != null && SelectedSector.Contains("all"))
            {
                IEnumerable<GardifyModels.Models.NewsLetter> nonSubList = null;
                IEnumerable<GardifyModels.Models.NewsLetter> subList = null;

                nonSubList = apiCall(Post_StatusNonSub);
                subList = apiCall(Post_StatusSub);
                ApiResult = nonSubList.Concat(subList);
            }
            else
            {
                ApiResult = apiCall(Post_StatusSub);
            }


            // NewsletterRecipientsLists = testReadSubscribersJson();
            var length = ApiResult.Count();

            var spoolEntries = ApiResult.Select(r => new NewsletterSpool
            {
                AddedToSpool = DateTime.Now,
                RecipientEmail = r.Email,
                Body = newsletter.NewsletterCompleteHtml,
                FromEmail = newsletter.SenderEmail,
                FromName = newsletter.SenderName,
                FromReplyTo = newsletter.SenderReplyTo,
                Html = true,
                UserId = r.UserId,
                NewsletterId = newsletter.NewsletterId,
                RecipientId = r.Id, // confirm if RecipientId is r.id in api
                Subject = newsletter.Subject,
            });

            _context.NewsletterSpool.AddRange(spoolEntries);
            _context.SaveChanges();

            return RedirectToPage("./SendNewsLetter", new { selectedSector = selectedSector });

        }

        public IActionResult OnGetClearSpool()
        {
            var spoolEntries = _context.NewsletterSpool;
            _context.NewsletterSpool.RemoveRange(spoolEntries);
            _context.SaveChanges();
            return RedirectToPage("./SendNewsLetter");
        }

        public async Task<IActionResult> OnGetSendSpoolAsync(int amount, string selectedSector)
        {
            var spoolEntries = _context.NewsletterSpool.Take(amount).ToList();


            //Send out all active entries
            foreach (var spoolEntry in spoolEntries)
            {
                string bodyModified;

                string newsletterUnsubscribe = "https://gardifybackend.sslbeta.de/api/newsletterapi/unsubscribe/";
                bodyModified = spoolEntry.Body.Replace("%GUID%", newsletterUnsubscribe + "" + spoolEntry.UserId.ToString());

                await SendEmailAsync(spoolEntry.FromEmail,
                    spoolEntry.FromName,
                    spoolEntry.RecipientEmail,
                    spoolEntry.FromReplyTo,
                    spoolEntry.Subject,
                    bodyModified);
            }

            //Write entries into SpoolArchive
            var archivedEntries = spoolEntries.Select(s => new NewsletterSpoolArchive
            {
                AddedToSpool = s.AddedToSpool,
                //Body = s.Body, //Body should stay empty to save space?
                Credentials = s.Credentials,
                FromEmail = s.FromEmail,
                FromName = s.FromName,
                FromReplyTo = s.FromReplyTo,
                Html = s.Html,
                NewsletterDistributionListId = s.NewsletterDistributionListId,
                NewsletterId = s.NewsletterId,
                Port = s.Port,
                RecipientEmail = s.RecipientEmail,
                RecipientId = s.RecipientId,
                Scheduled = s.Scheduled,
                Send = s.Send,
                SendedDate = DateTime.Now,
                SenderDomain = s.SenderDomain,
                Subject = s.Subject,
                UserId = s.UserId
            });
            _context.NewsletterSpoolArchive.AddRange(archivedEntries);

            //Remove entries from spool
            _context.RemoveRange(spoolEntries);
            _context.SaveChanges();

            return RedirectToPage("./SendNewsLetter");
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

        // getting recipients list from api 
        private static IEnumerable<GardifyModels.Models.NewsLetter> apiCall(string Post_Status)
        {
            IEnumerable<GardifyModels.Models.NewsLetter> result = null;
            using (var client = new HttpClient())
            {
                var stringContent = new StringContent(string.Empty);
                stringContent.Headers.ContentType = MediaTypeHeaderValue.Parse("application/x-www-form-urlencoded");
                var response = client.PostAsync(Post_Status, stringContent).Result;
                result = response.Content.ReadAsAsync<List<GardifyModels.Models.NewsLetter>>().Result;
                result = result.Where(u => (u.EmailConfirmed == true)); // takes only confirmed emails

            }

            return result;
        }

        private static IEnumerable<GardifyModels.Models.NewsLetter> testReadSubscribersJson()
        {
            IEnumerable<GardifyModels.Models.NewsLetter> NewsletterRecipientsLists = new List<GardifyModels.Models.NewsLetter>();

            using (StreamReader r = new StreamReader("C:\\test2.json"))
            {
                string json = r.ReadToEnd();
                NewsletterRecipientsLists = JsonConvert.DeserializeObject<List<GardifyModels.Models.NewsLetter>>(json);
            }

            return NewsletterRecipientsLists;
        }
    }
}
