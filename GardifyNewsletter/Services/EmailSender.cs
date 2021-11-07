using Castle.Core.Configuration;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Protocols;
using GardifyNewsletter.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace GardifyNewsletter.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string view, string email, string subject, object model, string[] filePaths = null);
        Task SendEmailAsync(string view, string emailTo, string emailFrom, string subject, object model, string[] filePaths = null);
    }

    public class EmailSender : IEmailSender
    {
        //const string EMAIL_API_URL ="http://localhost:44335/api/mails";
        const string EMAIL_API_URL = /*"http://localhost:51307/api/mails";*/"http://nlbeta.de/netzlabmails/api/mails";

        public const string DEFAULT_SENDER = "Jäger Drucklufttechnik GmbH & Co. KG <nv@netzlab.de>";
        private static readonly HttpClient client = new HttpClient();

        private readonly IRazorViewEngine _razorViewEngine;
        private readonly ITempDataProvider _tempDataProvider;
        private readonly IServiceProvider _serviceProvider;
        private readonly IHostingEnvironment _env;
        private readonly ITemplateService _templateService;
        private readonly GardifyNewsletter.Models.ApplicationDbContext _db;

        public EmailSender(IRazorViewEngine razorViewEngine,
            ITempDataProvider tempDataProvider,
            IServiceProvider serviceProvider,
            IHostingEnvironment env,
            ITemplateService templateService,
            GardifyNewsletter.Models.ApplicationDbContext db
          )
        {
            _razorViewEngine = razorViewEngine;
            _tempDataProvider = tempDataProvider;
            _serviceProvider = serviceProvider;
            _env = env;
            _templateService = templateService;
            _db = db;

        }

        public async Task SendEmail(string subject, string to, string viewName, object content, string[] filePaths = null)
        {
            //var view = await _templateService.RenderTemplateAsync("Kontakt/" + viewName, content);
            await SendEmail(subject, content.ToString(), DEFAULT_SENDER, to, filePaths);
        }

        //nlbeta.de/netzlabmails/api/mails
        public async Task<bool> SendEmail(string subject,
            string content,
            string from,
            string to,
            string[] filePaths = null)
        {
            string store = "false";
            //if (_settingsRepository.GetValue<bool>(SettingsRepository.EMAIL_STORE_LOCALLY))
            //{
            store = "true";
            //}
            var values = new Dictionary<string, string>{
               { "from", from },
               { "to", to },
               { "subject", subject },
               { "content", content },
               {"applicationId", "c512f1cb-7b9a-4840-938b-6b4f116bee50" },
               { "storelocally", store }
            };
            return !string.IsNullOrEmpty(await HttpPost(EMAIL_API_URL, values, filePaths));
        }

        private string HttpGet(string url)
        {
            string html = string.Empty;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(url);
            request.AutomaticDecompression = DecompressionMethods.GZip;

            using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                html = reader.ReadToEnd();
            }

            return html;
        }

        private async Task<string> HttpPost(string url, Dictionary<string, string> parameters, string[] files = null)
        {
            var urlEncodedContent = new FormUrlEncodedContent(parameters);

            MultipartFormDataContent form = new MultipartFormDataContent();
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (var parameter in parameters)
            {
                //query[parameter.Key] = parameter.Value;
                form.Add(new StringContent(parameter.Value), parameter.Key);
            }
            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();
            HttpContent content = new StringContent("files");
            form.Add(content, "files");
            List<FileStream> streams = new List<FileStream>();
            if (files != null)
            {
                foreach (var file in files)
                {
                    var stream = new FileStream(file, FileMode.Open);
                    streams.Add(stream);
                    var fileInfo = new FileInfo(file);
                    content = new StreamContent(stream);

                    form.Add(content);
                }
            }

            var response = await client.PostAsync(url, form);
            foreach (var stream in streams)
            {
                stream.Close();
            }
            return await response.Content.ReadAsStringAsync();
        }

        public async Task SendEmailAsync(string view, string email, string subject, object model, string[] filePaths = null)
        {
            //var rendered = await _templateService.RenderTemplateAsync("Kontakt/" + view, model);
            var result = await SendEmail(subject, model.ToString(), DEFAULT_SENDER, email, filePaths);
        }

        public async Task SendEmailAsync(string view, string emailTo, string emailFrom, string subject, object model, string[] filePaths = null)
        {
            //var rendered = await _templateService.RenderTemplateAsync("Kontakt/" + view, model);
            var result = await SendEmail(subject, model.ToString(), emailFrom, emailTo, filePaths);
        }
    }
}
