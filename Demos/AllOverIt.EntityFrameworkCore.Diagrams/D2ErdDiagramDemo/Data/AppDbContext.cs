using D2ErdDiagramDemo.Data.Entities;
using Microsoft.EntityFrameworkCore;

namespace D2ErdDiagramDemo.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<AuthorBlog> AuthorBlogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<WebSite> WebSites { get; set; }
        public DbSet<Settings> WebSiteSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite("Filename=:memory:");
        }
    }
}