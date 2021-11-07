namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("PointsPending")]
    public partial class PointsPending : _BaseEntity
    {
    
        public int Points { get; set; }

        public Guid UserId { get; set; }

        public bool Approved { get; set; }

        public bool RequiresApproval { get; set; }

        public DateTime? ApprovedDate { get; set; }

        public Guid? ApprovedBy { get; set; }

        [Required]
        public string Origin { get; set; }

      
    }
}
