using GardifyModels.Models;
using Microsoft.Ajax.Utilities;
using PflanzenApp.App_Code;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Core.Common.CommandTrees;
using System.Data.Entity.SqlServer;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using System.Web.Mvc;


namespace PflanzenApp.Controllers.AdminArea
{
    [CustomAuthorizeAttribute(Roles = "Admin,Expert")]
    public class AdminAreaStatsController : _BaseController
    {
        private readonly Guid ANONYMOUS_GUID = new Guid("64535925-4fd4-4e51-9001-b0e19f776379");

        private ApplicationDbContext db = new ApplicationDbContext();
        private readonly Calendar calendar = new GregorianCalendar();

        public ActionResult Index(DateTime? startDate, DateTime? endDate, string groupBy = "day", bool demoOnly = false)
        {
            if (startDate == null)
            {
                var lastWeekDat = DateTime.Now.AddDays(-7);
                startDate = new DateTime(lastWeekDat.Year, lastWeekDat.Month, lastWeekDat.Day);
                ViewBag.startDate = ((DateTime)startDate).ToString("yyyy-MM-dd");
            }

            if (endDate == null)
            {
                endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                endDate = ((DateTime)endDate).AddDays(1).AddMinutes(-1);
                ViewBag.endDate = ((DateTime)endDate).ToString("yyyy-MM-dd");
            }
            ViewBag.AdClicks = GroupingToSeriesArray(GroupByPeriod(GetStatisticEntriesPage(StatisticEventTypes.AdClicked, startDate, endDate, demoOnly), groupBy).ToList());

            ViewBag.Orders = GroupingToSeriesArray(GroupByPeriod(GetSuccessfulShopOrders(startDate, endDate), groupBy).ToList(), groupBy);
            ViewBag.PlantsSuggested = GroupingToSeriesArray(GroupByPeriod(GetStatisticEntriesPage(StatisticEventTypes.SuggestPlant, startDate, endDate, demoOnly), groupBy).ToList());
            ViewBag.GuidedTour = GroupingToSeriesArray(GroupByPeriod(GetStatisticEntriesPage(StatisticEventTypes.GuidedTour, startDate, endDate, demoOnly), groupBy).ToList());
            ViewBag.SavedBioScans = GroupingToSeriesArray(GroupByPeriod(GetStatisticEntriesPage(StatisticEventTypes.SaveBioScan, startDate, endDate, demoOnly), groupBy).ToList());
            ViewBag.TodoDiverseEntries = TodoStatEntries();

            // These 2 have more complicated logic and don't use SeriesArray, possibly try to abstract them like the rest
            ViewBag.Pageviews = GetPageViews(startDate, endDate, groupBy, demoOnly);
            ViewBag.PlantDoc = GetPlantDocData(startDate, endDate, groupBy, demoOnly);
            ViewBag.DemoOnly = demoOnly.ToString();
            getPlantsSuggests(startDate, endDate, demoOnly, groupBy);
            getQuestionsPlantsDoc(startDate, endDate, demoOnly, groupBy);
            getAnswersPlantsDoc(startDate, endDate, demoOnly, groupBy);



            return View("~/Views/AdminArea/AdminAreaStats/Index.cshtml");
        }

