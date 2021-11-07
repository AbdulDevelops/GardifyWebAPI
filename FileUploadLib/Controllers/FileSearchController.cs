using FileUploadLib.Data;
using FileUploadLib.Models;
using FileUploadLib.Repositories;
using FileUploadLib.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileUploadLib.Controllers
{
    public class FileSearchController : Controller
    {
        public const int ITEMS_PER_PAGE = 25;
        protected FileLibContext db { get; set; }
        protected FileRepository _fileRepository { get; set; }
        protected FileToModuleRepository _fileToModuleRepository { get; set; }
        protected ApplicationRepository _applicationRepository { get; set; }
        protected ModuleRepository _moduleRepository { get; set; }
        protected IHostingEnvironment _hostingEnvironment { get; set; }
        protected IServiceProvider _serviceProvider { get; set; }
        public FileSearchController(IServiceProvider serviceProvider, FileLibContext context, IHttpContextAccessor httpContextAccessor, IHostingEnvironment hostingEnvironment)
        {
            _serviceProvider = serviceProvider;
            db = context;
            _hostingEnvironment = hostingEnvironment;
            _fileRepository = new FileRepository(db, httpContextAccessor);
            _applicationRepository = new ApplicationRepository(db, httpContextAccessor);
            _fileToModuleRepository = new FileToModuleRepository(db, httpContextAccessor);
            _moduleRepository = new ModuleRepository(db, httpContextAccessor);
        }
        [BindProperty]
        public FileListViewModel FileListViewModel { get; set; }

        [HttpPost]
        [Route("api/FileSearch")]
        public FileUploadResponseMessage Post()
        {
            var httpContextAccessor = (IHttpContextAccessor)_serviceProvider.GetService(typeof(IHttpContextAccessor));
            var viewEngine = (IRazorViewEngine)_serviceProvider.GetService(typeof(IRazorViewEngine));
            var tempDataProvider = (ITempDataProvider)_serviceProvider.GetService(typeof(ITempDataProvider));
            var renderer = new ViewRenderService(viewEngine, _serviceProvider, httpContextAccessor, tempDataProvider);
            var viewModel = FileListViewModel;
            var responses = FillViewModel(viewModel).Result;
            if (responses != null && responses.Any(r => r.ResponseCode == -1))
            {
                var renderResult = renderer.Render<IEnumerable<FileUploadResponseMessage>>("Views/Files/Error.cshtml", responses);
                return responses.First();
            }
            else
            {
                var renderResult = renderer.Render<FileListViewModel>("Views/Shared/_List.cshtml", viewModel);
                return new FileUploadResponseMessage { ResponseCode = 1, ResponseObject = renderResult };
            }
        }

        public async Task<IEnumerable<FileUploadResponseMessage>> FillViewModel(FileListViewModel viewModel)
        {
            List<FileUploadResponseMessage> responses = new List<FileUploadResponseMessage>();
            if (viewModel.ApplicationId == null || viewModel.ApplicationId == Guid.Empty)
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Es muss eine ApplicationId angegeben werden." });
                return responses;
            }
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
                if (module == null)
                {
                    responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Die angegebene ModuleId, oder der angegebene ModuleName ist nicht registriert." });
                    return responses;
                }
            }
            else if (!string.IsNullOrEmpty(viewModel.ModuleName))
            {
                module = _moduleRepository.GetElementByName(viewModel.ModuleName, viewModel.ApplicationId);
                if (module == null)
                {
                    responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Die angegebene ModuleId, oder der angegebene ModuleName ist nicht registriert." });
                    return responses;
                }
            }

            viewModel.Modules = _moduleRepository.GetElements().Where(m => m.ApplicationId == viewModel.ApplicationId);

            var fileToModules = _fileToModuleRepository.GetElements();

            //Filter by TestEnvironment
            if (!viewModel.UsesTestEnvironment)
            {
                fileToModules = fileToModules.Where(f => !f.IsTestFile);
            }
            //Filter by given ModuleId
            if (module != null)
            {
                fileToModules = fileToModules.Where(f => f.ModuleId == module.ModuleId);
            }
            //Filter by filterKeyWord
            if (!string.IsNullOrEmpty(viewModel.FilterBy))
            {
                fileToModules = fileToModules.Where(f => f.Title.Contains(viewModel.FilterBy) || f.Description.Contains(viewModel.FilterBy) || f.File.Path.Contains(viewModel.FilterBy));
            }
            //Order elements
            if (!string.IsNullOrEmpty(viewModel.OrderBy))
            {
                switch (viewModel.OrderBy)
                {
                    case "ID":
                        fileToModules = fileToModules.OrderBy(f => f.Id);
                        break;
                    case "Description":
                        fileToModules = fileToModules.OrderBy(f => f.Description);
                        break;
                    case "Path":
                        fileToModules = fileToModules.OrderBy(f => f.File?.Path);
                        break;
                    case "EditedDate":
                        fileToModules = fileToModules.OrderBy(f => f.EditedDate);
                        break;
                }
            }
            if (fileToModules == null || !fileToModules.Any())
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = 0, ResponseMessage = $"Es wurden keine Dateien zu den angegebenen Werten gefunden." });
            }
            if (viewModel.ItemsPerPage == null)
            {
                viewModel.ItemsPerPage = ITEMS_PER_PAGE;
            }
            viewModel.FileCount = fileToModules.Count();
            //Pageination
            viewModel.PageCount = (int)Math.Ceiling((decimal)fileToModules.Count() / ITEMS_PER_PAGE);
            fileToModules = fileToModules.Skip(viewModel.Page * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE);
            viewModel.Files = fileToModules;
            return responses;
        }
    }
}
