using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ElmahCore.Mvc;
using ElmahCore.Sql;
using FileUploadLib.Repositories;
using GardifyNewsletter.Areas.Identity.Data;
using GardifyNewsletter.Models;
using GardifyNewsletter.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Hosting.Internal;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GardifyNewsletter
{
    public class Startup
    {

        //public readonly GardifyNewsletter.Models.ApplicationDbContext _context;

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //_context = context;

        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromHours(1);
                options.Lockout.MaxFailedAccessAttempts = 5;
                options.Lockout.AllowedForNewUsers = true;
            });

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            services.AddElmah<SqlErrorLog>(options =>
            {
                options.ConnectionString = @"Data Source=.\SQL2014;Initial Catalog=Elmah;Integrated Security=SSPI";
                options.CheckPermissionAction = context => context.User.Identity.IsAuthenticated && context.User.IsInRole("Superadmin");
            });

            //services.AddDbContext<ApplicationDbContext>(options =>
            //        options.UseSqlServer(Configuration.GetConnectionString("ApplicationDbContext")));

            string connectionString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseLazyLoadingProxies()
            .UseSqlServer(connectionString));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultUI()
                .AddDefaultTokenProviders();

            //Inject file services
            services.AddDbContext<FileUploadLib.Data.FileLibContext>(options => options.UseLazyLoadingProxies().UseSqlServer(Configuration.GetConnectionString("ImageLibConnection")));
            services.AddTransient(typeof(ApplicationRepository));
            services.AddTransient(typeof(FileRepository));
            services.AddTransient(typeof(FileToModuleRepository));
            services.AddTransient(typeof(ModuleRepository));

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IEmailSender, EmailSender>();
            services.AddHostedService<EmailQueueService>();
            services.AddScoped<ITemplateService, TemplateService>();
            services.AddAuthentication();
            // Adding new service for reading values of Appsettings.json file
            services.AddSingleton<IConfiguration>(Configuration);
            services.AddMvcCore().AddAuthorization();
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2)
                .AddRazorPagesOptions(options =>
                {

                }
            );
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }


            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();

            app.UseMvc();

            app.UseElmah();
        }
    }
}
