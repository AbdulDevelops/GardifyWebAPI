using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using PflanzenApp.Controllers;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace PflanzenApp.App_Code
{
    public static class Utilities
	{
        public static string stringToUri(string rawString)
		{
			string ret = "";

			ret = rawString.ToLower();

			ret = ret.Trim();

			ret = Regex.Replace(ret, @"[\s]+", " ");

			ret = ret.Replace("ü", "ue").Replace("Ü", "Ue").Replace("ä", "ae").Replace("Ä", "Ae").Replace("ö", "oe").Replace("Ö", "oe").Replace("ß", "ss").Replace(" ", "_");

			ret = Regex.Replace(ret, "/[^a-z0-9_]/", "");

            ret = Regex.Replace(ret, "[^a-zA-Z0-9_.]+", "", RegexOptions.Compiled);

			return ret;
		}

		public static List<Plant> sortOutPlantsByPositiveTagList(IEnumerable<Plant> plants, IEnumerable<PlantTag> positiveList)
		{
			if (plants == null || !plants.Any())
			{
				return plants.ToList();
			}

			if (positiveList == null || !positiveList.Any())
			{
				return plants.ToList();
			}

			IEnumerable<Plant> ret = plants;

			foreach (PlantTag tag in positiveList)
			{
				var plant_sel = ret.Where(p => p.PlantTags.Where(t => t.Id == tag.Id).Any());

				if (plant_sel == null || !plant_sel.Any())
				{
					return null;
				}
				ret = plant_sel;
			}
			return ret.ToList();
		}

		/// <summary>
		/// Perform a deep Copy of the object, using Json as a serialisation method.
		/// http://stackoverflow.com/a/78612
		/// </summary>
		/// <typeparam name="T">The type of object being copied.</typeparam>
		/// <param name="source">The object instance to copy.</param>
		/// <returns>The copied object.</returns>
		public static T CloneJson<T>(this T source)
		{
			// Don't serialize a null object, simply return the default for that object
			if (Object.ReferenceEquals(source, null))
			{
				return default(T);
			}

			// initialize inner objects individually
			// for example in default constructor some list property initialized with some values,
			// but in 'source' these items are cleaned -
			// without ObjectCreationHandling.Replace default constructor values will be added to result
			var deserializeSettings = new JsonSerializerSettings { ObjectCreationHandling = ObjectCreationHandling.Replace };

			return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(source), deserializeSettings);
		}

		public static List<string> databaseMessagesToText(List<ModelEnums.DatabaseMessage> messageCodes)
		{
			List<string> ret = null;

			if (messageCodes != null && messageCodes.Any())
			{
				ret = new List<string>();

				foreach (ModelEnums.DatabaseMessage messageCode in messageCodes)
				{
					ret.Add(databaseMessageToText(messageCode));
				}
			}

			return ret;
		}

		public static string databaseMessageToText(ModelEnums.DatabaseMessage messageCode)
		{
			string ret = "";

			switch (messageCode)
			{
				case ModelEnums.DatabaseMessage.Ok:
					ret = "Ok";
					break;

				case ModelEnums.DatabaseMessage.Created:
					ret = "Erfolgreich erstellt";
					break;

				case ModelEnums.DatabaseMessage.Deleted:
					ret = "Erfolgreich gelöscht";
					break;

				case ModelEnums.DatabaseMessage.Edited:
					ret = "Erfolgreich verändert";
					break;

				case ModelEnums.DatabaseMessage.DuplicatedEntry:
					ret = "Eintrag mit diesen Daten ist schon vorhanden";
					break;

				case ModelEnums.DatabaseMessage.ErrorOnSaveChanges:
					ret = "Fehler beim Datenbankzugriff (SaveChanges)";
					break;

				case ModelEnums.DatabaseMessage.ObjectNotFound:
					ret = "Eintrag wurde nicht gefunden";
					break;

				case ModelEnums.DatabaseMessage.FolderNotExists:
					ret = "Ordner existiert nicht";
					break;

				case ModelEnums.DatabaseMessage.FileNotExists:
					ret = "Datei existiert nicht";
					break;

				case ModelEnums.DatabaseMessage.EmptyResult:
					ret = "Leere Ergebnismenge";
					break;

				case ModelEnums.DatabaseMessage.WrongQuantity:
					ret = "Falsche Anzahl";
					break;

				case ModelEnums.DatabaseMessage.Undefined:
				default:
					ret = "Keine Beschreibung";
					break;
			}

			return ret;
		}

		public static List<_HtmlImageViewModel> getHtmlImageObjectsFromDbImageResponse(HelperClasses.DbResponse imageResponse, string rootPath)
		{
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            List<_HtmlImageViewModel> ret = new List<_HtmlImageViewModel>();
            if (imageResponse.Status == ModelEnums.ActionStatus.Success && imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            {
                foreach (FileToModule ftm in imageResponse.ResponseObjects)
                {
                    Files currentFile = rc.DbGetFile(ftm);
                    if(currentFile != null)
                    {
                        ret.Add(new _HtmlImageViewModel
                        {
                            Id = ftm.FileID,
                            FullTitle = ftm.AltText,
                            Author = currentFile.FileD,
                            License = currentFile.FileC,
                            FullDescription = ftm.Description,
                            SrcAttr = rootPath + currentFile.FilePath + currentFile.FileA,
                            TitleAttr = string.IsNullOrEmpty(ftm.AltText) ? "" : (ftm.AltText.Length > 120 ? ftm.AltText.Substring(0, 117) + "..." : ftm.AltText),
                            AltAttr = string.IsNullOrEmpty(ftm.Description) ? "" : (ftm.Description.Length > 120 ? ftm.Description.Substring(0, 117) + "..." : ftm.Description),
							IsMainImg=currentFile.IsMainImg
                        });
                    }
                }
            }
            return ret;
        }
       
        // returns filename
        public static string tryToSaveUploadedFile(HttpPostedFileBase imageFile, string path)
		{
			string fileNameWithoutExtension = stringToUri(System.IO.Path.GetFileNameWithoutExtension(imageFile.FileName));
			string extension = Path.GetExtension(imageFile.FileName).ToLower();
			string fullPath = Path.Combine(path, fileNameWithoutExtension + extension);
			if (File.Exists(fullPath))
			{
				int counter = 1;
				string tempFileName = "";
				do
				{
					tempFileName = fileNameWithoutExtension + "_V" + counter.ToString();
					fullPath = Path.Combine(path, tempFileName + extension);
					counter++;
				} while (File.Exists(fullPath));

				fileNameWithoutExtension = tempFileName;
			}

			using (var reader = new BinaryReader(imageFile.InputStream))
			{
				imageFile.SaveAs(Path.Combine(path, fileNameWithoutExtension + extension));
			}

			return fileNameWithoutExtension + extension;
		}


        public static string GetAbsolutePath(string relativePath)
        {
            return "";
        }
        public static Guid GetUserId()
		{
			return new Guid(HttpContext.Current.User.Identity.GetUserId());
		}

		public static string GetUserName()
		{
			return HttpContext.Current.User.Identity.GetUserName();
		}
		public static string GetBaseUrl()
		{
			var request = HttpContext.Current.Request;
			var appUrl = HttpRuntime.AppDomainAppVirtualPath;

			if (appUrl != "/") appUrl += "/";

			var baseUrl = string.Format("{0}://{1}{2}", request.Url.Scheme, request.Url.Authority, appUrl);

			return baseUrl;
		}

        public static Guid GetApplicationId()
        {
            return new Guid("3440b466-16d6-47f4-a94f-07a5c853db9d");
        }
    }
}