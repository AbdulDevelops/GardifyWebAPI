using FileUploadLib.Controllers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;

namespace FileUploadLib.Data
{
    public class File : _BaseModel
    {
        public int FileId { get; set; }
        public override int Id => FileId;
        [Required]
        public Guid ApplicationId { get; set; }
        [Required]
        public string Path { get; set; }
        /// <summary>
        /// Returns the image url
        /// </summary>
        public string UriPath => (Path ?? "").Replace("\\", "/");
        [Required]
        public string OriginalFileName { get; set; }
        [Required]
        public string NormalizedOriginalFileName { get; set; }
        public Guid Guid { get; set; }
        [Required]
        public bool IsTestFile { get; set; }
        [Required]
        public bool Published { get; set; }
        [Required]
        public bool IsGaleryImage { get; set; }
        [Required]
        public bool Downloadable { get; set; }
        [Required]
        public bool Editable { get; set; }
        [Required]
        public bool Deletable { get; set; }
        public int? ParentFileId { get; set; }
        public int? Width { get; set; }
        public int? Height { get; set; }

        public bool IsImageFile
        {
            get
            {
                return FileController.IsImageFile(Path);
            }
        }

        public bool IsPdfFile
        {
            get
            {
                return FileController.IsPdfFile(Path);
            }
        }

        public bool IsExcelFile
        {
            get
            {
                return FileController.IsExcelFile(Path);
            }
        }

        public bool IsZipFile
        {
            get
            {
                return FileController.IsZipFile(Path);
            }
        }

        public bool IsParentFile
        {
            get
            {
                return ParentFileId == null;
            }
        }
        
        public File GetNearest(int? maxWidth)
        {
            if (Files.Any())
            {
                var files = Files.Where(m => m.Width <= maxWidth).OrderByDescending(m => m.Width);
                var file = files.FirstOrDefault();
                return file;
            }
            else if(Width <= maxWidth)
            {
                return this;
            }
            else
            {
                return null;
            }
        }

        #region Navigation Properties
        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
        public virtual ICollection<FileToModule> FileToModules { get; set; }
        [ForeignKey("ParentFileId")]
        public virtual File FileParent { get; set; }
        public virtual ICollection<File> Files { get; set; }
        #endregion

    }
}
