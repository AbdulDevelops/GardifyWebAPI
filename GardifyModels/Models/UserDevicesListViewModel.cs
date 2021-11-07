using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class UserDevicesListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int GardenId { get; set; }
        public virtual IEnumerable<Device> UserDevices { get; set; }
    }
  

}