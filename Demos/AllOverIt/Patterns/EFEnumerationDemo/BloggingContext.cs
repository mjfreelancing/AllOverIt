using AllOverIt.EntityFrameworkCore.Extensions;
using EFEnumerationDemo.Entities;
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

            // Note: The migration code provided with this demo is using string values. To test with integers:
            //
            //  * delete the Migrations folder
            //  * swap the `UseEnrichedEnumXXX` lines below
            //  * run the 'add-migration Init' command
            //  * run the application.

            // use this to store the PostRating as a string - the 'longtext' is the type of string on the relational database
            modelBuilder.UseEnrichedEnumName("longtext");

            // use this to store the PostRating as an integer - the 'int' is the type of string on the relational database
            //modelBuilder.UseEnrichedEnumName("int");
        }
    }
}