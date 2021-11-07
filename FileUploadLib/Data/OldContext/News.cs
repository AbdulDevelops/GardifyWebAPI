using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class News
    {
        public int NewsId { get; set; }
        public int? NewsCategoryId { get; set; }
        public string Language { get; set; }
        public int? EntityId { get; set; }
        public int? MasterNews { get; set; }
        public int? NewsBelongsToNewsId { get; set; }
        public DateTime? NewsDateStart { get; set; }
        public DateTime? NewsDateEnd { get; set; }
        public string NewsDateText { get; set; }
        public string NewsLocation { get; set; }
        public string NewsPath { get; set; }
        public string NewsHeadline { get; set; }
        public string NewsSubline { get; set; }
        public string NewsTeaser { get; set; }
        public string NewsText { get; set; }
        public bool? Publish { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? NewsValidTo { get; set; }
        public DateTime? NewsValidFrom { get; set; }
        public DateTime? WrittenDate { get; set; }
        public string WrittenBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public string EditedBy { get; set; }

        public virtual NewsCategory NewsCategory { get; set; }
    }
}
