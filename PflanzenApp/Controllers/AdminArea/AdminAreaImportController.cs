using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text;
using PflanzenApp.Controllers.AdminArea;
using CsvHelper.Configuration;
using GardifyModels.Models;

namespace PflanzenApp.Controllers.AdminArea
{
    public class AdminAreaImportController : _BaseController
    {
        ImportContext importCtx = new ImportContext();
        PlantCharacteristicCategoryController pccc = new PlantCharacteristicCategoryController();
        PlantCharacteristicController pcc = new PlantCharacteristicController();
        PlantController pc = new PlantController();
        PlantTagCategoryController ptcc = new PlantTagCategoryController();
        PlantTagController ptc = new PlantTagController();
        AdminAreaContentContributionController aaccc = new AdminAreaContentContributionController();
        // GET: AdminAreaImport
        public ActionResult CsvImport()
        {
            TextReader tr = new StreamReader(@"C:\Users\nl17\Source\Repos\PflanzenApp\PflanzenApp\PflanzenApp\nfiles\ImportData\import.csv", Encoding.Default, true);
            //using (var fs = System.IO.File.OpenRead(@"C:\Users\nl17\Source\Repos\PflanzenApp\PflanzenApp\PflanzenApp\nfiles\ImportData\import.csv"))
            //using (var reader2 = new StreamReader(fs))
            //{
            //    List<string> listA = new List<string>();
            //    List<string> listB = new List<string>();
            //    while (!reader2.EndOfStream)
            //    {
            //        var line = reader2.ReadLine();
            //        var values = line.Split(';');

            //        listA.Add(values[0]);
            //        listB.Add(values[1]);
            //    }
            //}
            CultureInfo culture = new CultureInfo("de-DE");
            var reader = new CsvReader(tr, new CsvConfiguration
            {
                Delimiter = ";",
                CultureInfo = culture,
                Encoding = Encoding.UTF8
            });
            reader.Read();
            List<dynamic> parsed = new List<dynamic>();
            do
            {
                dynamic record = new System.Dynamic.ExpandoObject();

                record.Produkt = reader.GetField("Produkt");
                record.Bild = reader.GetField("Bild");
                record.Produktinformation = reader.GetField("Produktinformation");
                record.Preis = reader.GetField<string>("Preis");
                record.Groesse = reader.GetField<string>("Groesse");
                record.Gewicht = reader.GetField<string>("Gewicht");
                record.MWst = reader.GetField<string>("MWst.");
                record.Farbe = reader.GetField<string>("Farbe");
                record.Materialien = reader.GetField<string>("Materialien");
                record.Volumen = reader.GetField<string>("Volumen");
                record.Anwendungsbeispiel = reader.GetField<string>("Anwendungsbeispiel");
                record.Lebensdauer = reader.GetField<string>("Lebensdauer");
                record.Farbtemperatur = reader.GetField<string>("Farbtemperatur");
                record.Lichtleistung = reader.GetField<string>("Lichtleistung");
                parsed.Add(record);

            } while (reader.Read());
            //foreach (var dyn in parsed)
            //{
            //    var entry = ctx.Zutaten.Where(v => v.ZutatId == dyn.ZutatId).FirstOrDefault();
            //    entry.ZutatEiweiss = dyn.ZutatEiweiss;
            //    entry.ZutatFett = dyn.ZutatFett;
            //    entry.ZutatKalorien = dyn.ZutatKalorien;
            //    entry.ZutatKohlenhydrat = dyn.ZutatKohlenhydrat;
            //    entry.ZutatFreigeschaltet = true;
            //    ctx.SaveChanges();
            //}
            foreach (dynamic expando in parsed)
            {
                Article newArticle = new Article
                {
                    IsAvailable = false,
                    Name = expando.Produkt,
                    Price = !string.IsNullOrEmpty(expando.Preis) ? decimal.Parse(expando.Preis, culture) : 0,
                    PricePercentagePayableWithPoints = 0,
                    Description = expando.Produktinformation
                };

                if (!string.IsNullOrEmpty(expando.Groesse))
                {
                    newArticle.Description += "\n\nGröße: " + expando.Groesse;
                }
                if (!string.IsNullOrEmpty(expando.Gewicht))
                {
                    newArticle.Description += "\n\nGewicht: " + expando.Gewicht;
                }
                if (!string.IsNullOrEmpty(expando.Farbe))
                {
                    newArticle.Description += "\n\nFarbe: " + expando.Farbe;
                }
                if (!string.IsNullOrEmpty(expando.Materialien))
                {
                    newArticle.Description += "\n\nMaterialien: " + expando.Materialien;
                }
                if (!string.IsNullOrEmpty(expando.Volumen))
                {
                    newArticle.Description += "\n\nVolumen: " + expando.Volumen;
                }
                if (!string.IsNullOrEmpty(expando.Anwendungsbeispiel))
                {
                    newArticle.Description += "\n\nAnwendungsbeispiel: " + expando.Anwendungsbeispiel;
                }
                if (!string.IsNullOrEmpty(expando.Lebensdauer))
                {
                    newArticle.Description += "\n\nLebensdauer: " + expando.Lebensdauer;
                }
                if (!string.IsNullOrEmpty(expando.Farbtemperatur))
                {
                    newArticle.Description += "\n\nFarbtemperatur: " + expando.Farbtemperatur;
                }
                if (!string.IsNullOrEmpty(expando.Lichtleistung))
                {
                    newArticle.Description += "\n\nLichtleistung: " + expando.Lichtleistung;
                }

                if (string.IsNullOrEmpty(newArticle.Description))
                {
                    newArticle.Description = "-";
                }

                newArticle.OnCreate("SYSTEM");

                ctx.Articles.Add(newArticle);
            }
            try
            {
                ctx.SaveChanges();
            }
            catch (Exception e)
            {
                //asd
            }
            return null;
        }
        public ActionResult Index()
        {
            var imports = ctx.ImportHistory.ToList();
            var plants = importCtx.SimplePlant.ToList();
            List<SelectListItem> list = new List<SelectListItem>();
            SelectListItem item = new SelectListItem()
            {
                Selected = true,
                Text = "Bitte wählen Sie eine Pflanze zum Importieren aus",
                Value = "-1"
            };
            list.Add(item);
            foreach (var m in plants)
            {
                if (!imports.Any(v => v.SimplePlantId == m.Id))
                {
                    SelectListItem sel_item = new SelectListItem()
                    {
                        Value = m.Id.ToString(),
                        Selected = false,
                        Text = m.NameLatin
                    };
                    list.Add(sel_item);
                }
            }

            var plantsCopy = ctx.Plants.ToList();
            List<SelectListItem> listCopy = new List<SelectListItem>();
            SelectListItem itemCopy = new SelectListItem()
            {
                Selected = true,
                Text = "Bitte wählen Sie eine Pflanze zum kopieren aus",
                Value = "-1"
            };
            listCopy.Add(itemCopy);
            foreach (var m in plantsCopy)
            {
                SelectListItem sel_item = new SelectListItem()
                {
                    Value = m.Id.ToString(),
                    Selected = false,
                    Text = m.NameLatin
                };
                listCopy.Add(sel_item);
            }
            AdminAreaViewModels.AdminAreaImportIndexViewModel vm = new AdminAreaViewModels.AdminAreaImportIndexViewModel()
            {
                PlantList = list,
                SelectedPlantId = -1,
                PlantListCopy = listCopy,
                SelectedCopyPlantId = -1
            };
            return View("~/Views/AdminArea/AdminAreaImport/Index.cshtml", vm);
        }

