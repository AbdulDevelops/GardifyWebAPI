using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using static GardifyModels.Models.WeatherHelpers;

namespace GardifyWebAPI.Controllers
{
    public class WeatherAPIController : ApiController
    {
        [Route("api/WeatherAPI/daily")]
        public async Task<WeeklyForecastsObj> GetWeatherForecast(int days = 8, bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            WeeklyForecastsObj res = new WeeklyForecastsObj();
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
            {
                return res;
            }
            
              if (isIos)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromIos, (int)GardifyPages.Weather, EventObjectType.PageName);
                }
                else if (isAndroid)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromAndroid, (int)GardifyPages.Weather, EventObjectType.PageName);
                }
                else if (isWebPage)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage, (int)GardifyPages.Weather, EventObjectType.PageName);
                }

           
            else
            {
                new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), 0, (int)GardifyPages.Weather, EventObjectType.PageName);
            }
            WeatherHandler wh = new WeatherHandler();
            PropertyController pc = new PropertyController();
            Property p = pc.DbGetProperty(userId);
            res = await wh.getForecastByProperty(p, days);
            res.City = p.City == "Platzhalter" ? "PLZ: " + p.Zip : p.City;
            return res;
        }

        [Route("api/WeatherAPI/forecast")]
        public Task<TodaysForecastObj> GetTodaysWeather()
        {
            WeatherHandler wh = new WeatherHandler();
            PropertyController pc = new PropertyController();
            Property p = pc.DbGetProperty(Utilities.GetUserId());
            return wh.getTodaysWeatherByProperty(p);
        }
        [Route("api/WeatherAPI/todayForecast")]
        public Task<TodaysForecastObj> GetTodayWeather()
        {
            WeatherHandler wh = new WeatherHandler();
            PropertyController pc = new PropertyController();
            Property p = pc.DbGetProperty(Utilities.GetUserId());
            return wh.getOnlyTodaysWeatherByProperty(p);
        }
        [Route("api/WeatherAPI/tomorrowForecast")]
        public Task<TodaysForecastObj> GetTomorrowWeather()
        {
            WeatherHandler wh = new WeatherHandler();
            PropertyController pc = new PropertyController();
            Property p = pc.DbGetProperty(Utilities.GetUserId());
            return wh.getOnlyTomorrowsWeatherByProperty(p);
        }

        [Route("api/WeatherAPI/current")]
        public Task<CurrentForecastObj> GetCurrentWeather()
        {
            WeatherHandler wh = new WeatherHandler();
            PropertyController pc = new PropertyController();
            Property p = pc.DbGetProperty(Utilities.GetUserId());
            return wh.getCurrentWeatherByProperty(p);
        }

        [Route("api/WeatherAPI/sun")]
        public Task<TodaySunsetandSunriseObj> GetSunriseAndSunset()
        {
            WeatherHandler wh = new WeatherHandler();
            PropertyController pc = new PropertyController();
            Property p = pc.DbGetProperty(Utilities.GetUserId());
            if (p != null)
            {
                return wh.getSunsetSunriseByGeoCoords(p.Longtitude, p.Latitude);
            }
            return null;
        }
    }
}
