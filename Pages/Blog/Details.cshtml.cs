using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Razor_09_EntityFramework.Models;

namespace Razor_09_EntityFramework.Pages_Blog
{
    public class DetailsModel : PageModel
    {
        private readonly Razor_09_EntityFramework.Models.AppDbContext _context;

        public DetailsModel(Razor_09_EntityFramework.Models.AppDbContext context)
        {
            _context = context;
        }

        public Article Article { get; set; }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Article = await _context.articles.FirstOrDefaultAsync(m => m.id == id);

            if (Article == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
