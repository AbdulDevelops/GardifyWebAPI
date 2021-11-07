using GardifyModels.Models;
using Microsoft.AspNet.Identity;
using PflanzenApp.App_Code;
using PflanzenApp.Services;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using static GardifyModels.Models.AdminAreaViewModels;
using static GardifyModels.Models.ModelEnums;

namespace PflanzenApp.Controllers.AdminArea
{
    public class AdminAreaVorschlagenController : _BaseController
    {
        public readonly int PAGE_SIZE = 100;
        // GET: AdminAreaVorschlagen
        public ActionResult Index(_modalStatusMessageViewModel statusMessage = null, bool notPublished = false, bool published = false, string name = null, string orderBy = null, string filterBy = null, int page = 1)
        {
            //var pc = new PlantController();
            //var plants = pc.DbGetPlantList();

            //var match = plants.Where(p => p.Vorschlagen == true);

            //var plantList = (from p in plants
            //                 where p.Vorschlagen == true
            //                 orderby p.NameGerman
            //                 select p);
            //return View("~/Views/AdminArea/AdminAreaVorschlagen/Index.cshtml", plantList);
            //return View("~/Views/AdminArea/AdminAreaVorschlagen/Index.cshtml");



            PlantController pc = new PlantController();
            PlantCharacteristicController pcc = new PlantCharacteristicController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            PlantViewModels.PlantListViewModel viewModel = new PlantViewModels.PlantListViewModel();
            InternalCommentController icc = new InternalCommentController();
            AdminAreaTodoController tc = new AdminAreaTodoController();
            AlertController ac = new AlertController();

            var plantList = (from p in ctx.Plants
                             where  p.Vorschlagen == true
                             orderby p.NameGerman
                             select p);

            List<PlantViewModels.PlantViewModel> plantVms = new List<PlantViewModels.PlantViewModel>();

            if (plantList.Any())
            {
                foreach (Plant plant in plantList)
                {
                    PlantViewModels.PlantViewModel entryView = new PlantViewModels.PlantViewModel
                    {
                        Id = plant.Id,
                        NameLatin = plant.NameLatin,
                        NameGerman = plant.NameGerman,
                        Description = plant.Description,
                        PlantTags = plant.PlantTags,
                        Published = plant.Published,
                        Family = plant.Familie,
                        Herkunft = plant.Herkunft,
                        Synonym = plant.Synonym,
                        Genehmigt = plant.Genehmigt,
                        CreatedDate = plant.CreatedDate,
                        SuggestionStatusText = plant.SuggestionStatusText,
                        Deleted = plant.Deleted
                    };

                    //                    entryView.GardenCategory = ctx.GardenCategories.FirstOrDefault(c => c.Id == plant.GardenCategoryId);

                    //                  entryView.PlantGroupsOld = ctx.Groups.Where(g => !g.Deleted && g.PlantsWithThisGroupd.Any(p => p.Id == plant.Id));

                    entryView.PlantCharacteristicsOld = pcc.DbGetPlantCharacteristicsByPlantId(plant.Id);

                    HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages(plant.Id);
                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                    {
                        entryView.Images = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
                    }
                    else
                    {
                        entryView.Images.Add(new _HtmlImageViewModel
                        {
                            SrcAttr = Url.Content("~/Images/no-image.png"),
                            Id = 0,
                            TitleAttr = "Kein Bild vorhanden"
                        });
                    }

                    plantVms.Add(entryView);
                }
            }


            viewModel.ListEntries = plantVms.OrderBy(p => p.Genehmigt).ThenBy(p => p.CreatedDate).ToList();

            if (orderBy == null)
            {
                viewModel.ListEntries = plantVms.OrderBy(p => p.Deleted).ThenBy(p => p.Genehmigt).ThenBy(p => p.CreatedDate).ToList();

            }
            else
            {
                switch (orderBy)
                {
                    case "BotanicName":
                        viewModel.ListEntries = plantVms.OrderBy(p => p.Deleted).ThenBy(p => p.NameLatin).ToList();
                        break;
                    case "GermanName":
                        viewModel.ListEntries = plantVms.OrderBy(p => p.Deleted).ThenBy(p => p.NameGerman).ToList();
                        break;
                    case "Status":
                        viewModel.ListEntries = plantVms.OrderBy(p => p.Deleted).ThenBy(p => p.GenehmigtOrder).ThenBy(p => p.CreatedDate).ToList();
                        break;
                    case "Datum":
                        viewModel.ListEntries = plantVms.OrderBy(p => p.Deleted).ThenBy(p => p.CreatedDate).ToList();
                        break;
                    case "BotanicNameR":
                        viewModel.ListEntries = plantVms.OrderBy(p => p.Deleted).ThenByDescending(p => p.NameLatin).ToList();
                        break;
                    case "GermanNameR":
                        viewModel.ListEntries = plantVms.OrderBy(p => p.Deleted).ThenByDescending(p => p.NameGerman).ToList();
                        break;
                    case "StatusR":
                        viewModel.ListEntries = plantVms.OrderBy(p => p.Deleted).ThenByDescending(p => p.GenehmigtOrder).ThenBy(p => p.CreatedDate).ToList();
                        break;
                    case "DatumR":
                        viewModel.ListEntries = plantVms.OrderBy(p => p.Deleted).ThenByDescending(p => p.CreatedDate).ToList();
                        break;
                }
                ViewBag.currentOrder = orderBy;

            }

            var totalPages = Math.Max(1, (int)Math.Ceiling(Decimal.Divide(plantList.Count(), PAGE_SIZE)));

            if (!string.IsNullOrEmpty(name) || !string.IsNullOrEmpty(filterBy))
            {
                totalPages = Math.Max(1, (int)Math.Ceiling(Decimal.Divide(plantVms.Count(), PAGE_SIZE)));
            }


            var firstPage = 1;
            var lastPage = totalPages;
            ViewBag.HasPreviousPage = page > 1;
            ViewBag.HasNextPage = page < totalPages;
            ViewBag.prevPage = Math.Max(page - 1, firstPage);
            ViewBag.nextPage = Math.Min(page + 1, lastPage);
            ViewBag.totalPages = totalPages;
            ViewBag.currentPage = page;
            viewModel.StatusMessage = statusMessage;
            //SaveImagesTags();
            return View("~/Views/AdminArea/AdminAreaVorschlagen/Index.cshtml", viewModel);
        }

