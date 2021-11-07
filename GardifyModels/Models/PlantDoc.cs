using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class PlantDoc: _BaseEntity
    {
        [Required]
        public Guid QuestionAuthorId { get; set; }
        [Required]
        [StringLength(1024)]
        public string QuestionText { get; set; }
        [Required]
        public bool UserAllowsPublishment { get; set; }
        [Required]
        public bool IsOwnFoto { get; set; }
        [Required(AllowEmptyStrings = true)]
        public string Thema { get; set; }
        [Required]
        public string Headline{ get; set; }
        [Required(AllowEmptyStrings = true)]
        public string Description { get; set; }
        public bool AdminAllowsPublishment { get; set; }
        public string QuestionTextUpdate { get; set; }
        public string DescriptionUpdate { get; set; }
        public string ThemaUpdate { get; set; }
        public bool isEdited { get; set; }
        public bool PublishWithImages { get; set; }
    }
   public class PlantDocEntryModel
    {
        public Guid QuestionAuthorId { get; set; }
       
        public string QuestionText { get; set; }
       
        public bool UserAllowsPublishment { get; set; }
       
        public bool IsOwnFoto { get; set; }
        
        public string Thema { get; set; }
        public string Description { get; set; }
        
    }
    public class PlantDocViewModel
    {
        public Guid QuestionAuthorId { get; set; }
        public PlantDocViewModel()
        {
            Images = new List<_HtmlImageViewModel>();
        }
        public _HtmlImageViewModel mainImg { get; set; }
        public string QuestionText { get; set; }

        public bool UserAllowsPublishment { get; set; }

        public bool AdminAllowsPublishment { get; set; }
        public bool IsOwnFoto { get; set; }

        public string Thema { get; set; }

        public string Headline { get; set; }
        public string Description { get; set; }
        public DateTime PublishDate { get; set; }
        public List<_HtmlImageViewModel> Images { get; set; }
        public int QuestionId { get; set; }
        public int TotalAnswers { get; set; }
        public int SeenAnswers { get; set; }
        public bool isEdited { get; set; }
    }
    public class PlantDocDetailViewModel
    {
        public PlantDocViewModel PlantDocViewModel { get; set; }
        public IEnumerable<PlantDocAnswerViewModel> PlantDocAnswerList { get; set; }
        public string NewAnswer { get; set; }
        public string Thema { get; set; }
    }
    public class AdminPlantDocViewModel
    {
        public Guid QuestionAuthorId { get; set; }
     
        public AdminPlantDocViewModel()
        {
            Images = new List<_HtmlImageViewModel>();
        }
        public string QuestionText { get; set; }

        public bool UserAllowsPublishment { get; set; }

        public bool IsOwnFoto { get; set; }

        public string Thema { get; set; }

        public string Headline { get; set; }
        public string Description { get; set; }
        public DateTime PublishDate { get; set; }
        public List<_HtmlImageViewModel> Images { get; set; }
        public int QuestionId { get; set; }
        public bool AdminAllowsPublishment { get; set; }
        public bool Answer { get; set; }
        public bool isEdited { get; set; }
        public string ThemaUpdate { get; set; }
        public string DescriptionUpdate { get; set; }
        public string QuestionTextUpdate { get; set; }
        public bool PublishWithImages { get; set; }
        public DateTime Editdate { get; set; }
        public string AuthorName { get; set; }
    }
    public class AdminPlantDocListViewModel
    {
        public IEnumerable<AdminPlantDocViewModel> ListEntries { get; set; }
      
    }
    
}