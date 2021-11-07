using System.Web.Mvc;

namespace PflanzenApp.App_Code
{
	public class CustomAuthorizeAttribute : AuthorizeAttribute
	{
		protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
		{			
			filterContext.Result = new RedirectResult("~/Account/Login");
		}
	}
}