using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class GardenAlbum : _BaseEntity
    {
        public Guid UserId { get; set; }
        [Required]
        public string Name { get; set; }
        public string Description { get; set; }
        public GardenPresentationPersonMode ShowMode { get; set; }

    }



    public class GardenAlbumViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string AuthorName { get; set; }
        public Guid AuthorId { get; set; }

        public List<_HtmlImageViewModel> ProfilUrl { get; set; }
        public List<_HtmlImageViewModel> EntryImages { get; set; }

    }
    public class GardenAlbumFileToModuleViewModel
    {
        public string Headline { get; set; }
        public string Location { get; set; }
        public DateTime UserCreatedDate { get; set; }
        public DateTime AlternativeDate { get; set; }
        public bool ImgOwner { get; set; }
        public int Rating { get; set; }
        public string Source { get; set; }
        public string Tags { get; set; }
    }
    public class GardenAlbumFileToModule: _BaseEntity
    {
        public int GardenAlbumId { get; set; }

        public int FileToModuleID { get; set; }
        public string Headline { get; set; }
        public string Location { get; set; }
        public DateTime UserCreatedDate { get; set; }
        public DateTime AlternativeDate { get; set; }
        public bool ImgOwner { get; set; }
        public int Rating { get; set; }
        public string Source { get; set; }
        public string UserId { get; set; }
        public string Tags { get; set; }
    }
    public class GardenPresiFileToModule:_BaseEntity
    {
        public int GardenPresiId { get; set; }

        public int FileToModuleID { get; set; }
        public string Headline { get; set; }
        public string Location { get; set; }
        public DateTime UserCreatedDate { get; set; }
        public DateTime AlternativeDate { get; set; }
        public bool ImgOwner { get; set; }
        public int Rating { get; set; }
        public string Source { get; set; }
        public string UserId { get; set; }
        public string Tags { get; set; }
    }
    public class FavoriteGardenImage
    {
        public int FileToModuleID { get; set; }
        public Guid UserId { get; set; }
    }

    public class GardenPresentation : _BaseEntity
    {
        public Guid UserId { get; set; }
        public string Headline { get; set; }
        public bool ShowHeadline { get; set; }
        public bool ShowPictureNumber { get; set; }

        public virtual ICollection<GardenPresentationImage> GardenPresentationImages { get; set; }
        public GardenPresentationPersonMode ShowMode { get; set; }

    }

    public class GardenPresentationViewModel : _BaseEntity
    {
        public GardenPresentationViewModel(GardenPresentation model)
        {
            PresentationId = model.Id;
            Headline = model.Headline;
            ShowHeadline = model.ShowHeadline;
            ShowPictureNumber = model.ShowPictureNumber;
            ShowMode = model.ShowMode;
            EntryImages = new List<_HtmlImageViewModel>();
            AuthorId = model.UserId.ToString().ToLower();
        }
        public string Headline { get; set; }
        public bool ShowHeadline { get; set; }
        public bool ShowPictureNumber { get; set; }
        public int PresentationId { get; set; }

        public GardenPresentationPersonMode ShowMode { get; set; }

        public List<_HtmlImageViewModel> EntryImages { get; set; }

        public string AuthorName { get; set; }
        public string AuthorId { get; set; }

        public List<_HtmlImageViewModel> ProfilUrl { get; set; }
        public String ShowModeString => Enum.GetName(typeof(GardenPresentationPersonMode), ShowMode);

    }

    public class GardenPresentationCreateViewModel : _BaseEntity
    {
        public string Headline { get; set; }
        public bool ShowHeadline { get; set; }
        public bool ShowPictureNumber { get; set; }

        public GardenPresentationPersonMode ShowMode { get; set; }

        public string ImageIdList { get; set; }



    }

    public class GardenPresentationEditImageViewModel : _BaseEntity
    {
        public int Id { get; set; }

        public string ImageIdList { get; set; }



    }

    public class GardenPresentationImage : _BaseEntity
    {
        public int GardenPresentationId { get; set; }

        [ForeignKey("GardenPresentationId")]
        public virtual GardenPresentation GardenPresentation { get; set; }

        public int ImageOrder { get; set; }

        public int ImageId { get; set; }   // FileToModuleID (GardenImage) 

    }

    public class GardenContactShowStatus : _BaseEntity
    {
        public int ArticleId { get; set; }
        public GardenArticleType ArticleType { get; set; }
        public virtual ICollection<GardenContactList> GardenContactLists { get; set; }

    }


    public class GardenContactList : _BaseEntity
    {
        public int GardenContactShowId { get; set; }
        [ForeignKey("GardenContactShowId")]
        public virtual GardenContactShowStatus GardenContactShowStatus { get; set; }
        public string UserName { get; set; }
        public Guid UserId { get; set; }


    }

    public class GardenContactEditView
    {
        public string UserName { get; set; }
    }

    public enum GardenArticleType
    {
        GardenArchive,
        GardenPresentation
    }

    public enum GardenPresentationPersonMode
    {
        ShowEveryone,
        ShowToAllContact,
        ShowToSelectedContact
    }
}