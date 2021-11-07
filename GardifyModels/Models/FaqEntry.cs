namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FaqEntry : _BaseEntity
    {

        [Required]
        public Guid QuestionAuthorId { get; set; }
        [Required]
        [StringLength(1024)]
        public string QuestionText { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public bool IsOpen { get; set; }
        [Required]
        public bool UserAllowsPublishment { get; set; }
        [Required]
        public bool AdminAllowsPublishment { get; set; }
        public Guid AnswerAuthorId { get; set; }
        [StringLength(1024)]
        public string AnswerText { get; set; }
        [Required]
        public int ReferenceId { get; set; }
        [Required]
        public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }
    }
}
