using FileUploadLib.Data;
using FileUploadLib.Models;
using FileUploadLib.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using ImageEditLib.Data;
using System.Xml.XPath;
using System.Xml.Serialization;
using System.Xml.Linq;
using SixLabors.ImageSharp;
using System.Text;
using System.Globalization;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace FileUploadLib.Controllers
{
    public class FileController : Controller
    {
        protected FileLibContext db { get; set; }
        protected FileRepository _fileRepository { get; set; }
        protected FileToModuleRepository _fileToModuleRepository { get; set; }
        protected ApplicationRepository _applicationRepository { get; set; }
        protected ModuleRepository _moduleRepository { get; set; }
        protected IHostingEnvironment _hostingEnvironment { get; set; }
        public FileController(FileLibContext context, IHttpContextAccessor httpContextAccessor, IHostingEnvironment hostingEnvironment)//, FileRepository fileRepository)
        {
            db = context;
            _hostingEnvironment = hostingEnvironment;
            _fileRepository = new FileRepository(db, httpContextAccessor);
            _applicationRepository = new ApplicationRepository(db, httpContextAccessor);
            _fileToModuleRepository = new FileToModuleRepository(db, httpContextAccessor);
            _moduleRepository = new ModuleRepository(db, httpContextAccessor);
            if (RESIZE_WIDTHS.Count == 4 && RESIZE_WIDTHS.Contains(90) && RESIZE_WIDTHS.Contains(250) && RESIZE_WIDTHS.Contains(450) && RESIZE_WIDTHS.Contains(1000))
            {
                var xmlSettings = Path.Combine(GetSystemPath(), "Settings.xml");
                var document = XElement.Load(xmlSettings);
                var settings = document.Descendants();
                var imageSettings = settings.Elements("width").Select(s => int.Parse(s.Value.Trim())).ToList();
                RESIZE_WIDTHS = imageSettings;
            }
        }

        public static List<int> RESIZE_WIDTHS = new List<int> { 90, 250, 450, 1000 };
        public static List<string> IMAGE_EXTENSIONS = new List<string> { ".jpg", ".png", ".gif", ".jfif", ".jpeg" };
        public static List<string> PDF_EXTENSIONS = new List<string> { ".pdf" };
        public static List<string> ZIP_EXTENSIONS = new List<string> { ".zip" };
        public static List<string> EXCEL_EXTENSIONS = new List<string> { ".xlsx", ".csv", ".xlsm", ".xls", ".xlsb", ".xlam", ".xltx", ".xltm", ".xlt" };

        // GET: /<controller>/
        [Route("api/File")]
        public string Get()
        {
            return "Get";
        }

        [HttpPost]
        [Route("api/File")]
        public async Task<IEnumerable<FileUploadResponseMessage>> Post(FileUploadLibViewModel viewModel)
        {
            List<FileUploadResponseMessage> responses = new List<FileUploadResponseMessage>();
            if (viewModel.Files == null || !viewModel.Files.Any())
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "Es muss mindestens eine Datei angegeben werden." });
            }
            if (viewModel.UsesDescription && string.IsNullOrEmpty(viewModel.Description))
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "Es muss eine Beschreibung angegeben werden." });
            }
            if (viewModel.ApplicationId == null)
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
                using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = _fileRepository._db.Database.BeginTransaction())
                {
                    foreach (IFormFile formFile in viewModel.Files)
                    {
                        try
                        {
                            var fullFileName = await SaveFileToDisc(formFile, viewModel.ApplicationId, module);
                            Tuple<int, int> dimensions = null;
                            if (IsImageFile(formFile.FileName))
                            {
                                dimensions = GetDimensions(GetFullFilePath(fullFileName));
                            }
                            Data.File file = new Data.File
                            {
                                ApplicationId = application.Id,
                                IsTestFile = viewModel.UsesTestEnvironment,
                                NormalizedOriginalFileName = formFile.FileName.ToUpper(),
                                OriginalFileName = formFile.FileName,
                                Path = fullFileName,
                                Guid = Guid.NewGuid(),
                                Downloadable = viewModel.IsDownloadable,
                                Editable = viewModel.IsEditable,
                                IsGaleryImage = false,
                                Published = viewModel.IsPublished,
                                Width = dimensions?.Item1,
                                Height = dimensions?.Item2
                            };
                            if (viewModel.ReplaceExisting)
                            {
                                var existingFileRelations = _fileToModuleRepository.GetElements().Where(ftm => ftm.DetailId == viewModel.DetailId && ftm.ModuleId == module.Id);
                                if (existingFileRelations.Any() && !_fileToModuleRepository.DeleteRange(existingFileRelations))
                                {
                                    transaction.Rollback();
                                    throw new Exception("Beim ersetzen der alten Dateien ist ein Fehler aufgetreten.");
                                }
                            }
                            if (!await _fileRepository.AddAsync(file))
                            {
                                transaction.Rollback();
                                throw new Exception("Beim Speichern der Informationen zu Ihrer Datei ist es zu einem Fehler gekommen.");
                            }
                            if (IsImageFile(formFile.FileName))
                            {
                                List<Data.File> addedFiles = new List<Data.File>();
                                foreach (int width in RESIZE_WIDTHS)
                                {
                                    var newGuid = Guid.NewGuid();
                                    var resized = await ImageResize.ResizeAsync(formFile, GetFullFilePath(formFile.FileName, viewModel.ApplicationId, module, newGuid, width), width);
                                    var dimensionsResized = GetDimensions(resized);
                                    Data.File fileResized = new Data.File
                                    {
                                        ApplicationId = application.Id,
                                        IsTestFile = viewModel.UsesTestEnvironment,
                                        NormalizedOriginalFileName = file.OriginalFileName.ToUpper(),
                                        OriginalFileName = file.OriginalFileName,
                                        Path = GetFilePath(formFile.FileName, viewModel.ApplicationId, module, newGuid, width),
                                        Guid = Guid.NewGuid(),
                                        ParentFileId = file.Id,
                                        Downloadable = viewModel.IsDownloadable,
                                        Editable = viewModel.IsEditable,
                                        IsGaleryImage = false,
                                        Published = viewModel.IsPublished,
                                        Width = dimensionsResized.Item1,
                                        Height = dimensionsResized.Item2
                                    };
                                    addedFiles.Add(fileResized);
                                }
                                if (!await _fileRepository.AddRangeAsync(addedFiles))
                                {
                                    transaction.Rollback();
                                    throw new Exception("Beim Speichern der Informationen zu Ihrer Datei ist es zu einem Fehler gekommen.");
                                }
                            }
                            FileToModule fileToModule = new FileToModule
                            {
                                AltText = viewModel.UsesAltText ? viewModel.AltText : null,
                                Description = viewModel.Description,
                                Title = viewModel.Title,
                                FileId = file.Id,
                                IsTestFile = viewModel.UsesTestEnvironment,
                                Sort = viewModel.Sort,
                                ModuleId = module.Id,
                                DetailId = viewModel.DetailId
                            };
                            if (!await _fileToModuleRepository.AddAsync(fileToModule))
                            {
                                transaction.Rollback();
                                throw new Exception("Beim Speichern der Informationen zu Ihrer Datei ist es zu einem Fehler gekommen.");
                            }
                            responses.Add(new FileUploadResponseMessage { ResponseCode = 1, ResponseMessage = $"Die Datei {file.OriginalFileName} mit den zugehörigen Informationen wurde erfolgreich gespeichert.", ResponseObject = "$<img src='~/{ file.File.Path }' class='card-img-top' alt='{file.AltText}' />" });
                            transaction.Commit();
                        }
                        catch (Exception ex)
                        {
                            responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = $"Die Datei {formFile.FileName} konnte nicht gespeichert werden. Weitere Informationen siehe: {ex.Message}" });
                            return responses;
                        }
                    }
                }
            }
            return responses;
        }

        [HttpPut]
        [Route("api/File")]
        public async Task<IEnumerable<FileUploadResponseMessage>> Put(FileEditViewModel viewModel)
        {
            List<FileUploadResponseMessage> responses = new List<FileUploadResponseMessage>();
            if (viewModel.ApplicationId == null)
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Es muss eine ApplicationId angegeben werden." });
            }
            if ((viewModel.ModuleId == null || viewModel.ModuleId < 0) && string.IsNullOrEmpty(viewModel.ModuleName))
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Es muss eine ModuleId, oder ein ModuleName angegeben werden." });
            }
            if (viewModel.FileToModuleId == null)
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Es muss eine FileToModuleId angegeben werden." });
            }
            if (string.IsNullOrEmpty(viewModel.Description))
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "Es muss eine Beschreibung angegeben werden." });
            }
            if (string.IsNullOrEmpty(viewModel.Title))
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "Es muss ein Titel angegeben werden." });
            }
            if (responses.Any())
            {
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
            }
            else if (!string.IsNullOrEmpty(viewModel.ModuleName))
            {
                module = _moduleRepository.GetElementByName(viewModel.ModuleName, viewModel.ApplicationId);
            }
            if (module == null)
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Die angegebene ModuleId, oder der angegebene ModuleName ist nicht registriert." });
            }
            FileToModule fileToModule = await _fileToModuleRepository.GetElementAsync((int)viewModel.FileToModuleId);
            if (fileToModule == null)
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Die angegebene FileToModuleId ist nicht gültig." });
            }
            if (fileToModule.ModuleId != module.ModuleId)
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Die angegebene ModuleId zugehörig zur FileToModuleId ist nicht mit der angegebenen ModuleId gleich." });
            }
            if (responses.Any())
            {
                return responses;
            }
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = _fileRepository._db.Database.BeginTransaction())
            {
                fileToModule.AltText = viewModel.AltText;
                fileToModule.Description = viewModel.Description;
                fileToModule.Sort = viewModel.Sort;
                fileToModule.Title = viewModel.Title;
                if (!await _fileToModuleRepository.UpdateAsync(fileToModule))
                {
                    transaction.Rollback();
                    responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "Beim Ändern der Dateiinformationen ist ein Fehler aufgetreten." });
                    return responses;
                }
                responses.Add(new FileUploadResponseMessage { ResponseCode = 1, ResponseMessage = $"Die Dateiinformationen wurden erfolgreich gespeichert." });
                if (viewModel.File != null)
                {
                    var oldFile = fileToModule.File;
                    List<string> fullFilePaths = new List<string>();
                    foreach (Data.File referencedFile in oldFile.Files)
                    {
                        fullFilePaths.Add(GetFullFilePath(referencedFile.Path));
                        if (!await _fileRepository.DeleteAsync(referencedFile))
                        {
                            transaction.Rollback();
                            responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "Beim Löschen eines automatisch erstellten Bildes ist ein Fehler aufgetreten." });
                            return responses;
                        }
                    }
                    if (!await _fileRepository.DeleteAsync(oldFile))
                    {
                        transaction.Rollback();
                        responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "Beim Löschen einer alten Datei ist ein Fehler aufgetreten." });
                        return responses;
                    }
                    fullFilePaths.Add(GetFullFilePath(oldFile.Path));
                    foreach (var filePath in fullFilePaths)
                    {
                        if (!MoveFile(filePath, filePath + ".edited"))
                        {
                            transaction.Rollback();
                            responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "Beim Löschen einer Datei ist ein Fehler aufgetreten." });
                            return responses;
                        }
                    }
                    responses.Add(new FileUploadResponseMessage { ResponseCode = 1, ResponseMessage = "Die alten Dateien wurden erfolgreich als 'überschrieben' markiert." });
                    try
                    {
                        var fullFileName = await SaveFileToDisc(viewModel.File, viewModel.ApplicationId, module);
                        if (string.IsNullOrEmpty(fullFileName))
                        {
                            transaction.Rollback();
                            responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "Beim Speichern der neuen Datei ist ein Fehler aufgetreten." });
                            return responses;
                        }
                        Tuple<int, int> dimensions = null;
                        if (IsImageFile(fullFileName))
                        {
                            dimensions = GetDimensions(GetFullFilePath(fullFileName));
                        }
                        responses.Add(new FileUploadResponseMessage { ResponseCode = 1, ResponseMessage = "Die neue Datei wurde erfolgreich gespeichert." });
                        Data.File file = new Data.File
                        {
                            ApplicationId = application.Id,
                            IsTestFile = viewModel.UsesTestEnvironment,
                            NormalizedOriginalFileName = viewModel.File.FileName.ToUpper(),
                            OriginalFileName = viewModel.File.FileName,
                            Path = fullFileName,
                            Guid = Guid.NewGuid(),
                            IsGaleryImage = false,
                            Published = viewModel.IsPublished,
                            Width = dimensions.Item1,
                            Height = dimensions.Item2
                        };
                        if (!await _fileRepository.AddAsync(file))
                        {
                            transaction.Rollback();
                            responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "Beim Speichern der neuen Dateiinformationen ist ein Fehler aufgetreten." });
                            return responses;
                        }
                        if (IsImageFile(fullFileName))
                        {
                            List<Data.File> addedFiles = new List<Data.File>();
                            foreach (int width in RESIZE_WIDTHS)
                            {
                                var newGuid = Guid.NewGuid();
                                var resized = await ImageResize.ResizeAsync(viewModel.File, GetFullFilePath(viewModel.File.FileName, viewModel.ApplicationId, module, newGuid, width), width);
                                var dimensionsResized = GetDimensions(resized);
                                Data.File fileResized = new Data.File
                                {
                                    ApplicationId = application.Id,
                                    IsTestFile = viewModel.UsesTestEnvironment,
                                    NormalizedOriginalFileName = file.OriginalFileName.ToUpper(),
                                    OriginalFileName = file.OriginalFileName,
                                    Path = GetFilePath(viewModel.File.FileName, viewModel.ApplicationId, module, newGuid, width),
                                    Guid = Guid.NewGuid(),
                                    ParentFileId = file.Id,
                                    Width = dimensionsResized.Item1,
                                    Height = dimensionsResized.Item2
                                };
                                addedFiles.Add(fileResized);
                            }
                            if (!await _fileRepository.AddRangeAsync(addedFiles))
                            {
                                transaction.Rollback();
                                throw new Exception("Beim Speichern der automatisch erstellten Dateien ist es zu einem Fehler gekommen.");
                            }
                        }
                        fileToModule.FileId = file.FileId;
                        if (!await _fileToModuleRepository.UpdateAsync(fileToModule))
                        {
                            transaction.Rollback();
                            responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "Beim Speichern der Informationen zu Ihrer Datei ist es zu einem Fehler gekommen." });
                            return responses;
                        }
                        transaction.Commit();
                        responses.Add(new FileUploadResponseMessage { ResponseCode = 1, ResponseMessage = $"Die Datei {viewModel.File.FileName} wurde erfolgreich gespeichert." });
                        return responses;
                    }
                    catch (Exception e)
                    {
                        responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = $"Die Datei {viewModel.File.FileName} konnte nicht gespeichert werden. Weitere Informationen: {e.Message}" });
                        transaction.Rollback();
                        return responses;
                    }
                }
                transaction.Commit();
            }
            return responses;
        }

        [HttpDelete]
        [Route("api/File")]
        public async Task<IEnumerable<FileUploadResponseMessage>> Delete([FromBody] FileDeleteViewModel viewModel)
        {
            List<FileUploadResponseMessage> responses = new List<FileUploadResponseMessage>();
            if (viewModel.ApplicationId == null)
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Es muss eine ApplicationId angegeben werden." });
            }
            if ((viewModel.ModuleId == null) && string.IsNullOrEmpty(viewModel.ModuleId))
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Es muss eine ModuleId, oder ein ModuleName angegeben werden." });
            }
            if (viewModel.FileToModuleId == null)
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Es muss eine FileToModuleId angegeben werden." });
            }
            Module module;
            if (int.TryParse(viewModel.ModuleId, out var moduleId))
            {
                module = _moduleRepository.GetElement(moduleId);
            }
            else
            {
                module = _moduleRepository.GetElementByName(viewModel.ModuleId, viewModel.ApplicationId);
            }
            if (module == null)
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Es muss eine gültige ModuleId angegeben werden." });
            }
            var application = await _applicationRepository.GetElementAsync(viewModel.ApplicationId);
            if (application == null)
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Die angegebene Application ist nicht registriert." });
            }
            var fileToModule = _fileToModuleRepository.GetElement((int)viewModel.FileToModuleId);
            if (fileToModule == null)
            {
                responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "[Entwicklung] Die angegebene FileToModuleId ist nicht gültig." });
            }
            if (responses.Any())
            {
                return responses;
            }
            var file = fileToModule.File;
            using (Microsoft.EntityFrameworkCore.Storage.IDbContextTransaction transaction = _fileRepository._db.Database.BeginTransaction())
            {
                List<string> fullFilePaths = new List<string>();
                foreach (Data.File referencedFile in file.Files)
                {
                    fullFilePaths.Add(GetFullFilePath(referencedFile.Path));
                    if (!await _fileRepository.DeleteAsync(referencedFile))
                    {
                        transaction.Rollback();
                        responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "Beim Löschen eines automatisch erstellten Bildes ist ein Fehler aufgetreten." });
                        return responses;
                    }
                }
                if (!await _fileToModuleRepository.DeleteAsync(fileToModule))
                {
                    transaction.Rollback();
                    responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "Beim Löschen der Zusatzinfomationen der Datei ist ein Fehler aufgetreten." });
                    return responses;
                }
                if (!await _fileRepository.DeleteAsync(file))
                {
                    transaction.Rollback();
                    responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "Beim Löschen der Datei ist ein Fehler aufgetreten." });
                    return responses;
                }
                fullFilePaths.Add(GetFullFilePath(file.Path));
                foreach (var filePath in fullFilePaths)
                {
                    if (!MoveFile(filePath, filePath + ".deleted"))
                    {
                        transaction.Rollback();
                        responses.Add(new FileUploadResponseMessage { ResponseCode = -1, ResponseMessage = "Beim Löschen der Datei ist ein Fehler aufgetreten." });
                        return responses;
                    }
                }
                transaction.Commit();
            }
            responses.Add(new FileUploadResponseMessage { ResponseCode = 1, ResponseMessage = $"Die Datei {file.OriginalFileName} mit den zugehörigen Informationen wurde erfolgreich gelöscht.", });
            return responses;
        }

        [HttpGet]
        [Route("api/File")]
        public ActionResult Download([FromQuery] int? fileToModuleId)
        {
            var emptyFileResult = new NotFoundResult();
            if (fileToModuleId == null)
            {
                return new EmptyResult();
            }
            var fileToModule = _fileToModuleRepository.GetElement((int)fileToModuleId);
            if (fileToModule == null)
            {
                return emptyFileResult;
            }
            if (fileToModule.File == null)
            {
                return emptyFileResult;
            }
            if (!System.IO.File.Exists(GetFullFilePath(fileToModule.File.Path)))
            {
                return emptyFileResult;
            }
            var fileStream = System.IO.File.OpenRead(GetFullFilePath(fileToModule.File.Path));

            return File(fileStream, "application/octet-stream", fileToModule.File.OriginalFileName);
        }

        public static bool MoveFile(string fullFilePath, string newFilePath)
        {
            if (string.IsNullOrEmpty(fullFilePath) || string.IsNullOrEmpty(newFilePath))
            {
                return false;
            }
            if (!System.IO.File.Exists(fullFilePath))
            {
                return false;
            }
            System.IO.File.Move(fullFilePath, newFilePath);
            return true;
        }

        public static bool CopyFile(string fullFilePath, string newFilePath, bool @override = true)
        {
            if (string.IsNullOrEmpty(fullFilePath) || string.IsNullOrEmpty(newFilePath))
            {
                return false;
            }
            if (!System.IO.File.Exists(fullFilePath))
            {
                return false;
            }
            System.IO.File.Copy(fullFilePath, newFilePath, @override);
            return true;
        }

        private async Task<string> SaveFileToDisc(IFormFile file, Guid applicationId, Module module)
        {
            if (file.Length == 0)
            {
                return null;
            }
            var filePath = GetFilePath(file.FileName, applicationId, module, Guid.NewGuid());
            try
            {
                using (var stream = new FileStream(Path.Combine(GetSystemPath(), filePath), FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }
            }
            catch (Exception ex)
            {
                throw (ex);
            }
            return filePath;
        }

        public Tuple<int, int> GetDimensions(string fullFilePath)
        {
            using (var image = Image.Load(fullFilePath))
            {
                return Tuple.Create(image.Width, image.Height);
            }
        }

        public string GetFullFilePath(string fileName)
        {
            return Path.Combine(GetSystemPath(), fileName);
        }

        public string GetFullFilePath(string fileName, Guid applicationId, Module module, Guid newGuid, float? factor = null)
        {
            return Path.Combine(GetSystemPath(), GetFilePath(fileName, applicationId, module, newGuid, factor));
        }

        public string GetFilePath(string fileName, Guid applicationId, Module module, Guid newGuid, float? factor = null)
        {

            // replace german characters to english 
            fileName = RemoveDiacritics(fileName);

            string additionalFactor = factor.ToString();
            if (!string.IsNullOrEmpty(additionalFactor))
            {
                additionalFactor += "px";
            }
            string filePath;
            string folderPath;
            if (factor == null)
            {
                filePath = Path.Combine(GetDocumentPath(applicationId), module.Name, additionalFactor + fileName.Replace(Path.GetExtension(fileName), "") + "_" + newGuid.ToString() + Path.GetExtension(fileName));
                folderPath = Path.Combine(GetSystemPath(), GetDocumentPath(applicationId), module.Name);
            }
            else
            {
                filePath = Path.Combine(GetDocumentPath(applicationId), module.Name, factor.ToString() + "px", additionalFactor + fileName.Replace(Path.GetExtension(fileName), "") + "_" + newGuid.ToString() + Path.GetExtension(fileName));
                folderPath = Path.Combine(GetSystemPath(), GetDocumentPath(applicationId), module.Name, factor.ToString() + "px");
            }
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            return filePath;
        }

        public string GetDocumentPath(Guid applicationId)
        {
            var path = Path.Combine("files", applicationId.ToString());
            return path;
        }

        public string GetSystemPath()
        {
            return _hostingEnvironment.WebRootPath;
        }

        public static bool IsImageFile(string fileName)
        {
            var isImage = false;
            foreach (var extension in IMAGE_EXTENSIONS)
            {
                isImage |= fileName.ToUpper().EndsWith(extension.ToUpper());
            }
            return isImage;
        }

        public static bool IsPdfFile(string fileName)
        {
            var isPdf = false;
            foreach (var extension in PDF_EXTENSIONS)
            {
                isPdf |= fileName.ToUpper().EndsWith(extension.ToUpper());
            }
            return isPdf;
        }

        internal static bool IsZipFile(string fileName)
        {
            var isPdf = false;
            foreach (var extension in ZIP_EXTENSIONS)
            {
                isPdf |= fileName.ToUpper().EndsWith(extension.ToUpper());
            }
            return isPdf;
        }

        public static bool IsExcelFile(string fileName)
        {
            var isExcel = false;
            foreach (var extension in EXCEL_EXTENSIONS)
            {
                isExcel |= fileName.ToUpper().EndsWith(extension.ToUpper());
            }
            return isExcel;
        }

        //to convert german umlaut to english
        static string RemoveDiacritics(string germanText)
        {
            var map = new Dictionary<char, string>() {
              { 'ä', "ae" },
              { 'ö', "oe" },
              { 'ü', "ue" },
              { 'Ä', "Ae" },
              { 'Ö', "Oe" },
              { 'Ü', "Ue" },
              { 'ß', "ss" }
            };

            var res = germanText.Aggregate(
                          new StringBuilder(),
                          (sb, c) => map.TryGetValue(c, out var r) ? sb.Append(r) : sb.Append(c)
                          ).ToString();

            //convert extra diacritics characters

            var normalizedString = res.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }
    }
}
