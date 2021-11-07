using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GardifyNewsletter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace GardifyNewsletter.Pages.NewsLetter
{

    public class Newsletter_SignupModel : PageModel
    {
        private readonly Models.ApplicationDbContext _context;
        [BindProperty]
        public String SelectedSector { get; set; }
        [BindProperty]

        public string Email { get; set; }
        [BindProperty]
        public bool IsChecked { get; set; }

        public Newsletter_SignupModel(Models.ApplicationDbContext context)
        {
            _context = context;
        }
        [BindProperty]
        public string ErrorText { get; set; }
        [BindProperty]
        public IList<CheckBoxItem_NewsletterSignup> AllDistributors { get; set; }

        public async Task OnGetAsync()
        {
            AllDistributors = await _context.NewsletterDistributionLists.Select(vm => new CheckBoxItem_NewsletterSignup()
            {
                Id = vm.NewsletterDistributionListId,
                Name = vm.NewsletterDistributionListName,
                IsChecked = false
            }).ToListAsync();

        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (Email == null)
            {
                ErrorText = "Email can not be null!";
                return Page();
            }
            else if (IsChecked != true)
            {
                ErrorText = "Please accept terms conditions";
                return Page();
            }
            else
            {
                for (int i = 0; i < AllDistributors.Count(); i++)
                {
                    if (AllDistributors[i].IsChecked)
                    {

                        NewsletterRecipients NewsletterRecipients = new NewsletterRecipients();
                        NewsletterRecipients.RecipientEmail = Email;
                        NewsletterRecipients.NewsletterDistributionListId = AllDistributors[i].Id;
                        NewsletterRecipients.Confirmed = false;
                        NewsletterRecipients.Active = false;

                        NewsletterRecipients.WrittenDate = System.DateTime.Now;
                        NewsletterRecipients.EditedDate = System.DateTime.Now;
                        NewsletterRecipients.WrittenBy = User.Identity.Name;
                        NewsletterRecipients.EditedBy = User.Identity.Name;
                        _context.NewsletterRecipients.Add(NewsletterRecipients);

                    }

                }
                await _context.SaveChangesAsync();

                return RedirectToPage();
            }
        }
    }
}
