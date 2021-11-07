using FileUploadLib.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileUploadLib.Repositories
{
    public class ModuleRepository : _BaseRepository<Module, int>
    {
        public ModuleRepository(FileLibContext db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
        }

        public Module GetElementByName(string name, Guid applicationId)
        {
            return GetElements().Where(e => e.Name == name && e.ApplicationId == applicationId).FirstOrDefault();
        }
    }
}
