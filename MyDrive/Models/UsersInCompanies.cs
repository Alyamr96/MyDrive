using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyDrive.Models
{
    public class UsersInCompanies
    {
        [Key]
        public int Id { get; set; }

        public int CompanyId { get; set; }

        public string UserId { get; set; }
    }
}