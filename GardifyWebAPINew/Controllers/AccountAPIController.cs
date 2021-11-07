using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using GardifyWebAPI.App_Code;
using GardifyModels.Models;
using GardifyWebAPI.Results;
using GardifyWebAPI.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using System.Web;
using System.Security.Cryptography;
using System.Text;
using static GardifyModels.Models.UserPlantViewModels;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.Net.Mail;
using System.IO;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.Reflection;
using Newtonsoft.Json.Linq;
using static GardifyModels.Models.ModelEnums;
using RestSharp;

namespace GardifyWebAPI.Controllers
{
    public class AccountAPIController : ApiController
    {
        const string FRONTEND_URL = "https://gardify.de";
        const string ACCESS_TOKEN_SECRET = "EF2D424CF48BD04E6DC18F2BD5";
        private ApplicationDbContext db = new ApplicationDbContext();
        private AspNetUserManager userManager;
  

        public AspNetUserManager UserManager
        {
            get
            {
                return userManager ?? Request.GetOwinContext().GetUserManager<AspNetUserManager>();
            }
            private set
            {
                userManager = value;
            }
        }
        [Route("api/AccountAPI/redirect")]
        [HttpGet]
        public IHttpActionResult redirectRequest()
        {
            //var output = new MaintenanceView
            //{
            //    IsNoticeExist = true,
            //    Message =
            //}

            return BadRequest("Es findet eine Wartung statt. Bitte versuchen Sie es nach einiger Zeit erneut");
        }


        [Route("api/AccountAPI/iptest")]
        [HttpGet]
        public IHttpActionResult iptest()
        {
            var request = Request;

            if (request.Properties.ContainsKey("MS_HttpContext"))
            {
                var test1=  ((HttpContextWrapper)request.Properties["MS_HttpContext"]).Request.UserHostAddress;
            }

            else if (HttpContext.Current != null)
            {
                var test2 = HttpContext.Current.Request.UserHostAddress;
            }
    

            return Ok("Success");
        }

        [Route("api/AccountAPI/roles")]
        [HttpGet]
        public IEnumerable<string> GetUserRoles()
        {
            var userId = Utilities.GetUserId().ToString();
            return UserManager.GetRoles(userId).ToList();
        }

        [Route("api/AccountAPI/{id}")]
        [HttpGet]
        public IHttpActionResult GetUserById(string id)
        {
            ApplicationUser ApplicationUser = db.Users.Find(id);
            if (ApplicationUser == null || ApplicationUser.Deleted)
            {
                return NotFound();
            }

            return Ok(ApplicationUser); // response parse error
        }
        [Route("api/AccountAPI/profilImg")]
        [HttpGet]
        public UserProfilImgVM GetUserProfilImg()
        {
            //ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            var userId = Utilities.GetUserId();
            //var userProperty = new PropertyController().DbGetProperty(userId);
            //var userPropertyId = userProperty.Id;
            var profilImage = new UserProfilImgVM();
            //HelperClasses.DbResponse imageResponse = rc.DbGetProfilImgReferencedImages(userPropertyId);

            //if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            //{
            //    profilImage.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/")).OrderByDescending(r => r.InsertDate).ToList();
            //}
            profilImage.Images = getUserProfileImages(userId);

            return profilImage; 
        }

        [Route("api/AccountAPI/otherprofilImg/{userId}")]
        [HttpGet]
        public List<_HtmlImageViewModel> getUserProfileImages(Guid userId)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            List<_HtmlImageViewModel> output = new List<_HtmlImageViewModel>();

            var userProperty = new PropertyController().DbGetProperty(userId);
            var userPropertyId = userProperty.Id;
            HelperClasses.DbResponse imageResponse = rc.DbGetProfilImgReferencedImages(userPropertyId);

            if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            {
                output = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/")).OrderByDescending(r => r.InsertDate).ToList();
            }
            return output;
        }

