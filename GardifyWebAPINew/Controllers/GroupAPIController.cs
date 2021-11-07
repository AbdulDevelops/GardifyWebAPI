using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace GardifyWebAPI.Controllers
{
    [RoutePrefix("api/GroupAPI")]
    public class GroupAPIController : ApiController
    {
        private ApplicationDbContext db = new ApplicationDbContext();
        public GroupsSearchModel GetElements(bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
           
                if (isIos)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromIos, (int)GardifyPages.Search, EventObjectType.PageName);
                }
                else if (isAndroid)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromAndroid, (int)GardifyPages.Search, EventObjectType.PageName);
                }
                else if (isWebPage)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage, (int)GardifyPages.Search, EventObjectType.PageName);
                }


            GroupController gc = new GroupController();
            var res = new GroupsSearchModel();
            res.Groups = gc.DbGetGroups();
            res.GardenGroups = db.GardenCategories.Where(g => !g.Deleted)
                .Select(g => new GardenCategoryViewModel() { Name = g.ParentCategory.Name + " - " + g.Name, Id = g.Id})
                .OrderBy(g => g.Name);
            return res;
        }
    }

    public class GroupsSearchModel
    {
        public IEnumerable<Group> Groups { get; set; }
        public IEnumerable<GardenCategoryViewModel> GardenGroups { get; set; }
    }

    public class GardenCategoryViewModel
    {
        public string Name { get; set; }
        public int Id { get; set; }
    }
}
