using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class NewsCategory
    {
        public NewsCategory()
        {
            News = new HashSet<News>();
        }

        public int NewsCategoryId { get; set; }
        public string CategoryName { get; set; }
        public bool? IsEvent { get; set; }
        public bool? Publish { get; set; }

        public virtual ICollection<News> News { get; set; }
    }
}
