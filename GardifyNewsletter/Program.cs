using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace GardifyNewsletter
{
    public class Program
    {
        public const int EMAIL_AMOUNT = 500;
        public static string APPLICATION_ID = "677a8863-70f5-47da-9245-e9124bf411f4";
        private static CancellationTokenSource cancelTokenSource = new System.Threading.CancellationTokenSource();


        public static async Task Main(string[] args)
        {
            await CreateWebHostBuilder(args).Build().RunAsync(cancelTokenSource.Token);
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();

        public static void Shutdown()
        {
            cancelTokenSource.Cancel();
        }
    }
}
