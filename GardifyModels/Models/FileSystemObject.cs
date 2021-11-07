namespace GardifyModels.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class FileSystemObject : _BaseEntity
    {
        
        
        [Required]
        public string FileName { get; set; }

        public string RelativeFilePath { get; set; }

        public int FileExtension { get; set; }

        [StringLength(256)]
        public string OriginalFileName { get; set; }

        [Required]
        [StringLength(256)]
        public string Title { get; set; }

        [Required]
        public string Description { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<ReferencesToFileSystemObject> ReferencesToFileSystemObject { get; set; }

        [NotMapped]
        public const string NFILES_FOLDER = "nfiles/";
        [NotMapped]
        public const string PLANT_IMAGES_FOLDER = "PlantImages/";
        [NotMapped]
        public const string DIARY_IMAGES_FOLDER = "DiaryImages/";
        [NotMapped]
        public const string GARDEN_IMAGES_FOLDER = "GardenImages/";
        [NotMapped]
        public const string FAQ_IMAGES_FOLDER = "FaqImages/";
        [NotMapped]
        public const string NEWS_IMAGES_FOLDER = "NewsImages/";
        [NotMapped]
        public const string ARTICLE_IMAGES_FOLDER = "ArticleImages/";
        [NotMapped]
        public const string SCAN_IMAGES_FOLDER = "ScanImages/";
        [NotMapped]
        public const string DEVICE_IMAGES_FOLDER = "DeviceImages/";
        [NotMapped]
        public const string ECOELE_IMAGES_FOLDER = "EcoElementImages/";
        [NotMapped]
        public const string EVENTS_IMAGES_FOLDER = "EventsImages/";
        [NotMapped]
        public const string QUESTION_IMAGES_FOLDER = "QuestionsImages/";
        [NotMapped]
        public const string PLANTDOC_ANSWER_IMAGES_FOLDER = "PlantDocAnswersImages/";
        [NotMapped]
        public const string LEXICONTERMS_IMAGES_FOLDER = "LexiconTermImages/";
        [NotMapped]
        public const string USERPROFILES_IMAGES_FOLDER = "UserProfilImages/";
        [NotMapped]
        public const string ALBUM_IMAGES_FOLDER = "AlbumImages/";
        [NotMapped]
        public const string TAGS_IMAGES_FOLDER = "PlantTagImages/";
        [NotMapped]
        public const string PDF_FOLDER = "Pdf/";
    }
}
