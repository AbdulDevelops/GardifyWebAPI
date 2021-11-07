using GardifyModels.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
{
    public class PlantTagCategoryController : _BaseController
    {
        [NonAction]
        public IEnumerable<PlantTagCategory> DbGetPlantTagCategories(int skip = 0, int take = int.MaxValue)
        {
            return (from c in ctx.PlantTagCategory
                    where !c.Deleted
                    orderby c.Title ascending
                    select c).Skip(skip).Take(take);
        }


        public IEnumerable<PlantTagCategory> DbGetPlantTagCategoriesParents()
        {
            var parents = (from c in ctx.PlantTagCategory
                           where c.ParentId == null
                           && !c.Deleted
                           select c);
            return parents;

        }

        public IEnumerable<PlantTagCategory> DbGetPlantTagCategoriesByParentSubId(IEnumerable<PlantTagCategory> parentCategorySub)
        {
            var childsub = (from c in ctx.PlantTagCategory
                            where c.Id != null
                            select c.Id).ToList();
            foreach (int c in childsub)
            {

            }
            return parentCategorySub;
        }



        [NonAction]
        public IEnumerable<PlantTagCategory> DbGetPlantTagCategoriesByParentId(int parentCategoryId)
        {
            return (from c in ctx.PlantTagCategory
                    where !c.Deleted && c.ParentId == parentCategoryId
                    select c);
        }

        [NonAction]
        public PlantTagCategory DbGetPlantTagCategoryById(int id)
        {
            var ret = (from c in ctx.PlantTagCategory
                       where c.Id == id && !c.Deleted
                       select c);

            if (ret != null && ret.Any())
            {
                return ret.FirstOrDefault();
            }

            return null;
        }

        [NonAction]
        public PlantTagSuperCategory DbGetPlantTagSuperCategoryById(int id)
        {
            var ret = (from c in ctx.PlantTagSuperCategory
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
            return ctx.PlantTagCategory.Where(m => m.Title.ToUpper() == name.ToUpper() && !m.Deleted).FirstOrDefault();
        }
        [NonAction]
        public PlantTagCategory DbGetPlantTagCategoryByColor(string color)
        {
            return ctx.PlantTagCategory.Where(m => m.Title == color && !m.Deleted).FirstOrDefault();
        }

        [NonAction]
        public bool DbCreatePlantTagCategory(PlantTagCategory newCategory)
        {
            newCategory.Title = newCategory.Title.Trim();

            // check duplicates
            var nameCheck_sel = (from cn in ctx.PlantTagCategory
                                 where !cn.Deleted && cn.Title == newCategory.Title
                                 select cn);

            if (nameCheck_sel != null && nameCheck_sel.Any())
            {
                return false;
            }

            newCategory.OnCreate(newCategory.CreatedBy);
            ctx.PlantTagCategory.Add(newCategory);
            return ctx.SaveChanges() > 0 ? true : false;
        }

        public IEnumerable<PlantTagSuperCategory> DbGetSuperCategories()
        {
            return ctx.PlantTagSuperCategory.Where(c => !c.Deleted);
        }

        [NonAction]
        public bool DbCreatePlantTagSuperCategory(PlantTagSuperCategory newCategory)
        {
            newCategory.NameGerman = newCategory.NameGerman.Trim();
            newCategory.NameLatin = newCategory.NameLatin.Trim();
            newCategory.Synonym = newCategory.Synonym.Trim();

            newCategory.OnCreate(newCategory.CreatedBy);
            ctx.PlantTagSuperCategory.Add(newCategory);
            return ctx.SaveChanges() > 0 ? true : false;
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
            var nameCheck_sel = (from ptc in ctx.PlantTagCategory
                                 where !ptc.Deleted && ptc.Title == newData.Title && ptc.Id != newData.Id
                                 select ptc);

            if (nameCheck_sel != null && nameCheck_sel.Any())
            {
                return false;
            }

            var category_sel = (from c in ctx.PlantTagCategory
                                where !c.Deleted && c.Id == newData.Id
                                select c);

            if (category_sel == null || !category_sel.Any())
            {
                return false;
            }

            category_sel.FirstOrDefault().Title = newData.Title;
            category_sel.FirstOrDefault().Color = newData.Color;
            category_sel.FirstOrDefault().OnEdit(newData.EditedBy);

            ctx.Entry(category_sel.FirstOrDefault()).State = EntityState.Modified;
            return ctx.SaveChanges() > 0 ? true : false;
        }

        [NonAction]
        public bool DbEditPlantTagSuperCategory(PlantTagSuperCategory newData)
        {
            newData.NameLatin = newData.NameLatin.Trim();
            newData.NameGerman = newData.NameGerman.Trim();
            newData.Synonym = newData.Synonym.Trim();

            var category_sel = (from c in ctx.PlantTagSuperCategory
                                where !c.Deleted && c.Id == newData.Id
                                select c);

            if (category_sel == null || !category_sel.Any())
            {
                return false;
            }

            category_sel.FirstOrDefault().NameLatin = newData.NameLatin;
            category_sel.FirstOrDefault().NameGerman = newData.NameGerman;
            category_sel.FirstOrDefault().Synonym = newData.Synonym;
            category_sel.FirstOrDefault().Description = newData.Description;
            category_sel.FirstOrDefault().OnEdit(newData.EditedBy);

            ctx.Entry(category_sel.FirstOrDefault()).State = EntityState.Modified;
            return ctx.SaveChanges() > 0 ? true : false;
        }

        [NonAction]
        public bool DbDeletePlantTagSuperCategory(int id, string deletedBy)
        {
            var catCheck_sel = (from cn in ctx.PlantTagSuperCategory
                                where cn.Id == id && !cn.Deleted
                                select cn);

            catCheck_sel.FirstOrDefault().OnEdit(deletedBy);
            catCheck_sel.FirstOrDefault().Deleted = true;

            ctx.Entry(catCheck_sel.FirstOrDefault()).State = EntityState.Modified;
            return ctx.SaveChanges() > 0 ? true : false;
        }

        [NonAction]
        public bool DbDeletePlantTagCategory(int id, string deletedBy)
        {
            var catCheck_sel = (from cn in ctx.PlantTagCategory
                                where cn.Id == id && !cn.Deleted
                                select cn);

            // do not delete category that contains tags
            if (catCheck_sel != null && catCheck_sel.Any() && catCheck_sel.FirstOrDefault().TagsInThisCategory != null && catCheck_sel.FirstOrDefault().TagsInThisCategory.Any())
            {
                return false;
            }

            catCheck_sel.FirstOrDefault().OnEdit(deletedBy);
            catCheck_sel.FirstOrDefault().Deleted = true;

            ctx.Entry(catCheck_sel.FirstOrDefault()).State = EntityState.Modified;
            return ctx.SaveChanges() > 0 ? true : false;
        }
    }
}