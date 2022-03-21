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

namespace RazorWebApp.Admin.Role
{
    public class EditModel : RolePageModel
    {
        public EditModel(RoleManager<IdentityRole> roleManager, AppDbContext context) : base(roleManager, context)
        {
        }

        public class InputModel
        {
            [DisplayName("New Role Name")]
            [Required]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} has min {2} characters and max {1} characters")]
            public string Name { set; get; }
        }
        [BindProperty]
        public InputModel input { set; get; }

        public IdentityRole role { set; get; }
        public async Task<IActionResult> OnGet(string id)
        {
            if (id == null) return NotFound($"Can not find role has ID: {id} ");
            role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound($"Can not find role in database ");

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {

            if (id == null) return NotFound($"Can not find role has ID: {id} ");
            role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound($"Can not find role in database ");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            role.Name = input.Name;

            var result = await _roleManager.UpdateAsync(role);
            if (result.Succeeded)
            {
                StatusMessage = $"Update {role.Name} successfully";
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
