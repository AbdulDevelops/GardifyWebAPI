using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
	public class HelperClasses
	{

		public class DbResponse
		{
			public DbResponse()
			{
				Messages = new List<ModelEnums.DatabaseMessage>();
				ResponseObjects = new List<object>();
			}

			public List<ModelEnums.DatabaseMessage> Messages { get; set; }
			public ModelEnums.ActionStatus Status { get; set; }
			public List<object> ResponseObjects { get; set; }
		}
	}
}