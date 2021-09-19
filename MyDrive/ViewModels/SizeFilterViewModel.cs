using Foolproof;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MyDrive.ViewModels
{
    public class SizeFilterViewModel
    {
        [Display(Name = "Minimum File Size")]
        [RegularExpression("([0-9]*)", ErrorMessage ="Count must be a natural number")]
        public int MinSize { get; set; }

        [Display(Name = "Maximum File Size")]
        [RegularExpression("([0-9]*)", ErrorMessage = "Count must be a natural number")]
        public int MaxSize { get; set; }

    }
}