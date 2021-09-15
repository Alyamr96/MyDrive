using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyDrive.ViewModels
{
    public class DateFilterViewModel
    {
        [Required(ErrorMessage ="From Date Is Required")]
        [Display(Name = "From Date")]
        public DateTime? FromDate { get; set; }
        
        [Required(ErrorMessage ="To Date Is Required")]
        [GreaterThan("FromDate")]
        [Display(Name = "To Date")]
        public DateTime? ToDate { get; set; }
    }
}