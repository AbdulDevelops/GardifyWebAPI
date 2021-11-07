using System.Net;
using System.Web.Mvc;
using GardifyModels.Models;
using System.Linq;
using PflanzenApp.App_Code;
using System.Collections.Generic;
using Microsoft.AspNet.Identity;
using static GardifyModels.Models.AdminAreaViewModels;

namespace PflanzenApp.Controllers.AdminArea
{
    [CustomAuthorizeAttribute(Roles = "Admin,Expert")]
    public class AdminAreaTaxonomicTreeController : Controller
    {
        // GET: intern/taxonomic-tree
        public ActionResult Index(int taxonId = 0)
        {
            AdminAreaViewModels.TaxonomicTreeViewModel taxonomicTreeViewModel = new AdminAreaViewModels.TaxonomicTreeViewModel();
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            PlantController pc = new PlantController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            List<TaxonomicTree> allRoots = ttc.DbGetAllTreeRoots();

            if (allRoots != null && allRoots.Count > 0)
            {
                taxonomicTreeViewModel.TreeRootsWithChilds = new List<TaxonomicTree>();

                TaxonomicTree selectedTaxon = null;
                if (taxonId > 0)
                {
                    selectedTaxon = ttc.DbGetTreeNodeById(taxonId);
                }

                foreach (TaxonomicTree treeRoot in allRoots)
                {
                    if (selectedTaxon != null && selectedTaxon.RootID == treeRoot.Id)
                    {
                        selectedTaxon.IsParentOfOrSelectedTaxon = true;
                        taxonomicTreeViewModel.TreeRootsWithChilds.Add(ttc.DbGetParentTreeByDeepestNode_recursive(selectedTaxon));
                    }
                    else
                    {
                        taxonomicTreeViewModel.TreeRootsWithChilds.Add(treeRoot);
                    }
                }

                if (selectedTaxon != null && (selectedTaxon.Taxon == ModelEnums.TaxonomicRank.Species || selectedTaxon.Taxon == ModelEnums.TaxonomicRank.Genus || selectedTaxon.Taxon == ModelEnums.TaxonomicRank.Family || selectedTaxon.Taxon == ModelEnums.TaxonomicRank.Variety) && selectedTaxon.PlantId > 0)
                {
                    Plant taxonPlant = pc.DbGetPlantById((int)selectedTaxon.PlantId);

                    if (taxonPlant != null)
                    {
                        taxonomicTreeViewModel.Plant = taxonPlant;
                        HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages(taxonPlant.Id);
                        taxonomicTreeViewModel.PlantImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
                    }
                }
            }
            return View("~/Views/AdminArea/AdminAreaTaxonomicTree/Index.cshtml", taxonomicTreeViewModel);
        }

        public ActionResult MoveNode(int movedNodeId, int taxonId = 0)
        {
            AdminAreaViewModels.TaxonomicTreeViewModel taxonomicTreeViewModel = new AdminAreaViewModels.TaxonomicTreeViewModel();
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            PlantController pc = new PlantController();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            List<TaxonomicTree> allRoots = ttc.DbGetAllTreeRoots();

            if (allRoots != null && allRoots.Count > 0)
            {
                taxonomicTreeViewModel.TreeRootsWithChilds = new List<TaxonomicTree>();
                taxonomicTreeViewModel.MovingNodeId = movedNodeId;
                TaxonomicTree selectedTaxon = null;
                if (taxonId > 0)
                {
                    selectedTaxon = ttc.DbGetTreeNodeById(taxonId);
                }
                else
                {
                    taxonId = movedNodeId;
                    selectedTaxon = ttc.DbGetTreeNodeById(taxonId);
                    
                }

                taxonomicTreeViewModel.CurrentMovingNode = ttc.DbGetTreeNodeById(movedNodeId);

                foreach (TaxonomicTree treeRoot in allRoots)
                {
                    if (selectedTaxon != null && selectedTaxon.RootID == treeRoot.Id)
                    {
                        selectedTaxon.IsParentOfOrSelectedTaxon = true;
                        taxonomicTreeViewModel.TreeRootsWithChilds.Add(ttc.DbGetParentTreeByDeepestNode_recursive(selectedTaxon));
                    }
                    else
                    {
                        taxonomicTreeViewModel.TreeRootsWithChilds.Add(treeRoot);
                    }
                }

                if (selectedTaxon != null && (selectedTaxon.Taxon == ModelEnums.TaxonomicRank.Species || selectedTaxon.Taxon == ModelEnums.TaxonomicRank.Genus || selectedTaxon.Taxon == ModelEnums.TaxonomicRank.Family || selectedTaxon.Taxon == ModelEnums.TaxonomicRank.Variety) && selectedTaxon.PlantId > 0)
                {
                    Plant taxonPlant = pc.DbGetPlantById((int)selectedTaxon.PlantId);

                    if (taxonPlant != null)
                    {
                        taxonomicTreeViewModel.Plant = taxonPlant;
                        HelperClasses.DbResponse imageResponse = rc.DbGetPlantReferencedImages(taxonPlant.Id);
                        taxonomicTreeViewModel.PlantImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Url.Content("~/"));
                    }
                }
            }

