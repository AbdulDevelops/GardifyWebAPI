using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Http;
using static GardifyModels.Models.AlertViewModels;
using static GardifyModels.Models.ModelEnums;
using static GardifyModels.Models.UserPlantViewModels;
using static GardifyModels.Models.WeatherHelpers;
using GardifyWebAPI.Services;
using WebPush;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Data.Entity.Infrastructure;

namespace GardifyWebAPI.Controllers
{
    public class WarningAPIController : ApiController
    {
        protected ApplicationDbContext plantDB = new ApplicationDbContext();
        WeatherHandler wh = new WeatherHandler();
        PropertyController pc = new PropertyController();
        WarningController wc = new WarningController();

        [AllowAnonymous]
        [Route("api/WarningAPI/popbyscheduler")]
        [HttpGet]
        public async Task<IHttpActionResult> PopulateWarnings(bool forceUpdate = false)
        {
            var today = DateTime.Today;
            int day = today.Day;
            int month = today.Month;
            //check if datetimeTody is 30.06 and turn on all frost notifications
            if(day==30 && month == 6)
            {
                var userplants = plantDB.UserPlants.Where(plant => !plant.Deleted).ToList();
                foreach(var plant in userplants)
                {
                    plant.NotifyForFrost = true;
                }
                var userDevices = (from dev in plantDB.Devices where dev.AdminDevice.notifyForFrost && !dev.Deleted select dev).ToList();
                foreach (var device in userDevices)
                {
                    device.notifyForFrost = true;
                }
                var userSettings = plantDB.UsersSettings.Where(u => !u.Deleted).ToList();
                foreach (var us in userSettings)
                {
                    us.ActiveFrostAlert = true;
                }
                plantDB.SaveChanges();
            }
            // get users with incomplete address grouped by PLZ
            var usersByPLZ = plantDB.Users.Where(user => !user.Deleted && user.EmailConfirmed && !user.Email.Contains("UserDemo") && (user.City.Contains("Platzhalter") || user.Street.Contains("Platzhalter"))).GroupBy(u => u.PLZ).ToList();
            // rest of the users, ignore recently alerted and demo accounts
            var usersList = plantDB.Users.Where(user => !user.Deleted && user.EmailConfirmed && !user.Email.Contains("UserDemo") && !user.City.Contains("Platzhalter") && !user.Street.Contains("Platzhalter")).ToList().Where(user => (user.LastAlerted.HasValue ? DateTime.Now.Subtract(user.LastAlerted.Value).TotalDays >= 1 : true) || forceUpdate).ToList();

            //var testingUserList = plantDB.Users.Where(user => user.UserName == "schmoody" && !user.Deleted && user.EmailConfirmed && !user.Email.Contains("UserDemo") && !user.City.Contains("Platzhalter") && !user.Street.Contains("Platzhalter")).ToList();

            var notifiedUsers = 0;

            // get one representative for each PLZ zone, use the coords of his property to get a forecast for next 2 days
            foreach (IGrouping<int, ApplicationUser> group in usersByPLZ)
            {
                var usersInGroup = group.ToList();
                var groupRep = usersInGroup.FirstOrDefault();
                Property p = pc.DbGetProperty(new Guid(groupRep.Id));
                var forecast = await wh.getWarningForecastByDate(p, DateTime.Now.AddDays(1), 2);
                var repRes = await wc.WarnUser(groupRep, plantDB, forecast, p, null, forceUpdate);
                if (repRes) { notifiedUsers++; }

                // warn users in this group (same forecast)
                foreach (ApplicationUser auser in usersInGroup)
                {
                    var aRes = await wc.WarnUser(auser, plantDB, forecast, p, null, forceUpdate);
                    if (aRes) { notifiedUsers++; }
                }
            }

            // iterate single users in usersList
            foreach (ApplicationUser user in usersList)
            {
                var aRes = await wc.WarnUser(user, plantDB, forceUpdate: forceUpdate);
                if (aRes) { notifiedUsers++; }
            }

            return Ok(notifiedUsers);
        }

        [Route("api/WarningAPI/warnings")]
        [HttpGet]
        public List<ObjectWarningViewModel> GetUserWarnings()
        {
            var upc = new UserPlantController();
            var ac = new AlertController();
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return null;
            }
            var userSettings = plantDB.UsersSettings.Where(us => us.UserId == userId).FirstOrDefault();
            var userGarden = plantDB.Property.Where(pr => pr.UserId == userId && !pr.Deleted).FirstOrDefault().Gardens.FirstOrDefault();

            if (userGarden == null)
            {
                return null;
            }

