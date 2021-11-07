namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("TaxonomicTree")]
    public partial class TaxonomicTree : _BaseEntity
    {

        [Required]
        public int RootID { get; set; }
        [Required]
        public int ParentId { get; set; }
        public int? PlantId { get; set; }
        public ModelEnums.TaxonomicRank Taxon { get; set; }
        [Required]
        public ModelEnums.NodeType Type { get; set; }
        [Required]
        [StringLength(256, ErrorMessage = "Latin name cannot be longer than 256 characters.")]
        public string TitleLatin { get; set; }
        [StringLength(256, ErrorMessage = "German name cannot be longer than 256 characters.")]
        public string TitleGerman { get; set; }
        [NotMapped]
        public List<TaxonomicTree> Childs { get; set; }
        [NotMapped]
        public bool IsParentOfOrSelectedTaxon { get; set; }
        [NotMapped]

        public int? MovingNodeId { get; set; }

    }

    public class TaxonomicTreeMoveConfirmView
    {
        public int movedNodeId { get; set; }
        public int SelectedtaxonId { get; set; }

        public string message { get; set; }
        public bool DisableFeature { get; set; }


    }

    public class TaxonomicRelationTodoItemModel
    {
        public int TemplateId { get; set; }
        public string Title { get; set; }
        public ModelEnums.TodoCycleType cycle { get; set; }
        public string Description { get; set; }
        public int? TaxonomicTreeId { get; set; }
        public int? TaxonomicReferenceTemplateId { get; set; }
        public string nameLatin { get; set; }
        public string nameGerman { get; set; }
        public string nameComplete => nameLatin + " (" + nameGerman + ")";
        public int plantId { get; set; }
        public string titleLatin { get; set; }
        public string titleGerman { get; set; }
        public DateTime DateStart { get; set; }
        public DateTime DateEnd { get; set; }

        public string DateStartMonthString => DateStart.ToString("MM.dd");
        public string DateEndMonthString => DateEnd.ToString("MM.dd");

        public int TaxomId { get; set; }

    }
}
