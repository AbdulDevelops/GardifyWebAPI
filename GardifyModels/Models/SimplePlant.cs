using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class SimplePlant
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [Required]
        [StringLength(256)]
        public string CreatedBy { get; set; }
        [Required]
        public DateTime CreatedDate { get; set; }
        [StringLength(256)]
        public string EditedBy { get; set; }
        public DateTime EditedDate { get; set; }
        [Required]
        public bool Deleted { get; set; }
        [StringLength(256)]
        [DisplayName("Name")]
        public string NameGerman { get; set; }
        [Required]
        [StringLength(256)]
        [DisplayName("Botanischer Name")]
        public string NameLatin { get; set; }
        [StringLength(256)]
        [DisplayName("Alternativer Name")]
        public string NameAlternative { get; set; }
        [DisplayName("Wuchshöhe in cm (von)")]
        public int? HeightFrom { get; set; }
        [DisplayName("Wuchshöhe in cm (bis)")]
        public int? HeightTo { get; set; }
        [DisplayName("Wuchsbreite in cm (von)")]
        public int? WidthFrom { get; set; }
        [DisplayName("Wuchsbreite in cm (bis)")]
        public int? WidthTo { get; set; }
        [StringLength(256)]
        [DisplayName("Blütezeit (von)")]
        public string BloomingTimeFrom { get; set; }
        [StringLength(256)]
        [DisplayName("Blütezeit (bis)")]
        public string BloomingTimeTo { get; set; }
        [DisplayName("Vorsicht")]
        public bool Caution { get; set; }
        [DisplayName("Bemerkung")]
        public string Comment { get; set; }
        [DisplayName("Früchte von")]
        public string FruitsTimeFrom { get; set; }
        [DisplayName("Früchte bis")]
        public string FruitsTimeTo { get; set; }

        [DisplayName("Geruch")]
        public bool Scent { get; set; }

        [DisplayName("Schatten")]
        public bool LocationShade { get; set; }
        [DisplayName("Halbschatten")]
        public bool LocationSemiShade { get; set; }
        [DisplayName("Sonne")]
        public bool LocationSun { get; set; }
        [DisplayName("Topfpflanze")]
        public bool PotPlant { get; set; }
        [DisplayName("Schnittblume")]
        public bool CutFlower { get; set; }

        [DisplayName("Geruch")]
        public string Smell { get; set; }
        public ModelEnums.Location Location { get; set; }
        public ModelEnums.FrostResistence FrostResistence { get; set; }
        public ModelEnums.WaterUse WaterUse { get; set; }
        public ModelEnums.FlowerType FlowerType { get; set; }
    }
}