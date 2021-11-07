using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class CommunityAnswer: _BaseEntity
    {
        [Required]
        public Guid AuthorId { get; set; }

        [Required]
        //[StringLength(1024)]
        public string AnswerText { get; set; }

        public bool IsFromAdmin { get; set; }

        [Required]
        [ForeignKey("CommunityPost")]
        public int CommunityEntryId { get; set; }

        public virtual CommunityPost CommunityPost { get; set; }
    }
    public class CommunityFullAnswerViewModel
    {
        public CommunityFullAnswerViewModel()
        {
            AnswerImages = new List<_HtmlImageViewModel>();
            ProfilUrl = new List<_HtmlImageViewModel>();
        }

        public string AnswerText { get; set; }
        public string AutorName { get; set; }
        public DateTime Date { get; set; }
        public List<_HtmlImageViewModel> AnswerImages { get; set; }
        public List<_HtmlImageViewModel> ProfilUrl { get; set; }
        public int AnswerId { get; set; }
        //public bool EnableToEdit { get; set; }
        //public bool IsEdited { get; set; }
        //public string OriginalAnswer { get; set; }
        public bool IsFromAdmin { get; set; }
        
    }

    public class CommunityAnswerViewModel
    {
        public string AnswerText { get; set; }
        public int CommunityEntryId { get; set; }
        public Guid AuthorId { get; set; }
    }
}