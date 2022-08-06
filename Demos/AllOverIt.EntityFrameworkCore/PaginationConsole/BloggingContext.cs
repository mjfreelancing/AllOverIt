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
                    options.SetPostgresVersion(new Version(10, 18));
                    //options.SetPostgresVersion(new Version(13, 6));
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            if (DemoStartupOptions.Use == DatabaseChoice.PostgreSql)
            {
                // NOTE: Non-deterministic collations do not work with LIKE (or ILIKE) so sticking with "citext"

                modelBuilder.HasPostgresExtension("citext");


                // Create a non-deterministic, case-insensitive ICU collation ("ci_collation" is any arbitrary name - also used below)
                // https://unicode-org.github.io/icu/userguide/collation/
                //modelBuilder
                //    .HasCollation("ci_collation", locale: "en-u-ks-primary", provider: "icu", deterministic: false);

                // ??
                // modelBuilder.UseDefaultColumnCollation("ci_collation");
            }

            // Individual column
            //
            //modelBuilder
            //    .Entity<Blog>()
            //    .Property(blog => blog.Description)
            //    .HasColumnType("citext");
        }

        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            // All string columns
            //
            if (DemoStartupOptions.Use == DatabaseChoice.PostgreSql)
            {
                configurationBuilder
                    .Properties<string>()
                    .HaveColumnType("citext");
                    //.UseCollation("ci_collation");
            }
        }
    }
}