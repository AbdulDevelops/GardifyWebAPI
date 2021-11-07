using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace GardifyWebAPI.Controllers
{
    public class PlantTagCategoryController : _BaseController
    {
        [NonAction]
        public IEnumerable<PlantTagCategory> DbGetPlantTagCategories(int skip = 0, int take = int.MaxValue)
        {
            return (from c in plantDB.PlantTagCategory
                    where !c.Deleted
                    orderby c.Title ascending
                    select c).Skip(skip).Take(take);
        }

        [NonAction]
        public IEnumerable<PlantTagCategory> DbGetPlantTagCategoriesLite()
        {
            IEnumerable<PlantTagCategory> cats = (from c in plantDB.PlantTagCategory
                                                    where !c.Deleted
                                                    orderby c.Title ascending
                                                    select c);
            cats = cats.Select(g => new PlantTagCategory { Id = g.Id, Title = g.Title });
            return cats;
        }


        public IEnumerable<PlantTagCategory> DbGetPlantTagCategoriesParents()
        {
            var parents = (from c in plantDB.PlantTagCategory
                           where c.ParentId == null
                           && !c.Deleted
                           select c);
            return parents;

        }

        public IEnumerable<PlantTagCategory> DbGetPlantTagCategoriesByParentSubId(IEnumerable<PlantTagCategory> parentCategorySub)
        {
            var childsub = (from c in plantDB.PlantTagCategory
                            select c.Id).ToList();
            foreach (int c in childsub)
            {

            }
            return parentCategorySub;
        }



        [NonAction]
        public IEnumerable<PlantTagCategory> DbGetPlantTagCategoriesByParentId(int parentCategoryId)
        {
            return (from c in plantDB.PlantTagCategory
                    where !c.Deleted && c.ParentId == parentCategoryId
                    select c);
        }

        [NonAction]
        public IEnumerable<PlantTagCategory> DbGetPlantTagCategories()
        {
            return plantDB.PlantTagCategory.Where(p => !p.Deleted);
        }

        [NonAction]
        public PlantTagCategory DbGetPlantTagCategoryById(int id)
        {
            var ret = (from c in plantDB.PlantTagCategory
                       where c.Id == id && !c.Deleted
                       select c);

            if (ret != null && ret.Any())
            {
                return ret.FirstOrDefault();
            }

            return null;
        }

        [NonAction]
        public PlantTagCategory DbGetPlantTagCategoryByName(string name)
        {
            return plantDB.PlantTagCategory.Where(m => m.Title.ToUpper() == name.ToUpper() && !m.Deleted).FirstOrDefault();
        }
        [NonAction]
        public PlantTagCategory DbGetPlantTagCategoryByColor(string color)
        {
            return plantDB.PlantTagCategory.Where(m => m.Title == color && !m.Deleted).FirstOrDefault();
        }

        [NonAction]
        public bool DbCreatePlantTagCategory(PlantTagCategory newCategory)
        {
            newCategory.Title = newCategory.Title.Trim();

            // check duplicates
            var nameCheck_sel = (from cn in plantDB.PlantTagCategory
                                 where !cn.Deleted && cn.Title == newCategory.Title
                                 select cn);

            if (nameCheck_sel != null && nameCheck_sel.Any())
            {
                return false;
            }

            newCategory.OnCreate(newCategory.CreatedBy);
            plantDB.PlantTagCategory.Add(newCategory);
            return plantDB.SaveChanges() > 0 ? true : false;
        }

        [NonAction]
        public bool DbEditPlantTagCategory(PlantTagCategory newData)
        {
            if (string.IsNullOrEmpty(newData.Color))
            {
                newData.Color = "#b7bff7";
            }
            newData.Title = newData.Title.Trim();

            // check duplicates
            var nameCheck_sel = (from ptc in plantDB.PlantTagCategory
                                 where !ptc.Deleted && ptc.Title == newData.Title && ptc.Id != newData.Id
                                 select ptc);

            if (nameCheck_sel != null && nameCheck_sel.Any())
            {
                return false;
            }

            var category_sel = (from c in plantDB.PlantTagCategory
                                where !c.Deleted && c.Id == newData.Id
                                select c);

            if (category_sel == null || !category_sel.Any())
            {
                return false;
            }

            category_sel.FirstOrDefault().Title = newData.Title;
            category_sel.FirstOrDefault().Color = newData.Color;
            category_sel.FirstOrDefault().OnEdit(newData.EditedBy);

            plantDB.Entry(category_sel.FirstOrDefault()).State = EntityState.Modified;
            return plantDB.SaveChanges() > 0 ? true : false;
        }

        [NonAction]
        public bool DbDeletePlantTagCategory(int id, string deletedBy)
        {
            var catCheck_sel = (from cn in plantDB.PlantTagCategory
                                where cn.Id == id && !cn.Deleted
                                select cn);

            // do not delete category that contains tags
            if (catCheck_sel != null && catCheck_sel.Any() && catCheck_sel.FirstOrDefault().TagsInThisCategory != null && catCheck_sel.FirstOrDefault().TagsInThisCategory.Any())
            {
                return false;
            }

            catCheck_sel.FirstOrDefault().OnEdit(deletedBy);
            catCheck_sel.FirstOrDefault().Deleted = true;

            plantDB.Entry(catCheck_sel.FirstOrDefault()).State = EntityState.Modified;
            return plantDB.SaveChanges() > 0 ? true : false;
        }
    }
}