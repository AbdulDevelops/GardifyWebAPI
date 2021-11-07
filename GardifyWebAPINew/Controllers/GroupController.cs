using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GardifyWebAPI.Controllers
{
    public class GroupController : _BaseController
    {
        [NonAction]
        public IEnumerable<Group> DbGetGroups()
        {
            IEnumerable<Group> groups = from c in plantDB.Groups
                         where !c.Deleted
                         orderby c.Name ascending
                         select c;
            groups = groups.Select(g => new Group { Id = g.Id, Name = g.Name });
            return groups;
        }
    }
}