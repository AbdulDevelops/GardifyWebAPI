using GardifyModels.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

using System.Web.Script.Serialization;
using static GardifyModels.Models.WeatherHelpers;

namespace GardifyWebAPI.App_Code
{
	public class WeatherHandler
	{
		string login = "karljoest";
		string password = "Os23pQUJmzsnWxK7iaVSkSTvS9D6CTO8";
        string latitude = "51.165691";
        string longitude = "10.451526";
        //Gets forecast for tomorrow  00:00 bis 6h
        public async Task<TodaysForecastObj> getOnlyTomorrowsWeatherByProperty(Property prop)
		{
            if (prop != null)
            {
                latitude = prop.Latitude.ToString().Replace(",", ".");
                longitude = prop.Longtitude.ToString().Replace(",", ".");
            }
			var validFrom = DateTime.Now.Date.AddHours(23).ToString("yyyy-MM-ddTHH:mm:ssZ");
			var validUntil = DateTime.Now.Date.AddHours(38).ToString("yyyy-MM-ddTHH:mm:ssZ");
			string url = "https://point-forecast.weather.mg/search?fields=cloudCoverLowerThan2000MeterInOcta,airPressureAtSeaLevelInHectoPascal,airTemperatureInCelsius,precipitationAmountInMillimeter,precipitationProbabilityInPercent,weatherCode,weatherCodeTraditional,windSpeedInKilometerPerHour,relativeHumidityInPercent,windDirectionInDegree,windSpeedInBeaufort&validPeriod=PT0S,PT1H&validFrom=" + validFrom + "&validUntil=" + validUntil + "&locatedAt=" + longitude + "," + latitude;

			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers.Add("Authorization", "Bearer " + await GetAccessToken(login, password));

			string rawData = performRequest(url, "GET", null, headers);

			return JsonConvert.DeserializeObject<TodaysForecastObj>(rawData);
		}
		//Gets forecast for today 6h AM bis 00:00
		public async Task<TodaysForecastObj> getOnlyTodaysWeatherByProperty(Property prop)
		{
            if (prop != null)
            {
                latitude = prop.Latitude.ToString().Replace(",", ".");
                longitude = prop.Longtitude.ToString().Replace(",", ".");
            }
			var validFrom = DateTime.Now.Date.AddHours(6).ToString("yyyy-MM-ddTHH:mm:ssZ");
			var validUntil = DateTime.Now.Date.AddHours(23).ToString("yyyy-MM-ddTHH:mm:ssZ");
			string url = "https://point-forecast.weather.mg/search?fields=cloudCoverLowerThan2000MeterInOcta,airPressureAtSeaLevelInHectoPascal,airTemperatureInCelsius,precipitationAmountInMillimeter,precipitationProbabilityInPercent,weatherCode,weatherCodeTraditional,windSpeedInKilometerPerHour,relativeHumidityInPercent,windDirectionInDegree,windSpeedInBeaufort&validPeriod=PT0S,PT1H&validFrom=" + validFrom + "&validUntil=" + validUntil + "&locatedAt=" + longitude + "," + latitude;

			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers.Add("Authorization", "Bearer " + await GetAccessToken(login, password));

			string rawData = performRequest(url, "GET", null, headers);

			return JsonConvert.DeserializeObject<TodaysForecastObj>(rawData);
		}
		// Gets forecast for the next 24H
		public async Task<TodaysForecastObj> getTodaysWeatherByProperty(Property prop)
		{
            if (prop != null)
            {
                latitude = prop.Latitude.ToString().Replace(",", ".");
                longitude = prop.Longtitude.ToString().Replace(",", ".");
            }
			var validFrom = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
			var validUntil = DateTime.Today.AddDays(1).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
			string url = "https://point-forecast.weather.mg/search?fields=cloudCoverLowerThan2000MeterInOcta,airPressureAtSeaLevelInHectoPascal,airTemperatureInCelsius,precipitationAmountInMillimeter,precipitationProbabilityInPercent,weatherCode,weatherCodeTraditional,windSpeedInKilometerPerHour,relativeHumidityInPercent,windDirectionInDegree,windSpeedInBeaufort&validPeriod=PT0S,PT1H&validFrom=" + validFrom + "&validUntil=" + validUntil + "&locatedAt=" + longitude + "," + latitude;

			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers.Add("Authorization", "Bearer " + await GetAccessToken(login,password));
			
			string rawData = performRequest(url, "GET", null, headers);

			return JsonConvert.DeserializeObject<TodaysForecastObj>(rawData);
		}

