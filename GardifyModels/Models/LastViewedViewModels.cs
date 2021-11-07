using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
	public class LastViewedViewModels
	{
		public class LastViewedIndexViewModel : _BaseViewModel
		{
			public IEnumerable<PlantViewModels.PlantViewModel> UserLastViewed { get; set; }
			public IEnumerable<PlantViewModels.PlantViewModel> OtherLastViewed { get; set; }
		}
	}
}