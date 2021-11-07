using GardifyModels.Models;
using Microsoft.AspNet.Identity;
using Newtonsoft.Json;
using GardifyWebAPI.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using static GardifyModels.Models.ModelEnums;

namespace GardifyWebAPI.App_Code
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

        public static IEnumerable<Plant> sortOutPlantsByPositiveTagList(IEnumerable<Plant> plants, List<int> positiveList, List<int> freezeLvls = null, List<int> excludes = null, List<int> colors = null, List<int> leafColors = null)
        {
            if (plants == null || !plants.Any())
            {
                return plants.ToList();
            }

            if ((positiveList == null && freezeLvls == null && excludes == null && colors == null && leafColors == null) ||
                (!positiveList.Any() && !freezeLvls.Any() && !excludes.Any()) && !colors.Any() && !leafColors.Any())
            {
                return plants;
            }

            IEnumerable<Plant> ret = plants;

            if (positiveList != null)
            {
                foreach (var tagId in positiveList)
                {
                    var plant_sel = ret.Where(p => p.PlantTags.Where(t => t.Id == tagId).Any());

                    if (plant_sel == null || !plant_sel.Any())
                    {
                        return null;
                    }
                    ret = plant_sel;
                }
            }

            if (freezeLvls != null && freezeLvls.Count() > 0)
            {
         

                ret = ret.Where(p => p.PlantTags.Where(t => freezeLvls.Contains(t.Id)).Any());
            }


           

            if (excludes != null && excludes.Count() > 0)
            {
                ret = ret.Where(p => p.PlantTags.All(t => !excludes.Contains(t.Id)));
            }
            if (colors != null && colors.Any())
            {
                ret = ret.Where(p => p.PlantTags.Where(t => colors.Contains(t.Id)).Any());
            }
            if (leafColors != null && leafColors.Any())
            {
                ret = ret.Where(p => p.PlantTags.Where(t => leafColors.Contains(t.Id)).Any());
            }
            return ret;
        }

        public static FeatureAccess ActionAllowed(UserAction action)
        {
            const int PLANTS_LIMIT = 25;

            // quick flag for disabling access-checking (while testing, before production etc)
            const bool USE_PREMIUM_IN_PRODUCTION = false;
            if (!USE_PREMIUM_IN_PRODUCTION)
                return FeatureAccess.Allowed;

            var db = new ApplicationDbContext();
            var userId = GetUserId();
            GardenController gc = new GardenController();
            var gardenId = (userId == Guid.Empty) ? 0 : gc.DbGetGardensByUserId(userId).FirstOrDefault().Id;
            var userPlantsCount = (userId == Guid.Empty) ? 0 : db.UserPlantToUserLists.Where(u => !u.Deleted && u.UserPlant.Gardenid == gardenId).GroupBy(u => u.PlantId).Count();
            var hasPremium = (userId == Guid.Empty) ? false : db.Users.Find(userId.ToString()).HasPremium;

            // premium is allowed anything by default
            if (hasPremium)
                return FeatureAccess.Allowed;

            switch (action)
            {
                case UserAction.NewPlant:   // done
                case UserAction.EcoScan:     // done
                case UserAction.SuggestPlant:   // done
                case UserAction.NewTodo: return userPlantsCount < PLANTS_LIMIT ? FeatureAccess.Allowed : FeatureAccess.NotAllowed;   // done

                // limited access for non-premium (e.g. recent only)
                case UserAction.NewsArchive:    // feature not available yet
                case UserAction.VideosArchive:   // feature not available yet
                case UserAction.TodoVideos: return FeatureAccess.Limited;   // feature not available yet

                // premium-only actions
                case UserAction.PlantDoc:   // done (handling posting)
                case UserAction.SearchGardenImages:  // feature not available yet
                case UserAction.GardenPresentation:  // feature not available yet
                case UserAction.EditPlantScan:
                case UserAction.EditSuggestPlant:
                case UserAction.FavNews:        // feature not available yet
                case UserAction.FavVideos:      // feature not available yet
                case UserAction.NewTodoImage:   // done
                case UserAction.NewGardenImage:  // done
                case UserAction.WeatherInKalender: return FeatureAccess.NotAllowed;     // feature not available yet
                default: return hasPremium ? FeatureAccess.Allowed : FeatureAccess.NotAllowed;
            }
        }

        public static string GetAbsolutePath(string relativePath)
        {
            return "";
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

        public static List<_HtmlImageViewModel> getHtmlImageObjectsFromDbImageResponse(HelperClasses.DbResponse imageResponse, string rootPath, int take = int.MaxValue, int skip = 0, bool order = true)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            RatingController ratingController = new RatingController();
            AlbumImageInfoController aic = new AlbumImageInfoController();
            List<_HtmlImageViewModel> ret = new List<_HtmlImageViewModel>();
            if(imageResponse == null)
            {
                return ret.ToList();

            }
            if (imageResponse.Status == ModelEnums.ActionStatus.Success && imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            {
                foreach (FileToModule ftm in imageResponse.ResponseObjects)
                {
                    if (ret.Count == take)
                    {
                        continue;
                    }
                    Files currentFile = rc.DbGetFile(ftm);
                    if (currentFile != null)
                    {
                        ret.Add(new _HtmlImageViewModel
                        {
                            Id = ftm.FileToModuleID,
                            FullTitle = ftm.AltText,
                            Author = currentFile.FileD == null ? "Autor nicht spezifiziert" : (currentFile.FileD.Length > 0 ? currentFile.FileD : "Autor nicht spezifiziert"),
                            License = currentFile.FileC,
                            FullDescription = ftm.Description,
                            SrcAttr = rootPath + currentFile.FilePath + currentFile.FileA,
                            TitleAttr = string.IsNullOrEmpty(ftm.AltText) ? "" : (ftm.AltText.Length > 120 ? ftm.AltText.Substring(0, 117) + "..." : ftm.AltText),
                            AltAttr = string.IsNullOrEmpty(ftm.Description) ? "" : (ftm.Description.Length > 120 ? ftm.Description.Substring(0, 117) + "..." : ftm.Description),
                            Sort = ftm.Sort ?? 0,
                            InsertDate = ftm.InsertedDate ?? currentFile.WrittenDate.Value,
                            Comments = currentFile.FilePath.Contains("nfiles/PlantImages/") ? (rc.DbGetImagesTags(currentFile.FileID).Length > 0 ? rc.DbGetImagesTags(currentFile.FileID) : "Keine Kommentare vorhanden") : "",
                            Tags = currentFile.TagsDE,
                            TakenDate = currentFile.UserCreatedDate,
                            Note = currentFile.FileE,
                            Rating = ratingController.GetAverageRating(ftm.FileToModuleID, ModelEnums.RatableObject.GardenImage),
                            IsOwnImage = aic.GetEntry(ftm.FileToModuleID, GetUserId())?.IsOwnImage ?? true,
                            Albums = rc.GetImageAlbums(ftm.FileToModuleID, GetUserId()),
                            IsMainImg = currentFile.IsMainImg
                        }) ;
                        
                    }
                }
            }

            if (order)
            {
                return ret.OrderBy(r => r.Sort).ThenBy(r => r.Id).ToList();

            }
            else
            {
                return ret.ToList();
            }
        }

        // returns filename
        public static string tryToSaveUploadedFile(Stream data, string fileName, string destination)
        {
            string fileNameWithoutExtension = stringToUri(System.IO.Path.GetFileNameWithoutExtension(fileName));
            string extension = Path.GetExtension(fileName).ToLower();
            string fullPath = Path.Combine(destination, fileNameWithoutExtension + extension);
            if (File.Exists(fullPath))
            {
                int counter = 1;
                string tempFileName = "";
                do
                {
                    tempFileName = fileNameWithoutExtension + "_V" + counter.ToString();
                    fullPath = Path.Combine(destination, tempFileName + extension);
                    counter++;
                } while (File.Exists(fullPath));

                fileNameWithoutExtension = tempFileName;
            }

            using (FileStream fileStream = new FileStream(fullPath, FileMode.CreateNew))
            {
                data.Seek(0, SeekOrigin.Begin);
                data.CopyTo(fileStream);
            }

            return fileNameWithoutExtension + extension;
        }

        public static Guid GetUserId()
        {
            var userId = HttpContext.Current.User.Identity.GetUserId();
            if (userId != null)
            {
                return new Guid(userId);
            }
            else
            {
                return new Guid();
            }
        }

        public static string GetUserName()
        {
            //return HttpContext.Current.User.Identity.GetUserName();
            return "testuser@netzlab.de";
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