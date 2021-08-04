using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using MyDrive.ViewModels;

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
            string targetDirectory = @"C:\Users\dell\Desktop\MyDrive\MyDrive\Files";
            string[] fileEntries = Directory.GetFiles(targetDirectory);
            string testing;
            string[] filesNames = new String[fileEntries.Length];
            for (int i = 0; i < fileEntries.Length; i++)
            {
                testing = fileEntries[i].Substring(44);
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
            string targetDirectory = @"C:\Users\dell\Desktop\MyDrive\MyDrive\Files";
            string[] fileEntries = Directory.GetFiles(targetDirectory); 
            
            System.IO.File.Delete(fileEntries[Id]);
            

            return RedirectToAction("GetFiles");
        }
    }
}