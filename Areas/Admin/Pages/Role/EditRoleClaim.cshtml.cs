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
    public class EditRoleClaimModel : RolePageModel
    {
        public EditRoleClaimModel(RoleManager<IdentityRole> roleManager, AppDbContext context) : base(roleManager, context)
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

        public IdentityRoleClaim<string> claim { set; get; }


        public IActionResult OnGet(int claimid)
        {
            claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();

            if (claim == null) return NotFound($"Can not find claim has ID: {claimid}");

            input = new InputModel()
            {
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue
            };
            return Page();

        }

        public async Task<IActionResult> OnPostAsync(int claimid)
        {
            claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound($"Can not find claim has ID: {claimid}");
            if (_context.RoleClaims.Any(c => c.Id != claimid && c.ClaimType == input.ClaimType && c.ClaimValue == input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "Claim was exist");
            }
            var role = await _roleManager.FindByIdAsync(claim.RoleId);
            var deleteClaim = new Claim(claim.ClaimType, claim.ClaimValue);
            var updateClaim = new Claim(input.ClaimType, input.ClaimValue);
            var resultDelete = await _roleManager.RemoveClaimAsync(role, deleteClaim);
            if (!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }
            var resultUpdate = await _roleManager.AddClaimAsync(role, updateClaim);

            if (!resultUpdate.Succeeded)
            {
                resultUpdate.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }

            StatusMessage = "Update Claim successfully";

            return RedirectToPage("./Edit", new { id = claim.RoleId });
        }

        public async Task<IActionResult> OnPostDeleteAsync(int claimid)
        {
            if (claimid == null) return NotFound("Can not delete this claim");
            claim = _context.RoleClaims.Where(c => c.Id == claimid).FirstOrDefault();
            if (claim == null) return NotFound("Can not delete this claim");

            var role = await _roleManager.FindByIdAsync(claim.RoleId);
            var deleteClaim = new Claim(claim.ClaimType, claim.ClaimValue);
            var result = await _roleManager.RemoveClaimAsync(role, deleteClaim);

            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }

            StatusMessage = $"You just delete claim {claim.ClaimType}: {claim.ClaimValue} successfully";
            return RedirectToPage("./Edit", new { id = claim.RoleId });
        }
    }
}
