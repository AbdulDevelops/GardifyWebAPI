using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
	public class PlantDetailsViewModel : _BaseViewModel
	{
		public WeatherObject CurrentWeather { get; set; }
		public Plant Plant { get; set; }
		public bool IsInUserGarden { get; set; }
	}

	public class PlantSearchViewModel : _BaseViewModel
	{
		// from view
		public int? CategoryId { get; set; }
		public int? SubCategoryId { get; set; }
		public int? SelectedPositiveFilterTagId { get; set; }
		public int? SelectedTagId { get; set; }
		public string Input_search { get; set; }
		public int? TaxonId { get; set; }
		public int? SelHmin { get; set; }
		public int? SelHmax { get; set; }
        public int? SelMinMonth { get; set; }
        public int? SelMaxMonth { get; set; }
        public string Fm { get; set; }
        public IEnumerable<PlantViewModels.PlantViewModelLiteTodo> ResultSortedByInput { get; set; }
        public List<AppliedFilterVM> AppliedFilters { get; set; }
                                           // to view
        public TaxonomicTree TreeRoot { get; set; }
		public IEnumerable<PlantTagCategory> CategoryList { get; set; }
		public List<PlantTagCategory> SubCategoryList { get; set; }
		public IEnumerable<PlantTag> TagsList { get; set; }
		public IEnumerable<PlantTag> PositiveFilterTagsList { get; set; }		
        public IEnumerable<PlantTagSuperCategoryViewModel> SuperCategories { get; set; }
		public IEnumerable<PlantViewModels.PlantViewModelLiteTodo> Plants { get; set; }
		public IEnumerable<PlantViewModels.PlantViewModel> PlantsOld { get; set; }
		public IEnumerable<Plant> PlantList { get; set; }
		public IEnumerable<CheckboxHelper> MonthCheckboxes { get; set; }

        public IEnumerable<PlantSearchPropertyView> PlantProperties { get; set; }
        public int? HeightMin { get; set; }
		public int? HeightMax { get; set; }
        public int? MinMonth { get; set; }
        public int? MaxMonth { get; set; }
        public IEnumerable<SearchQuery> SearchQueries { get; set; }

		public class CheckboxHelper
		{
			public bool Checked { get; set; }
			public bool Disabled { get; set; }
			public int Value { get; set; }
			public string Text { get; set; }
		}
	}

    public class PlantSearchViewModelLite : _BaseViewModel
    {
        // from view
        public int? CategoryId { get; set; }
        public int? SubCategoryId { get; set; }
        public int? SelectedPositiveFilterTagId { get; set; }
        public int? SelectedTagId { get; set; }
        public string Input_search { get; set; }
        public int? TaxonId { get; set; }
        public int? SelHmin { get; set; }
        public int? SelHmax { get; set; }
        public int? SelMinMonth { get; set; }
        public int? SelMaxMonth { get; set; }
        public string Fm { get; set; }


        // to view
        public TaxonomicTree TreeRoot { get; set; }
        public IEnumerable<PlantTagCategoryLite> CategoryList { get; set; }
        public List<PlantTagCategory> SubCategoryList { get; set; }
        public IEnumerable<PlantTagLite> TagsList { get; set; }
        public IEnumerable<PlantTagLite> PositiveFilterTagsList { get; set; }
        public IEnumerable<PlantViewModels.PlantViewModel> Plants { get; set; }
        public IEnumerable<CheckboxHelper> MonthCheckboxes { get; set; }
        public int? HeightMin { get; set; }
        public int? HeightMax { get; set; }
        public int? MinMonth { get; set; }
        public int? MaxMonth { get; set; }
        public IEnumerable<SearchQuery> SearchQueries { get; set; }

        public class CheckboxHelper
        {
            public bool Checked { get; set; }
            public bool Disabled { get; set; }
            public int Value { get; set; }
            public string Text { get; set; }
        }
    }





}