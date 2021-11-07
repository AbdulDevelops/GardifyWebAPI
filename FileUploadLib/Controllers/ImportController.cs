using FileUploadLib.Data;
using FileUploadLib.Data.OldContext;
using FileUploadLib.Repositories;
using ImageEditLib.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileUploadLib.Controllers
{
    public class ImportController : Controller
    {
        protected FileLibContext db { get; set; }
        protected FileRepository _files { get; set; }
        protected FileToModuleRepository _fileToModules { get; set; }
        protected ApplicationRepository _applications { get; set; }
        protected ModuleRepository _modules { get; set; }
        protected IHostingEnvironment _hostingEnvironment { get; set; }
        protected FileController _fileController { get; set; }
        public ImportController(FileLibContext context, IHttpContextAccessor httpContextAccessor, IHostingEnvironment hostingEnvironment)//, FileRepository fileRepository)
        {
            db = context;
            _hostingEnvironment = hostingEnvironment;
            _files = new FileRepository(db, httpContextAccessor);
            _applications = new ApplicationRepository(db, httpContextAccessor);
            _fileToModules = new FileToModuleRepository(db, httpContextAccessor);
            _modules = new ModuleRepository(db, httpContextAccessor);
            _fileController = new FileController(context, httpContextAccessor, hostingEnvironment);
        }

        [Route("api/import")]
        public async System.Threading.Tasks.Task<ActionResult> ImportAsync()
        {
            JaegerCoreContext jaegerCoreContext = new JaegerCoreContext();
            var applicationId = "894E1DFB-23D8-44FA-A15B-0EE58E9A7E21";
            using (var transaction = _modules._db.Database.BeginTransaction())
            {
                Application app = new Application
                {
                    ApplicationId = new Guid(applicationId),
                    Description = "Jaeger-Drucklufttechnik",
                    Name = "Jaeger",
                    RootPath = "Jaeger"
                };
                if (!_applications.Add(app))
                {
                    transaction.Rollback();
                    return StatusCode(500);
                }
                List<Module> moduleList = new List<Module>();
                foreach (var module in jaegerCoreContext.CmspageTypes)
                {
                    Data.Module newModule = new Data.Module
                    {
                        ApplicationId = new Guid(applicationId),
                        Name = module.CmspageTypeName,
                        OldId = module.CmspageTypeId
                    };
                    moduleList.Add(newModule);
                }
                if (!_modules.AddRange(moduleList))
                {
                    transaction.Rollback();
                    return StatusCode(500);
                }
                List<File> fileList = new List<File>();
                foreach (var file in jaegerCoreContext.Files2)
                {
                    var path = "files\\" + file.FilePath + "\\" + file.FileA;
                    //if (!System.IO.File.Exists(_fileController.GetFullFilePath(path)))
                    //{
                    //    continue;
                    //}
                    File newFile = new File
                    {
                        ApplicationId = new Guid(applicationId),
                        Downloadable = file.DownloadArea ?? false,
                        Deletable = true,
                        Editable = true,
                        Deleted = false,
                        CreatedBy = file.WrittenBy,
                        CreatedDate = file.WrittenDate ?? DateTime.Now,
                        EditedBy = file.EditedBy,
                        EditedDate = file.EditedDate ?? DateTime.Now,
                        Guid = Guid.NewGuid(),
                        IsGaleryImage = file.PictureGalery ?? false,
                        IsTestFile = false,
                        NormalizedOriginalFileName = file.FileA.ToUpper(),
                        OriginalFileName = file.FileA,
                        OldId = file.FileId,
                        Path = path
                    };
                    fileList.Add(newFile);
                }
                if (!_files.AddRange(fileList))
                {
                    transaction.Rollback();
                    return StatusCode(500);
                }
                List<Data.FileToModule> fileToModuleList = new List<Data.FileToModule>();
                foreach (var fileToModule in jaegerCoreContext.FileToModule)
                {
                    var oldModule = moduleList.FirstOrDefault(m => m.OldId == fileToModule.ModuleId);
                    var oldFile = fileList.FirstOrDefault(f => f.OldId == fileToModule.FileId);
                    if(oldFile == null)
                    {
                        continue;
                    }
                    Data.FileToModule newFTM = new Data.FileToModule
                    {
                        AltText = fileToModule.AltText,
                        CreatedBy = fileToModule.InsertedBy,
                        CreatedDate = fileToModule.InsertedDate ?? DateTime.Now,
                        Deleted = false,
                        Description = fileToModule.Description,
                        DetailId = fileToModule.DetailId,
                        EditedBy = fileToModule.EditedBy,
                        EditedDate = fileToModule.EditedDate ?? DateTime.Now,
                        IsTestFile = false,
                        LinkTitle = fileToModule.LinkTitle,
                        LinkUrl = fileToModule.LinkFromPic,
                        ModuleId = oldModule.ModuleId,
                        OldId = fileToModule.FileToModuleId,
                        Sort = fileToModule.Sort,
                        FileId = oldFile.Id
                    };
                    fileToModuleList.Add(newFTM);
                }
                if (!_fileToModules.AddRange(fileToModuleList))
                {
                    transaction.Rollback();
                    return StatusCode(500);
                }
                try
                {
                    foreach (var file in fileToModuleList)
                    {
                        var documentPath = _fileController.GetFullFilePath(file.File.Path);
                        var newFileName = _fileController.GetFilePath(file.File.OriginalFileName, file.Module.ApplicationId, file.Module, Guid.NewGuid());
                        var newFilePath = _fileController.GetFullFilePath(newFileName);
                        if (!FileController.CopyFile(documentPath, newFilePath))
                        {
                            transaction.Rollback();
                            return StatusCode(500);
                        }
                        file.File.Path = newFileName;
                        if (!_files.Update(file.File))
                        {
                            transaction.Rollback();
                            return StatusCode(500);
                        }
                        if (FileController.IsImageFile(newFilePath))
                        {
                            List<Data.File> addedFiles = new List<Data.File>();
                            foreach (int width in FileController.RESIZE_WIDTHS)
                            {
                                var newGuid = Guid.NewGuid();
                                var resized = await ImageResize.ResizeAsync(newFilePath, _fileController.GetFullFilePath(file.File.OriginalFileName, file.Module.ApplicationId, file.Module, newGuid, width), width);
                                var dimensionsResized = _fileController.GetDimensions(resized);
                                Data.File fileResized = new Data.File
                                {
                                    ApplicationId = file.Module.ApplicationId,
                                    IsTestFile = file.File.IsTestFile,
                                    NormalizedOriginalFileName = file.File.OriginalFileName.ToUpper(),
                                    OriginalFileName = file.File.OriginalFileName,
                                    Path = _fileController.GetFilePath(file.File.OriginalFileName, file.Module.ApplicationId, file.Module, newGuid, width),
                                    Guid = Guid.NewGuid(),
                                    ParentFileId = file.File.Id,
                                    Downloadable = file.File.Downloadable,
                                    Editable = file.File.Editable,
                                    IsGaleryImage = false,
                                    Published = file.File.Published,
                                    Width = dimensionsResized.Item1,
                                    Height = dimensionsResized.Item2
                                };
                                addedFiles.Add(fileResized);
                            }
                            if (!await _files.AddRangeAsync(addedFiles))
                            {
                                transaction.Rollback();
                                throw new Exception("Beim Speichern der Informationen zu Ihrer Datei ist es zu einem Fehler gekommen.");
                            }
                        }
                        var dimensions = _fileController.GetDimensions(newFilePath);
                        file.File.Width = dimensions.Item1;
                        file.File.Height = dimensions.Item2;
                        if (!_files.Update(file.File))
                        {
                            transaction.Rollback();
                            return StatusCode(500);
                        }
                    }
                }
                catch (Exception e)
                {
                    transaction.Rollback();
                    return StatusCode(500);
                }
                transaction.Commit();
            }
            return Ok();
        }
    }
}
