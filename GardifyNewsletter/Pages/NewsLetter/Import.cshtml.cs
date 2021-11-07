using GardifyNewsletter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace GardifyNewsletter.Areas.Intern.Pages.NewsLetter
{
    [Authorize(Roles = "Admin,Superadmin")]

    public class ImportModel : PageModel
    {
        private readonly GardifyNewsletter.Models.ApplicationDbContext _context;
        private readonly IHostingEnvironment _environment;
        public int Count { get; set; }
        [BindProperty]
        public IList<NewsletterRecipients> NewsletterRecipients { get; set; }
        //[BindProperty]
        //public int? SelectedSector { get; set; }
        [BindProperty]
        public string ImportedFileName { get; set; }
        [BindProperty]
        public string Keywords { get; set; }
        [BindProperty]
        public string ErrorText { get; set; }
        [BindProperty]
        public string SuccessText { get; set; }
        public ImportModel(IHostingEnvironment environment, ApplicationDbContext context)
        {
            _environment = environment;
            _context = context;

        }

        [BindProperty]
        public IFormFile Upload { get; set; }

        //[BindProperty]
        //public IList<SelectListItem> DistributorsList { get; set; }
        public async Task OnGetAsync()
        {

            // entities dropdown
            //DistributorsList = await _context.NewsletterDistributionLists.Select(a =>
            //                          new SelectListItem
            //                          {
            //                              Value = a.NewsletterDistributionListId.ToString(),
            //                              Text = a.NewsletterDistributionListId.ToString() + ". " + a.NewsletterDistributionListName.ToString()
            //                          }).ToListAsync();

        }

        public async Task<IActionResult> OnPostAsync()
        {

            if (ImportedFileName == null)
            {
                ErrorText = "Geben Sie einen Dateinamen ein!";
                return Page();
            }
            else if (Upload == null)
            {
                ErrorText = "Keine Datei zum Hochladen angegeben";
                return Page();
            }
            else
            {

                var guid = Guid.NewGuid().ToString();
                string fileExtension = Path.GetExtension(Upload.FileName);

                var uniqueFileName = guid + ImportedFileName + fileExtension;


                string filename = Path.Combine(_environment.WebRootPath, "files", uniqueFileName);
                using (var fileStream = new FileStream(filename, FileMode.Create))
                {
                    await Upload.CopyToAsync(fileStream);
                }
                NewsletterRecipients = GetRecipientsList(filename);

                var list = NewsletterRecipients.Select(d => new {
                    LastName = "aa",
                    FirstName = "aa",
                    Email = d.RecipientEmail
                });

                // saving to api

                var json = JsonConvert.SerializeObject(list);

                string import_url = "https://gardifybackend.sslbeta.de/api/newsletterapi/addmanuals";

                var client = new RestClient(import_url);
                client.Timeout = -1;
                var request = new RestRequest(Method.POST);
                request.AddHeader("Content-Type", "application/json");
                request.AddParameter("application/json", json.ToString(), ParameterType.RequestBody);
                IRestResponse response = client.Execute(request);
                Console.WriteLine(response.Content);


                SuccessText = NewsletterRecipients.Count + " E-Mails importiert.";
                // saving to db

                //var newEmails = NewsletterRecipients.Select(x => x.RecipientEmail);
                //var existingEntries = _context.NewsletterRecipients
                //    .Where(r => r.NewsletterDistributionListId == SelectedSector && newEmails.Contains(r.RecipientEmail))
                //    .Select(r => r.RecipientEmail);
                //NewsletterRecipients = NewsletterRecipients.Where(r => !existingEntries.Contains(r.RecipientEmail)).ToList();
                //_context.AddRange(NewsletterRecipients);
                //_context.SaveChanges();
                //Count = NewsletterRecipients.Count;



                return Page();
            }

        }

        private List<NewsletterRecipients> GetRecipientsList(string fileName)
        {
            var lines = System.IO.File.ReadAllLines(fileName);
            var data = lines.Select(l => l.Split(";"));
            var recipients = data.Select(d => new NewsletterRecipients
            {
                RecipientEmail = d[0],
                Active = true,
                Confirmed = true,
                WrittenDate = System.DateTime.Now,
                EditedDate = System.DateTime.Now,
                WrittenBy = User.Identity.Name,
                EditedBy = User.Identity.Name
            });
            return recipients.ToList();
        }
    }



}
