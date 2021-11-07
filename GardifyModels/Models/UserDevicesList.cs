using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class UserDevicesList:_BaseEntity
    {
        [Required]
        [StringLength(256)]
        public string Name { get; set; }
        [StringLength(4096)]
        public string Description { get; set; }
        [Required]
        [ForeignKey("Garden")]
        public int GardenId { get; set; }
        public virtual Garden Garden { get; set; }
        public virtual IEnumerable<Device> UserDevices { get; set; }
    }
}
