namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    /// <summary>
    /// DO NOT USE THIS CLASS
    /// </summary>
    [Table("ReferencesToFileSystemObject")]    
    public partial class ReferencesToFileSystemObject : _BaseEntity
    {
    
        public int ObjectId { get; set; }

        public int ReferencedModelClass { get; set; }

        public int FileSystemObjectId { get; set; }

        public int ReferenceType { get; set; }

        [Required]
        [StringLength(256)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        public int PrivacyLevel { get; set; }

        public virtual FileSystemObject FileSystemObject { get; set; }
    }
}
