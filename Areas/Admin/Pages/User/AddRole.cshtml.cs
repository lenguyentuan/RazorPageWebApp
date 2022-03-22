using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using App.Models;

namespace App.Admin.User
{
    public class AddRoleModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        private readonly AppDbContext _context;
        public AddRoleModel(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager, AppDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }



        [DisplayName("Role List")]
        [BindProperty]
        public string[] RoleNames { set; get; }

        public SelectList AllRoles { set; get; }

        public AppUser user { set; get; }
        [TempData]
        public string StatusMessage { set; get; }

        public List<IdentityRoleClaim<string>> ClaimInRole { set; get; }
        public List<IdentityUserClaim<string>> ClaimInUserRole { set; get; }
        public async Task<IActionResult> OnGetAsync(string id)
        {

            if (string.IsNullOrEmpty(id)) return NotFound($"Can not find user has ID: {id}");
            user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound($"Can not find user has ID: {id}");
            List<string> roles = await _roleManager.Roles.OrderBy(r => r.Name).Select(u => u.Name).ToListAsync();
            AllRoles = new SelectList(roles);

            RoleNames = (await _userManager.GetRolesAsync(user)).ToArray();

            var listRoles = from ur in _context.UserRoles
                            join r in _context.Roles on ur.RoleId equals r.Id
                            where ur.UserId == id
                            select r;

            var claimsRole = from c in _context.RoleClaims
                             join r in listRoles on c.RoleId equals r.Id
                             select c;
            ClaimInRole = claimsRole.ToList();

            var userClaims = from c in _context.UserClaims where c.UserId == id select c;

            ClaimInUserRole = userClaims.ToList();

            return Page();
        }

        public async Task<IActionResult> OnPostAsync(string id)
        {
            if (string.IsNullOrEmpty(id)) return NotFound($"Can not find user has ID: {id}");
            user = await _userManager.FindByIdAsync(id);
            if (user == null) return NotFound($"Can not find user has ID: {id}");

            var OldRoleNames = (await _userManager.GetRolesAsync(user)).ToArray();
            var DeleteRoles = OldRoleNames.Where(r => !RoleNames.Contains(r));
            var AddRoles = RoleNames.Where(r => !OldRoleNames.Contains(r));


            List<string> roles = await _roleManager.Roles.OrderBy(r => r.Name).Select(u => u.Name).ToListAsync();
            AllRoles = new SelectList(roles);
            var resultDelete = await _userManager.RemoveFromRolesAsync(user, DeleteRoles);



            if (!resultDelete.Succeeded)
            {
                resultDelete.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }

            var resultAdd = await _userManager.AddToRolesAsync(user, AddRoles);
            if (!resultAdd.Succeeded)
            {
                resultAdd.Errors.ToList().ForEach(error =>
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                });
                return Page();
            }


            StatusMessage = $"Add Role for user {user.UserName} successfully";
            return RedirectToPage("./Index");
        }
    }
}
