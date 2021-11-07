using System;
using System.IO;
using System.Net;

namespace GardifyAdminKeepAlive
{
    class Program
    {
        static void Main(string[] args)
        {
            //var html = HttpGet("https://gardify.de/intern/");

            //var instaRefresh = HttpGet("https://gardifybackend.sslbeta.de/api/NewsEntriesAPI/getInstaPost");


            //var instaRefresh = HttpGet("https://graph.instagram.com/refresh_access_token?grant_type=ig_refresh_token&access_token=IGQVJYcG1pS1FXaUN2QlJldUtuMDF5MG1JQVVvNHM1WVhHeW90M1pPSFB2Skh5R3NpNnBtekJlNWVhRjhkaWp3Q3ZABOHFCN1B4VkVUY0ZACS0U2VklDNk8yTmlTaDhPbXFDbW1DdEFB");


            //var instaPosts = HttpGet("https://gardifybackend.sslbeta.de/api/NewsEntriesAPI/getRefreshInstaPost");

            //var userPlantCount = HttpGet("https://gardifybackend.sslbeta.de/api/UserPlantCountAPI");

            var resetPlantSearchTemp = HttpGet("https://gardifybackend.sslbeta.de/api/plantTagProperty");

        }

        private static string HttpGet(string url)
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
    }
}