        [Route("api/AccountAPI/otherprofilImgByUserName/{userName}")]
        [HttpGet]
        public UserProfilImgVM getUserProfileImagesByUserName(string userName)
        {
            var selectedUser = db.Users.FirstOrDefault(u => u.UserName == userName);

            if (selectedUser == null)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    ReasonPhrase = "Kunde existiert nicht"
                };
                throw new HttpResponseException(resp);
            }

            var profilImage = new UserProfilImgVM();
            profilImage.Images = getUserProfileImages(Guid.Parse(selectedUser.Id));



            return profilImage;
        }

        [Route("api/AccountAPI/getotheruserbyusername/{userName}")]
        [HttpGet]
        public IHttpActionResult getotheruserbyusername(string userName)
        {
            var user = db.Users.Where(u => u.UserName.ToLower().Contains(userName.ToLower()) && !u.Deleted).Select(u => new { userName= u.UserName, userId= u.Id });
            if (user.Any())
            {
                return Ok(user.OrderBy(u => u.userName));
            }
            return Ok();
        }

       


        [HttpPost]
        [Route("api/AccountAPI/suggest")]
        public async Task<IHttpActionResult> SuggestPlant(bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            if (Utilities.ActionAllowed(UserAction.SuggestPlant) == FeatureAccess.NotAllowed)
                return Unauthorized();

            UserPlantController upc = new UserPlantController();
            EmailSender es = new EmailSender(db);
            TemplateService ts = new TemplateService();

            var userId = Utilities.GetUserId().ToString();
            ApplicationUser user = await UserManager.FindByIdAsync(userId);

            if (user == null)
            {
                return BadRequest("Du musst dich erstmal anmelden bzw. registrieren.");
            }

            if (!user.EmailConfirmed)
            {
                await SendConfimationEmail(user);
                return BadRequest("Bitte bestätige erst deine Emailadresse. Wir haben dir soeben eine Bestätigungsmail geschickt.");
            }

            string plantName = HttpContext.Current.Request.Params["name"];
            string nameQuery = plantName.Trim().ToLower();
            bool ignoreMatches = Boolean.Parse(HttpContext.Current.Request.Params["ignoreMatches"]);
            bool possibleMatches = false;

            if (!ignoreMatches)
            {
                possibleMatches = db.Plants.Any(p => !p.Deleted && p.Published && (p.NameGerman.Contains(nameQuery) || p.NameLatin.Contains(nameQuery)));
            }

            if (possibleMatches)
            {
                return Ok(new { FoundMatches = true });
            }
            else
            {
                var uploaded = new List<string>();

                for (var i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                {
                    var imageFile = HttpContext.Current.Request.Files[i];
                    var imageTitle = HttpContext.Current.Request.Params["imageTitle" + i.ToString()];

                    HttpPostedFileBase filebase = new HttpPostedFileWrapper(imageFile);
                    uploaded.Add(upc.UploadUserSuggestion(filebase, imageTitle, plantName).FullRelativePath);
                }

                string content = ts.RenderTemplateAsync("Suggestion", new { Name = user.UserName, PlantName = plantName, user.Email });
                var subjectHeader = "[Pflanze Ergänzen] Vorschlag für neue Pflanze";
                subjectHeader = !String.IsNullOrEmpty(plantName) ? subjectHeader : subjectHeader + ": " + plantName;

                await es.SendEmail(subjectHeader, content, user.Email, "team@gardify.de", uploaded.ToArray());
                await es.SendEmail(subjectHeader, content, user.Email, "jo@bjvv.de", uploaded.ToArray());
                await es.SendEmail(subjectHeader, content, user.Email, "kk@netzlab.de", uploaded.ToArray());
              
                var currentStatistic = StatisticEventTypes.SuggestPlant;
               
                    if (isIos)
                    {
                        new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id),(int)StatisticEventTypes.ApiCallFromIos);
                    }
                    else if (isAndroid)
                    {
                        new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromAndroid);
                    }
                    else if (isWebPage)
                    {
                        new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromWebpage);
                    }

                   
               

                return Ok(new { FoundMatches = false });
            }

            
        }

        [HttpPost]
        [Route("api/AccountAPI/addPlant")]
        public async Task<IHttpActionResult> AddPlant(bool checksub = false)
        {
            var userId = Utilities.GetUserId().ToString();
            UserPlantController upc = new UserPlantController();
            UserPlantsAPIController upa = new UserPlantsAPIController();

            ApplicationUser user = await UserManager.FindByIdAsync(userId);
            PlantController pc = new PlantController();

            UserListController ulc = new UserListController();
            EmailSender es = new EmailSender(db);
            TemplateService ts = new TemplateService();
            string plantName = HttpContext.Current.Request.Params["name"];
            string nameQuery = plantName.Trim().ToLower();
            bool ignoreMatches = Boolean.Parse(HttpContext.Current.Request.Params["ignoreMatches"]);
            bool possibleMatches = false;

            if (!ignoreMatches)
            {
                possibleMatches = db.Plants.Any(p => !p.Deleted && p.Published && (p.NameGerman.Contains(nameQuery) || p.NameLatin.Contains(nameQuery)));
            }

            if (possibleMatches)
            {
                return Ok(new { FoundMatches = true });
            }


            if ( user != null)
            {
                //string plantName = HttpContext.Current.Request.Params["Name"];
                //string plantNameLatin = HttpContext.Current.Request.Params["NameLatin"];
                string plantNameLatin = plantName;
                string searchResult = HttpContext.Current.Request.Params["searchResult"].Replace("\n", " <br/>");
                string isAuthor = "false";

                Plant newPlant = new Plant();
                newPlant.NameLatin = plantNameLatin;
                newPlant.NameGerman = plantName;
                newPlant.Published = false;
                newPlant.Description = "Keine Beschreibung verfügbar.";
                newPlant.PublishedDate = DateTime.Now;
                newPlant.Vorschlagen = true;
                newPlant.OnCreate(user.UserName);
                newPlant.CreatedBy = user.UserName;
                HelperClasses.DbResponse response = pc.DbCreatePlant(newPlant);
                if (  response.Status == ActionStatus.Success )
                {
                    if (HttpContext.Current.Request.Files[0] == null)
                    {
                        return BadRequest("Ein Fehler ist aufgetreten.");

                    }
                    new StatisticsController().CreateEntry(StatisticEventTypes.SuggestPlant, new Guid(user.Id));

                    //var imageFile = HttpContext.Current.Request.Files[0];
                    //var imageTitle = HttpContext.Current.Request.Params["imageTitle"];
                    var uploaded = new List<string>();

                    //var uploaded = new List<string>();

                    for (var i = 0; i < HttpContext.Current.Request.Files.Count; i++)
                    {
                        var imageFile = HttpContext.Current.Request.Files[i];
                        var imageTitle = HttpContext.Current.Request.Params["imageTitle" + (i == 0 ? "": i.ToString())];

                        HttpPostedFileBase filebase = new HttpPostedFileWrapper(imageFile);
                        uploaded.Add(upc.UploadUserSuggestion(filebase, imageTitle, plantName).FullRelativePath);


                        pc.UploadAndRegisterFileFull(filebase, newPlant.Id, (int)ModelEnums.ReferenceToModelClass.Plant, ModelEnums.FileReferenceType.PlantImage, imageTitle, "Keine Beschreibung vorhanden.");

                    }

                    //HttpPostedFileBase filebase = new HttpPostedFileWrapper(imageFile);
                    //uploaded.Add(upc.UploadUserSuggestion(filebase, imageTitle, plantName).FullRelativePath);
                    Plant addedPlant = (Plant)response.ResponseObjects[0];
                    string content = ts.RenderTemplateAsync("AddPlant", new { Name = user.UserName, LatinPlantName = "", PlantName = plantName, user.Email, SearchResult = searchResult, IsAuthor = isAuthor });
                    var subjectHeader = "[Pflanze hinzufügen] Neue Anlage vom Benutzer hinzugefügt " + user.UserName;
                    subjectHeader = !String.IsNullOrEmpty(plantName) ? subjectHeader : subjectHeader + ": " + plantName;
                    await es.SendEmail(subjectHeader, content, user.Email, "team@gardify.de", uploaded.ToArray());
                    await es.SendEmail(subjectHeader, content, user.Email, "jo@bjvv.de", uploaded.ToArray());
                    await es.SendEmail(subjectHeader, content, user.Email, "mp@bjvv.de", uploaded.ToArray());
                    await es.SendEmail(subjectHeader, content, user.Email, "kk@netzlab.de", uploaded.ToArray());

                    //UserPlantToUserListView[] userlist = JsonConvert.DeserializeObject<UserPlantToUserListView[]>(HttpContext.Current.Request.Params["ArrayOfUserlist"]);
                    var list = ulc.GetUserLists();

                    if (list == null)
                    {
                        return Ok(new { FoundMatches = false });
                    }

                    var userListFirst = new List<UserPlantToUserListView>
                    {
                        new UserPlantToUserListView
                        {
                            UserListId = list.First().Id,
                            UserPlantId = 1
                        }
                    };

                    var up = new BorrowUserPlantViewModel
                    {
                        Count = 1,
                        InitialAgeInDays = 0,
                        IsInPot = false,
                        PlantId = newPlant.Id,
                        ArrayOfUserlist = userListFirst.ToArray()

                    };
                    UserPlant userPlantEnd = upa.AddUserPlantRaw(up, checksub);

                    var cacheUniqueId = userId + "_mygarden";
                    if (Utilities.GetUserId() == Guid.Empty)
                    {
                        return Ok(new { FoundMatches = false });
                    }

                    string cachedFilePath = HttpContext.Current.Server.MapPath("~") + "\\gardenCache\\" + cacheUniqueId + ".txt";
                    upc.DeleteIndexCache(cachedFilePath);

                    return Ok(new { FoundMatches = false });
                }
                else
                {
                    var plant_check = (from p in db.Plants
                                       where !p.Deleted && p.NameLatin == newPlant.NameLatin && p.NameGerman == newPlant.NameGerman
                                       select p).FirstOrDefault();

                    

                    // get first user list
                    var list = ulc.GetUserLists();

                    if (list == null)
                    {
                        return Ok(new { FoundMatches = true });
                    }

                    if (plant_check.Vorschlagen && plant_check.Genehmigt != SuggestionApproved.Approved)
                    {
                        return Ok(new { FoundMatches = true });

                    }

                    var userListFirst = new List<UserPlantToUserListView>
                    {
                        new UserPlantToUserListView
                        {
                            UserListId = list.First().Id,
                            UserPlantId = 1
                        }
                    };

                    var up = new BorrowUserPlantViewModel
                    {
                        Count = 1,
                        InitialAgeInDays = 0,
                        IsInPot = false,
                        PlantId = plant_check.Id,
                        ArrayOfUserlist = userListFirst.ToArray()
                    };


                    UserPlant userPlantEnd = upa.AddUserPlantRaw(up, checksub);

                    return Ok(new { FoundMatches = true });
                }

                return BadRequest("Ein Fehler ist aufgetreten.");
            }


            return BadRequest("Ein Fehler ist aufgetreten.");
        }

        [HttpPost]
        [Route("api/AccountAPI/sendScanMail")]
        public async Task<HttpResponseMessage> SendScanMail()
        {
            EmailSender es = new EmailSender(db);
            TemplateService ts = new TemplateService();

            var images = HttpContext.Current.Request.Params["image"];
            var sender = HttpContext.Current.Request.Params["fromMail"];
            var senderName = HttpContext.Current.Request.Params["fromName"];
            var receiverName = HttpContext.Current.Request.Params["toName"];
            var receiver = HttpContext.Current.Request.Params["email"];
            var text = HttpContext.Current.Request.Params["emailText"].Replace(System.Environment.NewLine, " <br />  &nbsp;").Replace("\\n", " <br />  &nbsp;");

            var possibleReceiver = receiver.Split(',');
            foreach (var rec in possibleReceiver)
            {
                string content = ts.RenderTemplateAsync("SendEmail", new { Text = text, Receiver = receiverName, Sender = senderName });
                string pdfContent = ts.RenderTemplateAsync("PdfAttachment", new { Image = images });
                var subjectHeader = "Neue Nachricht von Gardify";

                //var document = new HtmlToPdfDocument()

                Byte[] res = null;
                using (MemoryStream ms = new MemoryStream())
                {
                    var pdf = TheArtOfDev.HtmlRenderer.PdfSharp.PdfGenerator.GeneratePdf(pdfContent, PdfSharp.PageSize.A4);
                    pdf.Save(ms);
                    res = ms.ToArray();
                }

                var destination = Path.GetTempPath();
                var temporaryPath = Path.Combine(destination, "Ökoscan.pdf");
                File.WriteAllBytes(temporaryPath, res);
                es.SendEmail(subjectHeader, content, sender, rec, new string[] { temporaryPath });
            }

            return Request.CreateResponse(HttpStatusCode.OK, new { response = "success" });
        }

        [HttpPost]
        [Route("api/AccountAPI/contact")]
        public async Task<IHttpActionResult> Contact(ContactFormViewModel vm)
        {
            EmailSender es = new EmailSender(db);
            TemplateService ts = new TemplateService();

            var userId = Utilities.GetUserId().ToString();
            ApplicationUser user = await UserManager.FindByIdAsync(userId);
            var name = "Anonym";
            if(vm.FirstName != null)
            {
                name = vm.FirstName;
            }
            if (user != null)
            {
                name = user.Email.Contains("test") ? "Anonym" : user.FirstName;
                //await es.SendEmail("[Kontakt] " + vm.Subject, content, vm.Email, "team@gardify.de");

                //await es.SendEmail("[Kontakt] " + vm.Subject, content, vm.Email, "info@gardify.de");

            }
            string content = ts.RenderTemplateAsync("Contact", new { Name = name, vm.Email, vm.Text });


            await es.SendEmail("[Kontakt] " + vm.Subject, content, vm.Email, "team@gardify.de");

            await es.SendEmail("[Kontakt] " + vm.Subject, content, vm.Email, "info@gardify.de");

            return Ok();
        }

        [Authorize]
        [Route("api/AccountAPI/update/pass/{id}")]
        [HttpPost]
        public async Task<IHttpActionResult> UpdateAspNetUserPassword(string id, EditPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var user = await UserManager.FindByIdAsync(model.Id.ToString());

            if (user == null || user.Deleted)
            {
                return BadRequest("Email oder Passwort ist ungültig.");
            }

            if (!model.ConfirmPassword.Equals(model.Password))
            {
                return BadRequest("Passwörter stimmen nicht überein.");
            }

            var res = await UserManager.ChangePasswordAsync(model.Id.ToString(), model.OldPassword, model.Password);

            if (!res.Succeeded)
            {
                return BadRequest("Fehler beim aktualisieren des Passworts: " + res.Errors.FirstOrDefault());
            }

            return Ok();
        }

        [Route("api/AccountAPI/forgot")]
        [HttpPost]
        public async Task<IHttpActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            var user = await UserManager.FindByEmailAsync(model.Email);

            if (user != null && !user.Deleted)
            {
                var code = UserManager.GeneratePasswordResetToken(user.Id);
                var callbackUrl = "https://gardify.de/resetpassword?UserId=" + user.Id + "&c=" + code;
                callbackUrl = Uri.EscapeUriString(callbackUrl);
                var UserName = user.Gender + " " + user.LastName;
                var res = await SendEmail("info@gardify.de", user.Email, "Passwort Zurücksetzen", "ForgotPassword", new { UserName, Action_Url = callbackUrl }, null);
            }

            return Ok();
        }

        [Route("api/AccountAPI/reset")]
        [HttpPost]
        public async Task<IHttpActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            var user = UserManager.FindById(model.UserId);

            if (user == null || user.Deleted)
            {
                return BadRequest("Benutzer existiert nicht.");
            }

            if (!model.ConfirmPassword.Equals(model.Password))
            {
                return BadRequest("Passwörter stimmen nicht überein.");
            }

            IdentityResult resetResult = UserManager.ResetPassword(model.UserId, model.Code, model.Password);
            if (!resetResult.Succeeded)
            {
                var err = resetResult.Errors.Any() ? resetResult.Errors.FirstOrDefault() : "";
                return BadRequest("Passwort wurde nicht zurückgesetzt. " + err);
            }

            return Ok();
        }

        [Route("api/AccountAPI/login/{simpleMode}")]
        [HttpPost]
        public async Task<IHttpActionResult> LoginUser(LoginViewModel model, bool simpleMode = true, bool checkSub = false)
        {
            if (String.IsNullOrEmpty(model.Email))
            {
                return BadRequest("Bitte gib deine E-Mail Adresse an.");
            }

            if (String.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Bitte gib dein Passwort an.");
            }

            ApplicationUser user;
            if (!ModelState.IsValid)
            {
                return BadRequest("Eingabe ist ungültig.");
            }

            user = await UserManager.FindByNameAsync(model.Email);

            if (user == null)
            {
                user = await UserManager.FindByEmailAsync(model.Email);
                simpleMode = false;
            }

            if (user == null || user.Deleted)
            {
                return BadRequest("UserName oder Passwort ist ungültig.");
            }

            if (!user.EmailConfirmed)
            {
                //await SendConfimationEmail(user);
                return BadRequest("Bitte bestätige erst deine Emailadresse.");
            }
            var res = UserManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, model.Password);
            if (!(res == PasswordVerificationResult.Success))
            {
                return BadRequest("UserName oder Passwort ist ungültig.");
            }

            AccessTokenModel token = Authenticate(user, model.RememberMe, simpleMode);
            user.LastLogin = DateTime.Now;
            UserManager.Update(user);

            var currentStatistic = StatisticEventTypes.Login;
            if (model.model != null)
            {
                if (model.model.IsIos)
                {
                    new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromIos);
                }
                else if (model.model.IsAndroid)
                {
                    new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromAndroid);
                }
                else if (model.model.IsWebPage)
                {
                    new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromWebpage);
                }

            }
            else
            {
                new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id));
            }

            return Ok(token);
        }

        [Route("api/AccountAPI/register/{simpleMode}/{newsLetterAbo}")]
        [HttpPost]
        public async Task<IHttpActionResult> RegisterUser(SimpleRegisterBindingModel model, bool simpleMode,bool newsLetterAbo)
        {
            if (model.UserName.Length < 4)
            {
                return BadRequest("Username muss aus mindestens 4 Zeichen bestehen");
            }
            if (model.Password != model.ConfirmPassword)
            {
                return BadRequest("Die Passwörter stimmen nicht überein.");
            }

            if (model.PLZ.Length == 0)
            {
                return BadRequest("Bitte trage deine Postleitzahl ein.");
            }
            else
            {
                var count = model.PLZ.Length;
                if (count < 4 || count > 5)
                {
                    return BadRequest("Bitte trage eine richtige Postleitzahl ein.");

                }
                if (model.PLZ.Length > 0 && count == 4 && model.Country == "Deutschland")
                {
                    return BadRequest("Bitte trage eine richtige Postleitzahl ein.");
                }
                if (model.PLZ.Length > 0 && count == 5 && model.Country != "Deutschland")
                {
                    return BadRequest("Bitte trage eine richtige Postleitzahl ein.");
                }
            }
            
            if (String.IsNullOrEmpty(model.Country))
            {
                return BadRequest("Bitte gib dein Land an.");
            }

            if (String.IsNullOrEmpty(model.Email))
            {
                return BadRequest("Bitte gib deine E-Mail Adresse an.");
            }

            if (String.IsNullOrEmpty(model.Password))
            {
                return BadRequest("Bitte gib dein Passwort an.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Die Eingabe war falsch oder nicht vollständig");
            }            

            var user = new ApplicationUser()
            {
                CompleteSignup = false,
                UserName = model.UserName,
                Email = model.Email,
                FirstName = "Platzhalter",
                LastName = "Platzhalter",
                Street = "Platzhalter",
                PLZ = int.Parse( model.PLZ),
                Country = model.Country,
                HouseNr = "Platzhalter",
                City = "Platzhalter",
                Gender = "Herr/Frau",
                RegisterDate = DateTime.Now,
                ReferralURL = model.ReferralURL,
                RequestURL = model.RequestURL
            };

            IdentityResult result = await UserManager.CreateAsync(user, model.Password);

            if (!result.Succeeded && result.Errors.FirstOrDefault().StartsWith("Email"))
            {
                return BadRequest("Diese Email-Adresse ist bereits vergeben.");
            }

            if (!result.Succeeded && result.Errors.FirstOrDefault().StartsWith("Name"))
            {
                return BadRequest("Dieser Username ist bereits vergeben.");
            }

            if (!result.Succeeded && result.Errors.FirstOrDefault().StartsWith("Password"))
            {
                return BadRequest("Passwort muss Folgendes enthalten: Großbuchstaben, Nummer, Sonderzeichen");
            }

            DbCreateProperty(user.Street, user.PLZ.ToString(), user.City, user.Country, user.Id);
            if (newsLetterAbo && result.Succeeded)
            {
                NewsLetter nl = new NewsLetter();
                try
                {
                    nl.FirstName = model.UserName;
                    nl.LastName = model.UserName;
                    nl.Email = model.Email;
                    nl.Gender = "Herr/Frau";
                    nl.Birthday = DateTime.Now;
                    nl.UserId = new Guid(user.Id);
                    nl.EmailConfirmed = false;
                    nl.OnCreate(Utilities.GetUserName());
                    db.NewsLetters.Add(nl);
                  
                }catch(Exception e)
                {
                    return BadRequest("Ein Fehler ist aufgetreten");
                }
            }
            await SendConfimationEmail(user);
            var userSettings = setUserSettings(user);
            userSettings.OnCreate("Register");
            db.UsersSettings.Add(userSettings);
            db.SaveChanges();

            var currentStatistic = StatisticEventTypes.Register;
            if (model.model != null)
            {
                if (model.model.IsIos)
                {
                    new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromIos);
                }
                else if (model.model.IsAndroid)
                {
                    new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromAndroid);
                }
                else if (model.model.IsWebPage)
                {
                    new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromWebpage);
                }

            }
            else
            {
                new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id));
            }
            return Ok();
        }
        [Route("api/AccountAPI/register/registerDemo")]
        public async Task<IHttpActionResult> TestModeRegister(TestModeRegisterModel model)
        {
            if (model.PLZ <= 0)
            {
                return BadRequest("Bitte trage deine Postleitzahl ein.");
            }

            if (String.IsNullOrEmpty(model.Country))
            {
                return BadRequest("Bitte gib dein Land an.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest("Die Eingabe war falsch oder nicht vollständig");
            }

            var user = new ApplicationUser();
            var userName = "UserDemo" + new Guid(user.Id);
            user.CompleteSignup = false;
            user.UserName = userName;
            user.Email = userName + "@user.demo";
            user.FirstName = "Vorname";
            user.LastName = "Nachname";
            user.Street = "Straße";
            user.PLZ = model.PLZ;
            user.Country = model.Country;
            user.HouseNr = "Hausnummer";
            user.City = "Stadt";
            user.Gender = "Herr/Frau";
            user.RegisterDate = DateTime.Now;
            user.EmailConfirmed = true;
            user.LockoutEnabled = false;
            IdentityResult result = await UserManager.CreateAsync(user, userName);

            if (!result.Succeeded)
            {
                return BadRequest("Ein Fehler ist aufgetreten. " + result.Errors.FirstOrDefault());
            }
            DbCreateProperty(user.Street, user.PLZ.ToString(), user.City, user.Country, user.Id);

            var userSettings = setUserSettings(user);
            userSettings.OnCreate("RegisterDemo");
            db.UsersSettings.Add(userSettings);
            db.SaveChanges();
            GardenViewModels.GardenCreateViewModel ng = new GardenViewModels.GardenCreateViewModel();
            ng.Description = "Meine Gartenbeschreibung";
            ng.Name = "Mein Garten";
            GardenController gc = new GardenController();
            gc.CreateGardenOnRegister(ng, user.Id, true);
            var testUser = UserManager.FindById(user.Id);
            AccessTokenModel token = Authenticate(testUser, false, false);

            var currentStatistic = StatisticEventTypes.RegisterDemo;
            if (model.model != null)
            {
                if (model.model.IsIos)
                {
                    new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromIos);
                }
                else if (model.model.IsAndroid)
                {
                    new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromAndroid);
                }
                else if (model.model.IsWebPage)
                {
                    new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromWebpage);
                }

            }
            else
            {
                new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id));
            }
            return Ok(token);
        }

        [HttpPost]
        [Route("api/AccountAPI/register/convert")]
        public async Task<IHttpActionResult> ConvertUserAsync(UserConvertViewModel model)
        {
            var userId = Utilities.GetUserId();
            var user = UserManager.FindById(userId.ToString());

            if (user == null)
            {
                return BadRequest("Etwas ist schief gelaufen. Melde dich bitte nochmal an.");
            }

            user.UserName = model.UserName;
            user.Email = model.Email;
            user.RegisterDate = DateTime.Now;
            user.EmailConfirmed = false;
            user.CompleteSignup = true;
            IdentityResult res1 = UserManager.Update(user);

            if (!res1.Succeeded && res1.Errors.FirstOrDefault().StartsWith("Email"))
            {
                return BadRequest("Diese Email-Adresse ist bereits vergeben.");
            }

            if (!res1.Succeeded && res1.Errors.FirstOrDefault().StartsWith("Name"))
            {
                return BadRequest("Dieser Username ist bereits vergeben.");
            }

            var token = UserManager.GeneratePasswordResetToken(user.Id);
            IdentityResult res2 = UserManager.ResetPassword(user.Id, token, model.Password);

            if (!res2.Succeeded)
            {
                return BadRequest("Passwörter stimmen nicht überein." + res2.Errors.FirstOrDefault());
            }

            await SendConfimationEmail(user);
           
            var currentStatistic = StatisticEventTypes.RegisterConverted;
            if (model.model != null)
            {
                if (model.model.IsIos)
                {
                    new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromIos);
                }
                else if (model.model.IsAndroid)
                {
                    new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromAndroid);
                }
                else if (model.model.IsWebPage)
                {
                    new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id), (int)StatisticEventTypes.ApiCallFromWebpage);
                }

            }
            else
            {
                new StatisticsController().CreateEntry(currentStatistic, new Guid(user.Id));
            }

            return Ok();
        }

        public UserSettings setUserSettings(ApplicationUser user)
        {
            return new UserSettings()
            {
                UserId = new Guid(user.Id),
                ActiveFrostAlert = true,
                ActiveStormAlert = true,
                ActiveNewPlantAlert = true,
                FrostDegreeBuffer = 3,
                AlertByEmail = true,
                AlertByPush = false
            };
        }
        [Route("api/AccountAPI/resendConfEmail")]
        public async Task<IHttpActionResult> resendConfEmail(LoginViewModel model)
        {
            ApplicationUser user = await UserManager.FindByEmailAsync(model.Email);
            await SendConfimationEmail(user);
            return Ok("Wir haben dir eine neue Bestätigungsmail versendet!");
        }
        [NonAction]
        public async Task<bool> SendConfimationEmail(ApplicationUser user)
        {
            var code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
            var uri = new Uri(FRONTEND_URL + $"?userId={user.Id}&code={HttpUtility.UrlEncode(code)}");
            var callbackUrl = uri;// this.Url.Link(FRONTEND_URL, new { controller = "AccountAPI/confirm", userId = user.Id, code });

            return await SendEmail("info@gardify.de", user.Email, "Email Bestätigung", "ConfirmEmail", new { UserName = user.DisplayName(), Action_Url = callbackUrl }, null);
        }

        [Route("api/AccountAPI/data")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateUserData(UserViewModel model)
        {
            var userId = Utilities.GetUserId();
            ApplicationUser user = await UserManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return BadRequest("Ein Fehler ist aufgetreten.");
            }
            user.UserName = model.UserName;
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Street = model.Street;
            user.HouseNr = model.HouseNr;
            user.City = model.City;
            try { user.PLZ = Int32.Parse(model.Zip); } catch (Exception) { return BadRequest("Die PLZ ist ungültig"); }
            user.Country = model.Country;
            await UserManager.UpdateAsync(user);
            return Ok();
        }

        [Authorize]
        [Route("api/AccountAPI/update/{id}")]
        [HttpPut]
        public async Task<IHttpActionResult> UpdateUserCreds(Guid id, ChangeUserDataViewModel model)
        {
            ApplicationUser user = await UserManager.FindByIdAsync(model.Id.ToString());
            var match = UserManager.PasswordHasher.VerifyHashedPassword(user.PasswordHash, model.Password);

            if (user == null || user.Deleted || !(match == PasswordVerificationResult.Success))
            {
                return BadRequest("Email oder Passwort ist ungültig.");
            }
            if (!user.EmailConfirmed)
            {
                await SendConfimationEmail(user);
                return BadRequest("Bitte bestätige erst deine Emailadresse. Wir haben dir soeben eine Bestätigungsmail geschickt.");
            }

            TempEmail te = new TempEmail() { UserId = user.Id, Email = model.NewEmail };
            te.OnCreate(user.Email);
            db.TempEmails.Add(te);
            db.SaveChanges();

            var code = await UserManager.GenerateUserTokenAsync("ChangeEmail", user.Id);
            var callbackUrl = this.Url.Link("", new { controller = "AccountAPI/confirmnew", code });
            var UserName = user.Gender + " " + user.LastName;
            var email = await SendEmail("info@gardify.de", model.NewEmail, "Email Änderung", "VerifyEmail", new { UserName, Action_Url = callbackUrl }, null);


            if (email)
            {
                return StatusCode(HttpStatusCode.NoContent);
            }

            return BadRequest("Ein Fehler ist beim Aktualisieren aufgetreten.");
        }

        [Route("api/AccountAPI/confirm")]
        [HttpGet]
        public async Task<IHttpActionResult> ConfirmUserEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest("Ein Fehler ist aufgetreten: Ungültiges URL.");
            }
            IdentityResult result;
            try
            {
                result = await UserManager.ConfirmEmailAsync(userId, code);
            }
            catch (InvalidOperationException ioe)
            {
                // ConfirmEmailAsync throws when the userId is not found.
                return BadRequest("Ein Fehler ist aufgetreten.");
            }

            if (result.Succeeded)
            {
                GardenViewModels.GardenCreateViewModel ng = new GardenViewModels.GardenCreateViewModel();
                NewsLetterController nc = new NewsLetterController();
                ng.Description = "Meine Gartenbeschreibung";
                ng.Name = "Mein Garten";
                GardenController gc = new GardenController();
                gc.CreateGardenOnRegister(ng, userId, false);
                var user = UserManager.FindById(userId);
                var existEmail = (from e in db.NewsLetters where !e.Deleted && e.Email == user.Email select e).FirstOrDefault();
                if (existEmail != null) {
                    NewsLetter item= existEmail;
                    item.EmailConfirmed = true;
                    item.OnEdit("System");
                    db.SaveChanges();
                };
                AccessTokenModel token = Authenticate(user, true, false);
                return Ok(token);
            }

            // If we got this far, something failed.
            return BadRequest("Email-Bestätigung war nicht erfolgreich.");
        }

        [Route("api/AccountAPI/confirmnew/{userId}")]
        [HttpGet]
        public async Task<IHttpActionResult> ChangeUserEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest("Ein Fehler ist aufgetreten: Ungültiges URL.");
            }

            if (!UserManager.VerifyUserToken(userId, "ChangeEmail", code))
            {
                return BadRequest("Ein Fehler ist aufgetreten: Ungültiges URL.");
            }

            TempEmail te = db.TempEmails.Where(e => e.UserId == userId && !e.Deleted).OrderBy(e => e.CreatedDate).FirstOrDefault();
            if (te == null || te.Deleted)
            {
                return BadRequest("Ein Fehler ist aufgetreten: Ungültiges URL.");
            }
            var user = await UserManager.FindByIdAsync(userId);
            user.Email = te.Email;
            user.UserName = te.Email;

            var result = await UserManager.UpdateAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(result.Errors.ElementAt(0));
            }

            te.Deleted = true;
            db.SaveChanges();

            return Ok("Emailadresse wurde erfolgreich geändert.");
        }

        [Authorize]
        [Route("api/AccountAPI/updatesettings")]
        [HttpPut]
        public IHttpActionResult UpdateUserSettings(UserSettingsViewModel model)
        {
            var userId = Utilities.GetUserId();
            var settings = db.UsersSettings.Where(us => us.UserId == userId && !us.Deleted).FirstOrDefault();
            if (settings != null && userId == model.UserId)
            {
                settings.FrostDegreeBuffer = model.FrostDegreeBuffer;
                settings.ActiveFrostAlert = model.ActiveFrostAlert;
                settings.ActiveStormAlert = model.ActiveStormAlert;
                settings.ActiveNewPlantAlert = model.ActiveNewPlantAlert;
                settings.AlertByEmail = model.AlertByEmail;
                settings.AlertByPush = false; // model.AlertByPush;
                settings.AlertStormEmailDisable = model.AlertStormEmailDisable;
                settings.AlertFrostEmailDisable = model.AlertFrostEmailDisable;
                settings.OnEdit("System");
                db.SaveChanges();
                return Ok();
            }

            return BadRequest();
        }

        [Authorize]
        [Route("api/AccountAPI/settings")]
        [HttpGet]
        public UserSettingsViewModel GetUserSettings(bool isIos=false, bool isAndroid=false, bool isWebPage=false)
        {
            var userId = Utilities.GetUserId();
            var settings = db.UsersSettings.Where(us => us.UserId == userId && !us.Deleted).FirstOrDefault();
            if (settings != null)
            {
                
                
                    if (isIos)
                    {
                        new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromIos, (int)GardifyPages.Settings, EventObjectType.PageName);
                    }
                    else if (isAndroid)
                    {
                        new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromAndroid, (int)GardifyPages.Settings, EventObjectType.PageName);
                    }
                    else if (isWebPage)
                    {
                        new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage, (int)GardifyPages.Settings, EventObjectType.PageName);
                    }

                
                
                return new UserSettingsViewModel()
                {
                    FrostDegreeBuffer = settings.FrostDegreeBuffer,
                    ActiveFrostAlert = settings.ActiveFrostAlert,
                    ActiveStormAlert = settings.ActiveStormAlert,
                    ActiveNewPlantAlert = settings.ActiveNewPlantAlert,
                    AlertByEmail = settings.AlertByEmail,
                    AlertByPush = settings.AlertByPush,
                    UserId = settings.UserId,
                    AlertFrostEmailDisable = settings.AlertFrostEmailDisable,
                    AlertStormEmailDisable = settings.AlertStormEmailDisable
                };
            }

            return null;
        }

        [Authorize]
        [Route("api/AccountAPI/delete/{id}")]
        [HttpDelete]
        public IHttpActionResult DeleteAspNetUser(string id)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            ApplicationUser ApplicationUser = db.Users.Find(id);
            if (ApplicationUser == null || ApplicationUser.Deleted)
            {
                return NotFound();
            }

            var settings = db.UsersSettings.Where(u => u.UserId.ToString() == ApplicationUser.Id).FirstOrDefault();
            if (settings != null)
            {
                settings.Deleted = true;
            }

            var tmpSource = System.Text.UnicodeEncoding.UTF8.GetBytes(ApplicationUser.UserName + DateTime.Now);
            var tmpHash = new MD5CryptoServiceProvider().ComputeHash(tmpSource);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < tmpHash.Length; i++)
            {
                sb.Append(tmpHash[i].ToString("X2"));
            }
            var deletedHash = sb.ToString();

            ApplicationUser.UserName = "Deleted-" + deletedHash;
            ApplicationUser.Email = "Deleted-" + deletedHash;
            ApplicationUser.FirstName = "Deleted-" + deletedHash;
            ApplicationUser.LastName = "Deleted-" + deletedHash;
            ApplicationUser.Street = "Deleted-" + deletedHash;
            ApplicationUser.LastLogin = DateTime.Now;
            ApplicationUser.HouseNr = "Deleted-" + deletedHash;


            var prop = db.Property.Where(p => p.UserId.ToString() == ApplicationUser.Id).FirstOrDefault();

            if (prop != null)
            {
                foreach (Garden ug in prop.Gardens)
                {
                    // delete user garden images
                    HelperClasses.DbResponse imageResponse = rc.DbGetGardenReferencedImages(ug.Id);
                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                    {
                        foreach (FileToModule ftm in imageResponse.ResponseObjects)
                        {
                            rc.DbDeleteFTM(ftm.FileToModuleID);
                        }
                    }
                }
            }

            // delete user's todos and diaries
            var todos = db.Todoes.Where(t => t.UserId.ToString() == ApplicationUser.Id);
            var todosCyc = db.TodoCyclic.Where(t => t.UserId.ToString() == ApplicationUser.Id);
            var diary = db.DiaryEntry.Where(d => d.UserId.ToString() == ApplicationUser.Id);
            if (todos != null && todos.Any())
            {
                db.Todoes.RemoveRange(todos);
            }
            if (todosCyc != null && todosCyc.Any())
            {
                db.TodoCyclic.RemoveRange(todosCyc);
            }
            if (diary != null && diary.Any()) 
            {
                db.DiaryEntry.RemoveRange(diary);
            }

            // anonymize his posts
            var ques = db.PlantDocs.Where(q => q.QuestionAuthorId.ToString() == ApplicationUser.Id);
            if (ques != null && ques.Any())
            {
                foreach (PlantDoc q in ques)
                {
                    q.CreatedBy = "anonym";
                }
            }
            var ans = db.PlantDocAnswers.Where(a => a.AuthorId.ToString() == ApplicationUser.Id);
            if(ans!=null && ans.Any())
            {
                foreach (PlantDocAnswer a in ans)
                {
                    a.CreatedBy = "anonym";
                }
            }
           

            // remove garden relation

            GardenController gc = new GardenController();
            var userid = Utilities.GetUserId();
            var mainGarden = gc.DbGetGardensByUserId(userid).FirstOrDefault();
            mainGarden.Deleted = true;
            //unsubscribe to newsletter
            var existEmail = (from e in db.NewsLetters where !e.Deleted && e.UserId.ToString() == ApplicationUser.Id select e).FirstOrDefault();
            if (existEmail != null)
            {

                existEmail.Deleted = true;
                existEmail.OnEdit("System");

            };
            db.SaveChanges();

            return Ok();
        }

        [Route("api/AccountAPI/teamview")]
        [HttpGet]
        public IHttpActionResult AddTeamViewEntry(bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            var userId = Utilities.GetUserId();

                if (isIos)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromIos, (int)GardifyPages.Team, EventObjectType.PageName);
                }
                else if (isAndroid)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromAndroid, (int)GardifyPages.Team, EventObjectType.PageName);
                }
                else if (isWebPage)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.Pageview, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage, (int)GardifyPages.Team, EventObjectType.PageName);
                }

            return Ok();
        }

        [Route("api/AccountAPI/adclick")]
        [HttpGet]
        public IHttpActionResult AddAdClickEntry(bool isIos = false, bool isAndroid = false, bool isWebPage = false)
        {
            new StatisticsController().CreateEntry(StatisticEventTypes.AdClicked, Utilities.GetUserId());
           
                if (isIos)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.AdClicked, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromIos);
                }
                else if (isAndroid)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.AdClicked, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromAndroid);
                }
                else if (isWebPage)
                {
                    new StatisticsController().CreateEntry(StatisticEventTypes.AdClicked, Utilities.GetUserId(), (int)StatisticEventTypes.ApiCallFromWebpage);
                }

           
            return Ok();
        }

        [Route("api/AccountAPI/mail/")]
        [HttpGet]
        private async Task<bool> SendEmail(string from, string to, string subject, string contentVm, object model, string[] filePaths = null)
        {
            EmailSender es = new EmailSender(db);
            TemplateService ts = new TemplateService();

            string content = ts.RenderTemplateAsync(contentVm, model);
            var res = await es.SendEmail(subject, content, from, to, filePaths);

            return res;
        }

        [Route("api/AccountAPI/userinfo/")]
        [HttpGet]
        public UserViewModel GetUserInfo()
        {
            PropertyController pc = new PropertyController();

            var userId = Utilities.GetUserId().ToString();
            var res = new UserViewModel();
            var user = db.Users.Where(u => userId == u.Id && !u.Deleted).FirstOrDefault();
            if (user != null)
            {
                res = new UserViewModel(user);
                var property = pc.DbGetProperty(Guid.Parse(user.Id));
                if (property == null)
                {
                    return res;
                }

                switch (property.Country)
                {
                    case "AT":
                        property.Country = "Österreich";
                        break;
                    case "DE":
                        property.Country = "Deutschland";
                        break;
                    case "CH":
                        property.Country = "Schweiz";
                        break;
                    default:
                        property.Country = property.Country;
                        break;
                }
                var ziptext = property.Zip;
 

                res.PropertyModel = new PropertyViewModels.PropertyCreateViewModel()
                {
                    City = property.City == "Platzhalter" ? "" : property.City,
                    Street = property.Street == "Platzhalter" ? "" : property.Street,
                    Zip = property.Zip,
                    Country = property.Country == "Platzhalter" ? "" : property.Country
                };
            }
            return res;
        }


        [Route("api/AccountAPI/otheruserinfo/{username}/{userinfoId}")]
        [HttpGet]
        public UserViewModel GetOtherUserInfoByUserName(string username, string userinfoId)
        {
            PropertyController pc = new PropertyController();

            var userId = Utilities.GetUserId().ToString();
            var res = new UserViewModel();
            var user = db.Users.Where(u => userId == u.Id && !u.Deleted).FirstOrDefault();
            if (user == null)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    ReasonPhrase = "Bitte Einloggen"
                };
                throw new HttpResponseException(resp);
            }

            var selectedUser = db.Users.FirstOrDefault(u => (username != "0" && u.UserName == username) || (userinfoId != "0" && u.Id.ToLower() == userinfoId.ToLower()));

            if (selectedUser == null)
            {
                var resp = new HttpResponseMessage(HttpStatusCode.NotFound)
                {
                    ReasonPhrase = "Kunde existiert nicht"
                };
                throw new HttpResponseException(resp);
            }

            res = new UserViewModel(selectedUser);

            var property = pc.DbGetProperty(Guid.Parse(selectedUser.Id));
            if (property == null)
            {
                return res;
            }

            switch (property.Country)
            {
                case "AT":
                    property.Country = "Österreich";
                    break;
                case "DE":
                    property.Country = "Deutschland";
                    break;
                case "CH":
                    property.Country = "Schweiz";
                    break;
                default:
                    property.Country = property.Country;
                    break;
            }

            res.PropertyModel = new PropertyViewModels.PropertyCreateViewModel()
            {
                City = property.City == "Platzhalter" ? "" : property.City,
                Street = property.Street == "Platzhalter" ? "" : property.Street,
                Zip = property.Zip,
                Country = property.Country == "Platzhalter" ? "" : property.Country
            };

            return res;
        }

        [Route("api/AccountAPI/addtocontact")]
        [HttpPost]
        public IHttpActionResult AddOtherUserToContact(UserContactAddViewModel vm)
        {

            var userId = Utilities.GetUserId().ToString();
            var user = db.Users.Where(u => userId == u.Id && !u.Deleted).FirstOrDefault();
            if (userId == null)
            {
                return Content(HttpStatusCode.BadRequest, "Bitte einloggen");
            }

            var selectedUser = db.Users.FirstOrDefault(u => u.UserName == vm.Username);

            if (selectedUser == null)
            {
                return Content(HttpStatusCode.BadRequest, "Kunde existiert nicht");

            }

            UserContact contact = new UserContact
            {
                UserId = Guid.Parse(userId),
                ContactUserId = Guid.Parse(selectedUser.Id)
            };
            contact.OnCreate(user.UserName);
            var existing = db.UserContacts.Any(u => u.UserId == contact.UserId && u.ContactUserId == contact.ContactUserId && !u.Deleted);

            if (existing)
            {
                return Content(HttpStatusCode.BadRequest, "Kontakt bereits vorhanden");
            }

            db.UserContacts.Add(contact);
            db.SaveChanges();
            return Ok(contact);
        }

        [Route("api/AccountAPI/removeFromContact")]
        [HttpPost]
        public IHttpActionResult RemoveOtherUserToContact(UserContactAddViewModel vm)
        {

            var userId = Utilities.GetUserId();
            var user = db.Users.Where(u => userId.ToString() == u.Id && !u.Deleted).FirstOrDefault();

            if (userId == null)
            {
                return Content(HttpStatusCode.BadRequest, "Bitte einloggen");
            }

            var selectedUser = db.Users.FirstOrDefault(u => u.UserName == vm.Username);

            if (selectedUser == null)
            {
                return Content(HttpStatusCode.BadRequest, "Kunde existiert nicht");

            }

            var parsedUserId = (Guid.Parse(selectedUser.Id));

            var existing = db.UserContacts.FirstOrDefault(u => u.UserId == userId && u.ContactUserId == parsedUserId);


            //UserContact contact = new UserContact
            //{
            //    UserId = Guid.Parse(userId),
            //    ContactUserId = Guid.Parse(selectedUser.Id)
            //};
            //contact.OnCreate(user.UserName);
            //var existing = db.UserContacts.Any(u => u.UserId == contact.UserId && u.ContactUserId == contact.ContactUserId);

            if (existing == null)
            {
                return Content(HttpStatusCode.BadRequest, "Kontakt existiert nicht");
            }


            existing.Deleted = true;
            existing.OnEdit(user.UserName);
            db.SaveChanges();
            return Ok("Kontakt gelöscht");
        }

        [Route("api/AccountAPI/getcontact")]
        [HttpGet]
        public IHttpActionResult GetContactUser()
        {

            PropertyController pc = new PropertyController();

            var userId = Utilities.GetUserId();
            if (userId == null)
            {
                return Content(HttpStatusCode.BadRequest, "Bitte einloggen");
            }

            return Ok(GetContacts());
        }

        [Route("api/AccountAPI/checkcontact")]
        [HttpGet]
        public IHttpActionResult CheckContactUser(string contactUserName, string contactId)
        {

            PropertyController pc = new PropertyController();

            var userId = Utilities.GetUserId();
            if (userId == null)
            {
                return Content(HttpStatusCode.BadRequest, "Bitte einloggen");
            }

            return Ok(CheckContacts(contactUserName, contactId));
        }


        public List<UserViewModel> GetContacts()
        {
            PropertyController pc = new PropertyController();

            var userId = Utilities.GetUserId();
            if (userId == null)
            {
                return null;
            }

            var contactItems = db.UserContacts.Where(u => u.UserId == userId && !u.Deleted).ToList();

            List<UserViewModel> vm = new List<UserViewModel>();

            foreach (var item in contactItems)
            {
                var contact = db.Users.FirstOrDefault(u => u.Id == item.ContactUserId.ToString());
                if (contact == null)
                {
                    continue;
                }
                vm.Add(new UserViewModel(contact));
            }
            return vm;
        }

        public bool CheckContacts(string contactUserName, string contactUserId)
        {
            PropertyController pc = new PropertyController();

            var userId = Utilities.GetUserId();
            if (userId == null)
            {
                return false;
            }


            var contactItems = db.UserContacts.Where(u => u.UserId == userId && !u.Deleted && (contactUserId != null ? contactUserId.ToLower() == u.ContactUserId.ToString().ToLower() : true)).ToList();

            if (!contactItems.Any())
            {
                return false;
            }
            List<UserViewModel> vm = new List<UserViewModel>();

            foreach (var item in contactItems)
            {
                var contact = db.Users.FirstOrDefault(u => u.Id == item.ContactUserId.ToString());
                if (contact == null)
                {
                    continue;
                }

                if(contact.UserName == contactUserName)
                {
                    //vm.Add(new UserViewModel(contact));
                    return true;
                }

            }

            return false;
        }

        [HttpPost]
        [Route("api/AccountAPI/uploadProfilImg")]
        public async Task<IHttpActionResult> UploadRelatedAnswerImageAsync()
        {
            AccountController ac = new AccountController();
            var userId = Utilities.GetUserId();
            var userProperty = new PropertyController().DbGetProperty(userId);
            
            if (HttpContext.Current.Request.Files[0] != null && userProperty != null)
            {
                var userPropertyId = userProperty.Id;
                var imageFile = HttpContext.Current.Request.Files[0];
                var imageTitle = HttpContext.Current.Request.Params["imageTitle"];
                HttpPostedFileBase filebase = new HttpPostedFileWrapper(imageFile);
                ac.UploadProfilImage(filebase, imageFile, userPropertyId, imageTitle);
            }
            return StatusCode(HttpStatusCode.NoContent);
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private AccessTokenModel Authenticate(ApplicationUser user, bool rememberMe, bool simpleMode)
        {
            var identity = new ClaimsIdentity(Startup.OAuthOptions.AuthenticationType);
            identity.AddClaim(new Claim(ClaimTypes.Name, user.Id));
            identity.AddClaim(new Claim(ClaimTypes.NameIdentifier, user.Id));

            AuthenticationTicket ticket = new AuthenticationTicket(identity, new AuthenticationProperties());
            int validForDays = rememberMe ? 90 : 2;
            var currentUtc = DateTime.Now;
            var expiresAt = currentUtc.Add(TimeSpan.FromDays(validForDays));
            ticket.Properties.IssuedUtc = currentUtc;
            ticket.Properties.ExpiresUtc = expiresAt;
            string AccessToken = Startup.OAuthOptions.AccessTokenFormat.Protect(ticket);
            var token = new AccessTokenModel()
            {
                Token = AccessToken,
                ExpiresUtc = expiresAt,
                Admin = user.Roles.Join(db.Roles, ur => ur.RoleId, u => u.Id, (userRole, role) => role).Any(r => r.Name.Contains("Admin") || r.Name.Contains("Expert")),
                UserId = user.Id,
                Email = user.Email.Contains("Platzhalter") ? user.UserName : user.Email,
                Name = user.UserName,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Street = user.Street,
                PLZ = user.PLZ,
                HouseNr = user.HouseNr,
                Country = user.Country,
                City = user.City
            };            

            var tokenStrRes = "";
            var tokenStr = JsonConvert.SerializeObject(token);

            JsonReader reader = new JsonTextReader(new StringReader(tokenStr.ToString()));
            reader.DateParseHandling = DateParseHandling.None;
            JObject deToken = JObject.Load(reader);

            PropertyInfo[] tokenProps = typeof(AccessTokenModel).GetProperties().Where(p => p.Name != "Signature").OrderBy(p => p.Name).ToArray();
            foreach (PropertyInfo prop in tokenProps)
            {
                tokenStrRes += deToken[prop.Name];
            }

            using (SHA512 sha512Hash = SHA512.Create())
            {
                byte[] sourceBytes = Encoding.UTF8.GetBytes(tokenStrRes + ACCESS_TOKEN_SECRET);
                byte[] hashBytes = sha512Hash.ComputeHash(sourceBytes);
                token.Signature = BitConverter.ToString(hashBytes).Replace("-", String.Empty);
            }
            return token;
        }

        private class AccessTokenModel
        {
            public string Token { get; set; }
            public DateTime ExpiresUtc { get; set; }
            public string UserId { get; set; }
            public string Email { get; set; }
            public string Name { get; set; }
            public bool Admin { get; set; }
            public string Street { get; set; }
            public string HouseNr { get; set; }
            public int PLZ { get; set; }
            public string City { get; set; }
            public string Country { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Signature { get; set; }
        }

        public class ConfirmUserModel
        {
            public string code { get; set; }
            public string userId { get; set; }
        }

        public class UserViewModel
        {
            public UserViewModel()
            {

            }

            public UserViewModel(ApplicationUser user)
            {
                FirstName = user.FirstName != "Platzhalter" ? user.FirstName : "";
                LastName = user.LastName != "Platzhalter" ? user.LastName : "";
                UserName = user.UserName;
                City = user.City == "Platzhalter" ? "" : user.City;
                Street = user.Street == "Platzhalter" ? "" : user.Street;
                HouseNr = user.HouseNr;
                Zip = ((user.PLZ.ToString().Length != 5 && user.Country == "Deutschland") || (user.PLZ.ToString().Length != 4 && user.Country != "Deutschland"))? ("0"+user.PLZ.ToString()) : user.PLZ.ToString();
                Country = user.Country == "Platzhalter" ? "" : user.Country;
                ResetTodo = user.ResetTodo;
                RegistrationDate = user.RegisterDate;
                UserId = user.Id;
            }

            public string HouseNr { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string UserName { get; set; }
            public string City { get; set; }
            public string Street { get; set; }
            public string Zip { get; set; }
            public string UserId { get; set; }

            public string Country { get; set; }
            public DateTime? RegistrationDate { get; set; }
            public int ResetTodo { get; set; }

            public PropertyViewModels.PropertyCreateViewModel PropertyModel { get; set; }
        }

        private void DbCreateProperty(string street, string zip, string city, string country, string userId)
        {
            Property newProp = new Property();
            newProp.City = city;
            newProp.Country = country;
            newProp.CreatedBy = Utilities.GetUserName();
            newProp.CreatedDate = DateTime.Now;
            newProp.Deleted = false;
            newProp.EditedBy = Utilities.GetUserName();
            newProp.EditedDate = DateTime.Now;
            newProp.Street = street;
            newProp.UserId = new Guid(userId);
            newProp.Zip = zip;
            newProp.UpdateCoordinates();
            newProp.OnCreate("System");
            db.Property.Add(newProp);
            db.SaveChanges();


        }

        //Subscription Part

        [Route("api/AccountAPI/getSubscriptionStatus")]
        [HttpGet]
        public IHttpActionResult getSubscriptionStatus()
        {
            PropertyController pc = new PropertyController();

            var userId = Utilities.GetUserId();
            if (userId == null)
            {
                return Content(HttpStatusCode.BadRequest, "Bitte einloggen");
            }

            var subscriptionStatus = db.Subscription.FirstOrDefault(s => s.UserId == userId);


            if(subscriptionStatus == null)
            {
                Subscription newSub = new Subscription
                {
                    UserId = userId
                };

                newSub.OnCreate("system");

                db.Subscription.Add(newSub);
                db.SaveChanges();

                return Ok(newSub);
            }



            return Ok(subscriptionStatus);
        }

        [Route("api/AccountAPI/startTestPeriod")]
        [HttpPost]
        public IHttpActionResult startTestPeriod()
        {
            PropertyController pc = new PropertyController();

            var userId = Utilities.GetUserId();
            if (userId == null)
            {
                return Content(HttpStatusCode.BadRequest, "Bitte einloggen");
            }

            var subscriptionStatus = db.Subscription.FirstOrDefault(s => s.UserId == userId);

            var trialDaysPeriod = 15;
            var currentDate = DateTime.Now;

            var endTrialDate = currentDate.AddDays(trialDaysPeriod);

            if (subscriptionStatus == null)
            {
                Subscription newSub = new Subscription
                {
                    UserId = userId,
                    TestStartDate = currentDate,
                    TestEndDate = endTrialDate
                };

                newSub.OnCreate("system");

                db.Subscription.Add(newSub);
                db.SaveChanges();



                return Ok(newSub);
            }

            if (subscriptionStatus.IsTestReceive)
            {
                return Content(HttpStatusCode.BadRequest, "Trial Already Received");

            }

            subscriptionStatus.TestStartDate = currentDate;
            subscriptionStatus.TestEndDate = endTrialDate;
            subscriptionStatus.OnEdit("system");
            db.SaveChanges();

            return Ok(subscriptionStatus);
        }

        [Route("api/AccountAPI/subscriptionValidation")]
        [HttpPost]
        public IHttpActionResult subscriptionValidation(TransactionRequestModel vm)
        {
            var userId = Utilities.GetUserId();
            if (userId == null)
            {
                return Content(HttpStatusCode.BadRequest, "Bitte einloggen");
            }

            var output = iosValidationProcess(vm);

            var subscriptionStatus = db.Subscription.FirstOrDefault(s => s.UserId == userId);
            var utcDateString = output.latest_receipt_info.First().expires_date.Replace(" Etc/GMT", "");
            var localTime = DateTime.Parse(utcDateString).ToLocalTime();
            var currentDate = DateTime.Now;

            if(currentDate > localTime)
            {
                return Content(HttpStatusCode.BadRequest, "bitte gültigen Beleg eingeben");

            }

            if (subscriptionStatus == null)
            {
                Subscription newSub = new Subscription
                {
                    UserId = userId,
                    MonthlyStartDate = currentDate,
                    MonthlyEndDate = localTime
                };

                newSub.OnCreate("system");

                db.Subscription.Add(newSub);
                db.SaveChanges();



                return Ok(newSub);
            }
            //var endDate = currentDate.AddDays(trialDaysPeriod);
            if(subscriptionStatus.MonthlyStartDate == null)
            {
                subscriptionStatus.MonthlyStartDate = currentDate;

            }

            subscriptionStatus.MonthlyEndDate = localTime;
            subscriptionStatus.OnEdit("system");
            db.SaveChanges();

            return Ok(subscriptionStatus);
        }

        public IOSTransactionInputModel iosValidationProcess(TransactionRequestModel vm, bool isTest = true)
        {
            var client = new RestClient("https://sandbox.itunes.apple.com/verifyReceipt");
            if (!isTest)
            {
                client = new RestClient("https://buy.itunes.apple.com/verifyReceipt");
            }
            client.Timeout = -1;
            var request = new RestRequest(Method.POST);
            request.AddHeader("Content-Type", "application/json");
            var body = @"{
" + "\n" +
            @"    ""exclude-old-transactions"": true
" + "\n" +
            @"    ""password"": ""25d8723135df48af97832cf0b815a828""
" + "\n" +
            @"    ""receipt-data"": """ + vm.Data + @"""" + "\n" +
            @"
" + "\n" +
            @"}";
            request.AddParameter("application/json", body, ParameterType.RequestBody);
            IRestResponse response = client.Execute(request);
            Console.WriteLine(response.Content);

            var data = JsonConvert.DeserializeObject<IOSTransactionInputModel>(response.Content);

            return data;
        }
    }
}