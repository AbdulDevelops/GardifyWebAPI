using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class Settings
    {
        public int SettingId { get; set; }
        public string Analytics { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeywords { get; set; }
        public string MetaDescription { get; set; }
        public string MetaAuthor { get; set; }
        public string MetaCopyright { get; set; }
        public string MetaPublisher { get; set; }
        public string MetaGeoRegion { get; set; }
        public string MetaGeoPlacename { get; set; }
        public string MetaGeoPosition { get; set; }
        public string MetaIcbm { get; set; }
        public string WrittenBy { get; set; }
        public DateTime? WrittenDate { get; set; }
        public string EditedBy { get; set; }
        public DateTime? EditedDate { get; set; }
    }
}
