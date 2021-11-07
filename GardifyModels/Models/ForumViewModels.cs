using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static GardifyModels.Models._CustomValidations;

namespace GardifyModels.Models
{
	public class ForumViewModels
	{
		public class ForumIndexViewModel : _BaseViewModel
		{
			public ForumIndexViewModel()
			{
				Threads = new List<ForumHeaderViewModel>();
			}
			public List<ForumHeaderViewModel> Threads { get; set; }
			public ForumThreadViewModel CurrentThread { get; set; }
		}

		public class ForumThreadViewModel : _BaseViewModel
		{
			public ForumHeaderViewModel ThreadHeader { get; set; }
			public IEnumerable<ForumPostViewModel> RelatedPosts { get; set; }
		}

		public class ForumHeaderViewModel
		{
			public int Id { get; set; }
			public int RelatedObjectId { get; set; }
			public Guid AuthorId { get; set; }
			public string AuthorUrl { get; set; }
			public string AuthorName { get; set; }
			[Required]
            [_Title]
            public string Title { get; set; }
			[DefaultValue(0)]
			public int ParentId { get; set; }
			[DefaultValue(0)]
			public int RootId { get; set; }			
			public bool IsThread { get; set; }
			public int PostCount { get; set; }
			public DateTime CreatedDate { get; set; }
			public List<ForumHeaderViewModel> Childs { get; set; }
		}

		public class ForumPostViewModel
		{
			public int Id { get; set; }
			[Required]
			public int HeaderId { get; set; }
			public Guid AuthorId { get; set; }
			public string AuthorUrl { get; set; }
			public string AuthorName { get; set; }
			[Required]
			[_CustomValidations._Description]
			public string Content { get; set; }
			public DateTime CreatedDate { get; set; }
		}
	}
}