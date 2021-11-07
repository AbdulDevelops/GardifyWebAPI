using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static GardifyModels.Models.GardenViewModels;

namespace GardifyModels.Models
{
	public class ProfileViewModels
	{

		public class ProfileViewModel : _BaseViewModel
		{
			public string UserName { get; set; }
			public GardenIndexViewModel Gardens { get; set; }
		}

		public class ProfileListViewModel : _BaseViewModel
		{
			public ProfileListViewModel()
			{
				UserLinks = new List<LinkHelper>();
			}
			public List<LinkHelper> UserLinks { get; set; }

			public class LinkHelper
			{
				public string UserUrl { get; set; }
				public string UserName { get; set; }
				public int UserPoints { get; set; }
			}
		}
	}
}