namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class TodoTemplate : _BaseEntity
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public TodoTemplate()
        {
            ArticleReferences = new HashSet<ArticleReference>();
            SuperCategories = new HashSet<PlantTagSuperCategory>();
        }


        public int ReferenceId { get; set; }

        public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }

        public ModelEnums.TodoPrecisionType Precision { get; set; }

        public ModelEnums.TodoCycleType Cycle { get; set; }



        [Required]
        public string Description { get; set; }

        public DateTime DateStart { get; set; }

        public DateTime DateEnd { get; set; }

        [Required]
        [StringLength(256)]
        public string Title { get; set; }

        public int? TaxonomicTreeId { get; set; }

        public int? TaxonomicReferenceTemplateId { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ArticleReference> ArticleReferences { get; set; }
        public virtual ICollection<PlantTagSuperCategory> SuperCategories { get; set; }
        public bool Edited { get; set; }
    }
}
