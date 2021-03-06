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
using System.Windows.Forms;

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
            var companiesInDb = _context.Companies.ToList();
            List<SelectListItem> AvailableCompanies = new List<SelectListItem>();
            List<string> SelectedCompanies = new List<string>();
            foreach(var company in companiesInDb)
            {
                AvailableCompanies.Add(new SelectListItem { Text = company.Name, Value = company.Name });
            }
            // get list of permissions
            var userId = User.Identity.GetUserId();
            ApplicationUser user1 = UserManager.FindById(userId);
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var roles = roleManager.Roles.ToList();
            string RolesUserAssignedTo = "";
            var PermissionRecords = _context.RolePermissions.ToList();
            List<string> UserPermissions = new List<string>();
            foreach (var role in roles)
            {
                if (UserManager.IsInRole(userId, role.Name) == true)
                    RolesUserAssignedTo = role.Id;
            }
            foreach(var record in PermissionRecords)
            {
                if (record.RoleId == RolesUserAssignedTo)
                    UserPermissions.Add(record.PermissionName);
            }
            var CompaniesToViewFiles = _context.CompaniesToViewFiles2.ToList();
            List<string> CompaniesUserIn = new List<string>();
            var UsersInCompanies = _context.UsersInCompanies.ToList();
            foreach(var record in UsersInCompanies)
            {
                if (record.UserId == user1.Id)
                    CompaniesUserIn.Add(record.CompanyName);
            }
            var FoldersAndFiles = new FoldersandFilesViewModel
            {
                Folders = folders,
                Files = files,
                Companies = companiesInDb,
                UserPermissions = UserPermissions,
                AvailableCompanies = AvailableCompanies,
                SelectedCompanies = SelectedCompanies,
                SelectCompaniesToUploadFiles = false,
                AttachFileToUpload = false,
                CompaniesToViewFiles = CompaniesToViewFiles,
                CompaniesUserIn = CompaniesUserIn,
                AbsolotePath = Server.MapPath("~/Files/")
            };
           
            return View(FoldersAndFiles);
        }

        [Route("Folder/GetFolderFromPath/{folderName}")]
        public ActionResult GetFoldersFromPath(string folderName)
        {
            string myFolderName = folderName.Replace(";", @"\");
            string path = Server.MapPath("~/Files/") + myFolderName;
            /*string pathReplacement = path.Replace(@"\", ";");
            string[] words = pathReplacement.Split(';');
            List<string> myFinalPath = new List<string>();
            for(int i=0; i< words.Length; i++)
            {
                myFinalPath.Add(words[i]);
            }
            List<string> myFinalPathNoDups = myFinalPath.Distinct().ToList();
            string pathNoDups = "";
            for(int i=0; i<myFinalPathNoDups.Count; i++)
            {
                pathNoDups = pathNoDups + @"\" + myFinalPathNoDups[i];
            }
            pathNoDups = pathNoDups.Substring(1);
            ViewBag.abc = pathNoDups;
            ViewBag.abcd = path;*/
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
            var companiesInDb = _context.Companies.ToList();
            List<SelectListItem> AvailableCompanies = new List<SelectListItem>();
            List<string> SelectedCompanies = new List<string>();
            foreach (var company in companiesInDb)
            {
                AvailableCompanies.Add(new SelectListItem { Text = company.Name, Value = company.Name });
            }
            // get list of permissions
            var userId = User.Identity.GetUserId();
            ApplicationUser user1 = UserManager.FindById(userId);
            var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext());
            var roleManager = new RoleManager<IdentityRole>(roleStore);
            var roles = roleManager.Roles.ToList();
            string RolesUserAssignedTo = "";
            var PermissionRecords = _context.RolePermissions.ToList();
            List<string> UserPermissions = new List<string>();
            foreach (var role in roles)
            {
                if (UserManager.IsInRole(userId, role.Name) == true)
                    RolesUserAssignedTo = role.Id;
            }
            foreach (var record in PermissionRecords)
            {
                if (record.RoleId == RolesUserAssignedTo)
                    UserPermissions.Add(record.PermissionName);
            }
            var CompaniesToViewFiles = _context.CompaniesToViewFiles2.ToList();
            List<string> CompaniesUserIn = new List<string>();
            var UsersInCompanies = _context.UsersInCompanies.ToList();
            foreach (var record in UsersInCompanies)
            {
                if (record.UserId == user1.Id)
                    CompaniesUserIn.Add(record.CompanyName);
            }
            var FoldersAndFiles = new FoldersandFilesViewModel
            {
                Folders = folders,
                Files = files,
                Companies = companiesInDb,
                UserPermissions = UserPermissions,
                AvailableCompanies = AvailableCompanies,
                SelectedCompanies = SelectedCompanies,
                SelectCompaniesToUploadFiles = false,
                AttachFileToUpload = false,
                CompaniesToViewFiles = CompaniesToViewFiles,
                CompaniesUserIn = CompaniesUserIn,
                AbsolotePath = Server.MapPath("~/Files/")
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
        public ActionResult UploadFiles(FoldersandFilesViewModel viewModel)
        {
            string filePath = "";
            filePath = Server.MapPath("~/Files/");

            //if (viewModel.File == null)
                //postedFile = Request.Files["userFile"];

            if (!Directory.Exists(filePath))
                Directory.CreateDirectory(filePath);

            filePath = filePath + Path.GetFileName(viewModel.File.FileName);

            var companies = viewModel.SelectedCompanies;
            if (!System.IO.File.Exists(filePath))
            {
                viewModel.File.SaveAs(filePath);
            }
            else
                return RedirectToAction("FileExistsView");
            ViewBag.Message = "File Saved";
            if(companies == null)
            {
                var allCompanies = _context.Companies.ToList();
                foreach(var allcompany in allCompanies)
                {
                    var myRecord = new CompaniesToViewFiles2 { CompanyName = allcompany.Name, FilePath = filePath };
                    _context.CompaniesToViewFiles2.Add(myRecord);
                    _context.SaveChanges();
                }
            }
            else {
                foreach (var company in companies)
                {
                    var myRecord = new CompaniesToViewFiles2 { CompanyName = company, FilePath = filePath };
                    _context.CompaniesToViewFiles2.Add(myRecord);
                    _context.SaveChanges();
                }
            }
            var FileModel = new FileViewModel()
            {
                name = viewModel.File.FileName
            };
            var userId = User.Identity.GetUserId();
            ApplicationUser user1 = UserManager.FindById(userId);
            var myFile = new FileModel { Name = viewModel.File.FileName, Path = filePath, Date = DateTime.Now, UserId = user1.Id};
            _context.Files.Add(myFile);
            _context.SaveChanges();
            return RedirectToAction("GetFolders1");
        }

        [Route("Folder/UploadFileWithinPath/{folderName}")]
        public ActionResult UploadFileWithinPath(string folderName, FoldersandFilesViewModel viewModel)
        {
            string myFolderName = folderName.Replace(";", @"\");
            string path = Server.MapPath("~/Files/") + myFolderName;
            //if (viewModel.File == null)
                //postedFile = Request.Files["userFile"];

            string filePath = path + @"\" + Path.GetFileName(viewModel.File.FileName);
            if (!System.IO.File.Exists(filePath))
            {
                viewModel.File.SaveAs(filePath);
            }
            else
                return RedirectToAction("FileExistsView");

            var companies = viewModel.SelectedCompanies;
            if (companies == null)
            {
                var allCompanies = _context.Companies.ToList();
                foreach (var allcompany in allCompanies)
                {
                    var myRecord = new CompaniesToViewFiles2 { CompanyName = allcompany.Name, FilePath = filePath };
                    _context.CompaniesToViewFiles2.Add(myRecord);
                    _context.SaveChanges();
                }
            }
            else
            {
                foreach (var company in companies)
                {
                    var myRecord = new CompaniesToViewFiles2 { CompanyName = company, FilePath = filePath };
                    _context.CompaniesToViewFiles2.Add(myRecord);
                    _context.SaveChanges();
                }
            }
            var userId = User.Identity.GetUserId();
            ApplicationUser user1 = UserManager.FindById(userId);
            var myFile = new FileModel { Name = viewModel.File.FileName, Path = filePath, Date = DateTime.Now, UserId = user1.Id };
            _context.Files.Add(myFile);
            _context.SaveChanges();
            return RedirectToAction("GetFolderFromPath/" + folderName);
        }

        [Route("Folder/DeleteFolder/{folderPath}")]
        public void DeleteFolder(string folderPath)
        {
            //var userid = User.Identity.GetUserId();
            //ApplicationUser user1 = UserManager.FindByIdAsync(userid).Result;
            string myFolderPath = folderPath.Replace(";", @"\");
            string path = Server.MapPath("~/Files/") + myFolderPath;
            string[] fileEntries = Directory.GetFiles(path, "*", SearchOption.AllDirectories);
            for (int i = 0; i < fileEntries.Length; i++)
            {
                string myFilePath = fileEntries[i].Replace(@"\", ";");
                string myFinalPath = myFilePath.Replace(".", "'");
                for (int j = 0; j < myFinalPath.Length; j++)
                {
                    string test = myFinalPath.Substring(j, 5);
                    if (test.CompareTo("Files") == 0)
                    {
                        int number = j + 6;
                        DeleteFile(myFinalPath.Substring(number));
                        break;
                    }
                }
            }
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
            var DatabaseFiles1 = _context.CompaniesToViewFiles2.ToList();
            //var fileInDb = _context.Files.Single(c => c.Path == pathUsedForDelete);
            foreach (var DatabaseFile in DatabaseFiles1)
            {
                if (DatabaseFile.FilePath == pathUsedForDelete)
                {
                    _context.CompaniesToViewFiles2.Remove(DatabaseFile);
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
            ViewBag.AbsolotePath = Server.MapPath("~/Files/");
            var records = _context.CompaniesToViewFiles2.ToList();
            var viewModel = new ShowFilesInTableViewModel { Files = files, Records = records };

            return View(viewModel);
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
            List<string> FoldersToDeleteNoDuplicates = FoldersToDelete.Distinct().ToList();
            List<string> FilesToDeleteNoDuplicates = FilesToDelete.Distinct().ToList();
            // FoldersandFilesViewModel
            var FoldersAndFiles = new FoldersandFilesViewModel
            {
                Folders = folders,
                Files = files,
                FoldersToDelete = FoldersToDeleteNoDuplicates,
                FilesToDelete = FilesToDeleteNoDuplicates
            };
            
     
            return View(FoldersAndFiles);
        }

        [Route("Folder/StoreFolderInList/{folderPath}")]
        public ActionResult storeDeletedFoldersInArray(string folderPath)
        {
            string myFolderPath = folderPath.Replace(";", @"\");
            string path = absoloutePath + @"\" + myFolderPath;
            FoldersToDelete.Add(path);
            string[] words = folderPath.Split(';');
            string returnPath = "";
            for (int i = 0; i < words.Length - 1; i++)
            {
                returnPath = returnPath + words[i] + ";";
            }
            return RedirectToAction("DeleteMultiple/" + returnPath);
        }

        [Route("Folder/RemoveFolderFromList/{folderPath}")]
        public ActionResult RemoveDeletedFoldersFromArray(string folderPath)
        {
            string myFolderPath = folderPath.Replace(";", @"\");
            string path = Server.MapPath("~/Files/");
            path += myFolderPath;
            for(int i=0; i<FoldersToDelete.Count; i++)
            {
                if (FoldersToDelete[i].Replace(" ", "").Equals(path))
                    FoldersToDelete.RemoveAt(i);
            }
            //FoldersToDelete.Add(path);
            string[] words = folderPath.Split(';');
            string returnPath = "";
            for (int i = 0; i < words.Length - 1; i++)
            {
                returnPath = returnPath + words[i] + ";";
            }
            return RedirectToAction("DeleteMultiple/" + returnPath);
        }

        [Route("Folder/StoreFileInList/{filePath}")]
        public ActionResult StoreDeletedFilesInArray(string filePath)
        {
            string myFilePath = filePath.Replace(";", @"\");
            string myFinalPath = myFilePath.Replace("'", ".");
            string path = absoloutePath + @"\" + myFinalPath;
            FilesToDelete.Add(path);
            string[] words = filePath.Split(';');
            string returnPath = "";
            for (int i = 0; i < words.Length - 1; i++)
            {
                returnPath = returnPath + words[i] + ";";
            }
            return RedirectToAction("DeleteMultiple/" + returnPath);
        }

        [Route("Folder/RemoveFileFromList/{filePath}")]
        public ActionResult RemoveDeletedFilesInArray(string filePath)
        {
            string myFilePath = filePath.Replace(";", @"\");
            string myFinalPath = myFilePath.Replace("'", ".");
            string path = Server.MapPath("~/Files/") + myFinalPath;
            for (int i = 0; i < FilesToDelete.Count; i++)
            {
                if (FilesToDelete[i].Replace(" ", "").Equals(path))
                    FilesToDelete.RemoveAt(i);
            }
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

        [HttpPost]
        public ActionResult DeleteAllSelectedFoldersAndFiles()
        {
            List<string> FoldersToDeleteNoDuplicates = FoldersToDelete.Distinct().ToList();
            List<string> FilesToDeleteNoDuplicates = FilesToDelete.Distinct().ToList();
            for (int i = 0; i < FoldersToDeleteNoDuplicates.Count; i++)
            {
                string myFolderPath = FoldersToDeleteNoDuplicates[i].Replace(@"\", ";");
                for (int j = 0; j < myFolderPath.Length; j++)
                {
                    string test = myFolderPath.Substring(j, 5);
                    if (test.CompareTo("Files") == 0)
                    {
                        int number = j + 6;
                        DeleteFolder(myFolderPath.Substring(number));
                        break;
                    }
                }
            }
            for (int i = 0; i < FilesToDeleteNoDuplicates.Count; i++)
            {
                string myFilePath = FilesToDeleteNoDuplicates[i].Replace(@"\", ";");
                string myFinalPath = myFilePath.Replace(".", "'");
                for (int j = 0; j < myFinalPath.Length; j++)
                {
                    string test = myFinalPath.Substring(j, 5);
                    if (test.CompareTo("Files") == 0)
                    {
                        int number = j + 6;
                        DeleteFile(myFinalPath.Substring(number));
                        break;
                    }
                }
            }
            FoldersToDelete.Clear();
            FilesToDelete.Clear();
            // starting here are operations to return to folders
            string returnPath = "";
            if(FoldersToDeleteNoDuplicates.Count == 0)
            {
                if(FilesToDeleteNoDuplicates.Count == 0)
                {
                    return RedirectToAction("NothingSelected");
                }
                else
                  returnPath = FilesToDeleteNoDuplicates[0];
            }
            else
              returnPath = FoldersToDeleteNoDuplicates[0];
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

        [HttpPost]
        public ActionResult NothingSelected()
        {
            return View();
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

        [HttpPost]
        public ActionResult ConfirmPasswordForFile(FoldersandFilesViewModel viewModel, string filePath)
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

        [HttpPost]
        public ActionResult ConfirmPasswordForFolderRename(FoldersandFilesViewModel viewModel)
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

        [HttpPost]
        public ActionResult ConfirmPasswordForFileUpload(FoldersandFilesViewModel viewModel)
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

        [HttpPost]
        public ActionResult ConfirmPasswordToDeleteAllSelected(FoldersandFilesViewModel viewModel)
        {
                var userId = User.Identity.GetUserId();
                ApplicationUser user1 = UserManager.FindById(userId);
                try
                {
                    var result = UserManager.CheckPassword(user1, viewModel.Password);
                    if (result)
                    {
                        DeleteAllSelectedFoldersAndFiles();
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

        [HttpPost]
        public ActionResult ConfirmPasswordForCompanyDelete(FoldersandFilesViewModel viewModel, int id)
        {
            
                var userId = User.Identity.GetUserId();
                ApplicationUser user1 = UserManager.FindById(userId);
                try
                {
                    var result = UserManager.CheckPassword(user1, viewModel.Password);
                    if (result)
                    {
                        var companyInDb = _context.Companies.Single(c => c.Id == id);
                        var DatabaseFiles1 = _context.CompaniesToViewFiles2.ToList();
                        //var fileInDb = _context.Files.Single(c => c.Path == pathUsedForDelete);
                        foreach (var DatabaseFile in DatabaseFiles1)
                        {
                            if (DatabaseFile.CompanyName == companyInDb.Name)
                            {
                                _context.CompaniesToViewFiles2.Remove(DatabaseFile);
                                _context.SaveChanges();
                            }
                        }
                        var records = _context.UsersInCompanies.ToList();
                        foreach(var record in records)
                        {
                            if(record.CompanyName == companyInDb.Name)
                            {
                                _context.UsersInCompanies.Remove(record);
                                _context.SaveChanges();
                            }
                        }
                    //Directory.Delete(companyInDb.Path, true);
                    string myFolderPath = companyInDb.Path.Replace(@"\", ";");
                    for (int j = 0; j < myFolderPath.Length; j++)
                    {
                        string test = myFolderPath.Substring(j, 5);
                        if (test.CompareTo("Files") == 0)
                        {
                            int number = j + 6;
                            DeleteFolder(myFolderPath.Substring(number));
                            break;
                        }
                    }
                    System.IO.File.Delete(companyInDb.LogoPath);
                        _context.Companies.Remove(companyInDb);
                        _context.SaveChanges();                      
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (System.ArgumentNullException e)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            

        }

        [HttpPost]
        public ActionResult ConfirmPasswordForCompanyDelete1(FoldersandFilesViewModel viewModel, int id)
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
                        var companyInDb = _context.Companies.Single(c => c.Id == id);
                        var usersInDb = _context.Users.ToList();
                        var records = _context.UsersInCompanies.ToList();
                        List<ApplicationUser> UsersToBeAssignedToNewCompany = new List<ApplicationUser>();
                        foreach(var user in usersInDb)
                        {
                            List<UsersInCompanies> CompaniesUserAssignedTo = new List<UsersInCompanies>();
                            foreach(var record in records)
                            {
                                if (record.UserId == user.Id)
                                    CompaniesUserAssignedTo.Add(record);
                            }
                            if(CompaniesUserAssignedTo.Count == 1)
                            {
                                if (CompaniesUserAssignedTo[0].CompanyId == id)
                                    UsersToBeAssignedToNewCompany.Add(user);
                            }
                        }
                        if (UsersToBeAssignedToNewCompany.Count == 0)
                        {
                            _context.Companies.Remove(companyInDb);
                            _context.SaveChanges();
                            return Json(true, JsonRequestBehavior.AllowGet);
                        }
                        else
                            return Json("Some Users Wont be assigned to a Company", JsonRequestBehavior.AllowGet);
                        
                    }
                    else
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (System.ArgumentNullException e)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(false, JsonRequestBehavior.AllowGet);

        }

        [Route("Folder/AssignUsersToNewCompany/{companyId}")]
        public ActionResult AssignUsersToNewCompany(int companyId)
        {
            return Content(companyId.ToString());
        }

        [HttpPost]
        public ActionResult ConfirmPasswordForFileDeleteFromTable(ShowFilesInTableViewModel viewModel, string id)
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
                        string DeletePath = Server.MapPath("~/Files/") + id;
                        System.IO.File.Delete(DeletePath);
                        var FileRecords = _context.Files.ToList();
                        foreach(var FileRecord in FileRecords)
                        {
                            if(FileRecord.Path == DeletePath)
                            {
                                _context.Files.Remove(FileRecord);
                                _context.SaveChanges();
                            }
                        }
                        var FileRecord1 = _context.CompaniesToViewFiles2.ToList();
                        if(FileRecord1 != null)
                        {
                            foreach(var record in FileRecord1)
                            {
                                if(record.FilePath == DeletePath)
                                {
                                    _context.CompaniesToViewFiles2.Remove(record);
                                    _context.SaveChanges();
                                }
                            }
                        }
                        return Json(true, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        return Json(false, JsonRequestBehavior.AllowGet);
                    }

                }
                catch (System.ArgumentNullException e)
                {
                    return Json(false, JsonRequestBehavior.AllowGet);
                }
            }
            else
                return Json(false, JsonRequestBehavior.AllowGet);

        }

        public ActionResult ChangePassword()
        {
            var viewModel = new MyChangePasswordViewModel {Flag = false, Flag2 = false };
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult ChangePassword1(MyChangePasswordViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var returnModel = new MyChangePasswordViewModel { Password = viewModel.Password, OldPassword = viewModel.OldPassword, ConfirmPassword = viewModel.ConfirmPassword };
                return View("ChangePassword", returnModel);
            }
            else
            {
                var userId = User.Identity.GetUserId();
                ApplicationUser user1 = UserManager.FindById(userId);
                try
                {
                    var result = UserManager.CheckPassword(user1, viewModel.OldPassword);
                    if (result)
                    {
                        if (viewModel.Password.Any(char.IsDigit))
                        {
                            if (viewModel.Password.Any(char.IsLower))
                            {
                                if (viewModel.Password.Any(char.IsUpper))
                                {
                                    if(viewModel.Password.Any(ch => !char.IsLetterOrDigit(ch)))
                                    {
                                        if(viewModel.Password.Length > 8)
                                        {
                                            UserManager.ChangePassword(user1.Id, viewModel.OldPassword, viewModel.Password);
                                            return RedirectToAction("GetFolders1", "Folder");
                                        }
                                    }
                                }
                            }
                        }
                        var returnModel = new MyChangePasswordViewModel { Password = viewModel.Password, OldPassword = viewModel.OldPassword, ConfirmPassword = viewModel.ConfirmPassword, Flag2 = true };
                        return View("ChangePassword", returnModel);
                    }
                    else
                    {
                        var returnModel = new MyChangePasswordViewModel { Password = viewModel.Password, OldPassword = viewModel.OldPassword, ConfirmPassword = viewModel.ConfirmPassword, Flag = true };
                        return View("ChangePassword", returnModel);
                    }

                }
                catch (System.ArgumentNullException e)
                {
                    ModelState.AddModelError("", "Wrong Password");
                    return View("ChangePassword", viewModel);
                }
            }
        }
    }
}