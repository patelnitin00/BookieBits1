﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookieBits.Models
{
    public class Product
    {
        public int ID { get; set; }
        [Required]
        public string Title { get; set; }

        public string Description { get; set; }
        [Required]
        public string ISBN { get; set; }
        [Required]
        public string Author { get; set; }
        [Required]
        [Range(1,1000)]
        [Display(Name ="List Price")]
        public double ListPrice { get; set; }
        [Required]
        [Range(1,1000)]
        [Display(Name ="Price for 1-50")]
        public double Price { get; set; }
        [Required]
        [Range(1, 1000)]
        [Display(Name = "Price for 51-100")]
        public double Price50 { get; set; }
        [Required]
        [Range(1, 1000)]
        [Display(Name = "Price for 100+")]
        public double Price100 { get; set; }
        [ValidateNever]
        public string? ImageUrl { get; set; }

        //for foreign key relation mapping
        [Display(Name = "Category")]
        [Required]
        public int CategoryID { get; set; }
        [ForeignKey("CategoryID")] //if not written than also OK
       [ValidateNever]
        public Category category { get; set; }
        [Display(Name ="Cover Type")]
        [Required]
        public int CoverTypeID { get; set; }
        [ValidateNever]
        public CoverType coverType { get; set; }

    }
}
