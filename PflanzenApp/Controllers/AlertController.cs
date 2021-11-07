using PflanzenApp.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
{
    public class AlertController : _BaseController
    {
        // GET: Alert
        public ActionResult Index()
        {
            WeatherHandler wh = new WeatherHandler();
            WeatherForecast forecast = wh.getWeatherForecastByGeoCoords(51.5105434f, 7.4567403f);

            if (forecast != null)
            {
                ViewBag.MinTemp = wh.getMinTemperature(forecast.Forecasts.Hourly);
                ViewBag.MaxTemp = wh.getMaxTemperature(forecast.Forecasts.Hourly);
                ViewBag.MinSpeed = wh.getMinWindspeed(forecast.Forecasts.Hourly);
                ViewBag.MaxSpeed = wh.getMaxWindspeed(forecast.Forecasts.Hourly);

                AlertCondition con1 = new AlertCondition
                {
                    ValueType = ModelEnums.ComparedValueType.MaxTemperature,
                    ComparisonOperator = ModelEnums.ComparisonOperator.Greater,
                    FloatValue = 15,

                };
                con1.ReadableCondition = DbGetReadableCondition(con1);

                AlertCondition con2 = new AlertCondition
                {
                    ValueType = ModelEnums.ComparedValueType.MaxWindSpeed,
                    ComparisonOperator = ModelEnums.ComparisonOperator.Greater,
                    FloatValue = 1,
                };
                con2.ReadableCondition = DbGetReadableCondition(con2);

                AlertCondition con3 = new AlertCondition
                {
                    ValueType = ModelEnums.ComparedValueType.MaxWindSpeed,
                    ComparisonOperator = ModelEnums.ComparisonOperator.Greater,
                    FloatValue = 11,
                };
                con3.ReadableCondition = DbGetReadableCondition(con3);

                AlertCondition con4 = new AlertCondition
                {
                    ValueType = ModelEnums.ComparedValueType.MinTemperature,
                    ComparisonOperator = ModelEnums.ComparisonOperator.NotEqual,
                    FloatValue = 11,
                };
                con4.ReadableCondition = DbGetReadableCondition(con4);

                AlertCondition con5 = new AlertCondition
                {
                    ValueType = ModelEnums.ComparedValueType.MaxTemperature,
                    ComparisonOperator = ModelEnums.ComparisonOperator.Less,
                    FloatValue = 11,
                };
                con5.ReadableCondition = DbGetReadableCondition(con5);

                AlertCondition con6 = new AlertCondition
                {
                    ValueType = ModelEnums.ComparedValueType.MinTemperature,
                    ComparisonOperator = ModelEnums.ComparisonOperator.Greater,
                    FloatValue = -5,
                };
                con6.ReadableCondition = DbGetReadableCondition(con6);

                ViewBag.con1 = DbCheckCondition(con1, forecast);
                ViewBag.con2 = DbCheckCondition(con2, forecast);
                ViewBag.con3 = DbCheckCondition(con3, forecast);
                ViewBag.con4 = DbCheckCondition(con4, forecast);
                ViewBag.con5 = DbCheckCondition(con5, forecast);
                ViewBag.con6 = DbCheckCondition(con6, forecast);

                ViewBag.conName1 = con1.ReadableCondition;
                ViewBag.conName2 = con2.ReadableCondition;
                ViewBag.conName3 = con3.ReadableCondition;
                ViewBag.conName4 = con4.ReadableCondition;
                ViewBag.conName5 = con5.ReadableCondition;
                ViewBag.conName6 = con6.ReadableCondition;

                AlertCondition dateCon1 = new AlertCondition
                {
                    ValueType = ModelEnums.ComparedValueType.DaysSpan,
                    DateValue = DateTime.Parse("10.06.2016"),
                    ComparisonOperator = ModelEnums.ComparisonOperator.GreaterOrEqual,
                    FloatValue = 12,
                };
                dateCon1.ReadableCondition = DbGetReadableCondition(dateCon1);

                AlertCondition dateCon2 = new AlertCondition
                {
                    ValueType = ModelEnums.ComparedValueType.DaysSpan,
                    DateValue = DateTime.Parse("10.06.2016"),
                    ComparisonOperator = ModelEnums.ComparisonOperator.Less,
                    FloatValue = 4,
                };
                dateCon2.ReadableCondition = DbGetReadableCondition(dateCon2);

                AlertCondition dateCon3 = new AlertCondition
                {
                    ValueType = ModelEnums.ComparedValueType.ActualMonth,
                    ComparisonOperator = ModelEnums.ComparisonOperator.Equal,
                    DateValue = DateTime.Parse("10.06.2016"),
                };
                dateCon3.ReadableCondition = DbGetReadableCondition(dateCon3);

                AlertCondition dateCon4 = new AlertCondition
                {
                    ValueType = ModelEnums.ComparedValueType.ActualMonth,
                    ComparisonOperator = ModelEnums.ComparisonOperator.Greater,
                    DateValue = DateTime.Parse("10.07.2016"),
                };
                dateCon4.ReadableCondition = DbGetReadableCondition(dateCon4);

                AlertCondition dateCon5 = new AlertCondition
                {
                    ValueType = ModelEnums.ComparedValueType.YearsSpan,
                    DateValue = DateTime.Parse("10.06.2000"),
                    ComparisonOperator = ModelEnums.ComparisonOperator.Equal,
                    FloatValue = 16,
                };
                dateCon5.ReadableCondition = DbGetReadableCondition(dateCon5);

                AlertCondition dateCon6 = new AlertCondition
                {
                    ValueType = ModelEnums.ComparedValueType.YearsSpan,
                    DateValue = DateTime.Parse("10.06.2032"),
                    ComparisonOperator = ModelEnums.ComparisonOperator.Equal,
                    FloatValue = 16,
                };
                dateCon6.ReadableCondition = DbGetReadableCondition(dateCon6);

                ViewBag.dateCon1 = DbCheckCondition(dateCon1, forecast);
                ViewBag.dateCon2 = DbCheckCondition(dateCon2, forecast);
                ViewBag.dateCon3 = DbCheckCondition(dateCon3, forecast);
                ViewBag.dateCon4 = DbCheckCondition(dateCon4, forecast);
                ViewBag.dateCon5 = DbCheckCondition(dateCon5, forecast);
                ViewBag.dateCon6 = DbCheckCondition(dateCon6, forecast);

                ViewBag.dateConName1 = dateCon1.ReadableCondition;
                ViewBag.dateConName2 = dateCon2.ReadableCondition;
                ViewBag.dateConName3 = dateCon3.ReadableCondition;
                ViewBag.dateConName4 = dateCon4.ReadableCondition;
                ViewBag.dateConName5 = dateCon5.ReadableCondition;
                ViewBag.dateConName6 = dateCon6.ReadableCondition;

                AlertTrigger trigger1 = new AlertTrigger
                {
                    AlertId = 0,
                    Conditions = new List<AlertCondition> { con1, con2 }
                };

                AlertTrigger trigger2 = new AlertTrigger
                {
                    AlertId = 0,
                    Conditions = new List<AlertCondition> { con1, con2, con3 }
                };

                AlertTrigger trigger3 = new AlertTrigger
                {
                    AlertId = 0,
                    Conditions = new List<AlertCondition> { con3, con4, con5 }
                };

                AlertTrigger trigger4 = new AlertTrigger
                {
                    AlertId = 0,
                    Conditions = new List<AlertCondition> { dateCon2, dateCon5, con2 }
                };

                AlertTrigger trigger5 = new AlertTrigger
                {
                    AlertId = 0,
                    Conditions = new List<AlertCondition> { dateCon5, dateCon3 }
                };

                ViewBag.trigger1 = DbCheckTrigger(trigger1, forecast);
                ViewBag.trigger2 = DbCheckTrigger(trigger2, forecast);
                ViewBag.trigger3 = DbCheckTrigger(trigger3, forecast);
                ViewBag.trigger4 = DbCheckTrigger(trigger4, forecast);
                ViewBag.trigger5 = DbCheckTrigger(trigger5, forecast);

                ViewBag.triggerName1 = trigger1.ReadableCondition;
                ViewBag.triggerName2 = trigger2.ReadableCondition;
                ViewBag.triggerName3 = trigger3.ReadableCondition;
                ViewBag.triggerName4 = trigger4.ReadableCondition;
                ViewBag.triggerName5 = trigger5.ReadableCondition;

                Alert alert1 = new Alert
                {
                    Triggers = new List<AlertTrigger> { trigger1, trigger2 }
                };

                Alert alert2 = new Alert
                {
                    Triggers = new List<AlertTrigger> { trigger2, trigger3 }
                };

                Alert alert3 = new Alert
                {
                    Triggers = new List<AlertTrigger> { trigger5, trigger2 }
                };

                Alert alert4 = new Alert
                {
                    Triggers = new List<AlertTrigger> { trigger3, trigger5 }
                };

                ViewBag.alert1 = DbCheckAlert(alert1, forecast);
                ViewBag.alert2 = DbCheckAlert(alert2, forecast);
                ViewBag.alert3 = DbCheckAlert(alert3, forecast);
                ViewBag.alert4 = DbCheckAlert(alert4, forecast);

                ViewBag.AlertName1 = alert1.ReadableCondition;
                ViewBag.AlertName2 = alert2.ReadableCondition;
                ViewBag.AlertName3 = alert3.ReadableCondition;
                ViewBag.AlertName4 = alert4.ReadableCondition;
            }

            return View();
        }

        // GET: Alert/Details/5
        public ActionResult Details(int id)
        {
            return View();
        }

        // GET: Alert/Create
        public ActionResult CreateAlert(int? relatedObjectId, ModelEnums.ReferenceToModelClass? objectType)
        {
            if (relatedObjectId != null && objectType != null)
            {
                AlertViewModels.AlertViewModel viewModel = new AlertViewModels.AlertViewModel();
                viewModel.RelatedObjectId = (int)relatedObjectId;
                viewModel.ObjectType = (ModelEnums.ReferenceToModelClass)objectType;
                return View(viewModel);
            }
            else
            {
                return HttpNotFound();
            }
        }

        // POST: Alert/CreateAlert
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAlert(AlertViewModels.AlertViewModel alertData)
        {
            if (ModelState.IsValid)
            {
                Alert newAlert = new Alert
                {
                    Title = alertData.Title,
                    Text = alertData.Text,
                    ObjectType = alertData.ObjectType,
                    RelatedObjectId = alertData.RelatedObjectId,
                    CreatedBy = User.Identity.Name
                };
                HelperClasses.DbResponse response = DbCreateAlert(newAlert);

                if (response.Status == ModelEnums.ActionStatus.Success)
                {
                    return RedirectToAction("EditAlert", new { id = ((Alert)response.ResponseObjects.FirstOrDefault()).Id });
                }
            }
            return CreateAlert(alertData.RelatedObjectId, alertData.ObjectType);
        }

        // GET: Alert/Edit/5
        public ActionResult EditAlert(int? id)
        {
            if (id != null)
            {
                Alert alert = DbGetAlertById((int)id);

                if (alert != null)
                {
                    AlertViewModels.AlertViewModel viewModel = DbGetAlertViewModel(alert);
                    return View(viewModel);
                }

            }
            return HttpNotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAlert(AlertViewModels.AlertViewModel newData)
        {
            if (ModelState.IsValid)
            {
                Alert data = new Alert
                {
                    Id = newData.Id,
                    Title = newData.Title,
                    Text = newData.Text,
                    EditedBy = User.Identity.Name,
                };

                DbEditAlert(data);

                return RedirectToAction("EditAlert", new { id = newData.Id });
            }
            return RedirectToError("Seite konnte nicht gefunden werden.", HttpStatusCode.NotFound, "AlertController.EditAlert(" + newData.Title + ")");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAlert(int id, string returnUrl)
        {
            Alert alert = DbGetAlertById(id);
            if (alert != null)
            {
                bool isOk = DbDeleteAlert(id, User.Identity.Name);

                if (isOk)
                {
                    if (alert.ObjectType == ModelEnums.ReferenceToModelClass.Plant)
                    {
                        return RedirectToAction("Edit", "AdminAreaPlant", new { id = alert.RelatedObjectId });
                    }
                    else
                    {
                        RedirectToAction("Index", "AdminAreaPlant");
                    }
                }
                else
                {
                    return Redirect(returnUrl);
                }
            }

            return RedirectToError("Seite konnte nicht gefunden werden.", HttpStatusCode.NotFound, "AlertController.DeleteAlert(" + id + "," + returnUrl + ")");
        }

        // POST: Alert/CreateAlertCondition
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateAlertCondition(AlertViewModels.AlertConditionViewModel alertConditionData, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                AlertCondition newAlertCondition = new AlertCondition
                {
                    TriggerId = alertConditionData.TriggerId,
                    ComparisonOperator = alertConditionData.ComparisonOperator,
                    ValueType = alertConditionData.ValueType,
                    DateValue = alertConditionData.DateValue,
                    FloatValue = alertConditionData.FloatValue,
                    CreatedBy = User.Identity.Name
                };
                DbCreateAlertCondition(newAlertCondition);
            }
            return Redirect(returnUrl);
        }

        // POST: Alert/DeleteAlertCondition
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteAlertCondition(int alertConditionId, string returnUrl)
        {
            DbDeleteCondition(alertConditionId, User.Identity.Name);
            return Redirect(returnUrl);
        }

        #region DB

        [NonAction]
        public List<AlertViewModels.AlertViewModel> DbGetAlertViewModels(IEnumerable<Alert> alerts)
        {
            List<AlertViewModels.AlertViewModel> ret = new List<AlertViewModels.AlertViewModel>();
            if (alerts != null && alerts.Any())
            {
                foreach (Alert alert in alerts)
                {
                    ret.Add(DbGetAlertViewModel(alert));
                }
            }
            return ret;
        }

        [NonAction]
        public AlertViewModels.AlertViewModel DbGetAlertViewModel(Alert alert)
        {
            AlertViewModels.AlertViewModel alertViewModel = null;
            if (alert != null)
            {
                alertViewModel = new AlertViewModels.AlertViewModel
                {
                    Id = alert.Id,
                    ObjectType = alert.ObjectType,
                    RelatedObjectId = alert.RelatedObjectId,
                    Title = alert.Title,
                    Text = alert.Text,
                    ReadableCondition = alert.ReadableCondition,
                    Triggers = DbGetAlertTriggerViewModels(alert.Triggers)
                };
            }
            return alertViewModel;
        }

        [NonAction]
        public List<AlertViewModels.AlertTriggerViewModel> DbGetAlertTriggerViewModels(IEnumerable<AlertTrigger> triggers)
        {
            List<AlertViewModels.AlertTriggerViewModel> ret = new List<AlertViewModels.AlertTriggerViewModel>();
            if (triggers != null && triggers.Any(t => !t.Deleted))
            {
                foreach (AlertTrigger trigger in triggers.Where(t => !t.Deleted))
                {
                    ret.Add(DbGetAlertTriggerViewModel(trigger));
                }
            }
            return ret;
        }

        [NonAction]
        public AlertViewModels.AlertTriggerViewModel DbGetAlertTriggerViewModel(AlertTrigger trigger)
        {
            AlertViewModels.AlertTriggerViewModel triggerViewModel = null;
            if (trigger != null)
            {
                triggerViewModel = new AlertViewModels.AlertTriggerViewModel
                {
                    Id = trigger.Id,
                    AlertId = trigger.AlertId,
                    ReadableCondition = trigger.ReadableCondition,
                    Conditions = DbGetAlertConditionViewModels(trigger.Conditions)
                };
            }
            return triggerViewModel;
        }

        [NonAction]
        public List<AlertViewModels.AlertConditionViewModel> DbGetAlertConditionViewModels(IEnumerable<AlertCondition> conditions)
        {
            List<AlertViewModels.AlertConditionViewModel> ret = new List<AlertViewModels.AlertConditionViewModel>();
            if (conditions != null && conditions.Any(c => !c.Deleted))
            {
                foreach (AlertCondition condition in conditions.Where(c => !c.Deleted))
                {
                    ret.Add(DbGetAlertConditionViewModel(condition));
                }
            }
            return ret;
        }

        [NonAction]
        public AlertViewModels.AlertConditionViewModel DbGetAlertConditionViewModel(AlertCondition condition, bool checkCondition = false)
        {
            AlertViewModels.AlertConditionViewModel conditionViewModel = null;
            if (condition != null)
            {
                conditionViewModel = new AlertViewModels.AlertConditionViewModel
                {
                    Id = condition.Id,
                    TriggerId = condition.TriggerId,
                    ComparisonOperator = condition.ComparisonOperator,
                    ValueType = condition.ValueType,
                    DateValue = condition.DateValue,
                    FloatValue = condition.FloatValue,
                    ReadableCondition = condition.ReadableCondition
                };
            }
            return conditionViewModel;
        }

        [NonAction]
        public string DbGetReadableAlertCondition(Alert alert)
        {
            string ret = "";

            if (alert.Triggers != null && alert.Triggers.Any())
            {
                if (alert.Triggers.Count() > 1)
                {
                    ret += "(";
                }
                int count = 0;
                foreach (AlertTrigger trigger in alert.Triggers)
                {
                    if (count > 0)
                    {
                        ret += ") ODER (";
                    }
                    ret += DbGetReadableTriggerCondition(trigger);
                    count++;
                }
                if (alert.Triggers.Count() > 1)
                {
                    ret += ")";
                }
            }
            return ret;
        }

        [NonAction]
        public string DbGetReadableTriggerCondition(AlertTrigger trigger)
        {
            string ret = "";

            if (trigger.Conditions != null && trigger.Conditions.Any())
            {
                int count = 0;
                foreach (AlertCondition condition in trigger.Conditions)
                {
                    if (count > 0)
                    {
                        ret += " UND ";
                    }
                    ret += DbGetReadableCondition(condition);

                    count++;
                }
            }
            return ret;
        }

        [NonAction]
        public string DbGetReadableCondition(AlertCondition condition)
        {
            string ret = "";
            switch (condition.ValueType)
            {
                case ModelEnums.ComparedValueType.MinWindSpeed:
                    ret += "Minimale Windgeschwindigkeit ist " + CompOperatorToReadableString(condition.ComparisonOperator) + condition.FloatValue;
                    break;
                case ModelEnums.ComparedValueType.MaxWindSpeed:
                    ret += "Maximale Windgeschwindigkeit ist " + CompOperatorToReadableString(condition.ComparisonOperator) + condition.FloatValue;
                    break;
                case ModelEnums.ComparedValueType.MinTemperature:
                    ret += "Minimale Temperatur ist " + CompOperatorToReadableString(condition.ComparisonOperator) + condition.FloatValue;
                    break;
                case ModelEnums.ComparedValueType.MaxTemperature:
                    ret += "Maximale Temperatur ist " + CompOperatorToReadableString(condition.ComparisonOperator) + condition.FloatValue;
                    break;
                case ModelEnums.ComparedValueType.ActualYear:
                    ret += "Aktuelles Jahr ist " + CompOperatorToReadableString(condition.ComparisonOperator) + condition.FloatValue;
                    break;
                case ModelEnums.ComparedValueType.ActualMonth:
                    ret += "Aktueller Monat ist " + CompOperatorToReadableString(condition.ComparisonOperator) + condition.FloatValue;
                    break;
                case ModelEnums.ComparedValueType.ActualDay:
                    ret += "Aktueller Tag ist " + CompOperatorToReadableString(condition.ComparisonOperator) + condition.FloatValue;
                    break;
                case ModelEnums.ComparedValueType.ActualDate:
                    ret += "Aktuelles Datum ist " + CompOperatorToReadableString(condition.ComparisonOperator) + condition.DateValue.Value.Date;
                    break;
                case ModelEnums.ComparedValueType.YearsSpan:
                    ret += "Jahresintervall seit " + condition.DateValue.Value + " ist " + CompOperatorToReadableString(condition.ComparisonOperator) + condition.FloatValue;
                    break;
                case ModelEnums.ComparedValueType.MonthsSpan:
                    ret += "Monatsintervall seit " + condition.DateValue.Value + " ist " + CompOperatorToReadableString(condition.ComparisonOperator) + condition.FloatValue;
                    break;
                case ModelEnums.ComparedValueType.DaysSpan:
                    ret += "Tagesintervall seit " + condition.DateValue.Value + " ist " + CompOperatorToReadableString(condition.ComparisonOperator) + condition.FloatValue;
                    break;
                default:
                    ret += "Unbekannter Wert ";
                    break;
            }

            return ret;
        }

        [NonAction]
        private string CompOperatorToReadableString(ModelEnums.ComparisonOperator co)
        {
            string ret = "";
            switch (co)
            {
                case ModelEnums.ComparisonOperator.Less:
                    ret += "kleiner als ";
                    break;
                case ModelEnums.ComparisonOperator.LessOrEqual:
                    ret += "kleiner-gleich ";
                    break;
                case ModelEnums.ComparisonOperator.Greater:
                    ret += "größer als ";
                    break;
                case ModelEnums.ComparisonOperator.GreaterOrEqual:
                    ret += "größer-gleich ";
                    break;
                case ModelEnums.ComparisonOperator.Equal:
                    ret += "gleich ";
                    break;
                case ModelEnums.ComparisonOperator.NotEqual:
                    ret += "nicht gleich ";
                    break;
                default:
                    ret += "unbekanntes Vergleich ";
                    break;
            }
            return ret;
        }

        [NonAction]
        public HelperClasses.DbResponse DbCreateAlert(Alert newAlert)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            newAlert.OnCreate(newAlert.CreatedBy);
            ctx.Alert.Add(newAlert);
            ctx.SaveChanges();

            AlertTrigger defaultTrigger = new AlertTrigger
            {
                AlertId = newAlert.Id,
                CreatedBy = newAlert.CreatedBy,
            };
            defaultTrigger.OnCreate(defaultTrigger.CreatedBy);
            ctx.AlertTrigger.Add(defaultTrigger);

            bool isOk = ctx.SaveChanges() > 0 ? true : false;

            if (isOk)
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Created);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(newAlert);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }

        [NonAction]
        public Alert DbGetAlertById(int alertId)
        {
            Alert alert = (from a in ctx.Alert
                           where !a.Deleted && a.Id == alertId
                           select a).FirstOrDefault();

            return alert;
        }

        [NonAction]
        public bool DbEditAlert(Alert alertData)
        {
            Alert alertToEdit = (from a in ctx.Alert
                                 where !a.Deleted && a.Id == alertData.Id
                                 select a).FirstOrDefault();

            if (alertToEdit != null)
            {
                alertToEdit.Title = alertData.Title;
                alertToEdit.Text = alertData.Text;
                alertToEdit.OnEdit(alertData.EditedBy);
                return ctx.SaveChanges() > 0 ? true : false;
            }

            return false;
        }

        [NonAction]
        public IEnumerable<Alert> DbGetAlertsByRelatedObjectId(int objectId, ModelEnums.ReferenceToModelClass objectType)
        {
            var alert_sel = (from a in ctx.Alert.Include(a => a.Triggers.Select(t => t.Conditions))
                             where !a.Deleted && a.RelatedObjectId == objectId && a.ObjectType == objectType
                             select a);

            //var alert_sel = ctx.Alert.Include("Triggers.Conditions").Where(a => !a.Deleted && a.RelatedObjectId == objectId && a.ObjectType == objectType);

            if (alert_sel != null)
            {
                return alert_sel;
            }
            else
            {
                return null;
            }
        }

        [NonAction]
        public bool DbDeleteAlert(int alertId, string deletedBy)
        {
            Alert alert = (from a in ctx.Alert
                           where !a.Deleted && a.Id == alertId
                           select a).FirstOrDefault();
            if (alert != null)
            {
                alert.Deleted = true;
                alert.OnEdit(deletedBy);
                return ctx.SaveChanges() > 0 ? true : false;
            }
            return false;
        }

        [NonAction]
        public IEnumerable<Alert> DbGetTriggeredAlertsByPlantId(int plantId, WeatherForecast forecast)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            List<Alert> triggeredAlerts = new List<Alert>();

            long tmpTime = watch.ElapsedMilliseconds;
            IEnumerable<Alert> plantAlerts = DbGetAlertsByRelatedObjectId(plantId, ModelEnums.ReferenceToModelClass.Plant);
            Debug.WriteLine("Alerts für ID: " + plantId + " geladen in: " + (watch.ElapsedMilliseconds - tmpTime));

            tmpTime = watch.ElapsedMilliseconds;
            if (plantAlerts != null && plantAlerts.Any())
            {
                foreach (Alert alert in plantAlerts)
                {
                    bool isTriggered = DbCheckAlert(alert, forecast);
                    if (isTriggered)
                    {
                        triggeredAlerts.Add(alert);
                    }
                }
            }
            //Debug.WriteLine("Alerts für ID: " + plantId + " geprüft in: " + (watch.ElapsedMilliseconds - tmpTime));

            return triggeredAlerts;
        }

        [NonAction]
        public bool DbCreateAlertTrigger(AlertTrigger alertTriggerData)
        {
            alertTriggerData.OnCreate(alertTriggerData.CreatedBy);
            ctx.AlertTrigger.Add(alertTriggerData);
            return ctx.SaveChanges() > 0 ? true : false;
        }

        [NonAction]
        public IEnumerable<AlertTrigger> DbGetAlertTriggersByAlertId(int alertId)
        {
            IEnumerable<AlertTrigger> triggers = (from at in ctx.AlertTrigger
                                                  where !at.Deleted && at.AlertId == alertId
                                                  select at);

            if (triggers != null && triggers.Any())
            {
                foreach (AlertTrigger trigger in triggers)
                {
                    trigger.Conditions = DbGetAlertConditionByTriggerId(trigger.Id);
                }
            }

            return triggers;
        }

        [NonAction]
        public HelperClasses.DbResponse DbCreateAlertCondition(AlertCondition alertConditionData)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();
            alertConditionData.OnCreate(alertConditionData.CreatedBy);
            alertConditionData.ReadableCondition = DbGetReadableCondition(alertConditionData);
            ctx.AlertCondition.Add(alertConditionData);

            bool isOk = ctx.SaveChanges() > 0 ? true : false;

            if (isOk)
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Created);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(alertConditionData);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }

        [NonAction]
        public bool DbDeleteCondition(int id, string deletedBy)
        {
            AlertCondition cond = (from c in ctx.AlertCondition
                                   where !c.Deleted && c.Id == id
                                   select c).FirstOrDefault();

            if (cond != null)
            {
                cond.Deleted = true;
                cond.OnEdit(deletedBy);
                return ctx.SaveChanges() > 0 ? true : false;
            }

            return false;
        }

        [NonAction]
        public ICollection<AlertCondition> DbGetAlertConditionByTriggerId(int triggetId)
        {
            var trigger_sel = (from ac in ctx.AlertCondition
                               where !ac.Deleted && ac.TriggerId == triggetId
                               select ac);

            if (trigger_sel != null)
            {
                return trigger_sel.ToList();
            }
            else
            {
                return null;
            }
        }

        [NonAction]
        public bool DbCheckAlert(Alert alert, WeatherForecast forecast)
        {
            bool ret = false;

            if (alert.Triggers != null && alert.Triggers.Any())
            {
                // alle trigger werden mit ODER verknüpft. Also abbrechen sobald ein trigger true ist
                foreach (AlertTrigger trigger in alert.Triggers)
                {
                    bool triggerResult = DbCheckTrigger(trigger, forecast);
                    if (triggerResult == true)
                    {
                        return true;
                    }
                }
                ret = false;
            }

            return ret;
        }

        [NonAction]
        public bool DbCheckTrigger(AlertTrigger alertTrigger, WeatherForecast forecast)
        {
            bool ret = false;

            if (alertTrigger.Conditions != null && alertTrigger.Conditions.Any())
            {
                // alle bedingungen werden mit AND verknüpft. Also abbrechen sobald eine bedingung false ist
                foreach (AlertCondition condition in alertTrigger.Conditions)
                {
                    bool conditionResult = DbCheckCondition(condition, forecast);
                    if (conditionResult == false)
                    {
                        return false;
                    }
                }

                return true;
            }

            return ret;
        }

        [NonAction]
        public bool DbCheckCondition(AlertCondition condition, WeatherForecast forecast)
        {
            bool ret = false;

            // wenn DateValue nicht null ist, dann will man warscheinlich datum werte vergleichen
            if (condition.DateValue != null)
            {
                DateTime now = DateTime.Now;

                // falls DateValue UND FloatValue nicht null sind, dann werden komplexere vergleiche gemacht, wie "sind 5 tage seit 23.06.2016 vergangen?"
                if (condition.FloatValue != null)
                {
                    if (condition.ValueType == ModelEnums.ComparedValueType.DaysSpan || condition.ValueType == ModelEnums.ComparedValueType.MonthsSpan ||
                   condition.ValueType == ModelEnums.ComparedValueType.YearsSpan)
                    {

                        double timeSpan = 0;

                        switch (condition.ValueType)
                        {
                            case ModelEnums.ComparedValueType.YearsSpan:
                                timeSpan = Math.Abs(Math.Round((condition.DateValue.Value.Subtract(now).Days) / 365D));
                                break;
                            case ModelEnums.ComparedValueType.MonthsSpan:
                                timeSpan = Math.Abs(Math.Round((condition.DateValue.Value.Subtract(now).Days) / 30D));
                                break;
                            case ModelEnums.ComparedValueType.DaysSpan:
                                timeSpan = Math.Abs(condition.DateValue.Value.Subtract(now).Days);
                                break;
                        }
                        ret = compareFloatValues((float)timeSpan, (float)condition.FloatValue, condition.ComparisonOperator);
                    }
                }
                else if (condition.ValueType == ModelEnums.ComparedValueType.ActualDate)
                {
                    float condVal = 0;
                    float nowVal = 0;

                    switch (condition.ValueType)
                    {
                        case ModelEnums.ComparedValueType.ActualDate:
                            condVal = condition.DateValue.Value.Date.Ticks;
                            nowVal = now.Date.Ticks;
                            break;
                    }

                    ret = compareFloatValues(nowVal, condVal, condition.ComparisonOperator);
                }

            }
            // wenn FloatValue nicht null ist, dann will man warscheinlich float werte vergleichen
            else if (condition.FloatValue != null)
            {
                // compare float values
                float valueToCompare = 0;
                // if weather values - initiate weatherhandler
                if (condition.ValueType == ModelEnums.ComparedValueType.MinTemperature || condition.ValueType == ModelEnums.ComparedValueType.MaxTemperature ||
                    condition.ValueType == ModelEnums.ComparedValueType.MinWindSpeed || condition.ValueType == ModelEnums.ComparedValueType.MaxWindSpeed)
                {
                    if (forecast != null)
                    {
                        WeatherHandler wh = new WeatherHandler();

                        switch (condition.ValueType)
                        {
                            case ModelEnums.ComparedValueType.MinWindSpeed:
                                valueToCompare = wh.getMinWindspeed(forecast.Forecasts.Hourly);
                                break;
                            case ModelEnums.ComparedValueType.MaxWindSpeed:
                                valueToCompare = wh.getMaxWindspeed(forecast.Forecasts.Hourly);
                                break;
                            case ModelEnums.ComparedValueType.MinTemperature:
                                valueToCompare = wh.getMinTemperature(forecast.Forecasts.Hourly);
                                break;
                            case ModelEnums.ComparedValueType.MaxTemperature:
                                valueToCompare = wh.getMaxTemperature(forecast.Forecasts.Hourly);
                                break;
                            default:
                                break;
                        }
                    }
                }
                else if (condition.ValueType == ModelEnums.ComparedValueType.ActualDay || condition.ValueType == ModelEnums.ComparedValueType.ActualMonth ||
                  condition.ValueType == ModelEnums.ComparedValueType.ActualYear)
                {
                    DateTime now = DateTime.Now;

                    switch (condition.ValueType)
                    {
                        case ModelEnums.ComparedValueType.ActualYear:
                            valueToCompare = now.Year;
                            break;
                        case ModelEnums.ComparedValueType.ActualMonth:
                            valueToCompare = now.Month;
                            break;
                        case ModelEnums.ComparedValueType.ActualDay:
                            valueToCompare = now.Day;
                            break;
                    }
                }

                ret = compareFloatValues(valueToCompare, (float)condition.FloatValue, condition.ComparisonOperator);
            }

            return ret;
        }

        private bool compareFloatValues(float valLeft, float valRight, ModelEnums.ComparisonOperator compOperator)
        {
            bool ret = false;
            switch (compOperator)
            {
                case ModelEnums.ComparisonOperator.Less:
                    ret = valLeft < valRight;
                    break;
                case ModelEnums.ComparisonOperator.LessOrEqual:
                    ret = valLeft <= valRight;
                    break;
                case ModelEnums.ComparisonOperator.Greater:
                    ret = valLeft > valRight;
                    break;
                case ModelEnums.ComparisonOperator.GreaterOrEqual:
                    ret = valLeft >= valRight;
                    break;
                case ModelEnums.ComparisonOperator.Equal:
                    ret = valLeft == valRight;
                    break;
                case ModelEnums.ComparisonOperator.NotEqual:
                    ret = valLeft != valRight;
                    break;
                default:
                    break;
            }
            return ret;
        }

        #endregion
    }
}
