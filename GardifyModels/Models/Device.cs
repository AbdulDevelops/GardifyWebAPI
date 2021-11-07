using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class Device: _BaseEntity, IReferencedObject
    {
        public Device()
        {
            WarningsAboutThisDevice = new HashSet<UserWarning>();
            Count = 1;
        }
        [Required]
        public string Name { get; set; }
        public bool isActive { get; set; }
        public bool notifyForWind { get; set; }
        public bool notifyForFrost { get; set; }
        public string Note { get; set; }
        [ForeignKey("Garden")]
        public int Gardenid { get; set; }
        [JsonIgnore]
        public virtual Garden Garden { get; set; }
        
        public int? AdminDevId { get; set; }
        
        public int UserDevListId { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public virtual ICollection<UserWarning> WarningsAboutThisDevice { get; set; }

        [ForeignKey("AdminDevId")]
        public virtual AdminDevice AdminDevice { get; set; }

        public ModelEnums.ReferenceToModelClass ReferenceType
        {
            get
            {
                return ModelEnums.ReferenceToModelClass.UserDevice;
            }
        }

    }

    public class DeviceViewModel
    {
        public DeviceViewModel()
        {
            EntryImages = new List<_HtmlImageViewModel>();
        }
        public int Id { get; set; }
        public string Name { get; set; }
        public bool isActive { get; set; }
        public bool notifyForWind { get; set; }
        public bool notifyForFrost { get; set; }
        public string Note { get; set; }
        public int Gardenid { get; set; }
        public int? AdminDevId { get; set; }
        public DateTime CreatedBy { get; set; }
        public DateTime EditedBy { get; set; }
        public int UserDevListId { get; set; }
        public DateTime Date { get; set; }
        public int Count { get; set; }
        public List<_HtmlImageViewModel> EntryImages { get; set; }
        public IEnumerable<Todo> Todos { get; set; }
    }
    

}