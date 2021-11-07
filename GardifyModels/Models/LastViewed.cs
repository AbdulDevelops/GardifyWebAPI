namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("LastViewed")]
    public partial class LastViewed: _BaseEntity
    {

        public Guid UserId { get; set; }

        public int PlantId { get; set; }

  

        public virtual Plant Plant { get; set; }
    }
}
