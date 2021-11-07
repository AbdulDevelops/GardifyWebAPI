using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class UserPlantCount: _BaseEntity
    {
        [Required]
        [ForeignKey("Garden")]

        public int Gardenid { get; set; }

        public virtual Garden Garden { get; set; }

        public Guid UserId { get; set; }
        [Required]
        public int Count { get; set; }
    }
}