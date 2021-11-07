using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;

namespace GardifyWebAPI.Controllers
{
    public class FileSystemObjectController : _BaseController
    {

        #region DB
        [NonAction]
        public HelperClasses.DbResponse DbCreateFile(Files newFile)
        {
            nfilesEntities nFiles = new nfilesEntities();
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            //string appAbsolutePath = HttpRuntime.AppDomainAppPath;
            //string absolutePath = appAbsolutePath + newFile.FilePath;

            //DirectoryInfo directory = new DirectoryInfo(absolutePath);

            //if (!directory.Exists)
            //{
            //    response.Messages.Add(ModelEnums.DatabaseMessage.FolderNotExists);
            //    response.Status = ModelEnums.ActionStatus.Error;
            //    return response;
            //}

            //if (!System.IO.File.Exists(absolutePath + newFile.FileA))
            //{
            //    response.Messages.Add(ModelEnums.DatabaseMessage.FileNotExists);
            //    response.Status = ModelEnums.ActionStatus.Error;
            //    return response;
            //}

            nFiles.Files.Add(newFile);

            bool isOk = nFiles.SaveChanges() > 0 ? true : false;

            if (isOk)
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Created);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(newFile);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                response.Status = ModelEnums.ActionStatus.Error;
            }
            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbCreateFileSystemObject(FileSystemObject newObject)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            string appAbsolutePath = HttpRuntime.AppDomainAppPath;
            string absolutePath = appAbsolutePath + newObject.RelativeFilePath;

            DirectoryInfo directory = new DirectoryInfo(absolutePath);

            if (!directory.Exists)
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.FolderNotExists);
                response.Status = ModelEnums.ActionStatus.Error;
                return response;
            }

            if (!System.IO.File.Exists(absolutePath + newObject.FileName))
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.FileNotExists);
                response.Status = ModelEnums.ActionStatus.Error;
                return response;
            }

            newObject.OnCreate(newObject.CreatedBy);

            plantDB.FileSystemObject.Add(newObject);

            bool isOk = plantDB.SaveChanges() > 0 ? true : false;

            if (isOk)
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Created);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(newObject);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                response.Status = ModelEnums.ActionStatus.Error;
            }
            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbGetFileSystemObjectById(int objectId)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var object_sel = (from o in plantDB.FileSystemObject
                              where o.Id == objectId && !o.Deleted
                              select o);

            if (object_sel == null || !object_sel.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
                return response;
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Ok);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(object_sel.FirstOrDefault());
            }

            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbEditFileSystemObject(FileSystemObject newData)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var object_sel = (from o in plantDB.FileSystemObject
                              where o.Id == newData.Id && !o.Deleted
                              select o);

            if (object_sel == null || !object_sel.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;

            }
            else
            {
                FileSystemObject objectToEdit = object_sel.FirstOrDefault();

                objectToEdit.Title = newData.Title;
                objectToEdit.Description = newData.Description;
                objectToEdit.OnEdit(newData.EditedBy);

                plantDB.Entry(objectToEdit).State = EntityState.Modified;

                bool isOk = plantDB.SaveChanges() > 0 ? true : false;

                if (isOk)
                {
                    response.Messages.Add(ModelEnums.DatabaseMessage.Edited);
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
        public HelperClasses.DbResponse DbDeleteFileSystemObject(int objectId, string deletedBy)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            var object_sel = (from o in plantDB.FileSystemObject
                              where o.Id == objectId && !o.Deleted
                              select o);

            if (object_sel == null || !object_sel.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;

            }
            else
            {
                FileSystemObject objectToEdit = object_sel.FirstOrDefault();

                objectToEdit.Deleted = true;
                objectToEdit.OnEdit(deletedBy);

                plantDB.Entry(objectToEdit).State = EntityState.Modified;

                bool isOk = plantDB.SaveChanges() > 0 ? true : false;

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

        #endregion
    }
}