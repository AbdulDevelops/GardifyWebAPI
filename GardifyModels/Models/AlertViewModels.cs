using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using static GardifyModels.Models._CustomValidations;

namespace GardifyModels.Models
{
	public class AlertViewModels
	{

        public class UserPlantAlertsViewModel
        {
            public UserPlantAlertsViewModel()
            {
                AlertViewModels = new List<AlertViewModel>();
               
            }
            public int UserPlantId { get; set; }
            public string UserPlantName { get; set; }
            public Boolean IsInPot { get; set; }
            public int Count { get; set; }
            public Plant Plant { get; set; }
            public List<_HtmlImageViewModel> Images { get; set; }
            public List<AlertViewModel> AlertViewModels { get; set; }
           
        }

        public class WarningRelatedObject
        {
            public ModelEnums.ReferenceToModelClass ObjectType { get; set; }
            public string ObjectName { get; set; }
            public string Action { get; set; }
            public int ObjectId { get; set; }
            public int ParentId { get; set; } // PlantId for plants
            public float AlertConditionValue { get; set; }// Float value alertCondition
            public bool IsInPot { get; set; }
            public bool NotifyForWind { get; set; }
            public bool NotifyForFrost { get; set; }

        }


		public class AlertViewModel : _BaseViewModel
		{
            
            public int Id { get; set; }
			[Required]
			public int RelatedObjectId { get; set; }
			[Required]
			public ModelEnums.ReferenceToModelClass ObjectType { get; set; }
			[Required]
			[_Title]
			public string Title { get; set; }
            [Required]
            [_Description]
			public string Text { get; set; }
			public string ReadableCondition { get; set; }
			public IEnumerable<AlertTriggerViewModel> Triggers { get; set; }
			public bool IsTrue { get; set; }
            public List<AlertConditionViewModel> AlertConditionViewModel { get; set; }
            public int TriggerId { get; set; }
        }

        public class AlertLiteViewModel
        {
            public int Id { get; set; }
            public string Title { get; set; }
            public string Text { get; set; }
            public string RelatedObject { get; set; }
            public int RelatedObjectId { get; set; }
            public ModelEnums.ReferenceToModelClass ObjectType { get; set; }
            public ModelEnums.AlertType AlertType { get; set; }
            public ICollection<AlertTrigger> Trigger { get; set; }
        }

        public class AlertTriggerViewModel
		{
			public int Id { get; set; }
			public int AlertId { get; set; }
			public string ReadableCondition { get; set; }
			public IEnumerable<AlertConditionViewModel> Conditions { get; set; }
			public bool IsTrue { get; set; }
		}

		public class AlertConditionViewModel
		{
			public int Id { get; set; }
			[Required]
			public int TriggerId { get; set; }
			[Required]
			public ModelEnums.ComparisonOperator ComparisonOperator { get; set; }
			public float? FloatValue { get; set; }
			public DateTime? DateValue { get; set; }
			[Required]
			public ModelEnums.ComparedValueType ValueType { get; set; }
			public string ReadableCondition { get; set; }
			public bool IsTrue { get; set; }
            public bool IsActual { get; set; }
		}
    }
}