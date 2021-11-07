using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static GardifyModels.Models._CustomValidations;
using static GardifyModels.Models.InternalCommentViewModels;

namespace GardifyModels.Models
{
    public class AdminAreaViewModels
    {
        public class TaxonomicTreeViewModel
        {
            public List<TaxonomicTree> TreeRootsWithChilds { get; set; }
            public TaxonomicTree CurrentNode { get; set; }
            public Plant Plant { get; set; }
            public List<_HtmlImageViewModel> PlantImages { get; set; }
            public TaxonomicTree CurrentParentNode { get; set; }
            public int? MovingNodeId { get; set; }
            public TaxonomicTree CurrentMovingNode { get; set; }

        }

        public class TaxonomicBulkEditViewModel
        {
            public string Name { get; set; }
            public int TreeId { get; set; }
            [DisplayName("Eigenschaften")]
            public IEnumerable<PlantTag> PlantTagsList { get; set; }

            public IEnumerable<BulkTagViewModel> TagList { get; set; }
            public int SelectedTagCategoryId { get; set; }
            [DisplayName("Merkmale")]
            public IEnumerable<PlantCharacteristicCategory> characteristicCategories { get; set; }
            public PlantCharacteristic SelectedCharacteristic { get; set; }
            [DisplayName("Neue Warnung")]
            public Alert NewAlert { get; set; }
        }

        public class TaxonomicBulkAddViewModel
        {
            public string Name { get; set; }
            public int TreeId { get; set; }
            public int TagId { get; set; }

            [DisplayName("Eigenschaften")]
            public IEnumerable<PlantTag> PlantTagsList { get; set; }

            public IEnumerable<BulkTagViewModel> TagList { get; set; }
            public int SelectedTagCategoryId { get; set; }
            [DisplayName("Merkmale")]
            public IEnumerable<PlantCharacteristicCategory> characteristicCategories { get; set; }
            public PlantCharacteristic SelectedCharacteristic { get; set; }
            [DisplayName("Neue Warnung")]
            public Alert NewAlert { get; set; }
        }

        public class TaxonomicBulkDeleteViewModel
        {
            public int TreeId { get; set; }
            public int PlantId { get; set; }
            public string TagName { get; set; }

            public int TagId { get; set; }
            public bool fromDetail { get; set; }

        }

        public class TaxonomicBulkOptionViewModel
        {
            public string Name { get; set; }
            public int TreeId { get; set; }
            public IEnumerable<TodoTemplate> todoTemplates { get; set; }


        }

        public class TaxonomicBulkSelectViewModel
        {
            public int TreeId { get; set; }
            //PlantCharacteristics
            [DisplayName("Merkmale")]
            public PlantCharacteristic SelectedCharacteristic { get; set; }
            //PlantAlerts
            public Alert NewAlert { get; set; }
            //PlantTags
            public int TagId { get; set; }
            public string TagTitle { get; set; }
            public IEnumerable<BulkTagViewModel> TagList { get; set; }
            public int TagCategoryId { get; set; }
            public string TagCategoryTitle { get; set; }
        }

        public class BulkTagViewModel
        {
            public int PlantId { get; set; }
            public string PlantName { get; set; }
            public string PlantNameLatin { get; set; }
            public bool Published { get; set; }

            public bool AddTagToPlant { get; set; }
            public IEnumerable<AddPlantTagViewModel> PlantTags { get; set; }
        }

        public class AddPlantTagViewModel
        {
            public int PlantTagId { get; set; }
            public string PlantTagTitle { get; set; }
            public string PlantTagCategoryName { get; set; }
            public int PlantTagCategoryId { get; set; }
            public bool AddTagToPlant { get; set; }
        }


        public class PlantTagViewModel
        {
      
            public IEnumerable<PlantTagCategory> CategoryList { get; set; }
            public PlantTagCategory SelectedCategory { get; set; }
            public int SelectedListEntryId { get; set; }
            public IEnumerable<PlantTag> PlantTags { get; set; }
            public PlantTag PlantTag { get; set; }

            public IEnumerable<PlantCharacteristicCategory> CharacteristicCategories { get; set; }
        }

