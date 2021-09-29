﻿using Microsoft.AspNet.Identity;
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
        static readonly List<string> Permissions = new List<String>{"Create User" ,"View Users", "Delete User", "Assign Users To Companies", "View All Uploaded Files", "Delete File", "Upload File", "Delete Company", "Delete Folder", "Create Folder", "Delete Multiple Folders/Files", "View Roles", "Create Role", "Edit Role", "Delete Role", "View Filters", "Manage Companies"};
        List<string> AddedPermissionsToNewRole = new List<string>();
        List<string> RemovedPermissionsToNewRole = new List<string>();

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
            var viewModel = new CreateRoleViewModel { Permissions = Permissions, PermissionsAddedToRole = AddedPermissionsToNewRole, Flag = false };
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

                foreach(var error in result.Errors)
                {
                    ModelState.AddModelError("", error);
                }
            }
            return View("CreateRole",model);
        }
        [HttpGet]
        public ActionResult ListRoles()
        {
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var roles = roleManager.Roles.ToList();
            var viewModel = new ListRolesViewModel { Roles = roles };
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
        public ActionResult AssignPermissionToRole(string PermissionName)
        {
            AddedPermissionsToNewRole.Add(PermissionName);
            return Json(true, JsonRequestBehavior.AllowGet);
        }
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