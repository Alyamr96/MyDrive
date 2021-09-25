using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
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
        static string folderPathWhenRename = "";
        static List<string> FoldersToDelete = new List<string>();
        static List<string> FilesToDelete = new List<string>();
        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        private ApplicationDbContext _context;

        public FolderController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
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
        }
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
                return RedirectToAction("FolderExistsView");

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
            string path = absoloutePath + @"/" + myFolderName + @"/" + folder.Name;
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            else
                return RedirectToAction("FolderExistsView");

            //return HttpNotFound();
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
            if (!System.IO.File.Exists(filePath))
            {
                postedFile.SaveAs(filePath);
            }
            else
                return RedirectToAction("FileExistsView");
            ViewBag.Message = "File Saved";
            var FileModel = new FileViewModel()
            {
                name = postedFile.FileName
            };
            var myFile = new FileModel { Name = postedFile.FileName, Path = filePath, Date = DateTime.Now};
            _context.Files.Add(myFile);
            _context.SaveChanges();
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
            if (!System.IO.File.Exists(filePath))
            {
                postedFile.SaveAs(filePath);
            }
            else
                return RedirectToAction("FileExistsView");

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
            string pathUsedForDelete = absoloutePath + @"\" + myFinalPath;
            pathUsedForDelete = pathUsedForDelete.Replace(" ", "");
            var DatabaseFiles = _context.Files.ToList();
            //var fileInDb = _context.Files.Single(c => c.Path == pathUsedForDelete);
            foreach(var DatabaseFile in DatabaseFiles)
            {
                if(DatabaseFile.Path == pathUsedForDelete)
                {
                    _context.Files.Remove(DatabaseFile);
                    _context.SaveChanges();
                }
            }
            System.IO.File.Delete(path);
        }

        public ActionResult TestView()
        {
            return View();
        }

        public ActionResult ShowFilesInTable()
        {
            string[] fileEntries = Directory.GetFiles(absoloutePath, "*", SearchOption.AllDirectories);
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

            return View(files);
        }

        [Route("Folder/RenameFolder/{folderPath}")]
        public ActionResult RenameFolder(string folderPath)
        {
            folderPathWhenRename = folderPath;
            return View();
        }

        [HttpPost]
        public ActionResult RenameFolder2(Folder folder)
        {
            string path = folderPathWhenRename;
            folderPathWhenRename = "";
            string returnPath = "";
            string[] words = path.Split(';');
            string oldFolderName = words[words.Length - 1];
            string oldPath = "";

            for (int i = 0; i < words.Length - 1; i++)
            {
                returnPath = returnPath + words[i] + ";";
                oldPath = oldPath + words[i] + "/";
            }
            oldPath = oldPath + oldFolderName;

            oldPath = oldPath.Replace("/", @"\");
            oldPath = absoloutePath + @"\" + oldPath;
            DirectoryInfo di = new DirectoryInfo(oldPath);
            di.MoveTo(Path.Combine(di.Parent.FullName, folder.Name));

            if (returnPath != "")
            {
                return RedirectToAction("GetFolderFromPath/" + returnPath);
            }
            else
                return RedirectToAction("GetFolders1");

        }

        [Route("Folder/RenameFile/{filePath}")]
        public ActionResult RenameFile(string filePath)
        {
            folderPathWhenRename = filePath;
            return View();
        }

        [HttpPost]
        public ActionResult RenameFile2(Folder folder)
        {
            string path = folderPathWhenRename;
            folderPathWhenRename = "";
            path = path.Replace("'", ".");
            string[] extentionArray = path.Split('.');
            string extention = extentionArray[extentionArray.Length - 1];
            string NewFileName = folder.Name + '.' + extention;
            string returnPath = "";
            string[] words = path.Split(';');
            string oldFolderName = words[words.Length - 1];
            string oldPath = "";

            for (int i = 0; i < words.Length - 1; i++)
            {
                returnPath = returnPath + words[i] + ";";
                oldPath = oldPath + words[i] + "/";
            }
            oldPath = oldPath + oldFolderName;

            oldPath = oldPath.Replace("/", @"\");
            oldPath = absoloutePath + @"\" + oldPath;
            FileInfo di = new FileInfo(oldPath);
            di.MoveTo(Path.Combine(di.Directory.FullName, NewFileName));

            if (returnPath != "")
            {
                return RedirectToAction("GetFolderFromPath/" + returnPath);
            }
            else
                return RedirectToAction("GetFolders1");
            //return Content(oldPath +"+" + NewFileName + "+" + extention);

        }

        [Route("Folder/DeleteMultiple/{folderName}")]
        public ActionResult DeleteMultiple(string folderName)
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
        }

        [Route("Folder/StoreFolderInList/{folderPath}")]
        public ActionResult storeDeletedFoldersInArray(string folderPath)
        {
            string myFolderPath = folderPath.Replace(";", @"\");
            string path = absoloutePath + @"/" + myFolderPath;
            FoldersToDelete.Add(path);
            string[] words = folderPath.Split(';');
            string returnPath = "";
            for (int i = 0; i < words.Length - 1; i++)
            {
                returnPath = returnPath + words[i] + ";";
            }
            return RedirectToAction("DeleteMultiple/" + returnPath);
        }

        [Route("Folder/StoreFileInList/{filePath}")]
        public ActionResult storeDeletedFilesInArray(string filePath)
        {
            string myFilePath = filePath.Replace(";", @"\");
            string myFinalPath = myFilePath.Replace("'", ".");
            string path = absoloutePath + @"/" + myFinalPath;
            FilesToDelete.Add(path);
            string[] words = filePath.Split(';');
            string returnPath = "";
            for (int i = 0; i < words.Length - 1; i++)
            {
                returnPath = returnPath + words[i] + ";";
            }
            return RedirectToAction("DeleteMultiple/" + returnPath);
        }

        public ActionResult DeleteAllSelected()
        {
            List<string> FoldersToDeleteNoDuplicates = FoldersToDelete.Distinct().ToList();
            List<string> FilesToDeleteNoDuplicates = FilesToDelete.Distinct().ToList();
            //FoldersToDelete.Clear();
            //FilesToDelete.Clear();
            ViewBag.Folders = FoldersToDeleteNoDuplicates;
            ViewBag.Files = FilesToDeleteNoDuplicates;
            return View();
        }

        public ActionResult DeleteAllSelectedFoldersAndFiles()
        {
            List<string> FoldersToDeleteNoDuplicates = FoldersToDelete.Distinct().ToList();
            List<string> FilesToDeleteNoDuplicates = FilesToDelete.Distinct().ToList();
            for (int i = 0; i < FoldersToDeleteNoDuplicates.Count; i++)
            {
                Directory.Delete(FoldersToDeleteNoDuplicates[i], true);
            }
            for (int i = 0; i < FilesToDeleteNoDuplicates.Count; i++)
            {
                System.IO.File.Delete(FilesToDeleteNoDuplicates[i]);
            }
            FoldersToDelete.Clear();
            FilesToDelete.Clear();
            // starting here are operations to return to folders
            string returnPath = FoldersToDeleteNoDuplicates[0];
            for (int i = 0; i < returnPath.Length; i++)
            {
                string test = returnPath.Substring(i, 5);
                if (test.CompareTo("Files") == 0)
                {
                    int number = i + 6;
                    returnPath = returnPath.Substring(number);
                    break;
                }
            }
            returnPath = returnPath.Replace(@"\", ";");
            string[] words = returnPath.Split(';');
            string returnPath1 = "";
            for (int i = 0; i < words.Length - 1; i++)
            {
                returnPath1 = returnPath1 + words[i] + ";";
            }
            returnPath1 = returnPath1.Substring(0, returnPath1.Length - 1);
            return RedirectToAction("GetFolderFromPath/" + returnPath1);
        }

        public ActionResult FolderExistsView()
        {
            return View();
        }

        public ActionResult FileExistsView()
        {
            return View();
        }

        [Route("Folder/BackButton/{path}")]
        public ActionResult BackButton(string path)
        {
            string returnPath = path;
            string returnPath1 = "";
            string[] words = returnPath.Split(';');
            if (words.Length == 1)
                return RedirectToAction("GetFolders1");
            else
            {
                for (int i = 0; i < words.Length - 1; i++)
                {
                    returnPath1 = returnPath1 + words[i] + ";";
                }
                returnPath1 = returnPath1.Substring(0, returnPath1.Length - 1);
                return RedirectToAction("GetFolderFromPath/" + returnPath1);
            }
        }
        [HttpPost]
        public ActionResult ConfirmPassword(FoldersandFilesViewModel viewModel, string folderPath)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user1 = UserManager.FindById(userId);
                try
                {
                    var result = UserManager.CheckPassword(user1, viewModel.Password);
                    if (result)
                    {
                        DeleteFolder(folderPath);
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }
                       
                }
                catch(System.ArgumentNullException e)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }    
            }
            else
                return Json(false, JsonRequestBehavior.AllowGet);
            
        }

        [HttpPost]
        public ActionResult ConfirmPasswordForFile(FoldersandFilesViewModel viewModel, string filePath)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user1 = UserManager.FindById(userId);
                try
                {
                    var result = UserManager.CheckPassword(user1, viewModel.Password);
                    if (result)
                    {
                        DeleteFile(filePath);
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(false, JsonRequestBehavior.AllowGet);
                }
                catch (System.ArgumentNullException e)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(false, JsonRequestBehavior.AllowGet);
            
        }

        [HttpPost]
        public ActionResult ConfirmPasswordForFolderRename(FoldersandFilesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user1 = UserManager.FindById(userId);
                try
                {
                    var result = UserManager.CheckPassword(user1, viewModel.Password);
                    if (result)
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(false, JsonRequestBehavior.AllowGet);
                }
                catch (System.ArgumentNullException e)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ConfirmPasswordForFileRename(FoldersandFilesViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user1 = UserManager.FindById(userId);
                try
                {
                    var result = UserManager.CheckPassword(user1, viewModel.Password);
                    if (result)
                    {
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                        return Json(false, JsonRequestBehavior.AllowGet);
                }
                catch (System.ArgumentNullException e)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(false, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult RenameFolderModal(FoldersandFilesViewModel viewModel, string folderPath)
        {
            try
            {
                string path = folderPath;
                string[] words = path.Split(';');
                string oldFolderName = words[words.Length - 1];
                string oldPath = "";

                for (int i = 0; i < words.Length - 1; i++)
                {
                    oldPath = oldPath + words[i] + "/";
                }
                oldPath = oldPath + oldFolderName;

                oldPath = oldPath.Replace("/", @"\");
                oldPath = absoloutePath + @"\" + oldPath;
                DirectoryInfo di = new DirectoryInfo(oldPath);
                di.MoveTo(Path.Combine(di.Parent.FullName, viewModel.RenameFolderName));

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch(System.ArgumentNullException e)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }
            
        }

        [HttpPost]
        public ActionResult RenameFileModal(FoldersandFilesViewModel viewModel, string filePath)
        {
            try
            {
                string path = filePath;
                path = path.Replace("'", ".");
                string[] extentionArray = path.Split('.');
                string extention = extentionArray[extentionArray.Length - 1];
                string NewFileName = viewModel.RenameFolderName + '.' + extention;
                string[] words = path.Split(';');
                string oldFolderName = words[words.Length - 1];
                string oldPath = "";

                for (int i = 0; i < words.Length - 1; i++)
                {
                    oldPath = oldPath + words[i] + "/";
                }
                oldPath = oldPath + oldFolderName;

                oldPath = oldPath.Replace("/", @"\");
                oldPath = absoloutePath + @"\" + oldPath;
                FileInfo di = new FileInfo(oldPath);
                di.MoveTo(Path.Combine(di.Directory.FullName, NewFileName));

                return Json(true, JsonRequestBehavior.AllowGet);
            }
            catch (System.ArgumentNullException e)
            {
                return Json(false, JsonRequestBehavior.AllowGet);
            }

        }
    }
}