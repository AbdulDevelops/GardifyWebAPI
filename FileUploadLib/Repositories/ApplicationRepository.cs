using FileUploadLib.Data;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace FileUploadLib.Repositories
{
    public class ApplicationRepository : _BaseRepository<Application, Guid>
    {
        public ApplicationRepository(FileLibContext db, IHttpContextAccessor httpContextAccessor) : base(db, httpContextAccessor)
        {
        }
    }
}
