using FileUploadLib.Controllers;
using FileUploadLib.Data;
using FileUploadLib.Extensions;
using FileUploadLib.Models;
using FileUploadLib.Repositories;
using FileUploadLib.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace FileUploadLib.TagHelpers
{
    public class FileDisplayTagHelper : TagHelper
    {
        protected FileLibContext db { get; set; }
        protected FileRepository _fileRepository { get; set; }
        protected FileToModuleRepository _fileToModuleRepository { get; set; }
        protected ApplicationRepository _applicationRepository { get; set; }
        protected ModuleRepository _moduleRepository { get; set; }
        protected IHostingEnvironment _hostingEnvironment { get; set; }
        protected IServiceProvider _serviceProvider { get; set; }
        public FileDisplayTagHelper(IServiceProvider serviceProvider, FileLibContext context, IHttpContextAccessor httpContextAccessor, IHostingEnvironment hostingEnvironment)
        {
            _serviceProvider = serviceProvider;
            db = context;
            _hostingEnvironment = hostingEnvironment;
            _fileRepository = new FileRepository(db, httpContextAccessor);
            _applicationRepository = new ApplicationRepository(db, httpContextAccessor);
            _fileToModuleRepository = new FileToModuleRepository(db, httpContextAccessor);
            _moduleRepository = new ModuleRepository(db, httpContextAccessor);
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
        /// Sets the value of <see cref="FileUploadLib.Data.File.IsTestFile"/> and <see cref="FileUploadLib.Data.FileToModule.IsTestFile"/>
        /// </summary>
        public bool UsesTestEnvironment { get; set; }
        /// <summary>
        /// Decides, whether the images should be displayed as a slideshow
        /// </summary>
        public bool DisplaySlider { get; set; }
        /// <summary>
        /// Decides, whether the slideshow should show the indicators
        /// </summary>
        public bool DisplaySliderIndicators { get; set; }
        /// <summary>
        /// Control how many items are displayed
        /// </summary>
        public int? MaxItems { get; set; }
        /// <summary>
        /// Randomizes the items before rendering the view
        /// </summary>
        public bool RandomizeItems { get; set; }
        /// <summary>
        /// Determines the max pixel width of images displayed. The images with a smaller pixel width closest to <see cref="MaxWidth"/> are taken
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
        /// <summary>
        /// Displays a zoom icon to show a bigger version of the image
        /// </summary>
        public bool UsesZoom { get; set; }
        /// <summary>
        /// Sets additional css classes. Will be added to "class:" property of each img-Tag
        /// </summary>
        public string CssClasses { get; set; }
        /// <summary>
        /// Decides, whether Errors should be displayed to the user
        /// </summary>
        public bool DisplayErrors { get; set; } = true;
        [HtmlAttributeNotBound]
        public IEnumerable<FileToModule> Files { get; set; }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var httpContextAccessor = (IHttpContextAccessor)_serviceProvider.GetService(typeof(IHttpContextAccessor));
            var viewEngine = (IRazorViewEngine)_serviceProvider.GetService(typeof(IRazorViewEngine));
            var tempDataProvider = (ITempDataProvider)_serviceProvider.GetService(typeof(ITempDataProvider));
            var renderer = new ViewRenderService(viewEngine, _serviceProvider, httpContextAccessor, tempDataProvider);
            if (MaxWidth == null)
            {
                MaxWidth = int.MaxValue;
            }
            var responses = FillViewModel().Result;

            if (responses != null && responses.Any(r => r.ResponseCode == -1))
            {
                if (DisplayErrors)
                {
                    var renderResult = renderer.Render<IEnumerable<FileUploadResponseMessage>>("Views/Files/Error.cshtml", responses);
                    output.Content.SetHtmlContent(renderResult);
                }
                else
                {
                    output.Content.SetHtmlContent("");
                }
            }
            else
            {
                var renderResult = renderer.Render<FileDisplayTagHelper>("Views/Files/Details.cshtml", this);
                output.Content.SetHtmlContent(renderResult);
            }
        }

        public override void Init(TagHelperContext context)
        {
            base.Init(context);
        }

        public override Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            return base.ProcessAsync(context, output);
        }

        public async Task<IEnumerable<FileUploadResponseMessage>> FillViewModel()
        {
            Files = new List<FileToModule>();
            List<FileUploadResponseMessage> responses = new List<FileUploadResponseMessage>();
            if (ApplicationId == null || ApplicationId == Guid.Empty)
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Es muss eine ApplicationId angegeben werden." });
            }
            if ((ModuleId == null || ModuleId < 0) && string.IsNullOrEmpty(ModuleName))
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Es muss eine ModuleId, oder ein ModuleName angegeben werden." });
            }
            if (responses.Any())
            {
                return responses;
            }
            var application = await _applicationRepository.GetElementAsync(ApplicationId);
            if (application == null)
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Die angegebene Application ist nicht registriert." });
                return responses;
            }
            Module module = null;
            if (ModuleId != null && ModuleId >= 0)
            {
                module = await _moduleRepository.GetElementAsync((int)ModuleId);
            }
            else if (!string.IsNullOrEmpty(ModuleName))
            {
                module = _moduleRepository.GetElementByName(ModuleName, ApplicationId);
            }
            if (module == null)
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Die angegebene ModuleId, oder der angegebene ModuleName ist nicht registriert." });
                return responses;
            }
            try
            {
                List<FileToModule> files = new List<FileToModule>();
                var fileToModules = _fileToModuleRepository.GetElements().Where(f => !f.Deleted && f.ModuleId == module.Id && f.IsTestFile == UsesTestEnvironment && (DetailId == null || (DetailId != null && f.DetailId == DetailId))).ToList();
                if (fileToModules != null && fileToModules.Any())
                {
                    if (RandomizeItems)
                    {
                        fileToModules.Shuffle();
                    }
                    if (MaxItems != null)
                    {
                        fileToModules = fileToModules.Take((int)MaxItems).ToList();
                    }
                    files.AddRange(fileToModules);
                    if (UsesSort)
                    {
                        files = files.OrderBy(f => f.Sort).ToList();
                    }
                    Files = files;
                }
                else
                {
                    responses.Add(new FileUploadResponseMessage { ResponseCode = 0, ResponseMessage = $"Es wurden keine Dateien zu den angegebenen Werten gefunden." });
                }
            }
            catch (Exception ex)
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = $"Die Dateien konnten nicht angezeigt werden. Weitere Informationen siehe: {ex.Message}" });
                return responses;
            }
            return responses;
        }
    }
}
