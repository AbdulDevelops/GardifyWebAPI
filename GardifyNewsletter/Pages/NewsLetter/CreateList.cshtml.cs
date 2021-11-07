using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GardifyNewsletter.Models;
using Microsoft.AspNetCore.Authorization;

namespace GardifyNewsletter.Areas.Intern.Pages.Distributors
{
    [Authorize(Roles = "Admin,Superadmin")]

    public class CreateListModel : PageModel
    {
        private readonly GardifyNewsletter.Models.ApplicationDbContext _context;

        public CreateListModel(GardifyNewsletter.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            return Page();
        }

        [BindProperty]
        public bool IsChecked { get; set; }

        [BindProperty]
        public NewsletterDistributionLists NewsletterDistributionLists { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.NewsletterDistributionLists.Add(NewsletterDistributionLists);

            NewsletterDistributionLists.WrittenDate = DateTime.Now;
            NewsletterDistributionLists.EditedDate = DateTime.Now;
            NewsletterDistributionLists.EditedBy = User?.Identity?.Name;
            NewsletterDistributionLists.WrittenBy = User?.Identity?.Name;
            NewsletterDistributionLists.Active = IsChecked; // öffentlich wählbar
            NewsletterDistributionLists.ApplicationId = Program.APPLICATION_ID;
            await _context.SaveChangesAsync();

            return RedirectToPage("./IndexList");
        }
    }
}