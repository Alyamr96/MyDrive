using MyDrive.Models;
using MyDrive.ViewModels;
using System;
using System.Collections.Generic;
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
            List<string> filters = new List<string> { "User Name", "Date of Upload", "Size", "Folder/File Name", "Company"};
            var viewModel = new FiltersAppliedViewModel { FiltersApplied = filters};
            return View(viewModel);
        }

        [HttpPost]
        public ActionResult FilterSelected(FiltersAppliedViewModel model)
        {
            var DateFilterViewModel1 = new DateFilterViewModel {FromDate = null, ToDate = null };
            if (model.FilterSelected == "Date of Upload")
                return View("DateFilterView", DateFilterViewModel1);
            else if (model.FilterSelected == "User Name")
                return View("UserNameFilter");
            else if (model.FilterSelected == "Size")
                return View("SizeFilter");
            else if (model.FilterSelected == "Folder/File Name")
                return View("FolderFileNameFilter");
            else
                return View("CompanyFilter");
        }

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
    }
}