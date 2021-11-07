using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GardifyNewsletter.Models;
using Microsoft.AspNetCore.Authorization;

namespace GardifyNewsletter.Areas.Intern.Pages.Distributors
{
    [Authorize(Roles = "Admin,Superadmin")]

    public class IndexListModel : PageModel
    {
        private readonly GardifyNewsletter.Models.ApplicationDbContext _context;

        public IndexListModel(GardifyNewsletter.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<NewsletterDistributionLists> NewsletterDistributionLists { get;set; }

        public async Task OnGetAsync()
        {
            NewsletterDistributionLists = await _context.NewsletterDistributionLists.ToListAsync();
        }
    }
}