        public ActionResult GetUsersStats(DateTime? startDate, DateTime? endDate, string groupBy = "day", bool demoOnly = false)
        {
            if (startDate == null)
            {
                var lastWeekDat = DateTime.Now.AddDays(-7);
                startDate = new DateTime(lastWeekDat.Year, lastWeekDat.Month, lastWeekDat.Day);
                ViewBag.startDate = ((DateTime)startDate).ToString("yyyy-MM-dd");
            }

            if (endDate == null)
            {
                endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                endDate = ((DateTime)endDate).AddDays(1).AddMinutes(-1);
                ViewBag.endDate = ((DateTime)endDate).ToString("yyyy-MM-dd");
            }
            var groupedItem = GroupByPeriod(GetStatisticEntriesConvertedReg(demoOnly ? StatisticEventTypes.RegisterConverted : StatisticEventTypes.Login, startDate, endDate, demoOnly), groupBy).ToList();

           
            ViewBag.ConvertedReg = GroupingToSeriesArray(groupedItem, groupBy,  true);



            //ViewBag.PlantsCount = GetPlantsCount(demoOnly);
            //ViewBag.PlantsCount = 0;

            ViewBag.RegUsers = GroupingToSeriesArray(GroupByPeriod(GetStatisticEntries(StatisticEventTypes.Register, startDate, endDate, demoOnly,true), groupBy).ToList(), groupBy,  true);
           
            ViewBag.CumRegUsers = AccumulateSeriesArray(null, endDate, startDate, groupBy, demoOnly) ;
            if ((ViewBag.RegUsers.Values.Length - 1) >= 0)
            {
                ViewBag.TodayValue = ViewBag.RegUsers.Values[ViewBag.RegUsers.Values.Length - 1];

            }
            if ((ViewBag.RegUsers.Values.Length - 2) >= 0)
            {
                ViewBag.YesterdayValue = ViewBag.RegUsers.Values[ViewBag.RegUsers.Values.Length - 2];

            }
            if ((ViewBag.RegUsers.Values.Length - 3) >= 0)
            {
                ViewBag.BeforeYesterdayValue = ViewBag.RegUsers.Values[ViewBag.RegUsers.Values.Length - 3];

            }
            //ViewBag.CumRegUsersYesterday = ViewBag.CumRegUsers.Values[0];

            if ((ViewBag.CumRegUsers.Values.Length - 1) >= 0)
            {
                ViewBag.CumRegUsersYesterday = ViewBag.CumRegUsers.Values[ViewBag.CumRegUsers.Values.Length - 1];
                ViewBag.CumStartDate = ViewBag.CumRegUsers.Labels[0];

                ViewBag.CumEndDate = ViewBag.CumRegUsers.Labels[ViewBag.CumRegUsers.Values.Length - 1];
            }



            //@ViewBag.BeforeYesterdayValue) / Gestern(@ViewBag.YesterdayValue) / Heute(@ViewBag.TodayValue)

            ViewBag.DemoOnly = demoOnly.ToString();

            ////ViewBag.DemoUsers = GroupingToSeriesArray(GroupByPeriod(GetStatisticEntries(demoOnly ? StatisticEventTypes.RegisterDemo : StatisticEventTypes.Login, startDate, endDate, demoOnly), groupBy), groupBy);

            //ViewBag.UserVisitsAvg = GetUsersStatsAvg(startDate, endDate, groupBy,demoOnly) ;

            return View("~/Views/AdminArea/AdminAreaStats/UsersStats.cshtml");
        }
        public ActionResult GetAllActiveUsers(DateTime? startDate, DateTime? endDate, string groupBy = "month", bool demoOnly = false)
        {
            ViewBag.UserVisits = GetUsersStatsAvg(startDate, endDate, groupBy, demoOnly) ;
            getActivUser();
            return View("~/Views/AdminArea/AdminAreaStats/ActiveUser.cshtml");
        }
        public SeriesArray GetUsersStatsAvg(DateTime? startDate, DateTime? endDate, string groupBy = "month", bool demoOnly = false)
        {
            //var groupby = "month";
            var userVisits = GroupingToSeriesArray(GroupByPeriod(GetDailyUniqueUserStatisticEntries(startDate, endDate, demoOnly, groupBy), groupBy).ToList(), groupBy);

            return userVisits;
        }
        public ActionResult GetUserStatsFromAppAndWepPage(DateTime? startDate, DateTime? endDate, string groupBy = "day", bool demoOnly = false)
        {
            if (startDate == null)
            {
                var lastWeekDat = DateTime.Now.AddDays(-7);
                startDate = new DateTime(lastWeekDat.Year, lastWeekDat.Month, lastWeekDat.Day);
                ViewBag.startDate = ((DateTime)startDate).ToString("yyyy-MM-dd");
            }

            if (endDate == null)
            {
                endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);
                endDate = ((DateTime)endDate).AddDays(1).AddMinutes(-1);
                ViewBag.endDate = ((DateTime)endDate).ToString("yyyy-MM-dd");
            }
            getUserReg(startDate, endDate, demoOnly, groupBy);
            getUsersLog(startDate, endDate, demoOnly, groupBy);
            return View("~/Views/AdminArea/AdminAreaStats/UserRegAppWebPage.cshtml");
        }
        public SeriesArray GetPlantsCount(bool demoOnly=false)
        {
            double[] filterSizes = new double[] { 5, 10, 20, 50, 100 };

            var seriesArray = new SeriesArray() { Labels = filterSizes.Select(s => $">= {s}").ToArray() };
            List<double> values = new List<double>();
            var userplants = ctx.UserPlants.Where(up => !up.Deleted && up.Garden != null);
            //var userplants = ctx.UserPlants.Where(up => !up.Deleted );
            var userPlantCounts = ctx.UserPlantCounts.Where(up => !up.Deleted).DistinctBy(p => p.UserId);

            var deletedUser = db.Users.Where(u => u.Deleted).ToList();
            var demoUser = db.Users.Where(u => !u.Deleted && !u.CompleteSignup && u.Email.Contains("UserDemo")).ToList();


            List<UserPlantCount> filteredUsersPlants = new List<UserPlantCount>();
            foreach (UserPlantCount plant in userPlantCounts.ToList())
            {


                var relateduserId = plant.Garden.Property.UserId.ToString();
                var relatedgardenId = GetUserGardenId(plant, deletedUser, demoUser, demoOnly);
                if (relatedgardenId != 0)
                {
                    filteredUsersPlants.Add(plant);
                }

            }
            if (filteredUsersPlants != null)
            {

                foreach (var size in filterSizes)
                {
                    values.Add(filteredUsersPlants.Where(u => u.Count >= size).Count());
                }
            }

            seriesArray.Values = values.ToArray();
            return seriesArray;
        }
        public void getQuestionsPlantsDoc(DateTime? startDate = null, DateTime? endDate = null, bool demoOnly = false, string groupBy = "day")
        {
            var userActions = groupedStatsEntries((int)StatisticEventTypes.SubmitQuestion, startDate, endDate, demoOnly, groupBy);

            List<int> submitQuestionIos = new List<int>();
            List<int> submitQuestionWebPage = new List<int>();
            List<int> submitQuestionAndroid = new List<int>();
            if(userActions.groupedActions != null &&  userActions.groupedActions.Any() )
            {
                foreach (var i in userActions.groupedActions)
                {
                    submitQuestionIos.Add(i.ToList().Where(e => e.ApiCallEventId == (int)StatisticEventTypes.ApiCallFromIos).Count());
                    submitQuestionWebPage.Add(i.ToList().Where(e => e.ApiCallEventId != (int)StatisticEventTypes.ApiCallFromIos && e.ApiCallEventId != (int)StatisticEventTypes.ApiCallFromAndroid).Count());
                    submitQuestionAndroid.Add(i.ToList().Where(e => e.ApiCallEventId == (int)StatisticEventTypes.ApiCallFromAndroid).Count());

                }
            }
            
            ViewBag.SubmitQuestionIos = submitQuestionIos.ToArray();
            ViewBag.SubmitQuestionWebPage = submitQuestionWebPage.ToArray();
            ViewBag.SubmitQuestionAndroid = submitQuestionAndroid.ToArray();

            ViewBag.SubmitQuestionLabels = userActions.Labels;
        }
        public void getAnswersPlantsDoc(DateTime? startDate = null, DateTime? endDate = null, bool demoOnly = false, string groupBy = "day")
        {
            var userActions = groupedStatsEntries((int)StatisticEventTypes.SubmitAnswer, startDate, endDate, demoOnly, groupBy);

            List<int> submitAnswerIos = new List<int>();
            List<int> submitAnswerAndroid = new List<int>();
            List<int> submitAnswerWebPage = new List<int>();
            if (  userActions.groupedActions != null && userActions.groupedActions.Any())
            {
                foreach (var i in userActions.groupedActions)
                {
                    submitAnswerIos.Add(i.ToList().Where(e => e.ApiCallEventId == (int)StatisticEventTypes.ApiCallFromIos).Count());
                    submitAnswerAndroid.Add(i.ToList().Where(e => e.ApiCallEventId == (int)StatisticEventTypes.ApiCallFromAndroid).Count());
                    submitAnswerWebPage.Add(i.ToList().Where(e => e.ApiCallEventId != (int)StatisticEventTypes.ApiCallFromIos && e.ApiCallEventId != (int)StatisticEventTypes.ApiCallFromAndroid).Count());

                }
            }
               
            ViewBag.SubmitAnswerIos = submitAnswerIos.ToArray();
            ViewBag.SubmitAnswerWebPage = submitAnswerWebPage.ToArray();
            ViewBag.SubmitAnswerAndroid = submitAnswerAndroid.ToArray();

            ViewBag.SubmitAnswerLabels = userActions.Labels;
        }
        public void getPlantsSuggests(DateTime? startDate = null, DateTime? endDate = null, bool demoOnly = false, string groupBy = "day")
        {
            var userActions = groupedStatsEntries((int)StatisticEventTypes.SuggestPlant, startDate, endDate, demoOnly, groupBy);

            List<int> suggestPlantIos = new List<int>();
            List<int> suggestPlantWebPage = new List<int>();
            List<int> suggestPlantAndroid = new List<int>();
            if (userActions.groupedActions != null && userActions.groupedActions.Any())
            {
                foreach (var i in userActions.groupedActions)
                {
                    suggestPlantIos.Add(i.ToList().Where(e => e.ApiCallEventId == (int)StatisticEventTypes.ApiCallFromIos).Count());
                    suggestPlantWebPage.Add(i.ToList().Where(e => e.ApiCallEventId != (int)StatisticEventTypes.ApiCallFromIos && e.ApiCallEventId != (int)StatisticEventTypes.ApiCallFromAndroid).Count());
                    suggestPlantAndroid.Add(i.ToList().Where(e => e.ApiCallEventId == (int)StatisticEventTypes.ApiCallFromAndroid).Count());

                }
            }
               
            ViewBag.SuggestPlantIos = suggestPlantIos.ToArray();
            ViewBag.SuggestPlantWebPage = suggestPlantWebPage.ToArray();
            ViewBag.SuggestPlantAndroid = suggestPlantAndroid.ToArray();

            ViewBag.SuggestPlantsLabels = userActions.Labels;
        }
        public void getActivUser()
       {
            List<double> values = new List<double>();
            var deletedUsers = db.Users.Where(u => u.Deleted).ToList();
            var anothervalue = GroupByPeriod(GetDailyUniqueUserStatisticEntries(null, null, false, "month"), "month").ToList();
            var lastValue = GroupingToSeriesArray(anothervalue, "month");
            //GroupByPeriod(GetDailyUniqueUserStatisticEntries(startDate, endDate, demoOnly, groupBy), groupBy).ToList()
            var usersTemp = (from u in db.StatisticEntries where !u.DemoMode select u);
            //var users = (from s in db.StatisticEntries
            //                where  !s.DemoMode)
            //                select s);
            var users = usersTemp.AsEnumerable().Where(d => deletedUsers.FirstOrDefault(u => u.Id == d.UserId.ToString()) == null);

            //var filterdUsers=users.Where(d => deletedUsers.FirstOrDefault(u => u.Id == d.UserId.ToString()) == null);
            var groupedUsers = users.ToList().GroupBy(g => new { g.UserId, g.Date.Date }).Select(s => new { date = s.First().Date.Date, userId = s.Key.UserId, count = s.Count() }).ToList();
            var groupByMonth=groupedUsers. GroupBy(i => new { i.date.Month,i.date.Year });
            List<int> lessActivesUsers = new List<int>();
            List<int> occasActivUsers = new List<int>();
            List<int> activesUsers = new List<int>();
            List<int> mostActivesUsers = new List<int>();

            string[] labels = groupByMonth.Select(g => g.First().date.Date.ToString("MMM yyyy", CultureInfo.InvariantCulture)).ToArray();
             foreach(var i in groupByMonth) 
            {
                lessActivesUsers.Add(i.ToList().Where(e => e.count <= 1).Count());
                occasActivUsers.Add(i.ToList().Where(e => e.count >=2 && e.count <= 4).Count());
                activesUsers.Add(i.ToList().Where(e => e.count >= 5 && e.count <= 9).Count());
                mostActivesUsers.Add(i.ToList().Where(e => e.count >= 10).Count());
            }
            ViewBag.LessActivesUsers = lessActivesUsers.ToArray();
            ViewBag.OccasActivUsers = occasActivUsers.ToArray();
            ViewBag.ActivesUsers = activesUsers.ToArray();
            ViewBag.MostActivesUsers=mostActivesUsers.ToArray();
            ViewBag.Labels = labels;
        }
        public void getUserReg(DateTime? startDate = null, DateTime? endDate = null, bool demoOnly = false, string groupBy = "day")
        {
            var userActions = groupedStatsEntries((int)StatisticEventTypes.Register, startDate, endDate, demoOnly, groupBy);
            
            List<int> registerIos = new List<int>();
            List<int> registerWebPage = new List<int>();
            List<int> registerAndroid = new List<int>();
            if ( userActions.groupedActions != null && userActions.groupedActions.Any())
            {
                foreach (var i in userActions.groupedActions)
                {
                    registerIos.Add(i.ToList().Where(e => e.ApiCallEventId == (int)StatisticEventTypes.ApiCallFromIos).Count());
                    registerWebPage.Add(i.ToList().Where(e => e.ApiCallEventId != (int)StatisticEventTypes.ApiCallFromIos && e.ApiCallEventId != (int)StatisticEventTypes.ApiCallFromAndroid).Count());
                    registerAndroid.Add(i.ToList().Where(e => e.ApiCallEventId == (int)StatisticEventTypes.ApiCallFromAndroid).Count());

                }
            }
               
            ViewBag.RegisterIos = registerIos.ToArray();
            ViewBag.RegisterWebPage = registerWebPage.ToArray();
            ViewBag.RegisterAndroid = registerAndroid.ToArray();
            
            ViewBag.RegLabels = userActions.Labels;

        }
        public void getUsersLog( DateTime? startDate = null, DateTime? endDate = null, bool demoOnly = false, string groupBy = "day")
        {
            var userActions = groupedStatsEntries((int)StatisticEventTypes.Login, startDate, endDate, demoOnly, groupBy);
           
            List<int> loginIos = new List<int>();
            List<int> loginWebPage = new List<int>();
            List<int> loginAndroid = new List<int>();
            if ( userActions.groupedActions != null && userActions.groupedActions.Any() )
            {
                foreach (var i in userActions.groupedActions)
                {
                    loginIos.Add(i.ToList().Where(e => e.ApiCallEventId == (int)StatisticEventTypes.ApiCallFromIos).Count());
                    loginWebPage.Add(i.ToList().Where(e => e.ApiCallEventId != (int)StatisticEventTypes.ApiCallFromIos && e.ApiCallEventId != (int)StatisticEventTypes.ApiCallFromAndroid).Count());
                    loginAndroid.Add(i.ToList().Where(e => e.ApiCallEventId == (int)StatisticEventTypes.ApiCallFromAndroid).Count());

                }
            }
               
            ViewBag.LoginIos = loginIos.ToArray();
            ViewBag.LoginWebPage = loginWebPage.ToArray();
            ViewBag.LoginAndroid = loginAndroid.ToArray();
            ViewBag.LogLabels = userActions.Labels;

        }