            var affectedObjects = new List<ObjectWarningViewModel>();
            var userPlants = plantDB.UserPlants.Where(up => !up.Deleted && up.Gardenid == userGarden.Id);
            IEnumerable<Device> userDevices = plantDB.Devices.Where(dev => dev.Gardenid == userGarden.Id && !dev.Deleted);

            if (userPlants != null && userPlants.Any())
            {
                foreach (UserPlant up in userPlants)
                {
                    IEnumerable<Alert> plantAlerts = ac.DbGetAlertsByRelatedObjectId(up.PlantId, ModelEnums.ReferenceToModelClass.Plant);
                    if (plantAlerts == null || !plantAlerts.Any())
                    {
                        continue;
                    }
                    var a = plantAlerts.FirstOrDefault();

                    //if (up.WarningsAboutThisPlant.FirstOrDefault() == null)
                    //{
                    //    up.NotifyForFrost = true;
                    //    up.NotifyForWind = up.IsInPot ? true : false;

                    //}

                    var temp = new ObjectWarningViewModel()
                    {
                        AlertConditionValue = wc.GetAlertConditionValue(up.PlantId, ReferenceToModelClass.Plant),
                        Title = a.Title,
                        Text = a.Text,
                        RelatedObjectName = up.Name,
                        NotifyForFrost = up.NotifyForFrost,
                        NotifyForWind = up.NotifyForWind,
                        IsInPot = up.IsInPot,
                        RelatedObjectId = up.Id,
                        ObjectType = ReferenceToModelClass.Plant,
                        Dismissed = up.WarningsAboutThisPlant.FirstOrDefault()!=null? up.WarningsAboutThisPlant.FirstOrDefault().Dismissed :true
                    };
                    affectedObjects.Add(temp);
                   
                }

                //plantDB.SaveChanges();

            }

            if (userDevices != null && userDevices.Any())
            {
                foreach (Device dev in userDevices)
                {
                    var temp = new ObjectWarningViewModel()
                    {
                        AlertConditionValue = userSettings.FrostDegreeBuffer,
                        Title = dev.Name,
                        RelatedObjectName = dev.Name,
                        NotifyForFrost = dev.notifyForFrost,
                        NotifyForWind = dev.notifyForWind,
                        RelatedObjectId = dev.Id,
                        ObjectType = ReferenceToModelClass.Device,
                        Dismissed = dev.WarningsAboutThisDevice.FirstOrDefault() != null ? dev.WarningsAboutThisDevice.FirstOrDefault().Dismissed : true
                    };
                    affectedObjects.Add(temp);
                }
            }

            return affectedObjects;
        }

        [Route("api/WarningAPI/reset")]
        [HttpGet]
        public List<ObjectWarningViewModel> ResetUserWarnings()
        {
            var upc = new UserPlantController();
            var ac = new AlertController();
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return null;
            }
            var userSettings = plantDB.UsersSettings.Where(us => us.UserId == userId).FirstOrDefault();
            var userGarden = plantDB.Property.Where(pr => pr.UserId == userId && !pr.Deleted).FirstOrDefault().Gardens.FirstOrDefault();

            if (userGarden == null)
            {
                return null;
            }

            var affectedObjects = new List<ObjectWarningViewModel>();
            var userPlants = plantDB.UserPlants.Where(up => !up.Deleted && up.Gardenid == userGarden.Id).ToList();
            IEnumerable<Device> userDevices = plantDB.Devices.Where(dev => dev.Gardenid == userGarden.Id && !dev.Deleted).ToList();

            if (userPlants != null && userPlants.Any())
            {
                foreach (UserPlant up in userPlants)
                {
                    up.NotifyForWind = up.IsInPot ? true : false;
                    up.NotifyForFrost = true;

                    IEnumerable<Alert> plantAlerts = ac.DbGetAlertsByRelatedObjectId(up.PlantId, ModelEnums.ReferenceToModelClass.Plant);
                    if (plantAlerts == null || !plantAlerts.Any())
                    {
                        continue;
                    }
                    var a = plantAlerts.FirstOrDefault();

                    var temp = new ObjectWarningViewModel()
                    {
                        AlertConditionValue = wc.GetAlertConditionValue(up.PlantId, ReferenceToModelClass.Plant),
                        Title = a.Title,
                        Text = a.Text,
                        RelatedObjectName = up.Name,
                        NotifyForFrost = up.NotifyForFrost,
                        NotifyForWind = up.NotifyForWind,
                        IsInPot = up.IsInPot,
                        RelatedObjectId = up.Id,
                        ObjectType = ReferenceToModelClass.Plant,
                        Dismissed = up.WarningsAboutThisPlant.FirstOrDefault() != null ? up.WarningsAboutThisPlant.FirstOrDefault().Dismissed : true
                    };
                    affectedObjects.Add(temp);
                }
                plantDB.SaveChanges();
            }

