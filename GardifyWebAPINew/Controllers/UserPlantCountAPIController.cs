using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GardifyWebAPI.Controllers
{
    [RoutePrefix("api/UserPlantCountAPI")]

    public class UserPlantCountAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        [HttpGet]
        public IHttpActionResult GetUserPlants()
        {
            var userplants = db.UserPlants.Where(up => !up.Deleted && up.Garden != null);
            List<UserPlant> filteredUsersPlants = new List<UserPlant>();

            foreach(var plant in userplants)
            {
                if (plant.Garden != null)
                {

                  
                    filteredUsersPlants.Add(plant);
                    
                }
            }

            var countsByGarden = filteredUsersPlants
                .GroupBy(u => u.Gardenid)
                .AsEnumerable()
                .Select(u => new UserPlantCount { 
                    Gardenid = u.First().Gardenid,
                    Count = u.Aggregate(0, (acc, x) => acc + x.Count)
                })
                .ToList();

            var currentList = db.UserPlantCounts;


            foreach (var item in countsByGarden)
            {
                var existingItem = currentList.FirstOrDefault(g => g.Gardenid == item.Gardenid);
                if (existingItem != null)
                {

                    
                    //existingItem.UserId = item.Garden.Property.UserId;

                    existingItem.Count = item.Count;
                    if (existingItem.Garden.Property.Deleted)
                    {
                        existingItem.Deleted = true;
                    }
                    
                }
                else
                {
           
                    db.UserPlantCounts.Add(item);
                    if (item.Garden.Property.Deleted)
                    {
                        continue;
                    }
                    item.UserId = item.Garden.Property.UserId;

                }
            }

            
            db.SaveChanges();

            //userPlantToUserList = (from u in plantDB.UserPlantToUserLists where !u.Deleted && u.UserPlant.Gardenid == gardenId && u.UserListId == ListId orderby u.UserPlant.NameLatin select u);

            return Ok("Success");
        }
    }
}
