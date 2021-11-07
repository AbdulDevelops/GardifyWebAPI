using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class CmspageContent2
    {
        public int PageContentId { get; set; }
        public int? PlaceholderId { get; set; }
        public int? PageId { get; set; }
        public string Value { get; set; }
        public string Language { get; set; }
        public bool? Publish { get; set; }
        public bool? Deleted { get; set; }
        public string WrittenBy { get; set; }
        public DateTime? WrittenDate { get; set; }
        public string EditedBy { get; set; }
        public DateTime? EditedDate { get; set; }

        public virtual Cmspages2 Page { get; set; }
    }
}