        public UsersActionsOnDevices groupedStatsEntries( int eventType,DateTime? startDate = null, DateTime? endDate = null, bool demoOnly = false, string groupBy = "day")
        {

            List<double> values = new List<double>();
            if (!startDate.HasValue) { startDate = default(DateTime); }
            if (!endDate.HasValue) { endDate = DateTime.Now; }
            var stats = from u in db.StatisticEntries where u.Date >= startDate && u.Date <= endDate
                                        && (demoOnly ? u.DemoMode : !u.DemoMode) 
                                         && u.EventId==eventType && (
                                         u.ApiCallEventId==(int)StatisticEventTypes.ApiCallFromIos 
                                        || u.ApiCallEventId== (int)StatisticEventTypes.ApiCallFromAndroid 
                                        || u.ApiCallEventId== (int)StatisticEventTypes.ApiCallFromWebpage) select u;

            var deletedUsers = db.Users.Where(u => u.Deleted).ToList();


            //var statEventLogType = stats.AsEnumerable().Where(st => CheckIfUserIsDeleted(st.UserId.ToString())!=null?st.UserId.ToString()== CheckIfUserIsDeleted(st.UserId.ToString()):true);
            var statEventLogType = stats.AsEnumerable().Where(st => deletedUsers.FirstOrDefault(u => u.Id == st.UserId.ToString()) == null).OrderBy(d=>d.Date).ToList();

            IEnumerable<IEnumerable<StatisticEntry>> groupedData = null;
            string[] labels = null;
            if (statEventLogType.Any() && statEventLogType != null)
                switch (groupBy)
                {
                    case "week":
                        groupedData = statEventLogType.ToList().GroupBy(i => new { i.Date.Year, i.Date.Month, Week = GetGregorianWeekOfYear(i.Date) });

                        var groupedDataWhithKey = statEventLogType.ToList().GroupBy(i => new { i.Date.Year, i.Date.Month, Week = GetGregorianWeekOfYear(i.Date) });

                        if (groupedDataWhithKey != null && groupedDataWhithKey.Any())
                            labels = groupedDataWhithKey.Select(g => g.First().Date.ToString($"yyyy/{g.Key.Week}")).ToArray();

                        break;
                    case "month":
                        groupedData = statEventLogType.GroupBy(i => new { i.Date.Month, i.Date.Year }).ToList();
                        labels = groupedData.Select(g => g.First().Date.ToString("MMM yyyy", CultureInfo.InvariantCulture)).ToArray();
                        break;
                    case "year":
                        groupedData = statEventLogType.GroupBy(i => new { i.Date.Year }).ToList();
                        labels = groupedData.Select(g => g.First().Date.ToString("yyyy", CultureInfo.InvariantCulture)).ToArray();
                        break;
                    default:
                        groupedData = statEventLogType.GroupBy(i => new { i.Date.Year, i.Date.Month, i.Date.Day }).ToList();
                        labels = groupedData.Select(g => g.First().Date.ToString("dd MMM yy", CultureInfo.InvariantCulture)).ToArray();
                        break;
                }
            return new UsersActionsOnDevices { groupedActions=groupedData, Labels=labels};
        }
        public string CheckIfUserIsDeleted(string userId)
        {
            var userIsDeleted = db.Users.Find(userId);
            if (userIsDeleted!=null && !userIsDeleted.Deleted)
            {
                return userIsDeleted.Id.ToString();
            }
            return null;
            
        }
        public string CheckIfDemoAccount(string userId)
        {
            var isDemoAccount = db.Users.Find(userId);
            if (isDemoAccount != null && !isDemoAccount.Deleted  && !isDemoAccount.CompleteSignup)
            {
                return isDemoAccount.Id.ToString();
            }
            return null;

        }
        public int GetUserGardenId(UserPlantCount userplant, List<ApplicationUser> deletedUser, List<ApplicationUser> demoUser, bool demoOnly = false)
        {
            var userNotDeleted = deletedUser.FirstOrDefault(u => u.Id.ToString() == userplant.UserId.ToString());
            if (demoOnly)
            {
                var demoUserTemp = demoUser.FirstOrDefault(u => u.Id.ToString() == userplant.UserId.ToString());
                if (demoUserTemp != null)
                {
        

                    return userplant.Gardenid;
                }
            }
            else
            {
                var demoUserTemp = demoUser.FirstOrDefault(u => u.Id.ToString() == userplant.UserId.ToString());
                if (userNotDeleted == null && demoUserTemp == null)
                {
                    return userplant.Gardenid;

                }
            }
           
            return 0;
                
            
        }
        public List<StatisticEntry> GetStatisticEntries(StatisticEventTypes? eventType = null, DateTime? startDate = null, DateTime? endDate = null, bool demoOnly = false, bool includeletedUsers=false)
        {
            if (!startDate.HasValue) { startDate = default(DateTime); }
            if (!endDate.HasValue) { endDate = DateTime.Now; }
            IQueryable<StatisticEntry> dataFiltered = null;
            if(eventType== StatisticEventTypes.RegisterDemo)
            {
                
                dataFiltered = (from s in db.StatisticEntries
                                   where (s.Date >= startDate && s.Date <= endDate && (eventType.HasValue ? s.EventId == (int)eventType.Value : true) 
                                   && s.DemoMode==demoOnly)
                                   || (s.Date >= startDate && s.Date <= endDate && s.EventId == (int)StatisticEventTypes.Register
                                   && s.DemoMode == demoOnly )
                                  select s);
            }
            else
            {
                dataFiltered = (from s in db.StatisticEntries
                               where s.Date >= startDate && s.Date <= endDate 
                               && (eventType.HasValue ? s.EventId == (int)eventType.Value : true) && (demoOnly ? s.DemoMode : !s.DemoMode)
                               select s);
            }
            
            if (includeletedUsers)
            {
                return dataFiltered.ToList();
            }
            else
            {
                var deletedUsers = db.Users.Where(u => u.Deleted).ToList();
  

                var rslt = dataFiltered.AsEnumerable().Where(d => deletedUsers.FirstOrDefault(u => u.Id == d.UserId.ToString()) == null);
                //var rslt = dataFiltered.AsEnumerable().Where(st => CheckIfUserIsDeleted(st.UserId.ToString()) != null ? st.UserId.ToString() == CheckIfUserIsDeleted(st.UserId.ToString()) : true);
                return rslt.ToList();
            }
             
            
        }
        public List<StatisticEntry> GetStatisticEntriesPage(StatisticEventTypes? eventType = null, DateTime? startDate = null, DateTime? endDate = null, bool demoOnly = false)
        {
            if (!startDate.HasValue) { startDate = default(DateTime); }
            if (!endDate.HasValue) { endDate = DateTime.Now; }
            List<StatisticEntry> dataFiltered = null;
            if (eventType == StatisticEventTypes.RegisterDemo)
            {

                dataFiltered = (from s in db.StatisticEntries
                                where (s.Date >= startDate && s.Date <= endDate && (eventType.HasValue ? s.EventId == (int)eventType.Value : true)
                                && s.DemoMode == demoOnly)
                                || (s.Date >= startDate && s.Date <= endDate && s.EventId == (int)StatisticEventTypes.Register
                                && s.DemoMode == demoOnly)
                                select s).ToList();
            }
            else
            {
                dataFiltered = (from s in db.StatisticEntries
                                where s.Date >= startDate && s.Date <= endDate
                                && (eventType.HasValue ? s.EventId == (int)eventType.Value : true) && (demoOnly ? s.DemoMode : !s.DemoMode)
                                select s).ToList();
            }


            return dataFiltered;


        }

