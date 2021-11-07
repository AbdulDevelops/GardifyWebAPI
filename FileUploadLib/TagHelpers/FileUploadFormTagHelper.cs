using FileUploadLib.Models;
using FileUploadLib.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;

namespace FileUploadLib.TagHelpers
{
    //[HtmlTargetElement("file-upload-form", TagStructure = TagStructure.WithoutEndTag)]
    public class FileUploadFormTagHelper : TagHelper
    {
        private IServiceProvider _serviceProvider { get; set; }
        public FileUploadFormTagHelper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        /// <summary>
        /// References the Modules by its name, which should be used to store the file
        /// </summary>
        public int? ModuleId { get; set; }

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
        public bool UsesAltText{ get; set; }
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
        /// Decide whether or not existing files, for the same DetailId, should be overwritten upon upload
        /// </summary>
        public bool ReplaceExisting { get; set; }
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var httpContextAccessor = (IHttpContextAccessor)_serviceProvider.GetService(typeof(IHttpContextAccessor));
            var viewEngine = (IRazorViewEngine)_serviceProvider.GetService(typeof(IRazorViewEngine));
            var tempDataProvider = (ITempDataProvider)_serviceProvider.GetService(typeof(ITempDataProvider));
            var renderer = new ViewRenderService(viewEngine, _serviceProvider, httpContextAccessor, tempDataProvider);

            var renderResult = renderer.Render<FileUploadLibViewModel>("Views/Files/Index.cshtml", new FileUploadLibViewModel(this));
            output.Content.SetHtmlContent(renderResult);
        }

        public override void Init(TagHelperContext context)
        {
            base.Init(context);
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            return base.ProcessAsync(context, output);
        }
    }
}
