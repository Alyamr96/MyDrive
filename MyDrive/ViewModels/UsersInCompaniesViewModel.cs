using MyDrive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyDrive.ViewModels
{
    public class UsersInCompaniesViewModel
    {
        public List<ApplicationUser> UsersInCompany { get; set; }

        public List<ApplicationUser> UsersNotInCompany { get; set; }
    }
}