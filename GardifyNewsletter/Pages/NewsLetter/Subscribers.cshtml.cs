using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GardifyNewsletter.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using GardifyNewsletter.Utilities;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Text;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;

namespace GardifyNewsletter.Areas.Intern.Pages.Distributors
{
    [Authorize(Roles = "Admin,Superadmin")]

    public class SubscriberModel : PageModel
    {
        private readonly GardifyNewsletter.Models.ApplicationDbContext _context;
        private IConfiguration configuration;
        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
        [BindProperty]
        public string SelectedSector { get; set; }
        public int Count { get; set; }
        public string SearchString { get; set; }

        /// <summary>
        /// pagination
        /// </summary>
        public int CurrentPage { get; set; } = 1;

        public int PageSize { get; set; } = 10;

        public int TotalPages => (int)Math.Ceiling(decimal.Divide(Count, PageSize));

        public bool EnablePrevious => CurrentPage > 1;

        public bool EnableNext => CurrentPage < TotalPages;
        // pagination end 

        public SubscriberModel(GardifyNewsletter.Models.ApplicationDbContext context, IConfiguration iConfig)
        {
            _context = context;
            configuration = iConfig;
        }

        public IList<GardifyModels.Models.NewsLetter> NewsletterRecipientsLists { get; set; }
        public NewsletterRecipients RecipientsSingleItem { get; set; }


        public async Task OnGetAsync(string sortOrder, string currentFilter, string searchString, string selectedSector, int pageIndex)
        {
            SelectedSector = selectedSector;

            string gardifyBackendApi = configuration.GetSection("SiteSettings").GetSection("GardifyBackendApi").Value;


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


            CurrentSort = sortOrder;

            CurrentFilter = searchString;
            if (!String.IsNullOrEmpty(searchString))
            {
                ApiResult = ApiResult.Where(u => (u.Email.ToLower().Contains(searchString.ToLower())));
            }

            // new pagination
            CurrentPage = pageIndex == null ? 1 : pageIndex;

            Count = ApiResult.Count();

            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;

            NewsletterRecipientsLists = ApiResult
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();
        }

        // getting recipients list from api 
        private static IEnumerable<GardifyModels.Models.NewsLetter> apiCall(string Post_Status)
        {
            IEnumerable < GardifyModels.Models.NewsLetter> result = null;
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

        public async Task<IActionResult> OnGetDeleteAsync(Guid id)
        {
            if (id == null)
            {
                return NotFound();
            }

            string Post_Status = "https://gardifybackend.sslbeta.de/api/newsletterapi/unsubscribe/"+id;
            WebClient webClient = new WebClient();
            string json = webClient.DownloadString(Post_Status);

            return RedirectToPage("./Subscribers");
        }
    }
}
