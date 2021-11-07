using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
{
    public class PlantCharacteristicController : _BaseController
    {
        #region DB
        [NonAction]
        public bool DbCreatePlantCharacteristic(PlantCharacteristic newCharacteristicData, bool overwriteDuplicate = true)
        {
            // check category
            var cat_sel = (from c in ctx.PlantCharacteristicCategory
                           where c.Id == newCharacteristicData.CategoryId && !c.Deleted
                           select c);

            if (cat_sel == null || !cat_sel.Any())
            {
                return false;
            }

            // check plant
            var plant_sel = (from p in ctx.Plants
                             where p.Id == newCharacteristicData.PlantId && !p.Deleted
                             select p);

            if (plant_sel == null || !plant_sel.Any())
            {
                return false;
            }

            // check duplicated entries
            if (overwriteDuplicate)
            {
                var charac_sel = (from charac in ctx.PlantCharacteristic.Where(d => !d.Deleted)
                                  where charac.CategoryId == newCharacteristicData.CategoryId &&
                                  charac.PlantId == newCharacteristicData.PlantId
                                  select charac).FirstOrDefault();
                if(charac_sel != null)
                {
                    charac_sel.CategoryId = newCharacteristicData.CategoryId;
                    charac_sel.Max = newCharacteristicData.Max;
                    charac_sel.Min = newCharacteristicData.Min;
                    charac_sel.PlantId = newCharacteristicData.PlantId;
                    charac_sel.OnEdit("System");
                }
                else
                {
                    newCharacteristicData.OnCreate("System");
                    newCharacteristicData.CreatedDate = DateTime.Now;
                    ctx.PlantCharacteristic.Add(newCharacteristicData);
                }

                ////Old code: Redirect if duplicate is found
                //if (charac_sel != null && charac_sel.Any())
                //{
                //    return false;
                //}
            }
            else
            {
                newCharacteristicData.OnCreate("System");
                newCharacteristicData.CreatedDate = DateTime.Now;
                ctx.PlantCharacteristic.Add(newCharacteristicData);
            }

            return ctx.SaveChanges() > 0 ? true : false;
        }

        [NonAction]
        public PlantCharacteristic DbGetPlantCharacteristicById(int id)
        {
            var cat_sel = (from c in ctx.PlantCharacteristic
                           where c.Id == id && !c.Deleted
                           select c);
            if (cat_sel != null && cat_sel.Any())
            {
                return cat_sel.FirstOrDefault();
            }
            return null;
        }

        [NonAction]
        public IEnumerable<PlantCharacteristic> DbGetPlantCharacteristicsByPlantId(int plantId)
        {
            return (from c in ctx.PlantCharacteristic
                    where c.PlantId == plantId && !c.Deleted
                    select c).Include(c => c.Category);
        }

        [NonAction]
        public IEnumerable<PlantCharacteristic> DbGetPlantCharacteristicsByCategoryId(int categoryId)
        {
            return (from c in ctx.PlantCharacteristic
                    where c.CategoryId == categoryId && !c.Deleted
                    select c).Include(c => c.Plant);
        }

        [NonAction]
        public bool DbEditPlantCharacteristic(PlantCharacteristic plantCharacteristicToEdit)
        {
            var charac_sel = (from charac in ctx.PlantCharacteristic
                              where charac.Id == plantCharacteristicToEdit.Id && !charac.Deleted
                              select charac);

            if (charac_sel != null && charac_sel.Any())
            {
                PlantCharacteristic original = charac_sel.FirstOrDefault();
                original.Min = plantCharacteristicToEdit.Min;
                original.Max = plantCharacteristicToEdit.Max;
                original.OnEdit(plantCharacteristicToEdit.EditedBy);
                ctx.Entry(original).State = EntityState.Modified;
                return ctx.SaveChanges() > 0 ? true : false;
            }
            else
            {
                return false;
            }
        }

        //[NonAction]
        //public bool DbRemoveCharacteristicFromPlant(int plantId, int characteristicId, bool saveChanges = true)
        //{
        //    var checkPlant_sel = (from p in ctx.Plants
        //                          where p.Id == plantId && !p.Deleted
        //                          select p);

        //    // if plant not exists or plant tag is not in plant tags list return false
        //    if (checkPlant_sel == null || !checkPlant_sel.Any())
        //    {
        //        return false;
        //    }
        //    if (checkPlant_sel.FirstOrDefault().PlantCharacteristics != null &&
        //        (checkPlant_sel.FirstOrDefault().PlantCharacteristics.Where(t => t.Id == characteristicId) == null ||
        //        !checkPlant_sel.FirstOrDefault().PlantCharacteristics.Where(t => t.Id == characteristicId).Any()))
        //    {
        //        return false;
        //    }

        //    checkPlant_sel.FirstOrDefault().PlantCharacteristics.Remove(checkPlant_sel.FirstOrDefault().PlantCharacteristics.Where(t => t.Id == characteristicId).FirstOrDefault());
        //    if (saveChanges)
        //    {
        //        return ctx.SaveChanges() > 0 ? true : false;
        //    }
        //    return false;
        //}

        [NonAction]
        public bool DbDeletePlantCharacteristic(int id, string deletedBy)
        {
            var charac_sel = DbGetPlantCharacteristicById(id);
            if (charac_sel != null)
            {
                charac_sel.OnEdit(deletedBy);
                charac_sel.Deleted = true;
                ctx.Entry(charac_sel).State = EntityState.Modified;
                return ctx.SaveChanges() > 0 ? true : false;
            }
            else
            {
                return false;
            }
        }

        [NonAction]
        public PlantCharacteristic CreatePlantCharacteristic(int plantId, int categoryId, int min, int max)
        {
            PlantCharacteristic pc = new PlantCharacteristic()
            {
                CategoryId = categoryId,
                CreatedBy = "SYSTEM",
                CreatedDate = DateTime.Now,
                Deleted = false,
                EditedBy = "SYSTEM",
                EditedDate = DateTime.Now,
                Min = min,
                Max = max,
                PlantId = plantId
            };
            return pc;
        }
        
        #endregion plant characteristics
    }
}