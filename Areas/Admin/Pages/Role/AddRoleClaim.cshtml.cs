using System.Security.Claims;
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
    public class AddRoleClaim : RolePageModel
    {
        public AddRoleClaim(RoleManager<IdentityRole> roleManager, AppDbContext context) : base(roleManager, context)
        {

        }

        public class InputModel
        {
            [DisplayName("Claim Type")]
            [Required(ErrorMessage = "{0} is required")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} has min {2} characters and max {1} characters")]
            public string ClaimType { set; get; }

            [DisplayName("Claim Value")]
            [Required(ErrorMessage = "{0} is required")]
            [StringLength(256, MinimumLength = 3, ErrorMessage = "{0} has min {2} characters and max {1} characters")]
            public string ClaimValue { set; get; }
        }
        [BindProperty]
        public InputModel input { set; get; }

        public IdentityRole role { set; get; }

        public Claim claim { set; get; }


        public async Task<IActionResult> OnGetAsync(string roleid)
        {
            if (string.IsNullOrEmpty(roleid)) return NotFound($"Can not add claim for role has ID: {roleid}");
            role = await _roleManager.FindByIdAsync(roleid);
            return Page();

        }

        public async Task<IActionResult> OnPostAsync(string roleid)
        {
            if (string.IsNullOrEmpty(roleid)) return NotFound($"Can not add claim for role has ID: {roleid}");
            role = await _roleManager.FindByIdAsync(roleid);
            if (role == null) return NotFound($"Can not add claim for role has ID: {roleid}");
            if (!ModelState.IsValid)
            {
                return Page();
            }


            if ((await _roleManager.GetClaimsAsync(role)).Any(r => r.Type == input.ClaimType && r.Value == input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, $"This Claim was exist in role {role.Name}");
                return Page();
            }

            claim = new Claim(input.ClaimType, input.ClaimValue);

            var result = await _roleManager.AddClaimAsync(role, claim);

            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }

            StatusMessage = "Add claim successfully ";

            return RedirectToPage("./Edit", new { id = roleid });
        }


    }
}
