using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static GardifyModels.Models._CustomValidations;
using static GardifyModels.Models.TodoViewModels;
using static GardifyModels.Models.UserPlantViewModels;

namespace GardifyModels.Models
{

	public class GardenViewModels
	{
		public class GardenIndexViewModel : _BaseViewModel
		{
			public TodoIndexViewModel TodoList { get; set; }
			public IEnumerable<AlertViewModels.UserPlantAlertsViewModel> Alerts { get; set; }
			public IEnumerable<GardenDetailsViewModel> Gardens { get; set; }
			public IEnumerable<WatchlistEntry> WatchlistEntries { get; set; }
			public WeatherHelpers.HourlyForecast CurrentWeather { get; set; }
            public string City { get; set; }
		}

		public class GardenDetailsViewModel : _BaseViewModel
		{
			public GardenDetailsViewModel()
			{
				Images = new List<_HtmlImageViewModel>();
			}
			public int Id { get; set; }
			public string Name { get; set; }
			public int GroundType { get; set; }
			public int PhType { get; set; }
			public int Wetness { get; set; }
			public string Description { get; set; }
			//0=North, 1=East, 2=South, 3=West
			public int CardinalDirection { get; set; }
			//0=No shadow, 1=Half shadow, 2=Full shadow
			public int ShadowStrength { get; set; }
			public bool Inside { get; set; }
            public int MainImageId { get; set; }
			//Only if inside == true
			public int Temperature { get; set; }
			public int Light { get; set; }
			public bool IsPrivate { get; set; }
			public List<_HtmlImageViewModel> Images { get; set; }
			public IEnumerable<UserPlantLightViewModel> PlantsLight { get; set; }
			public UserPlantIndexViewModel Plants { get; set; }
            public TodoIndexViewModel TodoList { get; set; }
		}

		public class GardenCreateViewModel : _BaseViewModel
		{
            [_Title]
            [Required]
			public string Name { get; set; }
            [_Description]
            [Required]
            public string Description { get; set; }
            public int GroundType { get; set; }
			public int PhType { get; set; }
			public int Wetness { get; set; }
			//0=North, 1=East, 2=South, 3=West
			public int CardinalDirection { get; set; }
			//0=No shadow, 1=Half shadow, 2=Full shadow
			public int ShadowStrength { get; set; }
			public bool Inside { get; set; }
			//Only if inside == true
			public int Temperature { get; set; }
			public int Light { get; set; }
			public bool IsPrivate { get; set; }
		}

        public class GardenImageSortViewModel
        {
            public int Id { get; set; }
            public int Sort { get; set; }
        }


        public class GardenEditViewModel : _BaseViewModel
		{
			public int Id { get; set; }
            [_Title]
            [Required]
            public string Name { get; set; }
			public int GroundType { get; set; }
			public int PhType { get; set; }
			public int Wetness { get; set; }
            [_Description]
            [Required]
            public string Description { get; set; }
			//0=North, 1=East, 2=South, 3=West
			public int CardinalDirection { get; set; }
			//0=No shadow, 1=Half shadow, 2=Full shadow
			public int ShadowStrength { get; set; }
			public bool Inside { get; set; }
			//Only if inside == true
			public int Temperature { get; set; }
			public int Light { get; set; }
			public bool IsPrivate { get; set; }

            public int MainImageId { get; set; }
            public List<_HtmlImageViewModel> Images { get; set; }
		}

		public class GardenDeleteViewModel : _BaseViewModel
		{
			public int Id { get; set; }
			public string Name { get; set; }

		}
	}

}