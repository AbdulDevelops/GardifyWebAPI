//using Microsoft.AspNetCore.Http;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.IO;
using System.Net.Http;
using System.Web.Http;
using System.Threading.Tasks;
using System.Web.Routing;
using System.Web;

namespace PflanzenApp.Services
{
    public class TemplateService
    {
        public TemplateService()
        {
        }

        public string RenderTemplateAsync(string templateName, object viewModel)
        {
            string templateFilePath = System.Web.HttpContext.Current.Server.MapPath("~/Views/EmailTemplates/");
            string templatekey = Guid.NewGuid().ToString();
            string template = File.ReadAllText(templateFilePath + templateName + ".txt");
            string result = Engine.Razor.RunCompile(template, templatekey, null, viewModel);
            return System.Web.HttpContext.Current.Server.HtmlDecode(result);
        }
    }
}