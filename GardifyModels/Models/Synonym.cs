namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Synonym")]
    public partial class Synonym: _BaseEntity
    {
        [Required]
        [StringLength(256)]
        public string Text { get; set; }

        public int ReferenceId { get; set; }

        [Required]
        public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }

        [Required]
        Guid UserId { get; set; }
        [Required]
        bool finished { get; set; }

    }
}
