using KeysetPaginationConsole.Entities;
using Microsoft.EntityFrameworkCore;

namespace KeysetPaginationConsole
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            base.OnConfiguring(options);

            var connectionString =
                // MySql
                //"server=localhost;user=root;password=password;database=PaginatedBlogPosts";

                // Sqlite
                "Data Source=PaginatedBlogPosts.db";            

            options
                // MySql
                //.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 26)))

                // Sqlite
                .UseSqlite(connectionString)

                //.LogTo(Console.WriteLine, LogLevel.Information)
                .EnableDetailedErrors();
        }
    }
}