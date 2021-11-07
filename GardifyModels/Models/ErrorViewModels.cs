using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace GardifyModels.Models
{
    public class ErrorViewModels
    {
        public class ErrorDetailsViewModel : _BaseViewModel
        {
            public HttpStatusCode? HttpStatusCode { get; set; }
            public string ErrorMessage { get; set; }
        }
    }
}