		public async Task<CurrentForecastObj> getCurrentWeatherByProperty(Property prop)
		{
            if (prop != null)
            {
                latitude = prop.Latitude.ToString().Replace(",", ".");
                longitude = prop.Longtitude.ToString().Replace(",", ".");
            }
			var validFrom = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
			var validUntil = DateTime.UtcNow.AddHours(1).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");
			string url = "https://point-forecast.weather.mg/search?fields=cloudCoverLowerThan2000MeterInOcta,airTemperatureInCelsius,precipitationAmountInMillimeter,weatherCode,windSpeedInKilometerPerHour,windDirectionInDegree,windSpeedInBeaufort&validPeriod=PT0S,PT1H&validFrom=" + validFrom + "&validUntil=" + validUntil + "&locatedAt=" + longitude + "," + latitude;

			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers.Add("Authorization", "Bearer " + await GetAccessToken(login, password));

			string rawData = performRequest(url, "GET", null, headers);

			return JsonConvert.DeserializeObject<CurrentForecastObj>(rawData);
		}

		// Gets forecast for n-days in 24H intervals
		public async Task<WeeklyForecastsObj> getForecastByProperty(Property prop, int daysTrend = 8)
		{
            if (prop != null)
            {
                latitude = prop.Latitude.ToString().Replace(",", ".");
                longitude = prop.Longtitude.ToString().Replace(",", ".");
            }
			var validFrom = DateTime.UtcNow.ToString("yyyy-MM-ddTHH:mm:ssZ");
			var validUntil = DateTime.Today.AddDays(daysTrend).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

			string url = "https://point-forecast.weather.mg/search?fields=cloudCoverLowerThan2000MeterInOcta,maxAirTemperatureInCelsius,weatherCode,minAirTemperatureInCelsius,maxWindSpeedInKilometerPerHour,precipitationAmountInMillimeter,sunshineDurationInMinutes&validPeriod=PT24H&validFrom=" + validFrom + "&validUntil=" + validUntil + "&locatedAt=" + longitude + "," + latitude;

			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers.Add("Authorization", "Bearer " + await GetAccessToken(login, password));

			string rawData = performRequest(url, "GET", null, headers);

			return JsonConvert.DeserializeObject<WeeklyForecastsObj>(rawData);
		}

        // ONLY gets minTemp and maxWindSpeed starting from a given date
        public async Task<WeeklyForecastsObj> getWarningForecastByDate(Property prop, DateTime startDate, int daysTrend = 8)
        {
            if (prop != null)
            {
                latitude = prop.Latitude.ToString().Replace(",", ".");
                longitude = prop.Longtitude.ToString().Replace(",", ".");
            }
            var validFrom = startDate.ToString("yyyy-MM-ddTHH:mm:ssZ");
            var validUntil = startDate.AddDays(daysTrend).ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ssZ");

            string url = "https://point-forecast.weather.mg/search?fields=minAirTemperatureInCelsius,maxWindSpeedInKilometerPerHour&validPeriod=PT24H&validFrom=" + validFrom + "&validUntil=" + validUntil + "&locatedAt=" + longitude + "," + latitude;

            Dictionary<string, string> headers = new Dictionary<string, string>();
            headers.Add("Authorization", "Bearer " + await GetAccessToken(login, password));

            string rawData = performRequest(url, "GET", null, headers);

            return JsonConvert.DeserializeObject<WeeklyForecastsObj>(rawData);
        }

        public async Task<TodaySunsetandSunriseObj> getSunsetSunriseByGeoCoords(float longitude, float latitude)
		{
			var validAt = DateTime.Today.ToString("yyyy-MM-dd");
			

			string url = "https://point-forecast.weather.mg/sunrise-sunset?validAt=" + validAt +  "&locatedAt=" + longitude + "," + latitude;

			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers.Add("Authorization", "Bearer " + await GetAccessToken(login, password));

			string rawData = performRequest(url, "GET", null, headers);

			return JsonConvert.DeserializeObject<TodaySunsetandSunriseObj>(rawData);
		}

