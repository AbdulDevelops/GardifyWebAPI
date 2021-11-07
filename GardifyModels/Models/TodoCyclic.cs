namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TodoCyclic")]
    public partial class TodoCyclic: _BaseEntity
    {

        [Required]
        public ModelEnums.TodoPrecisionType Precision { get; set; }
        [Required]
        public ModelEnums.TodoCycleType Cycle { get; set; }

        public Guid UserId { get; set; }

        public int ReferenceId { get; set; }

        [Required]
        public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }
        public bool Ignored { get; set; }

        [Required]
        [StringLength(256)]
        public string Title { get; set; }

        public bool GeneratedFromTemplate { get; set; }
        public int RelatedTodoTemplateId { get; set; }
    }

    public class TodoCyclicVM
    {
        public int Id { get; set; }

        public string Description { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        public string Title { get; set; }

        public bool Ignored { get; set; }

        public bool Finished { get; set; }
    }
}
