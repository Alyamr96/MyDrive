using MyDrive.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyDrive.ViewModels
{
    public class FoldersandFilesViewModel
    {
        public List<Folder> Folders { get; set; }
        public List<FileModel> Files { get; set; }

        [Required]
        public string Password { get; set; }

        public string FolderPathToDelete { get; set; }

        public string RenameFolderName { get; set; }

        public List<string> FoldersToDelete { get; set; }

        public List<string> FilesToDelete { get; set; }

        public List<Company> Companies { get; set; }
    }
}