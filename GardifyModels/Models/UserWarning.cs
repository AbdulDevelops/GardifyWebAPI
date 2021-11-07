using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static GardifyModels.Models.AlertViewModels;
using static GardifyModels.Models.UserPlantViewModels;

namespace GardifyModels.Models
{
    /**
    displayed in the warnings page
    */
    public class UserWarning: _BaseEntity
    {
        public UserWarning()
        {
            AffectedDevices = new HashSet<Device>();
            AffectedPlants = new HashSet<UserPlant>();
        }
        public Guid UserId { get; set; }
        public DateTime ConditionDate { get; set; }     // when frost/storm started
        public ModelEnums.AlertType AlertType { get; set; } 
        public ModelEnums.NotificationType NotificationType { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public float ConditionValue { get; set; }   // frost degree or wind speed etc
        public bool Dismissed { get; set; }
        public virtual ICollection<UserPlant> AffectedPlants { get; set; }
        public virtual ICollection<Device> AffectedDevices { get; set; }
    }

    public class UserWarningViewModel
    {
        public int Id { get; set; }
        public Guid UserId { get; set; }
        public DateTime ConditionDate { get; set; }     // when frost/storm started
        public ModelEnums.AlertType AlertType { get; set; }
        public ModelEnums.NotificationType NotificationType { get; set; }
        public string Body { get; set; }
        public string Title { get; set; }
        public float ConditionValue { get; set; }   // frost degree or wind speed etc
        public bool Dismissed { get; set; }
        //public virtual ICollection<AlertViewModels.WarningRelatedObject> AffectedPlants { get; set; }
        //public virtual ICollection<AlertViewModels.WarningRelatedObject> AffectedDevices { get; set; }
        public virtual WarningRelatedObject WarningRelatedObject { get; set; }
    }

    public class ObjectWarningViewModel
    {
        public int RelatedObjectId { get; set; }
        public string RelatedObjectName { get; set; }
        public bool NotifyForWind { get; set; }
        public bool NotifyForFrost { get; set; }
        public bool IsInPot { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public float AlertConditionValue { get; set; }
        public ModelEnums.ReferenceToModelClass ObjectType { get; set; }
        public bool Dismissed { get; set; }
    }

    public class UserWarningListAfectedObjects
    {
        public IEnumerable<GroupedPlantsVModel> AffectedPlants { get; set; }
        public IEnumerable<GroupedPlantsVModel> AffectedDevices { get; set; }
    }
    public class GroupedPlantsVModel
    {
        public string Name { get; set; }
        public IEnumerable<UserWarningViewModel> AffectedObjects { get; set; }
    }
}