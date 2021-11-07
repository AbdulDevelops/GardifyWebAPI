namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ForumHeader: _BaseEntity
    {
        [Required]
        [StringLength(256)]
        public string Title { get; set; }
        [Required]
        public Guid AuthorId { get; set; }
        public int ParentId { get; set; }
        public int RootId { get; set; }
        [Required]
        public int RelatedObjectId { get; set; }
        [Required]
        public ModelEnums.ReferenceToModelClass RelatedObjectType { get; set; }

        public bool IsThread { get; set; }
        public virtual ICollection<ForumPost> RelatedPosts { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public ForumHeader()
        //{
        //    ForumPosts = new HashSet<ForumPost>();
        //}


        //[Required]
        //[StringLength(256)]
        //public string Title { get; set; }

        //public Guid AuthorId { get; set; }

        //public int ParentId { get; set; }

        //public int RootId { get; set; }

        //public int RelatedObjectId { get; set; }

        //[Required]
        //public ModelEnums.ReferenceToModelClass RelatedObjectType { get; set; }

        //public bool IsThread { get; set; }



        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<ForumPost> ForumPosts { get; set; }
    }
}
