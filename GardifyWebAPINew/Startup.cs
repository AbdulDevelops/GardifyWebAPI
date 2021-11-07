using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Owin;
using Owin;
using Microsoft.Owin.Cors;
using Microsoft.Extensions.DependencyInjection;
using DinkToPdf.Contracts;
using DinkToPdf;

[assembly: OwinStartup(typeof(GardifyWebAPI.Startup))]

namespace GardifyWebAPI
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.UseCors(Microsoft.Owin.Cors.CorsOptions.AllowAll);
            ConfigureAuth(app);
        }
    }
}
