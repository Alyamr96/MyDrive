using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MyDrive.Models;

namespace MyDrive.ViewModels
{
    public class DisplayUsersViewModel
    {
        public List<ApplicationUser> UsersWithoutCompanies { get; set; }

        public List<ApplicationUser> UsersWithCompanies { get; set; }

        public List<UsersInCompanies> RecordsOfUsersInCompanies { get; set; }

        public List<Company> Companies { get; set; }

        public string Password { get; set; }
    }
}