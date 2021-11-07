using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace FileUploadLib.Pages.Shared
{
    public class _UploadModel : PageModel
    {
        public string String { get; set; }
        public void OnGet()
        {
            String = "Test";
        }

        public static _UploadModel GetUploadModel()
        {
            _UploadModel model = new _UploadModel
            {
                String = "Test"
            };
            return model;
        }

        public bool OnPost()
        {
            return String == "Test";
        }
    }
}