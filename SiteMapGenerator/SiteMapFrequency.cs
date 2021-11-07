using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace SiteMapGenerator
{
    public enum SiteMapChangeFrequency
    {
        [XmlEnum(Name = "always")]
        Always,

        [XmlEnum(Name = "hourly")]
        Hourly,

        [XmlEnum(Name = "daily")]
        Daily,

        [XmlEnum(Name = "weekly")]
        Weekly,

        [XmlEnum(Name = "monthly")]
        Monthly,

        [XmlEnum(Name = "yearly")]
        Yearly,

        [XmlEnum(Name = "never")]
        Never
    }
}
