using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;


namespace SiteMapGenerator
{
    [XmlRoot("urlset", Namespace = "http://www.sitemaps.org/schemas/sitemap/0.9")]
    public class SiteMap
    {
        [XmlElement("url")]
        public List<SiteMapNode> Nodes = new List<SiteMapNode>();

        public void Add(SiteMapNode node)
        {
            Nodes.Add(node);
        }
    }
}
