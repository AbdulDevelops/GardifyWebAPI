using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using GardifyNewsletter.Models;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;

namespace GardifyNewsletter.Areas.Intern.Pages.Newsletter
{
    [Authorize(Roles = "Admin,Superadmin")]
    public class CreateModel : PageModel
    {
        private readonly GardifyNewsletter.Models.ApplicationDbContext _context;

        public CreateModel(GardifyNewsletter.Models.ApplicationDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Models.Newsletter Newsletter { get; set; }

        [BindProperty]
        public string ErrorText { get; set; }
        public IActionResult OnGet()
        {
            Newsletter = new Models.Newsletter();
            Newsletter.NewsletterHeaderText = "NEWSLETTER";
            Newsletter.NewsletterDateShownOnNewsletter = @DateTime.Now.ToString("MMMM yyyy");
            return Page();
        }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // validate email
            var attr = new EmailAddressAttribute();
            var isValidSender = attr.IsValid(Newsletter.SenderEmail);
            var isValidReplyTo = attr.IsValid(Newsletter.SenderReplyTo); // check if reply to email is valid

            if (!isValidSender || !isValidReplyTo || Newsletter.SenderEmail == null)
            {
                ErrorText = "Bitte geben Sie eine gültige E-Mail-Adresse ein.";
                return Page();
            }
            else
            {

                Newsletter.Active = false;
                Newsletter.Deleted = false;
                Newsletter.NewsletterSentDate = null;
                Newsletter.Sort = null;

                Newsletter.WrittenBy = User.Identity.Name;
                Newsletter.WrittenDate = DateTime.Now;
                Newsletter.EditedBy = User.Identity.Name;
                Newsletter.EditedDate = DateTime.Now;
                Newsletter.ApplicationId = Program.APPLICATION_ID;

                _context.Newsletter.Add(Newsletter);
                await _context.SaveChangesAsync();

                return RedirectToPage("./Edit", new { id = Newsletter.NewsletterId });
            }
        }
    }
}