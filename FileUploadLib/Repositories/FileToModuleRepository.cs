using FileUploadLib.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FileUploadLib.Repositories
{
    public class FileToModuleRepository : _BaseRepository<FileToModule, int>
    {
        public FileToModuleRepository(FileLibContext db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
        }

        public IEnumerable<FileToModule> GetFiles(string moduleName, int detailId)
        {
            return GetElements().Where(f => f.DetailId == detailId && f.Module.Name == moduleName);
        }
    }
}
