using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;

namespace GardifyModels.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public bool Deleted { get; set; }
        [Required]
        public int Points { get; set; }
        [StringLength(64)]
        public string ProfileUrl { get; set; }
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager, string authenticationType)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, authenticationType);
            // Add custom user claims here
            return userIdentity;
        }
        [Required]
        public string FirstName { get; set; }
        [Required]
        public string LastName { get; set; }
        public bool CompleteSignup { get; set; } // einfaches oder vollständiges Registrieren

        public bool NewsletterUnsubscribe { get; set; }
        public bool HasPremium { get; set; }
        public DateTime? PremiumStarts { get; set; }
        public DateTime? PremiumEnds { get; set; }
        public string Gender { get; set; }  // "Herr" oder "Frau"
        [Required]
        public string Street { get; set; }
        [Required]
        public string HouseNr { get; set; }
        [Required]
        public int PLZ { get; set; }
        [Required]
        public string City { get; set; }
        [Required]
        public string Country { get; set; }
        public DateTime? LastAlerted { get; set; }
        public DateTime? LastLogin { get; set; }
        public DateTime? RegisterDate { get; set; }
        public virtual UserSettings Settings { get; set; }

        public string ReferralURL { get; set; }
        public string RequestURL { get; set; }


        public int ResetTodo { get; set; } 

        public string DisplayName()
        {
            if (FirstName != "Platzhalter" && LastName != "Platzhalter" && !FirstName.Contains("UserDemo") && !LastName.Contains("UserDemo"))
            {
                return FirstName + " " + LastName;
            }
            return UserName;
        }
    }
}