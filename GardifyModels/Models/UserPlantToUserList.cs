using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{
    public class UserPlantToUserList : _BaseEntity
    {
        public UserPlantToUserList()
        {
            Count = 1;
        }
        [Required]
        public int PlantId { get; set; }
        [ForeignKey("PlantId")]
        public virtual UserPlant UserPlant { get; set; }
        [Required]
        public int UserListId { get; set; }
        [ForeignKey("UserListId")]
        public virtual UserList UserList { get; set; }
        public int Count { get; set; }
    }
    public class UserPlantToUserListViewModel 
    {
        [Required]
        public int PlantId { get; set; }
        public int Count { get; set; }
        public int UserListId { get; set; }
    }
    public class UserPlantToUserListView
    {
        public int UserPlantId { get; set; }
        public int Count { get; set; }
        //public int PlantId { get; set; }
        public int UserListId { get; set; }

        //public int GardenId { get; set; }
    }
}