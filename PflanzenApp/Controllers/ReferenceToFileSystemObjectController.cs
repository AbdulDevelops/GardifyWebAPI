using GardifyModels.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
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

            var fileSystemObject_sel = (from fs in ctx.FileSystemObject
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

            ctx.ReferencesToFileSystemObject.Add(newReference);

            bool isOk = ctx.SaveChanges() > 0 ? true : false;

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
        public HelperClasses.DbResponse DbGetReferencesToFileByObjectId(int objectId, int moduleId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            IEnumerable<FileToModule> references = (from r in nf.FileToModule
                                                    where r.DetailID == objectId && r.ModuleID == moduleId
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
        public HelperClasses.DbResponse DbSetOtherImgAsNotMain(int objectId, int moduleId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            IEnumerable<FileToModule> references = (from r in nf.FileToModule
                                                    where r.DetailID == objectId && r.ModuleID == moduleId
                                                    select r);


            int[] fileID = references.Select(re => re.FileID).ToArray();
            IEnumerable<Files> fileReferences = (from r in nf.Files
                                                 where fileID.Contains(r.FileID)
                                                 select r);
            if (references != null && references.Any())
            {
                foreach(var r in references)
                {
                    r.IsMainImg = false;
                }
                foreach (var r in fileReferences)
                {
                    r.IsMainImg = false;
                }
                nf.SaveChanges();
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
        public HelperClasses.DbResponse DbGetReferencesToFiles(int moduleId, string titelAtr = null)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            IEnumerable<FileToModule> references ;
            if (titelAtr != null)
            {
                references = (from r in nf.FileToModule
                              where r.ModuleID == moduleId && r.AltText.Contains(titelAtr)
                              select r);
            }
            else
            {
                references = (from r in nf.FileToModule
                              where r.ModuleID == moduleId
                              select r);
            }
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
        public HelperClasses.DbResponse DbGetAllReferencesToFile(int moduleId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            IEnumerable<FileToModule> references = (from file in nf.FileToModule where file.ModuleID == moduleId  select file);

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
        public HelperClasses.DbResponse DbGetPlantDocReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.PlantDocQuestion);
        }
        public HelperClasses.DbResponse DbGetPlantDocAnswerReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.PlantDocAnswer);
        }
        [NonAction]
        public HelperClasses.DbResponse DbGetNewsEntryReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.NewsEntry);
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetLexiconTermReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.LexiconTerm);
        }

       [NonAction]
        public HelperClasses.DbResponse DbGetEventReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.Event);
        }
        [NonAction]
        public HelperClasses.DbResponse DbGetArticleReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.Article);
        }
        [NonAction]
        public HelperClasses.DbResponse DbGetDeviceReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.AdminDevice);
        }
        [NonAction]
        public HelperClasses.DbResponse DbGetEcoElementReferencedImages(int entryId, bool publicOnly = false)
        {
            return DbGetReferencesToFileByObjectId(entryId, (int)ModelEnums.ReferenceToModelClass.EcoElement);
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

        [NonAction]
        public HelperClasses.DbResponse DbDeleteFileReference(int referenceId, string deletedBy)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var object_sel = (from o in nf.FileToModule
                              where o.FileID == referenceId
                              select o);

            var file_sel = (from o in nf.Files
                            where o.FileID == referenceId
                            select o);

            if (object_sel == null || !object_sel.Any() || file_sel == null || !file_sel.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.EmptyResult);
                response.Status = ModelEnums.ActionStatus.Warning;
            }
            else
            {
                FileToModule objectToEdit = object_sel.FirstOrDefault();
                Files fileToDelete = file_sel.FirstOrDefault();

                nf.FileToModule.Remove(objectToEdit);
                nf.Files.Remove(fileToDelete);

                bool isOk = nf.SaveChanges() > 0 ? true : false;

                if (isOk)
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.Deleted);
                    response.Status = ModelEnums.ActionStatus.Success;
                    response.ResponseObjects.Add(objectToEdit);
                }
                else
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                    response.Status = ModelEnums.ActionStatus.Error;
                }
            }

            return response;
        }
        [NonAction]
        public HelperClasses.DbResponse DbSetFileAsMain(int referenceId, string deletedBy)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var object_sel = (from o in nf.FileToModule
                              where o.FileID == referenceId
                              select o);

            var file_sel = (from o in nf.Files
                            where o.FileID == referenceId
                            select o);

            if (object_sel == null || !object_sel.Any() || file_sel == null || !file_sel.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.EmptyResult);
                response.Status = ModelEnums.ActionStatus.Warning;
            }
            else
            {
                FileToModule objectToEdit = object_sel.FirstOrDefault();
                Files fileToEdit = file_sel.FirstOrDefault();
                fileToEdit.IsMainImg = true;
                objectToEdit.IsMainImg = true;


                bool isOk = nf.SaveChanges() > 0 ? true : false;

                if (isOk)
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.Deleted);
                    response.Status = ModelEnums.ActionStatus.Success;
                    response.ResponseObjects.Add(objectToEdit);
                }
                else
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                    response.Status = ModelEnums.ActionStatus.Error;
                }
            }

            return response;
        }
        public Files DbGetFile(FileToModule ftm)
        {
            var file = (from fs in nf.Files
                        where fs.FileID == ftm.FileID
                        select fs).FirstOrDefault();

            return file;
        }

        #endregion
    }
}