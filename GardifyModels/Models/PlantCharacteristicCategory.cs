namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PlantCharacteristicCategory : _BaseEntity
    {
        [Required]
        [StringLength(256, ErrorMessage = "Title cannot be longer than 256 characters.")]
        public string Title { get; set; }
        [StringLength(256, ErrorMessage = "Image uri cannot be longer than 256 characters.")]
        public string TagImage { get; set; }
        [Required]
        public ModelEnums.CharacteristicValueType CharacteristicValueType { get; set; }
        [NotMapped]
        public int Count { get; set; }
        [NotMapped]
        public string Unit { get; set; }

        public int? PlantTagCategoryId { get; set; }
        [ForeignKey("PlantTagCategoryId")]
        public virtual PlantTagCategory PlantTagCategory { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public PlantCharacteristicCategory()
        //{
        //    PlantCharacteristics = new HashSet<PlantCharacteristic>();
        //}


        //[Required]
        //[StringLength(256)]
        //public string Title { get; set; }

        //[StringLength(256)]
        //public string TagImage { get; set; }

        //public int CharacteristicValueType { get; set; }


        //public int? PlantTagCategoryId { get; set; }

        //public virtual PlantTagCategory PlantTagCategory { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlantCharacteristic> PlantCharacteristics { get; set; }
    }
    
}
