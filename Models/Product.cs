using System;
using System.ComponentModel.DataAnnotations;

namespace Razor_09_EntityFramework.Models
{
    public class Product
    {
        [Key]
        public int id { set; get; }
        [StringLength(50, ErrorMessage = "{0} has max {1} characters ")]
        [Required(ErrorMessage = "{0} is required")]
        public string name { set; get; }
        [Required(ErrorMessage = "{0} is required")]
        public string description { set; get; }
        [Required(ErrorMessage = "{0} is required")]
        [Range(10000, 20000, ErrorMessage = "{0} is not between {1} and {2}")]
        public int price { set; get; }
    }
}