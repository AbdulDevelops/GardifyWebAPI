using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FileUploadLib.Data
{
    public class Application : _BaseGuidModel
    {
        public Guid ApplicationId { get; set; }
        public override Guid Id => ApplicationId;

        [Required]
        [StringLength(128)]
        public string Name { get; set; }
        [Required]
        [StringLength(128)]
        public string RootPath { get; set; }
        [StringLength(256)]
        public string Description { get; set; }

        #region Navigation Properties
        //public virtual ICollection<FileToModule> FileToModules { get; set; }
        public virtual ICollection<File> Files { get; set; }
        public virtual ICollection<Module> Modules { get; set; }

        #endregion
    }
}