            if (taxonomicTreeViewModel.CurrentMovingNode == null)
            {
                return View("~/Views/AdminArea/AdminAreaTaxonomicTree/Index.cshtml", taxonomicTreeViewModel);

            }

            return View("~/Views/AdminArea/AdminAreaTaxonomicTree/MoveNode.cshtml", taxonomicTreeViewModel);
        }

        public ActionResult MoveNodeConfirm(int movedNodeId, int taxonId = 0)
        {
            TaxonomicTreeController ttc = new TaxonomicTreeController();

            TaxonomicTree movedTaxon = null;

            TaxonomicTree parentTaxon = null;
            movedTaxon = ttc.DbGetTreeNodeById(movedNodeId);
            parentTaxon = ttc.DbGetTreeNodeById(taxonId);
            var message = "";

            var vm = new TaxonomicTreeMoveConfirmView
            {
                movedNodeId = movedNodeId,
                SelectedtaxonId = taxonId,
                message = message,
                DisableFeature = false

            };
            if (((int)movedTaxon.Taxon - 1) == ((int)parentTaxon.Taxon))
            {
            }
            else if (((int)movedTaxon.Taxon - 1) > ((int)parentTaxon.Taxon))
            {

            }
            else if (((int)movedTaxon.Taxon - 1) < ((int)parentTaxon.Taxon))
            {

                if (ttc.DbCheckIfContainChildren(movedNodeId, taxonId))
                {
                    vm.message = "Dieses Kind kann nicht als Elternteil des Baums festgelegt werden";
                    vm.DisableFeature = true;
                }
                else
                {
                    var lowestRank = ttc.DbCheckIfContainVariant(movedNodeId);
                    var rankDifferent = (int)movedTaxon.Taxon - (int)lowestRank;
                    var finalRank = (int)parentTaxon.Taxon + System.Math.Abs(rankDifferent);

                    // check if possible if child will be lower that variety
                    if (finalRank > 8)
                    {
                        vm.message = "Bitte entfernen Sie zuerst alle Kinder von diesem Baum.";
                        vm.DisableFeature = true;
                    }
                }
                

                
            }

            TempData["statusMessage"] = "lul";


            return View("~/Views/AdminArea/AdminAreaTaxonomicTree/MoveNodeConfirm.cshtml", vm);
        }