        public List<StatisticEntry> GetStatisticEntriesConvertedReg(StatisticEventTypes? eventType = null, DateTime? startDate = null, DateTime? endDate = null, bool demoOnly = false)
        {
            if (!startDate.HasValue) { startDate = default(DateTime); }
            if (!endDate.HasValue) { endDate = DateTime.Now; }

            var dataFiltered = (from s in db.StatisticEntries
                               where s.Date >= startDate && s.Date <= endDate 
                               
                               && (s.EventId == (int)StatisticEventTypes.Register)
                               select s);

            var groupedData= dataFiltered.ToList().GroupBy(d => d.UserId).Where(u => u.Count() > 1).Select(x=>x.FirstOrDefault()).ToList();
            var convertedReg = (from s in db.StatisticEntries
                               where s.Date >= startDate && s.Date <= endDate 
                               
                               && ((eventType.HasValue ? s.EventId == (int)eventType.Value : true) && !s.DemoMode)
                               select s);
            var result = groupedData.Concat(convertedReg.ToList());
            var deletedUsers = db.Users.Where(u => u.Deleted).ToList();
            //return result.AsEnumerable().Where(st => CheckIfUserIsDeleted(st.UserId.ToString()) != null ? st.UserId.ToString() == CheckIfUserIsDeleted(st.UserId.ToString()) : true).AsQueryable();

            return result.Where(d => deletedUsers.FirstOrDefault(u => u.Id == d.UserId.ToString()) == null).ToList();
        }
        public List<StatisticEntry> GetDailyUniqueUserStatisticEntries(DateTime? startDate, DateTime? endDate, bool demoOnly = false, string groupBy = "day")
        {
            var data = GetStatisticEntries(startDate: startDate, endDate: endDate, demoOnly: demoOnly).Where(x => x.UserId != ANONYMOUS_GUID);
            switch (groupBy)
            {
                case "week":
                    return data.ToList().GroupBy(i => new { i.Date.Year, i.Date.Month, Week = GetGregorianWeekOfYear(i.Date), i.UserId }).Select(g => g.FirstOrDefault()).ToList();
                case "month":
                    return data.GroupBy(i => new { i.Date.Year, i.Date.Month, i.UserId }).Select(g => g.FirstOrDefault()).ToList();
                case "year":
                    return data.GroupBy(i => new { i.Date.Year, i.UserId }).Select(g => g.FirstOrDefault()).ToList();
                default:
                    return data.GroupBy(i => new { i.Date.Year, i.Date.Month, i.Date.Day, i.UserId }).Select(g => g.FirstOrDefault()).ToList();
            }
        }
       

