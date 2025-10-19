using AllOverIt.EntityFrameworkCore.Diagrams.Tests.TestTypes.Entities;
using Microsoft.EntityFrameworkCore;

namespace AllOverIt.EntityFrameworkCore.Diagrams.Tests.TestTypes
{
    public class TestDbContext : DbContext
    {
        public DbSet<Author> Authors { get; set; }
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<AuthorBlog> AuthorBlogs { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<PostEntity> Posts { get; set; }
        public DbSet<WebSite> WebSites { get; set; }
        public DbSet<Settings> WebSiteSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite("Filename=:memory:");
        }
    }
}