using KeysetPaginationConsole.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace KeysetPaginationConsole
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            base.OnConfiguring(options);

            if (DatabaseStartupOptions.Use == DatabaseChoice.Mysql)
            {
                options.UseMySql("server=localhost;user=root;password=password;database=PaginatedBlogPosts", new MySqlServerVersion(new Version(8, 0, 26)));
            }
            else if (DatabaseStartupOptions.Use == DatabaseChoice.Sqlite)
            {
                options.UseSqlite("Data Source=PaginatedBlogPosts.db");
            }
            else
            {
                throw new NotImplementedException($"Unknown database type {DatabaseStartupOptions.Use}");
            }

            options
                //.LogTo(Console.WriteLine, LogLevel.Information)
                .EnableDetailedErrors();
        }
    }
}