using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static GardifyModels.Models._CustomValidations;

namespace GardifyModels.Models
{
	public class DiaryViewModels
	{

		public class DiaryListViewModel : _BaseViewModel
		{
			public int ObjectId { get; set; }
			public ModelEnums.ReferenceToModelClass DiaryType { get; set; }
			public IEnumerable<DiaryEntryViewModel> ListEntries { get; set; }
		}

		public class DiaryEntryViewModel : _BaseViewModel
		{
			public DiaryEntryViewModel()
			{
				EntryImages = new List<_HtmlImageViewModel>();
			}
			public int Id { get; set; }
			public int ObjectId { get; set; }
			public ModelEnums.ReferenceToModelClass DiaryType { get; set; }
            [Required]
            [_Title]
			public string Title { get; set; }
            [Required]
            [_Description]
            public string Description { get; set; }
			public DateTime Date { get; set; }
			public List<_HtmlImageViewModel> EntryImages { get; set; }

			public _modalStatusMessageViewModel StatusMessage { get; set; }
		}		
	}
}