            if (userDevices != null && userDevices.Any())
            {
                foreach (Device dev in userDevices)
                {
                    dev.notifyForFrost = plantDB.AdminDevices.Find(dev.AdminDevId)?.notifyForFrost ?? dev.notifyForFrost;
                    dev.notifyForWind = plantDB.AdminDevices.Find(dev.AdminDevId)?.notifyForWind ?? dev.notifyForWind;

                    var temp = new ObjectWarningViewModel()
                    {
                        AlertConditionValue = userSettings.FrostDegreeBuffer,
                        Title = dev.Name,
                        RelatedObjectName = dev.Name,
                        NotifyForFrost = dev.notifyForFrost,
                        NotifyForWind = dev.notifyForWind,
                        RelatedObjectId = dev.Id,
                        ObjectType = ReferenceToModelClass.Device,
                        Dismissed = dev.WarningsAboutThisDevice.FirstOrDefault() != null ? dev.WarningsAboutThisDevice.FirstOrDefault().Dismissed : true
                    };
                    affectedObjects.Add(temp);
                }
                plantDB.SaveChanges();
            }

            return affectedObjects;
        }

        [Route("api/WarningAPI/togglePlants/{forFrost}/{newState}")]
        [HttpGet]
        public List<ObjectWarningViewModel> ToggleUserPlantsWarnings(bool forFrost, bool newState)
        {
            var upc = new UserPlantController();
            var ac = new AlertController();
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return null;
            }
            var userSettings = plantDB.UsersSettings.Where(us => us.UserId == userId).FirstOrDefault();
            var userGarden = plantDB.Property.Where(pr => pr.UserId == userId && !pr.Deleted).FirstOrDefault().Gardens.FirstOrDefault();

            if (userGarden == null)
            {
                return null;
            }

            var affectedObjects = new List<ObjectWarningViewModel>();
            var userPlants = plantDB.UserPlants.Where(up => !up.Deleted && up.Gardenid == userGarden.Id).ToList();

            if (userPlants != null && userPlants.Any())
            {
                foreach (UserPlant up in userPlants)
                {
                    up.NotifyForWind = !forFrost ? newState : up.NotifyForWind;
                    up.NotifyForFrost = forFrost ? newState : up.NotifyForFrost;

                    IEnumerable<Alert> plantAlerts = ac.DbGetAlertsByRelatedObjectId(up.PlantId, ModelEnums.ReferenceToModelClass.Plant);
                    if (plantAlerts == null || !plantAlerts.Any())
                    {
                        continue;
                    }
                    var a = plantAlerts.FirstOrDefault();

                    var temp = new ObjectWarningViewModel()
                    {
                        AlertConditionValue = wc.GetAlertConditionValue(up.PlantId, ReferenceToModelClass.Plant),
                        Title = a.Title,
                        Text = a.Text,
                        RelatedObjectName = up.Name,
                        NotifyForFrost = up.NotifyForFrost,
                        NotifyForWind = up.NotifyForWind,
                        IsInPot = up.IsInPot,
                        RelatedObjectId = up.Id,
                        ObjectType = ReferenceToModelClass.Plant,
                        Dismissed = up.WarningsAboutThisPlant.FirstOrDefault() != null ? up.WarningsAboutThisPlant.FirstOrDefault().Dismissed : true
                    };
                    affectedObjects.Add(temp);
                }
                plantDB.SaveChanges();
            }

            return affectedObjects;
        }

        [Route("api/WarningAPI/toggleDevices/{forFrost}/{newState}")]
        [HttpGet]
        public List<ObjectWarningViewModel> ToggleUserDevicesWarnings(bool forFrost, bool newState)
        {
            var upc = new UserPlantController();
            var ac = new AlertController();
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return null;
            }
            var userSettings = plantDB.UsersSettings.Where(us => us.UserId == userId).FirstOrDefault();
            var userGarden = plantDB.Property.Where(pr => pr.UserId == userId && !pr.Deleted).FirstOrDefault().Gardens.FirstOrDefault();

            if (userGarden == null)
            {
                return null;
            }

            var affectedObjects = new List<ObjectWarningViewModel>();
            IEnumerable<Device> userDevices = plantDB.Devices.Where(dev => dev.Gardenid == userGarden.Id && !dev.Deleted).ToList();

            if (userDevices != null && userDevices.Any())
            {
                foreach (Device dev in userDevices)
                {
                    dev.notifyForFrost = forFrost ? newState : dev.notifyForFrost;
                    dev.notifyForWind = !forFrost ? newState : dev.notifyForWind;

                    var temp = new ObjectWarningViewModel()
                    {
                        AlertConditionValue = userSettings.FrostDegreeBuffer,
                        Title = dev.Name,
                        RelatedObjectName = dev.Name,
                        NotifyForFrost = dev.notifyForFrost,
                        NotifyForWind = dev.notifyForWind,
                        RelatedObjectId = dev.Id,
                        ObjectType = ReferenceToModelClass.Device,
                        Dismissed = dev.WarningsAboutThisDevice.FirstOrDefault() != null ? dev.WarningsAboutThisDevice.FirstOrDefault().Dismissed : true
                    };
                    affectedObjects.Add(temp);
                }
                plantDB.SaveChanges();
            }

            return affectedObjects;
        }

        [Route("api/WarningAPI/dismiss/{relatedObjId}/{objectType}")]
        [HttpGet]
        public IHttpActionResult DismissWarning(int relatedObjId, int objectType)
        {
            var userId = Utilities.GetUserId();
            if (objectType==(int) ReferenceToModelClass.Plant)
            {
                var userPlant = plantDB.UserPlants.Where(up => up.PlantId == relatedObjId).FirstOrDefault();
                if (userPlant != null)
                {
                    var relatedWarning = userPlant.WarningsAboutThisPlant.Where(wp => wp.UserId == userId);
                    foreach (var r in relatedWarning.ToList())
                    {
                        r.Dismissed = true;
                    }
                    plantDB.SaveChanges();
                }
            }
            if (objectType==(int)ReferenceToModelClass.Device)
            {
                var device = plantDB.Devices.Where(d => d.Id == relatedObjId).FirstOrDefault();
                if (device != null)
                {
                    var relatedWarning = device.WarningsAboutThisDevice.Where(wp => wp.UserId == userId);
                    foreach (var r in relatedWarning.ToList())
                    {
                        r.Dismissed = true;
                    }
                    plantDB.SaveChanges();
                }
            }
            return Ok();
            
        }

        [Route("api/WarningAPI/hide/{warningId}")]
        [HttpDelete]
        public IHttpActionResult HideWarning(int warningId)
        {
            var userId = Utilities.GetUserId();
            var warning = plantDB.UserWarnings.Where(uw => uw.Id == warningId && uw.UserId == userId).FirstOrDefault();
            if (warning != null)
            {
                warning.Deleted = true;
                plantDB.SaveChanges();
            }
            return Ok();
        }

        [Route("api/WarningAPI/pushsub")]
        [HttpPost]
        public IHttpActionResult AddPushSubscription(GardifyModels.Models.PushSubscription sub)
        {
            sub.UserId = Utilities.GetUserId();
            sub.OnCreate("System");
            plantDB.PushSubscriptions.Add(sub);
            var res = plantDB.SaveChanges();
            if (res > 0)
            {
                return Ok();
            }
            return BadRequest();
        }

        [Route("api/WarningAPI/pushsub")]
        [HttpDelete]
        public IHttpActionResult RemovePushSubscription()
        {
            var userId = Utilities.GetUserId();

            var sub = plantDB.PushSubscriptions.Where(s => s.UserId == userId && !s.Deleted).FirstOrDefault();
            if (sub != null)
            {
                sub.Deleted = true;
                plantDB.SaveChanges();
            }
            return Ok();
        }
       
       }

    public class WarningController
    {
        // Keys for sending out notifications to users
        public readonly string VAPID_PUBLIC_KEY = "BDhYJhVQLL5aCJ0H0wO3vGq9DjKx_u8hOxEW3jiRlUyFaRTfE-gqNMhZ57LTJecHvtDEhbbrnRbPdOfa__4r1Wo";
        public readonly string VAPID_PRIVATE_KEY = "9YsZ_NHbMSZHUSS_dP2MzM8BgHsm1AxnqcEfd77W6vs";
        protected ApplicationDbContext plantDB = new ApplicationDbContext();
        public readonly float MIN_WARNING_WIND_SPEED = 60f; //kmh
        WeatherHandler wh = new WeatherHandler();
        PropertyController pc = new PropertyController();
        AlertController ac = new AlertController();
        UserPlantController upc = new UserPlantController();

        // populates warnings for users and notifies them if needed
        public async Task<bool> WarnUser(ApplicationUser user, ApplicationDbContext context, WeeklyForecastsObj forecast = null, Property p = null, HashSet<UserPlantDetailsViewModel> userPlants = null, bool forceUpdate = false)
        {
            // alert user once every 2 days max (ignore if its single-plant only)
            if (!forceUpdate && userPlants == null && user.LastAlerted.HasValue && DateTime.Now.Subtract(user.LastAlerted.Value).Days < 1) { return false; }
            // fetch only when needed
            if (p == null) { p = pc.DbGetProperty(new Guid(user.Id)); }
            if (forecast == null) { forecast = await wh.getWarningForecastByDate(p, DateTime.Now.AddDays(1), 2); }

            var userSettings = plantDB.UsersSettings.Where(us => us.UserId.ToString() == user.Id).FirstOrDefault();
            if (userSettings == null || !(userSettings.ActiveFrostAlert || userSettings.ActiveStormAlert)
                                    || !(userSettings.AlertByEmail || userSettings.AlertByPush))
            {
                return false;   // ignore users that dont want alerts
            }

            var warning = new WarningViewModel();
            var userGarden = plantDB.Property.Where(pr => pr.UserId.ToString() == user.Id && !pr.Deleted).FirstOrDefault().Gardens.FirstOrDefault();

            if (userGarden == null || forecast == null)
            {
                return false;
            }

            // userPlants is either a single plant (called with parameter when added to garden) or every plant on daily check
            if (userPlants == null)
            {
                userPlants = upc.DbGetUserPlantsFromLists(userGarden.Id);
            }
            
            IEnumerable<Device> userDevices = plantDB.Devices.Where(dev => dev.Gardenid == userGarden.Id && !dev.Deleted).ToList();

            // get warnings for userDevices
            if (userGarden.Id > 0 && userDevices != null && userDevices.Any())
            {
                foreach (Device dev in userDevices)
                {
                    var alertType = GetAlertTypeForDevice(dev, forecast, userSettings.FrostDegreeBuffer);
                    if (alertType != AlertType.NotSet)
                    {
                        var alert = new AlertLiteViewModel()
                        {
                            RelatedObject = dev.Name,
                            ObjectType = ReferenceToModelClass.Device,
                            AlertType = alertType
                        };
                        warning.Alerts.Add(alert);
                        warning.UserDevices.Add(dev);
                        warning.UserSettings = userSettings;
                    }
                }
            }
            // get warnings for userPlants
            if (userGarden.Id > 0 && userPlants != null && userPlants.Any())
            {
                foreach (UserPlantDetailsViewModel up in userPlants)
                {
                    var alerts = ac.DbGetTriggeredAlertsByPlantId(up.PlantId, forecast, userSettings.FrostDegreeBuffer);
                    foreach (Alert a in alerts)
                    {
                        var alert = new AlertLiteViewModel()
                        {
                            Id = a.Id,
                            Title = a.Title,
                            Text = a.Text,
                            RelatedObject = up.Name,
                            RelatedObjectId = up.PlantId,
                            ObjectType = ReferenceToModelClass.Plant,
                            AlertType = GetAlertType(a),
                            Trigger= a.Triggers
                        };
                        warning.Alerts.Add(alert);
                        warning.UserPlants.Add(up);
                        warning.UserSettings = userSettings;
                    }
                }
            }

            // add general warnings where temp is below users FrostDegreeBuffer
            if (forecast.Forecasts.Any(f => f.MinAirTemperatureInCelsius <= userSettings.FrostDegreeBuffer))
            {
                warning.Alerts.Add(new AlertLiteViewModel()
                {
                    AlertType = AlertType.Frost,
                    RelatedObject = null,
                    RelatedObjectId = -1,
                    //ObjectType = null, Id = a.Id,
                    Title = "Es gibt eine allgemeine Frostwarnung",
                    Text = ""
                });
            }
            
            // nothing to alert
            if (!warning.Alerts.Any())
            {
                return false;
            }

            warning.Date = forecast.Forecasts.FirstOrDefault().ValidFrom;
            warning.MinTemp = GetMinTempInPeriod(forecast);
            warning.MaxWindSpeed = GetMaxWindSpeedInPeriod(forecast);
            warning.UserName = user.DisplayName();

            // make sure to notify for allowed alert types only
            var canAlert = new AlertableRes();
            try
            {
                canAlert = await UserCanBeAlerted(p, user.LastAlerted, warning);
            } catch(Exception e) { }

            var stormRelated = warning.Alerts.Any(a => a.AlertType == AlertType.Storm);
            var frostRelated = warning.Alerts.Any(a => a.AlertType == AlertType.Frost);

            bool emailRes = false;

            if (warning.UserSettings != null && ((warning.UserSettings.ActiveStormAlert && (canAlert.CanAlertForStorm && !forceUpdate) && stormRelated)
                                              || (warning.UserSettings.ActiveFrostAlert && (canAlert.CanAlertForFrost && !forceUpdate) && frostRelated)))
            {
                emailRes = await SendWarningEmail(user, warning, frostRelated ? AlertType.Frost : AlertType.Storm, context);
            }
            return emailRes;
        }

        private async Task<bool> SendWarningEmail(ApplicationUser to, WarningViewModel model, AlertType alertType, ApplicationDbContext context, string[] filePaths = null)
        {
            var CondValue = alertType == AlertType.Frost ? model.MinTemp : model.MaxWindSpeed;
            var resEntry = CreateUserWarningEntry(new Guid(to.Id), model.Date, false, CondValue, "", NotificationType.Email, alertType, model.UserPlants, model.UserDevices);

            if (!to.EmailConfirmed || !model.UserSettings.AlertByEmail || (model.UserSettings.AlertFrostEmailDisable && model.UserSettings.AlertStormEmailDisable))
            {
                return false;
            }

            EmailSender es = new EmailSender(context);
            TemplateService ts = new TemplateService();
            WarningViewModel vm = new WarningViewModel()
            {
                 UserSettings = model.UserSettings,
                 UserName = model.UserName,
                 Alerts =model.Alerts,
                 MaxWindSpeed = model.MaxWindSpeed,
                 MinTemp = model.MinTemp,
                 Date = model.Date,
                 UserPlants =model.UserPlants.Where(up=> (up.NotifyForWind && !model.UserSettings.AlertStormEmailDisable) || (up.NotifyForFrost && !model.UserSettings.AlertFrostEmailDisable)).Select(u=>u).ToList(),
                 UserDevices=model.UserDevices.Where(ud => (ud.notifyForWind && !model.UserSettings.AlertStormEmailDisable) || (ud.notifyForFrost && !model.UserSettings.AlertFrostEmailDisable)).Select(u => u).ToList()
            };
            string content = ts.RenderTemplateAsync("Warning", model);
            var resEmail = await es.SendEmail("Neue Warnung für Ihren Garten", content, "wetterwarnung@gardify.de", to.Email, filePaths);

            if (!resEntry && !resEmail)
            {
                return false;
            }

            try
            {
                to.LastAlerted = DateTime.Now;
                context.SaveChanges();
            }
            catch (Exception ex) { return false; }

            return true;
        }

        private bool SendWarningPush(string userId, PushNotificationPayload payload, AlertType alertType, DateTime condDate)
        {
            JsonSerializerSettings _jsonSerializerSettings = new JsonSerializerSettings()
            {
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var userSub = plantDB.PushSubscriptions.Where(ps => ps.UserId.ToString() == userId).FirstOrDefault();
            if (userSub == null || userSub.Deleted)
            {
                return false;
            }

            var subscription = new WebPush.PushSubscription(userSub.EndPoint, userSub.P256dh, userSub.Auth);
            var subject = "https://gardify.sslbeta.de/";
            var vapidDetails = new VapidDetails(subject, VAPID_PUBLIC_KEY, VAPID_PRIVATE_KEY);

            var payloadStr = "{\"notification\":{\"title\":\"Es gibt Frostwarnungen\",\"icon\":\"assets/FavIcon/Gardify_Favicon.png\",\"requireInteraction\":\"true\",\"body\":\"Ab 13.09.2019\"}}";

            var webPushClient = new WebPushClient();
            try
            {
                webPushClient.SendNotification(subscription, JsonConvert.SerializeObject(payload, _jsonSerializerSettings), vapidDetails);
                var user = plantDB.Users.Where(u => !u.Deleted && u.Id == userId).FirstOrDefault();
                user.LastAlerted = DateTime.Now;
                plantDB.SaveChanges();

                // keep empty for push 
                var plants = new List<UserPlantDetailsViewModel>();
                var devs = new List<Device>();
                CreateUserWarningEntry(new Guid(user.Id), condDate, false, 0, payload.notification.body, NotificationType.Push, alertType, plants, devs);
                return true;
            }
            catch (WebPushException exception)
            {
                Console.WriteLine("Http STATUS code" + exception.StatusCode);
                return false;
            }
        }

        [NonAction]
        public List<WarningRelatedObject> SimplifyAffectedObjects(ICollection<UserPlant> plants)
        {
            var res = new List<WarningRelatedObject>();
            foreach (UserPlant up in plants)
            {
                var temp = new WarningRelatedObject()
                {
                    ObjectName = up.Name,
                    ParentId = up.PlantId,
                    ObjectId = up.Id,
                    Action = GetAlertAction(up.PlantId, ReferenceToModelClass.Plant),
                    ObjectType = ReferenceToModelClass.UserPlant,
                    AlertConditionValue = GetAlertConditionValue(up.PlantId, ReferenceToModelClass.Plant),
                    IsInPot = up.IsInPot,
                    NotifyForWind = up.NotifyForWind,
                    NotifyForFrost= up.NotifyForFrost
                };
                res.Add(temp);
            }

            return res;
        }

        public List<WarningRelatedObject> SimplifyAffectedObjects(ICollection<Device> devices)
        {
            var res = new List<WarningRelatedObject>();
            foreach (Device up in devices)
            {
                var temp = new WarningRelatedObject()
                {
                    ObjectName = up.Name,
                    ParentId = up.Id,
                    ObjectId = up.Id,
                    Action = GetAlertAction(up.Id, ReferenceToModelClass.Plant),
                    ObjectType = ReferenceToModelClass.UserPlant,
                    AlertConditionValue = GetAlertConditionValue(up.Id, ReferenceToModelClass.Plant),
                    NotifyForWind = up.notifyForWind,
                    NotifyForFrost = up.notifyForWind
                };
                res.Add(temp);
            }
            return res;
        }

        public string GetAlertAction(int objectId, ModelEnums.ReferenceToModelClass objectType)
        {
            var alert = plantDB.Alert.Where(a => a.RelatedObjectId == objectId && a.ObjectType == objectType && !a.Deleted).FirstOrDefault();
            if (alert != null)
            {
                return alert.Text;
            }
            return "";
        }
        public float GetAlertConditionValue(int objectId, ModelEnums.ReferenceToModelClass objectType)
        {
            var alert = plantDB.Alert.Where(a => a.RelatedObjectId == objectId && a.ObjectType == objectType && !a.Deleted).Select(al=>al.Id).FirstOrDefault();
            var relatedAlertTrigger = (from a in plantDB.AlertTrigger where a.AlertId == alert select a).FirstOrDefault();
            float alertCondition = 0;
            if (relatedAlertTrigger != null)
            {
                if (plantDB.AlertCondition.Where(alc => alc.TriggerId == relatedAlertTrigger.Id).FirstOrDefault() != null)
                {
                    alertCondition = (float)(from a in plantDB.AlertTrigger where a.AlertId == alert join ac in plantDB.AlertCondition on a.Id equals ac.TriggerId select ac.FloatValue).FirstOrDefault();

                }

            }
            return alertCondition;
        }
        public bool IsFrostAlert(Alert alert)
        {
            return alert.Triggers.Any(t => t.Conditions.Any(c => c.ValueType == ComparedValueType.MinTemperature));
        }

        public bool IsStormAlert(Alert alert)
        {
            return alert.Triggers.Any(t => t.Conditions.Any(c => c.ValueType == ComparedValueType.MaxWindSpeed));
        }

        public AlertType GetAlertType(Alert alert)
        {
            return IsStormAlert(alert) ? AlertType.Storm : AlertType.Frost;
        }

        // returns true if more than 2 days have passed since last notification and a new condition is happening
        public async Task<AlertableRes> UserCanBeAlerted(Property prop, DateTime? lastAlerted, WarningViewModel wvm)
        {
            var res = new AlertableRes();
            DateTime newConditionDate = wvm.Date;
            var latestConditionDate = lastAlerted.GetValueOrDefault().AddDays(1);
            if (newConditionDate.Subtract(latestConditionDate).Days < 1)
            {
                return res;   // too soon
            }
            if (newConditionDate.Subtract(latestConditionDate).Days > 7)
            {
                res.CanAlertForStorm = true;    // alert once a week at the least
                res.CanAlertForFrost = true;
                return res;
            }
            int days = Convert.ToInt32(newConditionDate.Subtract(latestConditionDate).Days);
            var forecast = await wh.getWarningForecastByDate(prop, latestConditionDate.AddDays(1), days);

            // is it still the same condition?
            for (var i = 0; i < forecast.Forecasts.Count() - 1; i++)
            {
                // Frost related
                WeeklyForecast day = forecast.Forecasts.ElementAt(i);
                if (day.MinAirTemperatureInCelsius >= wvm.UserSettings.FrostDegreeBuffer)
                {
                    var secDay = forecast.Forecasts.ElementAt(i + 1);
                    if (secDay.MinAirTemperatureInCelsius >= wvm.UserSettings.FrostDegreeBuffer)
                    {
                        res.CanAlertForFrost = true;
                    }
                }
                //Storm related
                if (day.maxWindSpeedInKilometerPerHour <= MIN_WARNING_WIND_SPEED)  // considering 60 kmh the minimum for a storm
                {
                    var secDay = forecast.Forecasts.ElementAt(i + 1);
                    if (secDay.maxWindSpeedInKilometerPerHour <= MIN_WARNING_WIND_SPEED)
                    {
                        res.CanAlertForStorm = true;
                    }
                }
            }

            return res;
        }

        // returns max wind speed if it's higher than MIN_WARNING_WIND_SPEED, else 0
        public float GetMaxWindSpeedInPeriod(WeeklyForecastsObj forecast)
        {
            float maxSpeed = 0;
            foreach (WeeklyForecast day in forecast.Forecasts)
            {
                if (day.maxWindSpeedInKilometerPerHour > maxSpeed)
                {
                    maxSpeed = day.maxWindSpeedInKilometerPerHour;
                }
            }
            return maxSpeed >= MIN_WARNING_WIND_SPEED ? maxSpeed : 0;
        }

        public float GetMinTempInPeriod(WeeklyForecastsObj forecast)
        {
            float minTemp = 100;
            foreach (WeeklyForecast day in forecast.Forecasts)
            {
                if (day.MinAirTemperatureInCelsius < minTemp)
                {
                    minTemp = day.MinAirTemperatureInCelsius;
                }
            }
            return minTemp;
        }

        public AlertType GetAlertTypeForDevice(Device dev, GardifyModels.Models.WeatherHelpers.WeeklyForecastsObj forecast, float frostBuffer)
        {
            var isFrostCond = GetMinTempInPeriod(forecast) <= frostBuffer;
            var isStormCond = GetMaxWindSpeedInPeriod(forecast) >= MIN_WARNING_WIND_SPEED;
            var res = AlertType.NotSet;

            if (dev.notifyForFrost && isFrostCond)
            {
                res = AlertType.Frost;
            }

            if (dev.notifyForWind && isStormCond)
            {
                if (res == AlertType.Frost)
                {
                    return AlertType.FrostAndStorm;
                }
                return AlertType.Storm;
            }

            return res;
        }

        public bool CreateUserWarningEntry(
            Guid userId,
            DateTime condDate,
            bool dismissed,
            float condValue,
            string body,
            NotificationType notifyType,
            AlertType alertType,
            IEnumerable<UserPlantDetailsViewModel> userPlants,
            IEnumerable<Device> userDevices)
        {
            var userWarningEntry = new UserWarning()
            {
                UserId = userId,
                ConditionDate = condDate,
                Dismissed = dismissed,
                ConditionValue = condValue,
                Title = alertType == AlertType.Frost ? "Es gibt Frostwarnungen" : "Es gibt Sturmwarnungen",
                Body = body,
                NotificationType = notifyType
            };

            foreach (UserPlantDetailsViewModel up in userPlants)
            {
                var toAdd = plantDB.UserPlants.Where(u => u.Id == up.Id && !u.Deleted).FirstOrDefault();
                userWarningEntry.AffectedPlants.Add(toAdd);
            }
            foreach (Device ud in userDevices)
            {
                var toAdd = plantDB.Devices.Where(u => u.Id == ud.Id && !u.Deleted).FirstOrDefault();
                userWarningEntry.AffectedDevices.Add(toAdd);
            }
            userWarningEntry.OnCreate("System");
            var added = plantDB.UserWarnings.Add(userWarningEntry);

            try
            {
                plantDB.SaveChanges();
                return true;
            }
            catch (Exception ex) { return false; }
        }
    }

    public class PushNotificationPayload
    {
        public NotificationBody notification { get; set; }
    }

    public class NotificationBody
    {
        public NotificationBody()
        {
            icon = "assets/FavIcon/Gardify_Favicon.png";
            requireInteraction = true;
        }
        public string title { get; set; }
        public string body { get; set; }
        public string icon { get; set; }
        public bool requireInteraction { get; set; }
        public List<NotifyAction> actions { get; set; }
    }

    public class NotifyAction
    {
        public string action { get; set; }
        public string title { get; set; }
    }

    public class AlertableRes
    {
        public bool CanAlertForFrost { get; set; }
        public bool CanAlertForStorm { get; set; }
    }
}
