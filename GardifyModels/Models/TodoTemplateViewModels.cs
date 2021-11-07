using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using static GardifyModels.Models._CustomValidations;

namespace GardifyModels.Models
{
    public class TodoTemplateViewModels
    {
        public class TodoTemplateIndexViewModel : _BaseViewModel
        {
            public IEnumerable<TodoTemplateDetailsViewModel> TodoTemplates { get; set; }
        }
        public class TodoTemplateDetailsViewModel : _BaseViewModel
        {
            public int Id { get; set; }
            [Required]
            [StringLength(4096)]
            public string Description { get; set; }
            [Required]
            [StringLength(256)]
            public string Title { get; set; }
            public DateTime DateStart { get; set; }
            public DateTime DateEnd { get; set; }
            public ModelEnums.TodoCycleType Cycle { get; set; }
            public int ReferenceId { get; set; }
            public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }
            [NotMapped]
            public string ReferenceName { get; set; }
        }
        public class TodoTemplateCreateViewModel : _BaseViewModel
        {
            [Required]
            [_Title]
            public string Title { get; set; }
            [Required]
            [_Description]
            public string Description { get; set; }
            public DateTime DateStart { get; set; }
            public DateTime DateEnd { get; set; }
            public ModelEnums.TodoCycleType Cycle { get; set; }
            public int[] ReferenceId { get; set; }
            [Required]

            public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }
            public IEnumerable<IReferencedObject> InfoObjects { get; set; }
            public IEnumerable<TaxonomicTree> TaxonomicTreeObjects { get; set; }
            public int SelectedPlantId { get; set; }
            public string SelectedPlantName { get; set; }
            public int SelectedTaxonId { get; set; }

            public int TaxonomicTreeId { get; set; }
            public int Index { get; set; }
            public int DateStartDay { get; set; }
            public int DateStartMonth { get; set; }
            public int DateEndDay { get; set; }
            public int DateEndMonth { get; set; }
        }
        public class TodoTemplateEditViewModel : _BaseViewModel
        {
            public int Id { get; set; }
            [Required]
            [_Title]
            public string Title { get; set; }
            [Required]
            [_Description]
            public string Description { get; set; }
            public DateTime DateStart { get; set; }
            public DateTime DateEnd { get; set; }
            public ModelEnums.TodoCycleType Cycle { get; set; }
            public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }
            public int ReferenceTypeValue { get; set; }

            public int[] ReferenceId { get; set; }
            public IEnumerable<IReferencedObject> InfoObjects { get; set; }
            public int DateStartDay { get; set; }
            public int DateStartMonth { get; set; }
            public int DateEndDay { get; set; }
            public int DateEndMonth { get; set; }
        }

        public class TodoTemplateBulkAddViewModel : _BaseViewModel
        {
            public int BaseId { get; set; }

            public int[] ReferenceId { get; set; }

        }

        public class TodoTemplateBulkViewModel : _BaseViewModel
        {
            public int Id { get; set; }
            [Required]
            [_Title]
            public string Title { get; set; }
            [Required]
            [_Description]
            public string Description { get; set; }
            public DateTime DateStart { get; set; }
            public DateTime DateEnd { get; set; }
            public ModelEnums.TodoCycleType Cycle { get; set; }
            public ModelEnums.ReferenceToModelClass ReferenceType { get; set; }
            public int[] ReferenceId { get; set; }
            public IEnumerable<IReferencedObject> InfoObjects { get; set; }
            public int DateStartDay { get; set; }
            public int DateStartMonth { get; set; }
            public int DateEndDay { get; set; }
            public int DateEndMonth { get; set; }
            public List<TaxonomicRelationTodoItemModel> BulkTodoTemplates { get; set; }
        }

        public class TodoTemplateDeleteViewModel : _BaseViewModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
        }
    }
}