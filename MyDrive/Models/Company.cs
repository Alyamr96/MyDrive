using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyDrive.Models
{
    public class Company
    {
        [Required(ErrorMessage = "Company Name Is Required")]
        public string Name { get; set; }

        [Key]
        public string Path { get; set; }

        public string LogoPath { get; set; }

        public List<ApplicationUser> CompanyEmployees { get; set; }
    }
}