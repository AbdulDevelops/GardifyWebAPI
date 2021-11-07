using GardifyModels.Models;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using System.Web.Mvc;

namespace GardifyWebAPI.Controllers
{
    public class CityAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        //Delete entire rows first from table City before running this route again.
        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/CitiesAPI/importcsv")]
        public List<City> ParseCSV()
        {
            List<City> res = new List<City>();

            using (TextFieldParser parser = new TextFieldParser(@"C:\Users\till9\source\repos\GardifyWebAPI\GardifyWebAPINew\App_Data\Germanypostalcodes_New.csv"))
            {
                parser.TextFieldType = FieldType.Delimited;
                parser.SetDelimiters(";");
                parser.ReadFields();    // skip first line
                while (!parser.EndOfData)
                {
                    string[] row = parser.ReadFields();
                    string[] details = row.ElementAt(0).Split(',');
                    if(details.ElementAt(0) != "")
                    {
                        var postCodeString = details.ElementAt(0);
                        if(postCodeString.Length < 5)
                        {
                            postCodeString = "0" + postCodeString;
                        }
                        var city = new City()
                        {
                            CityName = details.ElementAt(1),
                            ZipCode = postCodeString
                        };
                        res.Add(city);
                    }
                }
            }
            db.Cities.AddRange(res);
            db.SaveChanges();
            return res;
        }


        [System.Web.Http.HttpGet]
        [System.Web.Http.Route("api/CitiesAPI/GetCityDetails")]
        public List<City> GetCityDetails()
        {
            CityController cc = new CityController();
            return cc.GetCityDetails();
        }

    }
}