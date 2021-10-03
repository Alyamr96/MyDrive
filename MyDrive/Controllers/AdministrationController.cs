using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using MyDrive.Models;
using MyDrive.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MyDrive.Controllers
{
    public class AdministrationController : Controller
    {
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _context;
        static readonly List<string> Permissions = new List<String>{"Create User" ,"View Users", "Delete User", "Assign Users To Companies", "View All Uploaded Files", "Delete File", "Upload File", "Delete Company", "Delete Folder", "Create Folder", "Delete Multiple FoldersAndFiles", "View Roles", "Create Role", "Edit Role", "Delete Role", "View Filters", "Manage Companies"};
        //static List<string> AddedPermissionsToNewRole = new List<string>();
        //List<string> RemovedPermissionsToNewRole = new List<string>();

        public AdministrationController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public AdministrationController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }
        [HttpGet]
        public ActionResult CreateRole()
        {
            var viewModel = new CreateRoleViewModel { Permissions = Permissions };
            return View(viewModel);
        }
        [HttpPost]
        public async Task<ActionResult> CreateRolePost(CreateRoleViewModel model)
        {
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            if (ModelState.IsValid)
            {
                IdentityRole identityRole = new IdentityRole
                {
                    Name = model.RoleName
                };
                IdentityResult result = await roleManager.CreateAsync(identityRole);
                if (result.Succeeded)
                {
                    return RedirectToAction("ListRoles", "Administration");
                }
                else
                {
                    var viewModel1 = new CreateRoleViewModel { RoleName = model.RoleName, Flag = true };
                    return View("CreateRole", viewModel1);
                }
            }

            return View("CreateRole",model);
        }
        [HttpGet]
        public ActionResult ListRoles()
        {
            var userId = User.Identity.GetUserId();
            ApplicationUser user1 = UserManager.FindById(userId);
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var roles = roleManager.Roles.ToList();
            string RolesUserAssignedTo = "";
            var PermissionRecords = _context.RolePermissions.ToList();
            List<string> UserPermissions = new List<string>();
            foreach (var role in roles)
            {
                if (UserManager.IsInRole(userId, role.Name) == true)
                    RolesUserAssignedTo = role.Id;
            }
            foreach (var record in PermissionRecords)
            {
                if (record.RoleId == RolesUserAssignedTo)
                    UserPermissions.Add(record.PermissionName);
            }
            var viewModel = new ListRolesViewModel { Roles = roles, UserPermissions = UserPermissions };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ConfirmPasswordForRoleCreate(ListRolesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user1 = UserManager.FindById(userId);
                try
                {
                    var result = UserManager.CheckPassword(user1, viewModel.Password);
                    if (result)
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (System.ArgumentNullException e)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ConfirmPasswordForRoleDelete(ListRolesViewModel viewModel, string id)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user1 = UserManager.FindById(userId);
                try
                {
                    var result = UserManager.CheckPassword(user1, viewModel.Password);
                    if (result)
                    {
                        var records = _context.RolePermissions.ToList();
                        foreach (var record in records)
                        {
                            if (record.RoleId == id)
                            {
                                _context.RolePermissions.Remove(record);
                            }
                        }
                        _context.SaveChanges();
                        var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
                        var roleManager = new RoleManager<IdentityRole>(roleStore);
                        var role = roleManager.FindById(id);
                        roleManager.Delete(role);
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (System.ArgumentNullException e)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ConfirmPasswordForRoleEdit(ListRolesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user1 = UserManager.FindById(userId);
                try
                {
                    var result = UserManager.CheckPassword(user1, viewModel.Password);
                    if (result)
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (System.ArgumentNullException e)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }

        [Route("Administration/EditRole/{roleId}")]
        public ActionResult EditRole2(string roleId)
        {
            roleId = roleId.Replace(';', '-');
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var role = roleManager.FindById(roleId);
            List<string> RolePermissions = new List<string>();
            var records = _context.RolePermissions.ToList();
            foreach(var record in records)
            {
                if (record.RoleId == role.Id)
                    RolePermissions.Add(record.PermissionName);
            }
            var viewModel = new EditRole2ViewModel {RoleId = role.Id, RoleName = role.Name, Permissions = Permissions, PermissionsAddedToRole = RolePermissions, Flag= false };
            return View(viewModel);
        }

        [Route("Administration/AssignPermissionToRole/{permission}/{roleId}")]
        public ActionResult AssignPermissionToRole(string permission, string roleId)
        {
            roleId = roleId.Replace(';', '-');
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var role = roleManager.FindById(roleId);
            RolePermissions myRecord = new RolePermissions {RoleId = role.Id, PermissionName = permission };
            _context.RolePermissions.Add(myRecord);
            _context.SaveChanges();
            List<string> RolePermissions = new List<string>();
            var records = _context.RolePermissions.ToList();
            foreach (var record in records)
            {
                if (record.RoleId == role.Id)
                    RolePermissions.Add(record.PermissionName);
            }
            var viewModel = new EditRole2ViewModel { RoleId = role.Id, RoleName = role.Name, Permissions = Permissions, PermissionsAddedToRole = RolePermissions, Flag = false };
            return View("EditRole2",viewModel);
        }

        [Route("Administration/RemovePermissionFromRole/{permission}/{roleId}")]
        public ActionResult RemovePermissionFromRole(string permission, string roleId)
        {
            roleId = roleId.Replace(';', '-');
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var role = roleManager.FindById(roleId);
            List<string> RolePermissions = new List<string>();
            var records = _context.RolePermissions.ToList();
            foreach (var record in records)
            {
                if (record.RoleId == role.Id && record.PermissionName == permission)
                {
                    _context.RolePermissions.Remove(record);
                }
                if (record.RoleId == role.Id && record.PermissionName != permission)
                    RolePermissions.Add(record.PermissionName);
            }
            _context.SaveChanges();
            var viewModel = new EditRole2ViewModel { RoleId = role.Id, RoleName = role.Name, Permissions = Permissions, PermissionsAddedToRole = RolePermissions, Flag = false };
            return View("EditRole2", viewModel);
        }

        [HttpPost]
        public ActionResult EditRolePost(EditRole2ViewModel viewModel)
        {
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var roles = roleManager.Roles.ToList();
            foreach(var role1 in roles)
            {
                if (role1.Id == viewModel.RoleId.Replace(';', '-')) 
                {
                    role1.Name = viewModel.RoleName;
                    _context.SaveChanges();
                }
            }
            var role = roleManager.FindById(viewModel.RoleId.Replace(';','-'));
            role.Name = viewModel.RoleName;
            _context.SaveChanges();
            //return Content(viewModel.RoleName + "//" + role.Name);
            return RedirectToAction("ListRoles");
        }

        [Route("Administration/UserRoles")]
        public ActionResult UserRole()
        {
            List<UserAndRole> UsersInRoles = new List<UserAndRole>();
            List<UserAndRole> UsersNotInRoles = new List<UserAndRole>();
            var users = _context.Users.ToList();
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var roles = roleManager.Roles.ToList();
            foreach(var user in users)
            {
                IdentityRole Role1 = null;
                int count = 0;
                foreach (var role in roles)
                {
                    Role1 = role;
                    if (UserManager.IsInRole(user.Id, role.Name) == true)
                    {
                        UsersInRoles.Add(new UserAndRole {User = user, Role = role });
                        count++;
                    }
                }
                if (count == 0)
                    UsersNotInRoles.Add(new UserAndRole { User = user, Role = Role1 });
            }
            var viewModel = new UserAndRoleViewModel {UsersWithRoles = UsersInRoles, UsersWithoutRoles = UsersNotInRoles };
            return View(viewModel);
        }

        [Route("Administration/AssignUserToRole/{userId}")]
        public ActionResult AssignUserToRole(string userId)
        {
            userId = userId.Replace(';', '-');
            string userIdNormalized = userId.Replace('-', ';');
            ViewBag.userId = userIdNormalized;
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var roles = roleManager.Roles.ToList();
            List<IdentityRole> RolesUserNotAssignedTo = new List<IdentityRole>();
            foreach(var role in roles)
            {
                if (UserManager.IsInRole(userId, role.Name) == false)
                    RolesUserNotAssignedTo.Add(role);
            }
            return View(RolesUserNotAssignedTo);
        }

        [Route("Administration/AssignUserToRole1/{userId}/{Name}")]
        public ActionResult AssignUserToRole1(string userId, string Name)
        {
            UserManager.AddToRole(userId.Replace(';','-'), Name);
            return RedirectToAction("UserRole");
        }

        [Route("Administration/RemoveUserFromRole1/{userIdNormalized}/{Name}")]
        public ActionResult RemoveUserFromRole1(string userIdNormalized, string Name)
        {
            UserManager.RemoveFromRole(userIdNormalized.Replace(';', '-'), Name);
            return RedirectToAction("UserRole");
        }

        public ActionResult ReturnNavBar()
        {
            var userId = User.Identity.GetUserId();
            ApplicationUser user1 = UserManager.FindById(userId);
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var roles = roleManager.Roles.ToList();
            string RolesUserAssignedTo = "";
            var PermissionRecords = _context.RolePermissions.ToList();
            List<string> UserPermissions = new List<string>();
            foreach (var role in roles)
            {
                try
                {
                    if (UserManager.IsInRole(userId, role.Name) == true)
                        RolesUserAssignedTo = role.Id;
                }
                catch(Exception e)
                {
                    FoldersandFilesViewModel vm1 = new FoldersandFilesViewModel { UserPermissions = UserPermissions };
                    return PartialView(vm1);
                }
            }
            foreach (var record in PermissionRecords)
            {
                if (record.RoleId == RolesUserAssignedTo)
                    UserPermissions.Add(record.PermissionName);
            }
            FoldersandFilesViewModel vm = new FoldersandFilesViewModel {UserPermissions = UserPermissions };
            return PartialView(vm);
        }

        /*public ActionResult ClickMe()
        {
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            //var roles = roleManager.Roles;
            var userId = User.Identity.GetUserId();
            ApplicationUser user1 = UserManager.FindById(userId);
            UserManager.AddToRole(user1.Id, "CanDoEverything");
            _context.SaveChanges();
            return RedirectToAction("ListRoles");
        }*/

        /*public async Task<ActionResult> EditRole(string id)
        {
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var role = await roleManager.FindByIdAsync(id);
            List<ApplicationUser> MyUsers = new List<ApplicationUser>();
            var model = new EditRoleViewModel
            {
                Id = role.Id,
                RoleName = role.Name
            };
            foreach(var user in UserManager.Users)
            {
                MyUsers.Add(user);
            }
            foreach (var user in MyUsers)
            {
               var result = await UserManager.IsInRoleAsync(user.Id, role.Id);
               if (result == true)
                {
                    model.Users.Add(user.Email);
                }
            }
            return View(model);
        }*/
        /*[Route("Administration/EditUsersInRole/{roleId}")]
        public async Task<ActionResult> EditUsersInRoleAsync(string roleId)
        {
            ViewBag.roleId = roleId;
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var role = await roleManager.FindByIdAsync(roleId);
            var model = new List<UserRoleViewModel>();
            List<ApplicationUser> MyUsers = new List<ApplicationUser>();
            foreach (var user in UserManager.Users)
            {
                MyUsers.Add(user);
            }
            foreach (var user in MyUsers)
            {
                var userRoleViewModel = new UserRoleViewModel
                {
                    UserId = user.Id,
                    UserName = user.Email
                };
                if (await UserManager.IsInRoleAsync(user.Id, role.Name))
                {
                    userRoleViewModel.IsSelected = true;
                }
                else
                    userRoleViewModel.IsSelected = false;
                model.Add(userRoleViewModel);
            }
            return View(model);
        }*/
        // shows all users who are not in role
        /*public ActionResult EditUsersInRole()
        {
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var users = UserManager.Users;
            List<ApplicationUser> myUsers = new List<ApplicationUser>();
            List<ApplicationUser> usersToShow = new List<ApplicationUser>();
            foreach(var user in users)
            {
               myUsers.Add(user);
            }
            foreach(var user in myUsers)
            {
                if (UserManager.IsInRole(user.Id, "DeleteFolders") == false)
                    usersToShow.Add(user);
            }
            return View(usersToShow);
        }
        // Shows all users who are in role
        public ActionResult EditUsersInRole2()
        {
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var users = UserManager.Users;
            List<ApplicationUser> myUsers = new List<ApplicationUser>();
            List<ApplicationUser> usersToShow = new List<ApplicationUser>();
            foreach (var user in users)
            {
                myUsers.Add(user);
            }
            foreach (var user in myUsers)
            {
                if (UserManager.IsInRole(user.Id, "DeleteFolders") == true)
                    usersToShow.Add(user);
            }
            return View(usersToShow);
        }

        [Route("Administration/AddUserToRole/{userId}")]
        public ActionResult AddUserToRole(string userId)
        {
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var roles = roleManager.Roles;
            UserManager.AddToRole(userId, "DeleteFolders");
            return RedirectToAction("DisplayUsers", "DriveUsers");
        }

        [Route("Administration/RemoveUserFromRole/{userId}")]
        public ActionResult RemoveUserFromRole(string userId)
        {
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var roles = roleManager.Roles;
            UserManager.RemoveFromRole(userId, "DeleteFolders");
            return RedirectToAction("DisplayUsers", "DriveUsers");
        }*/
    }
}