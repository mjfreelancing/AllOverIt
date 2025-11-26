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
        public DbSet<WebSiteEntity> WebSites { get; set; }
        public DbSet<Settings> WebSiteSettings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            optionsBuilder.UseSqlite("Filename=:memory:");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Sets up the many-to-many UserRole join table without an explicit model
            modelBuilder
                .Entity<Author>()
                .HasMany(user => user.Roles)
                .WithMany(role => role.Authors)
                .UsingEntity("AuthorRole");

            // Sets up the many-to-many RolePermission join table without an explicit model
            modelBuilder
                .Entity<Role>()
                .HasMany(role => role.Permissions)
                .WithMany(permission => permission.Roles)
                .UsingEntity("RolePermission");
        }
    }
}