using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class Product
    {
        public int ProductId { get; set; }
        public int? ProductCategoryId { get; set; }
        public string ArticleNo { get; set; }
        public string ArticleName { get; set; }
        public string Description { get; set; }
        public decimal? BendingLargerThanMm { get; set; }
        public string Application { get; set; }
        public string Reinforcement { get; set; }
        public decimal? Depth { get; set; }
        public string CornerJoint { get; set; }
        public string Surface { get; set; }
        public string DistanceBetweenIlluminants { get; set; }
        public string MaxSize { get; set; }
        public bool? Publish { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? WrittenDate { get; set; }
        public string WrittenBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public string EditedBy { get; set; }
    }
}
