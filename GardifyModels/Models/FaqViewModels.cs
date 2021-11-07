using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static GardifyModels.Models._CustomValidations;

namespace GardifyModels.Models
{
    public class FaqViewModels
    {
        public class FaqIndexViewModel : _BaseViewModel
        {
            public IEnumerable<FaqDetailsViewModel> FaqList { get; set; }
        }

        public class FaqDetailsViewModel : _BaseViewModel
        {
            public FaqDetailsViewModel()
            {
                EntryImages = new List<_HtmlImageViewModel>();
            }

            public int Id { get; set; }
            public string QuestionText { get; set; }
            public string AnswerText { get; set; }
            public bool IsOpen { get; set; }
            public bool UserAllowsPublishment { get; set; }
            public bool AdminAllowsPublishment { get; set; }
            public int ReferenceId { get; set; }
            public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }
            public DateTime Date { get; set; }
            public string CreatedBy { get; set; }
            public string AnsweredBy { get; set; }
            public string AnswererPlace { get; set; }
            public int AnswererCount { get; set; }
            public int AskerCount { get; set; }
            public int AnswersCount { get; set; }
            public List<_HtmlImageViewModel> EntryImages { get; set; }
            public List<FaqAnswerViewModel> Answers { get; set; }
        }

        public class FaqEditViewModel : _BaseViewModel
        {
            public FaqEditViewModel()
            {
                EntryImages = new List<_HtmlImageViewModel>();
            }
            public int Id { get; set; }
            [_Description]
            public string QuestionText { get; set; }
            [_Description]
            public string AnswerText { get; set; }
            public bool UserAllowsPublishment { get; set; }
            public bool AdminAllowsPublishment { get; set; }
            public int ReferenceId { get; set; }
            public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }
            public List<_HtmlImageViewModel> EntryImages { get; set; }
            public IEnumerable<IReferencedObject> InfoObjects { get; set; }
        }
        public class FaqCreateViewModel : _BaseViewModel
        {
            [_Description]
            public string QuestionText { get; set; }
            public bool UserAllowsPublishment { get; set; }
            public int ReferenceId { get; set; }
            public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }
            public IEnumerable<IReferencedObject> InfoObjects { get; set; }
        }
        public class FaqDeleteViewModel : _BaseViewModel
        {
            public int Id { get; set; }
            public string QuestionText { get; set; }
            public string AnswerText { get; set; }
        }
    }
}