using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class Jobs
    {
        public int JobId { get; set; }
        public int? EntityId { get; set; }
        public int? JobBelongsToJobId { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public bool? TopJob { get; set; }
        public bool? Publish { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? WrittenDate { get; set; }
        public string WrittenBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public string EditedBy { get; set; }
    }
}
