namespace GardifyModels.Models
{
    using GardifyModels.Models;
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Error")]
    public partial class Error : _BaseEntity
    {

        public HttpStatusCode? HttpStatusCode { get; set; }

        [Required]
        public string ErrorMessage { get; set; }

        public string Exception { get; set; }
    }
}
