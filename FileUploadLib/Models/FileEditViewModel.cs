using FileUploadLib.TagHelpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FileUploadLib.Models
{
    public class FileEditViewModel
    {
        public FileEditViewModel()
        {

        }

        public FileEditViewModel(FileEditTagHelper tagHelper)
        {
            ModuleId = tagHelper.ModuleId;
            ModuleName = tagHelper.ModuleName;
            DetailId = tagHelper.DetailId;
            ApplicationId = tagHelper.ApplicationId;
            UsesTestEnvironment = tagHelper.UsesTestEnvironment;
        }

        /// <summary>
        /// References the Modules by its name, which should be used to store the file
        /// </summary>
        public int? ModuleId { get; set; }
        /// <summary>
        /// References the FileToModule object, which should be edited
        /// </summary>
        public int? FileToModuleId { get; set; }

        /// <summary>
        /// References the Detail-object by its name, which should be used to store the file
        /// </summary>
        public int? DetailId { get; set; }
        /// <summary>
        /// References the Module by id, which should be used to store the file
        /// </summary>
        public string ModuleName { get; set; }
        [Required]
        public Guid ApplicationId { get; set; }
        /// <summary>
        /// Sets the value of <see cref="FileUploadLib.Data.File.IsTestFile"/> and <see cref="FileUploadLib.Data.FileToModule.IsTestFile"/>
        /// </summary>
        public bool UsesTestEnvironment { get; set; }
        /// <summary>
        /// Describes a number to sort by later on
        /// </summary>
        public int Sort { get; set; }
        /// <summary>
        /// Describes the alt text for the file
        /// </summary>
        public string AltText { get; set; }
        /// <summary>
        /// Describes the description for the file
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        /// Describes the title for the file
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Sets the attribute <see cref="FileUploadLib.Data.FileOption.Published"/>
        /// </summary>
        public bool IsPublished { get; set; }
        /// <summary>
        /// The updated file which is uploaded to replace the old file (A new File object is created in the DB)
        /// </summary>
        public IFormFile File { get; set; }
        public IEnumerable<Data.FileToModule> Files { get; set; }
    }
}
