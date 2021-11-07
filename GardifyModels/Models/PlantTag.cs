namespace GardifyModels.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PlantTag : _BaseEntity, IEquatable<PlantTag>
    {
        [Required]
        [ForeignKey("Category")]
        public int CategoryId { get; set; }
        public virtual PlantTagCategory Category { get; set; }
        [Required]
        [StringLength(256, ErrorMessage = "Title cannot be longer than 256 characters.")]
        public string Title { get; set; }
        [StringLength(256, ErrorMessage = "Image uri cannot be longer than 256 characters.")]
        public string TagImage { get; set; }
        [JsonIgnore]
        public virtual ICollection<Plant> PlantsWithThisTag { get; set; }
        public virtual ICollection<PlantTagSuperCategory> SuperCategories { get; set; }
        [NotMapped]
        public bool Selected { get; set; }

        [NotMapped]
        public virtual int Count
        {
            get
            {
                return PlantsWithThisTag != null ? PlantsWithThisTag.Count : 0;
            }
            private set
            {
            }
        }

        public bool Equals(PlantTag other)
        {
            return this.Id == other.Id;
        }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public PlantTag()
        //{
        //    Plants = new HashSet<Plant>();
        //}


        //public int CategoryId { get; set; }

        //[Required]
        //[StringLength(256)]
        //public string Title { get; set; }

        //[StringLength(256)]
        //public string TagImage { get; set; }



        //public virtual PlantTagCategory PlantTagCategory { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Plant> Plants { get; set; }
    }

    public class PlantTagView
    {

        public int id { get; set; }

        public string title { get; set; }
        //public PlantTagCategory Category { get; set; }

        public string tagTitle { get; set; }


    }

    public class PlantTagSearchLite
    {
        public int Id { get; set; }
        public int CategoryId { get; set; }
        public string Title { get; set; }
    }

    public partial class PlantTagLite
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        //[ForeignKey("Category")]
        public int CategoryId { get; set; }
        //public virtual PlantTagCategory Category { get; set; }
        [Required]
        [StringLength(256, ErrorMessage = "Title cannot be longer than 256 characters.")]
        public string Title { get; set; }
        [StringLength(256, ErrorMessage = "Image uri cannot be longer than 256 characters.")]
        public string TagImage { get; set; }
        //public virtual ICollection<Plant> PlantsWithThisTag { get; set; }
        [NotMapped]
        public bool Selected { get; set; }

    }

    public class AppliedFilterVM
    {
        public string pos { get; set; }
        public string t { get; set; }
    }
}
