using FileUploadLib.TagHelpers;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FileUploadLib.Models
{
    public class FileUploadLibViewModel
    {
        public FileUploadLibViewModel()
        {

        }

        public FileUploadLibViewModel(FileUploadFormTagHelper tagHelper)
        {
            ModuleId = tagHelper.ModuleId;
            ModuleName = tagHelper.ModuleName;
            DetailId = tagHelper.DetailId;
            ApplicationId = tagHelper.ApplicationId;
            MultipleFiles = tagHelper.MultipleFiles;
            UsesDescription = tagHelper.UsesDescription;
            UsesTitle = tagHelper.UsesTitle;
            UsesSort = tagHelper.UsesSort;
            UsesAltText = tagHelper.UsesAltText;
            UsesTestEnvironment = tagHelper.UsesTestEnvironment;
            IsDeletable = tagHelper.IsDeletable;
            IsEditable = tagHelper.IsEditable;
            IsPublished = tagHelper.IsPublished;
            IsDownloadable = tagHelper.IsDownloadable;
            CanDecideDownloadable = tagHelper.CanDecideDownloadable;
            CanDecidePublished = tagHelper.CanDecidePublished;
            ReplaceExisting = tagHelper.ReplaceExisting;
        }

        public string ModuleName { get; set; }
        public int? ModuleId { get; set; }
        public int? DetailId { get; set; }
        [Required]
        public Guid ApplicationId { get; set; }

        public IEnumerable<IFormFile> Files { get; set; }

        /// <summary>
        /// Allows the upload of multiple files
        /// </summary>
        public bool MultipleFiles { get; set; }

        /// <summary>
        /// Allows the user to set the value of <see cref="FileUploadLib.Data.FileToModule.Description"/>
        /// </summary>
        public bool UsesDescription { get; set; }
        /// <summary>
        /// Allows the user to set the value of <see cref="FileUploadLib.Data.FileToModule.Title"/>
        /// </summary>
        public bool UsesTitle { get; set; }
        /// <summary>
        /// Allows the user to set the value of <see cref="FileUploadLib.Data.FileToModule.Sort"/>
        /// </summary>
        public bool UsesSort { get; set; }
        /// <summary>
        /// Allows the user to set the value of <see cref="FileUploadLib.Data.FileToModule.AltText"/>
        /// </summary>
        public bool UsesAltText { get; set; }
        /// <summary>
        /// Sets the value of <see cref="FileUploadLib.Data.File.IsTestFile"/> and <see cref="FileUploadLib.Data.FileToModule.IsTestFile"/>
        /// </summary>
        public bool UsesTestEnvironment { get; set; }
        /// <summary>
        /// Sets the value of <see cref="FileUploadLib.Data.FileOption.Deletable"/>
        /// </summary>
        public bool IsDeletable { get; set; }
        /// <summary>
        /// Sets the value of <see cref="FileUploadLib.Data.FileOption.Editable"/>
        /// </summary>
        public bool IsEditable { get; set; }
        /// <summary>
        /// Sets the value of <see cref="FileUploadLib.Data.FileOption.Published"/>
        /// </summary>
        public bool IsPublished { get; set; }
        /// <summary>
        /// Sets the value of <see cref="FileUploadLib.Data.FileOption.Downloadable"/>
        /// </summary>
        public bool IsDownloadable { get; set; }
        /// <summary>
        /// Give the user the opportunity to decide whether the files should be published. If not set, the value of <see cref="IsPublished"/> is used
        /// </summary>
        public bool CanDecidePublished { get; set; }
        /// <summary>
        /// Give the user the opportunity to decide whether the files should be downloadable. If not set, the value of <see cref="IsDownloadable"/> is used
        /// </summary>
        public bool CanDecideDownloadable { get; set; }
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
        public bool ReplaceExisting { get; set; }
    }
}
