using System.Security.Claims;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorWebApp.Models;

namespace RazorWebApp.Admin.Role
{
    public class IndexModel : RolePageModel
    {
        public IndexModel(RoleManager<IdentityRole> roleManager, AppDbContext context) : base(roleManager, context)
        {
        }

        public class RoleModel : IdentityRole
        {
            public string[] Claims { set; get; }
        }

        public List<RoleModel> roles { set; get; }
        public async Task OnGet()
        {
            var r = await _roleManager.Roles.ToListAsync();
            roles = new List<RoleModel>();

            foreach (var _r in r)
            {
                var claims = await _roleManager.GetClaimsAsync(_r);
                var ClaimString = claims.Select(c => c.Type + "=" + c.Value);
                var rm = new RoleModel()
                {
                    Name = _r.Name,
                    Id = _r.Id,
                    Claims = ClaimString.ToArray()
                };
                roles.Add(rm);
            }
        }

        public void OnPost() => RedirectToPage();
    }
}

