using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RazorWebApp.Models;

namespace RazorWebApp.Pages_Blog
{
    public class EditModel : PageModel
    {
        private readonly RazorWebApp.Models.AppDbContext _context;
        private readonly IAuthorizationService _authorizationService;

        public EditModel(RazorWebApp.Models.AppDbContext context, IAuthorizationService authorizationService)
        {
            _authorizationService = authorizationService;
            _context = context;
        }

        [BindProperty]
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

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Article).State = EntityState.Modified;

            try
            {
                var canupdate = await _authorizationService.AuthorizeAsync(this.User, Article, "CanUpdateArticle");
                Console.WriteLine(canupdate.Succeeded);
                if (canupdate.Succeeded)
                {
                    await _context.SaveChangesAsync();
                }
                else
                {
                    return Content("Can not update article");
                }

            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(Article.id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index");
        }

        private bool ArticleExists(int id)
        {
            return _context.articles.Any(e => e.id == id);
        }
    }
}
