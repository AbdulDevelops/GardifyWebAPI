using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static GardifyModels.Models._CustomValidations;

namespace GardifyModels.Models
{
    public class PropertyViewModels
    {
        public class PropertyCreateViewModel : _BaseViewModel
        {
            [DisplayName("Straße")]
            [Required]
            public string Street { get; set; }
            [DisplayName("Postleitzahl")]
            [Required]
            public string Zip { get; set; }
            [DisplayName("Stadt")]
            [Required]
            public string City { get; set; }
            [DisplayName("Land")]
            [Required]
            public string Country { get; set; }
        }

        public class PropertyEditViewModel : _BaseViewModel
        { 
            public int Id { get; set; }
            [DisplayName("Straße")]
            [Required]
            public string Street { get; set; }
            [DisplayName("Postleitzahl")]
            [Required]
            public string Zip { get; set; }
            [DisplayName("Stadt")]
            [Required]
            public string City { get; set; }
            [DisplayName("Land")]
            [Required]
            public string Country { get; set; }
        }

        public class UpdatePropertyViewModel
        {
            [Required]
            public string Street { get; set; }
            [Required]
            [MinLength(4)]
            public string Zip { get; set; }
            [Required]
            public string City { get; set; }
            [Required]
            [MinLength(2)]
            public string Country { get; set; }
        }

        public class PropertyCoordsVM
        {
            public float Longtitude { get; set; }
            public float Latitude { get; set; }
        }
    }
}