		public WeatherForecast getWeatherForecastByGeoCoords(float longitude, float latitude)
		{
			string url = "https://api.weather.mg/forecast?location=" + longitude.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + latitude.ToString(CultureInfo.InvariantCulture.NumberFormat) + "&meters_per_second&degree&millimeter";

			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers.Add("Authorization", "Basic " + decodeCredentials(login, password));

			// X-Authentication: <API-Key>
			// X-TraceId: < Trace - Id >

			string rawData = performRequest(url, "GET", null, headers);
			rawData = rawData.Trim('[',']');

			WeatherForecast ret = null;
			try
			{
				ret = Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherForecast>(rawData);
				if (ret != null && ret.Forecasts != null && ret.Forecasts.Hourly != null)
				{
					ret.Forecasts.Hourly = ret.Forecasts.Hourly.OrderBy(f => f.ValidAt);
				}
			}
			catch
			{ }
			return ret;
		}

		public float getMinTemperature(IEnumerable<WeatherHelpers.HourlyForecast> forecast, int hoursCount = 26)
		{
			 return forecast.Take(hoursCount).Min(f => f.AirTemperature.Value);
		}

		public float getMaxTemperature(IEnumerable<WeatherHelpers.HourlyForecast> forecast, int hoursCount = 26)
		{
			return forecast.Take(hoursCount).Max(f => f.AirTemperature.Value);
		}

		public float getMinWindspeed(IEnumerable<WeatherHelpers.HourlyForecast> forecast, int hoursCount = 26)
		{
			return forecast.Take(hoursCount).Min(f => f.AverageWindSpeed.Value);
		}

		public float getMaxWindspeed(IEnumerable<WeatherHelpers.HourlyForecast> forecast, int hoursCount = 26)
		{
			return forecast.Take(hoursCount).Max(f => f.AverageWindSpeed.Value);
		}

		private string performRequest(string url, string httpMethod, object data = null, Dictionary<string, string> headers = null)
		{
			HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

			string postData = new JavaScriptSerializer().Serialize(data);

			byte[] bytedata = Encoding.ASCII.GetBytes(postData);

			request.Method = httpMethod;

			request.AllowAutoRedirect = true;
			request.MaximumAutomaticRedirections = 10;
			request.Accept = "application/json,text/html,application/xhtml+xml";

			if(headers != null && headers.Count > 0)
			{
				foreach(KeyValuePair<string, string> header in headers)
				{
					request.Headers[header.Key] = header.Value;
				}
			}

			if (httpMethod != "GET")
			{
				request.ContentType = "application/json";
				request.ContentLength = bytedata.Length;

				Stream newStream = request.GetRequestStream(); //open connection
				newStream.Write(bytedata, 0, bytedata.Length); // Send the data.
				newStream.Close();
			}

			// Get the response.
			HttpWebResponse response = null;
			try
			{
				response = (HttpWebResponse)request.GetResponse();

			}
			catch (System.Net.WebException exception)
			{
				return exception.Message;
			}

			// Get the stream containing content returned by the server.
			Stream dataStream = response.GetResponseStream();
			// Open the stream using a StreamReader for easy access.
			StreamReader reader = new StreamReader(dataStream);
			// Read the content.
			string responseFromServer = reader.ReadToEnd();
			// Display the content.

			// Cleanup the streams and the response.
			reader.Close();
			dataStream.Close();
			response.Close();

			return responseFromServer;
		}

		private string decodeCredentials(string login, string password)
		{
			//Base 64 encoded string
			string credString = login + ":" + password;
			Encoding encoding = Encoding.GetEncoding("iso-8859-1");
			credString = Convert.ToBase64String(encoding.GetBytes(credString));
			return credString;
		}

		private async System.Threading.Tasks.Task<dynamic> GetAccessToken(string login, string password)
		{
			HttpClient client = new HttpClient();
			var content = new FormUrlEncodedContent(new[]
			{
				new KeyValuePair<string, string>("grant_type", "client_credentials")
			});

			var byteArray = Encoding.ASCII.GetBytes(login+":"+password);
			client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

			HttpResponseMessage response = await client.PostAsync("https://auth.weather.mg/oauth/token", content);
			HttpContent resContent = response.Content;
			string result = await resContent.ReadAsStringAsync();
			var responseObject = JsonConvert.DeserializeObject<dynamic>(result);

			// ... Check Status Code                                
			Console.WriteLine("Response StatusCode: " + (int)response.StatusCode);

			// ... Read the string.

			return responseObject["access_token"];
		}
	}
}