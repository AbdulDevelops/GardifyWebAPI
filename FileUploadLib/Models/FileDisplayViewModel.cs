using FileUploadLib.TagHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace FileUploadLib.Models
{
    public class FileDisplayViewModel
    {
        public FileDisplayViewModel()
        {

        }
        public FileDisplayViewModel(FileDisplayTagHelper tagHelper)
        {
            ModuleId = tagHelper.ModuleId;
            ModuleName = tagHelper.ModuleName;
            DetailId = tagHelper.DetailId;
            MultipleFiles = tagHelper.MultipleFiles;
            UsesDescription = tagHelper.UsesDescription;
            UsesTitle = tagHelper.UsesTitle;
            UsesSort = tagHelper.UsesSort;
            ApplicationId = tagHelper.ApplicationId;
            DisplaySlider = tagHelper.DisplaySlider;
            DisplaySliderIndicators = tagHelper.DisplaySliderIndicators;
            MaxWidth = tagHelper.MaxWidth;
            DisplayAsGrid = tagHelper.DisplayAsGrid;
            GridWidth = tagHelper.GridWidth;
        }

        /// <summary>
        /// References the Modules by its id, which should be used to store the file
        /// </summary>
        public int? ModuleId { get; set; }
        [Required]
        public Guid ApplicationId { get; set; }

        /// <summary>
        /// References the Detail-object by its name, which should be used to store the file
        /// </summary>
        public int? DetailId { get; set; }
        /// <summary>
        /// References the Module by name, which should be used to store the file
        /// </summary>
        public string ModuleName { get; set; }

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
        /// Decides, whether the images should be displayed as a slideshow
        /// </summary>
        public bool DisplaySlider { get; set; }
        /// <summary>
        /// Decides, whether the slideshow should show the indicators
        /// </summary>
        public bool DisplaySliderIndicators { get; set; }
        /// <summary>
        /// Determines the max pixel width of images displayed. The images with a smaller pixel width closest to <see cref="FileDisplayTagHelper.MaxWidth"/> are taken
        /// </summary>
        public int? MaxWidth { get; set; }
        /// <summary>
        /// Displays the files and images in bootstrap col-x fashion. The "x" can be set with <see cref="GridWidth"/>
        /// </summary>
        public bool DisplayAsGrid { get; set; }
        /// <summary>
        /// Sets the width of the bootstrap col's. Value will be: col-GridWidth. (Maximum value: 12), only works with <see cref="DisplayAsGrid"/>
        /// </summary>
        public int? GridWidth { get; set; }

        public IEnumerable<Data.FileToModule> Files { get; set; }
    }
}
