//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    public partial class Files
    {
        public System.Guid ApplicationId { get; set; }
        public int FileID { get; set; }
        public string Guid { get; set; }
        public string FilePath { get; set; }
        public string OriginalFileNameA { get; set; }
        public string OriginalFileNameB { get; set; }
        public string OriginalFileNameC { get; set; }
        public string OriginalFileNameD { get; set; }
        public string OriginalFileNameE { get; set; }
        public string OriginalFileNameF { get; set; }
        public string FileA { get; set; }
        public string FileB { get; set; }
        // Image License
        public string FileC { get; set; }
        // Image Author
        public string FileD { get; set; }
        // Image Note
        public string FileE { get; set; }
        public string FileF { get; set; }
        public string DescriptionDE { get; set; }
        public string TagsDE { get; set; }
        public string DescriptionEN { get; set; }
        public string TagsEN { get; set; }
        public bool TestFile { get; set; }
        public bool FilePublish { get; set; }
        public bool PictureGalery { get; set; }
        public bool DownloadArea { get; set; }
        public Nullable<System.DateTime> WrittenDate { get; set; }
        public Nullable<System.DateTime> UserCreatedDate { get; set; }
        public string WrittenBy { get; set; }
        public Nullable<System.DateTime> EditedDate { get; set; }
        public string EditedBy { get; set; }
        public string FullRelativePath
        {
            get
            {
                if (string.IsNullOrEmpty(FilePath) || string.IsNullOrEmpty(FileA))
                {
                    return "";
                }
                if (FileA == "\\")
                {
                    return "nice try";
                }
                return Path.Combine(FilePath, FileA);
            }
        }
        public bool IsMainImg { get; set; }
    }

    public class ImageMetadataViewModel
    {
        public string Description { get; set; }
        public string Tags { get; set; }
        public string Note { get; set; }
        public DateTime? UserCreatedDate { get; set; }
        public string Title { get; set; }
        public int ImageId { get; set; }
    }
}