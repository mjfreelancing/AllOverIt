using AllOverIt.EntityFrameworkCore.Extensions;
using KeysetPaginationConsole.Entities;
using KeysetPaginationConsole.Models;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Note: To test a different configuration:
            //
            //  * delete the Migrations folder
            //  * edit setup as required
            //  * run the 'add-migration Init' command
            //  * run the application.

            // options is optional - if not used then all properties will be stored as integers
            modelBuilder.UseEnrichedEnum(options =>
            {
                // By type: Status1, Status2, Status3 as integer - could be left out as this is the default fallback
                options
                    .Entity<Blog>()
                    .Properties(typeof(BlogStatus));

                // By name: Status2 now becomes stored as a string (replaces the previous line for this property)
                options
                    .Entity<Blog>()
                    .Properties(nameof(Blog.Status2))
                    .AsName();

                // By type: Rating and RatingValue as Name
                options
                    .Entity<Post>()
                    .Properties(typeof(PostRating))
                    .AsName();

                // By name: Status as Name, leaving StatusValue as the default integer (since not configured)
                options
                    .Entity<Post>()
                    .Properties(nameof(Post.Status))
                    .AsName();

                /*
                // Specific entity / property / conversion type
                options.Entity<Blog>().Property(typeof(BlogStatus)).AsName();
                options.Entity<Post>().Properties(typeof(PostRating), typeof(PublishedStatus)).AsValue();

                // OR

                // All properties on the given entity type
                options.Entity<Blog>().AsName();
                options.Entity<Post>().AsValue();

                // OR

                // This will iterate over all entities configured and set them all to the requested Name / Value conversion
                // If no entities have been configured then all entities / properties are set
                options.AsName();
                options.AsValue();      // - last one wins
                */
            });
        }
    }
}