using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace WarningsScheduler
{
    class Program
    {
        static void Main(string[] args)
        {
            // start logger
            var path = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;
            var directory = Path.GetDirectoryName(path);
            var fullPath = Path.Combine(directory, "log.txt").Substring(6); // trim "file:\"

            StreamWriter outputFile = new StreamWriter(fullPath, true);
            
            var sDate = DateTime.Now;
            outputFile.WriteLine(sDate + ": Started");

            string baseUrl = "https://gardifybackend.sslbeta.de/";
            string localUrl = "https://localhost:44328/";
            string endPoint = "api/WarningAPI/popbyscheduler";
            
            HttpClient client = new HttpClient();
            client.Timeout = TimeSpan.FromHours(12);
            var res = client.GetAsync(baseUrl + endPoint);
            var resp = res.Result.Content.ReadAsStringAsync();

            Console.WriteLine(resp.Result);

            var eDate = DateTime.Now;
            var logRes = ": Finished with status code: " + res.Result.StatusCode + "(" + res.Result.ReasonPhrase + "). ";
            logRes += "Total users notified: " + resp.Result;
            outputFile.WriteLine(eDate + logRes);
            outputFile.WriteLine("------------------------");

            outputFile.Close();
        }
    }
}
