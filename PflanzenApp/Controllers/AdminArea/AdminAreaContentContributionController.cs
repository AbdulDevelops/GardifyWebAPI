using PflanzenApp.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace PflanzenApp.Controllers.AdminArea
{
    public class AdminAreaContentContributionController : _BaseController
    {
        // GET: AdminAreaContentContribution
        public ActionResult Index()
        {
            return View("~/Views/AdminArea/AdminAreaContentContribution/Index.cshtml", GetSynonymIndexViewModel());
        }

        public ActionResult Create()
        {
            SynonymViewModels.SynonymCreateViewModel vm = new SynonymViewModels.SynonymCreateViewModel();
            vm.InfoObjects = DbGetReferencedItems();
            return View("~/Views/AdminArea/AdminAreaContentContribution/Create.cshtml", vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Text,Date,ReferenceType,ReferenceId")] SynonymViewModels.SynonymCreateViewModel createView)
        {
            PropertyController proc = new PropertyController();
            if (ModelState.IsValid)
            {
                DbAddSynonym(createView);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToAction("Error.cshtml");
            }
        }

        public ActionResult Edit(int id)
        {
            return View("~/Views/AdminArea/AdminAreaContentContribution/Edit.cshtml", GetSynonymEditViewModel(id));
        }

        [HttpPost]
        public ActionResult Edit(SynonymViewModels.SynonymEditViewModel editView)
        {
            if (ModelState.IsValid)
            {
                DbEditSynonym(editView);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToError("Das übergebene Model ist nicht korrekt", HttpStatusCode.InternalServerError, "AdminAreaContentContributionController.Edit("+editView.Text+")"); 
            }
        }

        public ActionResult Delete(int id)
        {
            return View("~/Views/AdminArea/AdminAreaContentContribution/Delete.cshtml", GetSynonymDeleteViewModel(id));
        }
        [HttpPost]
        public ActionResult Delete(SynonymViewModels.SynonymDeleteViewModel dvm)
        {
            if (ModelState.IsValid)
            {
                DbDeleteSynonym(dvm);
                return RedirectToAction("Index");
            }
            else
            {
                return RedirectToError("Das übergebene Model ist nicht korrekt", HttpStatusCode.InternalServerError, "AdminAreaContentContributionController.Delete("+dvm.Text+")");
            }
        }

        #region DB

        [NonAction]
        public IEnumerable<Plant> GetPlantsAssociatedWithSynonym(string searchText)
        {
            var synonyms = (from syn in ctx.Synonym
                            where syn.Text.Contains(searchText)
                            && !syn.Deleted
                            && syn.ReferenceType == ModelEnums.ReferenceToModelClass.Plant
                            select syn.ReferenceId).ToList();
            PlantController pc = new PlantController();
            List<Plant> plants = new List<Plant>();
            foreach (int id in synonyms)
            {
                plants.Add(pc.DbGetPlantById(id));
            }
            return plants;
        }

        [NonAction]
        public SynonymViewModels.SynonymDeleteViewModel GetSynonymDeleteViewModel(int id)
        {
            Synonym syn = DbGetSynonym(id);
            SynonymViewModels.SynonymDeleteViewModel dvm = new SynonymViewModels.SynonymDeleteViewModel()
            {
                Id = syn.Id,
                Text = syn.Text
            };
            return dvm;
        }
        [NonAction]
        public void DbDeleteSynonym(SynonymViewModels.SynonymDeleteViewModel dvm)
        {
            var dbSynonym = (from syn in ctx.Synonym
                             where syn.Id == dvm.Id && !syn.Deleted
                             select syn).FirstOrDefault();
            dbSynonym.Deleted = true;
            ctx.SaveChanges();
        }
        [NonAction]
        public void DbEditSynonym(SynonymViewModels.SynonymEditViewModel evm)
        {
            var dbSynonym = (from syn in ctx.Synonym
                             where syn.Id == evm.Id && !syn.Deleted
                             select syn).FirstOrDefault();
            if(evm.ReferenceType == ModelEnums.ReferenceToModelClass.NotSet)
            {
                dbSynonym.ReferenceId = 0;
            }
            else
            {
                dbSynonym.ReferenceId = evm.ReferenceId;
            }
            dbSynonym.ReferenceType = evm.ReferenceType;
            dbSynonym.Text = evm.Text;
            ctx.SaveChanges();
        }

        [NonAction]
        public void DbAddSynonym(SynonymViewModels.SynonymCreateViewModel createView)
        {
            Synonym syn = new Synonym()
            {
                ReferenceId = createView.ReferenceId,
                ReferenceType = createView.ReferenceType,
                Text = createView.Text
            };
            syn.OnCreate(Utilities.GetUserName());
            ctx.Synonym.Add(syn);
            ctx.SaveChanges();
        }

        [NonAction]
        public IEnumerable<Synonym> DbGetSynonyms()
        {
            var synonyms = (from ctx in ctx.Synonym
                            where ctx.Deleted == false
                            select ctx);
            return synonyms;
        }


        [NonAction]
        public SynonymViewModels.SynonymIndexViewModel GetSynonymIndexViewModel()
        {
            List<SynonymViewModels.SynonymDetailsViewModel> list = new List<SynonymViewModels.SynonymDetailsViewModel>();
            var currentSynonyms = DbGetSynonyms();
            foreach (var syn in currentSynonyms)
            {
                var item = GetSynonymDetailViewModel(syn.Id);
                list.Add(item);
            }
            return new SynonymViewModels.SynonymIndexViewModel { SynonymList = list };
        }
        [NonAction]
        public SynonymViewModels.SynonymEditViewModel GetSynonymEditViewModel(int id)
        {
            var synonym = DbGetSynonym(id);
            SynonymViewModels.SynonymEditViewModel em = new SynonymViewModels.SynonymEditViewModel
            {
                Id = synonym.Id,
                InfoObjects = DbGetReferencedItems(),
                Text = synonym.Text,
                ReferenceType = synonym.ReferenceType,
                ReferenceId = synonym.ReferenceId
            };
            return em;
        }

        [NonAction]
        public SynonymViewModels.SynonymDetailsViewModel GetSynonymDetailViewModel(int id)
        {
            var synonym = DbGetSynonym(id);
            SynonymViewModels.SynonymDetailsViewModel vm = new SynonymViewModels.SynonymDetailsViewModel
            {
                Id = synonym.Id,
                ReferenceName = DbGetReferenceText(synonym.ReferenceType, synonym.ReferenceId),
                Text = synonym.Text,
                ReferenceType = synonym.ReferenceType
            };
            return vm;
        }

        [NonAction]
        public string DbGetReferenceText(ModelEnums.ReferenceToModelClass referenceType, int referenceId)
        {
            PlantController pc = new PlantController();
            switch (referenceType)
            {

                case ModelEnums.ReferenceToModelClass.Plant:
                    return pc.DbGetPlantName(referenceId);
                //TODO: Tools implementieren
                default:
                    return "-";
            }
        }
        public Synonym DbGetSynonym(int id)
        {
            var synonym = (from ctx in ctx.Synonym
                           where ctx.Id == id
                           select ctx).FirstOrDefault();
            return synonym;

        }



        [NonAction]
        public IEnumerable<IReferencedObject> DbGetReferencedItems(int referenceId)
        {
            ModelEnums.ReferenceToModelClass enumValue = (ModelEnums.ReferenceToModelClass)referenceId;
            IEnumerable<IReferencedObject> items;
            if (enumValue == ModelEnums.ReferenceToModelClass.UserPlant)
            {
                UserPlantController uc = new UserPlantController();
                var plants = uc.DbGetUserPlantsByUserId(Utilities.GetUserId());
                foreach (var pl in plants)
                {
                    if (pl.Count > 1)
                    {
                        pl.Name = "(" + pl.Count + ") " + pl.Name;
                    }
                }
                items = plants;
            }
            else if (enumValue == ModelEnums.ReferenceToModelClass.Garden)
            {
                GardenController gc = new GardenController();
                var gards = gc.DbGetGardensByUserId(Utilities.GetUserId());
                items = gards;
            }
            else if (enumValue == ModelEnums.ReferenceToModelClass.UserTool)
            {
                throw new NotImplementedException();
            }
            else if(enumValue == ModelEnums.ReferenceToModelClass.NotSet)
            {
                return new List<IReferencedObject>();
            }
            else
            {
                throw new NotImplementedException();
            }
            return items;
        }

        [NonAction]
        public List<IReferencedObject> DbGetReferencedItems()
        {
            List<IReferencedObject> items = new List<IReferencedObject>();
            PlantController pc = new PlantController();
            //TODO: Implement Tool stuff
            var plants = pc.DbGetPlantList().OrderBy(p => p.NameLatin).ToList();
            items.AddRange(plants);
            return items;
        }
        #endregion
    }
}