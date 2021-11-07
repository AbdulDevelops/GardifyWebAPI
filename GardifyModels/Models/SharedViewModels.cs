using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
	public class _modalAddPlantToGardenPropertyViewModel
	{
		public IEnumerable<Garden> Gardens { get; set; }
		[Required]
		public int PlantId { get; set; }
		public int PlantCount { get; set; }
		public int PropertyId { get; set; }
		public int InitialAgeInDays { get; set; }
        public int GardenId { get; set; }
        public bool IsInPot { get; set; }
        public IEnumerable<_TodoCheckedTemplateViewModel> Todos { get; set; }
	}

    public class _TodoCheckedTemplateViewModel : _BaseViewModel
    {
        public int TemplateId { get; set; }
        public string TemplateTitle { get; set; }
        public bool Checked { get; set; }
    }

	public class _modalAddPlantToWatchlistViewModel
	{
		[Required]
		public int PlantId { get; set; }
		[Required]
		public Guid UserId { get; set; }
		public int PlantCount { get; set; }
		public string NameLatin { get; set; }
		public string NameGerman { get; set; }
	}

	public class _modalStatusMessageViewModel
	{
		private List<string> _messages;
		public List<string> Messages
		{
			get
			{
				if (_messages == null)
				{
					_messages = new List<string>();
					return _messages;
				}
				else
				{
					return _messages;
				}
			}
			set { _messages = value; }
		}
		public ModelEnums.ActionStatus Status { get; set; }
	}

	public class _HtmlImageViewModel
	{
		public int Id { get; set; }
		public string Author { get; set; }
		public string License { get; set; }
		public string FullTitle { get; set; }
		public string FullDescription { get; set; }
		public string TitleAttr { get; set; }
		public string AltAttr { get; set; }
		public string SrcAttr { get; set; }
		public string SrcAttrBackend => SrcAttr.Replace("/intern", "");

		public int Sort { get; set; }
		public DateTime InsertDate { get; set; }
		public DateTime? TakenDate { get; set; }
		public string Comments { get; set; }
		public string Tags { get; set; }
		public string Note { get; set; }
		public decimal Rating { get; set; }
		public bool IsOwnImage { get; set; }

		public bool IsMainImg{ get; set; }
        public IEnumerable<GardenAlbumViewModel> Albums { get; set; }
		public bool highlighted { get; set; }
	}

    public class UpdateImageViewModel
    {
        [Required]
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime? TakenDate { get; set; }
        public string Tags { get; set; }
        public string Note { get; set; }
		public string imageTitle  { get; set; }

	}
	public class ImagesListViewModel
    {
		public ImagesListViewModel()
        {
			Images = new List<_HtmlImageViewModel>();
        }
		public List<_HtmlImageViewModel> Images;
    }
}