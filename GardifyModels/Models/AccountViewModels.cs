using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace GardifyModels.Models
{
    public class CreateUserViewModel : _BaseViewModel
    {
        [Required]
        [Display(Name = "E-Mail")]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Kennwort")]
        public string Password { get; set; }
        public IEnumerable<UserRoleCheckedViewModel> Roles { get; set; }
    }

    public class EditUserViewModel : _BaseViewModel
    {
        public Guid Id { get; set; }
        [Required]
        [Display(Name = "E-Mail")]
        [EmailAddress]
        public string Email { get; set; }

        [Display(Name = "Benutzer Name")]
        public string BenutzerName { get; set; }
        [Display(Name = "E-Mail bestätigt")]
        
        public bool EmailConfirmed { get; set; }
        public IEnumerable<UserRoleCheckedViewModel> Roles { get; set; }
    }

    public class ChangeUserDataViewModel : _BaseViewModel
    {
        [Required]
        public Guid Id { get; set; }
        [Required]
        [EmailAddress]
        public string NewEmail { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }

    public class EditPasswordViewModel
    {
        public Guid Id { get; set; }
        public string OldPassword { get; set; }
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
    }

    public class DeleteUserViewModel : _BaseViewModel
    {
        public Guid Id { get; set; }
        [Required]
        [Display(Name = "E-Mail")]
        [EmailAddress]
        public string Email { get; set; }
    }

    public class DetailsUserViewModel : _BaseViewModel
    {
        public Guid Id { get; set; }
        [Required]
        [Display(Name = "E-Mail")]
        [EmailAddress]
        public string Email { get; set; }      
        [Display(Name = "Benutzergruppe")]  
        public IEnumerable<string> RoleNames { get; set; }
    }

    public class UserRoleCheckedViewModel : _BaseViewModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public bool Checked { get; set; }
    }

    public class IndexUserViewModel : _BaseViewModel
    {
        public IEnumerable<DetailsUserViewModel> UserList { get; set; }
    }

    public class ExternalLoginConfirmationViewModel : _BaseViewModel
	{
        [Required]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }
    }

    public class ExternalLoginListViewModel : _BaseViewModel
	{
        public string ReturnUrl { get; set; }
    }

    public class SendCodeViewModel : _BaseViewModel
	{
        public string SelectedProvider { get; set; }
        public ICollection<System.Web.Mvc.SelectListItem> Providers { get; set; }
        public string ReturnUrl { get; set; }
        public bool RememberMe { get; set; }
    }

    public class VerifyCodeViewModel : _BaseViewModel
	{
        [Required]
        public string Provider { get; set; }

        [Required]
        [Display(Name = "Code")]
        public string Code { get; set; }
        public string ReturnUrl { get; set; }

        [Display(Name = "Diesen Browser merken?")]
        public bool RememberBrowser { get; set; }

        public bool RememberMe { get; set; }
    }

    public class ForgotViewModel : _BaseViewModel
	{
        [Required]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }
    }

    public class LoginViewModel : _BaseViewModel
	{
        [Required]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Kennwort")]
        public string Password { get; set; }

        [Display(Name = "Speichern?")]
        public bool RememberMe { get; set; }

        [Display(Name = "E-Mail")]
        [EmailAddress]
        public string alternateEmail { get; set; }
        [Display(Name = "Benachrichtigt werden, wenn es los geht!")]
        public bool notification { get; set; }
        [Display(Name = "Für die Beta eintragen!")]
        public bool beta { get; set; }

       public UserActionsFrom model { get; set; }
    }
    public class UserActionsFrom
    {
        public bool IsIos { get; set; }

        public bool IsAndroid { get; set; }
        public bool IsWebPage { get; set; }
    }

    public class RegisterViewModel : _BaseViewModel
	{
        [Required]
        [Display(Name = "Beta-Schlüssel")]
        public string BetaKey { get; set; }
        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "\"{0}\" muss mindestens {2} Zeichen lang sein.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Kennwort")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Kennwort bestätigen")]
        [Compare("Password", ErrorMessage = "Das Kennwort entspricht nicht dem Bestätigungskennwort.")]
        public string ConfirmPassword { get; set; }
    }

    public class ResetPasswordViewModel : _BaseViewModel
    {
        [EmailAddress]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }
        [Required]
        public string UserId { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "\"{0}\" muss mindestens {2} Zeichen lang sein.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Kennwort")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Kennwort bestätigen")]
        [Compare("Password", ErrorMessage = "Das Kennwort stimmt nicht mit dem Bestätigungskennwort überein.")]
        public string ConfirmPassword { get; set; }

        public string Code { get; set; }
    }

    public class ForgotPasswordViewModel : _BaseViewModel
	{
        [Required]
        [EmailAddress]
        [Display(Name = "E-Mail")]
        public string Email { get; set; }
    }
   public class UserProfilImgVM 
    {
        public UserProfilImgVM()
        {
            Images = new List<_HtmlImageViewModel>();
        }
        public List<_HtmlImageViewModel> Images { get; set; }
    }
    public enum MarkUserTodoReset
    {
        TodosNotReset = 0,
        TodosReset =1,
        
    }
}

