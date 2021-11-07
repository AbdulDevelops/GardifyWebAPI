using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class Files2
    {
        public Files2()
        {
            FileToModule = new HashSet<FileToModule>();
        }

        public int FileId { get; set; }
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
        public string FileC { get; set; }
        public string FileD { get; set; }
        public string FileE { get; set; }
        public string FileF { get; set; }
        public string DescriptionDe { get; set; }
        public string TagsDe { get; set; }
        public string DescriptionEn { get; set; }
        public string TagsEn { get; set; }
        public bool? FilePublish { get; set; }
        public bool? PictureGalery { get; set; }
        public bool? DownloadArea { get; set; }
        public DateTime? WrittenDate { get; set; }
        public string WrittenBy { get; set; }
        public DateTime? EditedDate { get; set; }
        public string EditedBy { get; set; }

        public virtual ICollection<FileToModule> FileToModule { get; set; }
    }
}
