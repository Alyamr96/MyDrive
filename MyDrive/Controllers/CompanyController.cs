using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyDrive.Models;

namespace MyDrive.Controllers
{
    public class CompanyController : Controller
    {
        private ApplicationDbContext _context;

        public CompanyController()
        {
            _context = new ApplicationDbContext();
        }

        protected override void Dispose(bool disposing)
        {
            _context.Dispose();
        }
        // GET: Company
        public ActionResult Index()
        {
            var CompaniesInDb = _context.Companies.ToList();
            return View(CompaniesInDb);
        }

        [Route("Company/CreateCompany")]
        public ActionResult CreateCompany()
        {
            return View();
        }

        [HttpPost] 
        public ActionResult CreateNewCompany(HttpPostedFileBase postedFile, Company company)
        {
            if (!ModelState.IsValid)
            {
                Company ReturnCompany = new Company { Name = company.Name };
                return View("CreateCompany", ReturnCompany);
            }
            else
            {
                string filePath = "";
                filePath = Server.MapPath("~/Images/");

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
                    return RedirectToAction("FileExistsView", "Folder");


                string pathInDb = Server.MapPath("~/Files/") + company.Name;
                if (!Directory.Exists(pathInDb))
                {
                    Directory.CreateDirectory(pathInDb);
                    var CompanyInDb = new Company { Name = company.Name, Path = pathInDb, LogoPath = filePath };
                    _context.Companies.Add(CompanyInDb);
                    _context.SaveChanges();
                }    
                else
                    return RedirectToAction("FolderExistsView");
                

                return RedirectToAction("Index");
            }
        }
    }
}