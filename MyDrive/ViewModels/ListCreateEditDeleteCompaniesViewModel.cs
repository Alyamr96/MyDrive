using MyDrive.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MyDrive.ViewModels
{
    public class ListCreateEditDeleteCompaniesViewModel
    {
        public List<Company> Companies { get; set; }

        public Company CreateCompany { get; set; }
    }
}