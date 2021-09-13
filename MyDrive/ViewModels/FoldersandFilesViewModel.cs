using MyDrive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyDrive.ViewModels
{
    public class FoldersandFilesViewModel
    {
        public List<Folder> Folders { get; set; }
        public List<FileModel> Files { get; set; }

        public string Password { get; set; }

        public string FolderPathToDelete { get; set; }
    }
}