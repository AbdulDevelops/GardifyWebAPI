namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BetaKeys : _BaseEntity
    {

        [Required]
        public string Key { get; set; }

        public Guid OwnerId { get; set; }

        public bool Used { get; set; }

        public DateTime? UsedDate { get; set; }

        public bool Notification { get; set; }

        public bool Activated { get; set; }

   
    }
}
