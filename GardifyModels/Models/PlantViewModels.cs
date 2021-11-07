using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static GardifyModels.Models._CustomValidations;
using static GardifyModels.Models.ArticleViewModels;

namespace GardifyModels.Models
{
	public class PlantViewModels
	{
		public class PlantListViewModel : _BaseViewModel
		{
			public IEnumerable<PlantViewModel> ListEntries { get; set; }
			public _modalStatusMessageViewModel StatusMessage { get; set; }
		}

        public class PlantViewModel : _BaseViewModel
        {
            public PlantViewModel()
            {
                Images = new List<_HtmlImageViewModel>();
            }
            public int Id { get; set; }
            [Required]
            [_Title]
            public string NameLatin { get; set; }
            [_Title]
            public string NameGerman { get; set; }
            [Required]
            [_Description]
            public string Description { get; set; }
            public bool IsInUserGarden { get; set; }
            public TaxonomicTree GenusTaxon { get; set; }
            public IEnumerable<PlantTag> PlantTags { get; set; }
            public IEnumerable<PlantCharacteristicSimplified> PlantCharacteristics { get; set; }
            public IEnumerable<PlantCharacteristic> PlantCharacteristicsOld { get; set; }
            public List<_HtmlImageViewModel> Images { get; set; }
            public _modalStatusMessageViewModel StatusMessage { get; set; }
            public IEnumerable<Article> Articles { get; set; }
            public bool Published { get; set; }
            public IEnumerable<InternalComment> Comments { get; set; }
            public IEnumerable<TodoTemplate> TodoTemplates { get; set; }
            public string Synonym { get; set; }
            public string Family { get; set; }
            public string Links { get; set; }
            public double Score { get; set; }
            public string Herkunft { get; set; }
            public ModelEnums.SuggestionApproved Genehmigt { get; set; }
            public int GenehmigtOrder =>
                Genehmigt == ModelEnums.SuggestionApproved.Undecided ? 0 :
                (Genehmigt == ModelEnums.SuggestionApproved.NotApproved ? 1 : 2);
            public string SuggestionStatusText { get; set; }
            public GardenCategory GardenCategory { get; set; }
            public IEnumerable<GroupSimplified> PlantGroups { get; set; }
            public IEnumerable<Group> PlantGroupsOld { get; set; }
            public DateTime CreatedDate { get; set; }
            public bool Deleted { get; set; }

        }

        public class PlantViewModelLite : _BaseViewModel
        {
            public PlantViewModelLite()
            {
                Images = new List<_HtmlImageViewModel>();
            }
            public int Id { get; set; }
            [Required]
            [_Title]
            public string NameLatin { get; set; }
            [_Title]
            public string NameGerman { get; set; }
            [Required]
            [_Description]
            public string Description { get; set; }
            //public bool IsInUserGarden { get; set; }
            //public TaxonomicTree GenusTaxon { get; set; }
            //public IEnumerable<PlantTag> PlantTags { get; set; }
            //public IEnumerable<PlantCharacteristic> PlantCharacteristics { get; set; }
            public List<_HtmlImageViewModel> Images { get; set; }
            //public _modalStatusMessageViewModel StatusMessage { get; set; }
            //public IEnumerable<Article> Articles { get; set; }
            //public bool Published { get; set; }
            //public IEnumerable<InternalComment> Comments { get; set; }
            //public IEnumerable<TodoTemplate> TodoTemplates { get; set; }
            //public string Synonym { get; set; }
            //public string Family { get; set; }
            //public string Herkunft { get; set; }
            //public GardenCategory GardenCategory { get; set; }
            //public IEnumerable<Group> PlantGroups { get; set; }
        }

        public class PlantViewModelLiteTodo
        {
            public int Id { get; set; }

            public string NameLatin { get; set; }
            public string NameGerman { get; set; }
            public string Description { get; set; }
            public bool IsInUserGarden { get; set; }
            public List<_HtmlImageViewModel> Images { get; set; }
            public List<TodoTemplate> Todos { get; set; }
            public IEnumerable<string> Colors { get; set; }
            public string Synonym { get; set; }
            public IEnumerable<PlantTagSearchLite> Badges { get; set; }
        }

        public class LatestPlantViewModel
        {
            public int Id { get; set; }
            public string NameLatin { get; set; }
            public string NameGerman { get; set; }
            public string Description { get; set; }
            public int MonthAdded { get; set; }
            public string Synonym { get; set; }
            public List<_HtmlImageViewModel> Images { get; set; }
            public IEnumerable<string> Colors { get; set; }
            public IEnumerable<PlantTagSearchLite> Badges { get; set; }
        }
        public class PlantDetailsViewModelLite
        {
            public PlantDetailsViewModelLite()
            {
                Images = new List<_HtmlImageViewModel>();
            }
            public int Id { get; set; }
            public string NameLatin { get; set; }
            public string NameGerman { get; set; }
            public string Description { get; set; }
            public bool IsInUserGarden { get; set; }
            public TaxonomicTree GenusTaxon { get; set; }
            public IEnumerable<PlantTagLite> PlantTags { get; set; }
            public IEnumerable<PlantCharacteristicSimplified> PlantCharacteristics { get; set; }
            public List<_HtmlImageViewModel> Images { get; set; }
            public IEnumerable<ArticleViewModelLite> Articles { get; set; }
            public IEnumerable<TodoTemplate> TodoTemplates { get; set; }
            public string Synonym { get; set; }
            public string Family { get; set; }
            public string Herkunft { get; set; }
            public IEnumerable<string> Colors { get; set; }
            public IEnumerable<GroupSimplified> PlantGroups { get; set; }
            public GardenCategory GardenCategory { get; set; }
        }

    }


}