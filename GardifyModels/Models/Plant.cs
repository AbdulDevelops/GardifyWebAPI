namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Plant : _BaseEntity, IReferencedObject
    {
        [Required]
        [StringLength(256)]
        public string NameLatin { get; set; }
        [StringLength(256)]
        public string NameGerman { get; set; }
        [Required]
        [StringLength(4096)]
        public string Description { get; set; }
        public virtual ICollection<PlantTag> PlantTags { get; set; }
        public virtual ICollection<Group> PlantGroups { get; set; }

        [StringLength(256)]
        public string Herkunft { get; set; }
        [StringLength(256)]
        public string Synonym { get; set; }
        [StringLength(256)]
        public string Familie { get; set; }

        public bool PremiumOnly { get; set; }
        public bool Vorschlagen { get; set; }
        public int? GardenCategoryId { get; set; }

        public ModelEnums.SuggestionApproved Genehmigt { get; set; }

        public string SuggestionStatusText =>
            Genehmigt == ModelEnums.SuggestionApproved.Approved ? "Genehmigen" :
            (Genehmigt == ModelEnums.SuggestionApproved.NotApproved ? "Nicht genehmigen" :
            (Genehmigt == ModelEnums.SuggestionApproved.Merged ? "Merged" : "Warten auf Eingaben"));

        [ForeignKey("GardenCategoryId")]
        public virtual GardenCategory GardenCategory { get; set; }
        
        //...

        [NotMapped]
        public virtual IEnumerable<Alert> Alerts { get; set; }
        
        [NotMapped]
        public IEnumerable<PlantCharacteristic> PlantCharacteristics { get; set; }

        public ModelEnums.ReferenceToModelClass ReferenceType
        {
            get
            {
                return ModelEnums.ReferenceToModelClass.Plant;
            }
        }

        [NotMapped]
        public string Name
        {
            get
            {
                return NameLatin != null ? (NameLatin + " (" + NameGerman + ")") : NameGerman;
            }
            set { }
        }



        public bool Published { get; set; }
        public DateTime PublishedDate { get; set; }
        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        //public Plant()
        //{
        //    ArticleReferences = new HashSet<ArticleReference>();
        //    LastVieweds = new HashSet<LastViewed>();
        //    PlantCharacteristics = new HashSet<PlantCharacteristic>();
        //    UserPlants = new HashSet<UserPlant>();
        //    WatchlistEntries = new HashSet<WatchlistEntry>();
        //    Groups = new HashSet<Group>();
        //    PlantTags = new HashSet<PlantTag>();
        //}



        //[Required]
        //[StringLength(256)]
        //public string NameLatin { get; set; }

        //[StringLength(256)]
        //public string NameGerman { get; set; }

        //[Required]
        //public string Description { get; set; }

        //public bool Published { get; set; }

        //[StringLength(256)]
        //public string Herkunft { get; set; }

        //public int? GardenCategoryId { get; set; }

        //[StringLength(256)]
        //public string Synonym { get; set; }

        //[StringLength(256)]
        //public string Familie { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<ArticleReference> ArticleReferences { get; set; }

        //public virtual GardenCategory GardenCategory { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<LastViewed> LastVieweds { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<PlantCharacteristic> PlantCharacteristics { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<UserPlant> UserPlants { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<WatchlistEntry> WatchlistEntries { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<Group> Groups { get; set; }

        //[System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        //public virtual ICollection<PlantTag> PlantTags { get; set; }
    }
}
