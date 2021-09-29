using MyDrive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyDrive.ViewModels
{
    public class ShowFilesInTableViewModel
    {
        public List<FileModel> Files { get; set; }

        public string Password { get; set; }
    }
}