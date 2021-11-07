using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class PayoneGetFile : PayoneRequest
    {
        public string RequestFileReference { get; set; }
        public string RequestFileType { get; set; }
        public string RequestFileFormat { get; set; }
    }
}