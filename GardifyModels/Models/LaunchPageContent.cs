using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class LaunchPageContent : _BaseEntity
    {
        public string PageContent { get; set; }
    }

    public class LaunchPageCreateModel
    {
        public int Id { get; set; }
        public string PageContent { get; set; }
        public bool IsSelected { get; set; }

    }

    public class LaunchPageViewModel
    {
        public List<LaunchPageContent> PageContents { get; set; }
    }
}