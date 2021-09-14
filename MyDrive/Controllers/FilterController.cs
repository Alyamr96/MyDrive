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
            if (model.FilterSelected == "Date of Upload")
                return View("DateFilterView");
            else if (model.FilterSelected == "User Name")
                return View("UserNameFilter");
            else if (model.FilterSelected == "Size")
                return View("SizeFilter");
            else if (model.FilterSelected == "Folder/File Name")
                return View("FolderFileNameFilter");
            else
                return View("CompanyFilter");
        }
    }
}