using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyDrive.Models;
using MyDrive.ViewModels;
using System.IO;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Microsoft.AspNet.Identity;
using System.Data.Entity.Validation;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity.EntityFramework;

namespace MyDrive.Controllers
{
    public class DriveUsersController : Controller
    {
        private ApplicationDbContext _context;
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        public DriveUsersController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }

        public DriveUsersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        // GET: DriveUsers

        public ActionResult DisplayUsers()
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
            var users = _context.Users.ToList();
            var records = _context.UsersInCompanies.ToList();
            List<ApplicationUser> usersWithoutCompanies = new List<ApplicationUser>();
            List<ApplicationUser> usersWithCompanies = new List<ApplicationUser>();
            //List<ApplicationUser> usersWithoutRoles = new List<ApplicationUser>();
            var companies = _context.Companies.ToList();
            foreach(var user in users)
            {
                int count = 0;
                foreach(var record in records)
                {
                    if(record.UserId == user.Id)
                    {
                        count++;
                    }
                }
                if (count == 0)
                    usersWithoutCompanies.Add(user);
                else
                    usersWithCompanies.Add(user);
            }
            var viewModel = new DisplayUsersViewModel { UsersWithoutCompanies = usersWithoutCompanies, UsersWithCompanies = usersWithCompanies, RecordsOfUsersInCompanies = records, Companies = companies, UserPermissions = UserPermissions};
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ConfirmPasswordForUserDelete(DisplayUsersViewModel viewModel, string id)
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
                        var user = _context.Users.Single(c => c.Id == id);
                        var UsersInCompanies = _context.UsersInCompanies.ToList();
                        var FileRecords = _context.Files.ToList();
                        foreach(var UserInCompany in UsersInCompanies)
                        {
                            if(UserInCompany.UserId == user.Id)
                            {
                                _context.UsersInCompanies.Remove(UserInCompany);
                                _context.SaveChanges();
                            }
                        }
                        foreach (var FileRecord in FileRecords)
                        {
                            if (FileRecord.UserId == user.Id)
                            {
                                _context.Files.Remove(FileRecord);
                                _context.SaveChanges();
                            }
                        }
                        _context.Users.Remove(user);
                        _context.SaveChanges();

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
        /*public ActionResult DisplayApiUsers()
        {
            IEnumerable<DriveUsers> Apiusers = null;
            var dbusers = _context.Users.ToList();
            using (var client = new HttpClient())
            {
                client.BaseAddress = new Uri("http://81.29.108.91/api/");
                var responseTask = client.GetAsync("users");
                responseTask.Wait();

                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var results = result.Content.ReadAsStringAsync().Result;
                    var o = JsonConvert.DeserializeObject<JObject>(results);
                    Apiusers = o.Value<JArray>("users")
                    .ToObject<IList<DriveUsers>>();
                    //var readTask = result.Content.ReadAsAsync<IList<DriveUsers>>();
                    //readTask.Wait();

                    //users = readTask.Result;
                }
                else
                {
                    Apiusers = Enumerable.Empty<DriveUsers>();
                    ModelState.AddModelError(string.Empty, "server error. please contact api admin");
                }
            }
            foreach (var user in Apiusers)
            {
                Boolean found = false;
                string email = user.email;
                foreach (var dbuser in dbusers)
                {
                    if (dbuser.Email == email)
                        found = true;
                }
                if (found == false)
                {
                    user.password = "12345678";
                    
                    var newuser = new ApplicationUser
                    {
                        Id = user.Id.ToString(),
                        UserName = user.email,
                        Email = user.email,
                        PhoneNumberInt = user.phone,
                        FirstName = user.Name,
                        PasswordHash = user.password,
                        LastName = "lastname"
                    };

                    try
                    {
                        _context.Users.Add(newuser);
                        _context.SaveChanges();
                    }
                    catch (DbEntityValidationException dbEx)
                    {
                        foreach (var validationErrors in dbEx.EntityValidationErrors)
                        {
                            foreach (var validationError in validationErrors.ValidationErrors)
                            {
                                System.Console.WriteLine("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
                                ViewBag.Error = validationError.ErrorMessage;
                            }
                        }
                    }

                }

            }
            return View(dbusers);
        }*/

    }
}