using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class FileToModule
    {
        public int FileToModuleId { get; set; }
        public int ModuleId { get; set; }
        public int? FileId { get; set; }
        public int? DetailId { get; set; }
        public string Legend { get; set; }
        public string Description { get; set; }
        public string AltText { get; set; }
        public string TextLong { get; set; }
        public string LinkFromPic { get; set; }
        public string LinkTitle { get; set; }
        public string LinkToPic { get; set; }
        public int? Sort { get; set; }
        public int? DestinationWidth { get; set; }
        public int? DestinationWidthDetail { get; set; }
        public int? DestinationWidthHeightModal { get; set; }
        public bool? Editable { get; set; }
        public DateTime? InsertedDate { get; set; }
        public string InsertedBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public string EditedBy { get; set; }

        public virtual Files2 File { get; set; }
    }
}
