using System.Data;
using Microsoft.EntityFrameworkCore;

/*
dotnet aspnet-codegenerator razorpage -m Razor_09_EntityFramework.Models.Article -dc Razor_09_EntityFramework.Models.AppDbContext -outDir Pages/Blog -udl --referenceScriptLibraries
*/
namespace Razor_09_EntityFramework.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Article> articles { set; get; }
    }
}