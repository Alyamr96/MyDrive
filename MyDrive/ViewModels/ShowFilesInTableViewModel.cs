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

        public List<CompaniesToViewFiles2> Records { get; set; }
    }
}