        public IQueryable<ShopOrder> GetSuccessfulShopOrders(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!startDate.HasValue) { startDate = default(DateTime); }
            if (!endDate.HasValue) { endDate = DateTime.Now; }
            var data = from p in db.ShopOrders
                       where ((p.PaymentConfirmed && p.TCResponseCode == "Y") || (p.PaidWith == "rechnung" && p.OrderConfirmed)) && p.CreatedDate >= startDate && p.CreatedDate <= endDate
                       select p;

            return data;
        }

        public IEnumerable<IGrouping<dynamic, StatisticEntry>> GroupByPeriod(List<StatisticEntry> data, string groupBy = "day")
        {
            
            switch (groupBy)
            {
                case "week":
                    return data.ToList().GroupBy(i => new { i.Date.Year, i.Date.Month, Week = GetGregorianWeekOfYear(i.Date) });
                case "month":
                    return data.GroupBy(i => new { i.Date.Year, i.Date.Month });
                case "year":
                    return data.GroupBy(i => new { i.Date.Year });
                default:

                    var dataList = data.ToList();
                    return dataList.GroupBy(i => new { i.Date.Year, i.Date.Month, i.Date.Day });
            }
        }

        public IEnumerable<IGrouping<dynamic, ShopOrder>> GroupByPeriod(IQueryable<ShopOrder> data, string groupBy = "day")
        {
            switch (groupBy)
            {
                case "week":
                    return data.ToList().GroupBy(i => new { i.CreatedDate.Year, i.CreatedDate.Month, Week = GetGregorianWeekOfYear(i.CreatedDate) });
                case "month":
                    return data.GroupBy(i => new { i.CreatedDate.Year, i.CreatedDate.Month });
                case "year":
                    return data.GroupBy(i => new { i.CreatedDate.Year });
                default:
                    return data.GroupBy(i => new { i.CreatedDate.Year, i.CreatedDate.Month, i.CreatedDate.Day });
            }
        }

        public SeriesArray GroupingToSeriesArray(List<IGrouping<dynamic, StatisticEntry>> periods, string groupedBy = "day", bool countlastReg = true)
        {
            ViewBag.TodayValue = 0;
            ViewBag.YesterdayValue = 0;
            ViewBag.BeforeYesterdayValue = 0;
            var values = new List<double>();
            var labels = new List<string>();
            if(periods!=null && periods.Any())
            foreach (IGrouping<dynamic, StatisticEntry> period in periods.OrderBy(p => p.FirstOrDefault().Date))
            {
                    if (period != null)
                    {
                        if (countlastReg)
                        {
                            if (period.FirstOrDefault().Date == DateTime.Now)
                            {
                                ViewBag.TodayValue = period.Count();
                            }
                            if (period.FirstOrDefault().Date == DateTime.Today.AddDays(-1))
                            {
                                ViewBag.YesterdayValue = period.Count();
                            }
                            if (period.FirstOrDefault().Date == DateTime.Today.AddDays(-2))
                            {
                                ViewBag.BeforeYesterdayValue = period.Count();
                            }
                        }
                        
                        values.Add(period.Count());
                        var firstEntry = period.FirstOrDefault();

                        switch (groupedBy)
                        {
                            case "week":
                                labels.Add(firstEntry.Date.ToString($"yyyy/{period.Key.Week}"));
                                break;
                            case "month":
                                labels.Add(firstEntry.Date.ToString("MMM yyyy", CultureInfo.InvariantCulture));
                                break;
                            case "year":
                                labels.Add(firstEntry.Date.ToString("yyyy", CultureInfo.InvariantCulture));
                                break;
                            default:
                                labels.Add(firstEntry.Date.ToString("dd MMM yy", CultureInfo.InvariantCulture));
                                break;
                        }
                    }
                
            }

            return new SeriesArray { Labels = labels.ToArray(), Values = values.ToArray() };
        }
        public SeriesArray GroupingToSeriesArrayAvg(IEnumerable<IGrouping<dynamic, StatisticEntry>> periods, string groupedBy = "month")
        {
            
            var values = new List<double>();
            var labels = new List<string>();
            var totalRegUser = (from s in db.StatisticEntries
                                where s.DemoMode == false && s.UserId != ANONYMOUS_GUID
                                select s.UserId).ToList().Count();

            foreach (var period in periods.OrderBy(p => p.FirstOrDefault().Date))
            {
                double value =(double) period.Count() / (double) totalRegUser;
                values.Add(value);
                var firstEntry = period.FirstOrDefault();

                switch (groupedBy)
                {
                    case "week":
                        labels.Add(firstEntry.Date.ToString($"yyyy/{period.Key.Week}"));
                        break;
                    case "month":
                        labels.Add(firstEntry.Date.ToString("MMM yyyy", CultureInfo.InvariantCulture));
                        break;
                    case "year":
                        labels.Add(firstEntry.Date.ToString("yyyy", CultureInfo.InvariantCulture));
                        break;
                    default:
                        labels.Add(firstEntry.Date.ToString("dd MMM yy", CultureInfo.InvariantCulture));
                        break;
                }
            }

            return new SeriesArray { Labels = labels.ToArray(), Values = values.ToArray() };
        }
        public SeriesArray GroupingToSeriesArray(IEnumerable<IGrouping<dynamic, ShopOrder>> periods, string groupedBy = "day")
        {
            var values = new List<double>();
            var labels = new List<string>();

            foreach (var period in periods.OrderBy(p => p.FirstOrDefault().CreatedDate))
            {
                values.Add((int)period.Sum(o => o.OrderAmount));
                var firstEntry = period.FirstOrDefault();

                switch (groupedBy)
                {
                    case "week":
                        labels.Add(firstEntry.CreatedDate.ToString($"yyyy/{period.Key.Week}"));
                        break;
                    case "month":
                        labels.Add(firstEntry.CreatedDate.ToString("MMM yyyy", CultureInfo.InvariantCulture));
                        break;
                    case "year":
                        labels.Add(firstEntry.CreatedDate.ToString("yyyy", CultureInfo.InvariantCulture));
                        break;
                    default:
                        labels.Add(firstEntry.CreatedDate.ToString("dd MMM yy", CultureInfo.InvariantCulture));
                        break;
                }
            }

            return new SeriesArray { Labels = labels.ToArray(), Values = values.ToArray() };
        }
       
        public SeriesArray AccumulateSeriesArray(DateTime? startDate, DateTime? endDate, DateTime? startDateLimit, string groupBy = "day", bool demoOnly = false)
        {
            ViewBag.ShowCumRegistration = "True";
            SeriesArray seriesArray = GroupingToSeriesArray(GroupByPeriod(GetStatisticEntries(StatisticEventTypes.Register, startDate, endDate, demoOnly,false), groupBy).ToList(), groupBy,false);
            
            
            var cumulativeSeriesArray = new SeriesArray() { Labels = seriesArray.Labels };
            var cumulativeCounts = new List<double>();
            double sum = 0;

            foreach (var value in seriesArray.Values)
            {
                sum += value;
                cumulativeCounts.Add(sum);
            }

            cumulativeSeriesArray.Values = cumulativeCounts.ToArray();
           
            if(startDateLimit != null)
            {
                var startIndex = seriesArray.Labels.ToList().IndexOf(((DateTime)startDateLimit).ToString("dd MMM yy"));

                cumulativeSeriesArray.Labels = cumulativeSeriesArray.Labels.Skip(startIndex).ToArray();
                cumulativeSeriesArray.Values = cumulativeSeriesArray.Values.Skip(startIndex).ToArray();

            }

            return cumulativeSeriesArray;
        }

        public PageviewsData GetPageViews(DateTime? startDate = null, DateTime? endDate = null, string groupBy = "day", bool demoOnly = false)
        {
            if (!startDate.HasValue) { startDate = default(DateTime); }
            if (!endDate.HasValue) { endDate = DateTime.Now; }

            var data = from s in db.StatisticEntries
                       where  s.EventId == (int)StatisticEventTypes.Pageview && s.ObjectType == EventObjectType.PageName && (demoOnly ? s.DemoMode : true) && s.Date >= startDate && s.Date <= endDate 
                       select s;

            List<PageSeries> grouped;
            switch (groupBy)
            {
                case "week":
                    grouped = data
                        .ToList()
                        .GroupBy(i => new { i.Date.Year, i.Date.Month, Week = GetGregorianWeekOfYear(i.Date), Page = i.ObjectId })
                        .Select(d => new PageSeries() { Label = d.FirstOrDefault().ObjectId.ToString(), Count = d.Count(), Date = d.FirstOrDefault().Date.ToString($"yyyy/{d.Key.Week}") })
                        .ToList();
                    break;
                case "month":
                    grouped = data
                        .ToList()
                        .GroupBy(i => new { i.Date.Year, i.Date.Month, Page = i.ObjectId })
                        .Select(d => new PageSeries() { Label = d.FirstOrDefault().ObjectId.ToString(), Count = d.Count(), Date = d.FirstOrDefault().Date.ToString("MMM yyyy", CultureInfo.InvariantCulture) })
                        .ToList();
                    break;
                case "year":
                    grouped = data
                        .ToList()
                        .GroupBy(i => new { i.Date.Year, Page = i.ObjectId })
                        .AsEnumerable()
                        .Select(d => new PageSeries() { Label = d.FirstOrDefault().ObjectId.ToString(), Count = d.Count(), Date = d.FirstOrDefault().Date.ToString("yyyy", CultureInfo.InvariantCulture) })
                        .ToList();
                    break;
                default:
                    grouped = data
                        .ToList()
                        .GroupBy(i => new { i.Date.Year, i.Date.Month, i.Date.Day, Page = i.ObjectId })
                        .AsEnumerable()
                        .Select(d => new PageSeries() { Label = d.FirstOrDefault().ObjectId.ToString(), Count = d.Count(), Date = d.FirstOrDefault().Date.ToString("dd MMM yy", CultureInfo.InvariantCulture) })
                        .ToList();
                    break;
            }

            string[] labels = grouped.Select(g => g.Date).Distinct().ToArray();

            return new PageviewsData()
            {
                labels = labels,
                data = grouped
            };
        }

        public PageviewsData GetPlantDocData(DateTime? startDate = null, DateTime? endDate = null, string groupBy = "day", bool demoOnly = false)
        {
            var deletedUsers = db.Users.Where(u => u.Deleted).ToList();
            if (!startDate.HasValue) { startDate = default(DateTime); }
            if (!endDate.HasValue) { endDate = DateTime.Now; }

            var deletedUser = db.Users.Where(u => u.Deleted).ToList();


            var data = (from s in db.StatisticEntries
                       where s.Date >= startDate && s.Date <= endDate
                       && (s.EventId == (int)StatisticEventTypes.SubmitAnswer || s.EventId == (int)StatisticEventTypes.SubmitQuestion) && (demoOnly ? s.DemoMode : !s.DemoMode)
                      
                       select s);
            //data = data.AsEnumerable().Where(d => deletedUsers.FirstOrDefault(u => u.Id == d.UserId.ToString()) == null).OrderByDescending(d=>d.Date);
            List<PageSeries> grouped;
            switch (groupBy)
            {
                case "week":
                    grouped = data
                        .ToList()
                        .GroupBy(i => new { i.Date.Year, i.Date.Month, Week = GetGregorianWeekOfYear(i.Date), Type = i.EventId })
                        .AsEnumerable()
                        .Select(d => new PageSeries() { Label = d.FirstOrDefault().EventId.ToString(), Count = d.Count(), Date = d.FirstOrDefault().Date.ToString($"yyyy/{d.Key.Week}") })
                        .ToList();
                    break;
                case "month":
                    grouped = data
                        .GroupBy(i => new { i.Date.Year, i.Date.Month, Type = i.EventId })
                        .AsEnumerable()
                        .Select(d => new PageSeries() { Label = d.FirstOrDefault().EventId.ToString(), Count = d.Count(), Date = d.FirstOrDefault().Date.ToString("MMM yyyy", CultureInfo.InvariantCulture) })
                        .ToList();
                    break;
                case "year":
                    grouped = data
                        .GroupBy(i => new { i.Date.Year, Type = i.EventId })
                        .AsEnumerable()
                        .Select(d => new PageSeries() { Label = d.FirstOrDefault().EventId.ToString(), Count = d.Count(), Date = d.FirstOrDefault().Date.ToString("yyyy", CultureInfo.InvariantCulture) })
                        .ToList();
                    break;
                default:
                    grouped = data
                        .GroupBy(i => new { i.Date.Year, i.Date.Month, i.Date.Day, Type = i.EventId })
                        .AsEnumerable()
                        .Select(d => new PageSeries() { Label = d.FirstOrDefault().EventId.ToString(), Count = d.Count(), Date = d.FirstOrDefault().Date.ToString("dd MMM yy", CultureInfo.InvariantCulture) })
                        .ToList();
                    break;
            }

            string[] labels = grouped.Select(g => g.Date).Distinct().ToArray();

            return new PageviewsData()
            {
                labels = labels,
                data = grouped
            };
        }
        
        public int GetGregorianWeekOfYear(DateTime dateTime)
        {
            return calendar.GetWeekOfYear(dateTime, CalendarWeekRule.FirstFullWeek, DayOfWeek.Monday);
        }
        public int countSingleTodoEntries()
        {
            var countUserSinglesTodoes = db.Todoes.Where(todo => todo.ReferenceId == 0 && todo.CyclicId==null && !todo.Deleted).Count();
            var countUserCyclicTodoes = db.TodoCyclic.Where(todo => todo.ReferenceId == 0 && !todo.Deleted).Count();
            var summe = countUserSinglesTodoes + countUserCyclicTodoes;
            return summe;
        }
        public int countDiaryEntries()
        {
            var count = db.DiaryEntry.Where(d => !d.Deleted).Count();
            return count;
        }
        public SeriesArray TodoStatEntries()
        {
            var values = new double[] { countSingleTodoEntries(), countDiaryEntries() };
            
            return new SeriesArray()
            {
                Labels = new string[]{ "Eigene To-dos", "Tagebucheinträge" },
                Values = values
            };
        }
    }

    public class PageSeries
    {
        public int Count { get; set; }
        public string Date { get; set; }
        public string Label { get; set; }
    }

    public class PageviewsData
    {
        public List<PageSeries> data { get; set; }
        public string[] labels { get; set; }
    }

    public class SeriesArray
    {
        public double[] Values { get; set; }
        public string[] Labels { get; set; }
    }

    public class UsersActionsOnDevices
    {
        public IEnumerable<IEnumerable<StatisticEntry>> groupedActions;
        public string[] Labels;
    }
}