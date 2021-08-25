using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using MyDrive.Models;
using MyDrive.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace MyDrive.Controllers
{
    [Authorize]
    public class FolderController : Controller
    {
        static string absoloutePath = @"C: \Users\cashless\Desktop\MyDrive\MyDrive\Files";
        /*private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;

        public FolderController()
        {
        }
        public FolderController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }*/
        // GET: Folder
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

            return RedirectToAction("GetFolders1");
        }

        public ActionResult GetFolders1()
        {
            string targetDirectory = @"C: \Users\cashless\Desktop\MyDrive\MyDrive\Files";
            List<string> dirs = new List<string>(Directory.EnumerateDirectories(targetDirectory));
            List<string> foldersPathAfterFile = new List<string>(dirs.Count);
            List<string> folderNames = new List<string>(dirs.Count);
            List<Folder> folders = new List<Folder>(dirs.Count);
            foreach (var directory in dirs)
            {
                string fullpath = Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar);
                string projectName = directory.Split(Path.DirectorySeparatorChar).Last();
                folderNames.Add(projectName);
            }
            foreach (var directory in dirs)
            {
                for (int i = 0; i < directory.Length; i++)
                {
                    string test = directory.Substring(i, 5);
                    if (test.CompareTo("Files") == 0)
                    {
                        int number = i + 6;
                        foldersPathAfterFile.Add(directory.Substring(number));
                        break;
                    }
                }
            }
            for (int i = 0; i < dirs.Count; i++)
            {
                folders.Add(new Folder { Name = folderNames[i], Path = foldersPathAfterFile[i] });
            }
            // starting here we are getting all files
            string[] fileEntries = Directory.GetFiles(absoloutePath);
            List<string> filesPathAfterFile = new List<string>(fileEntries.Length);
            string[] fileNames = new string[fileEntries.Length];
            List<FileModel> files = new List<FileModel>(fileEntries.Length);
            for (int i = 0; i < fileEntries.Length; i++)
            {
                string fullpath = Path.GetFullPath(fileEntries[i]).TrimEnd(Path.DirectorySeparatorChar);
                string projectName = fileEntries[i].Split(Path.DirectorySeparatorChar).Last();
                fileNames[i] = projectName;
            }
            for (int j = 0; j < fileEntries.Length; j++)
            {
                for (int i = 0; i < fileEntries[j].Length; i++)
                {
                    string test = fileEntries[j].Substring(i, 5);
                    if (test.CompareTo("Files") == 0)
                    {
                        int number = i + 6;
                        filesPathAfterFile.Add(fileEntries[j].Substring(number));
                        break;
                    }
                }
            }
            for (int i = 0; i < fileEntries.Length; i++)
            {
                files.Add(new FileModel { Name = fileNames[i], Path = filesPathAfterFile[i] });
            }
            // Folders and files viewModel
            var FoldersAndFiles = new FoldersandFilesViewModel
            {
                Folders = folders,
                Files = files
            };
            return View(FoldersAndFiles);
        }
        
        [Route("Folder/GetFolderFromPath/{folderName}")]
        public ActionResult GetFoldersFromPath(string folderName)
        {
            string myFolderName = folderName.Replace(";", @"\");
            string path = absoloutePath + @"/" + myFolderName;
            List<string> dirs = new List<string>(Directory.EnumerateDirectories(path));
            List<string> foldersPathAfterFile = new List<string>(dirs.Count);
            List<string> folderPathAfterFileWithoutBackSlash = new List<string>(dirs.Count);
            List<string> folderNames = new List<string>(dirs.Count);
            List<Folder> folders = new List<Folder>(dirs.Count);
            foreach (var directory in dirs)
            {
                string fullpath = Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar);
                string projectName = directory.Split(Path.DirectorySeparatorChar).Last();
                folderNames.Add(projectName);
            }
            foreach (var directory in dirs)
            {
                for (int i = 0; i < directory.Length; i++)
                {
                    string test = directory.Substring(i, 5);
                    if (test.CompareTo("Files") == 0)
                    {
                        int number = i + 6;
                        foldersPathAfterFile.Add(directory.Substring(number));
                        break;
                    }
                }
            }
            foreach (var directory in foldersPathAfterFile)
            {
                string testing = directory.Replace(@"\", ";");
                folderPathAfterFileWithoutBackSlash.Add(testing);
            }
            for (int i = 0; i < dirs.Count; i++)
            {
                folders.Add(new Folder { Name = folderNames[i], Path = folderPathAfterFileWithoutBackSlash[i] });
            }
            // Starting here we're getting the files
            string[] fileEntries = Directory.GetFiles(path);
            List<string> filesPathAfterFile = new List<string>(fileEntries.Length);
            string[] fileNames = new string[fileEntries.Length];
            List<FileModel> files = new List<FileModel>(fileEntries.Length);
            for (int i = 0; i < fileEntries.Length; i++)
            {
                string fullpath = Path.GetFullPath(fileEntries[i]).TrimEnd(Path.DirectorySeparatorChar);
                string projectName = fileEntries[i].Split(Path.DirectorySeparatorChar).Last();
                fileNames[i] = projectName;
            }
            for (int j = 0; j < fileEntries.Length; j++)
            {
                for (int i = 0; i < fileEntries[j].Length; i++)
                {
                    string test = fileEntries[j].Substring(i, 5);
                    if (test.CompareTo("Files") == 0)
                    {
                        int number = i + 6;
                        filesPathAfterFile.Add(fileEntries[j].Substring(number));
                        break;
                    }
                }
            }
            for (int i = 0; i < fileEntries.Length; i++)
            {
                files.Add(new FileModel { Name = fileNames[i], Path = filesPathAfterFile[i] });
            }
            // FoldersandFilesViewModel
            var FoldersAndFiles = new FoldersandFilesViewModel
            {
                Folders = folders,
                Files = files
            };
            return View(FoldersAndFiles);
            //return View(folders);
        }

        [Route("Folder/CreateFolderWithinPath/{folderName}")]
        public ActionResult CreateFolderWithinPath(string folderName, Folder folder)
        {
             string myFolderName = folderName.Replace(";", @"\");
             string path = absoloutePath + @"/" + myFolderName + @"/" +folder.Name;
             if (!Directory.Exists(path))
                 Directory.CreateDirectory(path);
             else
                 return HttpNotFound();
             return RedirectToAction("GetFolderFromPath/" + folderName);
        }

        [Route("Folder/GetFiles")]
        public ActionResult GetFiles1()
        {
            // used a list<folder> folders as it is a class made to store name and path and thats exactly whats needed to pass to the view
            string[] fileEntries = Directory.GetFiles(absoloutePath);
            List<string> filesPathAfterFile = new List<string>(fileEntries.Length);
            string[] fileNames = new string[fileEntries.Length];
            List<FileModel> files = new List<FileModel>(fileEntries.Length);
            for (int i =0; i<fileEntries.Length; i++)
            {
                string fullpath = Path.GetFullPath(fileEntries[i]).TrimEnd(Path.DirectorySeparatorChar);
                string projectName = fileEntries[i].Split(Path.DirectorySeparatorChar).Last();
                fileNames[i] = projectName;
            }
            for (int j =0; j<fileEntries.Length; j++)
            {
                for (int i = 0; i < fileEntries[j].Length; i++)
                {
                    string test = fileEntries[j].Substring(i, 5);
                    if (test.CompareTo("Files") == 0)
                    {
                        int number = i + 6;
                        filesPathAfterFile.Add(fileEntries[j].Substring(number));
                        break;
                    }
                }
            }
            for (int i = 0; i < fileEntries.Length; i++)
            {
                files.Add(new FileModel { Name = fileNames[i], Path = filesPathAfterFile[i] });
            }
            
            return View(files);
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
            var FileModel = new FileViewModel()
            {
                name = postedFile.FileName
            };
            return RedirectToAction("GetFolders1");
        }

        [Route("Folder/UploadFileWithinPath/{folderName}")]
        public ActionResult UploadFileWithinPath(string folderName, HttpPostedFileBase postedFile)
        {
            string myFolderName = folderName.Replace(";", @"\");
            string path = absoloutePath + @"/" + myFolderName;
            if (postedFile == null)
                postedFile = Request.Files["userFile"];

            string filePath = path + @"\" + Path.GetFileName(postedFile.FileName);
            postedFile.SaveAs(filePath);
            return RedirectToAction("GetFolderFromPath/" + folderName);
        }

        [Route("Folder/DeleteFolder/{folderPath}")]
        public void DeleteFolder(string folderPath)
        {
            //var userid = User.Identity.GetUserId();
            //ApplicationUser user1 = UserManager.FindByIdAsync(userid).Result;
            string myFolderPath = folderPath.Replace(";", @"\");
            string path = absoloutePath + @"/" + myFolderPath;
            Directory.Delete(path, true);
        }

        [Route("Folder/DeleteFile/{filePath}")]
        public void DeleteFile(string filePath)
        {
            string myFilePath = filePath.Replace(";", @"\");
            string myFinalPath = myFilePath.Replace("'", ".");
            string path = absoloutePath + @"/" + myFinalPath;
            System.IO.File.Delete(path);
        }
    }
}