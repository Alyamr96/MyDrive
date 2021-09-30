using MyDrive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyDrive.ViewModels
{
    public class UserAndRoleViewModel
    {
        public List<UserAndRole> UsersWithRoles { get; set; }

        public List<UserAndRole> UsersWithoutRoles { get; set; }
    }
}