        public ActionResult ConfirmDelete(int id)
        {
            return View("~/Views/AdminArea/AdminAreaVorschlagen/ConfirmDelete.cshtml");

        }
        // GET: intern/vorschlagen/Edit/5
        public ActionResult Edit(int? id)
        {
            PlantController pc = new PlantController();
            PlantCharacteristicController pcc = new PlantCharacteristicController();
            PlantCharacteristicCategoryController pccc = new PlantCharacteristicCategoryController();
            PlantTagCategoryController ptcc = new PlantTagCategoryController();
            PlantTagController ptc = new PlantTagController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            AlertController ac = new AlertController();
            InternalCommentController icc = new InternalCommentController();



            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Plant plant = ctx.Plants.FirstOrDefault(p => p.Id == id);
            if (plant == null)
            {
                return HttpNotFound();
            }

            var plantViewModel = GetPlantEditViewModel((int)id);
            ViewBag.alert = null;
            ViewBag.Username = plantViewModel.Plant.CreatedBy;
            ViewBag.Useremail = "";

            var user = ctx.Users.FirstOrDefault(u => u.UserName == plantViewModel.Plant.CreatedBy);
            if(user != null)
            {
                ViewBag.Useremail = user.Email;

            }
            ViewBag.mergeId = pc.DbGetPlantList().Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name  });
            return View("~/Views/AdminArea/AdminAreaVorschlagen/Edit.cshtml", plantViewModel);
        }

        public AdminAreaViewModels.PlantViewModel GetPlantEditViewModel(int id)
        {
            Plant plant = ctx.Plants.FirstOrDefault(p => p.Id == id);
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            AdminAreaViewModels.PlantViewModel plantViewModel = new AdminAreaViewModels.PlantViewModel();
            plantViewModel.Plant = plant;

            HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages((int)id);

            plantViewModel.PlantImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));

            plantViewModel.StatusMessage = TempData["statusMessage"] as _modalStatusMessageViewModel;
            plantViewModel.suggestion = plant.Genehmigt;
            return plantViewModel;
        }

        // POST: intern/vorschlagen/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("saveVorschlagen")]
        public async Task<ActionResult> saveVorschlagen(PlantVorschagenEditViewModel plantToEdit)
        {
            PlantController pc = new PlantController();
            InternalCommentController icc = new InternalCommentController();
            _modalStatusMessageViewModel statusMessage = new _modalStatusMessageViewModel();
            Plant plant = ctx.Plants.FirstOrDefault(p => p.Id == plantToEdit.Id);
            if (plant == null)
            {
                return HttpNotFound();
            }

            if(plantToEdit.author == null || plantToEdit.authorEmail == null)
            {
                var plantCancelModel = GetPlantEditViewModel((int)plantToEdit.Id);
                ViewBag.alert = "Bitte geben Sie den Namen und die E-Mail-Adresse des Autors ein";
                ViewBag.mergeId = pc.DbGetPlantList().Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name });
                return View("~/Views/AdminArea/AdminAreaVorschlagen/Edit.cshtml", plantCancelModel);
            }

            AdminAreaViewModels.PlantViewModel plantViewModel = new AdminAreaViewModels.PlantViewModel();
            plantViewModel.Plant = plant;
            plantViewModel.Plant.Genehmigt = plant.Genehmigt;
            //plantToEdit.EditedBy = User.Identity.GetUserName();
            ctx.SaveChanges();
            TemplateService ts = new TemplateService();
            EmailSender es = new EmailSender(ctx);
            // HelperClasses.DbResponse response = pc.DbEditPlant(plantToEdit);

                if (plantToEdit.decision == "Genehmigen") // Send Email saying approved
            {
        
                var tempPlant = ctx.Plants.FirstOrDefault(p => p.Id == plantToEdit.Id);
                tempPlant.Genehmigt = SuggestionApproved.Approved;

                ctx.SaveChanges();
                try
                {
                    string content = ts.RenderTemplateAsync("SuggestionApproved", new { UserName = plantToEdit.author, Plant = plant.Name + " (" + plant.NameLatin + ")" });

                    var emailSent = await es.SendEmail("Antwort", content, "info@gardify.de", plantToEdit.authorEmail, null);
                }
                catch (DbEntityValidationException e) { throw; };
            } else if (plantToEdit.decision == "Nicht genehmigen")
            {
                var tempPlant = ctx.Plants.FirstOrDefault(p => p.Id == plantToEdit.Id);
                tempPlant.Genehmigt = SuggestionApproved.NotApproved;
                tempPlant.Deleted = true;
                tempPlant.Published = false;


                ctx.SaveChanges();
                try
                {
                    string content = ts.RenderTemplateAsync("SuggestionDeny", new { UserName = plantToEdit.author, message = plantToEdit.message });

                    var emailSent = await es.SendEmail("Antwort", content, "info@gardify.de", plantToEdit.authorEmail, null);
                }
                catch (DbEntityValidationException e) { throw; };
            }
            else if (plantToEdit.decision == "Merge")
            {


                var tempPlant = ctx.Plants.FirstOrDefault(p => p.Id == plantToEdit.Id);


                if(tempPlant.Deleted == true && tempPlant.Genehmigt == SuggestionApproved.NotApproved)
                {
                    tempPlant.Genehmigt = SuggestionApproved.Merged;
                    tempPlant.Published = false;

                    ctx.SaveChanges();

                }
                else
                {
                    tempPlant.Genehmigt = SuggestionApproved.Merged;
                    tempPlant.Published = false;
                    tempPlant.Deleted = true;

                    ctx.SaveChanges();
                    AdminAreaPlantController apc = new AdminAreaPlantController();

                    var res = apc.MergeAllUserPlant(plantToEdit.Id, plantToEdit.mergeId);

                    var mergedPlant = ctx.Plants.FirstOrDefault(p => p.Id == plantToEdit.mergeId);

                    if (!res)
                    {
                        ViewBag.alert = "Es gibt ein fehler";

                        return RedirectToAction("Edit", new { id = plantToEdit.Id });
                    }
                    try
                    {
                        string content = ts.RenderTemplateAsync("SuggestionMerge", new { UserName = plantToEdit.author, message = plantToEdit.message, Plant = plant.Name + " (" + plant.NameLatin + ")", PlantMergeId = plantToEdit.mergeId, PlantMergeName = mergedPlant.Name });

                        var emailSent = await es.SendEmail("Antwort", content, "info@gardify.de", plantToEdit.authorEmail, null);
                    }
                    catch (DbEntityValidationException e) { throw; };
                }
               
            }

            var plantReturnModel = GetPlantEditViewModel((int)plantToEdit.Id);
            ViewBag.success = "der Anlagenstatus wird aktualisiert";
            ViewBag.mergeId = pc.DbGetPlantList().Select(p => new SelectListItem { Value = p.Id.ToString(), Text = p.Name });
            return View("~/Views/AdminArea/AdminAreaVorschlagen/Edit.cshtml", plantReturnModel);
            //return RedirectToAction("Edit", new { id = plantToEdit.Id });
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //[ActionName("mergeSuggestion")]
        //public async Task<ActionResult> mergeSuggestion([Bind(Prefix ="plant")] Plant planttoEdit)
        //{
        //    PlantController pc = new PlantController();
        //    Plant plant = pc.DbGetPlantById(planttoEdit.Id);
        //    if (plant == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    TemplateService ts = new TemplateService();
        //    EmailSender es = new EmailSender(ctx);
        //    ApplicationUser user = await UserManager.FindByIdAsync(plant.CreatedBy);

        //    string content = ts.RenderTemplateAsync("MergePlant", new { UserName = user.FirstName.Contains("Platzhalter") ? user.UserName.Split('@')[0] : user.FirstName, Plant = planttoEdit.NameGerman });

        //    var emailSent = await es.SendEmail("Antwort", content, "info@gardify.de", user.Email, null);

        //}
    }
}