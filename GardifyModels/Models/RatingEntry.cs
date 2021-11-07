using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static GardifyModels.Models.ModelEnums;

namespace GardifyModels.Models
{
    public class RatingEntry : _BaseEntity
    {
        public Guid UserId { get; set; }

        [Required]
        public int ObjectId { get; set; }   // FileToModuleID (GardenImage) or GardenId (Garden)

        [Required]
        public RatableObject ObjectType { get; set; }   // GardenImage or Garden

        [Required]
        public Rating Rating { get; set; }

        public string Comment { get; set; }
    }

    public class RatingEntryViewModel
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        [Required]
        public int ObjectId { get; set; }

        [Required]
        public RatableObject ObjectType { get; set; }

        [Required]
        public Rating Rating { get; set; }

        public string Comment { get; set; }
    }
}