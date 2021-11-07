namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class BetaMembers : _BaseEntity
    {

        [Required]
        public string Email { get; set; }

        [StringLength(1024)]
        public string Description { get; set; }

        public DateTime Date { get; set; }

        public bool Beta { get; set; }

        public bool Notification { get; set; }

        public bool Activated { get; set; }

      
    }
}
