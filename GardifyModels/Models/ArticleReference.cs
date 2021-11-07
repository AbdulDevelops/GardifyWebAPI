namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;
    using static GardifyModels.Models.ModelEnums;

    [Table("ArticleReference")]
    public partial class ArticleReference : _BaseEntity
    {
       

        public int ArticleId { get; set; }

        public int? PlantId { get; set; }

        public int? TodoTemplateId { get; set; }

        public ArticleReferenceType ReferenceType { get; set; }

    

        public virtual Article Article { get; set; }

        public virtual Plant Plant { get; set; }

        public virtual TodoTemplate TodoTemplate { get; set; }
    }
}
