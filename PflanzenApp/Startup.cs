using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin;
using Owin;
using GardifyModels.Models;

[assembly: OwinStartupAttribute(typeof(PflanzenApp.Startup))]
namespace PflanzenApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
			//CreateRoles();

		}

		private void CreateRoles()
		{
			ApplicationDbContext context = new ApplicationDbContext();
			var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
 
			if (!roleManager.RoleExists("Admin"))
			{
				var role = new IdentityRole();
				role.Name = "Admin";
				roleManager.Create(role);
			}

			if (!roleManager.RoleExists("Expert"))
			{
				var role = new IdentityRole();
				role.Name = "Expert";
				roleManager.Create(role);
			}
		}
    }
}
