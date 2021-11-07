using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class Tags
    {
        public int TagId { get; set; }
        public string TagName { get; set; }
        public bool? Publish { get; set; }
        public string EditedBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public string WrittenBy { get; set; }
        public DateTime? WrittenDate { get; set; }
    }
}
