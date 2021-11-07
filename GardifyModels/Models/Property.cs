namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Web;
    using System.Text;
    using System.Xml.Linq;

    public partial class Property : _BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Property()
        {
            Gardens = new HashSet<Garden>();
        }
        

        public float Latitude { get; set; }

        public float Longtitude { get; set; }

        [Required]
        [StringLength(256)]
        public string Street { get; set; }

        [Required]
        [StringLength(20)]
        public string Zip { get; set; }

        [Required]
        [StringLength(256)]
        public string City { get; set; }

        [Required]
        [StringLength(256)]
        public string Country { get; set; }

        public Guid UserId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Garden> Gardens { get; set; }

        public void UpdateCoordinates()
        {
            var coords = getGeocoords(Country + " " + Zip + " " + City + " " + Street);
            if (coords == null)
            {
                coords = getGeocoords(Zip + " " + City);
                if (coords == null)
                {
                    coords = getGeocoords(Zip);
                    if (coords == null)
                    {
                        coords = getGeocoords(Country);
                        if (coords == null)
                        {
                            coords = new double[] { 0, 0 };
                        }
                    }
                }
            }
            Latitude = (float)coords[0];
            Longtitude = (float)coords[1];
        }

        public double[] getGeocoords(string address)
        {
            double lat = 0;
            double lng = 0;

            // &key=AIzaSyBnzXTTHI4Q4_iKvdxFWRwHvTeVeGiPsJU
            // Use the Google Geocoding service to get information about the user-entered address


            var url = String.Format("https://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false&region=de&key=AIzaSyBnzXTTHI4Q4_iKvdxFWRwHvTeVeGiPsJU", HttpContext.Current.Server.UrlEncode(address));

            WebRequest request = WebRequest.Create(url);
            WebResponse response = null;

            try
            {
                response = request.GetResponse();
            }
            catch (Exception e)
            {
                //ErrorLog("intern - Unable to get response. Exception: " + e.Message + "");
                return null;
            }

            Stream receiveStream = response.GetResponseStream();
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);
            String responseBody = readStream.ReadToEnd();

            // Load the XML into an XElement object
            XElement results = null;
            try
            {
                results = XElement.Parse(responseBody);
            }
            catch (Exception e)
            {
                //ErrorLog("intern - Exception: " + e.Message + " ResponseBody: " + responseBody);
                return null;
            }

            if (results == null)
            {
                //ErrorLog("intern - Result == NULL" + " ResponseBody: " + responseBody);
                return null;
            }

            // Check the status
            var status = results.Element("status").Value;
            // OVER_QUERY_LIMIT
            if (status != "OK" || status == "ZERO_RESULTS")
            {
                //ErrorLog("intern - Status: " + status + " ResponseBody: " + responseBody);
                return null;
            }

            try
            {
                lat = double.Parse(results.Element("result").Element("geometry").Element("location").Element("lat").Value.ToString(), CultureInfo.InvariantCulture);
                lng = double.Parse(results.Element("result").Element("geometry").Element("location").Element("lng").Value.ToString(), CultureInfo.InvariantCulture);
            }
            catch
            {
                //ErrorLog("intern - Error in tryParse" + " ResponseBody: " + responseBody);
                return null;
            }

            return new double[] { lat, lng };
        }
    }
}
