namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VideoReference")]
    public partial class VideoReference: _BaseEntity
    {

        public int ReferenceId { get; set; }

        [Required]
        public ModelEnums.ReferenceToModelClass ReferenceTypeId { get; set; }

        [Required]
        public string VideoId { get; set; }

        public int Height { get; set; }

        public int Width { get; set; }

        [Required]
        [StringLength(256)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

    
    }
}
