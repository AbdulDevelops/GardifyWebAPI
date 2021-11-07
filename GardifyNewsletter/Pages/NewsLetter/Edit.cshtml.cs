using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FileUploadLib.Repositories;
using GardifyNewsletter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace GardifyNewsletter.Areas.Intern.Pages.Newsletter
{
    [Authorize(Roles = "Admin,Superadmin")]
    public class EditModel : PageModel
    {
        private readonly GardifyNewsletter.Models.ApplicationDbContext _context;
        private readonly IHostingEnvironment _env;
        private readonly FileToModuleRepository _fileToModules;
        private string websiteUrlBackend = "https://gardifybackend.sslbeta.de/";
        private string GardifyDeUrl = "https://gardify.de/";
        private string GardifyNewsletterUrl = "https://gardifynewsletter.sslbeta.de/";
        private static int textCharacterLimit = 680;
        [BindProperty]
        public Models.Newsletter Newsletter { get; set; }
        [BindProperty]
        public List<NewsletterSubComponents> SubComponentsList { get; set; }

        public EditModel(GardifyNewsletter.Models.ApplicationDbContext context, IHostingEnvironment env, FileToModuleRepository fileToModules)
        {
            _fileToModules = fileToModules;
            _env = env;
            _context = context;
        }
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return RedirectToPage("./Index");
            }
            Newsletter = _context.Newsletter.Find(id);
            if (Newsletter == null)
            {
                return RedirectToPage("./Index");
            }
            UpdateNewsletterHtml(Newsletter);
            Newsletter = _context.Newsletter.Find(id);



            /*
             * parse shop article list json
             */
            WebClient webClient = new WebClient();
            string shopUrl = websiteUrlBackend + "api/ArticlesAPI/getall";
            string json_shop = await webClient.DownloadStringTaskAsync(shopUrl);
            var modelShop = JsonConvert.DeserializeObject<ShopRootObj>(json_shop);

            IList<ListEntries> shopArticleList = modelShop.ListEntries;
            ViewData["ShortArticleList"] = shopArticleList;


            return Page();
        }

        public IActionResult OnPost()
        {
            _context.Newsletter.Update(Newsletter);
            _context.SaveChanges();
            UpdateNewsletterHtml(Newsletter.NewsletterId);
            return RedirectToPage(new { id = Newsletter.NewsletterId });
        }

        public async Task<IActionResult> OnGetAddComponentAsync(int id, ComponentType componentType)
        {
            var newsletter = _context.Newsletter.FirstOrDefault(n => n.NewsletterId == id);
            if (newsletter == null)
            {
                return RedirectToPage("./Index");
            }
            int? sort = null;
            if (componentType != ComponentType.PreText && componentType != ComponentType.PreTextWithoutImage && componentType != ComponentType.PostText && componentType != ComponentType.PostTextWithoutImage)
            {
                //New component is not pre or post text and has to receive a sort value

                //Find existing components to determine max sort
                sort = (_context.NewsletterComponents
                    .Where(n => n.BelongsToNewsletterId == id && n.Sort != null)
                    .Max(n => n.Sort) ?? -1) + 1;
            }

            NewsletterComponents newComp = new NewsletterComponents
            {
                Active = true,
                BelongsToNewsletterId = id,
                Deleted = false,
                EditedBy = User.Identity.Name,
                EditedDate = DateTime.Now,
                NotEditable = false,
                Sort = sort,
                WrittenBy = User.Identity.Name,
                WrittenDate = DateTime.Now,
                NewsletterComponentTemplateId = componentType
            };

            _context.Add(newComp);
            _context.SaveChanges();
            UpdateNewsletterHtml((int)newComp.BelongsToNewsletterId);


            return RedirectToPage(new { id });
        }

        public IActionResult OnPostUpdateComponent(NewsletterComponents component, int? newsId, int? shortArticleId)
        {
            var dbComponent = _context.NewsletterComponents.Include(t => t.NewsletterNewPlants).FirstOrDefault(t => t.NewsletterComponentId == component.NewsletterComponentId);

            if (dbComponent == null)
            {
                return RedirectToPage("./Index");
            }
            dbComponent.NewsId = newsId;
            dbComponent.ShortArticleId = shortArticleId;
            dbComponent.NewsletterMoreLink = component.NewsletterMoreLink;
            dbComponent.NewsletterNewPlants = component.NewsletterNewPlants;
            dbComponent.CustomLinkText = component.CustomLinkText;
            dbComponent.NewsleterComponentText = component.NewsleterComponentText;
            dbComponent.NewsletterComponentHeadline = component.NewsletterComponentHeadline;
            dbComponent.EditedBy = User.Identity.Name;
            dbComponent.EditedDate = DateTime.Now;
            _context.Update(dbComponent);
            _context.SaveChanges();
            UpdateNewsletterHtml((int)dbComponent.BelongsToNewsletterId);

            return RedirectToPage(new { id = dbComponent.BelongsToNewsletterId });
        }

        public IActionResult OnGetDeleteComponent(int id)
        {
            var component = _context.NewsletterComponents.Find(id);
            if (component == null)
            {
                return RedirectToPage("./Index");
            }
            _context.Remove(component);
            _context.SaveChanges();
            UpdateNewsletterHtml((int)component.BelongsToNewsletterId);
            return RedirectToPage(new { id = component.BelongsToNewsletterId });
        }

        public IActionResult OnGetSetComponentSort(int id, int sort)
        {
            var component = _context.NewsletterComponents.Find(id);
            if (component == null)
            {
                return RedirectToPage("./Index");
            }

            var componentWithSort = _context.NewsletterComponents.FirstOrDefault(c => c.BelongsToNewsletterId == component.BelongsToNewsletterId && c.Sort == sort);
            if (componentWithSort == null)
            {
                return RedirectToPage(new { id = component.BelongsToNewsletterId });
            }

            componentWithSort.Sort = component.Sort;
            component.Sort = sort;
            _context.Update(component);
            _context.Update(componentWithSort);
            _context.SaveChanges();

            UpdateNewsletterHtml((int)component.BelongsToNewsletterId);

            return RedirectToPage(new { id = component.BelongsToNewsletterId });
        }

        public IActionResult OnGetShowNewsletterPreview(int id)
        {
            var newsletter = _context.Newsletter.Find(id);
            if (newsletter == null)
            {
                return RedirectToPage("./Index");
            }
            return Content(newsletter.NewsletterCompleteHtml, "text/html", Encoding.UTF8);
        }


        /// <summary>
        /// Updates the NewsletterCompleteHtml property of the given newsletter id
        /// </summary>
        /// <param name="newsletter"></param>
        /// <returns></returns>
        public void UpdateNewsletterHtml(int id)
        {
            var newsletter = _context.Newsletter.Find(id);
            if (newsletter != null)
            {
                UpdateNewsletterHtml(newsletter);
            }
        }

        /// <summary>
        /// Updates the NewsletterCompleteHtml property of the given newsletter object
        /// </summary>
        /// <param name="newsletter"></param>
        /// <returns></returns>
        public bool UpdateNewsletterHtml(Models.Newsletter newsletter)
        {
            var newHtml = GenerateHtml(newsletter);
            newsletter.NewsletterCompleteHtml = newHtml;
            _context.Update(newsletter);
            return _context.SaveChanges() > 0;
        }

        /// <summary>
        /// Generates the full html code for a newsletter
        /// </summary>
        /// <param name="newsletter"></param>
        /// <returns></returns>
        public string GenerateHtml(Models.Newsletter newsletter)
        {
            string htmlString = string.Empty;

            string dividerHtml = GetTemplateText("Divider", newsletter.ApplicationId);

            //Generate header
            var header = GetRenderedTemplateText("Header",
                    newsletter.ApplicationId,
                ("Headline", newsletter.NewsletterHeaderText),
                ("Datum", newsletter.NewsletterDateShownOnNewsletter),
                ("NewsletterID", newsletter.NewsletterId.ToString())
                );
            htmlString += header;

            //Generate contents
            var mainComponents = newsletter.NewsletterComponents?.OrderByDescending(c => c.NewsletterComponentTemplateId == ComponentType.PreText || c.NewsletterComponentTemplateId == ComponentType.PreTextWithoutImage).ThenBy(c => c.NewsletterComponentTemplateId == ComponentType.PostText || c.NewsletterComponentTemplateId == ComponentType.PostTextWithoutImage).ThenBy(c => c.Sort);
            if (mainComponents != null && mainComponents.Any())
            {
                foreach (var component in mainComponents)
                {
                    //Find link for image
                    string picLink = component.NewsletterPicLink;
                    //Image was uploaded by the user
                    if (string.IsNullOrEmpty(picLink))
                    {
                        var relatedImage = _fileToModules.GetFiles("Newsletter", component.NewsletterComponentId).FirstOrDefault()?.File;
                        picLink = GardifyNewsletterUrl + relatedImage?.UriPath;
                    }

                    //Find texts and links for component
                    string headline = component.NewsletterComponentHeadline;
                    string text = component.NewsleterComponentText;
                    string moreLink = component.NewsletterMoreLink;
                    string CustomLinkText = component.CustomLinkText;
                    string plantColorSpan = "";
                    string plantBadgeSpan = "";

                    // new plant details
                    string headlineNewPlant1 = "", subHeadlineNewPlant1="", textNewPlant1 = "", 
                        headlineNewPlant2 = "", subHeadlineNewPlant2 = "", textNewPlant2 = "", newplantPicLink2 = "",
                        headlineNewPlant3 = "", subHeadlineNewPlant3 = "", textNewPlant3 = "", newplantPicLink3 = "";


                    if (component.NewsletterComponentTemplateId == ComponentType.NewsWithImage && component.NewsId != null)
                    {
                        // plant badges
                        Dictionary<string, string> badgesDictionary = getBadges();

                        // plant colors
                        Dictionary<string, string> colorsDictionary = getColors();

                        // parse json
                        string Post_Status = websiteUrlBackend + "api/PlantSearchAPI/" + component.NewsId;
                        WebClient webClient = new WebClient();
                        string json = webClient.DownloadString(Post_Status);
                        var myJObject = JObject.Parse(json);

                        headline = myJObject.SelectToken("NameGerman").Value<string>();
                        text = myJObject.SelectToken("Description").Value<string>();
                        var plantColorList = myJObject.SelectToken("Colors").ToList();
                        var plantTagsList = myJObject.SelectToken("PlantTags").ToList();

                        plantColorSpan = colorsGenerator(colorsDictionary, plantColorList);
                        plantBadgeSpan = badgeGenerator(badgesDictionary, plantTagsList);

                        moreLink = GardifyDeUrl + "pflanze/" + component.NewsId + "/" + headline;

                        // parse images array
                        var PlantImages = myJObject["Images"].ToArray();
                        if (PlantImages.Count() > 0)
                        {
                            picLink = myJObject["Images"].First["SrcAttr"].ToString();
                            picLink = GardifyDeUrl + "intern/" + picLink;
                        }
                        else
                        {
                            picLink = GardifyDeUrl + "intern/" + myJObject.SelectToken("PhotoLink").Value<string>();
                        }

                    }
                    else if (component.NewsletterComponentTemplateId == ComponentType.ArticleWithImage && component.ShortArticleId != null)
                    {
                        // parse json
                        string Post_Status = websiteUrlBackend + "api/articlesapi/articleById/" + component.ShortArticleId;
                        WebClient webClient = new WebClient();
                        string json = webClient.DownloadString(Post_Status);
                        var myJObject = JObject.Parse(json);

                        headline = myJObject.SelectToken("Name").Value<string>();
                        text = myJObject.SelectToken("Description").Value<string>();
                        moreLink = GardifyDeUrl + "artikel/" + component.ShortArticleId + "/" + headline;


                        var ArticleImages = myJObject["ArticleImages"].ToArray();
                        // parse images array
                        if (ArticleImages.Count() > 0)
                        {
                            picLink = myJObject["ArticleImages"].First["SrcAttr"].ToString();
                            picLink = GardifyDeUrl + "intern/" + picLink;
                        }
                        else
                        {
                            string photoLinkUrl = myJObject.SelectToken("PhotoLink").Value<string>();
                            if (ValidateUrl(photoLinkUrl))
                            {
                                picLink = GardifyDeUrl + "intern/" + myJObject.SelectToken("PhotoLink").Value<string>();
                            }
                            else
                            {
                                picLink = photoLinkUrl;
                            }
                        }
                    }
                    else if (component.NewsletterComponentTemplateId == ComponentType.NewPlantWithImage && component.NewsletterNewPlants != null)
                    {

                        // plant badges
                        Dictionary<string, string> badgesDictionary = getBadges();

                        // plant colors
                        Dictionary<string, string> colorsDictionary = getColors();

                        /*
                         * 
                         * new plant image 1
                         * 
                         */

                        string Post_Status = websiteUrlBackend + "api/plantsearchapi/" + component.NewsletterNewPlants.NewPlant1;
                        WebClient webClient = new WebClient();
                        string json = webClient.DownloadString(Post_Status);
                        var myJObject = JObject.Parse(json);
                        headlineNewPlant1 = myJObject.SelectToken("NameGerman").Value<string>();
                        textNewPlant1 = myJObject.SelectToken("Description").Value<string>();
                        subHeadlineNewPlant1 = component.NewsletterNewPlants.NewPlant1SubHeadline;

                        textNewPlant1 = ConvertKTag(textNewPlant1);

                        var plantColorList = myJObject.SelectToken("Colors").ToList();
                        var plantTagsList = myJObject.SelectToken("PlantTags").ToList();

                        plantColorSpan = colorsGenerator(colorsDictionary, plantColorList);
                        plantBadgeSpan = badgeGenerator(badgesDictionary, plantTagsList);

                        moreLink = component.NewsletterMoreLink;

                        // parse images array
                        var PlantImages = myJObject["Images"].ToArray();
                        if (PlantImages.Count() > 0)
                        {
                            picLink = myJObject["Images"].First["SrcAttr"].ToString();
                            picLink = GardifyDeUrl + "intern/" + picLink;
                        }
                        else
                        {
                            picLink = GardifyDeUrl + "intern/" + myJObject.SelectToken("PhotoLink").Value<string>();
                        }

                        /*
                        * 
                        * new plant image 2
                        * 
                        */
                        if (component.NewsletterNewPlants.NewPlant2 != null)
                        {
                            string Post_Status_new_plant2 = websiteUrlBackend + "api/plantsearchapi/" + component.NewsletterNewPlants.NewPlant2;
                            string json2 = webClient.DownloadString(Post_Status_new_plant2);
                            var myJObject2 = JObject.Parse(json2);

                            headlineNewPlant2 = myJObject2.SelectToken("NameGerman").Value<string>();
                            textNewPlant2 = myJObject2.SelectToken("Description").Value<string>();
                            subHeadlineNewPlant2 = component.NewsletterNewPlants.NewPlant2SubHeadline;

                            textNewPlant2 = ConvertKTag(textNewPlant2);

                            // parse images array
                            var PlantImages2 = myJObject["Images"].ToArray();
                            if (PlantImages2.Count() > 0)
                            {
                                newplantPicLink2 = myJObject2["Images"].First["SrcAttr"].ToString();
                                newplantPicLink2 = GardifyDeUrl + "intern/" + newplantPicLink2;
                            }
                            else
                            {
                                newplantPicLink2 = GardifyDeUrl + "intern/" + myJObject2.SelectToken("PhotoLink").Value<string>();
                            }
                        }


                        /*
                        * 
                        * new plant image 3
                        * 
                        */
                        if (component.NewsletterNewPlants.NewPlant3 != null)
                        {
                            string Post_Status_new_plant3 = websiteUrlBackend + "api/plantsearchapi/" + component.NewsletterNewPlants.NewPlant3;
                            string json3 = webClient.DownloadString(Post_Status_new_plant3);
                            var myJObject3 = JObject.Parse(json3);

                            headlineNewPlant3 = myJObject3.SelectToken("NameGerman").Value<string>();
                            textNewPlant3 = myJObject3.SelectToken("Description").Value<string>();
                            subHeadlineNewPlant3 = component.NewsletterNewPlants.NewPlant3SubHeadline;

                            textNewPlant3 = ConvertKTag(textNewPlant3);

                            // parse images array
                            var PlantImages3 = myJObject["Images"].ToArray();
                            if (PlantImages3.Count() > 0)
                            {
                                newplantPicLink3 = myJObject3["Images"].First["SrcAttr"].ToString();
                                newplantPicLink3 = GardifyDeUrl + "intern/" + newplantPicLink3;
                            }
                            else
                            {
                                newplantPicLink3 = GardifyDeUrl + "intern/" + myJObject3.SelectToken("PhotoLink").Value<string>();
                            }
                        }
                    }

                    text = ConvertKTag(text);

                    //  validating the URL                     
                    if (ValidateUrl(moreLink))
                    {
                        moreLink = "http://" + moreLink;
                    }

                    CustomLinkText = CustomLinkText != null ? CustomLinkText : "MEHR";

                    string moreBtnHtmlCol = "";
                    if (moreLink != null)
                    {
                        moreBtnHtmlCol = "<td bgcolor=\"#7a9d34\" width =\"160\" align =\"left\" style =\" vertical-align: middle; text-align: center; font-family:'Roboto', Calibri, 'Trebuchet MS', sans-serif; font-weight:500; font-size: 14px; padding: 8px; text-decoration: none;  line-height: 1.4em; max-height: 5.1em;\" > " +

                                "<a style=\"width: 150px; color: #FFFFFF; font-weight:400; text-decoration: none; display: inline-block;\" href=\"" + moreLink + "\">" + CustomLinkText + "</a></td>";
                    }
                    var componentHtml = GetRenderedTemplateText(component.NewsletterComponentTemplateId.ToString(),
                        newsletter.ApplicationId,
                    ("Headline", headline),
                    ("Text", text),
                    ("Image", picLink),
                    ("PlantColor", plantColorSpan),
                    ("BadgeIcon", plantBadgeSpan),
                    ("MoreLink", moreLink),
                    ("MoreLinkComplete", moreBtnHtmlCol),

                    ("headlineNewPlant1", headlineNewPlant1),// new Plants 1 details 
                    ("SubHeadingPlant1", subHeadlineNewPlant1),
                    ("textNewPlant1", textNewPlant1),

                    ("headlineNewPlant2", headlineNewPlant2),// new Plants 2 details 
                    ("SubHeadingPlant2", subHeadlineNewPlant2),
                    ("textNewPlant2", textNewPlant2),
                    ("newplantPicLink2", newplantPicLink2),

                    ("headlineNewPlant3", headlineNewPlant3),// new Plants 3 details 
                    ("SubHeadingPlant3", subHeadlineNewPlant3),
                    ("textNewPlant3", textNewPlant3),
                    ("newplantPicLink3", newplantPicLink3)
                    );
                    htmlString += componentHtml;
                    htmlString += dividerHtml;
                }
            }

            //Generate footer
            var footer = GetRenderedTemplateText("Footer", newsletter.ApplicationId);
            htmlString += footer;

            return htmlString;
        }

        // converts everything between the [k] and [/k] to italic <i>
        private static string ConvertKTag(string text)
        {
            if (text != null)
            {
                var replacedText = text.Replace("[", "<").Replace("]", ">");
                text = replacedText.Replace("<k>", "<i>").Replace("</k>", "</i>");
                return text;
            }
            return null;
        }


        /// <summary>
        /// Returns the rendered email template as a string
        /// </summary>
        /// <param name="templateName"></param>
        /// <param name="placeholders"></param>
        /// <returns></returns>
        public string GetRenderedTemplateText(string templateName, string applicationId, params (string, string)[] placeholders)
        {
            var fileContent = GetTemplateText(templateName, applicationId);
            var renderedText = fileContent;
            if (placeholders == null || !placeholders.Any())
            {
                return renderedText;
            }
            foreach (var placeHolder in placeholders)
            {
                renderedText = renderedText.Replace($"<%{placeHolder.Item1}%>", placeHolder.Item2);
            }
            return renderedText;
        }

        /// <summary>
        /// Returns the raw template html as a string (wwwroot\NewsletterTemplates\<ApplicationId>\<xxx>.html)
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        public string GetTemplateText(string templateName, string applicationId)
        {
            string basePath = Path.Combine(_env.WebRootPath, "NewsletterTemplates", applicationId);
            string targetPath = Path.Combine(basePath, templateName + ".html");
            if (System.IO.File.Exists(targetPath))
            {
                return System.IO.File.ReadAllText(targetPath);
            }
            else
            {
                throw new FileNotFoundException("Datei " + targetPath + " konnte nicht gefunden werden.");
            }
        }

        private string badgeGenerator(Dictionary<string, string> badgesDictionary, List<JToken> plantTags)
        {
            string builder = "";

            foreach (var tag in plantTags)
            {

                string badgeId = tag.SelectToken("Id").Value<string>();
                string badge = badgesDictionary.ContainsKey(badgeId) ? badgesDictionary[badgeId] : null;

                if (badge != null)
                {
                    string imageUrl = GardifyNewsletterUrl + "images/BadgeIcons/" + badge + ".png";
                    builder = builder + "<td width=\"30\" style=\"margin-right:10px; \"><img width=\"30\" height =\"30\" src =\"" + imageUrl + "\"  style =\"float:left; margin-right:10px; display:block; width: 30px; height: 30px; \"></img></td>";
                }
            }

            return "<table cellspacing=\"0\" cellpadding =\"0\" border =\"0\" > " + builder + "</table>";
        }

        private static string colorsGenerator(Dictionary<string, string> colorsDictionary, List<JToken> plantColor)
        {
            string builder = "";
            foreach (string item in plantColor)
            {
                string color = colorsDictionary[item];
                builder = builder + "<td width=\"20\" height=\"20\" style=\"background-color:" + color + ";   border: 1px solid LightGray;  margin-right:10px; width: 20px; height: 20px; \">&nbsp;</td><td width=\"5\">&nbsp;</td>";
            }
            return "<table cellspacing=\"0\" cellpadding =\"0\" border =\"0\" >" + builder + "</table>";
        }

        /*        private static bool TrimText(string text)
                {
                    return text != null && text.Length >= textCharacterLimit;
                }*/

        private static bool ValidateUrl(string moreLink)
        {
            return moreLink != null && !moreLink.StartsWith("http://", StringComparison.OrdinalIgnoreCase) && !moreLink.StartsWith("https://", StringComparison.OrdinalIgnoreCase);
        }

        private static Dictionary<string, string> getBadges()
        {
            Dictionary<string, string> badgesDictionary = new Dictionary<string, string>();
            badgesDictionary.Add("447", "Bienenfreundlich");
            badgesDictionary.Add("320,322", "Vogelfreundlich");
            badgesDictionary.Add("321", "Insektenfreundlich");
            badgesDictionary.Add("445", "Oekologisch_wertvoll");
            badgesDictionary.Add("531", "Schmetterlings_freundlich");
            badgesDictionary.Add("530", "Heimische_Pflanze");
            return badgesDictionary;
        }

        private static Dictionary<string, string> getColors()
        {
            Dictionary<string, string> colorsDictionary = new Dictionary<string, string>();
            colorsDictionary.Add("rot", "red");
            colorsDictionary.Add("weiß", "white");
            colorsDictionary.Add("beige", "rgb(233, 215, 187)");
            colorsDictionary.Add("gelb", "yellow");
            colorsDictionary.Add("gelblich", "#FFFF99");
            colorsDictionary.Add("hellorange", "orange");
            colorsDictionary.Add("orange", "rgb(255, 92, 32)");
            colorsDictionary.Add("oliv", "olive");
            colorsDictionary.Add("blau", "rgb(0, 81, 255)");
            colorsDictionary.Add("petrol", "rgb(20, 96, 126)");
            colorsDictionary.Add("grau", "rgb(172, 172, 172)");
            colorsDictionary.Add("grün", "green");
            colorsDictionary.Add("dunkelgrau", "darkred");
            colorsDictionary.Add("schwarz", "black");
            colorsDictionary.Add("braun", "brown");
            colorsDictionary.Add("pink", "rgb(255, 111, 199)");
            colorsDictionary.Add("apricot", "rgb(245, 149, 86)");
            colorsDictionary.Add("creme", "rgb(231, 225, 204)");
            colorsDictionary.Add("violett", "rgb(189, 125, 241)");
            colorsDictionary.Add("rosa", "rgb(250, 57, 176)");
            colorsDictionary.Add("purpur", "rgb(226, 21, 245)");
            colorsDictionary.Add("hellblau", "rgb(83, 169, 226)");
            colorsDictionary.Add("dunkelrot", "rgb(175, 0, 0)");
            colorsDictionary.Add("blaugrün", "rgb(33, 201, 187)");
            colorsDictionary.Add("gelbgrün", "rgb(166, 190, 25)");
            colorsDictionary.Add("rotbraun", "rgb(119, 18, 18)");
            colorsDictionary.Add("lachsfarben", "rgb(236, 179, 179)");
            colorsDictionary.Add("blauviolett", "rgb(55, 78, 172)");
            colorsDictionary.Add("silbrigweiß", "rgb(204, 204, 204)");
            colorsDictionary.Add("fliederfarben", "#c093c7");
            return colorsDictionary;
        }
    }
}