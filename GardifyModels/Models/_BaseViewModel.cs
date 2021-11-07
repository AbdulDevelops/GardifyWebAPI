using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
	public class _BaseViewModel
	{
		public int ShopcartCounter { get; set; }
		public int PlantCount { get; set; }
		public int Points { get; set; }
		public int CurrentTodoCount { get; set; }
		public int NewMessages { get; set; }
	}
}