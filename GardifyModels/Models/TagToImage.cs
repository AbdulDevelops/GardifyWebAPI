using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    /// <summary>
    /// Contains extra details for images, e.g. copyright
    /// </summary>
    public class TagToImage : _BaseEntity
    {
       
        [Required]
        public int ImageId { get; set; }
        public string Tags { get; set; }
        public string Copyright { get; set; }
        //public string AutorField { get; set; }

    }
}