using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;
using RazorEngine.Templating;

namespace GardifyWebAPI.Services
{
    public class EmailSender
    {
        const string EMAIL_API_URL = /*"http://localhost:51307/api/mails";*/"http://nlbeta.de/netzlabmails/api/mails";

        public const string DEFAULT_SENDER = "gardify <post@gardify.de>";
        private static readonly HttpClient client = new HttpClient();
        
        private readonly ApplicationDbContext _db;
        private readonly SettingsRepository _settingsRepository;

        public EmailSender(ApplicationDbContext db)
        {
            _db = db;
        }

        //nlbeta.de/netzlabmails/api/mails
        public async Task<bool> SendEmail(string subject,
            string content,
            string from,
            string to,
            string[] filePaths = null)
        {
            if (!String.IsNullOrEmpty(to) && !to.ToLower().Contains("platzhalter") && !to.ToLower().Contains("userdemo"))
            {
                var values = new Dictionary<string, string>{
                   { "from", from },
                   { "to", to },
                   { "subject", subject },
                   { "content", content },
                   { "applicationId", Utilities.GetApplicationId().ToString() },
                   { "storelocally", SettingsRepository.EMAIL_STORE_LOCALLY.ToString() }
                };

                var returnTemp = await HttpPost(EMAIL_API_URL, values, filePaths);

                return returnTemp;
            }
            return false;
        }


        private async Task<bool> HttpPost(string url, Dictionary<string, string> parameters, string[] files = null)
        {
            //var urlEncodedContent = new FormUrlEncodedContent(parameters);

            MultipartFormDataContent form = new MultipartFormDataContent();
            var uriBuilder = new UriBuilder(url);
            var query = HttpUtility.ParseQueryString(uriBuilder.Query);
            foreach (var parameter in parameters)
            {
                form.Add(new StringContent(parameter.Value), parameter.Key);
            }
            uriBuilder.Query = query.ToString();
            url = uriBuilder.ToString();
            HttpContent content = new StringContent("files");
            form.Add(content, "files");
            List<FileStream> streams = new List<FileStream>();
            if (files != null)
            {
                foreach (string file in files)
                {
                    string fullPath;
                    var pathIsAbsolute = file.Contains(@":\");
                    if (pathIsAbsolute)
                    {
                        fullPath = file;
                    }
                    else
                    {
                        var relativePath = "~/" + file;
                        fullPath = System.Web.HttpContext.Current.Server.MapPath(relativePath);
                    }

                    if (System.IO.File.Exists(fullPath))
                    {
                        FileStream stream = new FileStream(fullPath, FileMode.Open);
                        streams.Add(stream);
                        FileInfo fileInfo = new FileInfo(file);
                        content = new StreamContent(stream);
                        content.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                        {
                            Name = "files",
                            FileName = fileInfo.Name
                        };
                        form.Add(content);
                    }
                }
            }

            var response = await client.PostAsync(url, form);
            return response.IsSuccessStatusCode;
        }
    }

    public class SettingsRepository
    {
#if DEBUG
        public const bool EMAIL_STORE_LOCALLY = true;
#else
        public const bool EMAIL_STORE_LOCALLY = false;
#endif

    }
}