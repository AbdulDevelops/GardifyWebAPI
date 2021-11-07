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
    public class FileEditTagHelper : TagHelper
    {
        protected FileLibContext db { get; set; }
        protected FileRepository _fileRepository { get; set; }
        protected FileToModuleRepository _fileToModuleRepository { get; set; }
        protected ApplicationRepository _applicationRepository { get; set; }
        protected ModuleRepository _moduleRepository { get; set; }
        protected IHostingEnvironment _hostingEnvironment { get; set; }
        protected IServiceProvider _serviceProvider { get; set; }
        public FileEditTagHelper(IServiceProvider serviceProvider, FileLibContext context, IHttpContextAccessor httpContextAccessor, IHostingEnvironment hostingEnvironment)
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
        /// Sets the value of <see cref="FileUploadLib.Data.File.IsTestFile"/> and <see cref="FileUploadLib.Data.FileToModule.IsTestFile"/>
        /// </summary>
        public bool UsesTestEnvironment { get; set; }


        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            var httpContextAccessor = (IHttpContextAccessor)_serviceProvider.GetService(typeof(IHttpContextAccessor));
            var viewEngine = (IRazorViewEngine)_serviceProvider.GetService(typeof(IRazorViewEngine));
            var tempDataProvider = (ITempDataProvider)_serviceProvider.GetService(typeof(ITempDataProvider));
            var renderer = new ViewRenderService(viewEngine, _serviceProvider, httpContextAccessor, tempDataProvider);
            var viewModel = new FileEditViewModel(this);
            var responses = FillViewModel(viewModel).Result;

            if (responses != null && responses.Any())
            {
                var renderResult = renderer.Render<IEnumerable<FileUploadResponseMessage>>("Views/Files/Error.cshtml", responses);
                output.Content.SetHtmlContent(renderResult);
            }
            else
            {
                var renderResult = renderer.Render<FileEditViewModel>("Views/Files/Edit.cshtml", viewModel);
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


        public async Task<IEnumerable<FileUploadResponseMessage>> FillViewModel(FileEditViewModel viewModel)
        {
            List<FileUploadResponseMessage> responses = new List<FileUploadResponseMessage>();
            if (viewModel.ApplicationId == null || viewModel.ApplicationId == Guid.Empty)
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Es muss eine ApplicationId angegeben werden." });
            }
            if ((viewModel.ModuleId == null || viewModel.ModuleId < 0) && string.IsNullOrEmpty(viewModel.ModuleName))
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Es muss eine ModuleId, oder ein ModuleName angegeben werden." });
            }
            if (!responses.Any())
            {
                var application = await _applicationRepository.GetElementAsync(viewModel.ApplicationId);
                if (application == null)
                {
                    responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Die angegebene Application ist nicht registriert." });
                    return responses;
                }
                Module module = null;
                if (viewModel.ModuleId != null && viewModel.ModuleId >= 0)
                {
                    module = await _moduleRepository.GetElementAsync((int)viewModel.ModuleId);
                }
                else if (!string.IsNullOrEmpty(viewModel.ModuleName))
                {
                    module = _moduleRepository.GetElementByName(viewModel.ModuleName, viewModel.ApplicationId);
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
                        files.AddRange(fileToModules);
                        viewModel.Files = files;
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
            }
            return responses;
        }
    }
}
