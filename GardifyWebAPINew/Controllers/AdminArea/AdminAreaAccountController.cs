using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using GardifyModels.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace GardifyWebAPI.Controllers.AdminArea
{
    public class AdminAreaAccountController : _BaseController
    {
        ApplicationDbContext ctx;

        public AdminAreaAccountController() : base()
        {
            ctx = new ApplicationDbContext();
        }

        private AspNetUserManager _userManager;
        public ActionResult Create()
        {
            var acvm = GetCreateUserViewModel();
            return View("~/Views/AdminArea/AdminAreaAccount/Create.cshtml", acvm);
        }

        public ActionResult Delete(Guid id)
        {
            return View("~/Views/AdminArea/AdminAreaAccount/Delete.cshtml", GetDeleteUserViewModel(id));
        }

        public DeleteUserViewModel GetDeleteUserViewModel(Guid id)
        {
            var user = DbGetUser(id);
            DeleteUserViewModel vm = new DeleteUserViewModel()
            {
                Email = user.Email,
                Id = new Guid(user.Id)
            };
            return vm;
        }

        [HttpPost]
        public ActionResult Delete(DeleteUserViewModel vm)
        {
            if (DbDeleteUser(vm.Id))
            {
                return RedirectToAction("Index");
            }
            else
            {
                throw new Exception("Der Benutzer mit ID: " + vm.Id + " konnte nicht gelöscht werden");//, HttpStatusCode.InternalServerError, "AdminAreaAccountController.Delete("+vm.Id+")");
            }
        }

        public ActionResult Edit(Guid id)
        {
            return View("~/Views/AdminArea/AdminAreaAccount/Edit.cshtml", GetUserEditViewModel(id));
        }

        public EditUserViewModel GetUserEditViewModel(Guid userId)
        {
            var user = DbGetUser(userId);
            List<UserRoleCheckedViewModel> roles = DbGetRoleListWithCheckedByUser(userId).ToList();
            EditUserViewModel vm = new EditUserViewModel()
            {
                Id = new Guid(user.Id),
                Email = user.Email,
                Roles = roles
            };
            return vm;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(EditUserViewModel vm)
        {
            if (DbEditUser(vm))
            {
                return Details(vm.Id);
            }
            else
            {
                throw new Exception("Bearbeitung von Benutzer " + vm.Id + " ist fehlgeschlagen");//, HttpStatusCode.InternalServerError, "AdminAreaAccountController.Edit("+vm.Id+")");
            }            
        }

        [HttpPost]
        public async Task<ActionResult> Create(CreateUserViewModel vm)
        {
            Guid succeeded = await DbAddUser(vm);
            if (succeeded != null)
            {
                return View("~/Views/AdminArea/AdminAreaAccount/Details.cshtml", GetDetailsUserViewModel(succeeded));
            }
            else
            {
                return View(vm);
            }
        }

        public ActionResult Details(Guid id)
        {
            return View("~/Views/AdminArea/AdminAreaAccount/Details.cshtml", GetDetailsUserViewModel(id));
        }

        public ActionResult Index()
        {
            var listOfViewModels = DbGetUserDetailsViewList();
            IndexUserViewModel vm = new IndexUserViewModel()
            {
                UserList = listOfViewModels
            };
            return View("~/Views/AdminArea/AdminAreaAccount/Index.cshtml", vm);
        }

        public CreateUserViewModel GetCreateUserViewModel()
        {
            CreateUserViewModel vm = new CreateUserViewModel()
            {
                Roles = DbGetUserViewRoles()
            };
            return vm;
        }

        public DetailsUserViewModel GetDetailsUserViewModel(Guid id)
        {
            var user = DbGetUser(id);
            List<string> roles = new List<string>();
            foreach (var role in user.Roles)
            {
                roles.Add(DbGetUserRoleName(new Guid(role.RoleId)));
            }
            DetailsUserViewModel vm = new DetailsUserViewModel()
            {
                Id = id,
                Email = user.Email,
                RoleNames = roles
            };
            return vm;
        }
        #region DB
        [NonAction]
        public bool DbDeleteUser(Guid id)
        {
            var user = DbGetUser(id);
            user.Deleted = true;
            return ctx.SaveChanges() > 0;            
        }

        [NonAction]
        public bool DbEditUser(EditUserViewModel vm)
        {
            var store = new UserStore<ApplicationUser>(new ApplicationDbContext());
            var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var um = new UserManager<ApplicationUser>(store);
            ApplicationUser user = um.FindById(vm.Id.ToString());
            if (!user.Deleted)
            {
                user.Email = vm.Email;
                um.Update(user);
                var roles = user.Roles.ToList();
                foreach (var role in roles)
                {
                    IdentityRole currentRole = rm.FindById(role.RoleId);
                    um.RemoveFromRole(user.Id, currentRole.Name);
                }
                foreach (var role in vm.Roles)
                {
                    if (role.Checked == true)
                    {
                        um.AddToRole(user.Id, role.Name);
                    }
                }
                return true;
            }
            return false;
        }

        [NonAction]
        ///<summary>
        ///Returns the list of all roles
        ///the roles the user is part of, are checked
        /// </summary>
        public IEnumerable<UserRoleCheckedViewModel> DbGetRoleListWithCheckedByUser(Guid userId)
        {
            ApplicationUser user = DbGetUser(userId);
            IEnumerable<UserRoleCheckedViewModel>  roles = DbGetUserViewRoles();
            foreach(UserRoleCheckedViewModel role in roles)
            {
                if(checkUserIsInRole(user.Id.ToString(), role.Id.ToString()))
                {
                    role.Checked = true;
                }
            }
            return roles;
        }
        [NonAction]
        public bool checkUserIsInRole(string userId, string roleId)
        {
            var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            ApplicationUser user = um.FindById(userId);
            if (!user.Deleted)
            {
                IdentityRole role = rm.FindById(roleId);
                return um.IsInRole(user.Id, role.Name);
            }
            return false;
        }

        [NonAction]
        public bool setUserRole(string userId, string roleId)
        {
            var rm = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
            var um = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()));
            ApplicationUser user = um.FindById(userId);
            if (!user.Deleted)
            {
                IdentityRole role = rm.FindById(roleId);
                if (!um.IsInRole(user.Id, role.Id))
                {
                    var userResult = um.AddToRole(user.Id, role.Name);
                    return userResult.Succeeded;
                }
                return true;
            }
            return false;
        }
        [NonAction]
        public string DbGetUserRoleName(Guid roleId)
        {
            string inputId = roleId.ToString();
            var role = (from rol in ctx.Roles
                        where rol.Id == inputId
                        select rol).FirstOrDefault();
            return role.Name;
        }

        [NonAction]
        public ApplicationUser DbGetUser(Guid id)
        {
            string guidString = id.ToString();
            var user = (from us in ctx.Users
                        where us.Id == guidString
                        && !us.Deleted
                        select us).FirstOrDefault();
            return user;
        }

        [NonAction]
        public IEnumerable<UserRoleCheckedViewModel> DbGetUserViewRoles()
        {
            List<UserRoleCheckedViewModel> list = new List<UserRoleCheckedViewModel>();
            var userRoles = (from t in ctx.Roles
                             select t);
            foreach (var role in userRoles)
            {
                UserRoleCheckedViewModel vm = new UserRoleCheckedViewModel()
                {
                    Checked = false,
                    Id = new Guid(role.Id),
                    Name = role.Name
                };
                list.Add(vm);
            }
            return list;
        }
        [NonAction]
        public IEnumerable<DetailsUserViewModel> DbGetUserDetailsViewList()
        {
            List<DetailsUserViewModel> list = new List<DetailsUserViewModel>();
            var users = (from t in ctx.Users
                         where !t.Deleted
                         select t);
            foreach (var role in users)
            {
                DetailsUserViewModel vm = new DetailsUserViewModel()
                {
                    Email = role.Email,
                    Id = new Guid(role.Id)
                };
                list.Add(vm);
            }
            return list;
        }
        [NonAction]
        public async Task<Guid> DbAddUser(CreateUserViewModel vm)
        {
            var user = new ApplicationUser { UserName = vm.Email, Email = vm.Email };
            var result = await UserManager.CreateAsync(user, vm.Password);
            if (result.Succeeded)
            {
                BetaKeyController bkc = new BetaKeyController();
                bkc.DbAddBetaKeys(new Guid(user.Id));
            }
            foreach(var role in vm.Roles)
            {
                setUserRole(user.Id, role.Id.ToString());
            }
            return new Guid(user.Id);
        }


        public AspNetUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<AspNetUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        #endregion
    }
}