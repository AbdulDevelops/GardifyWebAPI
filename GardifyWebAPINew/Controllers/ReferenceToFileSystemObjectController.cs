using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GardifyWebAPI.Controllers
{
    public class ReferenceToFileSystemObjectController : _BaseController
    {
        nfilesEntities nf = new nfilesEntities();
        #region DB
        [NonAction]
        public HelperClasses.DbResponse DbCreateFileToModule(FileToModule ftm)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();


            var file = (from fs in nf.Files
                        where fs.FileID == ftm.FileID
                        select fs);

            if (file == null || !file.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
                return response;
            }

            nf.FileToModule.Add(ftm);

            bool isOk = nf.SaveChanges() > 0 ? true : false;

            if (isOk)
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Created);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(ftm);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }
        [NonAction]
        public HelperClasses.DbResponse DbCreateReferenceToFileSystemObject(ReferencesToFileSystemObject referenceData)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var fileSystemObject_sel = (from fs in plantDB.FileSystemObject
                                        where fs.Id == referenceData.FileSystemObjectId && !fs.Deleted
                                        select fs);

            if (fileSystemObject_sel == null || !fileSystemObject_sel.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
                return response;
            }

            ReferencesToFileSystemObject newReference = referenceData;
            newReference.OnCreate(referenceData.CreatedBy);

            plantDB.ReferencesToFileSystemObject.Add(newReference);

            bool isOk = plantDB.SaveChanges() > 0 ? true : false;

            if (isOk)
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Created);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(newReference);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetPlantReferencedImages()
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            IEnumerable<FileToModule> references = nf.FileToModule.Where(r => r.ModuleID == (int)ModelEnums.ReferenceToModelClass.Plant);

            if (references != null && references.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Created);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects = references.ToList<object>();
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }

        [NonAction]
        public IEnumerable<FileToModule> DbGetPlantReferencedImagesList()
        {
            IEnumerable<FileToModule> references = nf.FileToModule.Where(r => r.ModuleID == (int)ModelEnums.ReferenceToModelClass.Plant);
            return references;
        }
        [NonAction]
        public HelperClasses.DbResponse DbGetReferencesToFileByObjectId(int objectId, int moduleId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            //IEnumerable<FileToModule> references = (from r in nf.FileToModule
            //                                        where (r.DetailID == objectId || r.FileToModuleID==objectId) && r.ModuleID == moduleId
            //                                        select r);

            IEnumerable<FileToModule> references = (from r in nf.FileToModule
                                                    where (r.DetailID == objectId ) && r.ModuleID == moduleId
                                                    select r);

            if (references != null && references.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Created);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects = references.ToList<object>();
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetReferencesToFileByFileToModuleId(int ftmId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            IEnumerable<FileToModule> references = (from r in nf.FileToModule
                                                    where r.FileToModuleID == ftmId
                                                    select r);

            if (references != null && references.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Created);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects = references.ToList<object>();
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetReferencesToFiles( int moduleId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            IEnumerable<FileToModule> references = (from r in nf.FileToModule
                                                    where  r.ModuleID == moduleId orderby r.InsertedDate descending
                                                    select r).Take(50);

            if (references != null && references.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Created);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects = references.ToList<object>();
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetDiaryEntryReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.DiaryEntry);
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetFaqEntryReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.FaqEntry);
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetNewsEntryReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.NewsEntry);
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetEventReferencedImages(int eventId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(eventId, (int)ModelEnums.ReferenceToModelClass.Event);
        }
        [NonAction]
        public HelperClasses.DbResponse DbGetArticleReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.Article);
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetPlantReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.Plant);
        }
        [NonAction]
        public HelperClasses.DbResponse DbGetPlantTagReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.PlantTag);
        }
        
        public HelperClasses.DbResponse DbGetGardenReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.Garden);
        }

        public HelperClasses.DbResponse DbGetUserPlantReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.UserPlant);
        }

        public HelperClasses.DbResponse DbGetTodoReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.Todo);
        }
        public HelperClasses.DbResponse DbGetQuestionReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.PlantDocQuestion);
        }

        public HelperClasses.DbResponse DbGetCommunityPostReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.CommunityPost);
        }
        public HelperClasses.DbResponse DbGetCommunityAnswerReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.CommunityAnswer);
        }
        public HelperClasses.DbResponse DbGetPlantDocAnswerReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.PlantDocAnswer);
        }
        public HelperClasses.DbResponse DbGetLexiconTermReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.LexiconTerm);
        }
        public HelperClasses.DbResponse DbGetDeviceReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.AdminDevice);
        }
        public HelperClasses.DbResponse DbGetEcoElementReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.EcoElement);
        }
        public HelperClasses.DbResponse DbGetUserDeviceReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.UserDevice);
        }
        public HelperClasses.DbResponse DbGetProfilImgReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.UserProfil);
        }
        public HelperClasses.DbResponse DbGetAlbumImgReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.AlbumImage);
        }

        public HelperClasses.DbResponse DbGetReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByFileToModuleId(entryId);
        }

        public HelperClasses.DbResponse DbGetAllAlbumImgReferencedImages( bool publicOnly = false)
        {
            return DbGetReferencesToFiles((int)ModelEnums.ReferenceToModelClass.AlbumImage);
        }
        [NonAction]
        public bool DbDeleteFile(int fileToModuleId, int moduleId)
        {

            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            var ftm = nf.FileToModule.FirstOrDefault(f => f.FileToModuleID == fileToModuleId);
            if (ftm == null)
            {
                return false;
            }
            var file = nf.Files.FirstOrDefault(f => f.FileID == ftm.FileID);
            if (file == null)
            {
                return false;
            }
            nf.FileToModule.Remove(ftm);

            if (!(file.FileID == 2954 || file.FileID == 2956))
            {
                //Delete file only if its not a placeholder garden image
                var relativePath = "~/" + file.FullRelativePath;
                var fullPath = System.Web.HttpContext.Current.Server.MapPath(relativePath);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Delete(fullPath);
                }
                nf.Files.Remove(file);
            }
            
            return nf.SaveChanges() > 0;
        }

        [NonAction]
        public bool DbDeleteFTM(int fileToModuleId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            FileToModule ftm = nf.FileToModule.FirstOrDefault(f => f.FileToModuleID == fileToModuleId);
            var file = nf.Files.FirstOrDefault(f => f.FileID == ftm.FileID);

            if (ftm == null || file == null)
            {
                return false;
            }

            if (!(file.FileID == 2954 || file.FileID == 2956))
            {
                //mark files as deleted
                var relativePath = "~/" + file.FullRelativePath;
                var fullPath = System.Web.HttpContext.Current.Server.MapPath(relativePath);
                if (System.IO.File.Exists(fullPath))
                {
                    System.IO.File.Move(fullPath, fullPath + ".deleted.jpg");
                }
            }

            nf.FileToModule.Remove(ftm);

            return nf.SaveChanges() > 0;
        }

        public Files DbGetFile(FileToModule ftm)
        {
            var file = (from fs in nf.Files
                        where fs.FileID == ftm.FileID
                        select fs).FirstOrDefault();

            return file;
        }
        public string DbGetImagesTags(int fileID)
        {
            string tags = (from t in plantDB.TagToImages where fileID == t.ImageId select t.Tags).FirstOrDefault();
            return tags == null ? "" : tags;
        }
        public IEnumerable<Files> DbGetFiles(int[] fileToModuleIds)
        {
            var files = nf.Files.Where(f => fileToModuleIds.Any(i => i == f.FileID));
            return files;
        }

        public IEnumerable<Files> DbGetFiles()
        {
            var files = nf.Files;
            return files;
        }

        public IEnumerable<GardenAlbumViewModel> GetImageAlbums(int imageId, Guid userId)
        {
            var albums = (from p in plantDB.GardenAlbums
                          where !p.Deleted && p.UserId == userId
                          join g in plantDB.GardenAlbumFileToModules
                          on p.Id equals g.GardenAlbumId
                          where g.FileToModuleID == imageId
                          select p).Select(ga => new GardenAlbumViewModel() 
                            {
                                Id = ga.Id,
                                Description = ga.Description,
                                Name = ga.Name
                            });
            return albums;
        }

        public bool IsFavoriteImage(int imageId, Guid userId)
        {
            var isFav = plantDB.FavoriteGardenImages.Where(f => f.FileToModuleID == imageId && f.UserId == userId).FirstOrDefault() != null;
            return isFav;
        }

        #endregion
    }
}