using PflanzenApp.App_Code;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
{
    public class PlantController : _BaseController
    {
        [NonAction]
        public string DbGetPlantName(int plantId, bool allowUnpublished = false)
        {
            if (allowUnpublished)
            {
                var name = (from pln in ctx.Plants
                            where pln.Id == plantId && !pln.Deleted
                            select pln.NameGerman).FirstOrDefault();
                return name;
            }
            else
            {
                var name = (from pln in ctx.Plants
                            where pln.Id == plantId && !pln.Deleted && pln.Published
                            select pln.NameGerman).FirstOrDefault();
                return name;
            }
        }

        [NonAction]
        public IEnumerable<Plant> DbGetPublishedPlantList(int skip = 0, int take = int.MaxValue, ModelEnums.OrderBy sortBy = ModelEnums.OrderBy.nameGerman)
        {
            var plants = DbGetPlantList();
            return plants.Where(m => m.Published);
        }

        [NonAction]
        public IEnumerable<Plant> DbGetNotPublishedPlantList()
        {
            return ctx.Plants.Where(m => !m.Deleted && !m.Published);
        }

        [NonAction]
        public IEnumerable<Plant> DbGetPublishedPlantList()
        {
            return ctx.Plants.Where(m => !m.Deleted && m.Published);
        }

        [NonAction]
        public IEnumerable<Plant> DbGetPlantList(int skip = 0, int take = int.MaxValue, ModelEnums.OrderBy sortBy = ModelEnums.OrderBy.nameGerman)
        {
            if (take > 0)
            {
                var plantList = (from p in ctx.Plants
                                 where !p.Deleted
                                 select p);

                switch (sortBy)
                {
                    case ModelEnums.OrderBy.nameGerman:
                        plantList = plantList.OrderBy(p => p.NameGerman);
                        break;

                    case ModelEnums.OrderBy.NameGermanDesc:
                        plantList = plantList.OrderByDescending(p => p.NameGerman);
                        break;

                    case ModelEnums.OrderBy.NameLatin:
                        plantList = plantList.OrderBy(p => p.NameLatin);
                        break;

                    case ModelEnums.OrderBy.NameLatinDesc:
                        plantList = plantList.OrderByDescending(p => p.NameLatin);
                        break;

                    case ModelEnums.OrderBy.CreatedDate:
                        plantList = plantList.OrderBy(p => p.CreatedDate);
                        break;

                    case ModelEnums.OrderBy.CreatedDateDesc:
                        plantList = plantList.OrderByDescending(p => p.CreatedDate);
                        break;
                }
                return plantList.Skip(skip).Take(take);
            }
            else
            {
                return null;
            }
        }

        [NonAction]
        public IEnumerable<Plant> DbGetPlantsByText(string query)
        {
            if (!String.IsNullOrEmpty(query))
            {
                return (from plant in ctx.Plants
                        where !plant.Deleted && plant.Published &&  (plant.NameLatin.Contains(query) || plant.NameGerman.Contains(query))
                        select plant);
            }
            else
            {
                return null;
            }
        }

        [NonAction]
        public HelperClasses.DbResponse DbCreatePlant(Plant newPlant)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            // check duplicates
            var plant_check = (from p in ctx.Plants
                               where !p.Deleted && p.NameLatin == newPlant.NameLatin && p.NameGerman == newPlant.NameGerman
                               select p);

            if (plant_check != null && plant_check.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.DuplicatedEntry);
                response.Status = ModelEnums.ActionStatus.Error;
                return response;
            }
            newPlant.OnCreate(newPlant.CreatedBy);

            newPlant.PublishedDate = DateTime.Now;
            ctx.Plants.Add(newPlant);
            bool isOk;
            isOk = ctx.SaveChanges() > 0 ? true : false;
            if (isOk)
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Created);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(newPlant);
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                response.Status = ModelEnums.ActionStatus.Error;
            }
            return response;
        }

        [NonAction]
        public HelperClasses.DbResponse DbEditPlant(Plant newData)
        {
            HelperClasses.DbResponse dbResponse = new HelperClasses.DbResponse();

            newData.NameLatin = newData.NameLatin.Trim();
            if(newData.NameGerman != null)
            {
                newData.NameGerman = newData.NameGerman.Trim();
            }
            // check duplicates
            var plant_check = (from p in ctx.Plants
                               where !p.Deleted && p.Published == newData.Published && p.NameLatin == newData.NameLatin && p.NameGerman == newData.NameGerman && p.Id != newData.Id
                               select p);

            if (plant_check != null && plant_check.Any())
            {
                dbResponse.Messages.Add(ModelEnums.DatabaseMessage.DuplicatedEntry);
                dbResponse.Status = ModelEnums.ActionStatus.Error;
                return dbResponse;
            }

            //load ctx entry
            var plant_sel = (from p in ctx.Plants
                             where p.Id == newData.Id && !p.Deleted
                             select p);

            if (plant_sel == null || !plant_sel.Any())
            {
                dbResponse.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                dbResponse.Status = ModelEnums.ActionStatus.Error;
                return dbResponse;
            }

            Plant plantToEdit = plant_sel.FirstOrDefault();

            if (!plantToEdit.Published && newData.Published)
            {
                plantToEdit.PublishedDate = DateTime.Now;
            }

            plantToEdit.NameLatin = newData.NameLatin;
            plantToEdit.NameGerman = newData.NameGerman;
            plantToEdit.Description = newData.Description.Trim();
            plantToEdit.Herkunft = newData.Herkunft;
            plantToEdit.Published = newData.Published;
            plantToEdit.GardenCategoryId = newData.GardenCategoryId;
            plantToEdit.OnEdit(newData.EditedBy);
            plantToEdit.Synonym = newData.Synonym;
            plantToEdit.Familie = newData.Familie;
            plantToEdit.Vorschlagen = newData.Vorschlagen;
            plantToEdit.Genehmigt = newData.Genehmigt;

            ctx.Entry(plantToEdit).State = EntityState.Modified;

            var taxomTree = ctx.TaxonomicTree.Where(t => t.PlantId == plantToEdit.Id).ToList();

            if (taxomTree != null && taxomTree.Any())
            {
                foreach(var taxom in taxomTree)
                {
                    taxom.TitleGerman = newData.NameGerman;
                    taxom.TitleLatin = newData.NameLatin;

                    ctx.Entry(taxom).State = EntityState.Modified;

                }
            }


            bool isOk = ctx.SaveChanges() > 0 ? true : false;

            if (isOk)
            {
                dbResponse.Messages.Add(ModelEnums.DatabaseMessage.Edited);
                dbResponse.Status = ModelEnums.ActionStatus.Success;
                dbResponse.ResponseObjects.Add(plantToEdit);
            }
            else
            {
                dbResponse.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                dbResponse.Status = ModelEnums.ActionStatus.Error;
            }

            return dbResponse;
        }

        [NonAction]
        public HelperClasses.DbResponse DbDeletePlant(int plantId, string deletedBy)
        {
            HelperClasses.DbResponse response = new HelperClasses.DbResponse();

            //load ctx entry
            var plant_sel = (from p in ctx.Plants
                             where p.Id == plantId && !p.Deleted
                             select p);

            if (plant_sel == null || !plant_sel.Any())
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ObjectNotFound);
                response.Status = ModelEnums.ActionStatus.Error;
                return response;
            }

            Plant toDelete = plant_sel.FirstOrDefault();
            toDelete.Deleted = true;
            toDelete.OnEdit(deletedBy);

            /******************************Delete Entrys for TaxonomicTree******************************/
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            TaxonomicTree tree = ttc.GetTreeNodeByPlantId(plantId);
            if (tree != null)
            {
                ttc.DbDeleteTaxonomicTreeNode(tree.Id, "Deleted by referencing plant");
            }

            /******************************Delete Entrys for Characteristics*****************************/
            PlantCharacteristicController pcc = new PlantCharacteristicController();
            var characteristics = pcc.DbGetPlantCharacteristicsByPlantId(plantId).ToList();
            foreach (var c in characteristics)
            {
                pcc.DbDeletePlantCharacteristic(c.Id, "Deleted by referencing plant");
            }

            /*******************************Delete Entrys for Tags***************************************/
            PlantTagController ptc = new PlantTagController();
            var tags = ptc.DBGetPlantTagsByPlantId(plantId).ToList();
            foreach (var t in tags)
            {
                ptc.DbRemoveTagFromPlant(plantId, t.Id);
            }


            ctx.Entry(toDelete).State = EntityState.Modified;
            bool isOk = ctx.SaveChanges() > 0 ? true : false;

            if (isOk)
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.Deleted);
                response.Status = ModelEnums.ActionStatus.Success;
                response.ResponseObjects.Add(plant_sel.FirstOrDefault());
            }
            else
            {
                response.Messages.Add(ModelEnums.DatabaseMessage.ErrorOnSaveChanges);
                response.Status = ModelEnums.ActionStatus.Error;
            }

            return response;
        }

        [NonAction]
        public Plant DbGetPlantPublishedById(int id)
        {
            Plant ret = null;
            if (id > 0)
            {
                var plant_sel = (from plant in ctx.Plants
                                 where !plant.Deleted && plant.Id == id && plant.Published
                                 select plant).Include(p => p.PlantTags);

                if (plant_sel != null && plant_sel.Any())
                {
                    ret = plant_sel.FirstOrDefault();
                }
            }
            return ret;
        }

        [NonAction]
        public Plant DbGetPlantById(int id)
        {
            Plant ret = null;
            if (id > 0)
            {
                var plant_sel = (from plant in ctx.Plants
                                 where !plant.Deleted && plant.Id == id
                                 select plant).Include(p => p.PlantTags);

                if (plant_sel != null && plant_sel.Any())
                {
                    ret = plant_sel.FirstOrDefault();
                }
            }
            return ret;
        }

        [NonAction]
        public List<Plant> DbGetPlantsByPositiveTagsList(IEnumerable<PlantTag> positiveList)
        {
            var allPlants_sel = (from p in ctx.Plants
                                 where !p.Deleted && p.Published
                                 select p);
            List<Plant> ret = Utilities.sortOutPlantsByPositiveTagList(allPlants_sel, positiveList);
            return ret;
        }

        [NonAction]
        public List<Plant> DbGetChildrenTaxonPlantsByParentTaxonId(int taxonId)
        {
            List<Plant> ret = new List<Plant>();
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            TaxonomicTree taxon = ttc.DbGetTreeNodeById(taxonId, true);
            if (taxon == null)
            {
                return null;
            }

            if (taxon.Type == ModelEnums.NodeType.Leaf && taxon.PlantId > 0)
            {
                Plant tmp = DbGetPlantPublishedById((int)taxon.PlantId);
                if (tmp != null)
                {
                    ret.Add(tmp);
                }
                return ret;
            }
            else
            {
                if (taxon.Childs != null)
                {
                    foreach (TaxonomicTree child in taxon.Childs)
                    {
                        List<Plant> tmp = DbGetChildrenTaxonPlantsByParentTaxonId(child.Id);
                        if (tmp != null && tmp.Any())
                        {
                            ret.AddRange(tmp);
                        }

                    }
                }
            }
            return ret;
        }


        public IEnumerable<Plant> DbGetSiblings(int plantId)
        {
            var plant = DbGetPlantById(plantId);
            var taxElement = ctx.TaxonomicTree.Where(t => t.PlantId == plant.Id).FirstOrDefault();
            if(taxElement != null)
            {
                var siblings = ctx.TaxonomicTree.Where(t => t.ParentId == taxElement.ParentId && t.PlantId != 0);
                if(siblings != null && !siblings.Any())
                {
                    var plants = ctx.Plants.Where(p => siblings.Any(s => s.PlantId == p.Id));
                    return plants;
                }
            }
            return null;
        }

    }
}