using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyDrive.Models
{
    public class RolePermissions
    {
        [Key]
        public int Id { get; set; }

        public string RoleId { get; set; }

        public string PermissionName { get; set; }
    }
}