using GardifyModels.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
{
    public class TaxonomicTreeController : _BaseController
    {
        [NonAction]
        public List<TaxonomicTree> DbGetAllLeafs()
        {
            var leafs = (from le in ctx.TaxonomicTree
                         where le.PlantId != null && le.PlantId != 0
                         && !le.Deleted
                         select le);
            return leafs.OrderBy(v => v.TitleLatin).ToList();
        }

        [NonAction]
        public List<TaxonomicTree> DbGetAllLeafParents()
        {
            var leafs = DbGetAllLeafs();
            var parents = leafs.Select(l => l.ParentId).Distinct();
            return ctx.TaxonomicTree.Where(t => parents.Any(p => p == t.Id)).OrderBy(v => v.TitleLatin).ToList();
        }

        public List<TaxonomicTree> DbGetAllNodes()
        {
            var nodes = (from le in ctx.TaxonomicTree
                         where !le.Deleted
                         select le);
            return nodes.OrderBy(v => v.TitleLatin).ToList();
        }

        [NonAction]
        public List<TaxonomicTree> DbGetAllTreeRoots()
        {
            var sel = (from t in ctx.TaxonomicTree
                       where t.Type == ModelEnums.NodeType.Root && !t.Deleted
                       select t);
            if (sel != null && sel.Any())
            {
                foreach (TaxonomicTree treeRoot in sel)
                {
                    treeRoot.Childs = (from ch in ctx.TaxonomicTree where ch.ParentId == treeRoot.Id && !ch.Deleted select ch).ToList();
                }
                return sel.ToList();
            }
            return null;
        }

        [NonAction]
        public TaxonomicTree DbGetTreeNodeById(int nodeId, bool includeChilds = true)
        {
            TaxonomicTree ret = null;

            var node_sel = (from r in ctx.TaxonomicTree
                            where r.Id == nodeId && !r.Deleted
                            select r);

            if (node_sel != null && node_sel.Any())
            {
                ret = node_sel.FirstOrDefault();

                if (includeChilds)
                {
                    ret.Childs = (from t in ctx.TaxonomicTree
                                  where t.ParentId == ret.Id && !t.Deleted
                                  orderby t.TitleLatin ascending
                                  orderby t.TitleGerman ascending
                                  select t).ToList();
                }
            }
            return ret;
        }


        [NonAction]
        public bool DbCheckIfContainChildren(int currentTreeId, int targetTreeId)
        {
            TaxonomicTree ret = null;

            var node_sel = DbGetTreeNodeById(currentTreeId);

            if (node_sel != null)
            {
                ret = node_sel;

                if (ret.Id == targetTreeId)
                {
                    return true;
                }
                //var children = (from t in ctx.TaxonomicTree
                //                where t.ParentId == ret.Id && !t.Deleted
                //                select t).ToList();

                foreach (var child in ret.Childs)
                {
                    return DbCheckIfContainChildren(child.Id, targetTreeId);

                }
            }

            return false;
        }

        [NonAction]
        public ModelEnums.TaxonomicRank DbCheckIfContainVariant(int nodeId)
        {
            TaxonomicTree ret = null;

            var node_sel = (from r in ctx.TaxonomicTree
                            where r.Id == nodeId && !r.Deleted
                            select r);

            if (node_sel != null && node_sel.Any())
            {
                ret = node_sel.FirstOrDefault();

       
                var children = (from t in ctx.TaxonomicTree
                                  where t.ParentId == ret.Id && !t.Deleted
                                  select t).ToList();

                var lowestTaxon = ret.Taxon;
                //if (children == null || children.Count() == 0)
                //{
                //    lowestTaxon = CompareEnumMax(lowestTaxon, ret.Taxon);
                //    //return ret.Taxon;
                //}

                foreach(var child in children)
                {
                    if(child.Taxon == ModelEnums.TaxonomicRank.Variety)
                    {
                        lowestTaxon = CompareEnumMax(lowestTaxon, ModelEnums.TaxonomicRank.Variety);

                        //return ModelEnums.TaxonomicRank.Variety;
                    }
                    else
                    {
                        lowestTaxon = CompareEnumMax(lowestTaxon, DbCheckIfContainVariant(child.Id));

                        //return DbCheckIfContainVariant(child.Id);

                    }


                }

                return lowestTaxon;

            }
            else
            {
                return ModelEnums.TaxonomicRank.NotSet;
            }
            return ModelEnums.TaxonomicRank.NotSet;
        }

        public ModelEnums.TaxonomicRank CompareEnumMax(ModelEnums.TaxonomicRank first, ModelEnums.TaxonomicRank second)
        {
            if ((int)first >= (int)second)
            {
                return first;
            }
            else
            {
                return second;
            }
        }


        [NonAction]
        public void DbMoveTreeNode(int movedNodeId, int taxonId)
        {
            TaxonomicTree ret = null;

            var node_sel = (from r in ctx.TaxonomicTree
                            where r.Id == movedNodeId && !r.Deleted
                            select r).FirstOrDefault();

            var parent_sel = (from r in ctx.TaxonomicTree
                            where r.Id == taxonId && !r.Deleted
                            select r).FirstOrDefault();

            node_sel.ParentId = taxonId;
            node_sel.Taxon = parent_sel.Taxon + 1;
            ctx.SaveChanges();

            if (node_sel.Childs != null && node_sel.Childs.Any())
            {
                foreach (var child in node_sel.Childs)
                {
                    DbChangeTaxonOrder(child.Id, node_sel.Taxon);

                }
            }


            return;
        }


        [NonAction]
        public void DbChangeTaxonOrder(int taxonId, ModelEnums.TaxonomicRank taxon)
        {
            //TaxonomicTree ret = taxon;

            var currentTaxon = DbGetTreeNodeById(taxonId);


            currentTaxon.Taxon = taxon + 1;
            ctx.SaveChanges();

            if (currentTaxon.Childs != null && currentTaxon.Childs.Any())
            {
                foreach(var child in currentTaxon.Childs)
                {
                    DbChangeTaxonOrder(child.Id, currentTaxon.Taxon);

                }
            }


            return;
        }

        [NonAction]
        public TaxonomicTree DbGetParentTreeByDeepestNode_recursive(TaxonomicTree childNode)
        {
            if (childNode != null && childNode.Childs != null && childNode.Childs.Any())
            {
                childNode.Childs = childNode.Childs.OrderBy(c => c.TitleLatin).OrderBy(c => c.TitleGerman).ToList<TaxonomicTree>();
            }

            if (childNode.ParentId == 0 || childNode.Type == ModelEnums.NodeType.Root)
            {
                return childNode;
            }
            else
            {
                IEnumerable<TaxonomicTree> parentNode_sel = (from p in ctx.TaxonomicTree
                                                             where p.Id == childNode.ParentId && !p.Deleted
                                                             select p);

                if (parentNode_sel == null || !parentNode_sel.Any())
                {
                    return childNode;
                }
                else
                {
                    TaxonomicTree parentNode = parentNode_sel.FirstOrDefault();

                    parentNode.IsParentOfOrSelectedTaxon = true;

                    parentNode.Childs = new List<TaxonomicTree>();

                    var childs_sel = (from c in ctx.TaxonomicTree
                                      where c.ParentId == parentNode.Id && c.Id != childNode.Id && !c.Deleted
                                      select c);

                    parentNode.Childs.AddRange(childs_sel);

                    parentNode.Childs.Add(childNode);

                    parentNode.Childs = parentNode.Childs.OrderBy(c => c.TitleLatin).OrderBy(c => c.TitleGerman).ToList<TaxonomicTree>();

                    return DbGetParentTreeByDeepestNode_recursive(parentNode);
                }
            }
        }

        [NonAction]
        public bool DbCreateTaxonomicTreeNode(TaxonomicTree taxTree)
        {
            TaxonomicTree newNodeData = taxTree;
            if (newNodeData != null)
            {
                TaxonomicTree parentNode = DbGetTreeNodeById(newNodeData.ParentId);

                // check parent
                if (parentNode != null)
                {
                    // no childs for taxon "species"
                    if (parentNode.Taxon == ModelEnums.TaxonomicRank.Variety)
                    {
                        return false;
                    }

                    newNodeData.TitleGerman = string.IsNullOrEmpty(newNodeData.TitleGerman) ? "" : newNodeData.TitleGerman.Trim();
                    newNodeData.TitleLatin = newNodeData.TitleLatin.Trim();

                    // check child names
                    if (parentNode.Childs != null && parentNode.Childs.Any())
                    {
                        var nameCheck_sel = (from ch in parentNode.Childs
                                             where !ch.Deleted && (ch.TitleGerman == newNodeData.TitleGerman && ch.TitleLatin == newNodeData.TitleLatin)
                                             select ch);
                        if (nameCheck_sel != null && nameCheck_sel.Any())
                        {
                            return false;
                        }
                    }

                    // save new node
                    // root nodes have no rootid
                    newNodeData.RootID = parentNode.RootID > 0 ? parentNode.RootID : parentNode.Id;
                    newNodeData.Taxon = parentNode.Taxon + 1;
                    newNodeData.OnCreate(newNodeData.CreatedBy);

                    if (newNodeData.Taxon == ModelEnums.TaxonomicRank.Species || newNodeData.Taxon == ModelEnums.TaxonomicRank.Genus || newNodeData.Taxon == ModelEnums.TaxonomicRank.Family || newNodeData.Taxon == ModelEnums.TaxonomicRank.Variety)
                    {
                        newNodeData.Type = ModelEnums.NodeType.Leaf;
                        newNodeData.PlantId = newNodeData.PlantId;
                    }
                    else
                    {
                        newNodeData.Type = ModelEnums.NodeType.Node;
                        newNodeData.PlantId = null;
                    }

                    ctx.TaxonomicTree.Add(newNodeData);
                    return ctx.SaveChanges() > 0 ? true : false;
                }
            }
            return false;
        }
        
        [NonAction]
        public bool DbEditTaxonomicTreeNode(TaxonomicTree node)
        {
            TaxonomicTree nodeToEdit = DbGetTreeNodeById(node.Id);

            // validate input
            if (nodeToEdit.ParentId != node.ParentId)
            {
                return false;
            }

            TaxonomicTree parentNode = DbGetTreeNodeById(node.ParentId);

            // check parent
            if (parentNode != null)
            {
                node.TitleGerman = string.IsNullOrEmpty(node.TitleGerman) ? "" : node.TitleGerman.Trim();
                node.TitleLatin = node.TitleLatin.Trim();

                // check sibling names
                if (parentNode.Childs != null && parentNode.Childs.Any())
                {
                    var nameCheck_sel = (from ch in parentNode.Childs
                                         where !ch.Deleted && (ch.TitleGerman == node.TitleGerman || ch.TitleLatin == node.TitleLatin) && ch.Id != node.Id
                                         select ch);

                    if (nameCheck_sel != null && nameCheck_sel.Any())
                    {
                        return false;
                    }
                }

                // apply new data				
                if (nodeToEdit.Taxon == ModelEnums.TaxonomicRank.Species || nodeToEdit.Taxon == ModelEnums.TaxonomicRank.Genus || nodeToEdit.Taxon == ModelEnums.TaxonomicRank.Family || nodeToEdit.Taxon == ModelEnums.TaxonomicRank.Variety)
                {
                    nodeToEdit.PlantId = node.PlantId;
                }

                nodeToEdit.TitleLatin = node.TitleLatin;
                nodeToEdit.TitleGerman = node.TitleGerman;
                nodeToEdit.OnEdit(node.EditedBy);

                ctx.Entry(nodeToEdit).State = EntityState.Modified;

                var res = ctx.SaveChanges() > 0 ? true : false;

                if (res && nodeToEdit.PlantId != null)
                {
                    var plant = ctx.Plants.FirstOrDefault(p => p.Id == nodeToEdit.PlantId);
                    plant.NameGerman = node.TitleGerman;
                    plant.NameLatin = node.TitleLatin;
                    plant.OnEdit(plant.EditedBy);

                    ctx.Entry(plant).State = EntityState.Modified;
                    ctx.SaveChanges();

                }

                return res;
            }
            return false;
        }

        public TaxonomicTree GetTreeNodeByPlantId(int id)
        {
            var nodes = DbGetAllNodes();
            return nodes.Where(v => v.PlantId == id).FirstOrDefault();
        }

        [NonAction]
        public bool DbDeleteTaxonomicTreeNode(int id, string deletedBy)
        {
            var node_sel = (from n in ctx.TaxonomicTree
                            where n.Id == id && !n.Deleted
                            select n);

            if (node_sel == null || !node_sel.Any())
            {
                return false;
            }

            if (node_sel.FirstOrDefault().Childs != null && node_sel.FirstOrDefault().Childs.Any())
            {
                return false;
            }

            node_sel.FirstOrDefault().Deleted = true;
            node_sel.FirstOrDefault().OnEdit(deletedBy);

            ctx.Entry(node_sel.FirstOrDefault()).State = EntityState.Modified;
            return ctx.SaveChanges() > 0 ? true : false;
        }

        [NonAction]
        public TaxonomicTree DbGetGenusTaxonPlantId(int plantId)
        {
            return (from t in ctx.TaxonomicTree
                    where !t.Deleted && t.PlantId == plantId
                    join gen in ctx.TaxonomicTree
                    on t.ParentId equals gen.Id
                    where !gen.Deleted
                    select gen).FirstOrDefault();
        }

        [NonAction]
        public IEnumerable<Plant> DbGetAllPlantsUnderTree(int treeId, bool showPublished = true)
        {
            List<Plant> plantList = new List<Plant>();
            var node = (from nod in ctx.TaxonomicTree
                        where nod.Id == treeId
                        && nod.Deleted == false
                        select nod).Single();
            if (node.PlantId != null)// || node.Taxon == ModelEnums.TaxonomicRank.Genus)
            {
                PlantController pc = new PlantController();
                if(node.PlantId != 0)
                {
                    if (showPublished)
                    {
                        var plant = pc.DbGetPlantPublishedById((int)node.PlantId);
                        if (plant != null)
                        {
                            plantList.Add(plant);
                        }
                    }
                    else
                    {
                        var plant = pc.DbGetPlantById((int)node.PlantId);
                        if (plant != null)
                        {
                            plantList.Add(plant);
                        }
                    }
                }
            }
            foreach (TaxonomicTree tree in DbGetChildTrees(treeId))
            {
                plantList.AddRange(DbGetAllPlantsUnderTree(tree.Id));
            }
            return plantList;
        }

        [NonAction]
        public IEnumerable<TaxonomicTree> DbGetChildTrees(int treeId)
        {
            var children = (from child in ctx.TaxonomicTree
                            where child.ParentId == treeId
                            && child.Deleted == false
                            select child).ToList();
            return children;
        }
        
    }
}