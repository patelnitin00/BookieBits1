using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BookieBits.Models
{
    public class ShoppingCart
    {
        public int Id { get; set; }


        public int ProductID { get; set; }
       
        [ValidateNever]
        public Product Product { get; set; }
        [Range(1, 1000, ErrorMessage = "Please enter value between 1 and 1000")]
        public int Count { get; set; }

        public string ApplicationUserID { get; set; }
        [ForeignKey("ApplicationUserID")]
        [ValidateNever]
        public ApplicationUser ApplicationUser { get; set; }

        //this property is to hold dynamic price after user increases the qty
        //but we don not need this to be added in database
        //so we will use notmapped annotation
        [NotMapped]
        public double Price { get; set; }
    }
}
