using FileUploadLib.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileUploadLib.Repositories
{
    public class FileRepository : _BaseRepository<File, int>
    {
        public FileRepository(FileLibContext db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {

        }
    }
}
