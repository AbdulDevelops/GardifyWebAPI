using GardifyModels.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace GardifyWebAPI.Services
{
    public class PayoneService
    {
        const string PAYONE_API = "https://api.pay1.de/post-gateway/";

        public async void SendRequest(PayoneRequest req) 
        {
            HttpClient client = new HttpClient();

            var reqJson = JsonConvert.SerializeObject(req);
            string translateJsonRes = "";

            var res = await client.PostAsync(PAYONE_API, new StringContent(reqJson, Encoding.UTF8, "application/json"));
            translateJsonRes = await res.Content.ReadAsStringAsync();
        }

        public void ParseResponse() { }
    }
}