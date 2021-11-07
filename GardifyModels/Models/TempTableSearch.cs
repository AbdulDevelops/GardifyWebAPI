using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class TempTableSearch
    {
        public int Id { get; set; }
        public string SearchQuery { get; set; }
        public string SearchResult { get; set; }
        public DateTime SearchDate { get; set; }
    }
}