using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MyDrive.Models;
using MyDrive.ViewModels;

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
            var viewModel = new ListCreateEditDeleteCompaniesViewModel {Companies = CompaniesInDb };
            return View(viewModel);
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
                if (System.IO.File.Exists(filePath))
                {
                    string pathInDbDuplicate = Server.MapPath("~/Files/") + company.Name;
                    if (!Directory.Exists(pathInDbDuplicate))
                    {
                        Directory.CreateDirectory(pathInDbDuplicate);
                        var CompanyInDb = new Company { Name = company.Name, Path = pathInDbDuplicate, LogoPath = filePath };
                        _context.Companies.Add(CompanyInDb);
                        _context.SaveChanges();
                    }
                    else
                        return RedirectToAction("FolderExistsView");


                    return RedirectToAction("Index");
                }
                else
                {
                    try
                    {
                        postedFile.SaveAs(filePath);
                    }
                    catch (System.IO.DirectoryNotFoundException e)
                    {
                        Company ReturnCompany = new Company { Name = company.Name };
                        return View("CreateCompany", ReturnCompany);
                    }
                }

                string extention = Path.GetExtension(filePath);
                if (extention != ".png")
                {
                    if (extention != ".jpg")
                    {
                        if (extention != ".jpeg")
                        {
                            if (extention != ".jfif")
                            {
                                System.IO.File.Delete(filePath);
                                Company ReturnCompany = new Company { Name = company.Name };
                                return View("CreateCompany", ReturnCompany);
                            }
                        }
                    }
                }


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

        [HttpGet]
        public ActionResult FolderExistsView()
        {
            return View();
        }

        public ActionResult EditCompany(int id)
        {
            var company = _context.Companies.SingleOrDefault(c => c.Id == id);
            if (company == null)
                return HttpNotFound();
            
            Company EditCompany = new Company {Name = company.Name, Path= company.Path, Id = company.Id, LogoPath = company.LogoPath };
            return View(EditCompany);
        }

        [HttpPost]
        public ActionResult EditCompany1(HttpPostedFileBase postedFile, Company company)
        {
            if (!ModelState.IsValid)
            {
                Company ReturnCompany = new Company { Name = company.Name };
                return View("CreateCompany", ReturnCompany);
            }
            else
            {
                var companyInDb = _context.Companies.Single(c => c.Id == company.Id);
                string filePath = "";
                filePath = Server.MapPath("~/Images/");

                if (postedFile == null)
                    postedFile = Request.Files["userFile"];

                if (!Directory.Exists(filePath))
                    Directory.CreateDirectory(filePath);

                filePath = filePath + Path.GetFileName(postedFile.FileName);
                if (System.IO.File.Exists(filePath))
                {
                    string pathInDbDuplicate = Server.MapPath("~/Files/") + company.Name;
                    if (!Directory.Exists(pathInDbDuplicate))
                    {
                        Directory.CreateDirectory(pathInDbDuplicate);
                        companyInDb.Name = company.Name;
                        companyInDb.Path = pathInDbDuplicate;
                        companyInDb.LogoPath = filePath;
                        _context.SaveChanges();
                    }
                    else
                        return RedirectToAction("FolderExistsView");


                    return RedirectToAction("Index");
                }
                else
                {
                    try
                    {
                        postedFile.SaveAs(filePath);
                    }
                    catch (System.IO.DirectoryNotFoundException e)
                    {
                        Company ReturnCompany = new Company { Name = company.Name };
                        return View("CreateCompany", ReturnCompany);
                    }
                }

                string extention = Path.GetExtension(filePath);
                if (extention != ".png")
                {
                    if (extention != ".jpg")
                    {
                        if (extention != ".jpeg")
                        {
                            if (extention != ".jfif")
                            {
                                System.IO.File.Delete(filePath);
                                Company ReturnCompany = new Company { Name = company.Name };
                                return View("CreateCompany", ReturnCompany);
                            }
                        }
                    }
                }


                string pathInDb = Server.MapPath("~/Files/") + company.Name;
                if (!Directory.Exists(pathInDb))
                {
                    Directory.CreateDirectory(pathInDb);
                    companyInDb.Name = company.Name;
                    companyInDb.Path = pathInDb;
                    companyInDb.LogoPath = filePath;
                    _context.SaveChanges();
                }
                else
                    return RedirectToAction("FolderExistsView");


                return RedirectToAction("Index");
            }
        }

        public ActionResult ManageUsers(int id)
        {
            List<ApplicationUser> UsersNotInCompany = new List<ApplicationUser>();
            List<ApplicationUser> UsersInCompany = new List<ApplicationUser>();
            var usersInDb = _context.Users.ToList();
            var usersInCompanies = _context.UsersInCompanies.ToList();
            foreach( var user in usersInDb)
            {
                Boolean flag = false;
                string userId = user.Id;
                foreach(var record in usersInCompanies)
                {
                    if (record.UserId == userId && record.CompanyId == id)
                    {
                        UsersInCompany.Add(user);
                        flag = true;
                    }                       
                }
                if (flag == false)
                    UsersNotInCompany.Add(user);
            }
            var viewModel = new UsersInCompaniesViewModel { UsersInCompany = UsersInCompany, UsersNotInCompany = UsersNotInCompany};
            var CompanyFromDb = _context.Companies.Single(c => c.Id == id);
            ViewBag.CompanyName = CompanyFromDb.Name;
            return View(viewModel);
        }

        public ActionResult AssignToCompany(string CompanyName, int UserEmail)
        {
            return Content(CompanyName + UserEmail);
        }
    }
}