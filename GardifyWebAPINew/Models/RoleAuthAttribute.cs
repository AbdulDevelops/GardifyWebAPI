using GardifyModels.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;

namespace GardifyWebAPI.Models
{
    // Usage: [RoleAuth(Roles = "Admin,Expert")]
    public class RoleAuthAttribute : System.Web.Http.AuthorizeAttribute
    {
        protected AspNetUserManager _userManager;

        public AspNetUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.Current.GetOwinContext().GetUserManager<AspNetUserManager>();
            }
            set
            {
                _userManager = value;
            }
        }

        protected override void HandleUnauthorizedRequest(System.Web.Http.Controllers.HttpActionContext actionContext)
        {
            var roles = Roles.Split(',');
            var userId = HttpContext.Current.User.Identity.GetUserId();     // null if user is not logged in

            if (string.IsNullOrEmpty(userId) || !roles.Any(role => this.UserManager.IsInRole(userId, role)))
            {
                actionContext.Response = new System.Net.Http.HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            }
        }
    }
}