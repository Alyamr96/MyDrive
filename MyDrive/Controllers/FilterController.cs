using MyDrive.Models;
using MyDrive.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MyDrive.Controllers
{
    public class FilterController : Controller
    {
        private ApplicationDbContext _context;

        public FilterController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Filter
        public ActionResult FilterOptions()
        {
            List<string> filters = new List<string> {"User Email","Date of Upload", "Size", "Folder/File Name"};
            var viewModel = new FiltersAppliedViewModel { FiltersApplied = filters};
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult FilterSelected(FiltersAppliedViewModel model)
        {
            var DateFilterViewModel1 = new DateFilterViewModel {FromDate = null, ToDate = null };
            if (model.FilterSelected == "Date of Upload")
                return View("DateFilterView", DateFilterViewModel1);
            else if (model.FilterSelected == "User Email")
                return View("UserEmailFilter");
            else if (model.FilterSelected == "Size")
                return View("SizeFilter");
            else if (model.FilterSelected == "Folder/File Name")
                return View("FolderFileNameFilter");
            else
                return View("CompanyFilter");
        }

        [HttpPost]
        public ActionResult FilterByDate(DateFilterViewModel viewModel)
        {
            if(!ModelState.IsValid)
            {
                var viewModelToReturn = new DateFilterViewModel
                {
                    FromDate = viewModel.FromDate,
                    ToDate = viewModel.ToDate
                };
                return View("DateFilterView", viewModelToReturn);
            }
            var FileDates = _context.Files.ToList();
            List<FileModel> FilesToShow = new List<FileModel>();
            DateTime? fromDate = viewModel.FromDate;
            DateTime? toDate = viewModel.ToDate;

            foreach(var File in FileDates)
            {
                if (DateTime.Compare(File.Date.Date, (DateTime)fromDate.Value.Date) >= 0 && DateTime.Compare(File.Date.Date, (DateTime)toDate.Value.Date) <= 0)
                    FilesToShow.Add(File);
            }

            return View(FilesToShow);

        }

        [HttpPost]
        public ActionResult FilterBySize(SizeFilterViewModel viewModel)
        {
            if (!ModelState.IsValid)
            {
                var viewModelToReturn = new SizeFilterViewModel
                {
                    MinSize = viewModel.MinSize,
                    MaxSize = viewModel.MaxSize
                };
                return View("SizeFilter", viewModelToReturn);
            }
            else
            {
                string[] fileEntries = Directory.GetFiles(Server.MapPath("~/Files/"), "*", SearchOption.AllDirectories);
                if(viewModel.MaxSize == 0)
                {
                    List<FilterBySizeViewModel> ReturnList = new List<FilterBySizeViewModel>();
                    foreach(var file in fileEntries)
                    {
                        int length = (int)(new System.IO.FileInfo(file).Length / 1024000);
                        if(length > viewModel.MinSize)
                        {
                            string FileNormalized = file.Replace(@"\", ";");
                            string[] words = FileNormalized.Split(';');
                            string FileName = words[words.Length - 1];
                            string FilePath = "";
                            for (int i = 0; i < file.Length; i++)
                            {
                                string test = file.Substring(i, 5);
                                if (test.CompareTo("Files") == 0)
                                {
                                    int number = i + 6;
                                    FilePath = file.Substring(number);
                                    break;
                                }
                            }
                            FilePath = "home/" + FilePath;
                            var record = new FilterBySizeViewModel {Name = FileName, Path = FilePath, Size = length };
                            ReturnList.Add(record);
                        }
                    }
                    return View(ReturnList);
                }else if(viewModel.MinSize == 0)
                {
                    List<FilterBySizeViewModel> ReturnList = new List<FilterBySizeViewModel>();
                    foreach (var file in fileEntries)
                    {
                        int length = (int)(new System.IO.FileInfo(file).Length / 1024000);
                        if (length < viewModel.MaxSize)
                        {
                            string FileNormalized = file.Replace(@"\", ";");
                            string[] words = FileNormalized.Split(';');
                            string FileName = words[words.Length - 1];
                            string FilePath = "";
                            for (int i = 0; i < file.Length; i++)
                            {
                                string test = file.Substring(i, 5);
                                if (test.CompareTo("Files") == 0)
                                {
                                    int number = i + 6;
                                    FilePath = file.Substring(number);
                                    break;
                                }
                            }
                            FilePath = "home/" + FilePath;
                            var record = new FilterBySizeViewModel { Name = FileName, Path = FilePath, Size = length };
                            ReturnList.Add(record);
                        }
                    }
                    return View(ReturnList);
                }
                else
                {
                    List<FilterBySizeViewModel> ReturnList = new List<FilterBySizeViewModel>();
                    foreach (var file in fileEntries)
                    {
                        int length = (int)(new System.IO.FileInfo(file).Length / 1024000);
                        if (length > viewModel.MinSize && length < viewModel.MaxSize)
                        {
                            string FileNormalized = file.Replace(@"\", ";");
                            string[] words = FileNormalized.Split(';');
                            string FileName = words[words.Length - 1];
                            string FilePath = "";
                            for (int i = 0; i < file.Length; i++)
                            {
                                string test = file.Substring(i, 5);
                                if (test.CompareTo("Files") == 0)
                                {
                                    int number = i + 6;
                                    FilePath = file.Substring(number);
                                    break;
                                }
                            }
                            FilePath = "home/" + FilePath;
                            var record = new FilterBySizeViewModel { Name = FileName, Path = FilePath, Size = length };
                            ReturnList.Add(record);
                        }
                    }
                    return View(ReturnList);
                }
                //int length = (int)(new System.IO.FileInfo(fileEntries[1]).Length/1024000);
                //return Content(viewModel.MaxSize.ToString());
            }       
        }

        [HttpPost]
        public ActionResult FilterByName()
        {
            string NameToSearch = Request["FolderFileName"].ToString();
            if (NameToSearch == "")
                return View("FolderFileNameFilter");
            // Getting All Folders matching the search word
            List<string> dirs = new List<string>(Directory.EnumerateDirectories(Server.MapPath("~/Files/"), "*", SearchOption.AllDirectories));
            List<string> dirsToShow = new List<string>(dirs.Count);
            foreach(var dir in dirs)
            {
                string PathAfterFileWord = "";
                for (int i = 0; i < dir.Length; i++)
                {
                    string test = dir.Substring(i, 5);
                    if (test.CompareTo("Files") == 0)
                    {
                        int number = i + 6;
                        PathAfterFileWord = dir.Substring(number);
                        break;
                    }
                }
                string testFolder = PathAfterFileWord.Replace(@"\", ";");
                testFolder = testFolder.Replace(@"/", ";");
                string[] words = testFolder.Split(';');
                if (words[words.Length - 1].ToLower().Contains(NameToSearch.ToLower()))
                    dirsToShow.Add(dir);
            }
            List<string> foldersPathAfterFile = new List<string>(dirsToShow.Count);
            List<string> folderNames = new List<string>(dirsToShow.Count);
            List<Folder> folders = new List<Folder>(dirsToShow.Count);
            foreach (var directory in dirsToShow)
            {
                string fullpath = Path.GetFullPath(directory).TrimEnd(Path.DirectorySeparatorChar);
                string projectName = directory.Split(Path.DirectorySeparatorChar).Last();
                folderNames.Add(projectName);
            }
            foreach (var directory in dirsToShow)
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
            for (int i = 0; i < dirsToShow.Count; i++)
            {
                folders.Add(new Folder { Name = folderNames[i], Path = foldersPathAfterFile[i] });
            }
            // Files Area
            string[] fileEntries = Directory.GetFiles(Server.MapPath("~/Files/"), "*", SearchOption.AllDirectories);
            List<string> fileEntriesToShow = new List<string>(fileEntries.Length);
            foreach (var file in fileEntries)
            {
                string PathAfterFileWord = "";
                for (int i = 0; i < file.Length; i++)
                {
                    string test = file.Substring(i, 5);
                    if (test.CompareTo("Files") == 0)
                    {
                        int number = i + 6;
                        PathAfterFileWord = file.Substring(number);
                        break;
                    }
                }
                string testFile = PathAfterFileWord.Replace(@"\", ";");
                testFile = testFile.Replace(@"/", ";");
                string[] words = testFile.Split(';');
                if (words[words.Length-1].ToLower().Contains(NameToSearch.ToLower()))
                    fileEntriesToShow.Add(file);
            }
            List<string> filesPathAfterFile = new List<string>(fileEntriesToShow.Count);
            string[] fileNames = new string[fileEntriesToShow.Count];
            List<FileModel> files = new List<FileModel>(fileEntriesToShow.Count);
            for (int i = 0; i < fileEntriesToShow.Count; i++)
            {
                string fullpath = Path.GetFullPath(fileEntriesToShow[i]).TrimEnd(Path.DirectorySeparatorChar);
                string projectName = fileEntriesToShow[i].Split(Path.DirectorySeparatorChar).Last();
                fileNames[i] = projectName;
            }
            for (int j = 0; j < fileEntriesToShow.Count; j++)
            {
                for (int i = 0; i < fileEntriesToShow[j].Length; i++)
                {
                    string test = fileEntriesToShow[j].Substring(i, 5);
                    if (test.CompareTo("Files") == 0)
                    {
                        int number = i + 6;
                        filesPathAfterFile.Add(fileEntriesToShow[j].Substring(number));
                        break;
                    }
                }
            }
            for (int i = 0; i < fileEntriesToShow.Count; i++)
            {
                files.Add(new FileModel { Name = fileNames[i], Path = filesPathAfterFile[i] });
            }
            // Folder and files view Model
            var FoldersAndFiles = new FoldersandFilesViewModel
            {
                Folders = folders,
                Files = files
            };

            return View(FoldersAndFiles);
            //return Content(NameToSearch + "//" + "Download".Substring(0, NameToSearch.Length));
        }

        [HttpPost]
        public ActionResult FilterByEmail()
        {
            string EmailToSearch = Request["Email"].ToString();
            List<FileModel> files = new List<FileModel>();
            if (EmailToSearch == "")
                return View("UserEmailFilter");
            try
            {
                var UserInDb = _context.Users.Single(c => c.Email == EmailToSearch);
            }
            catch (Exception e)
            {
                return View("UserEmailFilter");
            }
            var FileUploads = _context.Files.ToList();
            var UserFromDb = _context.Users.Single(c => c.Email == EmailToSearch);
            foreach(var FileUpload in FileUploads)
            {
                if(FileUpload.UserId == UserFromDb.Id)
                {
                    string PathAfterFileWord = "";
                    for (int i = 0; i < FileUpload.Path.Length; i++)
                    {
                        string test = FileUpload.Path.Substring(i, 5);
                        if (test.CompareTo("Files") == 0)
                        {
                            int number = i + 6;
                            PathAfterFileWord = FileUpload.Path.Substring(number);
                            break;
                        }
                    }
                    var record = new FileModel {Name = FileUpload.Name, Path = PathAfterFileWord, Date = FileUpload.Date };
                    files.Add(record);
                }
            }

            return View(files);
        }
    }
}