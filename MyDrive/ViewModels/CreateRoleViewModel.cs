using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyDrive.ViewModels
{
    public class CreateRoleViewModel
    {
        [Required(ErrorMessage ="Role Name is Required")]
        [Display(Name ="Role Name")]
        public string RoleName { get; set; }

        public List<string> Permissions { get; set; }

        public List<string> PermissionsAddedToRole { get; set; }

        public Boolean Flag { get; set; }


    }
}