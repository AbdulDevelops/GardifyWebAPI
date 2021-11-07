using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class UserListViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int GardenId { get; set; }
        public bool ListSelected { get; set; }
    }
}