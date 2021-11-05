using AllOverIt.EntityFrameworkCore.Extensions;
using EFEnumerationDemo.Entities;
using EFEnumerationDemo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;

namespace EFEnumerationDemo
{
    public class BloggingContext : DbContext
    {
        public DbSet<Blog> Blogs { get; set; }
        public DbSet<Post> Posts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            base.OnConfiguring(options);

            var connectionString = "server=localhost;user=root;password=password;database=BlogPosts";
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 26));

            options
                .UseMySql(connectionString, serverVersion)
                .LogTo(Console.WriteLine, LogLevel.Information)
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

            // This will store ALL enriched enums as strings
            // modelBuilder.UseEnrichedEnumName();

            // This will store all properties of type PublishedStatus on the Post entity as a string.
            // modelBuilder.UseEnrichedEnumName<Post, PublishedStatus>();

            // This will store all properties of type PublishedStatus on the Post entity as an integer.
            // modelBuilder.UseEnrichedEnumValue<Post, PublishedStatus>();


            // All BlogStatus properties across all entities
            // Will automatically store as a string (based on storing the Name)
            modelBuilder.UseEnrichedEnumName<BlogStatus>();

            // Will automatically store as a string (based on storing the Name)
            modelBuilder.UseEnrichedEnumName<Post>(nameof(Post.Rating));
            modelBuilder.UseEnrichedEnumName<Post>(nameof(Post.Status));

            // Will automatically store as an integer (based on storing the Value)
            modelBuilder.UseEnrichedEnumValue<Post>(nameof(Post.RatingValue));
            modelBuilder.UseEnrichedEnumValue<Post>(nameof(Post.StatusValue));
        }
    }
}