using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Razor_11_Identity.Models
{
    public class Article
    {
        [Key]
        public int id { set; get; }
        [StringLength(255, ErrorMessage = "{0} has max {1} characters ")]
        [Required(ErrorMessage = "{0} is required")]
        [DisplayName("Tiêu đề")]
        public string title { set; get; }
        [DataType(DataType.Date)]
        [Required(ErrorMessage = "{0} is required")]
        [DisplayName("Ngày tạo bài")]
        public DateTime created { set; get; }
        [StringLength(255, ErrorMessage = "{0} has max {1} characters ")]
        [Required(ErrorMessage = "{0} is required")]
        [DisplayName("Nội dung")]
        public string content { set; get; }
    }
}