using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RazorWebApp.Models;

namespace RazorWebApp.Pages_Blog
{
    public class IndexModel : PageModel
    {
        private readonly RazorWebApp.Models.AppDbContext _context;

        public IndexModel(RazorWebApp.Models.AppDbContext context)
        {
            _context = context;
        }

        public IList<Article> Article { get; set; }

        public const int ITEMS_PER_PAGE = 10;

        [BindProperty(SupportsGet = true, Name = "p")]
        public int currentPage { set; get; }
        public int countPages { set; get; }

        public async Task OnGetAsync(string SearchString)
        {
            int totalArticle = await _context.articles.CountAsync();
            countPages = (int)Math.Ceiling((double)totalArticle / ITEMS_PER_PAGE);
            if (currentPage < 1) currentPage = 1;
            if (currentPage > countPages) currentPage = countPages;

            var qr = (from a in _context.articles
                      orderby a.created descending
                      select a).Skip((currentPage - 1) * ITEMS_PER_PAGE).Take(ITEMS_PER_PAGE);
            if (!string.IsNullOrEmpty(SearchString))
            {
                Article = await qr.Where(a => a.title.Contains(SearchString)).ToListAsync();
            }
            else
            {
                Article = await qr.ToListAsync();
            }

        }
    }
}
