using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
{
    public class PlantCharacteristicCategoryController : _BaseController
    {
        [NonAction]
        public bool DbCreatePlantCharacteristicCategory(PlantCharacteristicCategory newCategoryData)
        {
            newCategoryData.Title = newCategoryData.Title.Trim();
            string unitRangeResult = GetUnitRangeResult(newCategoryData.Unit, newCategoryData.CharacteristicValueType);
            newCategoryData.Title = newCategoryData.Title + unitRangeResult;
            // check duplicated entries
            var dupe_check = (from c in ctx.PlantCharacteristicCategory
                              where !c.Deleted && c.Title == newCategoryData.Title
                              select c);

            if (dupe_check != null && dupe_check.Any())
            {
                return false;
            }

            newCategoryData.OnCreate(newCategoryData.CreatedBy);
            ctx.PlantCharacteristicCategory.Add(newCategoryData);
            return ctx.SaveChanges() > 0 ? true : false;
        }

        /// <summary>
        /// TODO: When there are new CharacteristicValueType add them here aswell
        /// </summary>
        /// <param name="unit"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public string GetUnitRangeResult(string unit, GardifyModels.Models.ModelEnums.CharacteristicValueType type)
        {
            if (type == ModelEnums.CharacteristicValueType.DoNotUse)
            {
                return "";
            }
            else if (type == ModelEnums.CharacteristicValueType.LatinNumberRange || type == ModelEnums.CharacteristicValueType.MonthRange || type == ModelEnums.CharacteristicValueType.NumberRange)
            {
                return " (Von " + unit + " bis " + unit + ")";
            }
            else if (type == ModelEnums.CharacteristicValueType.SingleLatinNumber || type == ModelEnums.CharacteristicValueType.SingleMonth || type == ModelEnums.CharacteristicValueType.SingleNumber)
            {
                return " (in " + unit + ")";
            }
            else return "";
        }

        [NonAction]
        public IEnumerable<PlantCharacteristicCategory> DbGetPlantCharacteristicCategories(int skip = 0, int take = int.MaxValue)
        {
            var cat_sel = (from pc in ctx.PlantCharacteristicCategory
                           where !pc.Deleted
                           orderby pc.Title
                           select pc).Skip(skip).Take(take);

            if (cat_sel != null && cat_sel.Any())
            {
                foreach (PlantCharacteristicCategory category in cat_sel)
                {
                    category.Count = ctx.PlantCharacteristic.Count(t => t.CategoryId == category.Id && !t.Deleted);
                }

                return cat_sel;
            }

            return null;
        }

        [NonAction]
        public PlantCharacteristicCategory DbGetPlantCharacteristicCategoryById(int id)
        {
            var cat_sel = (from c in ctx.PlantCharacteristicCategory
                           where c.Id == id && !c.Deleted
                           select c);

            if (cat_sel != null && cat_sel.Any())
            {
                return cat_sel.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        [NonAction]
        public bool DbEditPlantCharacteristicCategory(PlantCharacteristicCategory categoryData)
        {
            categoryData.Title = categoryData.Title.Trim();
            // check duplicated entries
            var dupe_check = (from c in ctx.PlantCharacteristicCategory
                              where !c.Deleted && c.Title == categoryData.Title && c.Id != categoryData.Id
                              select c);

            if (dupe_check != null && dupe_check.Any())
            {
                return false;
            }

            PlantCharacteristicCategory cat_sel = ctx.PlantCharacteristicCategory.Find(categoryData.Id);

            if (cat_sel == null)
            {
                return false;
            }

            cat_sel.OnEdit(categoryData.EditedBy);
            cat_sel.Title = categoryData.Title;
            cat_sel.TagImage = categoryData.TagImage;
            cat_sel.CharacteristicValueType = categoryData.CharacteristicValueType;
            ctx.Entry(cat_sel).State = EntityState.Modified;
            return ctx.SaveChanges() > 0 ? true : false;
        }
        [NonAction]

        public bool DbDeletePlantCharacteristicCategory(int id, string deletedBy)
        {
            var cat_sel = (from c in ctx.PlantCharacteristicCategory
                           where c.Id == id && !c.Deleted
                           select c);

            if (cat_sel != null && cat_sel.Any())
            {
                cat_sel.FirstOrDefault().OnEdit(deletedBy);
                cat_sel.FirstOrDefault().Deleted = true;

                ctx.Entry(cat_sel.FirstOrDefault()).State = EntityState.Modified;

                var characteristicsInThisCategory_sel = (from citc in ctx.PlantCharacteristic
                                                         where !citc.Deleted && citc.CategoryId == id
                                                         select citc);

                if (characteristicsInThisCategory_sel != null && characteristicsInThisCategory_sel.Any())
                {
                    foreach (PlantCharacteristic pchar in characteristicsInThisCategory_sel)
                    {
                        pchar.EditedBy = deletedBy;
                        pchar.EditedDate = DateTime.Now;
                        pchar.Deleted = true;
                        ctx.Entry(pchar).State = EntityState.Modified;
                    }
                }

                return ctx.SaveChanges() > 0 ? true : false;
            }
            else
            {
                return false;
            }
        }

        public PlantCharacteristicCategory GetPlantCharacteristicCategoryByName(string name)
        {
            return ctx.PlantCharacteristicCategory.Where(m => m.Title.ToUpper() == name.ToUpper() && !m.Deleted).FirstOrDefault();
        }
    }
}