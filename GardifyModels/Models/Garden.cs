namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Garden")]
    public partial class Garden : _BaseEntity, IReferencedObject
    {

        [Required]
        [ForeignKey("Property")]
        public int PropertyId { get; set; }
        public virtual Property Property { get; set; }

        public int CardinalDirection { get; set; }

        public int ShadowStrength { get; set; }

        public bool Inside { get; set; }

        public int Temperature { get; set; }

        public int Light { get; set; }

        [Required]
        [StringLength(256)]
        public string Name { get; set; }

        public int GroundType { get; set; }

        public int PhType { get; set; }

        public int Wetness { get; set; }

        
        public ModelEnums.PrivacyLevel PrivacyLevel { get; set; }

        [Required]
        public string Description { get; set; }

        public int MainImageId { get; set; }

        [Required]
        public ModelEnums.ReferenceToModelClass ReferenceType
        {
            get
            {
                return ModelEnums.ReferenceToModelClass.Garden;
            }
        }
    }
}
