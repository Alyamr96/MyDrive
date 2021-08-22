using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyDrive.Models
{
    public class DriveUsers
    {
        public int Id { get; set; }
        public String Name { get; set; }
        public string user_name { get; set; }
        public string email { get; set; }
        public string email_verified_at { get; set; }
        public int phone { get; set; }
        public string password { get; set; }
        public string status { get; set; }
        public string created_at { get; set; }
        public string updated_at { get; set; }
    
    }
}