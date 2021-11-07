using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class FaqAnswer : _BaseEntity
    {
        [Required]
        public Guid AuthorId { get; set; }
        [Required]
        [StringLength(1024)]
        public string AnswerText { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        [ForeignKey("FaqEntry")]
        public int FaqEntryId { get; set; }
        public virtual FaqEntry FaqEntry { get; set; }
    }

    public class FaqAnswerViewModel
    {
        public string AnswerText { get; set; }
        public DateTime Date { get; set; }
        public string AnsweredBy { get; set; }
        public int UserPostsCount { get; set; }
        public IEnumerable<_HtmlImageViewModel> Images { get; set; }
    }
}