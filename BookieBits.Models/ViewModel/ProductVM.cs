using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookieBits.Models.ViewModel
{
    public class ProductVM
    {
        public Product Product { get; set; }

        //public int catw
        [ValidateNever]
        public IEnumerable<SelectListItem> CategoryListItems { get; set; }  
        [ValidateNever]
        public IEnumerable<SelectListItem> CoverTypeListItems { get; set; } 
    }
}
