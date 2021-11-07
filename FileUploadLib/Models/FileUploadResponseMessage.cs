using System;
using System.Collections.Generic;
using System.Text;

namespace FileUploadLib.Models
{
    public class FileUploadResponseMessage
    {
        public int ResponseCode { get; set; }
        public string ResponseMessage { get; set; }
        public object ResponseObject { get; set; }
    }
}
