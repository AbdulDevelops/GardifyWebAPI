namespace GardifyModels.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class UserPlant : _BaseEntity, IReferencedObject
    {
        public UserPlant()
        {
            WarningsAboutThisPlant = new HashSet<UserWarning>();
        }
        [Required]
        [StringLength(256)]
        public string Name { get; set; }
        public string NameLatin { get; set; }
        public string Synonym { get; set; }
        [Required]
        [StringLength(4096)]
        public string Description { get; set; }
        public string Notes { get; set; }
        [Required]
        [ForeignKey("Plant")]
        public int PlantId { get; set; }
        public virtual Plant Plant { get; set; }
        [Required]
        [ForeignKey("Garden")]
        public int Gardenid { get; set; }
        [Required]
        public int Count { get; set; }
        [JsonIgnore]
        public virtual Garden Garden { get; set; }
        [Required]
        public int InitialAgeInDays { get; set; }
        [Required]
        public bool IsInPot { get; set; }
        public bool NotifyForFrost { get; set; }
        public bool NotifyForWind { get; set; }

        public virtual ICollection<UserWarning> WarningsAboutThisPlant { get; set; }

        public string NameLatinClean => NameLatin.Replace("[k]", "");

        [NotMapped]
        public ModelEnums.ReferenceToModelClass ReferenceType
        {
            get
            {
                return ModelEnums.ReferenceToModelClass.UserPlant;
            }
        }

        //public int InitialAgeInDays { get; set; }

        //public int Gardenid { get; set; }

        //public int PlantId { get; set; }

        //public int Count { get; set; }

        //[Required]
        //public string Description { get; set; }


        //[Required]
        //[StringLength(256)]
        //public string Name { get; set; }

        //public bool IsInPot { get; set; }

        //public virtual Plant Plant { get; set; }
    }

    public class SuggestViewModel
    {
        public string PlantName { get; set; }
        public string ImgUrl { get; set; }
    }
}
