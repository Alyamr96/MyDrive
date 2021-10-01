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

                    //var justCreatedCompany = _context.Companies.Single(c => c.Name == company.Name);
                    _context.UsersInCompanies.Add(new UsersInCompanies { UserId = "a26e8a5a-1609-459f-a213-2d1ad04c8ab6", CompanyName = company.Name });
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
                        Console.WriteLine(e.Message);
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
                    _context.UsersInCompanies.Add(new UsersInCompanies { UserId = "a26e8a5a-1609-459f-a213-2d1ad04c8ab6", CompanyName = company.Name });
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
                        Console.WriteLine(e.Message);
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
            ViewBag.CompanyId = CompanyFromDb.Id;
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult AssignToCompany(string CompanyName, string UserId)
        {
            if (CompanyName == null)
                return Json(false, JsonRequestBehavior.AllowGet);          
            else if (UserId == null)
                return Json(false, JsonRequestBehavior.AllowGet);
            else
            {
                var company = _context.Companies.Single(c => c.Name == CompanyName);
                var record = new UsersInCompanies {CompanyId = company.Id, UserId = UserId, CompanyName = company.Name };
                _context.UsersInCompanies.Add(record);
                _context.SaveChanges();
                return Json(true, JsonRequestBehavior.AllowGet);
            }             
        }

        [HttpPost]
        public ActionResult RemoveFromCompany(string CompanyName, string UserId)
        {
            if (CompanyName == null)
                return Json(false, JsonRequestBehavior.AllowGet);
            else if (UserId == null)
                return Json(false, JsonRequestBehavior.AllowGet);
            else
            {
                var company = _context.Companies.Single(c => c.Name == CompanyName);
                var records = _context.UsersInCompanies.ToList();
                foreach(var record in records)
                {
                    if(record.CompanyId == company.Id)
                    {
                        if(record.UserId == UserId)
                        {
                            _context.UsersInCompanies.Remove(record);
                            _context.SaveChanges();
                        }
                    }
                }
                return Json(true, JsonRequestBehavior.AllowGet);
            }
        }
    }
}