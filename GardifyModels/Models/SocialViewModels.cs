using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static GardifyModels.Models._CustomValidations;

namespace GardifyModels.Models
{
	public class SocialViewModels
	{

		public class UserProfileViewModel : _BaseViewModel
		{
			public IEnumerable<UserProfileGardenView> UserGardens { get; set; }
			
		}

		public class UserProfileGardenView : _BaseViewModel
		{
			public int GardenId { get; set; }
            [Required]
            [_Description]
			public string Name { get; set; }
			public List<_HtmlImageViewModel> Images { get; set; }
			public List<Plant> Plants { get; set; }
			public List<DiaryEntry> DiaryEntries { get; set; }
		}
	}
}