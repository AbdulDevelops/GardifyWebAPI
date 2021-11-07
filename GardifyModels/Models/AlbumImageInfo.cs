using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class AlbumImageInfo: _BaseEntity
    {
        public Guid UserId { get; set; }

        [Required]
        public int ObjectId { get; set; }   // FileToModuleID (GardenImage) or GardenId (Garden)

        [Required]
        public bool IsOwnImage { get; set; }   // GardenImage or Garden
    }

    public class AlbumImageInfoViewModel 
    {
        public int Id { get; set; }

        public string UserName { get; set; }

        [Required]
        public int ObjectId { get; set; }



        public bool IsOwnImage { get; set; }
    }
}