        [HttpPost]
        [ActionName("node-move")]
        public ActionResult MoveNodeConfirmSubmit(int movedNodeId, int taxonId = 0)
        {
            TaxonomicTreeController ttc = new TaxonomicTreeController();

            TaxonomicTree movedTaxon = null;

            TaxonomicTree parentTaxon = null;
            movedTaxon = ttc.DbGetTreeNodeById(movedNodeId);
            parentTaxon = ttc.DbGetTreeNodeById(taxonId);

            var message = "";
  

            var vm = new TaxonomicTreeMoveConfirmView
            {
                movedNodeId = movedNodeId,
                SelectedtaxonId = taxonId,
                message = message,
                DisableFeature = false
            };

            if (((int)movedTaxon.Taxon - 1) < ((int)parentTaxon.Taxon))
            {
                if (ttc.DbCheckIfContainChildren(movedNodeId, taxonId))
                {
                    vm.message = "Dieses Kind kann nicht als Elternteil des Baums festgelegt werden";
                    vm.DisableFeature = true;
                    return View("~/Views/AdminArea/AdminAreaTaxonomicTree/MoveNodeConfirm.cshtml", vm);

                }
                var lowestRank = ttc.DbCheckIfContainVariant(movedNodeId);
                var rankDifferent = (int)movedTaxon.Taxon - (int)lowestRank;
                var finalRank = (int)parentTaxon.Taxon + System.Math.Abs(rankDifferent);

                

                // check if possible if child will be lower that variety
                if (finalRank > 8)
                {
                    vm.message = "Bitte entfernen Sie zuerst alle Kinder von diesem Baum.";
                    vm.DisableFeature = true;
                    return View("~/Views/AdminArea/AdminAreaTaxonomicTree/MoveNodeConfirm.cshtml", vm);
                }

                

            }

            ttc.DbMoveTreeNode(movedNodeId, taxonId);

            //movedTaxon.ParentId = taxonId;
            

            return RedirectToAction("MoveNode", new { movedNodeId = movedNodeId });
            //return View("~/Views/AdminArea/AdminAreaTaxonomicTree/MoveNodeConfirm.cshtml", vm);
        }
        // GET: intern/taxonomic-tree/Create
        public ActionResult Create(int? parentId)
        {
            TaxonomicTreeController ttc = new TaxonomicTreeController();

            if (parentId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            AdminAreaViewModels.TaxonomicTreeViewModel taxonomicTreeViewModel = new AdminAreaViewModels.TaxonomicTreeViewModel();

            TaxonomicTree parentNode = ttc.DbGetTreeNodeById((int)parentId, false);

            if (parentNode != null)
            {
                TaxonomicTree defaultData = new TaxonomicTree();
                defaultData.RootID = parentNode.RootID;
                defaultData.ParentId = parentNode.Id;
                defaultData.Taxon = parentNode.Taxon + 1;

                if (defaultData.Taxon != ModelEnums.TaxonomicRank.Species && defaultData.Taxon != ModelEnums.TaxonomicRank.Genus && defaultData.Taxon != ModelEnums.TaxonomicRank.Family && defaultData.Taxon != ModelEnums.TaxonomicRank.Variety)
                {
                    defaultData.Type = ModelEnums.NodeType.Node;
                }
                else
                {
                    defaultData.Type = ModelEnums.NodeType.Leaf;
                }

                taxonomicTreeViewModel.CurrentParentNode = parentNode;
                taxonomicTreeViewModel.CurrentNode = defaultData;

                return View("~/Views/AdminArea/AdminAreaTaxonomicTree/Create.cshtml", taxonomicTreeViewModel);
            }
            else
            {
                return RedirectToAction("Index");
            }
        }

        // POST: intern/taxonomic-tree/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "parentId,plantId,titleLatin,titleGerman", Prefix = "currentNode")] TaxonomicTree newNodeData)
        {
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            ModelState.Remove("currentNode.createdBy");
            if (ModelState.IsValid)
            {
                newNodeData.CreatedBy = User.Identity.GetUserName();
                bool isOk = ttc.DbCreateTaxonomicTreeNode(newNodeData);
                if (isOk)
                {
                    return RedirectToAction("Index", new { taxonId = newNodeData.ParentId });
                }
            }
            return RedirectToAction("Create", new { parentId = newNodeData.ParentId });
        }

        // GET: intern/taxonomic-tree/Edit/5
        public ActionResult Edit(int? id)
        {
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AdminAreaViewModels.TaxonomicTreeViewModel taxonomicTreeViewModel = new AdminAreaViewModels.TaxonomicTreeViewModel();

            taxonomicTreeViewModel.CurrentNode = ttc.DbGetTreeNodeById((int)id);

            if (taxonomicTreeViewModel.CurrentNode == null)
            {
                return HttpNotFound();
            }

            taxonomicTreeViewModel.CurrentParentNode = ttc.DbGetTreeNodeById(taxonomicTreeViewModel.CurrentNode.ParentId, false);

            return View("~/Views/AdminArea/AdminAreaTaxonomicTree/Edit.cshtml", taxonomicTreeViewModel);
        }

        // POST: intern/taxonomic-tree/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,parentId,plantId,titleLatin,titleGerman", Prefix = "currentNode")] TaxonomicTree newNodeData)
        {
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            ModelState.Remove("currentNode.createdBy");
            if (ModelState.IsValid)
            {
                newNodeData.EditedBy = User.Identity.GetUserName();
                if (ttc.DbEditTaxonomicTreeNode(newNodeData))
                {
                    return RedirectToAction("Index", new { taxonId = newNodeData.Id });
                }
            }
            return RedirectToAction("Edit", new { id = newNodeData.Id });
        }

        // POST: intern/taxonomic-tree/Delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int? id)
        {
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            TaxonomicTree taxonomicTree = ttc.DbGetTreeNodeById((int)id);
            if (taxonomicTree == null)
            {
                return HttpNotFound();
            }

            ttc.DbDeleteTaxonomicTreeNode((int)id, User.Identity.GetUserName());
            return RedirectToAction("Index", new { taxonId = taxonomicTree.ParentId });
        }

        public ActionResult BulkOption(int treeId, string treeName)
        {

            AdminAreaTodoController atc = new AdminAreaTodoController();
            var template = atc.DbGetTodoTemplateWithTaxonId(treeId);
            atc.DbFixAllTaxom();

            AdminAreaViewModels.TaxonomicBulkOptionViewModel vm = new AdminAreaViewModels.TaxonomicBulkOptionViewModel
            {
                TreeId = treeId,
                Name = treeName,
                todoTemplates = template
            };
            return View("~/Views/AdminArea/AdminAreaTaxonomicTree/BulkOption.cshtml", vm);
        }

 
        public ActionResult BulkEdit(int treeId)
        {
            AdminAreaViewModels.TaxonomicBulkEditViewModel vm = GetBulkEditViewModel(treeId);
            return View("~/Views/AdminArea/AdminAreaTaxonomicTree/BulkEdit.cshtml", vm);
        }

        public AdminAreaViewModels.TaxonomicBulkEditViewModel GetBulkEditViewModel(int treeId)
        {
            PlantTagController ptc = new PlantTagController();
            PlantCharacteristicCategoryController pccc = new PlantCharacteristicCategoryController();
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            var plantTagsList = new List<PlantTag>();
            plantTagsList.Add(new PlantTag
            {
                Id = -1,
                Title = "Bitte eine Eigenschaft auswählen"
            });
            var list = ptc.DbGetPlantTags();
            var list_tagsByViewModel = (from p in list
                                        orderby p.Category.Title ascending
                                        select new PlantTag
                                        {
                                            Id = p.Id,
                                            CategoryId = p.CategoryId,
                                            Category = p.Category,
                                            Title = p.Category.Title + ": " + p.Title
                                        }).ToList();
            plantTagsList.AddRange(list_tagsByViewModel);

            var characteristicCategoriesList = new List<PlantCharacteristicCategory>();
            characteristicCategoriesList.Add(new PlantCharacteristicCategory
            {
                Id = -1,
                Title = "Bitte ein Merkmal auswählen"
            });
            characteristicCategoriesList.AddRange(pccc.DbGetPlantCharacteristicCategories());

            var plantList = ttc.DbGetAllPlantsUnderTree(treeId, false);
            List<BulkTagViewModel> tagList = new List<BulkTagViewModel>();
            foreach (var plant in plantList)
            {
                var currentTagList = ptc.DBGetPlantTagsByPlantId(plant.Id);
                BulkTagViewModel btvm = new BulkTagViewModel()
                {
                    PlantId = plant.Id,
                    PlantName = plant.Name,
                    PlantNameLatin = plant.NameLatin,
                    Published = plant.Published,
                    PlantTags = currentTagList.Select(v => new AddPlantTagViewModel { PlantTagId = v.Id, PlantTagTitle = v.Title, PlantTagCategoryId = v.CategoryId, PlantTagCategoryName = v.Category.Title, AddTagToPlant = false }).OrderBy(p => p.PlantTagCategoryName)
                };
                tagList.Add(btvm);
            }

            var existingTags = tagList.SelectMany(p => p.PlantTags);

            AdminAreaViewModels.TaxonomicBulkEditViewModel vm = new AdminAreaViewModels.TaxonomicBulkEditViewModel
            {
                TreeId = treeId,
                TagList = tagList,
                PlantTagsList = plantTagsList,
                characteristicCategories = characteristicCategoriesList,
                Name = ttc.DbGetTreeNodeById(treeId, false).TitleLatin,

            };
            return vm;
        }

        public AdminAreaViewModels.TaxonomicBulkAddViewModel GetBulkAddViewModel(int tagId, int treeId)
        {
            PlantTagController ptc = new PlantTagController();
            PlantCharacteristicCategoryController pccc = new PlantCharacteristicCategoryController();
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            var plantTagsList = new List<PlantTag>();
            plantTagsList.Add(new PlantTag
            {
                Id = -1,
                Title = "Bitte eine Eigenschaft auswählen"
            });
            var list = ptc.DbGetPlantTags();
            var list_tagsByViewModel = (from p in list
                                        orderby p.Category.Title ascending
                                        select new PlantTag
                                        {
                                            Id = p.Id,
                                            CategoryId = p.CategoryId,
                                            Category = p.Category,
                                            Title = p.Category.Title + ": " + p.Title
                                        }).ToList();
            plantTagsList.AddRange(list_tagsByViewModel);

            //var characteristicCategoriesList = new List<PlantCharacteristicCategory>();
            //characteristicCategoriesList.Add(new PlantCharacteristicCategory
            //{
            //    Id = -1,
            //    Title = "Bitte ein Merkmal auswählen"
            //});
            //characteristicCategoriesList.AddRange(pccc.DbGetPlantCharacteristicCategories());

            var plantList = ttc.DbGetAllPlantsUnderTree(treeId, false);
            List<BulkTagViewModel> tagList = new List<BulkTagViewModel>();
            foreach (var plant in plantList)
            {
                var currentTagList = ptc.DBGetPlantTagsByPlantId(plant.Id).Where(t => t.Id == tagId);
                BulkTagViewModel btvm = new BulkTagViewModel()
                {
                    PlantId = plant.Id,
                    PlantName = plant.Name,
                    PlantNameLatin = plant.NameLatin,
                    Published = plant.Published,
                    PlantTags = currentTagList.Select(v => new AddPlantTagViewModel { PlantTagId = v.Id, PlantTagTitle = v.Title, PlantTagCategoryId = v.CategoryId, PlantTagCategoryName = v.Category.Title, AddTagToPlant = false }).OrderBy(p => p.PlantTagCategoryName)
                };
                tagList.Add(btvm);
            }

            var existingTags = tagList.SelectMany(p => p.PlantTags);

            AdminAreaViewModels.TaxonomicBulkAddViewModel vm = new AdminAreaViewModels.TaxonomicBulkAddViewModel
            {
                TreeId = treeId,
                TagList = tagList,
                PlantTagsList = plantTagsList,
                TagId = tagId,
                //characteristicCategories = characteristicCategoriesList,
                Name = ttc.DbGetTreeNodeById(treeId, false).TitleLatin,

            };
            return vm;
        }

        public AdminAreaViewModels.TaxonomicBulkAddViewModel AddAllTagToPlants(int tagId, int treeId)
        {
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            PlantTagController ptc = new PlantTagController();
            var tag = ptc.DbGetPlantTagById(tagId);

            var plantList = ttc.DbGetAllPlantsUnderTree(treeId, false);

            foreach (var plant in plantList)
            {
                var currentTagList = ptc.DBGetPlantTagsByPlantId(plant.Id).Where(v => v.CategoryId == tag.CategoryId
                //&& v.Id != tag.Id
                );

                if (!currentTagList.Any())
                {
                    ptc.DbAddTagToPlant(plant.Id, tagId, false);

                }
                //BulkTagViewModel btvm = new BulkTagViewModel()
                //{
                //    PlantId = plant.Id,
                //    PlantName = plant.Name,
                //    PlantNameLatin = plant.NameLatin,
                //    Published = plant.Published,
                //    AddTagToPlant = !currentTagList.Any(v => v.Id == tag.Id),
                //    PlantTags = currentTagList.Select(v => new AddPlantTagViewModel { PlantTagId = v.Id, PlantTagTitle = v.Title, AddTagToPlant = false })
                //};
                //tagList.Add(btvm);
            }
            ptc.SaveChanges();

            return null;

        }

        public AdminAreaViewModels.TaxonomicBulkAddViewModel DeleteAllTagToPlants(int tagId, int treeId)
        {
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            PlantTagController ptc = new PlantTagController();
            var tag = ptc.DbGetPlantTagById(tagId);

            var plantList = ttc.DbGetAllPlantsUnderTree(treeId, false);

            foreach (var plant in plantList)
            {
                var currentTagList = ptc.DBGetPlantTagsByPlantId(plant.Id).Where(v => v.CategoryId == tag.CategoryId
                //&& v.Id != tag.Id
                );

                if (currentTagList.Any())
                {
                    ptc.DbRemoveTagFromPlant(plant.Id, tagId, false);

                }

            }
            ptc.SaveChanges();

            return null;

        }

        [HttpPost]
        [ActionName("bulk-add-all")]
        public ActionResult bulkaddTagAll(int tagId, int treeId)
        {
            var all = AddAllTagToPlants(tagId, treeId);

            AdminAreaViewModels.TaxonomicBulkAddViewModel vm = GetBulkAddViewModel(tagId, treeId);
            return View("~/Views/AdminArea/AdminAreaTaxonomicTree/BulkAdd.cshtml", vm);
        }

        [HttpPost]
        [ActionName("bulk-delete-all")]
        public ActionResult bulkdeleteTagAll(int tagId, int treeId)
        {
            var all = DeleteAllTagToPlants(tagId, treeId);

            AdminAreaViewModels.TaxonomicBulkAddViewModel vm = GetBulkAddViewModel(tagId, treeId);
            return View("~/Views/AdminArea/AdminAreaTaxonomicTree/BulkAdd.cshtml", vm);
        }

        [HttpPost]
        [ActionName("bulk-add")]
        public ActionResult bulkaddTag(int tagId, int treeId)
        {
            AdminAreaViewModels.TaxonomicBulkAddViewModel vm = GetBulkAddViewModel(tagId,treeId);
            return View("~/Views/AdminArea/AdminAreaTaxonomicTree/BulkAdd.cshtml", vm);
        }

        [HttpPost]
        [ActionName("add-tag")]
        public ActionResult addTag(int tagId, int treeId)
        {
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            PlantTagController ptc = new PlantTagController();

            var tag = ptc.DbGetPlantTagById(tagId);
            var plantList = ttc.DbGetAllPlantsUnderTree(treeId, false);

            TaxonomicBulkSelectViewModel vm = new TaxonomicBulkSelectViewModel
            {
                TreeId = treeId,
                TagId = tag.Id,
                TagTitle = tag.Title,
                TagCategoryId = tag.Category.Id,
                TagCategoryTitle = tag.Category.Title
            };
            List<BulkTagViewModel> tagList = new List<BulkTagViewModel>();
            foreach (var plant in plantList)
            {
                var currentTagList = ptc.DBGetPlantTagsByPlantId(plant.Id).Where(v => v.CategoryId == tag.CategoryId
                //&& v.Id != tag.Id
                );
                BulkTagViewModel btvm = new BulkTagViewModel()
                {
                    PlantId = plant.Id,
                    PlantName = plant.Name,
                    PlantNameLatin = plant.NameLatin,
                    Published = plant.Published,
                    AddTagToPlant = !currentTagList.Any(v => v.Id == tag.Id),
                    PlantTags = currentTagList.Select(v => new AddPlantTagViewModel { PlantTagId = v.Id, PlantTagTitle = v.Title, AddTagToPlant = false })
                };
                tagList.Add(btvm);
            }
            vm.TagList = tagList;
            return View("~/Views/AdminArea/AdminAreaTaxonomicTree/BulkSelect.cshtml", vm);
        }

        [ActionName("delete-tags")]
        public ActionResult deleteTagsGet(int tagId, int treeId, int plantId, string tagName, bool fromDetail = false)
        {
            TaxonomicBulkDeleteViewModel vm = new TaxonomicBulkDeleteViewModel
            {
                TagId = tagId,
                TreeId = treeId,
                PlantId = plantId,
                TagName = tagName,
                fromDetail = fromDetail
            };
            return View("~/Views/AdminArea/AdminAreaTaxonomicTree/deleteTags.cshtml", vm);
        }

        [HttpPost]
        [ActionName("delete-tags")]
        public ActionResult deleteTags(int tagId, int treeId, int plantId, bool fromDetail = false)
        {
            PlantTagController ptc = new PlantTagController();


                ptc.DbRemoveTagFromPlant(plantId, tagId, false);

                ptc.SaveChanges();
            if (!fromDetail)
            {
                return RedirectToAction("BulkEdit", new { treeId = treeId });

            }

            AdminAreaViewModels.TaxonomicBulkAddViewModel vm = GetBulkAddViewModel(tagId, treeId);
            return View("~/Views/AdminArea/AdminAreaTaxonomicTree/BulkAdd.cshtml", vm);

        }

        [HttpPost]
        [ActionName("add-new-tags")]
        public ActionResult addNewTags(int tagId, int treeId, int plantId)
        {
            PlantTagController ptc = new PlantTagController();


            ptc.DbAddTagToPlant(plantId, tagId, false);

            ptc.SaveChanges();

            AdminAreaViewModels.TaxonomicBulkAddViewModel vm = GetBulkAddViewModel(tagId, treeId);
            return View("~/Views/AdminArea/AdminAreaTaxonomicTree/BulkAdd.cshtml", vm);
        }

        [HttpPost]
        [ActionName("update-tags")]
        public ActionResult updateTags(TaxonomicBulkSelectViewModel vm)
        {
            PlantTagController ptc = new PlantTagController();

            int treeId = vm.TreeId;
            int newTagId = vm.TagId;
            int categoryId = vm.TagCategoryId;

            int addCounter = 0;
            int replaceCounter = 0;
            //Checkboxen auslesen und Datenbank updaten
            foreach (BulkTagViewModel bsvm in vm.TagList)
            {
                int plantId = bsvm.PlantId;
                bool addNewTag = bsvm.AddTagToPlant;

                var currentTagList = ptc.DBGetPlantTagsByPlantId(plantId);
                if (addNewTag)
                {
                    //Neue Eigenschaft hinzufügen - Es werden keine vorhandenen ersetzt
                    if (currentTagList.Any(v => v.Id == newTagId))
                    {
                        //Die gewählte Eigenschaft ist bereits vorhanden und kann nicht erneut hinzugefügt werden - Abbruch
                        ViewBag.ErrorMessage = "Es wurde keine Änderung vorgenommen. Sie haben bei der Pflanze " +
                        bsvm.PlantName + " eine Eigenschaft hinzugefügt, die bereits vorhanden ist. Dies ist nicht zulässig.";
                        return View("~/Views/AdminArea/AdminAreaTaxonomicTree/BulkSelect.cshtml", vm);
                    }
                    ptc.DbAddTagToPlant(plantId, newTagId, false);
                    addCounter++;
                }
                if (bsvm.PlantTags != null)
                {
                    //Wenn die Liste nicht null ist, ist möglicherweise eine Ersetzung vorzunehmen
                    if (bsvm.PlantTags.Where(v => v.AddTagToPlant).Count() > 0 && addNewTag)
                    {
                        //Es wurde sowohl hinzufügen, als auch ersetzen bei der selben Pflanze gewählt - Abbruch
                        ViewBag.ErrorMessage = "Es wurde keine Änderung vorgenommen. Sie haben bei der Pflanze " +
                        bsvm.PlantName + " sowohl versucht, die Eigenschaft neu hinzuzufügen, als auch eine vorhandene zu ersetzen. Dies ist nicht zulässig.";
                        return View("~/Views/AdminArea/AdminAreaTaxonomicTree/BulkSelect.cshtml", vm);
                    }
                    if (bsvm.PlantTags.Where(v => v.AddTagToPlant).Count() > 1)
                    {
                        //Bei einer Pflanze wurden mehrere Tags ersetzt - Abbruch
                        ViewBag.ErrorMessage = "Es wurde keine Änderung vorgenommen. Sie haben bei der Pflanze " +
                        bsvm.PlantName + " bei mehreren Eigenschaften \"ersetzen\" gewählt, dies ist nicht zulässig.";
                        return View("~/Views/AdminArea/AdminAreaTaxonomicTree/BulkSelect.cshtml", vm);
                    }
                    //Finde entsprechendes Tag, lösche es und füge das neue hinzu
                    foreach (var btvm in bsvm.PlantTags)
                    {
                        int tagId = btvm.PlantTagId;
                        bool replaceExistingTag = btvm.AddTagToPlant;
                        if (replaceExistingTag)
                        {
                            if (currentTagList.Any(v => v.Id == newTagId && !(tagId == newTagId)))
                            {
                                //Die gewählte Eigenschaft ist bereits vorhanden und kann nicht erneut hinzugefügt werden - Abbruch
                                ViewBag.ErrorMessage = "Es wurde keine Änderung vorgenommen. Sie haben bei der Pflanze " +
                                bsvm.PlantName + " eine Eigenschaft hinzugefügt, die bereits vorhanden ist. Dies ist nicht zulässig.";
                                return View("~/Views/AdminArea/AdminAreaTaxonomicTree/BulkSelect.cshtml", vm);
                            }
                            //Altes Tag löschen
                            ptc.DbRemoveTagFromPlant(plantId, tagId, false);
                            //Neues hinzufügen
                            ptc.DbAddTagToPlant(plantId, newTagId, false);
                            replaceCounter++;
                        }
                    }
                }
            }
            ptc.SaveChanges();
            ViewBag.SuccessMessage = "Bulk-Operation erfolgreich ausgeführt: " + addCounter + " neue Eigenschaften hinzugefügt und " + replaceCounter + " Eigenschaften ersetzt.";
            return RedirectToAction("Index", new { taxonId = vm.TreeId });
        }

        [HttpPost]
        [ActionName("add-characteristic")]
        public ActionResult AddCharacteristic(int characteristicId, int treeId, AdminAreaViewModels.TaxonomicBulkEditViewModel tbevm)
        {
            TaxonomicTreeController ttc = new TaxonomicTreeController();
            PlantCharacteristicCategoryController pccc = new PlantCharacteristicCategoryController();

            var plantList = ttc.DbGetAllPlantsUnderTree(treeId, false);
            var characteristicCategory = pccc.DbGetPlantCharacteristicCategoryById(characteristicId);
            PlantCharacteristic pc = tbevm.SelectedCharacteristic;
            PlantCharacteristic characteristic = new PlantCharacteristic
            {
                Category = characteristicCategory,
                CategoryId = characteristicCategory.Id,
                Min = pc.Min,
                Max = pc.Max
            };

            TaxonomicBulkSelectViewModel vm = new TaxonomicBulkSelectViewModel
            {
                TreeId = treeId,
                //PlantList = plantList,
                SelectedCharacteristic = characteristic
            };
            return View("~/Views/AdminArea/AdminAreaTaxonomicTree/BulkSelect.cshtml", vm);
        }

        [HttpPost]
        [ActionName("add-alert")]
        public ActionResult addAlert(int treeId, AdminAreaViewModels.TaxonomicBulkEditViewModel tbevm)
        {
            TaxonomicTreeController ttc = new TaxonomicTreeController();

            var plantList = ttc.DbGetAllPlantsUnderTree(treeId);

            AdminAreaViewModels.TaxonomicBulkSelectViewModel vm = new AdminAreaViewModels.TaxonomicBulkSelectViewModel
            {
                TreeId = treeId,
                //PlantList = plantList,
                NewAlert = tbevm.NewAlert
            };

            return View("~/Views/AdminArea/AdminAreaTaxonomicTree/BulkSelect.cshtml", vm);
        }

        [HttpPost]
        [ActionName("update-characteristics")]
        public ActionResult updatecharacteristics(AdminAreaViewModels.TaxonomicBulkSelectViewModel vm)
        {
            //Checkboxen auslesen und Datenbank updaten
            return View();
        }

        [HttpPost]
        [ActionName("update-alerts")]
        public ActionResult updateAlerts(AdminAreaViewModels.TaxonomicBulkSelectViewModel vm)
        {
            //Checkboxen auslesen und Datenbank updaten
            return View();
        }
    }
}
