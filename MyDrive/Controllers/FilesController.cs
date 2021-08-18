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
            return RedirectToAction("GetFiles");
        }

        [Route("Files/UploadFilesInPath/{folderName}")]
        public ActionResult UploadFilesInPath(string folderName, HttpPostedFileBase postedFile)
        {
            string targetDirectory = @"C: \Users\cashless\Desktop\MyDrive\MyDrive\Files";
            string searchPattern = "*";
            string[] dirs = Directory.GetDirectories(targetDirectory, searchPattern, SearchOption.AllDirectories);
            string path = "";
            foreach (string dir in dirs)
            {
                if (dir.Substring((dir.Length - folderName.Length), folderName.Length) == folderName)
                {
                    path = dir;
                }
            }

            if (postedFile == null)
                postedFile = Request.Files["userFile"];

            string filePath = path + @"\" + Path.GetFileName(postedFile.FileName);
            postedFile.SaveAs(filePath);
            return RedirectToAction("GetFoldersByPath/" + folderName);
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
            ViewBag.filesNames = filesNames;

                return View(); 
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
                string fullpath = Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar);
                string projectName = directory.Split(Path.DirectorySeparatorChar).Last();
                folderNames.Add(projectName);
            }
            ViewBag.folderNames = folderNames;
            ViewBag.directories = dirs;
            return View();
        }

        public ActionResult CreateFolderPopUp()
        {
            return View();
        }

        [Route("Files/GetFoldersByPath/{folderName}")]
        public ActionResult GetFoldersByPath(string folderName)
        {
            string targetDirectory = @"C: \Users\cashless\Desktop\MyDrive\MyDrive\Files";
            string searchPattern = "*";
            string[] dirs = Directory.GetDirectories(targetDirectory,searchPattern ,SearchOption.AllDirectories);
            string path = "";
            foreach(string dir in dirs)
            {
                if(dir.Substring((dir.Length - folderName.Length), folderName.Length) == folderName)
                {
                    path = dir;
                }
            }
            List<string> dirsToShow = new List<string>(Directory.EnumerateDirectories(path));
            List<string> folderNamesToShow = new List<string>(dirsToShow.Count);
            foreach (var directory in dirsToShow)
            {
                string fullpath = Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar);
                string projectName = directory.Split(Path.DirectorySeparatorChar).Last();
                folderNamesToShow.Add(projectName);
            }
            ViewBag.directories = folderNamesToShow;
            string FolderNameSplitFromPath = path;
            string fullpath2 = Path.GetFullPath(FolderNameSplitFromPath).TrimEnd(Path.DirectorySeparatorChar);
            string projectName2 = FolderNameSplitFromPath.Split(Path.DirectorySeparatorChar).Last();

            ViewBag.path = projectName2;
            string[] fileEntries = Directory.GetFiles(path);
            List<string> fileNamesToShow = new List<string>(fileEntries.Length);
            string[] fileEntriesWithoutExtention = new string[fileEntries.Length];
            for (int i = 0; i < fileEntries.Length; i++)
            {
                string testing = System.IO.Path.GetFileNameWithoutExtension(fileEntries[i]);
                fileEntriesWithoutExtention[i] = testing;
            }
            foreach (string filename in fileEntries)
            {
                string fullpath3 = Path.GetFullPath(filename).TrimEnd(Path.DirectorySeparatorChar);
                string projectName3 = filename.Split(Path.DirectorySeparatorChar).Last();
                fileNamesToShow.Add(projectName3);
            }
            ViewBag.files = fileNamesToShow;
            string filespathHREF = path;
            string pathAfterFiles = "";
            for (int i =0; i<filespathHREF.Length; i++)
            {
                string test = filespathHREF.Substring(i, 5);
                if(test.CompareTo("Files") == 0)
                {
                    int number = i + 6;
                    pathAfterFiles = filespathHREF.Substring(number);
                    break;
                }
            }
            ViewBag.PathAfterFile = pathAfterFiles;
            return View();
        }

        [Route("Files/CreateFolderInPath/{folderName}")]
        public ActionResult CreateFolderInPath(string folderName, Folder folder)
        {
            string folderToBeCreated = folder.Name;
            string targetDirectory = @"C: \Users\cashless\Desktop\MyDrive\MyDrive\Files";
            string searchPattern = "*";
            string[] dirs = Directory.GetDirectories(targetDirectory, searchPattern, SearchOption.AllDirectories);
            string path = "";
            foreach (string dir in dirs)
            {
                if (dir.Substring((dir.Length - folderName.Length), folderName.Length) == folderName)
                {
                    path = dir;
                }
            }
            path = path + @"\" + folderToBeCreated;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else
                return HttpNotFound();
            return RedirectToAction("GetFoldersByPath/" + folderName);
        }

        [Route("Files/DeleteFolder/{nameToDelete}")]
        public void DeleteFolder(string nameToDelete)
        {
            string targetDirectory = @"C: \Users\cashless\Desktop\MyDrive\MyDrive\Files";
            string searchPattern = "*";
            string[] dirs = Directory.GetDirectories(targetDirectory, searchPattern, SearchOption.AllDirectories);
            string path = "";
            foreach (string dir in dirs)
            {
                if (dir.Substring((dir.Length - nameToDelete.Length), nameToDelete.Length) == nameToDelete)
                {
                    path = dir;
                }
            }
            Directory.Delete(path, true);
        }


        [Route("Files/DeleteFile/{fileNameToDelete}")]
        public void DeleteFile(string fileNameToDelete)
        {
            string targetDirectory = @"C: \Users\cashless\Desktop\MyDrive\MyDrive\Files";
            string[] fileEntries = Directory.GetFiles(targetDirectory, "*", SearchOption.AllDirectories);
            string[] fileEntriesWithoutExtention = new string[fileEntries.Length];
            string path = "";
            for(int i =0; i<fileEntries.Length; i++)
            {
                string testing = System.IO.Path.ChangeExtension(fileEntries[i],null);
                //fileEntriesWithoutExtention[i] = testing;
                if (testing.Substring((testing.Length - fileNameToDelete.Length), fileNameToDelete.Length) == fileNameToDelete)
                {
                    path = fileEntries[i];
                    break;
                }
            }
            /*string path = "";
            foreach (string dir in fileEntriesWithoutExtention)
            {
                if (dir.Substring((dir.Length - fileNameToDelete.Length), fileNameToDelete.Length) == fileNameToDelete)
                {
                    path = dir;
                }
            }*/

            System.IO.File.Delete(path);
            //ViewBag.files = fileEntriesWithoutExtention;
            //ViewBag.path = path;
            //System.IO.File.Delete(fileEntries[Id]);
            //return View();
        }
    }
}