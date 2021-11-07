using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class ModelEnums
    {
        ///<summary>
        ///Gibt an, wie genau ein Termin dargestellt werden soll. Bspl.:
        ///Termin 26.12.15
        ///0: 26.12.15
        ///1: 4. Dezember Woche
        ///2: Dezember
        ///3: 2015
        ///</summary>
        public enum TodoPrecisionType
        {
            Date = 0, Week = 1, Month = 2, Year = 3
        }

        ///<summary>
        ///Turnus, der bestimmt wie oft ein Todo wiederholt wird.
        ///0: Nur einmal
        ///1: Wöchtenlich
        ///2: Monatlich
        ///3: Jährlich
        ///4: 2-Jährlich
        ///</summary>
        public enum TodoCycleType
        {
            Once = 0, Weekly = 1, Monthly = 2, Yearly = 3, BiYearly = 4, Daily = 5, TriYearly = 6, QuadYearly = 7, PentYearly = 8
        }

        public enum NotificationType
        {
            Calendar = 0, CalendarHighlight = 1, Push = 2, Email = 3
        }

        public enum NodeType
        {
            NotSet = 0, Root, Node, Leaf
        }

        public enum TaxonomicRank
        {
            NotSet = 0, Phylum = 1, Subphylum, ClassTaxon, SubclassTaxon, Order, Family, Genus, Species, Variety
        }

        public enum AlertType
        {
            Frost = 0, Storm = 1, FrostAndStorm = 2, NotSet = 3
        }

        public enum OrderStatus
        {
            New = 0, Shipped = 1, Returned = 2, Completed = 3
        }

        // < <=	> >= == != 
        public enum ComparisonOperator
        {
            Less, LessOrEqual, Greater, GreaterOrEqual, Equal, NotEqual
        }

        public enum LogicalOperator
        {
            And, Or, Not
        }

        public enum SuggestionApproved
        {
            Undecided = 0,
            Approved = 1,
            NotApproved = 2,
            Merged = 3
        }

        public enum ComparedValueType
        {
            // nur float wert
            MinWindSpeed, MaxWindSpeed, MinTemperature, MaxTemperature, ActualYear, ActualMonth, ActualDay,
            // nur datum wert
            ActualDate,
            // datum UND float werte werten abgefragt
            YearsSpan, MonthsSpan, DaysSpan
        }

        public enum OrderBy
        {
            nameGerman, NameGermanDesc, NameLatin, NameLatinDesc, CreatedDate, CreatedDateDesc
        }

        /// <summary>
        /// If changed please change:
        /// - PlantCharacteristicCategoryController.GetUnitRangeResult
        /// </summary>
        public enum CharacteristicValueType
        {
            [Display(Name = "Bitte einen Typen auswählen")]
            DoNotUse,
            [Display(Name = "Einzelne Zahl [Bsp.: 122]")]
            SingleNumber,
            [Display(Name = "Zahl (Von-Bis) [Bsp.: 25-40]")]
            NumberRange,
            [Display(Name = "Einzelner Monat [Bsp.: 08]")]
            SingleMonth,
            [Display(Name = "Monate (Von-Bis) [Bsp.: 02-11]")]
            MonthRange,
            [Display(Name = "Einzelne Römische Zahl [Bsp.: XVI]")]
            SingleLatinNumber,
            [Display(Name = "Römische Zahl (Von-Bis) [Bsp.: V-VII]")]
            LatinNumberRange
        }

        public enum FileExtension
        {
            NotSet = 0, Jpg, Png, Gif, Doc, Txt
        }

        public enum FileReferenceType
        {
            NotSet = 0, PlantImage, UserPlantImage, GardenImage, TagImage, DiaryEntryImage, FaqEntryImage, Document, NewsEntryImage, ArticleImage, ScanImage,AdminDeviceImg,
            Suggestion,EcoElementImages, EventImage, QuestionImage,PlantDocAnswerImage, LexiconTermImage, UserProfileImage, AlbumImage, CommunityPost, CommunityAnswer, PresentationImage
        }

        public enum ReferenceToModelClass
        {
            NotSet = 0, 
            UserPlant, 
            UserTool, 
            Garden, 
            Plant, 
            Tool, 
            PlantTag, 
            PlantCharacteristicCategory, 
            DiaryEntry, 
            FaqEntry, 
            NewsEntry, 
            Article, 
            Todo, 
            GeneralInternalComment, 
            TaxonomicTree, 
            LexiconTerm, 
            UserDevice, 
            ScanImage,
            AdminDevice,
            Device,
            BioScan = 20,
            EcoElement, 
            Event, 
            PlantDocQuestion, 
            PlantDocAnswer, 
            UserProfil, 
            Suggestion,
            AlbumImage = 27,
            CommunityPost,
            CommunityAnswer,
            PresentationImage

        }

        public enum RatableObject
        {
            GardenImage, Garden
        }

        public enum Rating
        {
            NotSet = 0, One = 1, Two = 2, Three = 3, Four = 4, Five = 5
        }

        public enum ActionStatus
        {
            Undefined, Success, Warning, Error
        }

        public enum DatabaseMessage
        {
            Undefined, Ok, Created, Deleted, Edited, ObjectNotFound, DuplicatedEntry, ErrorOnSaveChanges, FolderNotExists, FileNotExists, EmptyResult, WrongQuantity
        }

        public enum PrivacyLevel
        {
            Private, FriendsOnly, Public, GardifyOnly
        }

        public enum FlowerType
        {
            NotSet, CutFlower, PotPlant
        }

        public enum WaterUse
        {
            NotSet, Small, Medium, Large
        }

        public enum FrostResistence
        {
            NotSet, Zero, Negative5, Negative15
        }

        public enum Location
        {
            NotSet, Sun, SemiShade, Shade
        }

        public enum ArticleReferenceType
        {
            [Display(Name = "Pflanze")]
            Plant,
            [Display(Name = "Todovorlage")]
            Todotemplate
        }

        public enum BonusPointType
        {
            Earned = 0, Spent = 1
        }

        public enum PointsForAction
        {
            // video = 30 .. 300
            Photo = 10,
            BookReview = 30,
            EventHint = 30,
            DamagePhoto = 30
        }

        public enum UserAction
        {
            NewPlant,
            EcoScan,
            SuggestPlant,
            NewTodo,
            NewsArchive,
            VideosArchive,
            TodoVideos,
            PlantDoc,
            SearchGardenImages,
            GardenPresentation,
            EditPlantScan,
            EditSuggestPlant,
            FavNews,
            FavVideos,
            NewTodoImage,
            NewGardenImage,
            WeatherInKalender
        }

        public enum FeatureAccess
        {
            NotAllowed, 
            Allowed,
            Limited // example: in case only recent news/videos are available
        }
    }
}