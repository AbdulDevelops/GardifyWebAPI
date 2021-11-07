using System;
using System.Collections.Generic;

namespace FileUploadLib.Data.OldContext
{
    public partial class TagToModule
    {
        public int TagToModuleId { get; set; }
        public int? ModuleId { get; set; }
        public int? DetailId { get; set; }
        public int? TagId { get; set; }
    }
}
