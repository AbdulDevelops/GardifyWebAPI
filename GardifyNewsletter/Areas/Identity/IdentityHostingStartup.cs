using System;
using GardifyNewsletter.Areas.Identity.Data;
using GardifyNewsletter.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: HostingStartup(typeof(GardifyNewsletter.Areas.Identity.IdentityHostingStartup))]
namespace GardifyNewsletter.Areas.Identity
{
    public class IdentityHostingStartup : IHostingStartup
    {
        public void Configure(IWebHostBuilder builder)
        {

            builder.ConfigureServices((context, services) => {
            });
            //builder.ConfigureServices((context, services) => {
            //    services.AddDbContext<IdentityContext>(options =>
            //        options.UseSqlServer(
            //            context.Configuration.GetConnectionString("DefaultConnection")));

            //    services.AddIdentity<ApplicationUser, IdentityRole>()
            //        .AddEntityFrameworkStores<IdentityContext>();
            //});
        }
    }
}