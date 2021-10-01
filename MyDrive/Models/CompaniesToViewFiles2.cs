using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyDrive.Models
{
    public class CompaniesToViewFiles2
    {
        [Key]
        public int Id { get; set; }

        public string CompanyName { get; set; }

        public string FilePath { get; set; }

    }
}
