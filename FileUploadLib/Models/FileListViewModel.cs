using FileUploadLib.Data;
using FileUploadLib.TagHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FileUploadLib.Models
{
    public class FileListViewModel
    {
        public FileListViewModel() { }
        public FileListViewModel(FileListTagHelper tagHelper)
        {
            ModuleId = tagHelper.ModuleId;
            ModuleName = tagHelper.ModuleName;
            ApplicationId = tagHelper.ApplicationId;
            UsesSort = tagHelper.UsesSort;
            UsesTestEnvironment = tagHelper.UsesTestEnvironment;
        }

        /// <summary>
        /// References the Modules by its name, which should be used to store the file
        /// </summary>
        public int? ModuleId { get; set; }
        /// <summary>
        /// References the Module by id, which should be used to store the file
        /// </summary>
        public string ModuleName { get; set; }
        [Required]
        public Guid ApplicationId { get; set; }
        /// <summary>
        /// Allows the user to set the value of <see cref="FileUploadLib.Data.FileToModule.Sort"/>
        /// </summary>
        public bool UsesSort { get; set; }
        /// <summary>
        /// Control how many items are displayed per page. Default value is 25
        /// </summary>
        public int? ItemsPerPage { get; set; }
        /// <summary>
        /// Sets the value of <see cref="FileUploadLib.Data.File.IsTestFile"/> and <see cref="FileUploadLib.Data.FileToModule.IsTestFile"/>
        /// </summary>
        public bool UsesTestEnvironment { get; set; }
        /// <summary>
        /// Specify filter text
        /// </summary>
        public string FilterBy { get; set; }
        /// <summary>
        /// Specify order item
        /// </summary>
        public string OrderBy { get; set; }
        /// <summary>
        /// Specifies the current page
        /// </summary>
        public int Page { get; set; }
        /// <summary>
        /// Specifies the total count of pages
        /// </summary>
        public int PageCount { get; set; }
        /// <summary>
        /// Specifies the total amount of files (without pageination)
        /// </summary>
        public int FileCount { get; set; }
        public IEnumerable<FileToModule> Files { get; set; }
        public IEnumerable<Module> Modules { get; set; }
    }
}
