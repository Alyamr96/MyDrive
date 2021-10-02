using MyDrive.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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

        public List<string> UserPermissions { get; set; } 

        public List<SelectListItem> AvailableCompanies { get; set; }

        public List<string> SelectedCompanies { get; set; }

        public Boolean SelectCompaniesToUploadFiles { get; set; }

        public Boolean AttachFileToUpload { get; set; }

        public List<CompaniesToViewFiles2> CompaniesToViewFiles { get; set; }

        public List<string> CompaniesUserIn { get; set; }

        public string AbsolotePath { get; set; }

        [Required(ErrorMessage = "File is Required")]
        [Display(Name = "Upload A File")]
        public HttpPostedFileBase File { get; set; }
    }
}