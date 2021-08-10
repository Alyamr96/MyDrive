using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using MyDrive.ViewModels;
using MyDrive.Models;

namespace MyDrive.Controllers
{
    [Authorize]
    public class FilesController : Controller
    {
        // GET: Files
        public ActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public ActionResult FileProfile()
        {
            return View();
        }

        [HttpPost]
        public ActionResult UploadFiles(HttpPostedFileBase postedFile)
        {
            string filePath = "";
            filePath = Server.MapPath("~/Files/");

            if (postedFile == null)
                postedFile = Request.Files["userFile"];

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            filePath = filePath + Path.GetFileName(postedFile.FileName);
            postedFile.SaveAs(filePath);
            ViewBag.Message = "File Saved";
            var FileModel = new FileViewModel() { 
            name = postedFile.FileName
            };
            return View(FileModel);
        }

        [Route("Files/FilesNames")]
        public ActionResult GetFiles()
        {
            string targetDirectory = @"C: \Users\cashless\Desktop\MyDrive\MyDrive\Files";
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            string testing;
            string[] filesNames = new String[fileEntries.Length];
            for (int i = 0; i < fileEntries.Length; i++)
            {
                testing = fileEntries[i];
                filesNames[i] = testing;
            }
            var FilesNamesToView = new FilesNamesViewModel
            {
                name = filesNames
            };

                return View(FilesNamesToView); 
        }

        
        [Route("Files/DeleteFile/{Id}")]
        public ActionResult DeleteFile(int Id)
        {
            string targetDirectory = @"C: \Users\cashless\Desktop\MyDrive\MyDrive\Files";
            string[] fileEntries = Directory.GetFiles(targetDirectory); 
            
            System.IO.File.Delete(fileEntries[Id]);
            

            return RedirectToAction("GetFiles");
        }

        public ActionResult New()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateFolder(Folder folder)
        {
            string FolderName = folder.Name;
            string folderPath = "";
            folderPath = Server.MapPath("~/Files/");
            folderPath = folderPath + FolderName;

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);
            else
                return HttpNotFound();

            return RedirectToAction("GetFolders");
        }

        public ActionResult GetFolders()
        {
            //string targetDirectory = "~/Files";
            string targetDirectory = @"C: \Users\cashless\Desktop\MyDrive\MyDrive\Files";
            //var dirs = from dir in Directory.EnumerateDirectories(targetDirectory) select dir;
            List<string> dirs = new List<string>(Directory.EnumerateDirectories(targetDirectory));
            List<string> folderNames = new List<string>(dirs.Count);
            foreach(var directory in dirs)
            {
                string testing = directory.Substring(49);
                folderNames.Add(testing);
            }
            var FoldersNamesToView = new FoldersNameViewModel
            {
                Directories = folderNames
            };
            return View(FoldersNamesToView);
        }
    }
}