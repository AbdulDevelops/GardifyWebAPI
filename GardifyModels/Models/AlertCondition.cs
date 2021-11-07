namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("AlertCondition")]
    public partial class AlertCondition: _BaseEntity
    {

        [Required]
        [ForeignKey("Trigger")]
        public int TriggerId { get; set; }
        public virtual AlertTrigger Trigger { get; set; }

        [Required]
        public ModelEnums.ComparisonOperator ComparisonOperator { get; set; }

        [Required]
        public ModelEnums.ComparedValueType ValueType { get; set; }


        public float? FloatValue { get; set; }

        public DateTime? DateValue { get; set; }

        public string ReadableCondition { get; set; }
        
    }
}
