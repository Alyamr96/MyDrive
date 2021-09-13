using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyDrive.Models
{
    public class FileModel
    {
        [Key]
        public string Path { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
    }
}