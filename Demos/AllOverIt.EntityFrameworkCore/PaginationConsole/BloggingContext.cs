using Microsoft.EntityFrameworkCore;
using PaginationConsole.Entities;
using System;

namespace PaginationConsole
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            base.OnConfiguring(options);

            if (DemoStartupOptions.Use == DatabaseChoice.Mysql)
            {
                options.UseMySql("server=localhost;user=root;password=password;database=PaginatedBlogPosts", new MySqlServerVersion(new Version(8, 0, 26)));
            }
            else if (DemoStartupOptions.Use == DatabaseChoice.Sqlite)
            {
                options.UseSqlite("Data Source=PaginatedBlogPosts.db");
            }
            else if (DemoStartupOptions.Use == DatabaseChoice.PostgreSql)
            {
                options.UseNpgsql("Host=localhost;Database=PaginatedBlogPosts;Username=postgres;Password=password", options =>
                {
                    options.SetPostgresVersion(new Version(13, 6));
                });
            }
            else
            {
                throw new NotImplementedException($"Unknown database type {DemoStartupOptions.Use}");
            }

            options
                //.LogTo(Console.WriteLine, LogLevel.Information)
                .EnableDetailedErrors();
        }
    }
}