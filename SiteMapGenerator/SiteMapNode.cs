using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SiteMapGenerator
{
    public class SiteMapNode
    {
        [XmlIgnore]
        private readonly CultureInfo CULTURE_INFO = new CultureInfo("en_US");

        [XmlElement("loc")]
        public string Location { get; set; }

        [XmlIgnore]
        public DateTime? LastModified { get; set; }

        [XmlElement("lastmod")]
        public string LastModifiedXML
        {
            get {
                return LastModified.Value.ToString("yyyy-MM-ddTHH:mm:sszzz");
            }
            set { }
        }

        public bool ShouldSerializeLastModifiedXML()
        {
            return LastModified != null;
        }

        [XmlElement("changefreq")]
        public SiteMapChangeFrequency? ChangeFrequency { get; set; }

        public bool ShouldSerializeChangeFrequency()
        {
            return ChangeFrequency != null;
        }

        [XmlIgnore]
        public decimal? Priority { get; set; }

        [XmlElement("priority")]
        public string XMLPriority
        {
            get
            {
                return String.Format(CULTURE_INFO, "{0:0.0}", Priority);
            }
            set { }
        }

        public bool ShouldSerializeXMLPriority()
        {
            return Priority != null;
        }
    }
}
