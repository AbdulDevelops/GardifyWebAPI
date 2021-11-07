using FileUploadLib.Controllers;
using FileUploadLib.Data;
using FileUploadLib.Models;
using FileUploadLib.Repositories;
using FileUploadLib.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploadLib.TagHelpers
{
    public class FileListTagHelper : TagHelper
    {
        public const int ITEMS_PER_PAGE = 25;
        protected FileLibContext db { get; set; }
        protected FileRepository _fileRepository { get; set; }
        protected FileToModuleRepository _fileToModuleRepository { get; set; }
        protected ApplicationRepository _applicationRepository { get; set; }
        protected ModuleRepository _moduleRepository { get; set; }
        protected IHostingEnvironment _hostingEnvironment { get; set; }
        protected IServiceProvider _serviceProvider { get; set; }
        protected FileSearchController _fileSearchController { get; set; }
        public FileListTagHelper(IServiceProvider serviceProvider, FileLibContext context, IHttpContextAccessor httpContextAccessor, IHostingEnvironment hostingEnvironment)
        {
            _serviceProvider = serviceProvider;
            db = context;
            _hostingEnvironment = hostingEnvironment;
            _fileRepository = new FileRepository(db, httpContextAccessor);
            _applicationRepository = new ApplicationRepository(db, httpContextAccessor);
            _fileToModuleRepository = new FileToModuleRepository(db, httpContextAccessor);
            _moduleRepository = new ModuleRepository(db, httpContextAccessor);
            _fileSearchController = new FileSearchController(serviceProvider, context, httpContextAccessor, hostingEnvironment);
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

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var httpContextAccessor = (IHttpContextAccessor)_serviceProvider.GetService(typeof(IHttpContextAccessor));
            var viewEngine = (IRazorViewEngine)_serviceProvider.GetService(typeof(IRazorViewEngine));
            var tempDataProvider = (ITempDataProvider)_serviceProvider.GetService(typeof(ITempDataProvider));
            var renderer = new ViewRenderService(viewEngine, _serviceProvider, httpContextAccessor, tempDataProvider);
            var viewModel = new FileListViewModel(this);
            var responses = _fileSearchController.FillViewModel(viewModel).Result;
            
            if (responses != null && responses.Any())
            {
                var renderResult = renderer.Render<IEnumerable<FileUploadResponseMessage>>("Views/Files/Error.cshtml", responses);
                output.Content.SetHtmlContent(renderResult);
            }
            else
            {
                var renderResult = renderer.Render<FileListViewModel>("Views/Files/List.cshtml", viewModel);
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
    }
}
