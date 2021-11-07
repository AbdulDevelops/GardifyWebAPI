using GardifyModels.Models;
using GardifyWebAPI.App_Code;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using System.Web.Http.Description;
using static GardifyModels.Models.ModelEnums;

namespace GardifyWebAPI.Controllers
{
    // Handles all garden albums related operations, and the new garden images details
    //[RoutePrefix("api/ImageAPI")]
    public class ImageAPIController : ApiController
    {
        private nfilesEntities nfiles = new nfilesEntities();
        private ApplicationDbContext db = new ApplicationDbContext();

        [Route("api/ImageAPI/album")]
        [HttpGet]
        public IHttpActionResult GetUserAlbums()
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            var userId = Utilities.GetUserId();
            var albums = db.GardenAlbums.Where(ga => ga.UserId == userId && !ga.Deleted).Select(ga => new GardenAlbumViewModel()
            {
                Id = ga.Id,
                Name = ga.Name,
                Description = ga.Description
            });


            List<GardenAlbumViewModel> albumViewModels = new List<GardenAlbumViewModel>();

            foreach (var album in albums)
            {


                HelperClasses.DbResponse imageResponse = rc.DbGetAlbumImgReferencedImages((int)album.Id);
                album.EntryImages = new List<_HtmlImageViewModel>();
                if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                {
                    album.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                }
                else
                {
                    //album.EntryImages.Add(new _HtmlImageViewModel
                    //{
                    //    SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                    //    Id = 0,
                    //    TitleAttr = "Kein Bild vorhanden"
                    //});
                }

                albumViewModels.Add(album);
            }


            return Ok(albumViewModels);
        }

        [Route("api/ImageAPI/otheralbum")]
        [HttpGet]
        public IHttpActionResult GetOtherUserAlbums(bool showAll = true, bool showContact = false, bool showAcceptedContact = false, int skip = 0, int take = int.MaxValue)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();


            var albumList = new List<GardenAlbum>();

            var userId = Utilities.GetUserId();
            var albums = db.GardenAlbums.Where(ga => !ga.Deleted);

            if (showAll)
            {
                var showAllAlbum = albums.Where(gp => gp.ShowMode == GardenPresentationPersonMode.ShowEveryone).ToList();
                albumList.AddRange(showAllAlbum);
            }

            if (showContact)
            {
                var privateAlbum = albums.Where(gp => gp.ShowMode == GardenPresentationPersonMode.ShowToAllContact);

                var albumContactList = db.UserContacts.Where(c => c.ContactUserId == userId && !c.Deleted).Select(c => c.UserId).ToList();
                var contactInAlbums = privateAlbum.Where(p => albumContactList.Contains(p.UserId)).ToList();

                albumList.AddRange(contactInAlbums);

            }

            if (showAcceptedContact)
            {
                var selectedAlbum = albums.Where(gp => gp.ShowMode == GardenPresentationPersonMode.ShowToSelectedContact);


                var selectedAlbumContactListId = db.GardenContactLists.Where(c => c.UserId == userId && c.GardenContactShowStatus.ArticleType == GardenArticleType.GardenArchive && !c.Deleted).Select(p => p.GardenContactShowStatus.ArticleId);

                var acceptedContactPres = selectedAlbum.Where(p => selectedAlbumContactListId.Contains(p.Id)).ToList();

                albumList.AddRange(acceptedContactPres);


            }


            var processedAlbum = albumList.Select(ga => new GardenAlbumViewModel()
            {
                Id = ga.Id,
                Name = ga.Name,
                Description = ga.Description,
                AuthorId = ga.UserId
            });

            List<GardenAlbumViewModel> albumViewModels = new List<GardenAlbumViewModel>();

            foreach (var album in processedAlbum)
            {

                album.ProfilUrl = GetUserProfilImg(album.AuthorId);
                var user = db.Users.Where(u => !u.Deleted && u.Id.ToLower() == album.AuthorId.ToString().ToLower()).FirstOrDefault();

                if (user != null)
                {
                    album.AuthorName = user.UserName.StartsWith("Deleted-") ? "anonym" : user.UserName.Split('@')[0];
                    //ICollection<Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole> role = user.Roles;
                    //if (role.Any())
                    //{
                    //    answerVm.IsAdminAnswer = true;
                    //}
                }


                HelperClasses.DbResponse imageResponse = rc.DbGetAlbumImgReferencedImages((int)album.Id);
                album.EntryImages = new List<_HtmlImageViewModel>();
                if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                {
                    album.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                }
                else
                {
                    //album.EntryImages.Add(new _HtmlImageViewModel
                    //{
                    //    SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                    //    Id = 0,
                    //    TitleAttr = "Kein Bild vorhanden"
                    //});
                }

                albumViewModels.Add(album);
            }


