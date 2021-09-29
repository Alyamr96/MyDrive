using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyDrive.ViewModels
{
    public class ListRolesViewModel
    {
        public List<IdentityRole> Roles { get; set; }

        public string Password { get; set; }
    }
}