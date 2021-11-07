namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class DiaryEntry: _BaseEntity
    {
        public Guid UserId { get; set; }

        [Required]
        [StringLength(256)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime Date { get; set; }

        [Required]
        public ModelEnums.ReferenceToModelClass EntryOf { get; set; }

        public int EntryObjectId { get; set; }


        public int? ImageId { get; set; }
    }
}
