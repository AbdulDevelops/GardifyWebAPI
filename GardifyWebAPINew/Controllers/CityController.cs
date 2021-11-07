using CsvHelper;
using GardifyModels.Models;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace GardifyWebAPI.Controllers
{
    public class CityController : _BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: City
        public ActionResult Index()
        {
            return View();
        }

        public List<City> GetCityDetails()
        {
            ApplicationDbContext appDb = new ApplicationDbContext();
            return appDb.Cities.ToList();
        }

    }
}