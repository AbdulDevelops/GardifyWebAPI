using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class Projects
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public decimal? SizeOfEnterprise { get; set; }
        public string IndustrialSector { get; set; }
        public string Category { get; set; }
        public bool? Publish { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? EditedDate { get; set; }
        public string EditedBy { get; set; }
        public DateTime? WrittenDate { get; set; }
        public string WrittenBy { get; set; }
    }
}
