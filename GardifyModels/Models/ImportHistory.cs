namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("ImportHistory")]
    public partial class ImportHistory
    {
        public int Id { get; set; }

        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; }

        public DateTime CreatedDate { get; set; }

        [StringLength(256)]
        public string EditedBy { get; set; }

        public DateTime EditedDate { get; set; }

        public bool Deleted { get; set; }

        public int PlantId { get; set; }

        public int SimplePlantId { get; set; }

        public bool Danger { get; set; }

        public string ImportText { get; set; }
    }
}
