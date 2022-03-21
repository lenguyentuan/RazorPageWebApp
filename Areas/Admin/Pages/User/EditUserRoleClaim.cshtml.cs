using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorWebApp.Models;

namespace RazorWebApp.Admin.User
{
    public class EditUserRoleClaimModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        private readonly AppDbContext _context;

        public EditUserRoleClaimModel(UserManager<AppUser> userManager, AppDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public class InputModel
        {
            [DisplayName("Claim Type")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} has min {2} characters and max {1} characters")]
            [Required(ErrorMessage = "{0} is required")]
            public string ClaimType { set; get; }
            [DisplayName("Claim Value")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} has min {2} characters and max {1} characters")]
            [Required(ErrorMessage = "{0} is required")]
            public string ClaimValue { set; get; }
        }
        [BindProperty]
        public InputModel Input { set; get; }

        public IActionResult OnGet()
        {
            return Page();
        }

        public async Task<IActionResult> OnPostEditClaimAsync()
        {

            return Page();
        }
    }
}
