using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorWebApp.Models;

namespace RazorWebApp.Admin.Role
{
    public class DeleteModel : RolePageModel
    {
        public DeleteModel(RoleManager<IdentityRole> roleManager, AppDbContext context) : base(roleManager, context)
        {

        }

        public IdentityRole role { set; get; }
        public async Task<IActionResult> OnGet(string id)
        {
            role = await _roleManager.FindByIdAsync(id);

            if (role != null)
            {
                return Page();
            }
            else
            {
                StatusMessage = $"Role {role.Name} invalid";
                return RedirectToPage("./Index");
            }
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (id == null) return NotFound($"Can not find this role has ID: {id}");
            role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound("Can not find this role in database");

            var result = await _roleManager.DeleteAsync(role);
            if (result.Succeeded)
            {
                StatusMessage = $"Delete {role.Name} successfully";
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
