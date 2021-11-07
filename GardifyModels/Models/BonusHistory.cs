namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("BonusHistory")]
    public partial class BonusHistory
    {
        public int Id { get; set; }

        public Guid UserId { get; set; }

        [Required]
        public string Action { get; set; }

        public int Points { get; set; }

        public int PointType { get; set; }
    }
}
