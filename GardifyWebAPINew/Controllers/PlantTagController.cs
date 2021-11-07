using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GardifyWebAPI.Controllers
{
    public class PlantTagController : _BaseController
    {


        [NonAction]
        public IEnumerable<PlantTag> DbGetPlantTags()
        {
            return (from tag in plantDB.PlantTags
                    where !tag.Deleted
                    orderby tag.Title
                    select tag);
        }

        [NonAction]
        public IEnumerable<PlantTagSearchLite> DbGetPlantTagsSearchLite()
        {
            IEnumerable<PlantTagSearchLite> tags = plantDB.PlantTags.Where(t => !t.Deleted).Select(t => new PlantTagSearchLite { Id = t.Id, CategoryId = t.CategoryId, Title = t.Title});
            return tags;
        }
        
       
        [NonAction]
        public PlantTag DbGetPlantTagById(int id)
        {
            var tag_sel = (from tag in plantDB.PlantTags
                           where tag.Id == id && !tag.Deleted
                           select tag);
            if (tag_sel != null && tag_sel.Any())
            {
                return tag_sel.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        [NonAction]
        public PlantTag DbGetPlantTagByName(string name)
        {
            return plantDB.PlantTags.Where(m => m.Title.ToUpper() == name.ToUpper()).FirstOrDefault();
        }

        [NonAction]
        public bool DbCreatePlantTag(PlantTag plantTagData)
        {
            plantTagData.Title = plantTagData.Title.Trim();

            // check Category
            var catCheck_sel = (from c in plantDB.PlantTagCategory
                                where c.Id == plantTagData.CategoryId && !c.Deleted
                                select c);

            if (catCheck_sel == null || !catCheck_sel.Any())
            {
                return false;
            }

            // check duplicated tags
            var nameCheck_sel = (from n in plantDB.PlantTags
                                 where !n.Deleted && n.CategoryId == plantTagData.CategoryId && n.Title == plantTagData.Title
                                 select n);

            if (nameCheck_sel != null && nameCheck_sel.Any())
            {
                return false;
            }

            plantTagData.OnCreate(plantTagData.CreatedBy);
            plantDB.PlantTags.Add(plantTagData);
            return plantDB.SaveChanges() > 0 ? true : false;
        }

        [NonAction]
        public bool DbEditPlantTag(PlantTag plantTagData)
        {
            plantTagData.Title = plantTagData.Title.Trim();

            // check Category
            var catCheck_sel = (from c in plantDB.PlantTagCategory
                                where c.Id == plantTagData.CategoryId && !c.Deleted
                                select c);

            if (catCheck_sel == null || !catCheck_sel.Any())
            {
                return false;
            }

            // check duplicated tags
            var nameCheck_sel = (from n in plantDB.PlantTags
                                 where n.CategoryId == plantTagData.CategoryId && n.Title == plantTagData.Title && n.Id != plantTagData.Id && !n.Deleted
                                 select n);

            if (nameCheck_sel != null && nameCheck_sel.Any())
            {
                return false;
            }

            // check if tag exists
            var tag_sel = (from t in plantDB.PlantTags
                           where !t.Deleted && t.Id == plantTagData.Id
                           select t);

            if (tag_sel == null || !tag_sel.Any())
            {
                return false;
            }

            tag_sel.FirstOrDefault().Title = plantTagData.Title;
            if (tag_sel.FirstOrDefault().TagImage != null)
            {
                tag_sel.FirstOrDefault().TagImage = plantTagData.TagImage.Trim();
            }
            tag_sel.FirstOrDefault().CategoryId = plantTagData.CategoryId;
            tag_sel.FirstOrDefault().EditedDate = DateTime.Now;

            plantDB.Entry(tag_sel.FirstOrDefault()).State = EntityState.Modified;
            return plantDB.SaveChanges() > 0 ? true : false;
        }

        [NonAction]
        public bool DbDeletePlantTag(int id, string deletedBy)
        {
            var tag_sel = (from t in plantDB.PlantTags
                           where t.Id == id && !t.Deleted
                           select t);

            tag_sel.FirstOrDefault().OnEdit(deletedBy);
            tag_sel.FirstOrDefault().Deleted = true;
            plantDB.Entry(tag_sel.FirstOrDefault()).State = EntityState.Modified;
            return plantDB.SaveChanges() > 0 ? true : false;
        }

        [NonAction]
        public bool DbAddTagToPlant(int plantId, int tagId, bool saveChanges = true)
        {
            var checkPlant_sel = (from p in plantDB.Plants
                                  where p.Id == plantId && !p.Deleted
                                  select p);

            // if plant not exists or plant tag already in plant tags list return false
            if (checkPlant_sel == null || !checkPlant_sel.Any())
            {
                return false;
            }

            if (checkPlant_sel.FirstOrDefault().PlantTags != null &&
                (checkPlant_sel.FirstOrDefault().PlantTags.Where(t => t.Id == tagId) != null &&
                checkPlant_sel.FirstOrDefault().PlantTags.Where(t => t.Id == tagId).Any()))
            {
                return false;
            }

            // if tag not exits return false
            var checkPlantTag_sel = (from t in plantDB.PlantTags
                                     where t.Id == tagId && !t.Deleted
                                     select t);

            if (checkPlantTag_sel == null || !checkPlantTag_sel.Any())
            {
                return false;
            }

            checkPlant_sel.FirstOrDefault().PlantTags.Add(checkPlantTag_sel.FirstOrDefault());
            if(saveChanges == true)
            {
                return plantDB.SaveChanges() > 0 ? true : false;
            }
            return false;
        }

        [NonAction]
        public bool DbRemoveTagFromPlant(int plantId, int tagId, bool saveChanges = true)
        {
            var checkPlant_sel = (from p in plantDB.Plants
                                  where p.Id == plantId && !p.Deleted
                                  select p);

            // if plant not exists or plant tag is not in plant tags list return false
            if (checkPlant_sel == null || !checkPlant_sel.Any())
            {
                return false;
            }
            if (checkPlant_sel.FirstOrDefault().PlantTags != null &&
                (checkPlant_sel.FirstOrDefault().PlantTags.Where(t => t.Id == tagId) == null ||
                !checkPlant_sel.FirstOrDefault().PlantTags.Where(t => t.Id == tagId).Any()))
            {
                return false;
            }

            checkPlant_sel.FirstOrDefault().PlantTags.Remove(checkPlant_sel.FirstOrDefault().PlantTags.Where(t => t.Id == tagId).FirstOrDefault());
            if (saveChanges)
            {
                return plantDB.SaveChanges() > 0 ? true : false;
            }
            return false;
        }

        [NonAction]
        public IEnumerable<PlantTag> DBGetPlantTagsByPlantId(int plantId)
        {
            PlantController pc = new PlantController();
            ICollection<PlantTag> tags = null;
            var plants = pc.DbGetPlantById(plantId);
            if(plants!=null)
            {
                tags = plants.PlantTags;
                
            }
            return tags;
        }

        [NonAction]
        public IEnumerable<PlantTag> DbGetPlantTagsByCategoryId(int categoryId)
        {
            return plantDB.PlantTags.Where(m => m.CategoryId == categoryId && !m.Deleted);
        }
    }
}