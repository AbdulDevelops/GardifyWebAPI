using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GardifyModels.Models
{


    public class WeatherObject
    {
        public WeatherHelpers.Location Location { get; set; }
        public WeatherHelpers.RelevantStation RelevantStation { get; set; }
        public DateTime ObservedAt { get; set; }
        public WeatherHelpers.AirTemperature AirTemperature { get; set; }
        public float AirPressureInHpa { get; set; }
        public WeatherHelpers.WindSpeed WindSpeed { get; set; }
        public WeatherHelpers.WindGust WindGust { get; set; }
        public int WindDirectionInDegree { get; set; }
        public WeatherHelpers.DewPointTemperature DewPointTemperature { get; set; }
        public WeatherHelpers.Precipitation Precipitation { get; set; }
        public int RelativeHumidityInPercent100based { get; set; }
        public int EffectiveCloudCoverInOcta { get; set; }
        public WeatherHelpers.Visibility Visibility { get; set; }
    }

    public class WeatherForecast
    {
        public WeatherHelpers.Forecasts Forecasts { get; set; }
    }

    public class WeatherHelpers
    {
        public class AirTemperature
        {
            public float Value { get; set; }
            public string Unit { get; set; }
        }
        public class Location
        {
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public string TimeZoneName { get; set; }
        }
        public class RelevantStation
        {
            public float Latitude { get; set; }
            public float Longitude { get; set; }
            public float HeightInMeters { get; set; }
            public int Id { get; set; }
            public int WmoId { get; set; }
            public string SourceType { get; set; }
            public string Name { get; set; }
        }
        public class WindSpeed
        {
            public float Value { get; set; }
            public string Unit { get; set; }
            public int TimeIntervalInMinutes { get; set; }
        }
        public class WindGust
        {
            public float Value { get; set; }
            public string Unit { get; set; }
            public int TimeIntervalInMinutes { get; set; }
        }
        public class DewPointTemperature
        {
            public float Value { get; set; }
            public string Unit { get; set; }
        }
        public class Precipitation
        {
            public float Value { get; set; }
            public string Unit { get; set; }
            public int TimeIntervalInMinutes { get; set; }
        }
        public class Visibility
        {
            public float Value { get; set; }
            public string Unit { get; set; }
        }

        public class Forecasts
        {
            public IEnumerable<HourlyForecast> Hourly { get; set; }
            public List<int> LocatedAt { get; set; }
            public DateTime ValidFrom { get; set; }
            public DateTime ValidUntil { get; set; }
            public float AverageWindSpeedInKilometerPerHour { get; set; }
            public string ValidPeriod { get; set; }

            // TODO interval6hours, interval12hours, daily
        }

        public class WeeklyForecastsObj
        {
            public string City { get; set; }
            public List<WeeklyForecast> Forecasts { get; set; }
        }
        public class ForecastAtDayLiteView
        {
            public float MinTemp { get; set; }
            public float MaxTemp { get; set; }
            public float MinWindSpeed { get; set; }
            public float MaxWindSpeed { get; set; }
            public DateTime Date { get; set; }
        }
        public class WeeklyForecast
        {
            public List<float> LocatedAt { get; set; }
            public DateTime ValidFrom { get; set; }
            public DateTime ValidUntil { get; set; }
            public float MinAirTemperatureInCelsius { get; set; }
            public float MaxAirTemperatureInCelsius { get; set; }
            public float maxWindSpeedInKilometerPerHour { get; set; }
            public int SunshineDurationInMinutes { get; set; }
            public float PrecipitationAmountInMillimeter { get; set; }
            public string ValidPeriod { get; set; }
            public float CloudCoverLowerThan2000MeterInOcta { get; set; }
        }
        public class TodaySunsetandSunriseObj
        {
            public List<TodaySunsetandSunrise> SunsetandSunrises { get; set; }
        }
        public class TodaySunsetandSunrise
        {
            public DateTime CivilSunriseTime { get; set; }
            public DateTime CivilSunsetTime { get; set; }
            public DateTime NauticalSunriseTime { get; set; }
            public DateTime NauticalSunsetTime { get; set; }
            public DateTime AstronomicalSunriseTime { get; set; }
            public DateTime AstronomicalSunsetTime { get; set; }
            public DateTime OfficialSunriseTime { get; set; }
            public DateTime OfficialSunsetTime { get; set; }
            public List<float> LocatedAt { get; set; }
            public DateTime ValidAt { get; set; }
            public string LocationTimeZoneName { get; set; }
        }
        public class TodaysForecastObj
        {
            public List<HourByHourForecast> Forecasts { get; set; }
        }
        public class CurrentForecastObj
        {
            public List<HourByHourLightForecast> Forecasts { get; set; }
        }
        public class HourByHourLightForecast
        {
            public List<float> LocatedAt { get; set; }
            public DateTime ValidFrom { get; set; }
            public DateTime ValidUntil { get; set; }
            public float AirTemperatureInCelsius { get; set; }
            public float WindSpeedInKilometerPerHour { get; set; }
            public int WindDirectionInDegree { get; set; }
            public int WeatherCode { get; set; }
            public string ValidPeriod { get; set; }
            public float PrecipitationAmountInMillimeter { get; set; }
            public int WindSpeedInBeaufort { get; set; }
            public float CloudCoverLowerThan2000MeterInOcta { get; set; }
        }
        public class HourByHourForecast
        {
            public List<float> LocatedAt { get; set; }
            public DateTime ValidFrom { get; set; }
            public DateTime ValidUntil { get; set; }
            public float AirTemperatureInCelsius { get; set; }
            public float AirPressureAtSeaLevelInHectoPascal { get; set; }
            public float WindSpeedInKilometerPerHour { get; set; }
            public int WindDirectionInDegree { get; set; }
            public int PrecipitationProbabilityInPercent { get; set; }
            public int WeatherCode { get; set; }
            public string ValidPeriod { get; set; }
            public float PrecipitationAmountInMillimeter { get; set; }
            public int WindSpeedInBeaufort { get; set; }
            public int RelativeHumidityInPercent { get; set; }
            public float CloudCoverLowerThan2000MeterInOcta { get; set; }
        }

        public class HourlyForecast
        {
            public DateTime ValidAt { get; set; }
            public AirTemperature AirTemperature { get; set; }
            public DewPointTemperature DewPointTemperature { get; set; }
            public float AirPressureInHpa { get; set; }
            public Precipitation PrecipitationPastInterval { get; set; }
            public WindSpeed AverageWindSpeed { get; set; }
            public WindGust WindGust { get; set; }
            public float WindDirectionInDegree { get; set; }
            public int EffectiveCloudCoverInOcta { get; set; }
            public float GlobalRadiationPast1HourInJoulePerCentimeterSquare { get; set; }
            public float RelativeHumidityInPercent100Based { get; set; }
        }
    }

}

