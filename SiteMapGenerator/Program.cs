using GardifyWebAPI.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SiteMapGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            var generator = new Generator();
            var content = generator.Generate();
            File.WriteAllBytes("sitemap.xml", content);
        }
    }
}
