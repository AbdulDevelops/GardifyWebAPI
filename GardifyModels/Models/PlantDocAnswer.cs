using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class PlantDocAnswer : _BaseEntity
    {

        [Required]
        public Guid AuthorId { get; set; }
        [Required]
        //[StringLength(1024)]
        public string AnswerText { get; set; }
        [Required]
        [ForeignKey("PlantDoc")]
        public int PlantDocEntryId { get; set; }

        public virtual PlantDoc PlantDoc { get; set; }
        public bool isAlreadyReaded { get; set; }
        public bool IsEdited { get; set; }
        public string UpdatedAnswer { get; set; }
    }
    public class PlantDocAnswerViewModel
    {
        public PlantDocAnswerViewModel()
        {
            AnswerImages = new List<_HtmlImageViewModel>();
        }
        public string AnswerText { get; set; }
        public string AutorName { get; set; }
        public DateTime Date { get; set; }
        public List<_HtmlImageViewModel> AnswerImages { get; set; }
        public int AnswerId { get; set; }
        public bool EnableToEdit { get; set; }
        public bool IsEdited { get; set; }
        public string OriginalAnswer { get; set; }
        public bool IsAdminAnswer { get; set; }
    }
    public class AnswerViewModel
    {
        public string AnswerText { get; set; }
        public int PlantDocEntryId { get; set; }
        public Guid AuthorId { get; set; }
    }
    public class AnswersCountViewModel
    {
        public int count { get; set; }
        public int PlantDocEntryId { get; set; }
    }
}