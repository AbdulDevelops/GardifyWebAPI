namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Todo")]
    public partial class Todo : _BaseEntity
    {

        public Guid UserId { get; set; }

        public int ReferenceId { get; set; }

        public ModelEnums.NotificationType Notification { get; set; }

        [Required]
        public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }

        [Required]
        public ModelEnums.TodoPrecisionType Precision { get; set; }

        public bool Ignored { get; set; }

        public bool Finished { get; set; }

        public string Notes { get; set; }
        public int? Index { get; set; }

        public int? CyclicId { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        [Required]
        [StringLength(256)]
        public string Title { get; set; }

        public bool PremiumOnly { get; set; }
        public int RelatedTodoTemplateId { get; set; }
    }
}
