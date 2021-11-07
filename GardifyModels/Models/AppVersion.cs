using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class AppVersion : _BaseEntity
    {
        public int VersionId { get; set; }
        public string VersionKey { get; set; }
    }

    //public class AppVersionCheckModel
    //{
    //    public int versionId { get; set; }
    //    public string versionKey { get; set; }
    //}
}