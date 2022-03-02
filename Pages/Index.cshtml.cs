using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Razor_09_EntityFramework.Models;

namespace Razor_09_EntityFramework.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;

        private readonly AppDbContext dbContext;

        public Article article { set; get; }

        public IndexModel(ILogger<IndexModel> logger, AppDbContext _dbContext)
        {
            _logger = logger;
            dbContext = _dbContext;
        }

        public void OnGet()
        {
            List<Article> articles = (from a in dbContext.articles select a).ToList();
            ViewData["articles"] = articles;
        }
    }
}
