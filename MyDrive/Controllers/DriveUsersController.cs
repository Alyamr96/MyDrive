﻿using System;
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

namespace MyDrive.Controllers
{
    public class DriveUsersController : Controller
    {
        private ApplicationDbContext _context;
        public DriveUsersController()
        {
            _context = new ApplicationDbContext();
        }
        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: DriveUsers

        public ActionResult DisplayUsers()
        {
            var users = _context.Users.ToList();
            var records = _context.UsersInCompanies.ToList();
            List<ApplicationUser> usersWithoutCompanies = new List<ApplicationUser>();
            List<ApplicationUser> usersWithCompanies = new List<ApplicationUser>();
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
            var viewModel = new DisplayUsersViewModel { UsersWithoutCompanies = usersWithoutCompanies, UsersWithCompanies = usersWithCompanies, RecordsOfUsersInCompanies = records, Companies = companies };
            return View(viewModel);
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