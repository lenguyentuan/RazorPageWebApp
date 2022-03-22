using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using App.Models;

namespace App.Admin.Role
{
    public class CreateModel : RolePageModel
    {
        public CreateModel(RoleManager<IdentityRole> roleManager, AppDbContext context) : base(roleManager, context)
        {

        }

        public class InputModel
        {
            [DisplayName("Role Name")]
            [Required]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} has min {2} characters and max {1} characters")]
            public string Name { set; get; }
        }
        [BindProperty]
        public InputModel input { set; get; }

        public IdentityRole role { set; get; }


        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            role = new IdentityRole(input.Name);
            var result = await _roleManager.CreateAsync(role);

            if (result.Succeeded)
            {
                StatusMessage = $"Create {input.Name} successfully";
                return RedirectToPage("./Index");
            }
            else
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }


            return Page();
        }


    }
}
