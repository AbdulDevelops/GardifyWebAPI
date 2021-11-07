using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class CommunityPost: _BaseEntity
    {
        [Required]
        public Guid QuestionAuthorId { get; set; }


        [Required]
        [StringLength(1024)]
        public string QuestionText { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string Thema { get; set; }
        public bool IsOnlyExpert { get; set; }

        public bool PublishWithImages { get; set; }

    }

    public class CommunityPostEntryModel
    {
        public Guid QuestionAuthorId { get; set; }

        public string QuestionText { get; set; }

        public bool IsOnlyExpert { get; set; }

        public string Thema { get; set; }
        //public string Description { get; set; }

    }

    public class CommunityPostViewModel 
    {
        public int QuestionId { get; set; }

        public Guid QuestionAuthorId { get; set; }
        public string QuestionAuthorName { get; set; }

        public string QuestionText { get; set; }
        public string Thema { get; set; }

        public DateTime PublishDate { get; set; }
        public List<_HtmlImageViewModel> Images { get; set; }
        public bool IsOnlyExpert { get; set; }

        public string AutorName { get; set; }


    }

    public class CommunityPostWithAnswerViewModel
    {
        public CommunityPostWithAnswerViewModel()
        {

            AutorProfilUrl = new List<_HtmlImageViewModel>();
        }
        public List<_HtmlImageViewModel> AutorProfilUrl { get; set; }
        public CommunityPostViewModel Post { get; set; }
        public IEnumerable<CommunityFullAnswerViewModel> CommunityAnswerList { get; set; }
    }

    public class CommunityPostDetailViewModel
    {
        public CommunityPostViewModel communityPostViewModel { get; set; }
        public IEnumerable<CommunityFullAnswerViewModel> communityAnswerList { get; set; }
        public string NewAnswer { get; set; }
        public string Thema { get; set; }
    }
}