using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using static GardifyModels.Models._CustomValidations;

namespace GardifyModels.Models
{
    public class TodoViewModels
    {
        public class TodoIndexViewModel : _BaseViewModel
        {
            public IEnumerable<TodoDetailsViewModel> TodoList { get; set; }
            public DateTime StartDate { get; set; }
            public DateTime EndDate { get; set; }
        }
        public class TodoDetailsViewModel : _BaseViewModel
        {
            public int Id { get; set; }
            public string Description { get; set; }
            public string Title { get; set; }
            public ModelEnums.NotificationType Notification { get; set; }
            public string Notes { get; set; }
            public DateTime DateStart { get; set; }
            public DateTime DateEnd { get; set; }
            public int ReferenceId { get; set; }
            public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }
            public string ReferenceText { get; set; }
            public bool Finished { get; set; }
            public bool Ignored { get; set; }
            public bool Deleted { get; set; }
            public int? CyclicId { get; set; }
            public Guid UserId { get; set; }
            public IEnumerable<VideoReferenceViewModels.VideoReferenceDetailsViewModel> VideoReferenceList { get; set; }
            public IEnumerable<Article> Articles { get; set; }
            public IEnumerable<_HtmlImageViewModel> EntryImages { get; set; }
            public int RelatedPlantId { get; set; }
            public string RelatedPlantName { get; set; }
            public DateTime CyclicDateEnd { get; set; }
            public int RelatedTodoTemplateId { get; set; }
        }

        public class TodoCountViewModel
        {
            public int Finished { get; set; }
            public int Open { get; set; }
            public int AllTodos { get; set; }
            public int AllTodosOfTheMonth { get; set; }
        }

        public class TodoCreateViewModel : _BaseViewModel
        {
            [Required]
            //[_Title]
            [StringLength(256)]
            public string Title { get; set; }
            [Required]
            //[_Description]
            [StringLength(4096)]
            public string Description { get; set; }
            public string Notes { get; set; }
            public DateTime DateStart { get; set; } // required
            public DateTime DateEnd { get; set; } // required
            public int ReferenceId { get; set; }
            public ModelEnums.TodoCycleType Cycle { get; set; }
            public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }
            public IEnumerable<IReferencedObject> InfoObjects { get; set; }
            public bool GeneratedFromTemplate { get; set; }
            public int RelatedTodoTemplateId { get; set; }
        }
    }
}