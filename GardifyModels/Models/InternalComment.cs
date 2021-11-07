namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("InternalComment")]
    public partial class InternalComment: _BaseEntity
    {

        [Required]
        public string Text { get; set; }

        public int ReferenceId { get; set; }

        [Required]
        public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }

        public bool Finished { get; set; }

        public Guid UserId { get; set; }
    }
}
