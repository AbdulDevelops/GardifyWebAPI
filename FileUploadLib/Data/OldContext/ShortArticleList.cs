using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class ShortArticleList
    {
        public int ShortArticleId { get; set; }
        public int? EntityId { get; set; }
        public int? ShortArticleBelongsToShortArticleId { get; set; }
        public string Language { get; set; }
        public string Articlename { get; set; }
        public string Producer { get; set; }
        public string Model { get; set; }
        public string Text { get; set; }
        public decimal? Weight { get; set; }
        public decimal? Width { get; set; }
        public decimal? Length { get; set; }
        public decimal? Depth { get; set; }
        public string Kw { get; set; }
        public string Volume { get; set; }
        public string YearOfManufacture { get; set; }
        public string HoursUsed { get; set; }
        public string OperatingPressure { get; set; }
        public string Condition { get; set; }
        public decimal? Price { get; set; }
        public bool? New { get; set; }
        public bool? Publish { get; set; }
        public bool? Deleted { get; set; }
        public DateTime? WrittenDate { get; set; }
        public string WrittenBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public string EditedBy { get; set; }
        public string Category { get; set; }
    }
}
