namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class WatchlistEntry : _BaseEntity
    {

        public int PlantId { get; set; }

        public Guid UserId { get; set; }

        public int Count { get; set; }

       

        public virtual Plant Plant { get; set; }
    }
}
