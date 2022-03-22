using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RazorWebApp.Models;
using Microsoft.EntityFrameworkCore;

namespace RazorWebApp.Admin.Role
{
    [Authorize(Policy = "AllowEditRole")]
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

        public List<IdentityRoleClaim<string>> Claims { set; get; }

        public IdentityRole role { set; get; }
        public async Task<IActionResult> OnGet(string id)
        {
            if (id == null) return NotFound($"Can not find role has ID: {id} ");
            role = await _roleManager.FindByIdAsync(id);
            if (role == null) return NotFound($"Can not find role in database ");

            input = new InputModel()
            {
                Name = role.Name
            };

            Claims = await _context.RoleClaims.Where(rc => rc.RoleId == role.Id).ToListAsync();
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
