using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class _CustomValidations
    {
        public class _Title : StringLengthAttribute
        {
            public _Title() : base(256)
            {
                ErrorMessage = "Der Titel kann nicht länger als 256 Zeichen sein.";
            }
        }
        public class _Description : StringLengthAttribute
        {
            public _Description() : base(4096)
            {
                ErrorMessage = "Die Beschreibung kann nicht länger als 4096 Zeichen sein.";
            }
        }
    }
}