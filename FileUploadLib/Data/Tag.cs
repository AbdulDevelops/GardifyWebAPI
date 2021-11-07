using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace FileUploadLib.Data
{
    public class Tag : _BaseModel
    {
        public int TagId { get; set; }
        public override int Id => TagId;

        public int FileToModuleId { get; set; }
        [StringLength(256)]
        public string Name { get; set; }

        #region Navigation Properties
        [ForeignKey("FileToModuleId")]
        public virtual FileToModule FileToModule { get; set; }
        #endregion
    }
}
