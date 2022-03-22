using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using App.Models;

namespace App.Admin.User
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public IndexModel(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public class UserAndRole : AppUser
        {
            public string RoleNames { set; get; }
        }
        public int countPages { set; get; }
        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { set; get; }

        private const int ITEMS_PER_PAGE = 10;

        public List<UserAndRole> users { set; get; }
        [TempData]
        public string StatusMessage { set; get; }


        public async Task OnGet()
        {
            var qr = _userManager.Users.OrderBy(u => u.UserName);
            int totalUsers = await qr.CountAsync();
            countPages = (int)Math.Ceiling((double)totalUsers / ITEMS_PER_PAGE);

            if (currentPage < 1) currentPage = 1;
            if (currentPage > countPages) currentPage = countPages;

            var qr1 = qr.Skip((currentPage - 1) * ITEMS_PER_PAGE)
                        .Take(ITEMS_PER_PAGE)
                        .Select(u => new UserAndRole()
                        {
                            Id = u.Id,
                            UserName = u.UserName
                        });

            users = await qr1.ToListAsync();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                user.RoleNames = string.Join(",", roles);
            }
        }

        public IActionResult OnPost() => RedirectToPage("./Index");

    }
}
