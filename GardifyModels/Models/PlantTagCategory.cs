namespace GardifyModels.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class PlantTagCategory : _BaseEntity, IEquatable<PlantTagCategory>
    {
        public int? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public virtual PlantTagCategory Parent { get; set; }
        [Required]
        [StringLength(256, ErrorMessage = "Title cannot be longer than 256 characters.")]
        public string Title { get; set; }
        [Required]
        [StringLength(256, ErrorMessage = "color cannot be longer than 256 characters.")]
        public string Color { get; set; }
        [JsonIgnore]
        public virtual ICollection<PlantTag> TagsInThisCategory { get; set; }
        [JsonIgnore]
        public virtual ICollection<PlantTagCategory> Childs { get; set; }
        public virtual ICollection<PlantTagSuperCategory> SuperCategories { get; set; }

        [NotMapped]
        public virtual int Count
        {
            get
            {
                return TagsInThisCategory != null ? TagsInThisCategory.Count : 0;
            }
            private set
            {
            }
        }

        public bool Equals(PlantTagCategory other)
        {
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }
        
    }

    public partial class PlantTagCategoryLite /*: _BaseEntity, IEquatable<PlantTagCategory>*/
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int? ParentId { get; set; }
        [ForeignKey("ParentId")]
        public virtual PlantTagCategory Parent { get; set; }
        [Required]
        [StringLength(256, ErrorMessage = "Title cannot be longer than 256 characters.")]
        public string Title { get; set; }
        [Required]
        [StringLength(256, ErrorMessage = "color cannot be longer than 256 characters.")]
        public string Color { get; set; }


        public bool Equals(PlantTagCategory other)
        {
            return other.Id == Id;
        }

        public override int GetHashCode()
        {
            return Id;
        }

    }
}
