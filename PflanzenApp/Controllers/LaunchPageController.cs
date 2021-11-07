using GardifyModels.Models;
using PflanzenApp.App_Code;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
{
    [CustomAuthorize]

    public class LaunchPageController : _BaseController
    {
        // GET: LaunchPage
        public List<LaunchPageContent> getDbContent()
        {
            var allLaunchPageContent = from l in ctx.LaunchPageContents
                                       where !l.Deleted
                                       select l;


            return allLaunchPageContent.ToList();
        }
    }
}