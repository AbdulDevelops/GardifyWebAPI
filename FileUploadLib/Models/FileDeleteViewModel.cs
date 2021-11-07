using System;

namespace FileUploadLib.Models
{
    public class FileDeleteViewModel
    {
        public string ModuleId { get; set; }
        public int? FileToModuleId { get; set; }
        public Guid ApplicationId { get; set; }
    }
}