            return Ok(albumViewModels);
        }

        [Route("api/ImageAPI/otherpresentation")]
        [HttpGet]
        public IHttpActionResult GetOtherUserPresentations(bool showAll = true, bool showContact = false, bool showAcceptedContact = false, int skip = 0, int take = int.MaxValue)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            var presentations = new List<GardenPresentation>();
            var allPresentation = db.GardenPresentations.Where(gp => !gp.Deleted);
            if (showAll)
            {
                var showAllPres = allPresentation.Where(gp => gp.ShowMode == GardenPresentationPersonMode.ShowEveryone).ToList();
                presentations.AddRange(showAllPres);
            }
            var userId = Utilities.GetUserId();

            if (showContact)
            {
                var privatePresentation = allPresentation.Where(gp => gp.ShowMode == GardenPresentationPersonMode.ShowToAllContact);

                var presentationContactList = db.UserContacts.Where(c => c.ContactUserId == userId && !c.Deleted).Select(c => c.UserId).ToList();
                var contactInPres = privatePresentation.Where(p => presentationContactList.Contains(p.UserId)).ToList();

                presentations.AddRange(contactInPres);

            }

            if (showAcceptedContact)
            {
                var selectedPresentation = allPresentation.Where(gp => gp.ShowMode == GardenPresentationPersonMode.ShowToSelectedContact);


                var selectedPresentationContactListId = db.GardenContactLists.Where(c => c.UserId == userId && c.GardenContactShowStatus.ArticleType == GardenArticleType.GardenPresentation && !c.Deleted).Select(p => p.GardenContactShowStatus.ArticleId);

                var acceptedContactPres = selectedPresentation.Where(p => selectedPresentationContactListId.Contains(p.Id)).ToList();

                presentations.AddRange(acceptedContactPres);


            }

            //var presentationContactList = db.UserContacts.Where(c => c.ContactUserId == userId);


            List<GardenPresentationViewModel> presentationViewModel = new List<GardenPresentationViewModel>();
            if (!presentations.Any())
            {
                return Ok(presentationViewModel);

            }
            foreach (var presentation in presentations.Skip(skip).Take(take))
            {
                GardenPresentationViewModel presentationModel = new GardenPresentationViewModel(presentation);

                HelperClasses.DbResponse imageResponse = null;
                foreach (var image in presentation.GardenPresentationImages.OrderBy(i => i.ImageOrder))
                {
                    if (imageResponse == null)
                    {
                        imageResponse = rc.DbGetReferencedImages(image.ImageId);
                    }
                    else
                    {
                        var currentResponse = rc.DbGetReferencedImages(image.ImageId);
                        imageResponse.ResponseObjects.AddRange(currentResponse.ResponseObjects);
                    }
                }

                presentationModel.EntryImages = new List<_HtmlImageViewModel>();
                presentationModel.ProfilUrl = GetUserProfilImg(presentation.UserId);
                var user = db.Users.Where(u => !u.Deleted && u.Id == presentationModel.AuthorId).FirstOrDefault();

                if (user != null)
                {
                    presentationModel.AuthorName = user.UserName.StartsWith("Deleted-") ? "anonym" : user.UserName.Split('@')[0];
                    //ICollection<Microsoft.AspNet.Identity.EntityFramework.IdentityUserRole> role = user.Roles;
                    //if (role.Any())
                    //{
                    //    answerVm.IsAdminAnswer = true;
                    //}
                }

                if (imageResponse == null)
                {

                }
                else if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                {
                    presentationModel.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"), order: false);
                }
                else
                {
                    presentationModel.EntryImages.Add(new _HtmlImageViewModel
                    {
                        SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                        Id = 0,
                        TitleAttr = "Kein Bild vorhanden"
                    });
                }

                presentationViewModel.Add(presentationModel);
            }


            return Ok(presentationViewModel);
        }

        public List<_HtmlImageViewModel> GetUserProfilImg(Guid userId)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            List<_HtmlImageViewModel> output = new List<_HtmlImageViewModel>();

            var userProperty = new PropertyController().DbGetProperty(userId);
            var userPropertyId = userProperty.Id;
            HelperClasses.DbResponse imageResponse = rc.DbGetProfilImgReferencedImages(userPropertyId);

            if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            {
                output = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/")).OrderByDescending(r => r.InsertDate).ToList();
            }
            return output;
        }

        [Route("api/ImageAPI/mypresentation")]
        [HttpGet]
        public IHttpActionResult GetUserPresentations()
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            var userId = Utilities.GetUserId();

            var presentations = db.GardenPresentations.Where(gp => gp.UserId == userId && !gp.Deleted);


            List<GardenPresentationViewModel> presentationViewModel = new List<GardenPresentationViewModel>();

            foreach (var presentation in presentations)
            {
                GardenPresentationViewModel presentationModel = new GardenPresentationViewModel(presentation);

                HelperClasses.DbResponse imageResponse = null;
                foreach (var image in presentation.GardenPresentationImages.OrderBy(i => i.ImageOrder))
                {
                    if (imageResponse == null)
                    {
                        imageResponse = rc.DbGetReferencedImages(image.ImageId);
                    }
                    else
                    {
                        var currentResponse = rc.DbGetReferencedImages(image.ImageId);
                        imageResponse.ResponseObjects.AddRange(currentResponse.ResponseObjects);
                    }
                }

                presentationModel.EntryImages = new List<_HtmlImageViewModel>();

                if(imageResponse == null)
                {

                }
                else if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                {
                    presentationModel.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"), order: false);
                }
                else
                {
                //    presentationModel.EntryImages.Add(new _HtmlImageViewModel
                //    {
                //        SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                //        Id = 0,
                //        TitleAttr = "Kein Bild vorhanden"
                //    });
                }

                presentationViewModel.Add(presentationModel);
            }


            return Ok(presentationViewModel);
        }

        [Route("api/ImageAPI/presentation/getcontact/{presId}")]
        [HttpGet]
        public IHttpActionResult GetContactInPresentation(int presId)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            var userId = Utilities.GetUserId();

            var presentation = db.GardenPresentations.FirstOrDefault(gp => gp.UserId == userId && gp.Id == presId && !gp.Deleted);
            if (presentation == null)
            {
                return BadRequest("No Presentation");
            }


            var contactStatusList = db.GardenContactShowStatuses.FirstOrDefault(c => c.ArticleId == presentation.Id && c.ArticleType == GardenArticleType.GardenPresentation && !c.Deleted);
            var output = new List<GardenContactList>();

            if (contactStatusList == null)
            {
                GardenContactShowStatus newContact = new GardenContactShowStatus
                {
                    ArticleId = presentation.Id,
                    ArticleType = GardenArticleType.GardenPresentation

                };
                newContact.OnCreate("system");
                db.GardenContactShowStatuses.Add(newContact);
                db.SaveChanges();
                return Ok(output);
            }

            var outputvm = contactStatusList.GardenContactLists.Where(c => !c.Deleted).ToList().Select(c => new {
                userName = c.UserName,
                userId = c.UserId
            });

            return Ok(outputvm);
        }

        [Route("api/ImageAPI/album/getcontact/{albumId}")]
        [HttpGet]
        public IHttpActionResult GetContactInAlbum(int albumId)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            var userId = Utilities.GetUserId();

            var album = db.GardenAlbums.FirstOrDefault(gp => gp.UserId == userId && gp.Id == albumId && !gp.Deleted);
            if (album == null)
            {
                return BadRequest("No Album");
            }


            var contactStatusList = db.GardenContactShowStatuses.FirstOrDefault(c => c.ArticleId == album.Id && c.ArticleType == GardenArticleType.GardenArchive && !c.Deleted);
            var output = new List<GardenContactList>();

            if (contactStatusList == null)
            {
                GardenContactShowStatus newContact = new GardenContactShowStatus
                {
                    ArticleId = album.Id,
                    ArticleType = GardenArticleType.GardenArchive

                };
                newContact.OnCreate("system");
                db.GardenContactShowStatuses.Add(newContact);
                db.SaveChanges();
                return Ok(output);
            }

            var outputvm = contactStatusList.GardenContactLists.Where(c => !c.Deleted).ToList().Select(c => new {
                userName = c.UserName,
                userId = c.UserId
            });

            return Ok(outputvm);
        }


        [Route("api/ImageAPI/findmember/{searchUserName}")]
        [HttpGet]
        public IHttpActionResult FindMember (string searchUserName)

        {
            var result = db.Users.Where(u => u.UserName.StartsWith(searchUserName)).Select(user=>user.UserName);
            return Ok(result);
        }
        [Route("api/ImageAPI/findmemberInContact/{searchUserName}")]
        [HttpGet]
        public IHttpActionResult FindMemberInContact(string searchUserName)

        {
            AccountAPIController ac = new AccountAPIController();
            var contactList = ac.GetContacts();
            var result = contactList.Where(u => u.UserName.StartsWith(searchUserName)).Select(user => user.UserName);
            return Ok(result);
        }

        [Route("api/ImageAPI/presentation/addContact/{presId}")]
        [HttpPost]
        public IHttpActionResult AddContactInPresentation(int presId, GardenContactEditView vm)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            var userId = Utilities.GetUserId();

            var presentation = db.GardenPresentations.FirstOrDefault(gp => gp.UserId == userId && gp.Id == presId && !gp.Deleted);
            if (presentation == null)
            {
                return BadRequest("No Presentation");
            }


            var contactStatusList = db.GardenContactShowStatuses.FirstOrDefault(c => c.ArticleId == presentation.Id && c.ArticleType == GardenArticleType.GardenPresentation && !c.Deleted);
            var output = new List<GardenContactList>();

            if (contactStatusList == null)
            {
                contactStatusList = new GardenContactShowStatus
                {
                    ArticleId = presentation.Id,
                    ArticleType = GardenArticleType.GardenPresentation

                };
                contactStatusList.OnCreate("system");
                db.GardenContactShowStatuses.Add(contactStatusList);
                db.SaveChanges();

                
            }

            var selectedUser = db.Users.FirstOrDefault(u => u.UserName == vm.UserName);

            if (selectedUser == null || contactStatusList == null)
            {
                return Content(HttpStatusCode.BadRequest, "Kunde existiert nicht");

            }
            
            if(contactStatusList.GardenContactLists != null && contactStatusList.GardenContactLists.Any(c => c.UserName == vm.UserName && !c.Deleted))
            {
                return Content(HttpStatusCode.BadRequest, "Dieser Kunde ist bereits in der Kontaktliste enthalten");

            }

            GardenContactList newList = new GardenContactList
            {
                GardenContactShowId = contactStatusList.Id,
                UserName = vm.UserName,
                UserId = Guid.Parse(selectedUser.Id)
            };

            newList.OnCreate("system");
            db.GardenContactLists.Add(newList);
            db.SaveChanges();

            return Ok("Success");
        }

        [Route("api/ImageAPI/presentation/removecontact/{presId}")]
        [HttpPost]
        public IHttpActionResult RemoveContactInPresentation(int presId, GardenContactEditView vm)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            var userId = Utilities.GetUserId();

            var presentation = db.GardenPresentations.FirstOrDefault(gp => gp.UserId == userId && gp.Id == presId && !gp.Deleted);
            if (presentation == null)
            {
                return BadRequest("No Presentation");
            }


            var contactStatusList = db.GardenContactShowStatuses.FirstOrDefault(c => c.ArticleId == presentation.Id && c.ArticleType == GardenArticleType.GardenPresentation && !c.Deleted);
            var output = new List<GardenContactList>();

            if (contactStatusList == null)
            {
                return BadRequest("Keine kontakt");



            }

            var deletedList = contactStatusList.GardenContactLists.FirstOrDefault(l => l.UserName == vm.UserName && !l.Deleted);

            if(deletedList == null)
            {
                return BadRequest("Keine kontakt");

            }

            deletedList.Deleted = true;
            deletedList.OnEdit("system");
            db.SaveChanges();

            return Ok("Success");
        }

        [Route("api/ImageAPI/album/addContact/{albumId}")]
        [HttpPost]
        public IHttpActionResult AddContactInAlbum(int albumId, GardenContactEditView vm)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            var userId = Utilities.GetUserId();

            //var presentation = db.GardenPresentations.FirstOrDefault(gp => gp.UserId == userId && gp.Id == presId && !gp.Deleted);
            var album = db.GardenAlbums.FirstOrDefault(ga => ga.UserId == userId && ga.Id == albumId && !ga.Deleted);
            if (album == null)
            {
                return BadRequest("No Album");
            }


            var contactStatusList = db.GardenContactShowStatuses.FirstOrDefault(c => c.ArticleId == album.Id && c.ArticleType == GardenArticleType.GardenArchive && !c.Deleted);
            var output = new List<GardenContactList>();

            if (contactStatusList == null)
            {
                contactStatusList = new GardenContactShowStatus
                {
                    ArticleId = album.Id,
                    ArticleType = GardenArticleType.GardenArchive

                };
                contactStatusList.OnCreate("system");
                db.GardenContactShowStatuses.Add(contactStatusList);
                db.SaveChanges();


            }

            var selectedUser = db.Users.FirstOrDefault(u => u.UserName == vm.UserName);

            if (selectedUser == null || contactStatusList == null)
            {
                return Content(HttpStatusCode.BadRequest, "Kunde existiert nicht");

            }

            if (contactStatusList.GardenContactLists != null && contactStatusList.GardenContactLists.Any(c => c.UserName == vm.UserName && !c.Deleted))
            {
                return Content(HttpStatusCode.BadRequest, "Dieser Kunde ist bereits in der Kontaktliste enthalten");

            }

            GardenContactList newList = new GardenContactList
            {
                GardenContactShowId = contactStatusList.Id,
                UserName = vm.UserName,
                UserId = Guid.Parse(selectedUser.Id)
            };

            newList.OnCreate("system");
            db.GardenContactLists.Add(newList);
            db.SaveChanges();

            return Ok("Success");
        }

        [Route("api/ImageAPI/album/removecontact/{albumId}")]
        [HttpPost]
        public IHttpActionResult RemoveContactInAlbum(int albumId, GardenContactEditView vm)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            var userId = Utilities.GetUserId();

            var album = db.GardenAlbums.FirstOrDefault(ga => ga.UserId == userId && ga.Id == albumId && !ga.Deleted);
            if (album == null)
            {
                return BadRequest("No Album");
            }


            var contactStatusList = db.GardenContactShowStatuses.FirstOrDefault(c => c.ArticleId == album.Id && c.ArticleType == GardenArticleType.GardenArchive && !c.Deleted);
            var output = new List<GardenContactList>();

            if (contactStatusList == null)
            {
                return BadRequest("Keine kontakt");



            }

            var deletedList = contactStatusList.GardenContactLists.FirstOrDefault(l => l.UserName == vm.UserName && !l.Deleted);

            if (deletedList == null)
            {
                return BadRequest("Keine kontakt");

            }

            deletedList.Deleted = true;
            deletedList.OnEdit("system");
            db.SaveChanges();

            return Ok("Success");
        }


        [Route("api/ImageAPI/mypresentation/{id}")]
        [HttpGet]
        public IHttpActionResult GetUserPresentationById(int id)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            var userId = Utilities.GetUserId();

            var presentations = db.GardenPresentations.Where(gp => gp.UserId == userId && gp.Id == id && !gp.Deleted);


            List<GardenPresentationViewModel> presentationViewModel = new List<GardenPresentationViewModel>();

            foreach (var presentation in presentations)
            {
                GardenPresentationViewModel presentationModel = new GardenPresentationViewModel(presentation);

                HelperClasses.DbResponse imageResponse = null;
                foreach (var image in presentation.GardenPresentationImages)
                {
                    if (imageResponse == null)
                    {
                        imageResponse = rc.DbGetReferencedImages(image.ImageId);
                    }
                    else
                    {
                        var currentResponse = rc.DbGetReferencedImages(image.ImageId);
                        imageResponse.ResponseObjects.AddRange(currentResponse.ResponseObjects);
                    }
                }

                presentationModel.EntryImages = new List<_HtmlImageViewModel>();

                //HelperClasses.DbResponse imageResponse = rc.DbGetReferencedImages(presentation);
                //album.EntryImages = new List<_HtmlImageViewModel>();
                if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                {
                    presentationModel.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                }
                else
                {
                    presentationModel.EntryImages.Add(new _HtmlImageViewModel
                    {
                        SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                        Id = 0,
                        TitleAttr = "Kein Bild vorhanden"
                    });
                }

                presentationViewModel.Add(presentationModel);
            }


            return Ok(presentationViewModel);
        }



        [Route("api/ImageAPI/album/{albumId}")]
        [HttpGet]
        public IHttpActionResult GetUserAlbumsById(int albumId)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            var userId = Utilities.GetUserId();
            var albums = db.GardenAlbums.Where(ga => ga.Id == albumId & ga.UserId == userId && !ga.Deleted).Select(ga => new GardenAlbumViewModel()
            {
                Id = ga.Id,
                Name = ga.Name,
                Description = ga.Description
            });

            if(albums == null)
            {
                return BadRequest("No album");
            }

            //var albumData = db.GardenAlbums.Where(ga => ga.UserId == userId && !ga.Deleted);

            List<GardenAlbumViewModel> albumViewModels = new List<GardenAlbumViewModel>();

            foreach (var album in albums)
            {


                HelperClasses.DbResponse imageResponse = rc.DbGetAlbumImgReferencedImages((int)album.Id);
                album.EntryImages = new List<_HtmlImageViewModel>();
                if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                {
                    album.EntryImages = Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"));
                }
                else
                {
                    album.EntryImages.Add(new _HtmlImageViewModel
                    {
                        SrcAttr = Utilities.GetBaseUrl() + "/Images/gardify_Pflanzenbild_Platzhalter.svg",
                        Id = 0,
                        TitleAttr = "Kein Bild vorhanden"
                    });
                }

                albumViewModels.Add(album);
            }


            return Ok(albumViewModels);
        }

        [Route("api/ImageAPI/allImages")]
        [HttpGet]
        public IHttpActionResult GetAllUserImages()
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();


            List<_HtmlImageViewModel> images = new List<_HtmlImageViewModel>();
            var userId = Utilities.GetUserId();
            var albums = db.GardenAlbums.Where(ga => ga.UserId == userId && !ga.Deleted).Select(ga => ga.Id).ToArray();
            foreach (var albumId in albums)
            {


                HelperClasses.DbResponse imageResponse = rc.DbGetAlbumImgReferencedImages((int)albumId);

                if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                {
                    images.AddRange(Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/")));
                }
                

                
            }


            return Ok(images);
        }

        [Route("api/ImageAPI/album")]
        [HttpPost]
        public IHttpActionResult CreateNewAlbum(GardenAlbumViewModel model)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest("Albumdetails sind unvollständig");

            var userId = Utilities.GetUserId();
            var album = new GardenAlbum()
            {
                UserId = userId,
                Name = model.Name,
                Description = model.Description
            };
            album.OnCreate(Utilities.GetUserName());
            db.GardenAlbums.Add(album);
            db.SaveChanges();

            return Ok(album.Id);
        }

        [Route("api/ImageAPI/presentation")]
        [HttpPost]
        public IHttpActionResult CreateNewPresentation(GardenPresentationCreateViewModel model)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest("Albumdetails sind unvollständig");

            var userId = Utilities.GetUserId();

            if (userId == null || userId == Guid.Empty)
            {
                return BadRequest("Bitte einloggen");
            }

            var presentation = new GardenPresentation()
            {
                UserId = userId,
                Headline = model.Headline,
                ShowHeadline = model.ShowHeadline,
                ShowPictureNumber = model.ShowPictureNumber,
                ShowMode = model.ShowMode

            };
            presentation.OnCreate(Utilities.GetUserName());
            db.GardenPresentations.Add(presentation);
            db.SaveChanges();

            if(model.ImageIdList == null)
            {
                return Ok(presentation.Id);

            }

            var imageIds = model.ImageIdList.Split(',');

            var imageOrderCount = 1;
            foreach(var imageid in imageIds)
            {
                if (imageid != "")
                {
                    GardenPresentationImage newImage = new GardenPresentationImage
                    {
                        ImageId = int.Parse(imageid),
                        GardenPresentationId = presentation.Id,
                        ImageOrder = imageOrderCount++
                    };
                    newImage.OnCreate(Utilities.GetUserName());

                    db.GardenPresentationImages.Add(newImage);

                    db.SaveChanges();
                }
                

            }

            return Ok(presentation.Id);
        }

        [Route("api/ImageAPI/presentation/addImage/{presId}/{imageId}")]
        [HttpPost]
        public IHttpActionResult AddNewPresentationImage(int presId, int imageId)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest("Albumdetails sind unvollständig");
            var userId = Utilities.GetUserId();
            var presentation = db.GardenPresentations.Where(gp => gp.UserId == userId && !gp.Deleted && gp.Id == presId).FirstOrDefault();
            if (presentation == null)
            {
                return BadRequest("Präsentation existiert nicht mehr.");
            }

            var lastImage = presentation.GardenPresentationImages.OrderBy(i => i.ImageOrder);
            var counter = 0;
            if (lastImage != null && lastImage.Any())
            {
                counter = lastImage.Last().ImageOrder + 1;
            }

            GardenPresentationImage newImage = new GardenPresentationImage
            {
                ImageId = imageId,
                GardenPresentationId = presId,
                ImageOrder = counter
            };
            newImage.OnCreate(presentation.EditedBy);

            db.GardenPresentationImages.Add(newImage);
          
            try
            {
                db.SaveChanges();

            }
            catch (Exception e)
            {
                var ex = e;
            }
            return Ok(presId);
        }

        [Route("api/ImageAPI/presentation")]
        [HttpPut]
        public IHttpActionResult EditPresentationDetails(GardenPresentationCreateViewModel model)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest("Präsentation existiert nicht mehr.");

            var userId = Utilities.GetUserId();
            var presentation = db.GardenPresentations.Where(gp => gp.UserId == userId && !gp.Deleted && gp.Id == model.Id).FirstOrDefault();
            if (presentation == null)
            {
                return BadRequest("Präsentation existiert nicht mehr.");
            }

            presentation.Headline = model.Headline;
            presentation.ShowHeadline = model.ShowHeadline;
            presentation.ShowMode = model.ShowMode;
            presentation.ShowPictureNumber = model.ShowPictureNumber;

            presentation.OnEdit(Utilities.GetUserName());
            db.SaveChanges();
            return Ok(model);
        }

        [Route("api/ImageAPI/presentation/imageorder")]
        [HttpPut]
        public IHttpActionResult EditPresentationImageOrder(GardenPresentationEditImageViewModel model)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest("Präsentation existiert nicht mehr.");

            var userId = Utilities.GetUserId();
            var presentation = db.GardenPresentations.Where(gp => !gp.Deleted && gp.Id == model.Id).FirstOrDefault();
            //var presentation = db.GardenPresentations.Where(gp => gp.UserId == userId && !gp.Deleted && gp.Id == model.Id).FirstOrDefault();

            if (presentation == null)
            {
                return BadRequest("Präsentation existiert nicht mehr.");
            }

            var images = presentation.GardenPresentationImages.ToList();
            var entryImageList = model.ImageIdList.Split(',');

            foreach (var image in images)
            {
                if (!entryImageList.ToList().Contains(image.ImageId.ToString()))
                {
                    var deletedImage = db.GardenPresentationImages.FirstOrDefault(g => g.Id == image.Id);

                    db.GardenPresentationImages.Remove(deletedImage);

                    db.SaveChanges();
                }
            }


            var imageCnt = 1;

            foreach (var imageString in model.ImageIdList.Split(','))
            {
                int imageId = int.Parse(imageString);
                var updateImage = db.GardenPresentationImages.FirstOrDefault(i => i.ImageId == imageId && i.GardenPresentationId == model.Id);

                if(updateImage == null)
                {
                    GardenPresentationImage newImage = new GardenPresentationImage
                    {
                        ImageId = imageId,
                        GardenPresentationId = model.Id,
                        ImageOrder = imageCnt++
                    };

                    db.GardenPresentationImages.Add(newImage);
                }
                else
                {
                    updateImage.ImageOrder = imageCnt++;

                    updateImage.OnEdit(Utilities.GetUserName());
                }

                try
                {
                    db.SaveChanges();

                }
                catch
                {
                    return BadRequest("Es gibt ein fehler.");

                }

            }


            return Ok("Success");
        }

        [Route("api/ImageAPI/presentation/deleteImage/{presId}/{imageId}")]
        [HttpDelete]
        public IHttpActionResult DeletePresentationImage(int presId, int imageId)
        {
            var userId = Utilities.GetUserId();
            //var presentation = db.GardenPresentations.Where(gp => !gp.Deleted && gp.Id == model.Id).FirstOrDefault();
            var presentation = db.GardenPresentations.FirstOrDefault(gp => gp.UserId == userId && !gp.Deleted && gp.Id == presId);

            if (presentation == null)
            {
                return BadRequest("Präsentation existiert nicht mehr.");
            }

            var images = presentation.GardenPresentationImages.ToList();
            var deletedImage = images.FirstOrDefault(i => i.ImageId == imageId);

            if (deletedImage == null)
            {
                return BadRequest("Bild existiert nicht mehr.");
            }
            var isOk = false;
           
            db.GardenPresentationImages.Remove(deletedImage);

            isOk = db.SaveChanges() > 0 ? true : false;
            return Ok(isOk);
        }

        [Route("api/ImageAPI/album")]
        [HttpPut]
        public IHttpActionResult EditAlbumDetails(GardenAlbumViewModel model)
        {
            //if (!ModelState.IsValid)
            //    return BadRequest("Album existiert nicht mehr.");

            var userId = Utilities.GetUserId();
            var album = db.GardenAlbums.Where(ga => ga.UserId == userId && !ga.Deleted && ga.Id == model.Id).FirstOrDefault();
            if (album == null)
                return BadRequest("Album existiert nicht mehr.");

            album.Name = model.Name;
            album.Description = model.Description;

            album.OnEdit(Utilities.GetUserName());
            db.SaveChanges();

            return Ok(model);
        }


        [Route("api/imageAPI/album/{id}")]

        //[Route("album")]
        [HttpDelete]
        public IHttpActionResult DeleteAlbum(int id)
        {
            var userId = Utilities.GetUserId();
            var album = db.GardenAlbums.Where(ga => ga.UserId == userId && !ga.Deleted && ga.Id == id).FirstOrDefault();
            if (album == null)
                return BadRequest("Album existiert nicht mehr.");

            album.Deleted = true;
            var imgs = db.GardenAlbumFileToModules.Where(ga => ga.GardenAlbumId == id);
            db.GardenAlbumFileToModules.RemoveRange(imgs);
            bool isOk = false;

            isOk= db.SaveChanges()>0? true: false;

            return Ok(isOk);
        }
        [Route("api/imageAPI/presentation/{id}")]

        //[Route("presentation/delete")]
        [HttpDelete]
        public IHttpActionResult DeletePresentation(int id)
        {
            var userId = Utilities.GetUserId();
            var pres = db.GardenPresentations.Where(ga => ga.UserId == userId && !ga.Deleted && ga.Id == id).FirstOrDefault();
            if (pres == null)
                return BadRequest("Präsentation existiert nicht mehr.");

            pres.Deleted = true;

            bool isOk = false;

            isOk = db.SaveChanges() > 0 ? true : false;

            return Ok(isOk);
        }

        [HttpPost]
        [Route("api/ImageAPI/upload")]
        public IHttpActionResult UploadGardenImage()
        {
            //if (Utilities.ActionAllowed(UserAction.NewGardenImage) == FeatureAccess.NotAllowed)
            //    return Unauthorized();

            HttpPostedFile imageFile = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;
            var imageMetadata = new ImageMetadataViewModel();

            imageMetadata.ImageId = Convert.ToInt32(HttpContext.Current.Request.Params["id"]);
            imageMetadata.Title = HttpContext.Current.Request.Params["imageTitle"];
            imageMetadata.Description = HttpContext.Current.Request.Params["imageDescription"];
            imageMetadata.Note = HttpContext.Current.Request.Params["imageNote"];
            imageMetadata.Tags = HttpContext.Current.Request.Params["imageTags"];
            DateTime imageCreatedDate;
            if (!DateTime.TryParse(HttpContext.Current.Request.Params["imageCreatedDate"].Replace("_", "-"), out imageCreatedDate))
            {
                imageCreatedDate = DateTime.Now;
            }
            imageMetadata.UserCreatedDate = imageCreatedDate;

            HttpPostedFileBase filebase = new HttpPostedFileWrapper(imageFile);

            GardenController gc = new GardenController();

            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            var isOk = gc.UploadAlbumImageWithId(filebase, imageFile, imageMetadata);

            if (isOk != 0)
            {
                return Ok(isOk);
            }
            return BadRequest("Details sind unvollständig");

        }

        [HttpPost]
        [Route("api/ImageAPI/presentation/upload")]
        public IHttpActionResult UploadPresentationImage()
        {
            //if (Utilities.ActionAllowed(UserAction.NewGardenImage) == FeatureAccess.NotAllowed)
            //    return Unauthorized();

            HttpPostedFile imageFile = HttpContext.Current.Request.Files.Count > 0 ? HttpContext.Current.Request.Files[0] : null;
            var imageMetadata = new ImageMetadataViewModel();

            imageMetadata.ImageId = 0;
            imageMetadata.Title = HttpContext.Current.Request.Params["imageTitle"];
            imageMetadata.Description = HttpContext.Current.Request.Params["imageDescription"];
            imageMetadata.Note = HttpContext.Current.Request.Params["imageNote"];
            imageMetadata.Tags = HttpContext.Current.Request.Params["imageTags"];
            DateTime imageCreatedDate;

            var dateText = HttpContext.Current.Request.Params["imageCreatedDate"];
            if (dateText == null || !DateTime.TryParse(dateText.Replace("_", "-"), out imageCreatedDate))
            {
                imageCreatedDate = DateTime.Now;
            }
            imageMetadata.UserCreatedDate = imageCreatedDate;

            HttpPostedFileBase filebase = new HttpPostedFileWrapper(imageFile);

            GardenController gc = new GardenController();

            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
            var isOk = gc.UploadPresentationImageWithId(filebase, imageFile, imageMetadata);

            if (isOk != 0)
            {
                return Ok(isOk);
            }
            return BadRequest("Details sind unvollständig");

        }


        [Route("api/ImageAPI/fav/{imageId}")]
        [HttpPut]
        public IHttpActionResult ToggleFavoriteImage(int imageId)
        {
            var userId = Utilities.GetUserId();
            if (userId == Guid.Empty)
                return BadRequest("Bitte melde dich zuerst an");

            var prevState = db.FavoriteGardenImages.Where(f => f.UserId == userId && f.FileToModuleID == imageId).FirstOrDefault();
            // if user already faved image, delete entry
            if (prevState != null)
            {
                db.FavoriteGardenImages.Remove(prevState);
            } else
            {
                var entry = new FavoriteGardenImage() { FileToModuleID = imageId, UserId = userId };
                db.FavoriteGardenImages.Add(entry);
            }

            db.SaveChanges();

            return Ok(imageId);
        }
        [Route("api/ImageAPI/image/{imageId}")]
        [HttpGet]
        public IHttpActionResult GetImgById(int imageId)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            if (imageId == 0 )
                return BadRequest("Albumdetails sind unvollständig");
            HelperClasses.DbResponse imageResponse = rc.DbGetReferencedImages((int)imageId);
           _HtmlImageViewModel image = new _HtmlImageViewModel();
            if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
            {
                image=Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/")).FirstOrDefault();
                if(image!=null)
                {
                    var props = db.GardenAlbumFileToModules.Where(fm => fm.FileToModuleID == imageId).FirstOrDefault();
                    if (props != null)
                    {
                        var imgProperties = props;
                        var res = new { image, imgProperties };
                        return Ok(res);
                    }
                    else
                    {
                        var imgProperties = db.GardenPresiFileToModules.Where(fm => fm.FileToModuleID == imageId).FirstOrDefault();
                        var res = new { image, imgProperties };
                        return Ok(res);
                    }

                }
            }

            return null;

            
           
           
        }

        [Route("api/ImageAPI/{imageId}/{albumId}")]
        [HttpPost]
        public IHttpActionResult AddImageToAlbum(int imageId, int albumId, GardenAlbumFileToModuleViewModel data)
        {
            if (imageId == 0 || albumId == 0)
                return BadRequest("Albumdetails sind unvollständig");

            var userId = Utilities.GetUserId();
            var album = db.GardenAlbums.Where(ga => ga.UserId == userId && !ga.Deleted && ga.Id == albumId).FirstOrDefault();
            var sameOwner = album != null && album.UserId != userId;
            if (sameOwner)
               return BadRequest("Nicht erlaubt");
            if(data != null)
            {
                var entry = new GardenAlbumFileToModule()
                {
                    GardenAlbumId = albumId,
                    FileToModuleID = imageId,
                    Headline=data.Headline,
                    Location=data.Location,
                    UserCreatedDate=data.UserCreatedDate,
                    AlternativeDate=data.AlternativeDate,
                    ImgOwner=data.ImgOwner,
                    Source=data.Source,
                    Rating=data.Rating,
                    Tags=data.Tags
                };
                
                try
                {
                    entry.OnCreate(Utilities.GetUserName());
                    db.GardenAlbumFileToModules.Add(entry);
                    db.SaveChanges();
                }
                catch (DbEntityValidationException e)
                {
                    foreach (var eve in e.EntityValidationErrors)
                    {
                        Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                            eve.Entry.Entity.GetType().Name, eve.Entry.State);
                        foreach (var ve in eve.ValidationErrors)
                        {
                            Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                ve.PropertyName, ve.ErrorMessage);
                        }
                    }
                    throw;
                }
                
            }
           

            return Ok();
        }

        [Route("api/ImageAPI/{imageId}/{presiId}/addImageToPresi")]
        [HttpPost]
        public IHttpActionResult AddImageToPresi(int imageId, int presiId, GardenAlbumFileToModuleViewModel data)
        {
            if (imageId == 0 || presiId == 0)
                return BadRequest("Albumdetails sind unvollständig");

            var userId = Utilities.GetUserId();
            var presi = db.GardenPresentations.Where(pre => pre.UserId == userId && !pre.Deleted && pre.Id == presiId).FirstOrDefault();
            var sameOwner = presi != null && presi.UserId == userId;

            if (!sameOwner)
                return BadRequest("Nicht erlaubt");

            var entry = new GardenPresiFileToModule();
            entry = new GardenPresiFileToModule()
            {
                GardenPresiId = presiId,
                FileToModuleID = imageId
            };
            if (data == null)
            {
                var imageProp = db.GardenAlbumFileToModules.Where(alb => alb.FileToModuleID == imageId && !alb.Deleted).FirstOrDefault();

                if (imageProp != null)
                {
                    entry.Headline = imageProp.Headline;
                    entry.Location = imageProp.Location;
                    entry.UserCreatedDate = imageProp.UserCreatedDate;
                    entry.AlternativeDate = imageProp.AlternativeDate;
                    entry.ImgOwner = imageProp.ImgOwner;
                    entry.Rating = imageProp.Rating;
                    entry.Source = imageProp.Source;
                    entry.Tags = imageProp.Tags;

                }
            }
            else
            {
                entry.Headline = data.Headline;
                entry.Location = data.Location;
                entry.UserCreatedDate = data.UserCreatedDate;
                entry.AlternativeDate = data.AlternativeDate;
                entry.ImgOwner = data.ImgOwner;
                entry.Rating = data.Rating;
                entry.Source = data.Source;
                entry.Tags = data.Tags;
            }
            try
            {
                entry.OnCreate(Utilities.GetUserName());
                db.GardenPresiFileToModules.Add(entry);
                db.SaveChanges();
            }
            catch (DbEntityValidationException e)
            {
                foreach (var eve in e.EntityValidationErrors)
                {
                    Console.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                        eve.Entry.Entity.GetType().Name, eve.Entry.State);
                    foreach (var ve in eve.ValidationErrors)
                    {
                        Console.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                            ve.PropertyName, ve.ErrorMessage);
                    }
                }
                throw;
            }
            

            return Ok();
        }
        [Route("api/ImageAPI")]

        [HttpPut]
        public IHttpActionResult UpdateImageDetails(UpdateImageViewModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Details sind unvollständig");

            var ftm = (from r in nfiles.FileToModule
                                where r.FileToModuleID == model.Id && (r.ModuleID == (int)ModelEnums.ReferenceToModelClass.AlbumImage || r.ModuleID == (int)ModelEnums.ReferenceToModelClass.PresentationImage)
                       select r).FirstOrDefault();

            if (ftm == null)
            {
                return NotFound();
            }

            var imageFile = (from fs in nfiles.Files
                            where fs.FileID == ftm.FileID
                            select fs).FirstOrDefault();

            ftm.AltText = model.imageTitle;
            imageFile.DescriptionDE = string.IsNullOrEmpty(model.Description) ? imageFile.DescriptionDE : model.Description;
            ftm.Description = imageFile.DescriptionDE;
            imageFile.TagsDE = string.IsNullOrEmpty(model.Tags) ? imageFile.TagsDE : model.Tags; //TODO: clear all TagsDE fields before live - not used
            imageFile.UserCreatedDate = model.TakenDate == null ? imageFile.UserCreatedDate : model.TakenDate;
            imageFile.FileE = string.IsNullOrEmpty(model.Note) ? imageFile.FileE : model.Note;
          

            nfiles.SaveChanges();
            return Ok();
        }

        [Route("api/ImageAPI/{imageId}/{albumId}")]
        [HttpDelete]
        public IHttpActionResult RemoveImageFromAlbum(int imageId, int albumId)
        {
            var userId = Utilities.GetUserId();
            var album = db.GardenAlbums.Where(ga => ga.UserId == userId && !ga.Deleted && ga.Id == albumId).FirstOrDefault();
            var sameOwner = album != null && album.UserId == userId;
            if (!sameOwner)
                return BadRequest("Nicht erlaubt");

            var entry = db.GardenAlbumFileToModules.Where(ga => ga.FileToModuleID == imageId && ga.GardenAlbumId == albumId && !ga.Deleted).FirstOrDefault();
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();

            var res = rc.DbDeleteFile(imageId, albumId);
            var isOk = false;
            
            if (entry != null)
            {
                db.GardenAlbumFileToModules.Remove(entry);
              isOk=  db.SaveChanges() > 0 ? true : false;
                return Ok(isOk);

            }

            return Ok(res);
        }

        [Route("api/ImageAPI/filterImages")]
        [HttpGet]
        public IHttpActionResult FilterAlbumImgByDate(DateTime inpuDate,string filterMode)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();


            List<_HtmlImageViewModel> albumImages = new List<_HtmlImageViewModel>();
            var userId = Utilities.GetUserId();
            //HelperClasses.DbResponse imageResponse = rc.DbGetAllAlbumImgReferencedImages();

            var albums = db.GardenAlbums.Where(ga => ga.UserId == userId && !ga.Deleted).Select(ga => ga.Id).ToArray();
            foreach (var a in albums)
            {
                var albumFileToModuleIds = db.GardenAlbumFileToModules.Where(fm => fm.GardenAlbumId == a).Select(m => m.FileToModuleID);
                foreach (var aId in albumFileToModuleIds)
                {
                    HelperClasses.DbResponse imageResponse = rc.DbGetAlbumImgReferencedImages((int)aId);
                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                    {
                        albumImages.Add(Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/"))[0]);
                    }

                }
            }
            if (albumImages != null && albumImages.Any())
                if (filterMode == "year" && inpuDate!=null)
                {
                    var filteredRes = albumImages.Where(a => a.InsertDate.Year == inpuDate.Year).ToList();
                     var res = filteredRes.GroupBy(img => img.InsertDate.Month).ToList();
                    return Ok(res);
                }
                else if (filterMode == "month" && inpuDate != null)
                {
                    var filteredRes = albumImages.Where(a => a.InsertDate.Month == inpuDate.Month).ToList();
                    var res = filteredRes.GroupBy(img => img.InsertDate.Day).ToList();
                    return Ok(res);
                }
               
            return Ok();
        }

        [Route("api/ImageAPI/sortImages")]
        [HttpGet]
        public IHttpActionResult SortImgFromAlbums(string sortType)
        {
            ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();


            List<_HtmlImageViewModel> albumImages = new List<_HtmlImageViewModel>();
            var userId = Utilities.GetUserId();
            //HelperClasses.DbResponse imageResponse = rc.DbGetAllAlbumImgReferencedImages();
           
            var albums = db.GardenAlbums.Where(ga => ga.UserId == userId && !ga.Deleted).Select(ga => ga.Id).ToArray();
            foreach (var a in albums)
            {
              
                    HelperClasses.DbResponse imageResponse = rc.DbGetAlbumImgReferencedImages((int)a);
                    if (imageResponse.ResponseObjects != null && imageResponse.ResponseObjects.Any())
                    {
                        albumImages.AddRange(Utilities.getHtmlImageObjectsFromDbImageResponse(imageResponse, Utilities.GetAbsolutePath("~/")));
                    }
                   
                
            }
            if (albumImages!=null && albumImages.Any())
            if (sortType == "year")
            {
                var res= albumImages.GroupBy(img => img.InsertDate.Year).ToList();
                return Ok(res);
            }
            else if (sortType == "day")
            {
                var res = albumImages.GroupBy(img => img.InsertDate.Day).ToList();
                    return Ok(res);
            } 
            else if( sortType=="month"){
                var res = albumImages.GroupBy(img => img.InsertDate.Month).ToList();
                return Ok(res);
            }
            return Ok();
        }

        [Route("api/ImageAPI/deleteImageGlobal")]
        [HttpPost]
        public IHttpActionResult DeleteImgFromGardenArchiv( int[] imagesIds)
        {
            if(imagesIds!=null && imagesIds.Length > 0)
            {
                foreach(var imageId in imagesIds)
                {
                    ReferenceToFileSystemObjectController rc = new ReferenceToFileSystemObjectController();
                    // remove in GardenAlbumFileToModules
                    var entriesInAlbumFileToModule = db.GardenAlbumFileToModules.Where(ga => ga.FileToModuleID == imageId && !ga.Deleted);
                    if (entriesInAlbumFileToModule != null && entriesInAlbumFileToModule.Any())
                    {
                        foreach (var f in entriesInAlbumFileToModule)
                        {
                            db.GardenAlbumFileToModules.Remove(f);
                        }
                    }
                    // remove in GardenPresiFileToModules
                    var entriesInPresiFiles = db.GardenPresiFileToModules.Where(ga => ga.FileToModuleID == imageId && !ga.Deleted);
                    var entriesInPresiImages = db.GardenPresentationImages.Where(ga => ga.ImageId == imageId && !ga.Deleted);
                    if (entriesInPresiFiles != null && entriesInPresiFiles.Any())
                    {
                        foreach (var f in entriesInPresiFiles)
                        {
                            db.GardenPresiFileToModules.Remove(f);
                        }
                    }
                    if (entriesInPresiImages != null && entriesInPresiImages.Any())
                    {
                        foreach (var f in entriesInPresiImages)
                        {
                            db.GardenPresentationImages.Remove(f);
                        }
                    }
                    var res = rc.DbDeleteFile(imageId, 0);
                }
            }
                
            var isOk = false;
                isOk = db.SaveChanges() > 0 ? true : false;
            

            return Ok(isOk);
        }
       
    }
}