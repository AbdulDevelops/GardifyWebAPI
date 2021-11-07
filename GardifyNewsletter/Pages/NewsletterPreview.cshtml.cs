using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace GardifyNewsletter.Pages
{
    public class NewsletterPreviewModel : PageModel
    {
        private readonly GardifyNewsletter.Models.ApplicationDbContext _context;

        public NewsletterPreviewModel(GardifyNewsletter.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet(int id)
        {
            var newsletter = _context.Newsletter.Find(id);
            if (newsletter == null)
            {
                return Redirect("https://gardify.de/NewsletterPreview");
            }
            return Content(newsletter.NewsletterCompleteHtml, "text/html", Encoding.UTF8);
        }
    }
}