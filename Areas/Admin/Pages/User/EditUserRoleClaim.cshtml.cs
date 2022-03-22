using System.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using App.Models;

namespace App.Admin.User
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
        public string StatusMessage { set; get; }


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

        public AppUser user { set; get; }

        public IdentityUserClaim<string> userclaim { set; get; }

        public IActionResult OnGet() => NotFound("Can not access by method get");



        public async Task<IActionResult> OnGetAddClaimAsync(string userid)
        {
            if (string.IsNullOrEmpty(userid)) return NotFound("userid is not exist");
            user = await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound("user is not exist");
            return Page();
        }

        public async Task<IActionResult> OnPostAddClaimAsync(string userid)
        {
            if (string.IsNullOrEmpty(userid)) return NotFound("userid is not exist");
            user = await _userManager.FindByIdAsync(userid);
            if (user == null) return NotFound("user is not exist");

            if (!ModelState.IsValid)
            {
                return Page();
            }

            if (_context.UserClaims.Any(uc => uc.ClaimType == Input.ClaimType && uc.ClaimValue == Input.ClaimValue))
            {
                ModelState.AddModelError(string.Empty, "user claim was exist");
                return Page();
            }

            var result = await _userManager.AddClaimAsync(user, new Claim(Input.ClaimType, Input.ClaimValue));

            if (!result.Succeeded)
            {
                result.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
            }

            StatusMessage = "Add user claim successfully";

            return RedirectToPage("./AddRole", new { id = user.Id });
        }

        public async Task<IActionResult> OnGetEditClaimAsync(int? claimid)
        {
            if (claimid == null) return NotFound("calimid was not exist");
            userclaim = _context.UserClaims.Where(uc => uc.Id == claimid).FirstOrDefault();
            if (userclaim == null) return NotFound("User Claim was not exists");
            user = await _userManager.FindByIdAsync(userclaim.UserId);
            if (user == null) return NotFound("Can not find user");

            Input = new InputModel()
            {
                ClaimType = userclaim.ClaimType,
                ClaimValue = userclaim.ClaimValue
            };
            return Page();
        }

        public async Task<IActionResult> OnPostEditClaimAsync(int? claimid)
        {
            if (claimid == null) return NotFound("calimid was not exist");
            userclaim = _context.UserClaims.Where(uc => uc.Id == claimid).FirstOrDefault();
            if (userclaim == null) return NotFound("User Claim was not exists");
            user = await _userManager.FindByIdAsync(userclaim.UserId);
            if (user == null) return NotFound("Can not find user");

            if (!ModelState.IsValid)
            {
                return Page();
            }
            userclaim.ClaimType = Input.ClaimType;
            userclaim.ClaimValue = Input.ClaimValue;

            await _context.SaveChangesAsync();

            StatusMessage = "Update user claim successfully";

            return RedirectToPage("./AddRole", new { id = userclaim.UserId });
        }

        public async Task<IActionResult> OnGetDeleteClaimAsync(int? claimid)
        {
            if (claimid == null) return NotFound("calimid was not exist");
            userclaim = _context.UserClaims.Where(uc => uc.Id == claimid).FirstOrDefault();
            if (userclaim == null) return NotFound("User Claim was not exists");

            var removeResult = _context.UserClaims.Remove(userclaim);

            if (removeResult == null)
            {
                ModelState.AddModelError(string.Empty, "Can not delete user calaim");
                return Page();
            }

            await _context.SaveChangesAsync();
            StatusMessage = "Delete user claim successfully";

            return RedirectToPage("./AddRole", new { id = userclaim.UserId });
        }
    }
}