        [HttpPost]
        public ActionResult Index(AdminAreaViewModels.AdminAreaImportIndexViewModel vm)
        {
            AdminAreaPlantController aapc = new AdminAreaPlantController();
            var simplePlant = importCtx.SimplePlant.Find(vm.SelectedPlantId);
            var x = GetPlantBySimplePlant(simplePlant, vm.SelectedCopyPlantId);
            return RedirectToAction("Edit", "AdminAreaPlant", new { id = x.Id });
        }

        public SimplePlant GetSimplePlant(int id)
        {
            return importCtx.SimplePlant.Find(id);
        }

        public Plant GetPlantBySimplePlant(SimplePlant sp, int? selectedCopyPlantId)
        {
            var comment = sp.Comment ?? "Beschreibung";
            string importText = sp.Comment == null ? "Es wurde keine Beschreibung importiert! <br/>" : "";
            Plant p = new Plant()
            {
                CreatedBy = "Import",
                CreatedDate = DateTime.Now,
                Deleted = false,
                EditedBy = "Import",
                EditedDate = DateTime.Now,
                NameGerman = sp.NameGerman,
                NameLatin = sp.NameLatin,
                Description = comment,
                Published = false,
                Synonym = sp.NameAlternative
            };
            ctx.Plants.Add(p);
            ctx.SaveChanges();
            if (sp.NameAlternative != null)
            {
                SynonymViewModels.SynonymCreateViewModel vm = new SynonymViewModels.SynonymCreateViewModel()
                {
                    ReferenceType = ModelEnums.ReferenceToModelClass.Plant,
                    ReferenceId = p.Id,
                    Text = sp.NameAlternative
                };
                aaccc.DbAddSynonym(vm);
            }
            if (sp.BloomingTimeFrom != "" || sp.BloomingTimeTo != "")
            {
                var category = pccc.GetPlantCharacteristicCategoryByName("Blütezeit (Von Monat bis Monat)");
                var characteristic = pcc.CreatePlantCharacteristic(p.Id, category.Id, ConvertSeasonsToMonths(sp.BloomingTimeFrom), ConvertSeasonsToMonths(sp.BloomingTimeTo));
                pcc.DbCreatePlantCharacteristic(characteristic);
                importText += "Die Blütezeit wurde mit den Werten: " + sp.BloomingTimeFrom + " und " + sp.BloomingTimeTo + " importiert. <br/>";
            }
            if (sp.HeightFrom != null || sp.HeightTo != null)
            {
                var category = pccc.GetPlantCharacteristicCategoryByName("Wuchshöhe (Von cm bis cm)");
                int min = (sp.HeightFrom == null) ? -1 : (int)sp.HeightFrom;
                int max = (sp.HeightTo == null) ? -1 : (int)sp.HeightTo;
                var characteristic = pcc.CreatePlantCharacteristic(p.Id, category.Id, min, max);
                pcc.DbCreatePlantCharacteristic(characteristic);
                importText += "Die Wuchshöhe wurde mit den Werten: " + sp.HeightFrom + " und " + sp.HeightTo + " importiert. <br/>";
            }
            if (sp.WidthFrom != null || sp.WidthTo != null)
            {
                var category = pccc.GetPlantCharacteristicCategoryByName("Wuchsbreite (Von cm bis cm)");
                int min = (sp.WidthFrom == null) ? -1 : (int)sp.WidthFrom;
                int max = (sp.WidthTo == null) ? -1 : (int)sp.WidthTo;
                var characteristic = pcc.CreatePlantCharacteristic(p.Id, category.Id, min, max);
                pcc.DbCreatePlantCharacteristic(characteristic);
                importText += "Die Wuchsbreite wurde mit den Werten: " + sp.WidthFrom + " und " + sp.WidthTo + " importiert. <br/>";
            }
            if (sp.FruitsTimeFrom != null || sp.FruitsTimeTo != null)
            {
                var category = pccc.GetPlantCharacteristicCategoryByName("Fruchtzeit (Von Monat bis Monat)");
                var characteristic = pcc.CreatePlantCharacteristic(p.Id, category.Id, ConvertSeasonsToMonths(sp.FruitsTimeFrom), ConvertSeasonsToMonths(sp.FruitsTimeTo));
                pcc.DbCreatePlantCharacteristic(characteristic);
                importText += "Die Wuchsbreite wurde mit den Werten: " + sp.FruitsTimeFrom + " und " + sp.FruitsTimeTo + " importiert. <br/>";
            }
            if (sp.CutFlower)
            {
                var tag = ptc.DbGetPlantTagByName("Schnittblume");
                ptc.DbAddTagToPlant(p.Id, tag.Id);
                importText += "Die Pflanze wurde als Schnittblume importiert. <br/>";
            }
            if (sp.PotPlant)
            {
                var tag = ptc.DbGetPlantTagByName("Kübelpflanze");
                ptc.DbAddTagToPlant(p.Id, tag.Id);
                importText += "Die Pflanze wurde als Topfpflanze importiert. <br/>";
            }
            if (sp.LocationSun)
            {
                var tag = ptc.DbGetPlantTagByName("Sonne");
                ptc.DbAddTagToPlant(p.Id, tag.Id);
                importText += "Die Pflanze wurde mit der Eigenschaft ''verträgt Sonne'' importiert. <br/>";
            }
            if (sp.LocationSemiShade)
            {
                var tag = ptc.DbGetPlantTagByName("Halbschatten");
                ptc.DbAddTagToPlant(p.Id, tag.Id);
                importText += "Die Pflanze wurde mit der Eigenschaft ''verträgt Halbschatten'' importiert. <br/>";
            }
            if (sp.LocationShade)
            {
                var tag = ptc.DbGetPlantTagByName("Schatten");
                ptc.DbAddTagToPlant(p.Id, tag.Id);
                importText += "Die Pflanze wurde mit der Eigenschaft ''verträgt Schatten'' importiert. <br/>";
            }
            if (sp.WaterUse != ModelEnums.WaterUse.NotSet)
            {
                var category = ptcc.DbGetPlantTagCategoryByName("Wasserbedarf");
                var tags = ptc.DbGetPlantTagsByCategoryId(category.Id);
                if (sp.WaterUse == ModelEnums.WaterUse.Small)
                {
                    var tag = tags.Where(m => m.Title.ToUpper() == "Niedrig".ToUpper()).FirstOrDefault();
                    ptc.DbAddTagToPlant(p.Id, tag.Id);
                    importText += "Die Pflanze wurde mit der Eigenschaft ''Wasserbedarf: gering'' importiert. <br/>";
                }
                else if (sp.WaterUse == ModelEnums.WaterUse.Medium)
                {
                    var tag = tags.Where(m => m.Title.ToUpper() == "Mittel".ToUpper()).FirstOrDefault();
                    ptc.DbAddTagToPlant(p.Id, tag.Id);
                    importText += "Die Pflanze wurde mit der Eigenschaft ''Wasserbedarf: mittel'' importiert. <br/>";
                }
                else if (sp.WaterUse == ModelEnums.WaterUse.Large)
                {
                    var tag = tags.Where(m => m.Title.ToUpper() == "Hoch".ToUpper()).FirstOrDefault();
                    ptc.DbAddTagToPlant(p.Id, tag.Id);
                    importText += "Die Pflanze wurde mit der Eigenschaft ''Wasserbedarf: hoch'' importiert. <br/>";
                }
            }
            if (sp.FrostResistence != ModelEnums.FrostResistence.NotSet)
            {
                var category = ptcc.DbGetPlantTagCategoryByName("Winterhärte");
                var tags = ptc.DbGetPlantTagsByCategoryId(category.Id);
                if (sp.FrostResistence == ModelEnums.FrostResistence.Zero)
                {
                    var tag = tags.Where(m => m.Title == "Z8 (-12,3° bis -6,7°)").FirstOrDefault();
                    ptc.DbAddTagToPlant(p.Id, tag.Id);
                    importText += "Die Pflanze wurde mit der Eigenschaft ''z8'' importiert. <br/>";
                }
                else if (sp.FrostResistence == ModelEnums.FrostResistence.Negative5)
                {
                    var tag = tags.Where(m => m.Title == "Z7 (-17,8° bis -12,3°)").FirstOrDefault();
                    ptc.DbAddTagToPlant(p.Id, tag.Id);
                    importText += "Die Pflanze wurde mit der Eigenschaft ''z7'' importiert. <br/>";
                }
                else if (sp.FrostResistence == ModelEnums.FrostResistence.Negative15)
                {
                    var tag = tags.Where(m => m.Title == "Z6 (-23,4° bis -17,8°)").FirstOrDefault();
                    ptc.DbAddTagToPlant(p.Id, tag.Id);
                    importText += "Die Pflanze wurde mit der Eigenschaft ''z6'' importiert. <br/>";
                }
            }

            AdminAreaPlantController aapc = new AdminAreaPlantController();

            //Copy values from selected plant
            if (selectedCopyPlantId != -1)
            {
                aapc.copyCharacteristicsFromPlant(p.Id, (int)selectedCopyPlantId);
                importText += "Erfolgreich Eigenschaften mit Werten kopiert. <br/>";
                aapc.copyTagFromPlant(p.Id, (int)selectedCopyPlantId);
                importText += "Erfolgreich Eigenschaften kopiert. <br/>";
                aapc.copyAlertsFromPlant(p.Id, (int)selectedCopyPlantId);
                importText += "Erfolgreich Warnungen kopiert. <br/>";
            }

            ImportHistory hist = new ImportHistory()
            {
                CreatedBy = "Import",
                CreatedDate = DateTime.Now,
                Deleted = false,
                EditedBy = "Import",
                EditedDate = DateTime.Now,
                PlantId = p.Id,
                SimplePlantId = sp.Id,
                Danger = sp.Caution,
                ImportText = importText
            };
            ctx.ImportHistory.Add(hist);
            ctx.SaveChanges();
            return p;
        }

        public int ConvertSeasonsToMonths(string season)
        {
            if (season == "Frühjahr")
            {
                return 3;
            }
            if (season == "Sommer")
            {
                return 6;
            }
            if (season == "Herbst")
            {
                return 9;
            }
            if (season == "Winter")
            {
                return 12;
            }
            else return -1;
        }
    }
}