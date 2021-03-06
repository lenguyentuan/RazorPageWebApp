using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using App.Models;

namespace App.Pages_Blog
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly App.Models.AppDbContext _context;

        public IndexModel(App.Models.AppDbContext context)
        {
            _context = context;
        }

        public List<Article> Article { get; set; }
        public const int ITEMS_PER_PAGE = 10;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { set; get; }
        public int countPages { set; get; }

        public async Task OnPostAsync(string searchString)
        {
            var qr = from a in _context.articles
                     orderby a.created descending
                     select a;
            if (!string.IsNullOrEmpty(searchString))
            {
                Article = await qr.Where(a => a.title.Contains(searchString)).ToListAsync();
            }
            else
            {
                Article = await qr.ToListAsync();
            }
        }
        public async Task OnGetAsync()
        {
            int totalArticle = await _context.articles.CountAsync();
            countPages = (int)Math.Ceiling((double)totalArticle / ITEMS_PER_PAGE);

            if (currentPage < 1) currentPage = 1;
            if (currentPage > countPages) currentPage = countPages;

            var qr = (from a in _context.articles
                      orderby a.created descending
                      select a).Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE);

            Article = await qr.ToListAsync();
        }


    }
}
