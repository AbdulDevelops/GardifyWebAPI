using Microsoft.AspNet.Identity;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace PflanzenApp.Controllers
{
    public class ForumController : _BaseController
    {

		public ActionResult Index(int? taxonId)
		{
			ForumViewModels.ForumIndexViewModel viewModel = new ForumViewModels.ForumIndexViewModel();
			if(taxonId != null)
			{
				 
				PlantController pc = new PlantController();
				List<Plant> plants = pc.DbGetChildrenTaxonPlantsByParentTaxonId((int) taxonId);

				if(plants != null && plants.Any())
				{
					foreach (Plant plant in plants)
					{
						IEnumerable<ForumHeader> plantForumHeaders = DbGetForumHeadersByRelatedObjectId(plant.Id, ModelEnums.ReferenceToModelClass.Plant);

						if(plantForumHeaders != null && plantForumHeaders.Any())
						{

							foreach (ForumHeader header in plantForumHeaders)
							{
								if(header.IsThread)
								{
									ForumViewModels.ForumHeaderViewModel plantThread = DbGetForumHeaderViewModel(header);
									plantThread.PostCount = DbGetPostsByHeaderId(header.Id).Count();
									viewModel.Threads.Add(plantThread);
								}
							}
						}
					}
				}				
			}
			else
			{
				IEnumerable<ForumHeader> plantForumHeaders = ctx.ForumHeaders.Where(f => f.IsThread);

				if (plantForumHeaders != null && plantForumHeaders.Any())
				{

					foreach (ForumHeader header in plantForumHeaders)
					{
						if (header.IsThread)
						{
							ForumViewModels.ForumHeaderViewModel plantThread = DbGetForumHeaderViewModel(header);
							plantThread.PostCount = DbGetPostsByHeaderId(header.Id).Count();
							viewModel.Threads.Add(plantThread);
						}
					}
				}
			}
			return View(viewModel);
		}

		public ActionResult Thread(int? id)
		{
			if(id != null)
			{
				ForumViewModels.ForumThreadViewModel viewModel = new ForumViewModels.ForumThreadViewModel();
				ForumHeader header = DbGetForumHeaderById((int)id);
				if (header != null)
				{
					viewModel.ThreadHeader = DbGetForumHeaderViewModel(header);
					viewModel.RelatedPosts = DbGetForumPostViewModels(DbGetPostsByHeaderId((int)id));
				}
				return View(viewModel);
			}
            return RedirectToError("Seite konnte nicht gefunden werden.", HttpStatusCode.NotFound, "ForumController.Thread("+id+")");
        }

        public ActionResult PlantThread(int plantId)
        {
			ForumViewModels.ForumThreadViewModel viewModel = new ForumViewModels.ForumThreadViewModel();

			PlantController pc = new PlantController();
			Plant relatedPlant = pc.DbGetPlantPublishedById(plantId);

			if(relatedPlant == null)
			{
                return RedirectToError("Seite konnte nicht gefunden werden.", HttpStatusCode.NotFound, "ForumController.PlantThread("+plantId+")");
            }

			IEnumerable<ForumHeader> headers = DbGetForumHeadersByRelatedObjectId(plantId, ModelEnums.ReferenceToModelClass.Plant);

			if(headers == null || !headers.Any()) // erstelle automatisch ein Thread für die Pflanze
			{
				ForumHeader newHeader = new ForumHeader
				{
					AuthorId = Guid.Empty,
					IsThread = true,
					CreatedBy = "CREATED_BY_SYSTEM",
					Title = "Thread für " + relatedPlant.NameGerman + " (" + relatedPlant.NameLatin + ")",
					RelatedObjectId = plantId,
					RelatedObjectType = ModelEnums.ReferenceToModelClass.Plant,
				};
				DbCreateForumHeader(newHeader);

				viewModel.ThreadHeader = DbGetForumHeaderViewModel(newHeader);
				return PartialView("_ThreadView", viewModel);
			}

			ForumHeader header = headers.FirstOrDefault();
			viewModel.ThreadHeader = DbGetForumHeaderViewModel(header);

			IEnumerable<ForumPost> posts = DbGetPostsByHeaderId(header.Id);
			if(posts != null && posts.Any())
			{
				viewModel.RelatedPosts = DbGetForumPostViewModels(posts);
			}

			return PartialView("_ThreadView", viewModel);
		}

        [HttpPost]
		[ValidateAntiForgeryToken]
        public ActionResult CreatePlantPost(ForumViewModels.ForumPostViewModel newPostData, int plantId)
        {
           if(ModelState.IsValid)
			{
				Guid userId = Guid.Parse(User.Identity.GetUserId());
				string userName = User.Identity.GetUserName();
				ForumPost newPost = new ForumPost
				{
					AuthorId = userId,
					Content = newPostData.Content,
					CreatedBy = userName,
					ForumHeaderId = newPostData.HeaderId,					
				};
				DbCreatePost(newPost);
			}
			return PlantThread(plantId);
        }

        // POST: Forum/Edit/5
        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        // POST: Forum/Delete/5
        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }		

		#region DB

		[NonAction]
		public ForumViewModels.ForumHeaderViewModel DbGetForumHeaderViewModel(ForumHeader header)
		{			
			if (header != null)
			{
				ApplicationUser author;
				if (header.AuthorId == Guid.Empty) // created by system!!
				{
					author = new ApplicationUser
					{
						Id = header.AuthorId.ToString(),
						UserName = "autogenerated",
						ProfileUrl = ""
					};
				}
				else
				{
					author = UserManager.FindById(header.AuthorId.ToString());
				}

				return new ForumViewModels.ForumHeaderViewModel
				{
					Id = header.Id,
					RelatedObjectId = header.RelatedObjectId,
					AuthorId = header.AuthorId,
					AuthorName = author.UserName,
					AuthorUrl = author.ProfileUrl,
					IsThread = header.IsThread,
					CreatedDate = header.CreatedDate,
					ParentId = header.ParentId,
					RootId = header.RootId,
					Title = header.Title
				};
			}
			return null;
		}

		[NonAction]
		public ForumViewModels.ForumPostViewModel DbGetForumPostViewModel(ForumPost post)
		{
			if(post != null)
			{
				ApplicationUser author;
				if (post.AuthorId == Guid.Empty) // created by system!!
				{
					author = new ApplicationUser
					{
						Id = post.AuthorId.ToString(),
						UserName = "autogenerated",
						ProfileUrl = ""
					};
				} else
				{
					author = UserManager.FindById(post.AuthorId.ToString());
				}
								
				return new ForumViewModels.ForumPostViewModel
				{
					Id = post.Id,
					AuthorId = post.AuthorId,
					AuthorName = author.UserName,
					AuthorUrl = author.ProfileUrl,
					Content = post.Content,
					CreatedDate = post.CreatedDate
				};
			}
			return null;
		}

		[NonAction]
		public IEnumerable<ForumViewModels.ForumPostViewModel> DbGetForumPostViewModels (IEnumerable<ForumPost> posts)
		{
			if(posts != null)
			{
				return posts.Select(p => DbGetForumPostViewModel(p));
			}
			return null;
		}

		[NonAction]
		public bool DbCreateForumHeader(ForumHeader newForumHeader)
		{
			// existiert rootknoten?
			if (newForumHeader.RootId > 0)
			{
				ForumHeader root = DbGetForumHeaderById(newForumHeader.RootId);
				if (root == null)
				{
					return false;
				}
			}

			// existiert elternknoten?
			if (newForumHeader.ParentId > 0)
			{
				ForumHeader parent = DbGetForumHeaderById(newForumHeader.ParentId);
				// nur headers, die keine threads sind, dürfen eltern für ein anderes header sein
				if (parent == null || parent.IsThread)
				{
					return false;
				}
			}

			newForumHeader.OnCreate(newForumHeader.CreatedBy);
			ctx.ForumHeaders.Add(newForumHeader);
			return ctx.SaveChanges() > 0 ? true : false;
		}

		[NonAction]
		public ForumHeader DbGetForumHeaderById (int headerId)
		{
			return (from h in ctx.ForumHeaders
					where !h.Deleted && h.Id == headerId
					select h).FirstOrDefault();
		}

		public IEnumerable<ForumHeader> DbGetForumHeadersByRelatedObjectId(int objectId, ModelEnums.ReferenceToModelClass objectType)
		{
			return (from h in ctx.ForumHeaders
					where !h.Deleted && h.RelatedObjectId == objectId && h.RelatedObjectType == objectType
					orderby h.CreatedDate descending
					select h);
		}

		[NonAction]
		public bool DbCreatePost(ForumPost newPost)
		{
			// existiert elternknoten?
			if (newPost.ForumHeaderId > 0)
			{
				ForumHeader parent = DbGetForumHeaderById(newPost.ForumHeaderId);
				// nur headers, die thread sind, dürfen posts haben
				if (parent == null || !parent.IsThread)
				{
					return false;
				}
			}

			newPost.OnCreate(newPost.CreatedBy);
			ctx.ForumPost.Add(newPost);
			return ctx.SaveChanges() > 0 ? true : false;
		}

		[NonAction]
		public IEnumerable<ForumPost> DbGetPostsByHeaderId(int headerId)
		{
			return (from p in ctx.ForumPost
					where !p.Deleted && p.ForumHeaderId == headerId
					orderby p.CreatedDate descending
					select p);
		}


		#endregion
	}
}
