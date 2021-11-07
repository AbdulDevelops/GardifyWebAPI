using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static GardifyModels.Models.ModelEnums;

namespace GardifyModels.Models
{
    public class AlbumImageComment: _BaseEntity
    {
        public Guid UserId { get; set; }

        [Required]
        public int ObjectId { get; set; }   // FileToModuleID (GardenImage) or GardenId (Garden)

        [Required]
        public RatableObject ObjectType { get; set; }   // GardenImage or Garden

        [Required]

        public string Comment { get; set; }
    }
}