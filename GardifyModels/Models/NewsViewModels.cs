using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static GardifyModels.Models._CustomValidations;


namespace GardifyModels.Models
{
	public class NewsViewModels
	{
		public class NewsListViewModel : _BaseViewModel
		{
			public IEnumerable<NewsEntryViewModel> ListEntries { get; set; }
			public _modalStatusMessageViewModel StatusMessage { get; set; }
		}

		public class InstaNewsViewModel:_BaseViewModel
		{
			public List<InstaNewsEntryListModel> data { get; set; }

			public InstaNewsPagingModel paging { get; set; }

		}

		public class InstaNewsEntryListModel:_BaseViewModel
		{
			public string id { get; set; }
			public string caption { get; set; }
			public string captionRaw { get; set; }

			public string media_type { get; set; }

			public string media_url { get; set; }

			public string username { get; set; }
            public string permalink { get; set; }

            public string timestamp { get; set; }

			public string thumbnail_url { get; set; }
			public string RelatedLink { get; set; }
            public InstaNewsChildModel children { get; set; }
            public string childrenId { get; set; }
            public _modalStatusMessageViewModel StatusMessage { get; set; }
        }

		public class InstaNewsEntrySingleModel : _BaseViewModel
		{
			public string id { get; set; }

			public string media_type { get; set; }

			public string media_url { get; set; }

			public string username { get; set; }

			public string timestamp { get; set; }

		}

		public class InstaNewsChildModel
		{
			public List<InstaNewsChildDataModel> data { get; set; }

		}

		public class InstaNewsChildDataModel
		{
			public string id { get; set; }

		}

		public class InstaNewsPagingModel
		{
			public string next { get; set; }
			public string prev { get; set; }

		}


		public class NewsEntryViewModel : _BaseViewModel
		{
			public NewsEntryViewModel()
			{
				EntryImages = new List<_HtmlImageViewModel>();
			}
            public string SubTitle { get; set; }
            [Required]
            public string Theme { get; set; }
            public string Author { get; set; }
            public string Timing { get; set; }
            public string Tipp { get; set; }

            public int Id { get; set; }
			[Required]
            [_Title]
            public string Title { get; set; }
            [Required]
            [_Description]
            public string Text { get; set; }
			[Required]
			public DateTime Date { get; set; }
			[Required]
			public DateTime ValidFrom { get; set; }
			[Required]
			public DateTime ValidTo { get; set; }
			public bool IsVisibleOnPage { get; set; }
			public List<_HtmlImageViewModel> EntryImages { get; set; }
			public _modalStatusMessageViewModel StatusMessage { get; set; }			
		}
	}
}