        public class PlantViewModel
        {
            public PlantViewModel()
            {
                PlantImages = new List<_HtmlImageViewModel>();
            }
            public TodoTemplateViewModels.TodoTemplateIndexViewModel TodoTemplateIndexViewModel { get; set; }
            public TodoTemplateViewModels.TodoTemplateCreateViewModel NewTodoTemplate { get; set; }
            public TaxonomicTreeInsertViewModel TaxonomicTreeInsertViewModel { get; set; }
            public Plant Plant { get; set; }
            public IEnumerable<Plant> PlantList { get; set; }
            public IEnumerable<PlantTagCategory> TagCategories { get; set; }
            public IEnumerable<PlantTag> PlantTagsList { get; set; }
            public IEnumerable<PlantCharacteristic> PlantCharacteristicsList { get; set; }

            public IEnumerable<Group> PlantGroupsList { get; set; }
            public int SelectedTagCategoryId { get; set; }
            public IEnumerable<PlantCharacteristicCategory> characteristicCategories { get; set; }
            public PlantCharacteristic NewCharacteristic { get; set; }
            public List<_HtmlImageViewModel> PlantImages { get; set; }
            public InternalCommentIndexViewModel PlantInternalComments { get; set; }
            public string NewInternalComment { get; set; }
            public _modalStatusMessageViewModel StatusMessage { get; set; }
            public ImportHistory ImportHistory { get; set; }
            public String EmailText { get; set; }
            public ModelEnums.SuggestionApproved suggestion { get; set; }

        }

        public class PlantVorschagenEditViewModel
        {
            public int Id { get; set; }
            public string author { get; set; }

            public string authorEmail { get; set; }
            public string message { get; set; }
            public int mergeId { get; set; }

            public string decision { get; set; }
        }

        public class TaxonomicTreeInsertViewModel : _BaseViewModel
        {
            public TaxonomicTreeInsertViewModel()
            {
                AvailableTrees = new List<TaxonomicTree>();
            }

            public List<TaxonomicTree> AvailableTrees { get; set; }
            public int PlantId { get; set; }
            public int TreeId { get; set; }
        }

        public class PlantCreateViewModel : _BaseViewModel
        {
            [Required]
            [_CustomValidations._Description]
            public string Description { get; set; }
            [Required]
            [_Title]
            public string NameLatin { get; set; }
            [_Title]
            public string NameGerman { get; set; }
            public string Herkunft { get; set; }
            public IEnumerable<IReferencedObject> PlantList { get; set; }
            [Display(Name = "Eigenschaften von Pflanze kopieren")]
            public int SelectedPlantId { get; set; }
            [StringLength(4096)]
            [DisplayName("Interne Bemerkung")]
            public string InternalComment { get; set; }
            public _modalStatusMessageViewModel StatusMessage { get; set; }
        }

        public class UtilsViewModel
        {
            // input
            [DisplayFormat(DataFormatString = "{0:dd.MM.yyyy}", ApplyFormatInEditMode = true)]
            public DateTime CustomCurrentDate { get; set; }
            public int CustomCurrentTemperature { get; set; }
            public bool UseCustomData { get; set; }

            // output
            public WeatherHelpers.HourlyForecast CurrentWeather { get; set; }
            public string RawWeather { get; set; }
            public Message StatusMessage { get; set; }
            public decimal PointValue { get; set; }

            public class Message
            {
                public Message(string messageBody = "", string messageClass = "panel-primary")
                {
                    this.MessageBody = messageBody;
                    this.MessageCssClass = messageClass;
                }

                private string _messageCssClass;
                public string MessageCssClass
                {
                    get { return this._messageCssClass ?? "panel-primary"; }
                    set { _messageCssClass = value; }
                }
                public string MessageBody { get; set; }
            }
        }

        public class AdminAreaImportIndexViewModel
        {
            public List<SelectListItem> PlantList { get; set; }
            public List<SelectListItem> PlantListCopy { get; set; }
            public int SelectedPlantId { get; set; }
            public int? SelectedCopyPlantId { get; set; }
        }
    }
}