/*
{"location":{
	"latitude":51.51054,
	"longitude":7.45674,
	"timeZoneName":"Europe/Berlin"
},
"forecasts":{
		"hourly":[
		{
			"validAt":"2016-05-09T16:00:00+02:00",
			"airTemperature":{
				"value":24.4,
				"unit":"DEGREE_CELSIUS"
			},
			"dewPointTemperature":{
				"value":9.1,
				"unit":"DEGREE_CELSIUS"
			},
			"airPressureInHpa":1013.1,
			"precipitationPastInterval":{
				"value":0.0,
				"unit":"MILLIMETER"
			},
			"averageWindSpeed":{
				"value":6.02,
				"unit":"METER_PER_SECOND",
				"timeIntervalInMinutes":-60
			},
			"windGust":{
				"value":19.8,
				"unit":"METER_PER_SECOND",
				"timeIntervalInMinutes":-60
			},
			"windDirectionInDegree":100.0,
			"effectiveCloudCoverInOcta":1,
			"globalRadiationPast1HourInJoulePerCentimeterSquare":250.0,
			"relativeHumidityInPercent100Based":38.0
		},
	
	{"validAt":"2016-05-09T17:00:00+02:00",
	"airTemperature":{
		"value":24.2,
	"unit":"DEGREE_CELSIUS"
	},
	"dewPointTemperature":{
		"value":9.3,
	"unit":"DEGREE_CELSIUS"
	},
	"airPressureInHpa":1012.9,
	"precipitationPastInterval":{
		"value":0.0,
	"unit":"MILLIMETER"
	},
	"averageWindSpeed":{
		"value":6.02,
	"unit":"METER_PER_SECOND",
	"timeIntervalInMinutes":-60
	},
	"windGust":{
		"value":22.3,
	"unit":"METER_PER_SECOND",
	"timeIntervalInMinutes":-60
	},
	"windDirectionInDegree":99.0,
	"effectiveCloudCoverInOcta":1,
	"globalRadiationPast1HourInJoulePerCentimeterSquare":223.0,
	"relativeHumidityInPercent100Based":39.0
	},{"validAt":"2016-05-09T18:00:00+02:00",
	"airTemperature":{
		"value":23.8,
	"unit":"DEGREE_CELSIUS"
	},
	"dewPointTemperature":{
		"value":9.4,
	"unit":"DEGREE_CELSIUS"
	},
	"airPressureInHpa":1012.7,
	"precipitationPastInterval":{
		"value":0.0,
	"unit":"MILLIMETER"
	},
	"averageWindSpeed":{
		"value":5.35,
	"unit":"METER_PER_SECOND",
	"timeIntervalInMinutes":-60
	},
	"windGust":{
		"value":22.4,
	"unit":"METER_PER_SECOND",
	"timeIntervalInMinutes":-60
	},
	"windDirectionInDegree":96.0,
	"effectiveCloudCoverInOcta":1,
	"globalRadiationPast1HourInJoulePerCentimeterSquare":174.0,
	"relativeHumidityInPercent100Based":40.0
	},{"validAt":"2016-05-09T19:00:00+02:00",
	"airTemperature":{
		"value":22.8,
	"unit":"DEGREE_CELSIUS"
	},
	"dewPointTemperature":{
		"value":9.6,
	"unit":"DEGREE_CELSIUS"
	},
	"airPressureInHpa":1012.6,
	"precipitationPastInterval":{
		"value":0.0,
	"unit":"MILLIMETER"
	},
	"averageWindSpeed":{
		"value":5.04,
	"unit":"METER_PER_SECOND",
	"timeIntervalInMinutes":-60
	},
	"windGust":{
		"value":22.6,
	"unit":"METER_PER_SECOND",
	"timeIntervalInMinutes":-60
	},
	"windDirectionInDegree":93.0,
	"effectiveCloudCoverInOcta":1,
	"globalRadiationPast1HourInJoulePerCentimeterSquare":117.0,
	"relativeHumidityInPercent100Based":43.0
	},{"validAt":"2016-05-09T20:00:00+02:00",
	"airTemperature":{
		"value":21.3,
	"unit":"DEGREE_CELSIUS"
	},
	"dewPointTemperature":{
		"value":10.0,
	"unit":"DEGREE_CELSIUS"
	},
	"airPressureInHpa":1012.6,
	"sunshineDurationPastIntervalInMinutes":792,
	"precipitationPastInterval":{
		"value":0.0,
	"unit":"MILLIMETER"
	},
	"precipitationProbabilityInPercent100based":3,
	"averageWindSpeed":{
		"value":4.78,
	"unit":"METER_PER_SECOND",
	"timeIntervalInMinutes":-60
	},
	"windGust":{
		"value":22.0,
	"unit":"METER_PER_SECOND",
	"timeIntervalInMinutes":-60
	},
	"windDirectionInDegree":89.0,
	"effectiveCloudCoverInOcta":1,
	"globalRadiationPast1HourInJoulePerCentimeterSquare":65.0,
	"relativeHumidityInPercent100Based":49.0
	},{"validAt":"2016-05-09T21:00:00+02:00",
	"airTemperature":{
		"value":19.7,
	"unit":"DEGREE_CELSIUS"
	},
	"dewPointTemperature":{
		"value":10.2,
	"unit":"DEGREE_CELSIUS"
	},
	"airPressureInHpa":1012.9,
	"precipitationPastInterval":{
		"value":0.0,
	"unit":"MILLIMETER"
	},
	"averageWindSpeed":{
		"value":4.42,
	"unit":"METER_PER_SECOND",
	"timeIntervalInMinutes":-60
	},
	"windGust":{
		"value":20.6,
	"unit":"METER_PER_SECOND",
	"timeIntervalInMinutes":-60
	},
	"windDirectionInDegree":84.0,
	"effectiveCloudCoverInOcta":1,
	"globalRadiationPast1HourInJoulePerCentimeterSquare":20.0,
	"relativeHumidityInPercent100Based":55.0
	},{"validAt":"2016-05-10T02:00:00+02:00",
	"airTemperature":{
		"value":15.1,
	"unit":"DEGREE_CELSIUS"
	},
	"dewPointTemperature":{
		"value":9.5,
	"unit":"DEGREE_CELSIUS"
	},
	"airPressureInHpa":1012.9,
	"sunshineDurationPastIntervalInMinutes":780,
	"precipitationPastInterval":{
		"value":0.0,
	"unit":"MILLIMETER"
	},
	"precipitationProbabilityInPercent100based":5,
	"averageWindSpeed":{
		"value":3.19,
	"unit":"METER_PER_SECOND",
	"timeIntervalInMinutes":-60
	},
	"windGust":{
		"value":13.9,
	"unit":"METER_PER_SECOND",
	"timeIntervalInMinutes":-60
	},
	"windDirectionInDegree":72.0,
	"effectiveCloudCoverInOcta":1,
	"globalRadiationPast1HourInJoulePerCentimeterSquare":0.0,
	"relativeHumidityInPercent100Based":70.0}]}
}

	*/
