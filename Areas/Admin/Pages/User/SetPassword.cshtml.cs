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
    public class SetPasswordModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;

        public SetPasswordModel(UserManager<AppUser> userManager)
        {
            _userManager = userManager;
        }

        public class InputModel
        {
            [DisplayName("New Password")]
            [Required]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} has min {2} characters and max {1} characters")]
            [DataType(DataType.Password)]
            public string NewPassword { set; get; }

            [DisplayName("Confirm Password")]
            [Required]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} has min {2} characters and max {1} characters")]
            [DataType(DataType.Password)]
            [Compare("NewPassword", ErrorMessage = "Password and Confirm password do not match !")]

            public string ConfirmPassword { set; get; }
        }
        [BindProperty]
        public InputModel input { set; get; }

        public AppUser user { set; get; }
        [TempData]
        public string StatusMessage { set; get; }
        public async Task<IActionResult> OnGetAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound($"Can not find user has ID: {id}");
            user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound($"Can not find user has ID: {id}");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound($"Can not find user has ID: {id}");
            user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound($"Can not find user has ID: {id}");

            await _userManager.RemovePasswordAsync(user);

            var resultAddPassword = await _userManager.AddPasswordAsync(user, input.NewPassword);

            if (resultAddPassword.Succeeded)
            {
                StatusMessage = $"Add Password for user {user.UserName} successfully ";
                return RedirectToPage("./Index");
            }
            else
            {
                resultAddPassword.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }
            return Page();
        }


    }
}
