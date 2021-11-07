using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web.Script.Serialization;

namespace PflanzenApp.App_Code
{
    public class WeatherHandler
	{
		// api key 5503129b5ac3d8f75906c6a9a1288b76

		// current weather
		// by zip api.openweathermap.org/data/2.5/weather?zip=94040,us&units=metric&APPID={APIKEY}
		// by coords api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&units=metric&APPID={APIKEY}
		// by city id api.openweathermap.org/data/2.5/weather?id=2172797&units=metric&APPID={APIKEY}
		// by city name api.openweathermap.org/data/2.5/weather?q=London,uk&units=metric&APPID={APIKEY}

		/*
		------------------ openweather code --------------------

		private string apiKey = "5503129b5ac3d8f75906c6a9a1288b76";

		public string getCurrentWeatherByZip(string zip)
		{
			string ret = "";

			string url = "http://api.openweathermap.org/data/2.5/weather?zip="+zip+ "&type=like&units=metric&lang=de&APPID=" + apiKey;

			ret = performRequest(url, "GET");

			return ret;
		}

		public string getCurrentWeatherByCityId(string cityId)
		{
			string ret = "";

			string url = "http://api.openweathermap.org/data/2.5/weather?id=" + cityId + "&type=like&units=metric&lang=de&APPID=" + apiKey;

			ret = performRequest(url, "GET");

			return ret;
		}

		------------------ /openweather code --------------------
		*/

		string login = "kfjoest";
		string password = "TJyYUCWXo72JxhdY";

		public WeatherObject getCurrentWeatherByGeoCoords(float latitude, float longitude)
		{
			string url = "https://api.weather.mg/observation?location="+ latitude + ","+ longitude + "&meters_per_second&degree&millimeter";

			Dictionary<string, string> headers = new Dictionary<string, string>();
			headers.Add("Authorization", "Basic " + decodeCredentials(login,password));

			// X-Authentication: <API-Key>
			// X-TraceId: < Trace - Id >

			string rawData = performRequest(url, "GET", null, headers);

			return Newtonsoft.Json.JsonConvert.DeserializeObject<WeatherObject>(rawData);
		}

		public WeatherForecast getWeatherForecastByGeoCoords(float latitude, float longitude)
		{
			string url = "https://api.weather.mg/forecast?location=" + latitude.ToString(CultureInfo.InvariantCulture.NumberFormat) + "," + longitude.ToString(CultureInfo.InvariantCulture.NumberFormat) + "&meters_per_second&degree&millimeter";

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
				if(ret != null && ret.Forecasts != null && ret.Forecasts.Hourly != null)
				{
					ret.Forecasts.Hourly = ret.Forecasts.Hourly.OrderBy(f => f.ValidAt);
				}
            } catch
            {}
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
	}
}