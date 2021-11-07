using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using GardifyNewsletter.Models;
using Microsoft.AspNetCore.Authorization;

namespace GardifyNewsletter.Areas.Intern.Pages.Newsletter
{
    [Authorize(Roles = "Admin,Superadmin")]
    public class IndexModel : PageModel
    {
        private readonly GardifyNewsletter.Models.ApplicationDbContext _context;
        public IndexModel(GardifyNewsletter.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        public IList<Models.Newsletter> Newsletter { get;set; }
        public Models.Newsletter NewsLetterSingleItem { get; set; }

        public IList<NewsletterComponents> NewsletterComponents { get; set; }

        public string CurrentFilter { get; set; }
        public string CurrentSort { get; set; }
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

        public async Task OnGetAsync(int pageIndex)
        {
            IEnumerable <Models.Newsletter> NewsletterX = await _context.Newsletter.OrderByDescending(n => n.WrittenDate).ToListAsync();

            // new pagination
            CurrentPage = pageIndex == null ? 1 : pageIndex;

            Count = NewsletterX.Count();

            if (CurrentPage > TotalPages)
                CurrentPage = TotalPages;

            Newsletter = NewsletterX
            .Skip((CurrentPage - 1) * PageSize)
            .Take(PageSize)
            .ToList();
        }

        public async Task<IActionResult> OnGetDeleteAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            NewsLetterSingleItem = await _context.Newsletter.FindAsync(id);


            if (NewsLetterSingleItem != null)
            {
                _context.Newsletter.Remove(NewsLetterSingleItem);

                // remove range related to specified id
                _context.NewsletterComponents.RemoveRange(_context.NewsletterComponents.Where(p => p.BelongsToNewsletterId == id));
                await _context.SaveChangesAsync();
            }
            return RedirectToPage("./Index");
        }
    }
}
