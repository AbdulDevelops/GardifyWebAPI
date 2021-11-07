using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class ProductCategory
    {
        public int ProductCategoryId { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public int? EntityId { get; set; }
        public bool? Publish { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? WrittenDate { get; set; }
        public string WrittenBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public string EditedBy { get; set; }
    }
}
