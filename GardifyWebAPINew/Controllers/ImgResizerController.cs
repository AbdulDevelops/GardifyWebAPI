using ImageResizer;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace GardifyWebAPI.Controllers
{
    public class ImgResizerController : ApiController
    {
        // GET: api/ImgResizer
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }

        // GET: api/ImgResizer/5
        public string Get(int id)
        {
            return "value";
        }

        // POST: api/ImgResizer
        [System.Web.Http.HttpPost]
        public IHttpActionResult Upload(string path, HttpPostedFile file, string savedFileName = null)
        {
         //var file = System.Web.HttpContext.Current.Request.Files.Count > 0 ?
         //System.Web.HttpContext.Current.Request.Files[0]: null;
            var small_height= ConfigurationManager.AppSettings["small_height"];
            var small_width = ConfigurationManager.AppSettings["small_width"];
            var med_height = ConfigurationManager.AppSettings["medium_height"];
            var med_width = ConfigurationManager.AppSettings["medium_width"];
            var large_height = ConfigurationManager.AppSettings["large_height"];
            var large_width = ConfigurationManager.AppSettings["large_width"];
            var format = System.IO.Path.GetExtension(file.FileName);

            if (file != null )
            {
                
                //Declare a new dictionary to store the parameters for the image versions.
                var versions = new Dictionary<string, string>();

              // var path = System.Web.Hosting.HostingEnvironment.MapPath("~/nfiles/GardenImages/");
                //var pathSmallImg= System.Web.Hosting.HostingEnvironment.MapPath("~/Images/small/");
               // var pathMedImg = System.Web.Hosting.HostingEnvironment.MapPath("~/Images/medium/");
               // var pathLargImg = System.Web.Hosting.HostingEnvironment.MapPath("~/Images/large/");
                //Define the versions to generate
                versions.Add("_small", $"maxwidth={small_width}&maxheight={small_height}&format={format}");
                versions.Add("_medium", $"maxwidth={med_width}&maxheight={med_height}&format={format}");
                versions.Add("_large", $"maxwidth={large_width}&maxheight={large_height}&format={format}");
                var fileName = savedFileName == null ? file.FileName : savedFileName;
                //Generate each version
                foreach (var suffix in versions.Keys)
                {
                    
                    
                    //Let the image builder add the correct extension based on the output file type
                    try {
                        file.InputStream.Seek(0, SeekOrigin.Begin);
                        switch (suffix)
                        {
                            case "_small":
                                ImageBuilder.Current.Build(
                                 new ImageJob(
                                     file.InputStream,
                                     path + "ImgSmall/" + Path.GetFileName(fileName),
                                     new Instructions(versions[suffix]),
                                     false,
                                     false)); break;

                            case "_medium":
                                ImageBuilder.Current.Build(
                                 new ImageJob(
                                     file.InputStream,
                                     path + "ImgMed/" + Path.GetFileName(fileName),
                                     new Instructions(versions[suffix]),
                                     false,
                                     false)); break;

                            case "_large":
                                ImageBuilder.Current.Build(
                                    new ImageJob(
                                        file.InputStream,
                                        path + "ImgLarge/" + Path.GetFileName(fileName),
                                        new Instructions(versions[suffix]),
                                        false,
                                        false)); break;
                        }
                    }
                    catch (ImageCorruptedException) { }
                    
                }
            }

            return Ok();
        }

  


        // PUT: api/ImgResizer/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE: api/ImgResizer/5
        public void Delete(int id)
        {
        }
    }
}
