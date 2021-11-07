using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FileUploadLib.Data
{
    public class FileToModule : _BaseModel
    {
        public int FileToModuleId { get; set; }
        public override int Id => FileToModuleId;
        public int FileId { get; set; }
        public int ModuleId { get; set; }
        public int? DetailId { get; set; }

        public string Description { get; set; }
        public string Title { get; set; }
        public string AltText { get; set; }
        public int? Sort { get; set; }

        public string Tags { get; set; }

        public string LinkTitle { get; set; }
        public string LinkUrl { get; set; }

        [Required]
        public bool IsTestFile { get; set; }

        #region Navigation Properties
        [ForeignKey("FileId")]
        public virtual File File { get; set; }
        [ForeignKey("ModuleId")]
        public virtual Module Module { get; set; }
        #endregion
    }
}
