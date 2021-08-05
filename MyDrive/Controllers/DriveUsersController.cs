using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyDrive.Models;
using MyDrive.ViewModels;
using System.IO;

namespace MyDrive.Controllers
{
    public class DriveUsersController : Controller
    {
        // GET: DriveUsers
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DisplayUsers()
        {
            var users = new List<DriveUsers>{
                new DriveUsers {Name = "Aly Amr", Id = 1},
                new DriveUsers {Name = "Mohamed Osama", Id = 2},
                new DriveUsers {Name = "Ahmed Emad", Id = 3}
            };

            var viewModel = new DisplayUsersViewModel()
            {
                Users = users
            };
            return View(viewModel); 
        }

        [Route("DriveUsers/released/{year:regex(\\d{4})}/{month:regex(\\d{2}):range(1,12)}")]
        public ActionResult ByReleaseDate(int year, int month)
        {
            return Content(year + "/" + month);
        }

        [Route("DriveUsers/UsersInfo/{Id}")]
        public ActionResult ViewInformation(int Id)
        {
            var users = new List<DriveUsers>{
                new DriveUsers {Name = "Aly Amr", Id = 1},
                new DriveUsers {Name = "Mohamed Osama", Id = 2},
                new DriveUsers {Name = "Ahmed Emad", Id = 3}
            };

            DriveUsers user = null;

            for (int i = 0; i < users.Count; i++)
            {
                if (users[i].Id == Id)
                    user = users[i];
            }
            if (user != null)
                return View(user);
            else
                return HttpNotFound();
        }

    }
}