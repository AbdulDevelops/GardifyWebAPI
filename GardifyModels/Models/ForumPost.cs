namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class ForumPost: _BaseEntity
    {

        public int ForumHeaderId { get; set; }

        public Guid AuthorId { get; set; }

        [Required]
        public string Content { get; set; }

        

        public virtual ForumHeader ForumHeader { get; set; }
    }
}
