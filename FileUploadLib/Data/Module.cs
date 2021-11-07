using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FileUploadLib.Data
{
    public class Module : _BaseModel
    {
        public int ModuleId { get; set; }
        public override int Id => ModuleId;
        [Required]
        public Guid ApplicationId { get; set; }

        [Required]
        [StringLength(64)]
        public string Name { get; set; }

        #region Navigation Properties
        public virtual ICollection<FileToModule> FileToModules { get; set; }
        [ForeignKey("ApplicationId")]
        public virtual Application Application { get; set; }
        #endregion
    }
}
