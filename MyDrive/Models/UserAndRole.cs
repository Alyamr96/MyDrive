using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyDrive.Models
{
    public class UserAndRole
    {
        public ApplicationUser User { get; set; }

        public IdentityRole Role { get; set; }
    }
}