using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class NewsLetter :_BaseEntity
    {

        [Required]
        public string LastName { get; set; }

        [Required]
        public string FirstName { get; set; }

        [Required]
        public string Email { get; set; }

        [Column(TypeName = "datetime2")]
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public Guid UserId { get; set; }
        public bool EmailConfirmed { get; set; }
    }
}